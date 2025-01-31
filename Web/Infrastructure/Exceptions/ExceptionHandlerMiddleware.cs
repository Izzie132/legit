using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Infrastructure.Extensions;

namespace Web.Infrastructure.Exceptions;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger,
    IOptions<ExceptionOptions> exceptionOptions
)
{
    private readonly ExceptionOptions exceptionOptions = exceptionOptions.Value;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var demystifiedException = exception.Demystify();

            LogException(demystifiedException);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await HandleException(context, demystifiedException);
        }
    }

    private static HttpStatusCode GetStatusCode(Exception exception) =>
        exception switch
        {
            UserVisibleValidationException => HttpStatusCode.BadRequest,
            UserVisibleNotFoundException => HttpStatusCode.NotFound,

            _ => HttpStatusCode.InternalServerError,
        };

    private async Task HandleException(HttpContext context, Exception exception)
    {
        var errorResponse = new ApiErrorResponse(
            message: exceptionOptions.ExposeExceptionDetails ? exception.Message : null,
            stackTrace: exceptionOptions.ExposeExceptionDetails ? exception.StackTrace : null,
            userVisibleMessage: exception is UserVisibleException userVisibleException
                ? userVisibleException.UserVisibleMessage
                : null
        );

        await context.WriteJsonResponseAsync(statusCode: GetStatusCode(exception), responseBody: errorResponse);
    }

    private void LogException(Exception exception)
    {
        // Validation/Not Found exceptions should be treated as warnings, not errors.
        switch (exception)
        {
            case UserVisibleValidationException:
                logger.LogWarning(exception, "A user visible validation error occurred");
                break;
            case UserVisibleException:
                logger.LogError(exception, "A user visible error occurred");
                break;
            default:
                logger.LogError(exception, "An unexpected error occurred");
                break;
        }
    }
}
