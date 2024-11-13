using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using NUnit.Framework;

using Xamarin.Utils;

#nullable disable // until we get around to fixing this file

namespace Xamarin.Tests {
	static partial class Configuration {

		static string TestAssemblyDirectory {
			get {
				return TestContext.CurrentContext.WorkDirectory;
			}
		}

#if !XAMMAC_TESTS
		public static void AssertRuntimeIdentifierAvailable (ApplePlatform platform, string runtimeIdentifier)
		{
			if (string.IsNullOrEmpty (runtimeIdentifier))
				return;

			if (GetRuntimeIdentifiers (platform).Contains (runtimeIdentifier))
				return;

			Assert.Ignore ($"The runtime identifier {runtimeIdentifier} is not available on {platform}");
		}

		public static void AssertRuntimeIdentifiersAvailable (ApplePlatform platform, string runtimeIdentifiers)
		{
			if (string.IsNullOrEmpty (runtimeIdentifiers))
				return;

			foreach (var rid in runtimeIdentifiers.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				AssertRuntimeIdentifierAvailable (platform, rid);
		}

		public static void AssertiOS32BitAvailable ()
		{
			Assert.Ignore ($"32-bit iOS support is not available in the current build.");
		}
#endif // !XAMMAC_TESTS

		public static void AssertDeviceAvailable ()
		{
			if (include_device)
				return;
			Assert.Ignore ("This build does not include device support.");
		}

		public static void AssertDotNetAvailable ()
		{
		}

		public static void AssertLegacyXamarinAvailable ()
		{
			Assert.Ignore ("Legacy xamarin build not enabled");
		}

		// Calls Assert.Ignore if the given platform isn't included in the current build.
		public static void IgnoreIfIgnoredPlatform (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				if (!include_ios)
					Assert.Ignore ("iOS is not included in this build");
				break;
			case ApplePlatform.TVOS:
				if (!include_tvos)
					Assert.Ignore ("tvOS is not included in this build");
				break;
			case ApplePlatform.MacOSX:
				if (!include_mac)
					Assert.Ignore ("macOS is not included in this build");
				break;
			case ApplePlatform.MacCatalyst:
				if (!include_maccatalyst)
					Assert.Ignore ("Mac Catalyst is not included in this build");
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		// Calls Assert.Ignore if the given platform isn't included in the current build.
		public static void IgnoreIfIgnoredPlatform (string platform)
		{
			switch (platform.ToLower ()) {
			case "ios":
			case "tvos":
			case "watchos":
			case "macosx":
			case "maccatalyst":
				IgnoreIfIgnoredPlatform ((ApplePlatform) Enum.Parse (typeof (ApplePlatform), platform, true));
				break;
			case "macos":
				IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
				break;
			default:
				throw new ArgumentOutOfRangeException ($"Unknown platform: {platform}");
			}
		}

		public static bool AnyIgnoredPlatforms ()
		{
			return AnyIgnoredPlatforms (out var _);
		}

		public static bool AnyIgnoredPlatforms (out IEnumerable<ApplePlatform> notIncluded)
		{
			var allPlatforms = GetAllPlatforms ();
			var includedPlatforms = GetIncludedPlatforms ();
			notIncluded = allPlatforms.Where (v => !includedPlatforms.Contains (v)).ToArray ();
			return notIncluded.Any ();
		}

		public static void IgnoreIfAnyIgnoredPlatforms ()
		{
			if (AnyIgnoredPlatforms (out var notIncluded))
				Assert.Ignore ($"This test requires all platforms to be included, but the following platforms aren't included: {string.Join (", ", notIncluded.Select (v => v.AsString ()))}");
		}

		public static void IgnoreIfNotOnMacOS ()
		{
			IgnoreIfNotOn (System.Runtime.InteropServices.OSPlatform.OSX);
		}

		public static void IgnoreIfNotOnWindows ()
		{
			IgnoreIfNotOn (System.Runtime.InteropServices.OSPlatform.Windows);
		}

		public static void IgnoreIfNotOn (System.Runtime.InteropServices.OSPlatform platform)
		{
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (platform))
				return;
			Assert.Ignore ($"This test is only applicable on {platform}");
		}

		public static void IgnoreIfNotXamarinEnabled ()
		{
			if (EnableXamarin)
				return;
			Assert.Ignore ($"This test is only applicable if Xamarin-specific bits are enabled.");
		}

	}
}
