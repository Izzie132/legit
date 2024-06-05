# API Client Generation

This project [NSwag](https://github.com/RicoSuter/NSwag) to automatically generate a Typescript API client that defines
methods for each endpoint in defined using FastEndpoints. This is designed to reduce the workload of the developer, who
now can define an API endpoint in the backend, and then start using it straight away in the frontend

## NSwag

[NSwag](https://github.com/RicoSuter/NSwag) is a Swagger/OpenAPI 2.0 and 3.0 toolchain for .NET, that allows for the
generation of of OpenAPI specifications from existing ASP.NET Web API controllers and client code from these OpenAPI
specifications.

In this implementation, we are using the generated Swagger/Open API document as an input to the NSwag toolchain. This
is then used to generate the Typescript API client.

For NSwag to correctly generate the Typescript API client, it requires that the Swagger definition of each endpoint is
accurate. More information about configuring this can be found within the [Swagger documentation](./Swagger.md).

### Running NSwag

Currently, NSwag is run as a build step within the `Web` project by method of a Target that runs after the core build.

This is supported by an optional argument that can be passed to the application. When running the `Web` project with
the `--generateclients true` flag, the application will start up, generate the defined API clients, and then exit.

This will output the generated `ApiClient.ts` file within the `Web/client-app/src/api` directory.

### Customising NSwag

NSwag can be customised to generate API clients in line with your needs for a project. Some configuration has been
included in this project to structure these generated files in a way that matches our usual working practices.

You can customise both the Swagger document generation, as well as the Typescript client generator. This configuration
can be found in `Web\Configuration\Extensions\OpenApiConfigExtensions.cs`.

If more control is needed than can be configured through these settings, you can create custom Liquid templates for the
Typescript generator, as defined in [the documentation](https://github.com/RicoSuter/NSwag/wiki/Templates). An example
in this project is the custom `Web/client-app/src/api/templates/File.Header.liquid` file used to remove TSLint ignores.
