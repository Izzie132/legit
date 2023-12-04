using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.SqlServer;
using Microsoft.Azure.Services.AppAuthentication;

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
                name: "--useAzureIdentity",
                description: "Configures the database connection to authenticate with Azure Active Directory",
                getDefaultValue: () => false
            ),
            new Option<bool>(
                name: "--cleanFirst",
                description: "Runs scripts to clean the database before running migrations (should only be used in development/test scenarios)",
                getDefaultValue: () => false
            ),
            new Option<bool>(
                name: "--quiet",
                description: "Suppresses console output",
                getDefaultValue: () => false
            ),
        };

        rootCommand.Handler = CommandHandler.Create(
            (string connectionString, bool useAzureIdentity, bool cleanFirst, bool quiet) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new ArgumentException(
                            "Please pass a connection string as the first argument"
                        );
                    }

                    IConnectionManager connectionManager;

                    string? azureAccessToken = null;
                    if (useAzureIdentity)
                    {
                        var httpClient = new HttpClient();
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        azureAccessToken = azureServiceTokenProvider
                            .GetAccessTokenAsync("https://database.windows.net/")
                            .Result;
                        connectionManager = new AzureSqlConnectionManager(
                            connectionString,
                            azureAccessToken
                        );
                    }
                    else
                    {
                        connectionManager = new SqlConnectionManager(connectionString);
                    }

                    var upgrader = DeployChanges
                        .To
                        .SqlDatabase(connectionManager)
                        .WithScriptsEmbeddedInAssembly(
                            Assembly.GetExecutingAssembly(),
                            s => s.Contains("Migrations.Scripts")
                        )
                        .LogToConsole()
                        .LogScriptOutput()
                        .WithTransaction()
                        .Build();

                    if (cleanFirst)
                    {
                        RunCleanScripts(connectionManager, quiet);
                    }

                    RunVersionedMigrations(connectionManager, quiet);

                    if (!quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Success!");
                        Console.ResetColor();
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 1;
                }
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
            .To
            .SqlDatabase(connectionManager)
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
            .PerformUpgradeAndLogResult(
                successMessage: quiet ? null : "Finished running clean scripts"
            );
    }

    private static void RunVersionedMigrations(IConnectionManager connectionManager, bool quiet)
    {
        if (!quiet)
        {
            Console.WriteLine("Running versioned migration scripts");
        }

        var upgradeEngineBuilder = DeployChanges
            .To
            .SqlDatabase(connectionManager)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                scriptPath =>
                    !scriptPath.Contains(CleanScriptsDirectory)
                    && !scriptPath.Contains(RepeatableScriptsDirectory)
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
            .PerformUpgradeAndLogResult(
                successMessage: quiet ? null : "Finished running versioned migration scripts"
            );
    }

    private static void PerformUpgradeAndLogResult(
        this UpgradeEngine upgradeEngine,
        string? successMessage
    )
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
