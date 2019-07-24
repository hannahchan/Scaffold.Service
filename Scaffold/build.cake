#addin nuget:?package=Cake.Coverlet
#tool "nuget:?package=ReportGenerator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Default");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./artifacts";
string configuration = "Release";
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
        string testArtifacts = $"{artifacts}/Test";

        if (DirectoryExists($"{testArtifacts}/CoverageReport"))
        {
            DeleteDirectory($"{testArtifacts}/CoverageReport", new DeleteDirectorySettings { Force = true, Recursive = true });
        }

        if (DirectoryExists($"{testArtifacts}/OpenCover"))
        {
            DeleteDirectory($"{testArtifacts}/OpenCover", new DeleteDirectorySettings { Force = true, Recursive = true });
        }

        DotNetCoreTestSettings testSettings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true
        };

        List<String> exclude = new List<string>()
        {
            $"[Scaffold.Repositories.EntityFrameworkCore]Scaffold.Repositories.EntityFrameworkCore.Migrations.*",
            $"[Scaffold.WebApi]Scaffold.WebApi.Program",
            $"[Scaffold.WebApi]Scaffold.WebApi.Startup*"
        };

        foreach (FilePath filePath in GetFiles("./Tests/**/*.UnitTests.csproj"))
        {
            string projectName = filePath.GetFilename().ToString().Replace(".UnitTests.csproj", string.Empty);

            CoverletSettings coverletSettings = new CoverletSettings
            {
                CollectCoverage = true,
                CoverletOutputDirectory = $"{testArtifacts}/OpenCover",
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputName = $"{projectName}.xml",
                Exclude = exclude,
                Include = new List<string>() { $"[{projectName}]*" }
            };

            DotNetCoreTest(filePath, testSettings, coverletSettings);
        };

        ReportGeneratorSettings reportGeneratorSettings = new ReportGeneratorSettings
        {
            HistoryDirectory = $"{testArtifacts}/CoverageHistory"
        };

        ReportGenerator($"{testArtifacts}/OpenCover/*.xml", $"{testArtifacts}/CoverageReport", reportGeneratorSettings);
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        string buildArtifacts = $"{artifacts}/Build";

        if (DirectoryExists(buildArtifacts))
        {
            DeleteDirectory(buildArtifacts, new DeleteDirectorySettings { Force = true, Recursive = true });
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
