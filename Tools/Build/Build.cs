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

    private static AbsolutePath BuildOutputDirectory => RootDirectory / "build-output";

    Target CleanSolution =>
        _ =>
            _.Description("Cleans .NET solution")
                .Executes(() =>
                {
                    DotNetTasks.DotNetClean();
                });
    Target RestoreSolution =>
        _ =>
            _.Description("Restores NuGet packages for .NET solution")
                .DependsOn(CleanSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNetRestore();
                });

    Target RestoreFrontEnd =>
        _ =>
            _.Description("Restores npm packages for React SPA")
                .Executes(() =>
                {
                    NpmTasks.Npm("install", ReactClientDirectory);
                });

    Target CompileSolution =>
        _ =>
            _.Description("Compiles .NET solution")
                .DependsOn(RestoreSolution)
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
            _.Description("Runs tests and code quality checks")
                .DependsOn(
                    CompileSolution,
                    CheckFrontEndCompiles,
                    CheckFrontEndCodeQuality,
                    RunFrontEndTests,
                    CheckBackEndCodeQuality,
                    RunBackendTests
                );

    Target Publish =>
        _ =>
            _.Description("Publishes the Web and Migrations projects")
                .DependsOn(BuildAndTest)
                .Executes(() =>
                {
                    DotNetTasks.DotNetPublish(
                        s =>
                            s.SetProject(WebProjectDirectory)
                                .SetConfiguration(Configuration)
                                .SetOutput(BuildOutputDirectory / "Web")
                                .EnableNoRestore()
                    );

                    DotNetTasks.DotNetPublish(
                        s =>
                            s.SetProject(MigrationsDirectory)
                                .SetConfiguration(Configuration)
                                .SetOutput(BuildOutputDirectory / "Migrations")
                                .EnableNoRestore()
                    );
                });

    #region Tests

    Target CheckFrontEndCompiles =>
        _ =>
            _.Description("Check frontend typescript compiles")
                .DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run typecheck", ReactClientDirectory);
                });

    Target CheckFrontEndCodeQuality =>
        _ =>
            _.Description("Run Prettier and ESLint on the frontend")
                .DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run lint:ci", ReactClientDirectory);
                    NpmTasks.Npm("run prettier:ci", ReactClientDirectory);
                });

    Target RunFrontEndTests =>
        _ =>
            _.Description("Run test on the frontend")
                .DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run test:ci", ReactClientDirectory);
                });

    Target CheckBackEndCodeQuality =>
        _ =>
            _.Description("Run CSharpier on solution")
                .Executes(() =>
                {
                    DotNetTasks.DotNetToolRestore();
                    DotNetTasks.DotNet("csharpier --check .");
                });

    Target RunBackendTests =>
        _ =>
            _.Description("Run backend tests")
                .DependsOn(CompileSolution, ResetTestDatabase)
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
    static readonly string DevelopmentDatabasePassword = "DefinitelyDurable1!";
    static string DevelopmentDatabaseConnectionString =>
        DatabaseHelper.GetConnectionString(
            DatabaseServer,
            DevelopmentDatabaseName,
            DevelopmentDatabasePassword
        );

    static readonly string TestDatabaseName = $"{ProjectName}Test";
    static readonly string TestDatabasePassword = "TotallyTrusted2!";
    static string TestDatabaseConnectionString =>
        DatabaseHelper.GetConnectionString(DatabaseServer, TestDatabaseName, TestDatabasePassword);

    Target CreateDatabaseContainer =>
        _ =>
            _.Description("Create SQL Server 2022 Docker container")
                .Executes(() =>
                {
                    DockerTasks.DockerPull(
                        c => c.SetName("mcr.microsoft.com/mssql/server:2022-latest")
                    );

                    DockerTasks.DockerCreate(
                        c =>
                            c.SetImage("mcr.microsoft.com/mssql/server:2022-latest")
                                .SetName(ProjectName)
                                .SetEnv(
                                    "ACCEPT_EULA=Y",
                                    $"SA_PASSWORD={DatabaseServerAdminPassword}"
                                )
                                .SetPublish($"{DatabasePort}:1433")
                    );

                    DockerTasks.DockerStart(c => c.SetContainers(ProjectName));
                });

    Target DestroyDatabaseContainer =>
        _ =>
            _.Description("Remove SQL Server 2022 Docker Container")
                .Executes(() =>
                {
                    DockerTasks.DockerStop(c => c.SetContainers(ProjectName));
                    DockerTasks.DockerRm(c => c.SetContainers(ProjectName));
                });

    Target EnsureDatabaseContainerResponsive =>
        _ =>
            _.Description("Wait for Docker SQL Server to respond to connections")
                .After(CreateDatabaseContainer)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(
                        DatabaseServer,
                        DatabaseServerAdminPassword
                    );

                    await databaseHelper.WaitForSqlServerResponse();
                });

    Target CreateDevelopmentDatabase =>
        _ =>
            _.Description("Create development database and user")
                .DependsOn(EnsureDatabaseContainerResponsive)
                .After(CreateDatabaseContainer)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(
                        DatabaseServer,
                        DatabaseServerAdminPassword
                    );

                    await databaseHelper.CreateDatabase(
                        DevelopmentDatabaseName,
                        DevelopmentDatabaseName,
                        DevelopmentDatabasePassword
                    );
                });

    Target CreateTestDatabase =>
        _ =>
            _.Description("Create test database and user")
                .DependsOn(EnsureDatabaseContainerResponsive)
                .After(CreateDatabaseContainer)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(
                        DatabaseServer,
                        DatabaseServerAdminPassword
                    );

                    await databaseHelper.CreateDatabase(
                        TestDatabaseName,
                        TestDatabaseName,
                        TestDatabasePassword
                    );
                });

    Target MigrateDevelopmentDatabase =>
        _ =>
            _.Description("Run migrations against development database")
                .DependsOn(CompileSolution)
                .After(CreateDevelopmentDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {DevelopmentDatabaseConnectionString}"
                    );
                });

    Target MigrateTestDatabase =>
        _ =>
            _.Description("Run migrations against test database")
                .DependsOn(CompileSolution)
                .After(CreateTestDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{MigrationsDllFile} {TestDatabaseConnectionString}");
                });

    Target ResetDevelopmentDatabase =>
        _ =>
            _.Description("Clean and run migrations against development database")
                .DependsOn(CompileSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {DevelopmentDatabaseConnectionString} --cleanFirst"
                    );
                });

    Target ResetTestDatabase =>
        _ =>
            _.Description("Clean and run migrations against test database")
                .DependsOn(CompileSolution)
                .Executes(() =>
                {
                    Log.Information(MigrationsDllFile);
                    Log.Information(TestDatabaseConnectionString);
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {TestDatabaseConnectionString} --cleanFirst"
                    );
                });

    Target CreateAndSetupDatabaseDockerContainer =>
        _ =>
            _.Description("Create SQl Server container and migrated databases")
                .DependsOn(
                    CreateDatabaseContainer,
                    CreateDevelopmentDatabase,
                    MigrateDevelopmentDatabase,
                    CreateTestDatabase,
                    MigrateTestDatabase
                );

    #endregion

    #region Development

    Target SetupDevelopmentEnvironment =>
        _ =>
            _.Description(
                    "Setup development environment with docker database and required packages"
                )
                .DependsOn(CreateAndSetupDatabaseDockerContainer, RestoreSolution, RestoreFrontEnd);

    #endregion
}
