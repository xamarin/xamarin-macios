#load "provision-shared.csx"

// Provision Xcode
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.

var vs_install_url = FindVariable("MIN_VISUAL_STUDIO_URL");
var vs_install_path = "/Applications/Visual Studio.app";

Console.WriteLine($"vs_install_url: {vs_install_url}");
Console.WriteLine($"vs_install_path: {vs_install_path}");

if (Directory.Exists(vs_install_path))
{
    var vs_version_min = FindVariable("MIN_VISUAL_STUDIO_VERSION");
    var vs_version_max = FindVariable("MAX_VISUAL_STUDIO_VERSION");

    Console.WriteLine($"vs_version_min: {vs_version_min}");
    Console.WriteLine($"vs_version_max: {vs_version_max}");

    var vs_version_current = string.Empty;

    var sourceDirectory = Env("BUILD_SOURCESDIRECTORY");
    Console.WriteLine($"BUILD_SOURCESDIRECTORY: {sourceDirectory}");

    var vsMacVersionScript = Path.Combine(sourceDirectory, "tools", "devops", "automation", "VSMacVersion.ps1");
    var lines = Exec("pwsh", vsMacVersionScript);
    if (lines != null && lines.Count > 0)
    {
        vs_version_current = lines[0];
    }

    Console.WriteLine($"vs_version_current: {vs_version_current}");
}
else
{
    Console.WriteLine("Installing Visual Studio");
    Item(vs_install_url);
}


