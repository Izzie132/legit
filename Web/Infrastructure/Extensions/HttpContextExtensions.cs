using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Web.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    public static async Task WriteJsonResponseAsync<T>(
        this HttpContext context,
        HttpStatusCode statusCode,
        T responseBody
    )
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync(Serialize(responseBody));
    }

    private static string Serialize<T>(T responseBody) =>
        JsonSerializer.Serialize(
            value: responseBody,
            options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            }
        );
}
