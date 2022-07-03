//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

string target = Argument("Target", "Pack");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////

string artifacts = "./artifacts";
string releaseArtifacts = $"{artifacts}/Release";

string project = "./Scaffold.Service.csproj";
string configuration = "Release";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Test")
    .Description("Executes 'dotnet test' in template directory.")
    .Does(() =>
    {
        DotNetTest("./Scaffold", new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = false
        });
    });

Task("Clean")
    .Description("Executes 'git clean -dxf' to clean template directory.")
    .Does(() =>
    {
        StartProcess("git", new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("clean")
                .Append("-dxf")
        });
    });

Task("Pack")
    .Description("Produces a release ready package.")
    .IsDependentOn("Test")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetPack(project, new DotNetPackSettings
        {
            Configuration = configuration,
            NoBuild = false,
            OutputDirectory = releaseArtifacts
        });
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
