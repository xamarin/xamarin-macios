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
		[TestCase (Profile.macOSSystem, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS, 16)]
		[TestCase (Profile.macOSFull, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS, 16)]
		[TestCase (Profile.macOSMobile, MachO.LoadCommands.MinMacOSX, MachO.Platform.MacOS, 16)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, MachO.Platform.IOS, 28)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, MachO.Platform.WatchOS, 28)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, MachO.Platform.TvOS, 28)]
		public void MinOSVersion (Profile profile, MachO.LoadCommands load_command, MachO.Platform platform, int minFileCount)
		{
			var machoFiles = new HashSet<string> ();
			var devSdkPath = Configuration.GetSdkPath (profile, true);
			if (Directory.Exists (devSdkPath))
				machoFiles.UnionWith (Directory.GetFiles (devSdkPath, "*", SearchOption.AllDirectories));
			var simSdkPath = Configuration.GetSdkPath (profile, false);
			if (devSdkPath != simSdkPath && Directory.Exists (simSdkPath))
				machoFiles.UnionWith (Directory.GetFiles (simSdkPath, "*", SearchOption.AllDirectories));

			machoFiles.RemoveWhere ((v => {
				if (v.Contains ("dylib.dSYM/Contents/Resources/DWARF")) {
					// Don't include *.dylib from inside .dSYMs.
					return true;
				} else if (v.Contains ("libxammac-classic") || v.Contains ("libxammac-system-classic")) {
					// We don't care about XM Classic, those are binary dependencies.
					return true;
				}
				var ext = Path.GetExtension (v);
				return ext != ".a" && ext != ".dylib";
			}));

			Assert.GreaterOrEqual (machoFiles.Count, minFileCount, "Minimum number of files to verify");

			var failed = new List<string> ();
			foreach (var machoFile in machoFiles) {
				var fatfile = MachO.Read (machoFile);
				foreach (var slice in fatfile) {
					var any_load_command = false;
					bool device;
					switch (slice.Architecture) {
					case MachO.Architectures.ARM64:
					case MachO.Architectures.ARM64e:
					case MachO.Architectures.ARM64_32:
					case MachO.Architectures.ARMv6:
					case MachO.Architectures.ARMv7:
					case MachO.Architectures.ARMv7k:
					case MachO.Architectures.ARMv7s:
						device = true;
						break;
					case MachO.Architectures.i386:
					case MachO.Architectures.x86_64:
						device = false;
						break;
					default:
						throw new NotImplementedException ($"Architecture: {slice.Architecture}");
					}

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
							if (slice.IsDynamicLibrary && device) {
								if (version.Major < 7)
									version = new Version (7, 0, 0); // dylibs are supported starting with iOS 7.
								alternate_version = new Version (8, 0, 0); // some iOS dylibs also have min OS 8.0 (if they're used as frameworks as well).
							} else if (slice.Architecture == MachO.Architectures.ARM64) {
								alternate_version = new Version (7, 0, 0); // our arm64 slices has min iOS 7.0.
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

						switch (Path.GetFileName (machoFile)) {
						case "libmono-native-compat.dylib":
							if (mono_native_compat_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_compat_version}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						case "libmono-native-unified.dylib":
							if (mono_native_unified_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {mono_native_unified_version}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						default:
							if (version != lc_min_version && alternate_version != lc_min_version)
								failed.Add ($"Unexpected minOS version (expected {version}, alternatively {alternate_version}, found {lc_min_version}) in {machoFile} ({slice.Filename}).");
							break;
						}
						any_load_command = true;
					}
					if (!any_load_command)
						failed.Add ($"No minOS version found in {machoFile}.");
				}
			}
			Assert.IsEmpty (string.Join ("\n", failed), "Failures");
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

