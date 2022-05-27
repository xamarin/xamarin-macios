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

bool IsAtLeastVersion(string actualVer, string minVer)
{
    if (actualVer.Equals(minVer, StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    var actualVerChars = actualVer.ToCharArray();
    var minVerChars = minVer.ToCharArray();

    var length = Math.Min (minVerChars.Length, actualVerChars.Length);

    var i = 0;
    while (i < length)
    {
        if (actualVerChars[i] > minVerChars[i])
        {
            return true;
        }
        else if (minVerChars[i] > actualVerChars[i])
        {
            return false;
        }
        i++;
    }

    if (actualVerChars.Length == minVerChars.Length)
    {
        return true;
    }

    return actualVerChars.Length > minVerChars.Length;
}

void RemoveXcodeSymlinks (string xcodePath)
{
	Console.WriteLine ($"Checkig if '{xcodePath}' is a symlink...");
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
