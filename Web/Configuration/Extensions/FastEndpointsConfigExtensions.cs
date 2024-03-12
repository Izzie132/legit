using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Web.Infrastructure.Exceptions;

namespace Web.Configuration.Extensions;

public static class FastEndpointsConfigExtensions
{
    public static void ConfigureFastEndpoints(this WebApplication app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Serializer.Options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
            {
                return new ApiErrorResponse(
                    message: "",
                    stackTrace: "",
                    userVisibleMessage: "One or more validation errors occurred:"
                        + string.Join(
                            string.Empty,
                            failures.Select(failure =>
                                $"{Environment.NewLine}- {failure.PropertyName}: {failure.ErrorMessage}"
                            )
                        )
                );
            };
            c.Errors.ProducesMetadataType = typeof(ApiErrorResponse);

            c.Endpoints.Configurator = endpoints =>
            {
                endpoints.Description(b => b.WithName(endpoints.EndpointType.DeclaringType!.Name));
                endpoints.Description(b => b.Produces<ApiErrorResponse>(500, "application/problem+json"));
            };
        });
    }
}
