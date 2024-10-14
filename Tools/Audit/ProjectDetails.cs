namespace Audit;

public class ProjectDetails(
    string name,
    ICollection<PackageDetails> topLevelPackages,
    ICollection<PackageDetails> transitivePackages
)
{
    public string Name { get; } = name;
    public ICollection<PackageDetails> TopLevelPackages { get; private set; } = topLevelPackages;
    public ICollection<PackageDetails> TransitivePackages { get; private set; } = transitivePackages;

    public ICollection<PackageDetails> AllPackages =>
        TopLevelPackages
            .Concat(TransitivePackages)
            .DistinctBy(p => new { p.Name, p.Resolved })
            .OrderBy(p => p.Name)
            .ToList();

    public void RemoveTopLevelPackagesWhere(Func<PackageDetails, bool> filter)
    {
        TopLevelPackages = TopLevelPackages.Where(p => !filter(p)).ToList();
    }

    public void RemoveTransitivePackagesWhere(Func<PackageDetails, bool> filter)
    {
        TransitivePackages = TransitivePackages.Where(p => !filter(p)).ToList();
    }
}
