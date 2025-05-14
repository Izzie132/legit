using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Audit;

var rootCommand = new RootCommand
{
    new Option<string>(
        name: "packageListFilePath",
        description: "The path to the file containing the output from the `dotnet list package` command",
        getDefaultValue: () => "../../nuget_packages.txt"
    ),
    new Option<string>(
        name: "sonatypeUsername",
        description: "The email address of the Sonatype account that will be used to make authenticated calls to the Sonatype OSS Index API",
        getDefaultValue: () => Environment.GetEnvironmentVariable("SONATYPE_OSS_INDEX_USERNAME") ?? ""
    ),
    new Option<string>(
        name: "sonatypeApiToken",
        description: "The API token of the Sonatype account that will be used to make authenticated calls to the Sonatype OSS Index API",
        getDefaultValue: () => Environment.GetEnvironmentVariable("SONATYPE_OSS_INDEX_API_TOKEN") ?? ""
    ),
};

rootCommand.Handler = CommandHandler.Create(Handle);

var exitCode = await rootCommand.InvokeAsync(args);

return exitCode;

static async Task<int> Handle(string packageListFilePath, string sonatypeUsername, string sonatypeApiToken)
{
    var inputFileAbsolutePath = Path.GetFullPath(packageListFilePath);

    await Console.Out.WriteLineAsync($"Attempting to read package list from '{inputFileAbsolutePath}' ...");

    if (!File.Exists(inputFileAbsolutePath))
    {
        throw new ArgumentException(
            $"Package list file '{inputFileAbsolutePath}' does not exist. Have you run ./Tools/Audit/GenerateNuGetPackageList.ps1?"
        );
    }

    var input = await File.ReadAllTextAsync(inputFileAbsolutePath);

    var projects = PackageListParser.Parse(input);

    var allPackages = projects
        .SelectMany(project => project.AllPackages)
        .DistinctBy(package => new { package.Name, package.Resolved })
        .OrderBy(package => package.Name)
        .ToList();

    var vulnerabilities = new List<VulnerabilityDetails>();

    const int batchSize = 128;

    foreach (var batch in allPackages.Chunk(batchSize))
    {
        vulnerabilities.AddRange(
            await PackageChecker.AuditPackages(
                packages: batch,
                projects: projects,
                sonatypeOssIndexUsername: sonatypeUsername,
                sonatypeOssIndexApiToken: sonatypeApiToken
            )
        );
    }

    var success = true;

    foreach (var ignoredVulnerability in Ignored.Vulnerabilities)
    {
        var numberRemoved = vulnerabilities.RemoveAll(ignoredVulnerability.IsMatch);

        if (numberRemoved == 0)
        {
            success = false;
            await ConsoleHelpers.WriteLineRed(
                $"Ignored vulnerability {ignoredVulnerability.Cve} for {ignoredVulnerability.PackageName}@{ignoredVulnerability.PackageVersion} does not apply. Please remove it from the ignore list."
            );
        }
    }

    var vulnerabilityCount = vulnerabilities.Count;

    if (vulnerabilityCount == 0)
    {
        await ConsoleHelpers.WriteLineGreen($"Scanned {allPackages.Count} packages - no vulnerabilities found.");
    }
    else
    {
        success = false;

        var vulnerabilityNoun = vulnerabilityCount > 1 ? "vulnerabilities" : "vulnerability";

        await Console.Error.WriteLineAsync(
            $"Scanned {allPackages.Count} packages - {vulnerabilityCount} {vulnerabilityNoun} found."
        );

        foreach (var vulnerability in vulnerabilities)
        {
            var foregroundColor = Console.ForegroundColor;

            Console.ForegroundColor = vulnerability.Severity switch
            {
                VulnerabilitySeverity.Low => ConsoleColor.Gray,
                VulnerabilitySeverity.Medium => ConsoleColor.Yellow,
                VulnerabilitySeverity.High => ConsoleColor.Red,
                VulnerabilitySeverity.Critical => ConsoleColor.DarkRed,
                _ => foregroundColor,
            };

            await Console.Error.WriteLineAsync(vulnerability.DisplayString);

            Console.ForegroundColor = foregroundColor;
        }
    }

    return success ? 0 : 1;
}
