using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Reflection;
using DataSeeder;
using DbUp;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.SqlServer;

namespace Migrations;

public static class MigrationRunner
{
    private const string CleanScriptsDirectory = "Clean";
    private const string RepeatableScriptsDirectory = "Repeatable";

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand
        {
            new Argument<string>("connectionString"),
            new Option<bool>(
                name: "--cleanFirst",
                description: "Runs scripts to clean the database before running migrations (should only be used in development/test scenarios)",
                getDefaultValue: () => false
            ),
            new Option<DataSeedMode>(
                name: "--dataSeedMode",
                description: "Runs the test data seeder to populate the database with realistic test data with the given mode (None, Minimal, Full, Load)",
                getDefaultValue: () => DataSeedMode.None
            ),
            new Option<bool>(name: "--quiet", description: "Suppresses console output", getDefaultValue: () => false),
        };

        rootCommand.Handler = CommandHandler.Create(
            (string connectionString, bool cleanFirst, DataSeedMode dataSeedMode, bool quiet) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new ArgumentException("Please pass a connection string as the first argument");
                    }

                    var connectionManager = new SqlConnectionManager(connectionString);

                    if (cleanFirst)
                    {
                        RunCleanScripts(connectionManager, quiet);
                    }

                    RunVersionedMigrations(connectionManager, quiet);

                    if (dataSeedMode != DataSeedMode.None)
                    {
                        DataSeeder.DataSeeder.SeedTestData(connectionString, dataSeedMode, quiet);
                    }

                    if (!quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Success!");
                        Console.ResetColor();
                    }

                    return 0;
                }
#pragma warning disable CA1031 // (Do not catch general exception types) We specifically want to catch all exceptions and return the correct exit code
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 1;
                }
#pragma warning restore CA1031
            }
        );

        return rootCommand.Invoke(args);
    }

    private static void RunCleanScripts(IConnectionManager connectionManager, bool quiet)
    {
        if (quiet)
        {
            Console.WriteLine("Running clean scripts");
        }

        var upgradeEngineBuilder = DeployChanges
            .To.SqlDatabase(connectionManager)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                scriptPath => scriptPath.Contains(CleanScriptsDirectory)
            )
            .WithTransactionPerScript()
            .WithExecutionTimeout(TimeSpan.FromSeconds(120))
            .JournalTo(new NullJournal());

        if (quiet)
        {
            upgradeEngineBuilder.LogToNowhere();
        }
        else
        {
            upgradeEngineBuilder.LogToConsole();
        }

        upgradeEngineBuilder
            .Build()
            .PerformUpgradeAndLogResult(successMessage: quiet ? null : "Finished running clean scripts");
    }

    private static void RunVersionedMigrations(IConnectionManager connectionManager, bool quiet)
    {
        if (!quiet)
        {
            Console.WriteLine("Running versioned migration scripts");
        }

        var upgradeEngineBuilder = DeployChanges
            .To.SqlDatabase(connectionManager)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                scriptPath => !scriptPath.Contains(CleanScriptsDirectory) && !scriptPath.Contains(RepeatableScriptsDirectory)
            )
            .WithTransactionPerScript()
            .WithExecutionTimeout(TimeSpan.FromSeconds(120));

        if (quiet)
        {
            upgradeEngineBuilder.LogToNowhere();
        }
        else
        {
            upgradeEngineBuilder.LogToConsole();
        }

        upgradeEngineBuilder
            .Build()
            .PerformUpgradeAndLogResult(successMessage: quiet ? null : "Finished running versioned migration scripts");
    }

    private static void PerformUpgradeAndLogResult(this UpgradeEngine upgradeEngine, string? successMessage)
    {
        var result = upgradeEngine.PerformUpgrade();

        if (!result.Successful)
        {
            throw result.Error;
        }

        if (!string.IsNullOrEmpty(successMessage))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(successMessage);
            Console.ResetColor();
        }
    }
}
