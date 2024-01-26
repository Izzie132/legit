using Azure.Identity;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Web.Database;
using Web.Infrastructure.Exceptions;

namespace Web.Configuration.Extensions;

public static class WebApplicationConfigurationExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var keyVaultUri = Environment.GetEnvironmentVariable("KEYVAULT_URI");
        if (keyVaultUri != null)
        {
            builder.Configuration.AddAzureKeyVault(vaultUri: new Uri(keyVaultUri), credential: new DefaultAzureCredential());
        }

        services.AddApplicationInsightsTelemetry();

        services.ConfigureOptions(builder.Configuration);

        services.AddDbContext<DataContext>();

        services.ConfigureNodaTime();

        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddSpaStaticFiles(spaStaticFiles =>
            {
                spaStaticFiles.RootPath = "client-app/dist";
            });
        }

        builder.Services.AddFastEndpoints();
    }

    public static WebApplication ConfigureApp(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseStaticFiles();

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
        });

        if (!app.Environment.IsDevelopment())
        {
            app.MapFallbackToFile("index.html");
        }

        return app;
    }
}
