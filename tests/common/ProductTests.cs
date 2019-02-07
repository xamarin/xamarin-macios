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
		[TestCase (Profile.macOSSystem, MachO.LoadCommands.MinMacOSX)]
		[TestCase (Profile.macOSFull, MachO.LoadCommands.MinMacOSX)]
		[TestCase (Profile.macOSMobile, MachO.LoadCommands.MinMacOSX)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, false)]
		[TestCase (Profile.iOS, MachO.LoadCommands.MiniPhoneOS, true)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, false)]
		[TestCase (Profile.watchOS, MachO.LoadCommands.MinwatchOS, true)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, false)]
		[TestCase (Profile.tvOS, MachO.LoadCommands.MintvOS, true)]
		public void MinOSVersion (Profile profile, MachO.LoadCommands load_command, bool device = false)
		{
			if (device)
				Configuration.AssertDeviceAvailable ();

			var dylibs = Directory.GetFiles (Configuration.GetSdkPath (profile, device), "*.dylib", SearchOption.AllDirectories)
				.Where ((v) => !v.Contains ("dylib.dSYM/Contents/Resources/DWARF")); // Don't include *.dylib from inside .dSYMs.

			var failed = new List<string> ();
			foreach (var dylib in dylibs) {
				var fatfile = MachO.Read (dylib);
				foreach (var slice in fatfile) {
					var any_load_command = false;
					foreach (var lc in slice.load_commands) {
						var mincmd = lc as MinCommand;
						if (mincmd == null)
							continue;
						// Console.WriteLine ($"    {mincmd.Command} version: {mincmd.version}=0x{mincmd.version.ToString ("x")}={mincmd.Version} sdk: {mincmd.sdk}=0x{mincmd.sdk.ToString ("x")}={mincmd.Sdk}");

						Assert.AreEqual (load_command, mincmd.Command, "Unexpected min load command");

						Version version;
						Version alternate_version = null;
						switch (load_command) {
						case MachO.LoadCommands.MinMacOSX:
							version = SdkVersions.MinOSXVersion;
							break;
						case MachO.LoadCommands.MiniPhoneOS:
							version = SdkVersions.MiniOSVersion;
							if (device) {
								if (version.Major < 7)
									version = new Version (7, 0, 0); // dylibs are supported starting with iOS 7.
								alternate_version = new Version (8, 0, 0); // some iOS dylibs also have min OS 8.0 (if they're used as frameworks as well).
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

						version = new Version (version.Major, version.Minor, version.Build < 0 ? 0 : version.Build);
						if (alternate_version == null)
							alternate_version = version;

						if (version != mincmd.Version && alternate_version != mincmd.Version)
							failed.Add ($"Unexpected minOS version (expected {version}, alternatively {alternate_version}, found {mincmd.Version}) in {dylib}.");
						any_load_command = true;
					}
					if (!any_load_command)
						failed.Add ($"No minOS version found in {dylib}.");
				}
			}
			CollectionAssert.IsEmpty (failed, "Failures");
		}
	}
}

