#tool "nuget:?package=ReportGenerator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string configuration = Argument("Configuration", "Debug");
string target = Argument("Target", "Default");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./Artifacts";
string buildArtifacts = $"{artifacts}/Build";
string testArtifacts = $"{artifacts}/Test";

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

        ReportGeneratorSettings reportGeneratorSettings = new ReportGeneratorSettings
        {
            Verbosity = ReportGeneratorVerbosity.Error
        };

        reportGeneratorSettings.HistoryDirectory = $"{coverageHistory}/Combined";
        ReportGenerator(
            $"./Tests/**/coverage.cobertura.xml",
            $"{coverageReports}/Combined",
            reportGeneratorSettings);

        reportGeneratorSettings.HistoryDirectory = $"{coverageHistory}/IntegrationTests";
        ReportGenerator(
            $"./Tests/**/*.IntegrationTests/**/coverage.cobertura.xml",
            $"{coverageReports}/IntegrationTests",
            reportGeneratorSettings);

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
        if (DirectoryExists(buildArtifacts))
        {
            DeleteDirectory(buildArtifacts, new DeleteDirectorySettings
            {
                Force = true,
                Recursive = true
            });
        }

        DotNetCorePublishSettings settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{buildArtifacts}/Scaffold.WebApi"
        };

        DotNetCorePublish("./Sources/Scaffold.WebApi", settings);
        Zip(settings.OutputDirectory, $"{settings.OutputDirectory}.zip");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
