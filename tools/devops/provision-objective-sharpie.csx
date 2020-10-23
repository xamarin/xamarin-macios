#load "provision-shared.csx"

// Provision Objective Sharpie
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.

Console.WriteLine ($"Provisioning objective sharpie from {provision_from_commit}...");

var sharpie_version_file = "/Library/Frameworks/ObjectiveSharpie.framework/Versions/Current/Version";
var min_sharpie_version = FindVariable("MIN_SHARPIE_VERSION");
var max_sharpie_version = FindVariable("MIN_SHARPIE_VERSION");
Console.WriteLine($"min_sharpie_version: {min_sharpie_version}");
Console.WriteLine($"max_sharpie_version: {max_sharpie_version}");

if (File.Exists(sharpie_version_file))
{
    var current_sharpie_version = Exec("cat", sharpie_version_file)[0];
    Console.WriteLine($"current_sharpie_version: {current_sharpie_version}");

    var currentWithMin = IsAtLeastVersion(current_sharpie_version, min_sharpie_version);
    var currentWithMax = IsAtLeastVersion(current_sharpie_version, max_sharpie_version);

    Console.WriteLine($"IsAtLeastVersion: Current version '{current_sharpie_version}' compared with minimum version '{min_sharpie_version}': {currentWithMin}");
    Console.WriteLine($"IsAtLeastVersion: Current version '{current_sharpie_version}' compared with maximum version '{max_sharpie_version}': {currentWithMax}");
}
else
{
    var sharpie_url = FindVariable("MIN_SHARPIE_URL");
    Console.WriteLine($"Provisioning Objective Sharpie: {sharpie_url}");
    Item(sharpie_url);
}
