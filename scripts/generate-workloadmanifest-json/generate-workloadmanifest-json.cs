// arguments are: <platform> <outputPath>

using System.IO;
using System.Xml;

var expectedArgumentCount = 9;
if (args.Length != expectedArgumentCount) {
	Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {args.Length}");
	return 1;
}

var argumentIndex = 0;
var platform = args [argumentIndex++];
var version = args [argumentIndex++];
var net8Version = args [argumentIndex++];
var runtimeIdentifiers = args [argumentIndex++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
var outputPath = args [argumentIndex++];
var windowsPlatforms = args [argumentIndex++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
var hasWindows = Array.IndexOf (windowsPlatforms, platform) >= 0;
var currentApiVersion = args [argumentIndex++];
var supportedApiVersions = args [argumentIndex++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
var versionsPropsPath = args [argumentIndex++];

var platformLowerCase = platform.ToLowerInvariant ();

var allApiVersions = new List<string> (supportedApiVersions);
allApiVersions = allApiVersions.Select (v => v.Replace ('-', '_')).ToList ();

var versionsPropsTable = File.ReadAllLines (versionsPropsPath).
				Where (v => v.Count (f => f == '>') > 1).
				Select (v => {
					var split = v.Trim ().Split (new char [] { '<', '>', '/' }, StringSplitOptions.RemoveEmptyEntries);
					var name = split [0];
					var value = split [1];
					return new Tuple<string, string> (name, value);
				}).
				ToDictionary (v => v.Item1, v => v.Item2, StringComparer.OrdinalIgnoreCase);

var sortedAllApiVersions = allApiVersions.
				Select (v => {
					v = v.Replace ("net", "");
					v = v [0..v.IndexOf ('_')];
					return v;
				}).
				Select (Version.Parse).
				Distinct ().
				OrderBy (v => v).
				ToArray ();
var earliestDotNetVersion = sortedAllApiVersions.First ().Major;
var latestDotNetVersion = sortedAllApiVersions.Last ().Major;

var failed = false;
using (TextWriter writer = new StreamWriter (outputPath)) {
	writer.WriteLine ($"{{");
	writer.WriteLine ($"	\"version\": \"{version}\",");
	writer.WriteLine ($"	\"workloads\": {{");
	writer.WriteLine ($"		\"{platformLowerCase}\": {{");
	writer.WriteLine ($"			\"description\": \".NET SDK Workload for building {platform} applications.\",");
	writer.WriteLine ($"			\"packs\": [");
	foreach (var tfm in allApiVersions) {
		writer.WriteLine ($"				\"Microsoft.{platform}.Sdk.{(tfm == "net8.0" ? "net8" : tfm)}\",");
	}
	if (hasWindows) {
		foreach (var tfm in allApiVersions) {
			writer.WriteLine ($"				\"Microsoft.{platform}.Windows.Sdk.Aliased.{(tfm == "net8.0" ? "net8" : tfm)}\",");
		}
	}
	writer.WriteLine ($"				\"Microsoft.{platform}.Ref.{currentApiVersion}\",");
	foreach (var rid in runtimeIdentifiers) {
		writer.WriteLine ($"				\"Microsoft.{platform}.Runtime.{rid}.{currentApiVersion}\",");
	}
	writer.WriteLine ($"				\"Microsoft.{platform}.Templates.net9\"");
	writer.WriteLine ($"			],");
	writer.WriteLine ($"			\"extends\": [");
	if (platform == "macOS") {
		writer.WriteLine ($"				\"microsoft-net-runtime-mono-tooling\",");
		for (var i = earliestDotNetVersion; i < latestDotNetVersion; i++)
			writer.WriteLine ($"				\"microsoft-net-runtime-mono-tooling-net{i}\",");
	} else {
		writer.WriteLine ($"				\"microsoft-net-runtime-{platformLowerCase}\",");
		for (var i = earliestDotNetVersion; i < latestDotNetVersion; i++)
			writer.WriteLine ($"				\"microsoft-net-runtime-{platformLowerCase}-net{i}\",");
	}
	writer.WriteLine ($"			]");
	writer.WriteLine ($"		}},");
	writer.WriteLine ($"	}},");
	writer.WriteLine ($"	\"packs\": {{");
	foreach (var tfmVersion in allApiVersions) {
		string? apiVersion = null;
		var tfm = tfmVersion;
		if (tfm == currentApiVersion) {
			apiVersion = version;
		} else if (tfm == "net8.0") {
			apiVersion = net8Version;
		} else {
			var propsPackageName = $"Microsoft{platform}Sdk" + tfm.Replace ("-", "").Replace (".", "") + "PackageVersion";
			if (!versionsPropsTable.TryGetValue (propsPackageName, out apiVersion)) {
				Console.Error.WriteLine ($"❌ Unable to find a package version for {platform}/{tfm} in {versionsPropsPath}. Package name: {propsPackageName}");
				apiVersion = "?";
				failed = true;
			}
		}
		writer.WriteLine ($"		\"Microsoft.{platform}.Sdk.{(tfm == "net8.0" ? "net8" : tfm)}\": {{");
		writer.WriteLine ($"			\"kind\": \"sdk\",");
		writer.WriteLine ($"			\"version\": \"{apiVersion}\",");
		if (tfm == "net8.0") {
			writer.WriteLine ($"			\"alias-to\": {{");
			writer.WriteLine ($"				\"any\": \"Microsoft.{platform}.Sdk\"");
			writer.WriteLine ($"			}}");
		}
		writer.WriteLine ($"		}},");
		if (hasWindows) {
			writer.WriteLine ($"		\"Microsoft.{platform}.Windows.Sdk.Aliased.{(tfm == "net8.0" ? "net8" : tfm)}\": {{");
			writer.WriteLine ($"			\"kind\": \"sdk\",");
			writer.WriteLine ($"			\"version\": \"{apiVersion}\",");
			writer.WriteLine ($"			\"alias-to\": {{");
			if (tfm == "net8.0") {
				writer.WriteLine ($"				\"win-x64\": \"Microsoft.{platform}.Windows.Sdk\",");
				writer.WriteLine ($"				\"win-x86\": \"Microsoft.{platform}.Windows.Sdk\",");
				writer.WriteLine ($"				\"win-arm64\": \"Microsoft.{platform}.Windows.Sdk\",");
			} else {
				writer.WriteLine ($"				\"win-x64\": \"Microsoft.{platform}.Windows.Sdk.{tfm}\",");
				writer.WriteLine ($"				\"win-x86\": \"Microsoft.{platform}.Windows.Sdk.{tfm}\",");
				writer.WriteLine ($"				\"win-arm64\": \"Microsoft.{platform}.Windows.Sdk.{tfm}\",");
			}
			writer.WriteLine ($"			}}");
			writer.WriteLine ($"		}},");
		}
	}
	writer.WriteLine ($"		\"Microsoft.{platform}.Ref.{currentApiVersion}\": {{");
	writer.WriteLine ($"			\"kind\": \"framework\",");
	writer.WriteLine ($"			\"version\": \"{version}\"");
	writer.WriteLine ($"		}},");
	foreach (var rid in runtimeIdentifiers) {
		writer.WriteLine ($"		\"Microsoft.{platform}.Runtime.{rid}.{currentApiVersion}\": {{");
		writer.WriteLine ($"			\"kind\": \"framework\",");
		writer.WriteLine ($"			\"version\": \"{version}\"");
		writer.WriteLine ($"		}},");
	}
	writer.WriteLine ($"		\"Microsoft.{platform}.Templates.net9\": {{");
	writer.WriteLine ($"			\"kind\": \"template\",");
	writer.WriteLine ($"			\"version\": \"{version}\",");
	writer.WriteLine ($"			\"alias-to\": {{");
	writer.WriteLine ($"				\"any\": \"Microsoft.{platform}.Templates\",");
	writer.WriteLine ($"			}}");
	writer.WriteLine ($"		}}");
	writer.WriteLine ($"	}}");
	writer.WriteLine ($"}}");
}

return failed ? 1 : 0;
