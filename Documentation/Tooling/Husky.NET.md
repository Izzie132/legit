# Husky.NET

To run both frontend and backend formatting on commit, this project used Husky.NET.

## Installation

The required tools will be installed by either the `SetupDevelopmentEnvironment` or `RestoreDotNetTools` NUKE tasks,
but the tools can be installed manually using the following commands:

```powershell
dotnet tool install Husky
dotnet husky install
```

## Configuration

Within this project there is a single `pre-commit` hook configured that will run all of the tasks defined within
`task-runner.json`. These files can be found within the `.husky` folder.

To configure new tasks to run on commit, you can add them to the `task-runner.json` file using the documentation that
can be found [here](https://alirezanet.github.io/Husky.Net/guide/task-configuration.html).
