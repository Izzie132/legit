# NUKE Build

[NUKE Build](https://nuke.build/) is a build automation library that can be used to automate common processes using 
C# / .NET. This project relies heavily on NUKE Build from automating developer setup, to scripting CI/CD pipelines in a 
developer friendly language.

"Tasks" are defined within the `Build.cs` and can be chained together to build up larger and more complex tasks. This 
file can be found in the `Tools/Build` project.

There are many tasks available in this project, many of which are dependent on each-other. Some of the main are listed below:

| Task Name                   | Description                                                                                                                                                                                                                         |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BuildAndTest                | Run the test and code quality checks on both the backend and the frontend. Should be run in the PR pipeline.                                                                                                                        |
| Publish                     | Run the BuildAndTest task, followed by publishing the Web and Migration projects to the `build-output` directory. Should be used in deployment pipelines where a build artifact is required.                                        |
| ResetDevelopmentDatabase    | Cleans and migrates the development database within the Docker SQL Server                                                                                                                                                           |
| SetupDevelopmentEnvironment | Restores packages on the frontend and backend. Creates the Docker SQL Server container, creates the development and test databases with corresponding logins, and runs migrations against both. Used for initial development setup. |

To see a full list of tasks, and their dependencies, run `build.ps1 --help`. You can also see an interactive map of the
tasks and their dependencies using `build.ps1 --plan`.

To run these tasks, run `build.ps1 <TASK_NAME>` from within the root of the project. For example, to run the `BuildAndTest` task, run `build.ps1 BuildAndTest`.
If you are not using PowerShell, you can also use `build.cmd` or `build.sh` as a substitute for `build.ps1`.

You can also run these by calling `nuke <TASK_NAME>` from anywhere within the project by installing the [NUKE Global Tool](https://www.nuget.org/packages/Nuke.GlobalTool)
by running `dotnet tool install Nuke.GlobalTool --global`.
