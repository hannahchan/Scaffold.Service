//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Publish");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./Artifacts";
string buildArtifacts = $"{artifacts}/Build";
string testArtifacts = $"{artifacts}/Test";

string projectName = "Scaffold";
string solution = "./Scaffold.sln";
string configuration = "Release";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        DotNetCoreClean(solution, new DotNetCoreCleanSettings
        {
            Configuration = configuration
        });
    });

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
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
        Zip(settings.OutputDirectory, $"{settings.OutputDirectory}.zip");
        DeleteDirectories(GetDirectories(settings.OutputDirectory.ToString()), deleteSettings);
    });

Task("Containerize")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        Unzip($"{buildArtifacts}/Scaffold.WebApi.zip", $"{buildArtifacts}/Scaffold.WebApi");

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
