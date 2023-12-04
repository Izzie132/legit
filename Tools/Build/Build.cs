using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.PowerShell;
using Serilog;
using static Nuke.Common.EnvironmentInfo;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.CompileSolution);

    [Solution]
    readonly Solution Solution;

    readonly Configuration Configuration = Configuration.Release;

    static string ProjectName = "ProjectName";

    private static AbsolutePath WebProjectDirectory => RootDirectory / "Web";

    private static AbsolutePath ReactClientDirectory => WebProjectDirectory / "client-app";
    AbsolutePath MigrationsDirectory => RootDirectory / "Tools" / "Migrations";
    AbsolutePath MigrationsDllFile =>
        MigrationsDirectory / $"bin/{Configuration}/net8.0/Migrations.dll";

    Target CleanSolution =>
        _ =>
            _.Executes(() =>
            {
                DotNetTasks.DotNetClean();
            });
    Target RestoreSolution =>
        _ =>
            _.DependsOn(CleanSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNetRestore();
                });

    Target RestoreFrontEnd =>
        _ =>
            _.Executes(() =>
            {
                NpmTasks.Npm("install", ReactClientDirectory);
            });

    Target CompileSolution =>
        _ =>
            _.DependsOn(RestoreSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNetBuild(
                        s =>
                            s.SetProjectFile(Solution)
                                .SetConfiguration(Configuration)
                                .EnableNoRestore()
                    );
                });

    Target BuildAndTest =>
        _ =>
            _.DependsOn(
                CompileSolution,
                CheckFrontEndCompiles,
                CheckFrontEndCodeQuality,
                RunFrontEndTests,
                CheckBackEndCodeQuality,
                RunBackendTests
            );

    #region Tests

    Target CheckFrontEndCompiles =>
        _ =>
            _.DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run typecheck", ReactClientDirectory);
                });

    Target CheckFrontEndCodeQuality =>
        _ =>
            _.DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run lint:ci", ReactClientDirectory);
                    NpmTasks.Npm("run prettier:ci", ReactClientDirectory);
                });

    Target RunFrontEndTests =>
        _ =>
            _.DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run test:ci", ReactClientDirectory);
                });

    Target CheckBackEndCodeQuality =>
        _ =>
            _.Executes(() =>
            {
                DotNetTasks.DotNetToolRestore();
                DotNetTasks.DotNet("csharpier --check .");
            });

    Target RunBackendTests =>
        _ =>
            _.DependsOn(CompileSolution, ResetTestDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNetTest(
                        s =>
                            s.SetProjectFile(Solution)
                                .SetConfiguration(Configuration)
                                .EnableNoRestore()
                    );
                });

    #endregion

    #region Databases

    static readonly int DatabasePort = 1407;
    static string DatabaseServer => $"localhost,{DatabasePort}";
    static readonly string DatabaseServerAdminPassword = "SuperSecure0!";

    static readonly string DevelopmentDatabaseName = ProjectName;
    static readonly string TestDatabaseName = $"{ProjectName}Test";

    Target CreateDatabaseDockerContainer =>
        _ =>
            _.Executes(() =>
            {
                DockerTasks.DockerCreate(
                    c =>
                        c.SetImage("mcr.microsoft.com/mssql/server:2022-latest")
                            .SetName(ProjectName)
                            .SetEnv("ACCEPT_EULA=Y", $"SA_PASSWORD={DatabaseServerAdminPassword}")
                            .SetPublish($"{DatabasePort}:1433")
                );

                DockerTasks.DockerStart(c => c.SetContainers(ProjectName));
            });

    Target DestroyDatabaseDockerContainer =>
        _ =>
            _.Executes(() =>
            {
                var list = DockerTasks.DockerContainerLs(c => c.EnableAll());

                if (list.Any(output => output.Text.Contains(ProjectName)))
                {
                    Log.Debug($"{ProjectName} found... Deleting container");
                    DockerTasks.DockerStop(c => c.SetContainers(ProjectName));
                    DockerTasks.DockerRm(c => c.SetContainers(ProjectName));
                }
                else
                {
                    Log.Debug($"{ProjectName} not found.");
                }
            });

    Target EnsureDatabaseContainerResponsive =>
        _ =>
            _.Executes(async () =>
            {
                var databaseHelper = new DatabaseHelper(
                    DatabaseServer,
                    DatabaseServerAdminPassword
                );

                await databaseHelper.WaitForSqlServerResponse();
            });

    Target CreateDevelopmentDatabase =>
        _ =>
            _.DependsOn(EnsureDatabaseContainerResponsive)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(
                        DatabaseServer,
                        DatabaseServerAdminPassword
                    );

                    var appConnectionString =
                        await databaseHelper.CreateDatabaseAndGetConnectionString(
                            DevelopmentDatabaseName,
                            DevelopmentDatabaseName,
                            "SuperSecure0!"
                        );
                    Environment.SetEnvironmentVariable(
                        DevelopmentDatabaseName.ToConnectionStringEnvironmentVariableName(),
                        appConnectionString,
                        EnvironmentVariableTarget.User
                    );
                });

    Target CreateTestDatabase =>
        _ =>
            _.DependsOn(EnsureDatabaseContainerResponsive)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(
                        DatabaseServer,
                        DatabaseServerAdminPassword
                    );

                    var testConnectionString =
                        await databaseHelper.CreateDatabaseAndGetConnectionString(
                            TestDatabaseName,
                            TestDatabaseName,
                            "SuperSecure0!"
                        );
                    Environment.SetEnvironmentVariable(
                        TestDatabaseName.ToConnectionStringEnvironmentVariableName(),
                        testConnectionString,
                        EnvironmentVariableTarget.User
                    );
                    Console.WriteLine(
                        $"##vso[task.setvariable variable={TestDatabaseName.ToConnectionStringEnvironmentVariableName()}]{testConnectionString}"
                    );
                });

    readonly string DevelopmentDatabaseConnectionString = GetVariable<string>(
        DevelopmentDatabaseName.ToConnectionStringEnvironmentVariableName()
    );

    readonly string TestDatabaseConnectionString = GetVariable<string>(
        TestDatabaseName.ToConnectionStringEnvironmentVariableName()
    );

    Target MigrateDevelopmentDatabase =>
        _ =>
            _.DependsOn(CompileSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {DevelopmentDatabaseConnectionString}"
                    );
                });

    Target MigrateTestDatabase =>
        _ =>
            _.DependsOn(CompileSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{MigrationsDllFile} {TestDatabaseConnectionString}");
                });

    Target ResetDevelopmentDatabase =>
        _ =>
            _.DependsOn(CompileSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {DevelopmentDatabaseConnectionString} --cleanFirst"
                    );
                });

    Target ResetTestDatabase =>
        _ =>
            _.DependsOn(CompileSolution)
                .Executes(() =>
                {
                    Log.Information(MigrationsDllFile);
                    Log.Information(TestDatabaseConnectionString);
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {TestDatabaseConnectionString} --cleanFirst"
                    );
                });

    Target SetupDevelopmentDatabases =>
        _ =>
            _.DependsOn(
                CreateDatabaseDockerContainer,
                CreateDevelopmentDatabase,
                MigrateDevelopmentDatabase,
                MigrateTestDatabase
            );

    #endregion
}

public static class Extensions
{
    public static string ToConnectionStringEnvironmentVariableName(this string databaseName) =>
        $"{databaseName.ToUpper()}__CONNECTIONSTRING";
}
