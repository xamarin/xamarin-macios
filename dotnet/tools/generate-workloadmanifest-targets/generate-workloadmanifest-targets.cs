// arguments are: <platform> <outputPath>

var expectedArgumentCount = 5;
if (args.Length != expectedArgumentCount) {
	Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {args.Length}");
	Environment.Exit (1);
	return;
}

var argumentIndex = 0;
var platform = args [argumentIndex++];
var outputPath = args [argumentIndex++];
var windowsPlatforms = args [argumentIndex++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
var hasWindows = Array.IndexOf (windowsPlatforms, platform) >= 0;
var currentApiVersion = args [argumentIndex++];
var supportedApiVersions = args [argumentIndex++];

var platformLowerCase = platform.ToLowerInvariant ();
// var tfm = currentApiVersion;

var supportedTFMs = new List<string> ();
supportedTFMs.AddRange (supportedApiVersions.Split (' ').Select (v => v.Replace ('-', '_')));
supportedTFMs.Sort ();

var supportedTFVs = new List<string> ();

var tfmToTpvAndTfv = new Func<string, (string Tfv, string Tpv)> (tfm => {
	var tfv = tfm.Replace ("net", "");
	var sep = tfv.IndexOfAny (new char [] { '-', '_' });
	var tpv = "";
	if (sep >= 0) {
		tpv = tfv.Substring (sep + 1);
		tfv = tfv.Substring (0, sep);
	}
	return (tfv, tpv);
});

var separators = new char [] { '-', '_' };

// Find the latest TFM for each major .NET version, skipping any TFM where the OS version is higher than the current OS version (those are preview versions)
// We import the workload for this TFM if there's no TPV specified in the TargetFramework.
var currentOSVersion = float.Parse (currentApiVersion.Split (new char [] { '-', '_' }).Last ());

using (var writer = new StreamWriter (outputPath)) {
	writer.WriteLine ($"<Project>");
	// Add one import for each TFM we support, with an explicit check on the TargetPlatfromVersion version.
	foreach (var tfm in supportedTFMs) {
		var parsed = tfmToTpvAndTfv (tfm);
		var tfv = parsed.Tfv;
		var tpv = parsed.Tpv;
		supportedTFVs.Add (tfv);
		writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true' And $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '{tfv}')) And '$(TargetPlatformVersion)' == '{tpv}'\">");
		writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{tfm}\" />");
		if (hasWindows) {
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Windows.Sdk.Aliased.{tfm}\" Condition=\" $([MSBuild]::IsOSPlatform('windows'))\" />");
		}
		writer.WriteLine ($"	</ImportGroup>");
		writer.WriteLine ();
	}

	// Add one import for each TargetFramework (version) we support, with no check on the TargetPlatfromVersion version.
	// This is the default import if the TargetPlatformVersion isn't set.
	// The imported workload is the one for the last OS version in supportedTFMs that isn't
	// a preview version (and we define "preview version" as a version where OS version is > current OS version)
	var supportedTargetFrameworks = supportedTFMs.Select (v => v.Split (new char [] { '-', '_' }) [0]).Distinct ();
	foreach (var supportedTargetFramework in supportedTargetFrameworks.OrderByDescending (v => float.Parse (v.Replace ("net", "")))) {
		var lastOSVersionForTargetFramework = supportedTFMs
			.Where (v => v.StartsWith (supportedTargetFramework, StringComparison.Ordinal))
			.Where (v => float.Parse (v.Split (separators).Last ()) <= currentOSVersion)
			.OrderByDescending (v => float.Parse (v.Split (separators).Last ()) <= currentOSVersion)
			.Last ();

		var parsed = tfmToTpvAndTfv (lastOSVersionForTargetFramework);

		writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true' And $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '{parsed.Tfv}'))\">");
		writer.WriteLine ($"        <!-- this SDK version will validate the TargetPlatformVersion and show an error (in .NET 9+) or a warning (.NET 8) if it's not valid -->");
		writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{lastOSVersionForTargetFramework}\" />");
		if (hasWindows) {
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Windows.Sdk.Aliased.{lastOSVersionForTargetFramework}\" Condition=\" $([MSBuild]::IsOSPlatform('windows'))\" />");
		}
		writer.WriteLine ($"	</ImportGroup>");
		writer.WriteLine ();
	}

	var earliestSupportedTFV = supportedTFVs.Select (v => Version.Parse (v)).OrderBy (v => v).First ();
	var latestSupportedTFV = supportedTFVs.Select (v => Version.Parse (v)).OrderBy (v => v).Last ();
	writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true'\">");
	writer.WriteLine ($"		<Import Project=\"Sdk-eol.props\" Sdk=\"Microsoft.{platform}.Sdk.{currentApiVersion}\" Condition=\" $([MSBuild]::VersionLessThan($(TargetFrameworkVersion), '{earliestSupportedTFV}'))\" />");
	writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{currentApiVersion}\" Condition=\" $([MSBuild]::VersionGreaterThan($(TargetFrameworkVersion), '{latestSupportedTFV}'))\" />");
	writer.WriteLine ($"	</ImportGroup>");
	writer.WriteLine ();
	writer.WriteLine ($"	<ItemGroup Condition=\" '$(TargetFrameworkIdentifier)' == '.NETCoreApp' and $([MSBuild]::VersionGreaterThanOrEquals($(TargetFrameworkVersion), '6.0')) \">");
	writer.WriteLine ($"		<SdkSupportedTargetPlatformIdentifier Include=\"{platformLowerCase}\" DisplayName=\"{platform}\" />");
	writer.WriteLine ($"	</ItemGroup>");

	writer.WriteLine ($"</Project>");
	writer.WriteLine ();
}
