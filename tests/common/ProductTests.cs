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
						switch (load_command) {
						case MachO.LoadCommands.MinMacOSX:
							version = SdkVersions.MinOSXVersion;
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
							break;
						case MachO.LoadCommands.MintvOS:
							version = SdkVersions.MinTVOSVersion;
							break;
						case MachO.LoadCommands.MinwatchOS:
							version = SdkVersions.MinWatchOSVersion;
							break;
						default:
							throw new NotImplementedException (load_command.ToString ());
						}

						version = version.WithBuild ();
						if (alternate_version == null)
							alternate_version = version;

						if (version != mincmd.Version && alternate_version != mincmd.Version)
							failed.Add ($"Unexpected minOS version (expected {version}, alternatively {alternate_version}, found {mincmd.Version}) in {machoFile} ({slice.Filename}).");
						any_load_command = true;
					}
					if (!any_load_command)
						failed.Add ($"No minOS version found in {machoFile}.");
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

