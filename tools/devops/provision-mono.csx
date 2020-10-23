#load "provision-shared.csx"

// Provision Mono
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.

Console.WriteLine ($"Provisioning mono from {provision_from_commit}...");

var mono_version_file = "/Library/Frameworks/Mono.framework/Versions/Current/VERSION";
var min_mono_version = FindVariable("MIN_MONO_VERSION");
var max_mono_version = FindVariable("MAX_MONO_VERSION");
Console.WriteLine($"min_mono_version: {min_mono_version}");
Console.WriteLine($"max_mono_version: {max_mono_version}");

if (File.Exists(mono_version_file))
{
    var current_mono_version = Exec("cat", mono_version_file)[0];
    Console.WriteLine($"current_mono_version: {current_mono_version}");

    // is_at_least_version $ACTUAL_MONO_VERSION $MIN_MONO_VERSION
    var currentWithMin = IsAtLeastVersion(current_mono_version, min_mono_version);
    var currentWithMax = IsAtLeastVersion(current_mono_version, max_mono_version);

    Console.WriteLine($"IsAtLeastVersion: Current version '{current_mono_version}' compared with minimum version '{min_mono_version}': {currentWithMin}");
    Console.WriteLine($"IsAtLeastVersion: Current version '{current_mono_version}' compared with maximum version '{max_mono_version}': {currentWithMax}");
}
else
{
    var mono_url = FindVariable("MIN_MONO_URL");
    Item(mono_url);
}
