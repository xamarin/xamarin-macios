using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Xamarin.Provisioning;
using Xamarin.Provisioning.Model;

// Provision Xcode
//
// Overrides:
// * The current commit can be overridden by setting the PROVISION_FROM_COMMIT variable.

var commit = Environment.GetEnvironmentVariable ("BUILD_SOURCEVERSION");
var provision_from_commit = Environment.GetEnvironmentVariable ("PROVISION_FROM_COMMIT") ?? commit;

// Looks for a variable either in the environment, or in current repo's Make.config.
// Returns null if the variable couldn't be found.
IEnumerable<string> make_config = null;
string FindConfigurationVariable (string variable, string hash = "HEAD")
{
	var value = Environment.GetEnvironmentVariable (variable);
	if (!string.IsNullOrEmpty (value))
		return value;

	if (make_config == null) {
		try {
			make_config = Exec ("git", "show", $"{hash}:Make.config");
		} catch {
			Console.WriteLine ("Could not find a Make.config");
			return null;	
		}
	}
	foreach (var line in make_config) {
		if (line.StartsWith (variable + "=", StringComparison.Ordinal))
			return line.Substring (variable.Length + 1);
	}

	return null;
}

string FindVariable (string variable)
{
	var value = FindConfigurationVariable (variable, provision_from_commit);
	if (!string.IsNullOrEmpty (value))
		return value;

	throw new Exception ($"Could not find {variable} in environment nor in the commit's ({commit}) manifest.");
}

void ExecVerbose (string filename, params string[] args)
{
	Console.WriteLine ($"{filename} {string.Join (" ", args)}");
	Exec (filename, args);
}

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
