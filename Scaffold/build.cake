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
        if (DirectoryExists(artifacts))
        {
            DeleteDirectory(artifacts, new DeleteDirectorySettings
            {
                Force = true,
                Recursive = true
            });
        }

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
        DotNetCoreTestSettings testSettings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true
        };

        List<String> exclude = new List<string>()
        {
            $"[Scaffold.Data]Scaffold.Data.Migrations.*"
        };

        string testArtifacts = $"{artifacts}/Test";

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

        ReportGenerator($"{testArtifacts}/OpenCover/*.xml", $"{testArtifacts}/CoverageReport");
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCorePublishSettings settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoBuild = true,
            OutputDirectory = $"{artifacts}/Build/Scaffold.WebApi"
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
