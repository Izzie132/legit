# Zero to Hero

## Pre-requisites

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js 20](https://nodejs.org/en)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## First Time Setup

- Clone the repository to your local machine.
- Open a terminal and navigate to the root of the project.
- Run `./build.ps1 SetupDevelopmentEnvironment` to setup the development environment. This will:
  - Install required NuGet and NPM packages for the frontend and backend.
  - Create and setup a docker container running an instance of SQL Server.

## Running the Application

Once you have completed the above steps, you can either:

- Start the application by running the `Run Web App` run configuration in Rider
- Run the backend and frontend independently by running the following commands in separate terminals:
  - `dotnet run watch` to start the backend
  - `npm run dev` to start the frontend

This will start the ASP.NET backend running on port `5000`, and the React/Vite frontend running on port `3000`.
Requests to `/api` on the frontend will be automatically proxied to the backend, so the application should be accessed
from https://localhost:3000.
