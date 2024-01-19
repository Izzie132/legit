# Backend

The backend of this application uses a .NET 8 Web API project. It is configured to use the following core libraries:

- [FastEndpoints](https://fast-endpoints.com/) for endpoint mapping.
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) for database access.
- [CSharpier](https://csharpier.com/) for code formatting.
- [xUnit](https://xunit.net/) for testing.

This project is setup in a [vertical slices architecture](https://www.ghyston.com/insights/architecting-for-maintainability-through-vertical-slices),
where each API endpoint will have a single file within the `Features` folder structure, containing the endpoint logic,
request and response models, validation, and any other required classes. Where possible, the endpoint logic will be
contained within a single file, however, if there is a lot of shared logic, it might make sense to split this out into
a service class.
