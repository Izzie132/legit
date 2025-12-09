$ErrorActionPreference = "Stop"

$solutionName = "Legit"

$projectFolderPathsFromRoot = @(
    "Tests/Builders",
    "Tests/WebTests",
    "Tools/Build",
    "Tools/DataSeeder",
    "Tools/Migrations",
    "Web"
)

$scriptsSearchPath = Join-Path -Path "\Tools" -ChildPath "Audit"
$rootDirectory = $PSScriptRoot.Substring(0, $PSScriptRoot.LastIndexOf($scriptsSearchPath))

Write-Host "Discovered root directory: '$rootDirectory'"

$fullFolderPaths = $projectFolderPathsFromRoot | ForEach-Object { Join-Path -Path $rootDirectory -ChildPath $_ }

function DeleteFolder([string]$folderPath) {
    if (Test-Path -Path $folderPath) {
        Write-Verbose "Deleting '$folderPath' ..."
        Remove-Item -Path $folderPath -Recurse -Force
    } else {
        Write-Verbose "No '$folderPath' folder found."
    }
}

Write-Host "Cleaning up bin and obj folders ..."

foreach ($folderPath in $fullFolderPaths) {
    DeleteFolder (Join-Path -Path $folderPath -ChildPath "bin")
    DeleteFolder (Join-Path -Path $folderPath -ChildPath "obj")
}

function GetDotnetVerbosity {
    switch ($VerbosePreference) {
        "SilentlyContinue" {
            return "q"
        }
        default {
            return "m"
        }
    }
}

$dotnetVerbosity = GetDotnetVerbosity

$buildProjectPath = Join-Path -Path $rootDirectory -ChildPath "\Tools\Build\Build.csproj"

Write-Host "Cleaning projects ..."
dotnet clean $rootDirectory -v $dotnetVerbosity
dotnet clean $buildProjectPath -v $dotnetVerbosity

Write-Host "Restoring projects ..."
dotnet restore $rootDirectory -v $dotnetVerbosity
dotnet restore $buildProjectPath -v $dotnetVerbosity

$solutionPath = Join-Path -Path $rootDirectory -ChildPath "$solutionName.sln"
$outputFilePath = Join-Path -Path $rootDirectory -ChildPath "nuget_packages.txt"

Write-Host "Deleting existing package list file at '$outputFilePath' ..."
DeleteFolder $outputFilePath

Write-Host "Generating package list for solution '$solutionPath' ..."
dotnet list $solutionPath package --include-transitive | Out-File -FilePath $outputFilePath -Encoding UTF8

Write-Host "Rebuilding projects ..."
dotnet build $rootDirectory -v $dotnetVerbosity

Write-Host "Done."
