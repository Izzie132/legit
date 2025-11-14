using System.Reflection;
using FakeItEasy.Sdk;
using FastEndpoints.ClientGen;
using FastEndpoints.Swagger;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.TypeScript;

namespace Web.Configuration.Extensions;

public static class OpenApiConfigExtensions
{
    public static bool ShouldGenerateClients(this WebApplicationBuilder builder)
    {
        return builder.Configuration["generateclients"] == "true";
    }

    public static void ConfigureSwaggerDocument(this IServiceCollection serviceCollection)
    {
        serviceCollection.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.DocumentName = "v1";
                s.Title = "QQ Project Name";
                s.Version = "v1";

                s.MarkNonNullablePropsAsRequired();

                s.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                s.SchemaSettings.SchemaNameGenerator = new CustomSchemaNameGenerator();
            };
        });
    }

    public static async Task GenerateTypescriptApiClientAndExitAsync(this WebApplicationBuilder builder)
    {
        /* Replace any services with a mock that are not required for client generation and will cause errors on initialization
        e.g. ReplaceWithMock<IExampleService>();*/

        var app = builder.Build();
        app.ConfigureFastEndpoints();
        app.UseSwaggerGen();

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

                c.TypeScriptGeneratorSettings.TypeStyle = TypeScriptTypeStyle.Interface;
                c.TypeScriptGeneratorSettings.Namespace = "";
                c.TypeScriptGeneratorSettings.GenerateTypeCheckFunctions = true;
                c.TypeScriptGeneratorSettings.TemplateDirectory = "client-app/src/api/templates";
                c.TypeScriptGeneratorSettings.DateTimeType = TypeScriptDateTimeType.String;
                c.TypeScriptGeneratorSettings.EnumStyle = TypeScriptEnumStyle.StringLiteral;
            }
        );

        return;

#pragma warning disable CS8321 // Local function is declared but never used - remove when used
        void ReplaceWithMock<T>()
#pragma warning restore CS8321 // Local function is declared but never used
        {
            var descriptorsToReplace = builder
                .Services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(T))
                .ToList();

            foreach (var descriptor in descriptorsToReplace)
            {
                var mock = Create.Fake(descriptor.ServiceType);
                builder.Services.Remove(descriptor);
                builder.Services.Add(ServiceDescriptor.Singleton(descriptor.ServiceType, mock));
            }
        }
    }

    private sealed class CustomSchemaNameGenerator : ISchemaNameGenerator
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
