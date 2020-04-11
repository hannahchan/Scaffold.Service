#tool "nuget:?package=ReportGenerator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string configuration = Argument("Configuration", "Debug");
string target = Argument("Target", "Publish");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./Artifacts";
string buildArtifacts = $"{artifacts}/Build";
string testArtifacts = $"{artifacts}/Test";

string projectName = "Scaffold";
string solution = "./Scaffold.WebApi.sln";

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
            ArgumentCustomization = args =>
                args.Append("--collect:\"XPlat Code Coverage\""),
            Configuration = configuration,
            NoBuild = true
        };

        DotNetCoreTest(solution, testSettings);

        string coverageHistory = $"{testArtifacts}/CoverageHistory";
        string coverageReports = $"{testArtifacts}/CoverageReports";

        DeleteDirectories(GetDirectories(coverageReports), deleteSettings);

        string reportTitle = string.Empty;

        ReportGeneratorSettings reportGeneratorSettings = new ReportGeneratorSettings
        {
            ArgumentCustomization = args =>
                args.Append($"--title:\"{reportTitle}\""),
            Verbosity = ReportGeneratorVerbosity.Error
        };

        reportTitle = $"{projectName} Combined (Integration + Unit) Tests";
        reportGeneratorSettings.HistoryDirectory = $"{coverageHistory}/Combined";
        ReportGenerator(
            $"./Tests/**/coverage.cobertura.xml",
            $"{coverageReports}/Combined",
            reportGeneratorSettings);

        reportTitle = $"{projectName} Integration Tests";
        reportGeneratorSettings.HistoryDirectory = $"{coverageHistory}/IntegrationTests";
        ReportGenerator(
            $"./Tests/**/*.IntegrationTests/**/coverage.cobertura.xml",
            $"{coverageReports}/IntegrationTests",
            reportGeneratorSettings);

        reportTitle = $"{projectName} Unit Tests";
        reportGeneratorSettings.HistoryDirectory = $"{coverageHistory}/UnitTests";
        ReportGenerator(
            $"./Tests/**/*.UnitTests/**/coverage.cobertura.xml",
            $"{coverageReports}/UnitTests",
            reportGeneratorSettings);
    });

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DeleteDirectorySettings deleteSettings = new DeleteDirectorySettings
        {
            Force = true,
            Recursive = true,
        };

        DeleteDirectories(GetDirectories(buildArtifacts), deleteSettings);

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

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
