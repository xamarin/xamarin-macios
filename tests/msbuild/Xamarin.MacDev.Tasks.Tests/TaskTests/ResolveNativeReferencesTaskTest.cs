using System;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks.Tests {

	[TestFixture]
	public class ResolveNativeReferencesTaskTest {

		TaskLoggingHelper log = new TaskLoggingHelper (new TestEngine (), "ResolveNativeReferences");

		// single arch request (subset are fine)
		[TestCase (TargetFramework.DotNet_iOS_String, false, "arm64", "ios-arm64/Universal.framework/Universal")]
		[TestCase (TargetFramework.DotNet_iOS_String, true, "x86_64", "ios-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_MacCatalyst_String, false, "x86_64", "ios-arm64_x86_64-maccatalyst/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_tvOS_String, false, "arm64", "tvos-arm64/Universal.framework/Universal")]
		[TestCase (TargetFramework.DotNet_tvOS_String, true, "x86_64", "tvos-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.Xamarin_WatchOS_1_0_String, false, "arm64_32", "watchos-arm64_32_armv7k/Universal.framework/Universal")]
		[TestCase (TargetFramework.DotNet_watchOS_String, true, "x86_64", "watchos-arm64_x86_64-simulator/Universal.framework/Universal")] // subset
		[TestCase (TargetFramework.DotNet_macOS_String, false, "x86_64", "macos-arm64_x86_64/Universal.framework/Universal")] // subset

		// multiple arch request (all must be present)
		[TestCase (TargetFramework.DotNet_macOS_String, false, "x86_64, arm64", "macos-arm64_x86_64/Universal.framework/Universal")]

		// failure to resolve requested architecture
		[TestCase (TargetFramework.DotNet_iOS_String, true, "i386, x86_64", null)] // i386 not available

		// failure to resolve mismatched variant
		[TestCase (TargetFramework.DotNet_macOS_String, true, "x86_64", null)] // simulator not available on macOS
		public void Xcode12_x (string targetFrameworkMoniker, bool isSimulator, string architecture, string expected)
		{
			// some architecture changes recently, e.g.
			// in Xcode 12.1+ watchOS does not have an i386 architecture anymore
			// on Xcode 12.2+ you get arm64 for all (iOS, tvOS and watchOS) simulators
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "xcf-xcode12.2.plist");
			var plist = PDictionary.FromFile (path);
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", targetFrameworkMoniker, isSimulator, architecture, out var frameworkPath);
			Assert.AreEqual (result, !string.IsNullOrEmpty (expected), "result");
			Assert.That (frameworkPath, Is.EqualTo (expected), "frameworkPath");
		}

		[TestCase (TargetFramework.DotNet_iOS_String, false, "ARMv7", "ios-arm64_armv7_armv7s/XTest.framework/XTest")]
		// there was no 64bits simulator for watchOS but a i386 one was available
		[TestCase (TargetFramework.Xamarin_WatchOS_1_0_String, true, "x86_64", null)]
		[TestCase (TargetFramework.Xamarin_WatchOS_1_0_String, true, "i386", "watchos-i386-simulator/XTest.framework/XTest")]
		public void PreXcode12 (string targetFrameworkMoniker, bool isSimulator, string architecture, string expected)
		{
			var path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "xcf-prexcode12.plist");
			var plist = PDictionary.FromFile (path);
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", targetFrameworkMoniker, isSimulator, architecture, out var frameworkPath);
			Assert.AreEqual (result, !string.IsNullOrEmpty (expected), "result");
			Assert.That (frameworkPath, Is.EqualTo (expected), "frameworkPath");
		}

		[Test]
		public void BadInfoPlist ()
		{
			var plist = new PDictionary ();
			var result = ResolveNativeReferences.TryResolveXCFramework (log, plist, "N/A", TargetFramework.DotNet_iOS_String, false, "x86_64", out var frameworkPath);
			Assert.IsFalse (result, "Invalid Info.plist");
		}
	}
}
