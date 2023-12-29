# Zero to Hero

## Pre-requisites
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js 20](https://nodejs.org/en)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## First Time Setup

- Clone the repository to your local machine.
- Open a terminal and navigate to the root of the project.
- Run `build.ps1 SetupDevEnvironment` to setup the development environment. This will:
    - Install required NuGet and NPM packages for the frontend and backend.
    - Create and setup a docker container running an instance of SQL Server.