// this script is to make sure our versions.plist files are not out of date with our min/max supported OS versions.

// arguments are: plistPath iOSMinVersion iOSMaxVersion tvOSMinVersion tvOSMaxVersion macOSMinVersion macOSMaxVersion MacCatalystOSMinVersion MacCatalystOSMaxVersion

using System.IO;
using System.Xml;

try {
	var expectedArgumentCount = 9;
	if (args.Length != expectedArgumentCount) {
		Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {0}", args.Length);
		return 1;
	}

	var idx = 0;
	var plistPath = args [idx++];
	var iOSMin = args [idx++];
	var iOSMax = args [idx++];
	var tvOSMin = args [idx++];
	var tvOSMax = args [idx++];
	var macOSMin = args [idx++];
	var macOSMax = args [idx++];
	var MacCatalystMin = args [idx++];
	var MacCatalystMax = args [idx++];

	var doc = new System.Xml.XmlDocument ();
	doc.Load (plistPath);

	var failed = false;

	var check = new Action<string, string, string> ((product, min, max) => {
		var minVersion = Version.Parse (min);
		var maxVersion = Version.Parse (max);
		var foundMax = false;
		var foundMin = false;
		var versions = doc.SelectNodes ($"/plist/dict/key[text()='KnownVersions']/following-sibling::dict[1]/key[text()='{product}']/following-sibling::array[1]/string")!;
		if (versions.Count == 0) {
			// Skip this (iOS/tvOS versions for macOS, or vice versa)
			return;
		}
		var versionsHashSet = new HashSet<string> (versions.Cast<XmlNode> ().Select (v => v.InnerText));
		foreach (XmlNode node in versions) {
			// Console.WriteLine ($"{product}: checking: {node.InnerText}");
			var v = node.InnerText;
			var version = Version.Parse (v);
			if (version < minVersion) {
				Console.Error.WriteLine ($"Found the {product} version {v} in {Path.GetFileName (plistPath)}, but it's smaller than the supported min {product} version we support ({min}).");
				failed = true;
			} else if (version > maxVersion) {
				Console.Error.WriteLine ($"Found the {product} version {v} in {Path.GetFileName (plistPath)}, but it's higher than the supported max {product} version we support ({max}).");
				failed = true;
			}
			if (version == maxVersion)
				foundMax = true;
			if (version == minVersion)
				foundMin = true;
		}
		if (!foundMax) {
			Console.Error.WriteLine ($"Could not find the max {product} version {max} in {Path.GetFileName (plistPath)}.");
			failed = true;
		}
		if (!foundMin) {
			Console.Error.WriteLine ($"Could not find the min {product} version {min} in {Path.GetFileName (plistPath)}.");
			failed = true;
		}

		var supportedTPVNodes = doc.SelectNodes ($"/plist/dict/key[text()='SupportedTargetPlatformVersions']/following-sibling::dict[1]/key[text()='{product}']/following-sibling::array[1]/string")!.Cast<XmlNode> ().ToArray ();
		var supportedTPVs = supportedTPVNodes.Select (v => v.InnerText).ToArray ();
		if (supportedTPVs?.Any () == true) {
			var supportedTPVSet = new HashSet<string> (supportedTPVs);
			var missingTPVs = versionsHashSet.Except (supportedTPVSet);
			if (missingTPVs.Any ()) {
				Console.Error.WriteLine ($"The array SupportedTargetPlatformVersions are missing the following entries (they're in the KnownVersions array): {string.Join (", ", missingTPVs)}");
				failed = true;
			}
		} else if (plistPath.Contains ("/builds/")) {
			Console.Error.WriteLine ($"No SupportedTargetPlatformVersions array found in the plist: {plistPath}");
			failed = true;
		}
	});

	check ("iOS", iOSMin, iOSMax);
	check ("tvOS", tvOSMin, tvOSMax);
	check ("macOS", macOSMin, macOSMax);
	check ("MacCatalyst", MacCatalystMin, MacCatalystMax);

	return failed ? 1 : 0;
} catch (Exception e) {
	Console.WriteLine (e);
	return 1;
}
