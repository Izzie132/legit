using Azure.Identity;
using FastEndpoints.Swagger;
using Web.Configuration.Extensions;
using Web.Database;
using Web.Infrastructure.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();
var app = await builder.ConfigureApp();
app.Run();

public static class WebApplicationBuilderExtensions
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
            services.AddSpaStaticFiles(spaStaticFiles =>
            {
                spaStaticFiles.RootPath = "client-app/dist";
            });
        }

        services.AddFastEndpoints();
        services.ConfigureSwaggerDocument();
    }

    public static async Task<WebApplication> ConfigureApp(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseStaticFiles();

        app.ConfigureFastEndpoints();

        app.UseSwaggerGen();

        await app.GenerateTypescriptApiClientAndExitAsync();

        if (!app.Environment.IsDevelopment())
        {
            app.MapFallbackToFile("index.html");
        }

        return app;
    }
}

// Needed to make the `Program` class available to the test projects.
public partial class Program { }
