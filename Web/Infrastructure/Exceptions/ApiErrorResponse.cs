namespace Web.Exceptions;

public class ApiErrorResponse
{
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? UserVisibleMessage { get; set; }

    public ApiErrorResponse(string? message, string? stackTrace, string? userVisibleMessage)
    {
        Message = message;
        StackTrace = stackTrace;
        UserVisibleMessage = userVisibleMessage;
    }
}
