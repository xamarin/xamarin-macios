#!/usr/bin/env /Library/Frameworks/Mono.framework/Commands/csharp

// this script is to make sure SdkVersions.plist.in is updated with new SDK versions.

var args = Environment.GetCommandLineArgs ();
if (args.Length != 5) {
	// first arg is "/Library/Frameworks/Mono.framework/Versions/4.8.0/lib/mono/4.5/csharp.exe"
	// second arg the script itself
	Console.WriteLine ("Need 3 arguments, got {0}", args.Length - 2);
	Environment.Exit (1);
	return;
}

var doc = new System.Xml.XmlDocument ();
doc.Load ("SdkVersions.plist");

var failed = false;
if (doc.SelectSingleNode ($"/plist/dict/dict/key[text()='iOS']/following-sibling::array/string[text()='{args [2]}']") == null) {
	Console.Error.WriteLine ($"Could not find the iOS version {args [2]} in SdkVersions.plist.in.");
	failed = true;
}

if (doc.SelectSingleNode ($"/plist/dict/dict/key[text()='tvOS']/following-sibling::array/string[text()='{args [3]}']") == null) {
	Console.Error.WriteLine ($"Could not find the tvOS version {args [3]} in SdkVersions.plist.in.");
	failed = true;
}

if (doc.SelectSingleNode ($"/plist/dict/dict/key[text()='watchOS']/following-sibling::array/string[text()='{args [4]}']") == null) {
	Console.Error.WriteLine ($"Could not find the watchOS version {args [4]} in SdkVersions.plist.in.");
	failed = true;
}

Environment.Exit (failed ? 1 : 0);
