using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using DataSeeder.Features.Users;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Database;

namespace DataSeeder;

public static class DataSeeder
{
    private static DataContext dataContext = default!;
    public static DataSeederModeAmounts DataSeederModeAmounts = default!;
    public static Stopwatch Stopwatch = default!;
    private static bool quiet = default!;

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand
        {
            new Argument<string>("connectionString"),
            new Option<DataSeedMode>(
                name: "--dataSeedMode",
                description: "The seed mode to use",
                getDefaultValue: () => DataSeedMode.Full
            ),
            new Option<bool>(name: "--quiet", description: "Suppresses console output", getDefaultValue: () => false),
        };

        rootCommand.Handler = CommandHandler.Create(SeedTestData);

        return rootCommand.Invoke(args);
    }

    public static void SeedTestData(string connectionString, DataSeedMode dataSeedMode, bool quiet)
    {
        if (dataSeedMode == DataSeedMode.None)
            return;

        dataContext = new DataContext(
            new DbContextOptionsBuilder<DataContext>().Options,
            new OptionsWrapper<ProjectNameOptions>(new ProjectNameOptions { ConnectionString = connectionString })
        );
        DataSeeder.quiet = quiet;

        switch (dataSeedMode)
        {
            case DataSeedMode.Minimal:
                DataSeederModeAmounts = DataSeederModeAmounts.Minimal;
                SeedData();
                break;
            case DataSeedMode.Full:
                DataSeederModeAmounts = DataSeederModeAmounts.Full;
                SeedData();
                break;
            case DataSeedMode.Load:
                DataSeederModeAmounts = DataSeederModeAmounts.Load;
                SeedData();
                break;
            case DataSeedMode.None:
            default:
                Console.WriteLine($"Unsupported seed mode {dataSeedMode} - no data will be seeded");
                break;
        }
    }

    private static void SeedData()
    {
        Stopwatch = Stopwatch.StartNew();

        IfVerbose(Console.WriteLine, "Seeding data...");

        TestUsers.BuildAndSeedUsers();

        dataContext.SaveChanges();

        IfVerbose(Console.WriteLine, $"Seeding data complete in {Stopwatch.ElapsedMilliseconds}ms");
    }

    public static void Insert<TEntity>(ICollection<TEntity> entities)
        where TEntity : class
    {
        IfVerbose(Console.Write, $"Seeding {typeof(TEntity).Name} x {GetHumanReadableEntityCount(entities.Count)} ...");
        const SqlBulkCopyOptions bulkCopyOptions = SqlBulkCopyOptions.Default;
        dataContext.BulkInsert(entities, new BulkConfig { SqlBulkCopyOptions = bulkCopyOptions });
        IfVerbose(Console.WriteLine, Stopwatch.ElapsedMilliseconds);
        Stopwatch.Restart();
    }

    private static string GetHumanReadableEntityCount(int entityCount) =>
        entityCount switch
        {
            < 1_000 => $"{entityCount}",
            < 1_000_000 => $"{entityCount / 1000.0:0.##}k",
            _ => $"{entityCount / 1000_000.0:0.##}m"
        };

    private static string GetHumanReadableElapsedTime(long elapsedMilliseconds) =>
        elapsedMilliseconds switch
        {
            < 1_000 => $"{elapsedMilliseconds}ms",
            < 60_000 => $"{elapsedMilliseconds / 1000}s",
            _ => $"{elapsedMilliseconds / 1000 / 60}m {elapsedMilliseconds / 1000 % 60}s"
        };

    private static ConsoleColor GetConsoleColor(long elapsedMilliseconds) =>
        elapsedMilliseconds switch
        {
            < 1_000 => ConsoleColor.Green,
            < 10_000 => ConsoleColor.Yellow,
            < 60_000 => ConsoleColor.Red,
            _ => ConsoleColor.DarkRed
        };

    public static void WriteLineElapsedTime(long elapsedMilliseconds)
    {
        var foregroundColor = Console.ForegroundColor;
        Console.ForegroundColor = GetConsoleColor(elapsedMilliseconds);
        Console.WriteLine($" {GetHumanReadableElapsedTime(elapsedMilliseconds)}");
        Console.ForegroundColor = foregroundColor;
    }

    public static void IfVerbose<T>(Action<T> action, T value)
    {
        if (!quiet)
        {
            action(value);
        }
    }
}
