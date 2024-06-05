# Swagger

This project uses Swagger both for a development tool, allowing easy documentation of the API, but also to generate API
clients as described in the [API Client Generation](API-Client-Generation.md) documentation. For this reason, the
Swagger descriptions must be kept accurate.

## Default Swagger Documentation

By default, Swagger will automatically detect and produce metadata for endpoints that are created within this application.
The metadata produced by default includes:

- Accepts Metadata
  - **GET/HEAD/DELETE** endpoints will by default accept `*/*` and `application/json` content types.
  - **POST/PUT/PATCH** by default only accepts `application/json` content type.
- Produces Metadata
  - **200 - Success** "produces metadata" is added to all endpoints.
  - **400 - Bad Request** is added if there's a Validator associated with the endpoint.
  - **401 - Unauthorized** is added if the endpoint is not accessible anonymously.
  - **403 - Forbidden** is added if any claims/roles/permissions/policies are required by the endpoint.

## Manual Swagger Documentation

If the default Swagger metadata does not fully document an endpoint, we can manually configure metadata within our
endpoints `Configure()` method. An example from the FastEndpoints documentation is included below, but you can read the
[full documentation](https://fast-endpoints.com/docs/swagger-support#describe-endpoints) for more information on
configuring Swagger metadata.

```csharp
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        Post("/item/create");
        Description(b => b
            .ProducesProblemDetails(400, "application/json+problem") //if using RFC errors
            .ProducesProblemFE<InternalErrorResponse>(500)); //if using FE exception handler
    }
}
```
