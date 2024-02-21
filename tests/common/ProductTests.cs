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

namespace Xamarin.Tests {
	[TestFixture]
	public class ProductTests {
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
		[TestCase (Profile.macOSSystem, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS)]
		[TestCase (Profile.macOSFull, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS)]
		[TestCase (Profile.macOSMobile, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, MachO.Platform.IOSSimulator, false)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, MachO.Platform.IOS, true)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, MachO.Platform.WatchOSSimulator, false)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, MachO.Platform.WatchOS, true)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, MachO.Platform.TvOSSimulator, false)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, MachO.Platform.TvOS, true)]
		public void MinOSVersion (Profile profile, MachO.LoadCommands load_command, MachO.Platform platform, bool device = false)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			if (device)
				Configuration.AssertDeviceAvailable ();

			var machoFiles = Directory.GetFiles (Configuration.GetSdkPath (profile, device), "*", SearchOption.AllDirectories)
						.Where ((v) => {
							if (v.Contains ("dylib.dSYM/Contents/Resources/DWARF")) {
								// Don't include *.dylib from inside .dSYMs.
								return false;
							} else if (v.Contains ("libxammac-classic") || v.Contains ("libxammac-system-classic")) {
								// We don't care about XM Classic, those are binary dependencies.
								return false;
							}
							var ext = Path.GetExtension (v);
							return ext == ".a" || ext == ".dylib";
						}
					);

			var failed = new List<string> ();
			foreach (var machoFile in machoFiles) {
				var fatfile = MachO.Read (machoFile);
				foreach (var slice in fatfile) {
					if (slice.IsDynamicLibrary && slice.Architecture == MachO.Architectures.x86_64 && slice.Parent is not null && slice.Parent.size < 10240 /* this is the dummy x86_64 slice to appease Apple's notarization tooling */)
						continue;
					else if (!slice.IsDynamicLibrary && slice.Architecture == MachO.Architectures.x86_64 && slice.Filename == "x86-64-slice.o" /* a static version of the x86_64 dummy slice */)
						continue;
					var any_load_command = false;
					foreach (var lc in slice.load_commands) {

						Version lc_min_version;
						var mincmd = lc as MinCommand;
						if (mincmd is not null) {
							Assert.AreEqual (load_command, mincmd.Command, "Unexpected min load command");
							lc_min_version = mincmd.Version;
						} else {
							// starting from iOS SDK 12 the LC_BUILD_VERSION is used instead
							var buildver = lc as BuildVersionCommand;
							if (buildver is null)
								continue;

							var alternativePlatform = (MachO.Platform) 0;
							switch (platform) {
							case MachO.Platform.IOSSimulator:
								alternativePlatform = MachO.Platform.IOS;
								break;
							case MachO.Platform.TvOSSimulator:
								alternativePlatform = MachO.Platform.TvOS;
								break;
							case MachO.Platform.WatchOSSimulator:
								alternativePlatform = MachO.Platform.WatchOS;
								break;
							}
							Assert.That (buildver.Platform, Is.EqualTo (platform).Or.EqualTo (alternativePlatform), $"Unexpected build version command in {machoFile} ({slice.Filename})");
							lc_min_version = buildver.MinOS;
						}

						Version version;
						Version alternate_version = null;
						Version alternate_version2 = null;
						Version mono_native_compat_version;
						Version alternate_mono_native_compat_version = null;
						Version mono_native_unified_version;
						Version alternate_mono_native_unified_version = null;
						switch (load_command) {
						case MachO.LoadCommands.MinMacOSX:
							version = SdkVersions.MinOSXVersion;
							if (slice.Architecture == MachO.Architectures.ARM64) {
								alternate_version = new Version (11, 0, 0);
							}
							mono_native_compat_version = SdkVersions.MinOSXVersion;
							mono_native_unified_version = new Version (10, 12, 0);
							break;
						case MachO.LoadCommands.MiniPhoneOS:
							version = SdkVersions.MiniOSVersion;
							alternate_version = new Version (7, 0, 0); // some libs from mono are still iOS 7.0
							mono_native_compat_version = version;
							mono_native_unified_version = new Version (10, 0, 0);
							alternate_mono_native_compat_version = new Version (7, 0, 0); // Xcode 12.4 built binaries
							break;
						case MachO.LoadCommands.MintvOS:
							version = SdkVersions.MinTVOSVersion;
							alternate_version = new Version (9, 0, 0); // some libs from mono are still tvOS 9.0
							mono_native_compat_version = new Version (9, 0, 0);
							mono_native_unified_version = new Version (10, 0, 0);
							break;
						case MachO.LoadCommands.MinwatchOS:
							version = SdkVersions.MinWatchOSVersion;
							alternate_version = new Version (5, 1, 0); // arm64_32 has min OS 5.1
							alternate_version2 = new Version (2, 0, 0); // some libs from mono are still watchOS 2.0
							mono_native_compat_version = new Version (2, 0, 0);
							mono_native_unified_version = new Version (5, 0, 0);
							if (device)
								alternate_mono_native_unified_version = new Version (5, 1, 0); // armv7k has 5.0, arm64_32 has 5.1
							break;
						default:
							throw new NotImplementedException (load_command.ToString ());
						}

						version = version.WithBuild ();
						mono_native_compat_version = mono_native_compat_version.WithBuild ();
						mono_native_unified_version = mono_native_unified_version.WithBuild ();
						if (alternate_version is null)
							alternate_version = version;
						if (alternate_version2 is null)
							alternate_version2 = version;
						if (alternate_mono_native_unified_version is null)
							alternate_mono_native_unified_version = mono_native_unified_version;
						if (alternate_mono_native_compat_version is null)
							alternate_mono_native_compat_version = mono_native_compat_version;

						switch (Path.GetFileName (machoFile)) {
						case "libmono-native-compat.dylib":
						case "libmono-native-compat.a":
							if (mono_native_compat_version != lc_min_version && alternate_mono_native_compat_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_compat_version}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						case "libmono-native-unified.dylib":
						case "libmono-native-unified.a":
							if (mono_native_unified_version != lc_min_version && alternate_mono_native_unified_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_unified_version}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						default:
							if (version != lc_min_version && alternate_version != lc_min_version && alternate_version2 != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {version}, alternatively {alternate_version} or {alternate_version2}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						}
						any_load_command = true;
					}
					if (!any_load_command)
						failed.Add ($"No minOS version found in {machoFile}.");
				}
			}
			CollectionAssert.IsEmpty (failed, "Failures");
		}
	}

	static class VersionExtensions {
		public static Version WithBuild (this Version version)
		{
			return new Version (version.Major, version.Minor, version.Build < 0 ? 0 : version.Build);
		}
	}
}
