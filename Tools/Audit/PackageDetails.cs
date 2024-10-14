using System.Text.RegularExpressions;

namespace Audit;

public partial record PackageDetails(string Name, string Requested, string Resolved)
{
    public static PackageDetails FromPackageListLine(string line)
    {
        // If the "Requested" and "Resolved" are the same, the line will be of the form:
        // "> Azure.Core                                1.32.0"
        //
        // If they are different, the line will be of the form:
        // "> Azure.Core                1.32.0          1.32.1"

        var trimmedLine = line.Trim();

        var lineParts = TwoOrMoreSpacesRegex().Split(trimmedLine);

        // Strip the "> " prefix
        var skipLength = "> ".Length;
        var packageName = lineParts[0].Trim()[skipLength..];

        // e.g. "1.32.0"
        var requestedVersion = lineParts[1].Trim();

        // e.g. "1.32.1"
        var resolvedVersion = lineParts.Length < 3 ? requestedVersion : lineParts[2].Trim();

        return new PackageDetails(Name: packageName, Requested: requestedVersion, Resolved: resolvedVersion);
    }

    public string Coordinates => $"pkg:nuget/{Name}@{Resolved}";

    public bool IsMatch(ComponentReportResponse reportResponse) =>
        Name == reportResponse.PackageName && Resolved == reportResponse.PackageVersion;

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex TwoOrMoreSpacesRegex();
}
