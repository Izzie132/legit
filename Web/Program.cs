using Azure.Identity;
using FastEndpoints.Swagger;
using Web.Configuration.Extensions;
using Web.Database;
using Web.Infrastructure.Exceptions;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);

if (builder.ShouldGenerateClients())
{
    await builder.GenerateTypescriptApiClientAndExitAsync();
}

var app = ConfigureApp(builder);
await app.RunAsync();

static void ConfigureServices(WebApplicationBuilder builder)
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
        services.AddSpaStaticFiles(spaStaticFiles =>
        {
            spaStaticFiles.RootPath = "client-app/dist";
        });
    }

    services.AddCors();
    services.AddFastEndpoints();
    services.ConfigureSwaggerDocument();
}

static WebApplication ConfigureApp(WebApplicationBuilder builder)
{
    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseStaticFiles();

    app.ConfigureFastEndpoints();

    app.UseSwaggerGen();

    // ToDo isd - check whether this is right
    app.UseCors(builder => builder.WithOrigins("http://localhost:8081").AllowAnyMethod().AllowAnyHeader());

    if (!app.Environment.IsDevelopment())
    {
        app.MapFallbackToFile("index.html");
    }

    return app;
}

// Needed to make the `Program` class available to the test projects.
public partial class Program;
