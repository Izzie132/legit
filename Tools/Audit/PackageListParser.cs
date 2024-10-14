using System.Text.RegularExpressions;

namespace Audit;

// `dotnet list package` produces semi-structured output that looks like this:
//
// Project 'Web' has the following package references
//    [net8.0]:
//    Top-level Package                                        Requested   Resolved
//    > Azure.Extensions.AspNetCore.Configuration.Secrets                  1.2.2
//    > Azure.Identity                                                     1.9.0
//
//    Transitive Package                                                  Resolved
//    > Azure.Core                                                        1.32.0
//    > Azure.Security.KeyVault.Secrets                                   4.2.0
//
// Project 'Migrations' has the following package references
//    [net8.0]:
//    Top-level Package                                       Requested   Resolved
//    > dbup-sqlserver                                                    5.0.8
//    > Microsoft.Data.SqlClient                                          5.1.1
//
//    Transitive Package                                                  Resolved
//    > Azure.Core                                                        1.32.0
//    > Azure.Extensions.AspNetCore.Configuration.Secrets                 1.2.2
//
// ...

public static partial class PackageListParser
{
    private enum PackageType
    {
        TopLevel,
        Transitive,
    }

    public static ICollection<ProjectDetails> Parse(string input)
    {
        // We normalise to UNIX line endings to ensure compatibility with both Windows and Linux
        var normalisedInput = NormaliseLineEndings(input);

        var lines = normalisedInput.Split("\n");

        var projects = new List<ProjectDetails>();
        ProjectDetails? currentProject = null;
        PackageType? currentPackageType = null;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (IsProjectSectionHeader(line))
            {
                var projectName = EnclosedInSingleQuotesRegex().Match(trimmedLine).Groups[1].Value;

                currentProject = new ProjectDetails(
                    name: projectName,
                    topLevelPackages: new List<PackageDetails>(),
                    transitivePackages: new List<PackageDetails>()
                );

                projects.Add(currentProject);
            }
            else if (IsTopLevelPackageSectionHeader(line))
            {
                currentPackageType = PackageType.TopLevel;
            }
            else if (IsTransitivePackageSectionHeader(line))
            {
                currentPackageType = PackageType.Transitive;
            }
            else if (IsDependencyDetails(line))
            {
                if (currentProject == null)
                {
                    continue;
                }

                var packageDetails = PackageDetails.FromPackageListLine(line);

                switch (currentPackageType)
                {
                    case PackageType.TopLevel:
                        currentProject.TopLevelPackages.Add(packageDetails);
                        break;
                    case PackageType.Transitive:
                        currentProject.TransitivePackages.Add(packageDetails);
                        break;
                    default:
                        throw new ArgumentException(nameof(currentPackageType));
                }
            }
        }

        var allProjectNames = projects.Select(p => p.Name).ToHashSet();

        foreach (var project in projects)
        {
            project.RemoveTopLevelPackagesWhere(IsInternalPackageReference);
            project.RemoveTransitivePackagesWhere(IsInternalPackageReference);
            continue;

            // Cross-project references are included in the output, so we may end up with dependencies
            // such as "Web@1.0.0" or "Builders@1.0.0", which we do not want to send to Sonatype since
            // they are not real open-source packages.
            bool IsInternalPackageReference(PackageDetails packageDetails) =>
                allProjectNames.Contains(packageDetails.Name) && packageDetails.Resolved == "1.0.0";
        }

        return projects;
    }

    private static string NormaliseLineEndings(string value) => value.Replace("\r\n", "\n").Replace("\r", "\n");

    // e.g. "Project 'Web' has the following package references"
    private static bool IsProjectSectionHeader(string line) => line.Trim().StartsWith("Project");

    // e.g. "Top-level Package               Requested   Resolved"
    private static bool IsTopLevelPackageSectionHeader(string line) => line.Trim().StartsWith("Top-level Package");

    // e.g. "Transitive Package                          Resolved"
    private static bool IsTransitivePackageSectionHeader(string line) => line.Trim().StartsWith("Transitive Package");

    // e.g. "> Azure.Core                                1.32.0"
    private static bool IsDependencyDetails(string line) => line.Trim().StartsWith('>');

    // This will match the project name (surrounded by single quotes) in the project section header
    [GeneratedRegex("'([^']+)'")]
    private static partial Regex EnclosedInSingleQuotesRegex();
}
