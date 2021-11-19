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

        DotNetCoreTool("CycloneDX", new DotNetCoreToolSettings
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
        DotNetCoreClean(solution, new DotNetCoreCleanSettings
        {
            Configuration = configuration
        });
    });

Task("Restore")
    .Description("Executes 'dotnet restore'.")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
    .Description("Executes 'dotnet build'.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
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

        DotNetCoreTestSettings testSettings = new DotNetCoreTestSettings
        {
            ArgumentCustomization = args => args
                .Append("--collect:\"XPlat Code Coverage\"")
                .Append("--logger:\"console;verbosity=minimal\"")
                .Append("--logger:\"trx;\"")
                .Append("--nologo"),
            Configuration = configuration,
            NoBuild = true
        };

        DotNetCoreTest(solution, testSettings);

        string coverageHistory = $"{testArtifacts}/CoverageHistory";
        string coverageReports = $"{testArtifacts}/CoverageReports";
        string reportTypes = "Html;Cobertura;";

        DeleteDirectories(GetDirectories(coverageReports), deleteSettings);

        // Coverage Report - Combined (Integration + Unit) Tests
        DotNetCoreTool("reportgenerator", new DotNetCoreToolSettings
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
        DotNetCoreTool("reportgenerator", new DotNetCoreToolSettings
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
        DotNetCoreTool("reportgenerator", new DotNetCoreToolSettings
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

        DotNetCorePublishSettings settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{buildArtifacts}/Scaffold.WebApi"
        };

        DotNetCorePublish("./Sources/Scaffold.WebApi", settings);
    });

Task("Containerize")
    .Description("Produces a release ready container image.")
    .IsDependentOn("Publish")
    .Does(() =>
    {
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
