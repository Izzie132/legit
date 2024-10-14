namespace Audit;

public record ComponentReportResponse(
    string Coordinates,
    string Description,
    string Reference,
    ICollection<ComponentReportVulnerabilityResponse> Vulnerabilities
)
{
    public string PackageName => Coordinates.Split("@")[0].Split("/")[1];
    public string PackageVersion => Coordinates.Split("@")[1];
}
