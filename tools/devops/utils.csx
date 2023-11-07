using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

string DownloadWithGithubAuth (string uri)
{
	var downloader = new Downloader ();
	var path = Path.GetTempFileName ();
	var headers = new List<(string, string)> ();
	var authToken = AuthToken ("github.com");
	if (!string.IsNullOrEmpty (authToken))
		headers.Add (("Authorization", $"token {authToken}"));
	path = downloader
		.DownloadItemAsync (
			uri,
			headers.ToArray (),
			Path.GetDirectoryName (path),
			Path.GetFileName (path),
			options: Downloader.Options.Default.WithUseCache (false))
		.GetAwaiter ()
		.GetResult ();
	try {
		return File.ReadAllText (path);
	} finally {
		File.Delete (path);
	}
}

string manifest_url = null;
string GetManifestUrl (string hash)
{
	var page = 1;
	var hasContent = true;
	while (manifest_url == null && hasContent) {
		var url = $"https://api.github.com/repos/xamarin/xamarin-macios/statuses/{hash}?page={page}";
		var json = (JArray) JToken.Parse (DownloadWithGithubAuth (url));
		hasContent &= json.HasValues;
		if (hasContent) {
			manifest_url = (string) ((JValue) json.Where ((v) => v ["context"].ToString () == "vsts-devdiv artifacts").Select ((v) => v ["target_url"]).FirstOrDefault ())?.Value;
		}
		page++;
	}

	if (manifest_url == null)
		throw new Exception ($"Could not find the manifest for {hash}. Is the commit already built by CI?");
	return manifest_url;
}

string[] manifest = null;
string[] GetManifest (string hash)
{
	// The manifest is gone but we can fake it until we make it using the "vsts-devdiv artifacts"
	if (manifest == null) {
		var artifactsUrl = GetManifestUrl (hash);
		var artifactsjson = (JArray) JToken.Parse (DownloadWithGithubAuth (artifactsUrl));
		manifest = artifactsjson.Select ((v) => (string) v ["url"])?.ToArray ();
	}
	return manifest;
}

// Looks for a variable either in the environment, or in current repo's Make.config.
// Returns null if the variable couldn't be found.
IEnumerable<string> make_config = null;
string FindConfigurationVariable (string variable, string hash = "HEAD")
{
	var value = Environment.GetEnvironmentVariable (variable);
	if (!string.IsNullOrEmpty (value))
		return value;

	if (make_config == null)
		make_config = Exec ("git", "show", $"{hash}:Make.config");
	foreach (var line in make_config) {
		if (line.StartsWith (variable + "=", StringComparison.Ordinal))
			return line.Substring (variable.Length + 1);
	}

	return null;
}

void InstallPackage (string name, string url)
{
	Console.WriteLine ($"Installing {name} from {url}");
	var version = Regex.Match (url, "[0-9]+[.][0-9]+[.][0-9]+([.][0-9]+)?").Value;
	Item (name, version).Source (url);
}
