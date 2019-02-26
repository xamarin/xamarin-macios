/*
 * Shared unit tests between XI and XM.
 **/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;

using NUnit.Framework;

using Xamarin;

namespace Xamarin.Tests
{
	[TestFixture]
	public class ProductTests
	{
		[Test]
		public void MonoVersion ()
		{
			// Verify that the mono version is in the Versions.plist, and that it's a parsable version number.
			var settings = new XmlReaderSettings () {
				XmlResolver = null,
				DtdProcessing = DtdProcessing.Parse
			};

			var plist = Path.Combine (Configuration.SdkRoot, "Versions.plist");
			var xml = new XmlDocument ();
			using (var sr = new StreamReader (plist, Encoding.UTF8, true))
			using (var reader = XmlReader.Create (sr, settings)) {
				xml.Load (reader);
				var version = xml.SelectSingleNode ("//dict/key[text()='MonoVersion']")?.NextSibling?.InnerText;
				Assert.DoesNotThrow (() => Version.Parse (version), "version");
			}
		}

		[Test]
		[TestCase (Profile.macOSSystem, MachO.LoadCommands.MinMacOSX, MachO.Platform.PLATFORM_MACOS)]
		[TestCase (Profile.macOSFull, MachO.LoadCommands.MinMacOSX, MachO.Platform.PLATFORM_MACOS)]
		[TestCase (Profile.macOSMobile, MachO.LoadCommands.MinMacOSX, MachO.Platform.PLATFORM_MACOS)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, MachO.Platform.PLATFORM_IOSSIMULATOR, false)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, MachO.Platform.PLATFORM_IOS, true)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, MachO.Platform.PLATFORM_WATCHOSSIMULATOR, false)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, MachO.Platform.PLATFORM_WATCHOS, true)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, MachO.Platform.PLATFORM_TVOSSIMULATOR, false)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, MachO.Platform.PLATFORM_TVOS, true)]
		public void MinOSVersion (Profile profile, MachO.LoadCommands load_command, MachO.Platform platform, bool device = false)
		{
			if (device)
				Configuration.AssertDeviceAvailable ();

			// TODO: add .a files
			var dylibs = Directory.GetFiles (Configuration.GetSdkPath (profile, device), "*.dylib", SearchOption.AllDirectories)
				.Where ((v) => !v.Contains ("dylib.dSYM/Contents/Resources/DWARF")); // Don't include *.dylib from inside .dSYMs.

			var failed = new List<string> ();
			foreach (var dylib in dylibs) {
				var fatfile = MachO.Read (dylib);
				foreach (var slice in fatfile) {
					var any_load_command = false;
					foreach (var lc in slice.load_commands) {

						Version lc_min_version;
						var mincmd = lc as MinCommand;
						if (mincmd != null){
							Assert.AreEqual (load_command, mincmd.Command, "Unexpected min load command");
							lc_min_version = mincmd.Version;
						} else {
							// starting from iOS SDK 12 the LC_BUILD_VERSION is used instead
							var buildver = lc as BuildVersionCommand;
							if (buildver == null)
								continue;

							Assert.AreEqual (platform, buildver.Platform, "Unexpected build version command");
							lc_min_version = buildver.MinOS;
						}

						Version version;
						Version alternate_version = null;
						Version mono_native_compat_version;
						Version mono_native_unified_version;
						switch (load_command) {
						case MachO.LoadCommands.MinMacOSX:
							version = SdkVersions.MinOSXVersion;
							mono_native_compat_version = SdkVersions.MinOSXVersion;
							mono_native_unified_version = new Version (10, 12, 0);
							break;
						case MachO.LoadCommands.MiniPhoneOS:
							version = SdkVersions.MiniOSVersion;
							if (device) {
								if (version.Major < 7)
									version = new Version (7, 0, 0); // dylibs are supported starting with iOS 7.
								alternate_version = new Version (8, 0, 0); // some iOS dylibs also have min OS 8.0 (if they're used as frameworks as well).
							}
							mono_native_compat_version = SdkVersions.MiniOSVersion;
							mono_native_unified_version = new Version (10, 0, 0);
							break;
						case MachO.LoadCommands.MintvOS:
							version = SdkVersions.MinTVOSVersion;
							mono_native_compat_version = SdkVersions.MinTVOSVersion;
							mono_native_unified_version = new Version (10, 0, 0);
							break;
						case MachO.LoadCommands.MinwatchOS:
							version = SdkVersions.MinWatchOSVersion;
							if (device)
								alternate_version = new Version (5, 1, 0); // arm64_32 has min OS 5.1
							mono_native_compat_version = SdkVersions.MinWatchOSVersion;
							mono_native_unified_version = new Version (5, 0, 0);
							break;
						default:
							throw new NotImplementedException (load_command.ToString ());
						}

						version = version.WithBuild ();
						mono_native_compat_version = mono_native_compat_version.WithBuild ();
						mono_native_unified_version = mono_native_unified_version.WithBuild ();
						if (alternate_version == null)
							alternate_version = version;

						switch (Path.GetFileName (dylib)) {
						case "libmono-native-compat.dylib":
							if (mono_native_compat_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_compat_version}, found {lc_min_version}) in {dylib}.");
							break;
						case "libmono-native-unified.dylib":
							if (mono_native_unified_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_unified_version}, found {lc_min_version}) in {dylib}.");
							break;
						default:
							if (version != lc_min_version && alternate_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {version}, alternatively {alternate_version}, found {lc_min_version}) in {dylib}.");
							break;
						}
						any_load_command = true;
					}
					if (!any_load_command)
						failed.Add ($"No minOS version found in {dylib}.");
				}
			}
			CollectionAssert.IsEmpty (failed, "Failures");
		}
	}

	static class VersionExtensions
	{
			public static Version WithBuild (this Version version)
			{
				return new Version (version.Major, version.Minor, version.Build < 0 ? 0 : version.Build);
			}
	}
}

