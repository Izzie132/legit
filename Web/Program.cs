using Azure.Identity;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Web.Configuration;
using Web.Database;
using Web.Infrastructure.Exceptions;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = Environment.GetEnvironmentVariable("KEYVAULT_URI");
if (keyVaultUri != null)
{
    builder.Configuration.AddAzureKeyVault(vaultUri: new Uri(keyVaultUri), credential: new DefaultAzureCredential());
}

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<ProjectNameOptions>(builder.Configuration.GetSection(key: ProjectNameOptions.ConfigurationKey));

builder.Services.Configure<ExceptionOptions>(builder.Configuration.GetSection(key: ExceptionOptions.ConfigurationKey));

builder.Services.AddDbContext<DataContext>();

builder.Services.AddScoped<IClockService, ClockService>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSpaStaticFiles(spaStaticFiles =>
    {
        spaStaticFiles.RootPath = "client-app/dist";
    });
}

builder.Services.AddFastEndpoints();

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
                    failures.Select(failure => $"{Environment.NewLine}- {failure.PropertyName}: {failure.ErrorMessage}")
                )
        );
    };
});

if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();

namespace Web
{
    public partial class Program { }
}
