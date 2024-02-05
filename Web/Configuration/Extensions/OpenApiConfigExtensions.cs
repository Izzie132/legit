using System.Reflection;
using FastEndpoints.ClientGen;
using FastEndpoints.Swagger;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.TypeScript;

namespace Web.Configuration.Extensions;

public static class OpenApiConfigExtensions
{
    public static void ConfigureSwaggerDocument(this IServiceCollection serviceCollection)
    {
        serviceCollection.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.DocumentName = "v1";
                s.Title = "Project Name";
                s.Version = "v1";

                s.MarkNonNullablePropsAsRequired();

                s.SchemaSettings.SchemaNameGenerator = new CustomSchemaNameGenerator();
            };
        });
    }

    public static async Task GenerateTypescriptApiClientAndExitAsync(this WebApplication app)
    {
        await app.GenerateClientsAndExitAsync(
            documentName: "v1",
            destinationPath: "client-app/src/api",
            csSettings: null,
            tsSettings: c =>
            {
                c.Template = TypeScriptTemplate.Fetch;
                c.PromiseType = PromiseType.Promise;

                c.UseAbortSignal = true;
                c.WrapDtoExceptions = true;
                c.GenerateClientInterfaces = true;

                c.TypeScriptGeneratorSettings.TypeStyle = TypeScriptTypeStyle.Interface;
                c.TypeScriptGeneratorSettings.Namespace = "";
                c.TypeScriptGeneratorSettings.GenerateTypeCheckFunctions = true;
                c.TypeScriptGeneratorSettings.TemplateDirectory = "client-app/src/api/templates";
            }
        );
    }

    private class CustomSchemaNameGenerator : ISchemaNameGenerator
    {
        public string Generate(Type type)
        {
            return GetName(type);
        }

        private static string GetName(MemberInfo x)
        {
            return x.DeclaringType is null ? x.Name : $"{GetName(x.DeclaringType)}{x.Name}";
        }
    }
}
