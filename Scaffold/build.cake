//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var artifacts = Directory("./artifacts");
var solution = File("./Scaffold.WebApi.sln");

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
        Configuration = "Release"
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
        Configuration = "Release",
        NoRestore = true
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest(solution, new DotNetCoreTestSettings
    {
        Configuration = "Release",
        NoBuild = true
    });
});

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
    {
        Configuration = "Release",
        NoBuild = true,
        OutputDirectory = Directory($"{artifacts}/Scaffold.WebApi")
    };

    DotNetCorePublish("./Sources/Scaffold.WebApi", settings);
    Zip(settings.OutputDirectory, File($"{settings.OutputDirectory}.zip"));
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
