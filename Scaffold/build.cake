using System.IO.Compression;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Publish");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./Artifacts";
string auditArtifacts = $"{artifacts}/Audit";
string buildArtifacts = $"{artifacts}/Release";
string testArtifacts = $"{artifacts}";

string projectName = "Scaffold";
string solution = "./Scaffold.sln";
string configuration = "Release";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Audit")
    .Description("Produces a \"Software Bill of Materials\" in CycloneDX format.")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories(auditArtifacts), deleteSettings);

        DotNetTool("CycloneDX", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append(solution)
                .Append($"--json")
                .Append($"--out {auditArtifacts}")
        });
    });

Task("Clean")
    .Description("Executes 'dotnet clean'.")
    .Does(() =>
    {
        DotNetClean(solution, new DotNetCleanSettings
        {
            Configuration = configuration
        });
    });

Task("Restore")
    .Description("Executes 'dotnet restore'.")
    .Does(() =>
    {
        DotNetRestore(solution);
    });

Task("Build")
    .Description("Executes 'dotnet build'.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(solution, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("Test")
    .Description("Executes 'dotnet test' and produces code coverage reports.")
    .IsDependentOn("Build")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories("./Tests/**/TestResults"), deleteSettings);

        DotNetTestSettings testSettings = new DotNetTestSettings
        {
            ArgumentCustomization = args => args
                .Append("--collect:\"XPlat Code Coverage\"")
                .Append("--logger:\"console;verbosity=minimal\"")
                .Append("--logger:\"trx;\"")
                .Append("--nologo"),
            Configuration = configuration,
            NoBuild = true
        };

        DotNetTest(solution, testSettings);

        string coverageHistory = $"{testArtifacts}/CoverageHistory";
        string coverageReports = $"{testArtifacts}/CoverageReports";
        string reportTypes = "Html;Cobertura;";

        DeleteDirectories(GetDirectories(coverageReports), deleteSettings);

        // Coverage Report - Combined (Integration + Unit) Tests
        DotNetTool("reportgenerator", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append($"-reports:\"./Tests/**/TestResults/*/coverage.cobertura.xml\"")
                .Append($"-reporttypes:\"{reportTypes}\"")
                .Append($"-targetdir:\"{coverageReports}/Combined\"")
                .Append($"-historydir:\"{coverageHistory}/Combined\"")
                .Append($"-title:\"{projectName} Combined (Integration + Unit) Tests\"")
                .Append($"-verbosity:\"Error\"")
        });

        // Coverage Report - Integration Tests
        DotNetTool("reportgenerator", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append($"-reports:\"./Tests/IntegrationTests/**/TestResults/*/coverage.cobertura.xml\"")
                .Append($"-reporttypes:\"{reportTypes}\"")
                .Append($"-targetdir:\"{coverageReports}/IntegrationTests\"")
                .Append($"-historydir:\"{coverageHistory}/IntegrationTests\"")
                .Append($"-title:\"{projectName} Integration Tests\"")
                .Append($"-verbosity:\"Error\"")
        });

        // Coverage Report - Unit Tests
        DotNetTool("reportgenerator", new DotNetToolSettings
        {
            ArgumentCustomization = args => args
                .Append($"-reports:\"./Tests/UnitTests/**/TestResults/*/coverage.cobertura.xml\"")
                .Append($"-reporttypes:\"{reportTypes}\"")
                .Append($"-targetdir:\"{coverageReports}/UnitTests\"")
                .Append($"-historydir:\"{coverageHistory}/UnitTests\"")
                .Append($"-title:\"{projectName} Unit Tests\"")
                .Append($"-verbosity:\"Error\"")
        });
    });

Task("Publish")
    .Description("Produces a release ready build.")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories($"{buildArtifacts}"), deleteSettings);

        DotNetPublishSettings settings = new DotNetPublishSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{buildArtifacts}/Scaffold.WebApi"
        };

        DotNetPublish("./Sources/Scaffold.WebApi", settings);

        ZipFile.CreateFromDirectory(
            sourceDirectoryName: $"{settings.OutputDirectory}",
            destinationArchiveFileName: $"{settings.OutputDirectory}.zip",
            compressionLevel: CompressionLevel.SmallestSize,
            includeBaseDirectory: false);

        DeleteDirectories(GetDirectories(settings.OutputDirectory.ToString()), deleteSettings);
    });

Task("Containerize")
    .Description("Produces a release ready container image.")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        ZipFile.ExtractToDirectory(
            sourceArchiveFileName: $"{buildArtifacts}/Scaffold.WebApi.zip",
            destinationDirectoryName: $"{buildArtifacts}/Scaffold.WebApi",
            overwriteFiles: true);

        StartProcess("docker", new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("build")
                .Append("--force-rm")
                .Append("--pull")
                .Append("--tag scaffold:latest")
                .Append($"{buildArtifacts}/Scaffold.WebApi")
        });
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
