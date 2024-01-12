global using FastEndpoints;
using Azure.Identity;
using Web.Configuration;
using Web.Database;
using Web.Exceptions;
using Web.Infrastructure.Exceptions;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = Environment.GetEnvironmentVariable("KEYVAULT_URI");
if (keyVaultUri != null)
{
    builder.Configuration.AddAzureKeyVault(vaultUri: new Uri(keyVaultUri), credential: new DefaultAzureCredential());
}

builder.Services.Configure<ProjectNameOptions>(builder.Configuration.GetSection(key: ProjectNameOptions.ConfigurationKey));

builder.Services.Configure<ExceptionOptions>(builder.Configuration.GetSection(key: ExceptionOptions.ConfigurationKey));

builder.Services.AddDbContext<DataContext>();

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

public partial class Program { }
