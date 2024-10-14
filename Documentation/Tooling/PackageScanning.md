# Package Scanning

The [Sonatype OSS Index REST API](https://ossindex.sonatype.org/doc/rest) is used to automatically produce vulnerability reports for both NPM and NuGet packages used in the project.

## Credentials

Note that requests to the Sonatype OSS Index API are rate-limited unless authenticated. To pass credentials, set the following environment variables on your local machine:

- `SONATYPE_OSS_INDEX_USERNAME`
- `SONATYPE_OSS_INDEX_API_TOKEN`

Note that these credentials should also be set via appropriate environment variables in the CI pipeline.

## NPM

To scan NPM packages, we use the community-maintained [auditjs](https://github.com/sonatype-nexus-community/auditjs) package, which hooks into the Sonatype OSS Index API.

To run a scan locally, run the following NUKE target:

```bash
nuke Scan-Front-End-Packages
```

## NuGet

To scan NuGet packages, since we haven't found any decent open source packages wrapping the Sonatype OSS Index API, we've built our own basic tooling.

The tooling to audit NuGet packages is contained in the `./Tools/Audit` project. To run a scan locally, you must first generate a list of NuGet packages:

```bash
./Tools/Audit/GenerateNuGetPackageList.ps1
```

This will output a list of NuGet packages (in a semi-structured format) in the `./nuget_packages.txt` file.

Next, run the following NUKE target:

```bash
nuke Scan-Back-End-Packages
```

If you need to ignore certain vulnerabilities (i.e. if you are seeing false positives), you can add the CVEs to the vulnerability list in `./Tools/Audit/Ignored.cs` (make sure you explain your reason for ignoring the vulnerability and provide evidence to back it up).
