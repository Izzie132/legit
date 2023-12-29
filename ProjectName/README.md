# Project Name

## Description
This is a template project created using .NET and React.

## Developer Setup
### Pre-requisites
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js 20](https://nodejs.org/en)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### First Time Setup

- Clone the repository to your local machine.
- Open a terminal and navigate to the root of the project.
- Run `build.ps1 SetupDevEnvironment` to setup the development environment. This will:
  - Install required NuGet and NPM packages for the frontend and backend.
  - Create and setup a docker container running an instance of SQL Server.

### Running the Project
Either run the `Web` project from within your IDE, or run `dotnet run watch` from within the `.\Web` directory.
This will start both the backend and the frontend, with requests from the frontend being proxied to the backend.

## Technical Details
### .NET Backend
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

### React Frontend
The frontend of this application is a Single Page App (SPA) built using React. It is configured to use the following core:
- [React](https://react.dev/) as the frontend framework.
- [Tailwind CSS](https://tailwindcss.com/) for styling.
- [React Router](https://reactrouter.com/) for routing.
- [shadcn/ui](https://ui.shadcn.com/) for common components.
- [Axios](https://axios-http.com/) for making HTTP requests.
- [React Hook Form](https://react-hook-form.com/) and [Zod](https://zod.dev/) for forms and validation.
- [Vite.js](https://vitejs.dev/) for frontend tooling.
- [Prettier](https://prettier.io/) and [ESLint](https://eslint.org/) for code formatting and linting.
- [Vitest](https://vitest.dev/) and [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/) for testing.

Similar to the backend, the frontend is setup in a vertical slices architecture, where each page will have a single file.
This file will contain the page logic, any required components, and any required styles. Where possible, the page logic
will be contained within a single file, however, if there is a lot of shared logic, it might make sense to split this out
into shared components or helper functions.

### SQL Server Database
The backend of this application integrates with a [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads)
database. 

In development, this database is run in a docker container on port 1407. It will have 2 separate databases setup as described below.

| Database Name   | User            | Password            | Description                                                   |
|-----------------|-----------------|---------------------|---------------------------------------------------------------|
| ProjectName     | ProjectName     | DefinitelyDurable1! | Used for the main .NET backend while running the application. |
| ProjectNameTest | ProjectNameTest | TotallyTrusted2!    | Used by the test project when running integration tests.      |

When connecting to the SQL Server, you can either use the above users, or connect using the admin user details:
- User: `SA`
- Password: `SuperSecure0!`

### NUKE Build
[NUKE Build](https://nuke.build/) is used for building and testing the application for CI/CD, but can also be used 
locally for performing common tasks. There are many tasks available, many of which are dependent on each-other. Some of 
the main are listed below:

| Task Name                   | Description                                                                                                                                                                                                                         |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BuildAndTest                | Run the test and code quality checks on both the backend and the frontend. Should be run in the PR pipeline.                                                                                                                        |
| Publish                     | Run the BuildAndTest task, followed by publishing the Web and Migration projects to the `build-output` directory. Should be used in deployment pipelines where a build artifact is required.                                        |
| ResetDevelopmentDatabase    | Cleans and migrates the development database within the Docker SQL Server                                                                                                                                                           |
| SetupDevelopmentEnvironment | Restores packages on the frontend and backend. Creates the Docker SQL Server container, creates the development and test databases with corresponding logins, and runs migrations against both. Used for initial development setup. |

To see a full list of tasks, and their dependencies, run `build.ps1 --help`. You can also see an intreactive map of the
tasks and their dependencies using `build.ps1 --plan`.

To run these tasks, run `build.ps1 <task name>` from within the root of the project. For example, to run the 
`BuildAndTest` task, run `build.ps1 BuildAndTest`.

You can also run these by calling `nuke <task name>` from anywhere within the project by installing the [NUKE Global Tool](https://www.nuget.org/packages/Nuke.GlobalTool).


