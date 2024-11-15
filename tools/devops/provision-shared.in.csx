using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Xamarin.Provisioning;
using Xamarin.Provisioning.Model;
using Xamarin.Provisioning.IO;

var commit = Environment.GetEnvironmentVariable ("BUILD_SOURCEVERSION");
var provision_from_commit = Environment.GetEnvironmentVariable ("PROVISION_FROM_COMMIT") ?? commit;

void RemoveXcodeSymlinks (string xcodePath)
{
	Console.WriteLine ($"Checking if '{xcodePath}' is a symlink...");
	var resolvedPath = Symlink.Resolve (xcodePath);
	Console.WriteLine ($"Path resolved: '{resolvedPath}'");
	if (resolvedPath is string) {
		Console.WriteLine ($"Removing '{xcodePath}' symlink.");
		Symlink.Delete (xcodePath);
		Console.WriteLine ($"Renaming '{resolvedPath}' into '{xcodePath}'");
		ElevatedExec ("/bin/mv", resolvedPath, xcodePath);
	} else
		Console.WriteLine ($"'{xcodePath}' is not a symlink.");
}

void ListXcodes ()
{
	Console.WriteLine ($"Xcodes:");
	var lines = Exec ("bash", "-c", "ls -lad /Applications/Xcode*");
	foreach (var line in lines)
		Console.WriteLine ($"\t{line}");
}

// Provision Xcode using the xip name declared in Make.config
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.
bool ProvisionXcode ()
{

	if (string.IsNullOrEmpty (provision_from_commit)) {
		Console.Error.WriteLine ($"Either BUILD_SOURCEVERSION or PROVISION_FROM_COMMIT must be set.");
		Environment.Exit (1);
		return false;
	}

	ListXcodes ();

	// Provision Xcode
	Console.WriteLine ($"Provisioning Xcode from {provision_from_commit}...");

	// Let's turn symlink into an actual path
	var reqXcode = Path.GetDirectoryName (Path.GetDirectoryName ("@XCODE_ROOT_PATH@"));
	RemoveXcodeSymlinks (reqXcode);

	// Provision Xcode using the xip name declared in Make.config
	Xcode ("@XCODE_XIP_NAME@").XcodeSelect (allowUntrusted: true);

	LogInstalledXcodes ();
	return true;
}

void ProvisionBrewPackages ()
{
	BrewPackages (
		"shellcheck",
		"yamllint",
		"p7zip",
		"azure-cli"
	 );
}

void SetDefaultXcodeInVSMac ()
{
	var appleSdkOverride = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Preferences", "Xamarin", "Settings.plist");
	Item ("Override Apple SDK Settings")
		.Condition (item => !File.Exists (appleSdkOverride) || GetSettingValue (appleSdkOverride, "AppleSdkRoot") != GetSelectedXcodePath ())
		.Action (item => {
			DeleteSafe (appleSdkOverride);
			CreateSetting (appleSdkOverride, "AppleSdkRoot", GetSelectedXcodePath ());
			Console.WriteLine ($"New VSMac iOS SDK Location: {GetSelectedXcodePath ()}");
		});

}

void DeleteSafe (string file)
{
	if (File.Exists (file))
		File.Delete (file);
}

void CreateSetting (string settingFile, string key, string value)
{
	Exec ("defaults", "write", settingFile, key, value);
}

string GetSettingValue (string settingFile, string keyName)
{
	return Exec ("defaults", "read", settingFile, keyName).FirstOrDefault ();
}

void ProvisionMono ()
{
	Item ("@MONO_PACKAGE@");
}

void ProvisionSharpie ()
{
	Item ("@MIN_SHARPIE_URL@");
}
