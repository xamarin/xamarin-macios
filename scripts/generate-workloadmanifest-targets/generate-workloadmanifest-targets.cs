// arguments are: <platform> <outputPath>

using System.IO;
using System.Xml;

var expectedArgumentCount = 5;
if (args.Length != expectedArgumentCount) {
	Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {args.Length}");
	return 1;
}

var argumentIndex = 0;
var platform = args [argumentIndex++];
var outputPath = args [argumentIndex++];
var windowsPlatforms = args [argumentIndex++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
var hasWindows = Array.IndexOf (windowsPlatforms, platform) >= 0;
var currentApiVersion = args [argumentIndex++];
var supportedApiVersions = args [argumentIndex++];

var platformLowerCase = platform.ToLowerInvariant ();

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

// Find the latest TFM for each major .NET version.
// We import the workload for this TFM if there's no TPV specified in the TargetFramework.
var groupedByMajorDotNetVersion = supportedTFMs.
										Where (v => v.IndexOfAny (new char [] { '-', '_' }) >= 0).
										GroupBy (v => v.Split (new char [] { '-', '_' }) [0]);
var highestTpvPerMajorDotNet = groupedByMajorDotNetVersion.
			Select (gr => {
				var max = gr.OrderByDescending (el => {
					var rv = tfmToTpvAndTfv (el);
					return float.Parse (rv.Tpv, System.Globalization.CultureInfo.InvariantCulture);
				}).First ();
				return max;
			}).
			ToHashSet ();

using (var writer = new StreamWriter (outputPath)) {
	writer.WriteLine ($"<Project>");
	foreach (var tfm in supportedTFMs) {
		var parsed = tfmToTpvAndTfv (tfm);
		var tfv = parsed.Tfv;
		var tpv = parsed.Tpv;
		supportedTFVs.Add (tfv);
		var workloadVersion = tfm;
		if (tfv [0] == '7')
			workloadVersion = tfm.Replace (".0", "");
		if (highestTpvPerMajorDotNet.Contains (tfm)) {
			writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true' And $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '{tfv}'))\">");
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{workloadVersion}\" /> <!-- this SDK version will validate the TargetPlatformVersion and show an error (in .NET 9+) or a warning (.NET 8) if it's not valid -->");
		} else if (tpv.Length > 0) {
			writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true' And $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '{tfv}')) And '$(TargetPlatformVersion)' == '{tpv}'\">");
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{workloadVersion}\" />");
		} else {
			writer.WriteLine ($"	<ImportGroup Condition=\" '$(TargetPlatformIdentifier)' == '{platform}' And '$(UsingAppleNETSdk)' != 'true' And $([MSBuild]::VersionEquals($(TargetFrameworkVersion), '{tfv}'))\">");
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Sdk.{tfm}\" />");
		}

		if (hasWindows) {
			writer.WriteLine ($"		<Import Project=\"Sdk.props\" Sdk=\"Microsoft.{platform}.Windows.Sdk.Aliased.{tfm}\" Condition=\" $([MSBuild]::IsOSPlatform('windows'))\" />");
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

return 0;
