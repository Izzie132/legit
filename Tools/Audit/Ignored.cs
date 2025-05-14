namespace Audit;

public record IgnoredVulnerability(string PackageName, string PackageVersion, string Cve, string Reason, string Evidence)
{
    public bool IsMatch(VulnerabilityDetails vulnerabilityDetails) =>
        vulnerabilityDetails.PackageName == PackageName
        && vulnerabilityDetails.PackageVersion == PackageVersion
        && vulnerabilityDetails.Report.Cve == Cve;
}

public static class Ignored
{
    public static ICollection<IgnoredVulnerability> Vulnerabilities =>
        new List<IgnoredVulnerability>
        {
            new(
                PackageName: "System.Text.Json",
                PackageVersion: "9.0.3",
                Cve: "CVE-2024-43485",
                Reason: "This is a false positive for v9, and we're not using the [ExtensionData] feature which was affected in previous versions anyway",
                Evidence: "https://github.com/dotnet/announcements/issues/329"
                    + ", "
                    + "https://github.com/DependencyTrack/dependency-track/issues/4568"
            ),
        }.AsReadOnly();
}
