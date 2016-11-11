#!/usr/bin/env /Library/Frameworks/Mono.framework/Commands/csharp

// this script is to make sure SdkVersions.plist.in is not out of date with our min/max SDK versions.

// arguments are: plistPath iOSMinVersion iOSMaxVersion tvOSMinVersion tvOSMaxVersion watchOSMinVersion watchOSMaxVersion

using System.Xml;

var args = Environment.GetCommandLineArgs ();
if (args.Length != 9 /* 2 default + 7 real */) {
	// first arg is "/Library/Frameworks/Mono.framework/Versions/4.8.0/lib/mono/4.5/csharp.exe"
	// second arg the script itself
	// then comes the ones we care about
	Console.WriteLine ("Need 7 arguments, got {0}", args.Length - 2);
	Environment.Exit (1);
	return;
}

var plistPath = args [2];
var iOSMin = args [3];
var iOSMax = args [4];
var tvOSMin = args [5];
var tvOSMax = args [6];
var watchOSMin = args [7];
var watchOSMax = args [8];

var doc = new System.Xml.XmlDocument ();
doc.Load (plistPath);

var failed = false;

var check = new Action<string, string, string> ((product, min, max) =>
{
	var minVersion = Version.Parse (min);
	var maxVersion = Version.Parse (max);
	var foundMax = false;
	var foundMin = false;
	var versions = doc.SelectNodes ($"/plist/dict/key[text()='KnownVersions']/following-sibling::dict[1]/key[text()='{product}']/following-sibling::array[1]/string");
	foreach (XmlNode node in versions) {
		// Console.WriteLine ($"{product}: checking: {node.InnerText}");
		var v = node.InnerText;
		var version = Version.Parse (v);
		if (version < minVersion) {
			Console.Error.WriteLine ($"Found the {product} version {v} in SdkVersions.plist.in, but it's smaller the supported min {product} version we support ({min}).");
			failed = true;
		} else if (version > maxVersion) {
			Console.Error.WriteLine ($"Found the {product} version {v} in SdkVersions.plist.in, but it's higher the supported max {product} version we support ({max}).");
			failed = true;
		} else if (version == maxVersion) {
			foundMax = true;
		} else if (version == minVersion) {
			foundMin = true;
		}
	}
	if (!foundMax) {
		Console.Error.WriteLine ($"Could not find the max {product} version {max} in SdkVersions.plist.in.");
		failed = true;
	}
	if (!foundMin) {
		Console.Error.WriteLine ($"Could not find the min {product} version {min} in SdkVersions.plist.in.");
		failed = true;
	}
});

check ("iOS", iOSMin, iOSMax);
check ("tvOS", tvOSMin, tvOSMax);
check ("watchOS", watchOSMin, watchOSMax);

Environment.Exit (failed ? 1 : 0);
