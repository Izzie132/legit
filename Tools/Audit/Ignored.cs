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
    public static ICollection<IgnoredVulnerability> Vulnerabilities => new List<IgnoredVulnerability> { }.AsReadOnly();
}
