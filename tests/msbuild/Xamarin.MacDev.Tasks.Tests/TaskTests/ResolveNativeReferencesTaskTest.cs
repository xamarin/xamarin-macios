using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.MacDev.Tasks.Tests {

	[TestFixture]
	public class ResolveNativeReferencesTaskTest {

		// single arch request (subset are fine)
		[TestCase ("iOS", null, "arm64", "ios-arm64/Universal.framework/Universal")]
		[TestCase ("iOS", "simulator", "x86_64", "ios-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase ("iOS", "maccatalyst", "x86_64", "ios-arm64_x86_64-maccatalyst/Universal.framework/Universal")] // subset
		[TestCase ("tvOS", null, "arm64", "tvos-arm64/Universal.framework/Universal")]
		[TestCase ("tvOS", "simulator", "x86_64", "tvos-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase ("watchOS", null, "arm64_32", "watchos-arm64_32_armv7k/Universal.framework/Universal")]
		[TestCase ("watchOS", "simulator", "x86_64", "watchos-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase ("macOS", null, "x86_64", "macos-arm64_x86_64/Universal.framework/Universal")] // subset
																								 // multiple arch request (all must be present)
		[TestCase ("macOS", null, "x86_64, arm64", "macos-arm64_x86_64/Universal.framework/Universal")]
		// failure to resolve requested architecture
		[TestCase ("iOS", "simulator", "i386, x86_64", "")] // i386 not available
															// failure to resolve mismatched variant
		[TestCase ("macOS", "maccatalyst", "x86_64", "")] // maccatalyst not available on macOS (it's on iOS)
		public void Xcode12_x (string platform, string variant, string architecture, string expected)
		{
			// some architecture changes recently, e.g.
			// in Xcode 12.1+ watchOS does not have an i386 architecture anymore
			// on Xcode 12.2+ you get arm64 for all (iOS, tvOS and watchOS) simulators
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "xcf-xcode12.2.plist");
			var plist = PDictionary.FromFile (path);
			var result = ResolveNativeReferences.ResolveXCFramework (plist, platform, variant, architecture);
			Assert.That (result, Is.EqualTo (expected), expected);
		}

		[TestCase ("iOS", null, "ARMv7", "ios-arm64_armv7_armv7s/XTest.framework/XTest")]
		// there was no 64bits simulator for watchOS but a i386 one was available
		[TestCase ("watchOS", "simulator", "x86_64", "")]
		[TestCase ("watchOS", "simulator", "i386", "watchos-i386-simulator/XTest.framework/XTest")]
		public void PreXcode12 (string platform, string variant, string architecture, string expected)
		{
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "xcf-prexcode12.plist");
			var plist = PDictionary.FromFile (path);
			var result = ResolveNativeReferences.ResolveXCFramework (plist, platform, variant, architecture);
			Assert.That (result, Is.EqualTo (expected), expected);
		}

		[Test]
		public void BadInfoPlist ()
		{
			var plist = new PDictionary ();
			var result = ResolveNativeReferences.ResolveXCFramework (plist, "iOS", null, "x86_64");
			Assert.Null (result, "Invalid Info.plist");
		}
	}
}
