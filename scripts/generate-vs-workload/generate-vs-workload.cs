// arguments are:
//   --shorten long=short
//   --platform <platform> <version>
//   --windows-platform <platform>

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

var shorten = new Dictionary<string, string> ();
var platforms = new List<(string, string)> ();
var windowsPlatforms = new List<string> ();
var tfm = string.Empty;
var xcodeName = string.Empty;
var outputPath = string.Empty;

var queue = new Queue<string> (args);

while (queue.Any ()) {
	var arg = queue.Dequeue ();
	switch (arg) {
	case "--shorten":
		var values = queue.Dequeue ().Split ('=');
		shorten [values [0]] = values [1];
		break;
	case "--platform":
		var platform = queue.Dequeue ();
		var version = queue.Dequeue ();
		platforms.Add ((platform, version));
		break;
	case "--windows-platform":
		windowsPlatforms.Add (queue.Dequeue ());
		break;
	case "--output":
		outputPath = queue.Dequeue ();
		break;
	case "--tfm":
		tfm = queue.Dequeue ();
		break;
	case "--xcode":
		xcodeName = $"xcode{queue.Dequeue ()}";
		break;
	default:
		Console.Error.WriteLine ($"Unknown argument: {arg}");
		return 1;
	}
}

using (TextWriter writer = new StreamWriter (outputPath)) {
	writer.WriteLine ($"<?xml version=\"1.0\" encoding=\"utf-8\"?>");
	writer.WriteLine ($"<Project>");
	writer.WriteLine ($"  <PropertyGroup>");
	var allPlatforms = string.Join (".", platforms.Select (v => v.Item1).OrderBy (v => v));
	writer.WriteLine ($"    <TargetName>Microsoft.NET.Sdk.{allPlatforms}.Workload.{tfm}.{xcodeName}</TargetName>");
	// Find the iOS version, otherwise use the version of the first platform listed.
	var iOSPlatform = platforms.Where (v => v.Item1 == "iOS");
	var manifestBuildVersion = iOSPlatform.Any () ? iOSPlatform.First ().Item2 : platforms.First ().Item2;
	writer.WriteLine ($"    <ManifestBuildVersion>{manifestBuildVersion}</ManifestBuildVersion>");
	writer.WriteLine ($"    <EnableSideBySideManifests>true</EnableSideBySideManifests>");
	writer.WriteLine ($"    <UseVisualStudioComponentPrefix>false</UseVisualStudioComponentPrefix>");
	writer.WriteLine ($"  </PropertyGroup>");
	writer.WriteLine ($"  <ItemGroup>");
	writer.WriteLine ($"    <!-- Shorten package names to avoid long path caching issues in Visual Studio -->");
	foreach (var entry in shorten) {
		var longName = entry.Key;
		var shortName = entry.Value;
		writer.WriteLine ($"    <ShortNames Include=\"{longName}\">");
		writer.WriteLine ($"      <Replacement>{shortName}</Replacement>");
		writer.WriteLine ($"    </ShortNames>");
	}
	foreach (var entry in platforms) {
		var platform = entry.Item1;
		var version = entry.Item2;
		var longPlatform = platform;
		var description = $".NET SDK Workload for building {platform} applications.";
		if (platform == "MacCatalyst") {
			longPlatform = "Mac Catalyst";
			description = ".NET SDK Workload for building macOS applications with Mac Catalyst.";
		}
		writer.WriteLine ($"    <ComponentResources Include=\"{platform.ToLower ()}\" Version=\"{version}\" Category=\".NET\" Title=\".NET SDK for {longPlatform}\" Description=\"{description}\"/>");
	}
	foreach (var entry in platforms) {
		var platform = entry.Item1;
		var version = entry.Item2;
		writer.WriteLine ($"    <WorkloadPackages Include=\"$(NuGetPackagePath)\\Microsoft.NET.Sdk.{platform}.Manifest*.nupkg\" Version=\"{version}\" SupportsMachineArch=\"true\" />");
	}
	writer.WriteLine ("  </ItemGroup>");
	writer.WriteLine ("</Project>");
}

return 0;
