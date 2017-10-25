var target = Argument("Target", "Default");

var configuration = 
    HasArgument("Configuration") 
        ? Argument<string>("Configuration") 
        : EnvironmentVariable("Configuration") ?? "Release";

var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 0;

var version = HasArgument("ShortVersion") ? Argument<string>("ShortVersion") : EnvironmentVariable("ShortVersion");
version = !string.IsNullOrWhiteSpace(version) ? version : "1.0.0";
var assemblyVersion = $"{version}.{buildNumber}";
var versionSuffix = HasArgument("VersionSuffix") ? Argument<string>("VersionSuffix") : EnvironmentVariable("VersionSuffix");
var packageVersion = $"{version}.{buildNumber}" + (!string.IsNullOrWhiteSpace(versionSuffix) ? $"-{versionSuffix}" : "");
 
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));
 
Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });
 
 Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        var projects = GetFiles("../**/*.csproj");
        foreach(var project in projects)
        {
           DotNetCoreBuild(
                project.GetDirectory().FullPath,
                new DotNetCoreBuildSettings()
                {
                    Configuration = configuration,
                    ArgumentCustomization = args => args
                        .Append($"/p:Version={version}")
                        .Append($"/p:AssemblyVersion={assemblyVersion}")
                });
        }
    });
	
RunTarget(target);