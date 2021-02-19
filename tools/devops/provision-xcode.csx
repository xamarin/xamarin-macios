#load "provision-shared.csx"

// Provision Xcode
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.

void ListXcodes ()
{
	Console.WriteLine ($"Xcodes:");
	var lines = Exec ("bash", "-c", "ls -lad /Applications/Xcode*");
	foreach (var line in lines)
		Console.WriteLine ($"\t{line}");
}

if (string.IsNullOrEmpty (provision_from_commit)) {
	Console.Error.WriteLine ($"Either BUILD_SOURCEVERSION or PROVISION_FROM_COMMIT must be set.");
	Environment.Exit (1);
	return 1;
}

ListXcodes ();

// Provision Xcode
Console.WriteLine ($"Provisioning Xcode from {provision_from_commit}...");
var xcode_path = Path.GetDirectoryName (Path.GetDirectoryName (FindVariable ("XCODE_DEVELOPER_ROOT")));
if (!Directory.Exists (xcode_path)) {
	// Provision
	var root = Path.GetDirectoryName (Path.GetDirectoryName (FindVariable ("XCODE_DEVELOPER_ROOT")));
	var url = FindVariable ("XCODE_URL");
	Console.WriteLine ($"Could not find an already installed Xcode in {root}, will download and install from {url}.");
	var xcode_provisionator_name = Path.GetFileNameWithoutExtension (url).Substring (6); // Strip off 'Xcode_' from the start of the filename
	Xcode (xcode_provisionator_name)
		.XcodeSelect ()
		.Action ((v) => {
			ExecVerbose ("ln", "-Fhs", "/Applications/Xcode.app", root);
			ListXcodes ();
		});
} else {
	// We already have it, symlink into /Applications/Xcode.app
	Console.WriteLine ($"The required Xcode is already installed.");
	ExecVerbose ("ln", "-Fhs", xcode_path, "/Applications/Xcode.app");
}

var appleSdkOverride = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Library", "Preferences", "Xamarin", "Settings.plist");
Item ("Override Apple SDK Settings")
	.Condition (item => !File.Exists (appleSdkOverride) || GetSettingValue (appleSdkOverride, "AppleSdkRoot") != GetSelectedXcodePath ())
	.Action (item => {
		DeleteSafe (appleSdkOverride);
		CreateSetting (appleSdkOverride, "AppleSdkRoot", GetSelectedXcodePath ());
		Console.WriteLine ($"New VSMac iOS SDK Location: {GetSelectedXcodePath ()}");
	});

LogInstalledXcodes ();
