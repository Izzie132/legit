using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Serilog;

#pragma warning disable SA1124 // (DoNotUseRegions) Regions are useful in this file
#pragma warning disable SA1203 // (ConstantsMustAppearBeforeFields) This would prevent us from putting e.g. the database constants with the database code
#pragma warning disable IDE0051 // (Remove unused private member) Nuke accesses these via reflection
sealed class Build : NukeBuild
{
    // Support plugins are available for:
    //   - JetBrains ReSharper        https://nuke.build/resharper
    //   - JetBrains Rider            https://nuke.build/rider
    //   - Microsoft VisualStudio     https://nuke.build/visualstudio
    //   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.CompileSolution);

    [Solution("QQProjectName.sln")]
    readonly Solution solution;

    readonly BuildConfiguration buildConfiguration = BuildConfiguration.Release;

    const string ProjectName = "QQProjectName";
    const string DotNetVersion = "net8.0";

    T GetRequiredEnvVar<T>(string name) =>
        EnvironmentInfo.GetVariable<T>(name)
        ?? throw new InvalidOperationException($"Environment variable {name} is required but not set.");

    static string NugetPackageListFilePath => RootDirectory / "nuget_packages.txt";

    static AbsolutePath WebProjectDirectory => RootDirectory / "Web";

    static AbsolutePath ReactClientDirectory => WebProjectDirectory / "client-app";

    static AbsolutePath BuildOutputDirectory => RootDirectory / "build-output";

    string SonatypeOssIndexUsername => GetRequiredEnvVar<string>("SONATYPE_OSS_INDEX_USERNAME");
    string SonatypeOssIndexApiToken => GetRequiredEnvVar<string>("SONATYPE_OSS_INDEX_API_TOKEN");

    AbsolutePath MigrationsDirectory => RootDirectory / "Tools" / "Migrations";
    AbsolutePath MigrationsDllFile => MigrationsDirectory / $"bin/{buildConfiguration}/{DotNetVersion}/Migrations.dll";

    AbsolutePath DataSeederDirectory => RootDirectory / "Tools" / "DataSeeder";
    AbsolutePath DataSeederDllFile => DataSeederDirectory / $"bin/{buildConfiguration}/{DotNetVersion}/DataSeeder.dll";

    AbsolutePath AuditProjectDirectory => RootDirectory / "Tools" / "Audit";
    AbsolutePath AuditProjectDllFile => AuditProjectDirectory / "bin" / buildConfiguration / DotNetVersion / "Audit.dll";

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

    Target RestoreDotNetTools =>
        _ =>
            _.Description("Restores dotnet tools and installs prerequisites")
                .Executes(() =>
                {
                    DotNetTasks.DotNetToolRestore();
                    DotNetTasks.DotNet("husky install");
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
                    DotNetTasks.DotNetBuild(s =>
                        s.SetProjectFile(solution).SetConfiguration(buildConfiguration).EnableNoRestore()
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
                    AuditFrontEndPackages,
                    CheckBackEndCodeQuality,
                    RunBackendTests,
                    AuditBackEndPackages,
                    CheckCleanGit
                );

    Target Publish =>
        _ =>
            _.Description("Publishes the Web and Migrations projects")
                .DependsOn(BuildAndTest)
                .Executes(() =>
                {
                    DotNetTasks.DotNetPublish(s =>
                        s.SetProject(WebProjectDirectory)
                            .SetConfiguration(buildConfiguration)
                            .SetOutput(BuildOutputDirectory / "Web")
                            .EnableNoRestore()
                    );

                    DotNetTasks.DotNetPublish(s =>
                        s.SetProject(MigrationsDirectory)
                            .SetConfiguration(buildConfiguration)
                            .SetOutput(BuildOutputDirectory / "Migrations")
                            .EnableNoRestore()
                    );

                    File.Copy(RootDirectory / "global.json", BuildOutputDirectory / "global.json", overwrite: true);
                });

    #region Tests

    Target CheckFrontEndCompiles =>
        _ =>
            _.Description("Check build runs successfully")
                .DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    NpmTasks.Npm("run build", ReactClientDirectory);
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
                .DependsOn(RestoreFrontEnd, CheckFrontEndCompiles, CheckFrontEndCodeQuality)
                .Executes(() =>
                {
                    NpmTasks.Npm("run test:ci", ReactClientDirectory);
                });

    Target AuditFrontEndPackages =>
        _ =>
            _.DependsOn(RestoreFrontEnd)
                .Executes(() =>
                {
                    var arguments = $"run scan -- --user {SonatypeOssIndexUsername} --password {SonatypeOssIndexApiToken}";
                    ProcessTasks.StartProcess("npm", arguments, ReactClientDirectory).AssertZeroExitCode();
                });

    Target CheckBackEndCodeQuality =>
        _ =>
            _.Description("Run CSharpier on solution")
                .DependsOn(RestoreDotNetTools)
                .Executes(() =>
                {
                    DotNetTasks.DotNet("format style --verify-no-changes");
                    DotNetTasks.DotNet("format analyzers --verify-no-changes");
                    DotNetTasks.DotNet("csharpier check .");
                });

    Target RunBackendTests =>
        _ =>
            _.Description("Run backend tests")
                .DependsOn(CompileSolution, ResetTestDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNetTest(s =>
                        s.SetProjectFile(solution).SetConfiguration(buildConfiguration).EnableNoRestore()
                    );
                });

    Target AuditBackEndPackages =>
        _ =>
            _.DependsOn(CompileSolution)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{AuditProjectDllFile} "
                            + $"packageListFilePath {NugetPackageListFilePath} "
                            + $"sonatypeUsername {SonatypeOssIndexUsername} "
                            + $"sonatypeApiToken {SonatypeOssIndexApiToken}"
                    );
                });

    Target CheckCleanGit =>
        _ =>
            _.Executes(() =>
                {
                    var status = GitTasks.Git($"status --porcelain");
                    if (status.Count != 0)
                    {
                        Log.Error("There are uncommitted changes in the working directory");
                        status.ForEach(x => Log.Error("{file}", x.Text));
                        throw new InvalidOperationException("Git working directory is not clean.");
                    }
                    else
                    {
                        Log.Information("Git working directory is clean.");
                    }
                })
                .DependsOn(CompileSolution, CheckFrontEndCompiles);

    #endregion

    #region Databases

    const int DatabasePort = 1407;
    static string DatabaseContainerName => $"{ProjectName}_Database";

    static string DatabaseServer => $"localhost,{DatabasePort}";
    const string DatabaseServerAdminPassword = "SuperSecure0!";

    const string DevelopmentDatabaseName = ProjectName;
    const string DevelopmentDatabasePassword = "DefinitelyDurable1!";
    static string DevelopmentDatabaseConnectionString =>
        DatabaseHelper.GetConnectionString(DatabaseServer, DevelopmentDatabaseName, DevelopmentDatabasePassword);

    const string TestDatabaseName = $"{ProjectName}Test";
    const string TestDatabasePassword = "TotallyTrusted2!";
    static string TestDatabaseConnectionString =>
        DatabaseHelper.GetConnectionString(DatabaseServer, TestDatabaseName, TestDatabasePassword);

    Target CreateDatabaseContainer =>
        _ =>
            _.Description("Create SQL Server 2022 Docker container")
                .DependsOn(DestroyDatabaseContainer)
                .Executes(() =>
                {
                    DockerTasks.DockerPull(c => c.SetName("mcr.microsoft.com/mssql/server:2022-latest"));

                    DockerTasks.DockerCreate(c =>
                        c.SetImage("mcr.microsoft.com/mssql/server:2022-latest")
                            .SetName(DatabaseContainerName)
                            .SetEnv("ACCEPT_EULA=Y", $"SA_PASSWORD={DatabaseServerAdminPassword}")
                            .SetPublish($"{DatabasePort}:1433")
                    );

                    DockerTasks.DockerStart(c => c.SetContainers(DatabaseContainerName));
                });

    Target DestroyDatabaseContainer =>
        _ =>
            _.Description("Remove SQL Server 2022 Docker Container if it exists")
                .Executes(() =>
                {
                    var list = DockerTasks.DockerContainerLs(c => c.EnableAll());

                    if (list.Any(output => output.Text.Contains(DatabaseContainerName)))
                    {
                        Log.Information($"{DatabaseContainerName} found... Deleting container");
                        DockerTasks.DockerStop(c => c.SetContainers(DatabaseContainerName));
                        DockerTasks.DockerRm(c => c.SetContainers(DatabaseContainerName));
                    }
                    else
                    {
                        Log.Information($"{DatabaseContainerName} not found.");
                    }
                });

    Target EnsureDatabaseContainerResponsive =>
        _ =>
            _.Description("Wait for Docker SQL Server to respond to connections")
                .After(CreateDatabaseContainer)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(DatabaseServer, DatabaseServerAdminPassword);

                    await databaseHelper.WaitForSqlServerResponse();
                });

    Target CreateDevelopmentDatabase =>
        _ =>
            _.Description("Create development database and user")
                .DependsOn(EnsureDatabaseContainerResponsive)
                .After(CreateDatabaseContainer)
                .Executes(async () =>
                {
                    var databaseHelper = new DatabaseHelper(DatabaseServer, DatabaseServerAdminPassword);

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
                    var databaseHelper = new DatabaseHelper(DatabaseServer, DatabaseServerAdminPassword);

                    await databaseHelper.CreateDatabase(TestDatabaseName, TestDatabaseName, TestDatabasePassword);
                });

    Target MigrateDevelopmentDatabase =>
        _ =>
            _.Description("Run migrations against development database")
                .DependsOn(CompileSolution, EnsureDatabaseContainerResponsive)
                .After(CreateDevelopmentDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{MigrationsDllFile} {DevelopmentDatabaseConnectionString}");
                });

    Target MigrateTestDatabase =>
        _ =>
            _.Description("Run migrations against test database")
                .DependsOn(CompileSolution, EnsureDatabaseContainerResponsive)
                .After(CreateTestDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{MigrationsDllFile} {TestDatabaseConnectionString}");
                });

    Target SeedDevelopmentDatabase =>
        _ =>
            _.Description("Seed development database")
                .DependsOn(CompileSolution, EnsureDatabaseContainerResponsive)
                .After(MigrateDevelopmentDatabase)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{DataSeederDllFile} {DevelopmentDatabaseConnectionString} --dataSeedMode Full");
                });

    Target ResetDevelopmentDatabase =>
        _ =>
            _.Description("Clean and run migrations against development database")
                .DependsOn(CompileSolution, EnsureDatabaseContainerResponsive)
                .Executes(() =>
                {
                    DotNetTasks.DotNet(
                        $"{MigrationsDllFile} {DevelopmentDatabaseConnectionString} --cleanFirst --dataSeedMode Full"
                    );
                });

    Target ResetTestDatabase =>
        _ =>
            _.Description("Clean and run migrations against test database")
                .DependsOn(CompileSolution, EnsureDatabaseContainerResponsive)
                .Executes(() =>
                {
                    DotNetTasks.DotNet($"{MigrationsDllFile} {TestDatabaseConnectionString} --cleanFirst");
                });

    Target ResetDatabases =>
        _ =>
            _.Description("Clean and run migrations against both development and test databases")
                .DependsOn(ResetDevelopmentDatabase, ResetTestDatabase);

    Target CreateAndSetupDatabaseDockerContainer =>
        _ =>
            _.Description("Create SQl Server container and migrated databases")
                .DependsOn(
                    CreateDatabaseContainer,
                    CreateDevelopmentDatabase,
                    MigrateDevelopmentDatabase,
                    SeedDevelopmentDatabase,
                    CreateTestDatabase,
                    MigrateTestDatabase
                );

    #endregion

    #region Development

    Target SetupDevelopmentEnvironment =>
        _ =>
            _.Description("Setup development environment with docker database and required packages")
                .DependsOn(CreateAndSetupDatabaseDockerContainer, RestoreSolution, RestoreFrontEnd, RestoreDotNetTools);

    #endregion
}
#pragma warning restore SA1124, SA1203, IDE0051
