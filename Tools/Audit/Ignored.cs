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
                PackageName: "System.Private.DataContractSerialization",
                PackageVersion: "4.3.0",
                Cve: "CVE-2023-21538",
                Reason: "This vulnerability only affects .NET 6 versions (we are using .NET 8).",
                Evidence: "https://github.com/dotnet/announcements/issues/244"
            ),
        }
            .ToList()
            .AsReadOnly();
}
