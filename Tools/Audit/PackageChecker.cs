namespace Audit;

public static class PackageChecker
{
    public static async Task<ICollection<VulnerabilityDetails>> AuditPackages(
        ICollection<ProjectDetails> projects,
        ICollection<PackageDetails> packages,
        string sonatypeOssIndexUsername,
        string sonatypeOssIndexApiToken
    )
    {
        var responses = await SonatypeService.GetResponses(packages, sonatypeOssIndexUsername, sonatypeOssIndexApiToken);

        var vulnerabilities = new List<VulnerabilityDetails>();

        foreach (var reportResponse in responses)
        {
            if (reportResponse.Vulnerabilities.Count == 0)
            {
                await ConsoleHelpers.WriteLineGreen($"PASS | {reportResponse.PackageName}@{reportResponse.PackageVersion}");
            }
            else
            {
                var affectedProjects = GetAffectedProjects(projects, reportResponse);

                var vulnerabilitiesToAdd = reportResponse
                    .Vulnerabilities.Select(report =>
                        VulnerabilityDetails.FromReportResponse(reportResponse, report, affectedProjects)
                    )
                    .ToList();

                vulnerabilities.AddRange(vulnerabilitiesToAdd);

                if (vulnerabilitiesToAdd.All(v => Ignored.Vulnerabilities.Any(ignored => ignored.IsMatch(v))))
                {
                    await ConsoleHelpers.WriteLineYellow(
                        $"SKIP | {reportResponse.PackageName}@{reportResponse.PackageVersion}"
                    );
                }
                else
                {
                    await ConsoleHelpers.WriteLineRed(
                        $"FAIL | {reportResponse.PackageName}@{reportResponse.PackageVersion}"
                    );
                }
            }
        }

        return vulnerabilities;
    }

    private static List<string> GetAffectedProjects(
        ICollection<ProjectDetails> projects,
        ComponentReportResponse reportResponse
    )
    {
        var affectedProjects = new List<string>();

        foreach (var project in projects)
        {
            affectedProjects.AddRange(
                project.AllPackages.Where(package => package.IsMatch(reportResponse)).Select(_ => project.Name)
            );
        }

        return affectedProjects.Distinct().OrderBy(project => project).ToList();
    }
}
