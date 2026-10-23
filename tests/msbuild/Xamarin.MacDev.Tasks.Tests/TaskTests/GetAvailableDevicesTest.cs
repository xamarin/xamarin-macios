using System.IO;
using System.Linq;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class GetAvailableDevicesTests : TestBase {
		class GetAvailableDevicesTaskWrapper : GetAvailableDevices {
			public string SimCtlJson = string.Empty;
			public string DeviceCtlJson = string.Empty;
			protected override async System.Threading.Tasks.Task<string> ExecuteCtlAsync (params string [] args)
			{
				switch (args [0]) {
				case "simctl":
					return SimCtlJson;
				case "devicectl":
					return DeviceCtlJson;
				default:
					throw new Exception ();
				}
			}
		}

		GetAvailableDevicesTaskWrapper CreateTask (ApplePlatform platform, string simctlJson, string devicectlJson, string appManifest = "")
		{
			var task = new GetAvailableDevicesTaskWrapper () {
				SimCtlJson = simctlJson,
				DeviceCtlJson = devicectlJson,
			};
			task.SdkDevPath = Configuration.XcodeLocation;
			task.TargetFrameworkMoniker = TargetFramework.GetTargetFramework (platform).ToString ();

			if (!string.IsNullOrEmpty (appManifest)) {
				var tmpdir = Cache.CreateTemporaryDirectory ();
				var appManifestPath = Path.Combine (tmpdir, "Info.plist");
				File.WriteAllText (appManifestPath, appManifest);
				task.AppBundleManifestPath = appManifestPath;
			}

			return task;
		}

		[Test]
		[TestCase ("", "")]
		[TestCase ("{}", "{}")]
		[TestCase ("[]", "[]")]
		[TestCase ("{ \"devicetypes\": {}, \"runtimes\": {}, \"devices\": {} }", "")]
		[TestCase ("{ \"devicetypes\": {}, \"runtimes\": {} }", "")]
		[TestCase ("{ \"devicetypes\": {}, \"devices\": {} }", "")]
		[TestCase ("{ \"devices\": {} }", "")]
		[TestCase ("", "{\"result\" : {} }")]
		[TestCase ("", "{\"result\" : { \"devices\": {} } }")]
		public void EmptyJsons (string simctl, string devicectl)
		{
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, simctl, devicectl);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.That (task.Devices.Count, Is.EqualTo (0), "Devices should be empty.");
			Assert.That (task.DiscardedDevices.Count, Is.EqualTo (0), "No devices should have been discarded.");
		}

		[Test]
		public void DeviceCtl1 ()
		{
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, "", DEVICECTL_JSON_1);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (3), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (1), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device 1 mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 Platform mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Status"), Is.EqualTo ("Paired"), "Device 1 Status mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 2 Platform mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Status"), Is.EqualTo ("Unpaired"), "Device 2 Status mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.Devices [2].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Device 3 mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Device 3 Name mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Device 3 Platform mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Status"), Is.EqualTo ("Paired"), "Device 3 Status mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("DiscardedReason"), Is.Empty, "Device 3 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 1 Description mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 1 reason mismatch.");
			});
		}

		[Test]
		public void DeviceCtlDuplicateSimulator ()
		{
			if (!GetAvailableDevices.CanRunArm64)
				Assert.Ignore ("arm64 simulators require an Apple Silicon Mac.");
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1.Replace ("00008003-012301230123ABCD", "3F1C114D-FC3D-481A-9CA1-499EE1339390"));
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");

			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (4), "Devices count mismatch.");
				Assert.That (task.Devices.Count (d => d.ItemSpec == "3F1C114D-FC3D-481A-9CA1-499EE1339390"), Is.EqualTo (1), "Duplicate simulator count mismatch.");
				Assert.That (task.Devices.Single (d => d.ItemSpec == "3F1C114D-FC3D-481A-9CA1-499EE1339390").GetMetadata ("Type"), Is.EqualTo ("Simulator"), "Simulator metadata mismatch.");
			});
		}

		public void DeviceCtlJsonVersion5 ()
		{
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, "", DEVICECTL_JSON_VERSION_5);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (1), "Devices count mismatch.");
				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 27.0"), "Device description mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Status"), Is.EqualTo ("Paired"), "Device Status mismatch.");
			});
		}

		[Test]
		public void SimCtl1 ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, "");
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (2), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (3), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 1 mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 1 Name mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 2 mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 2 Name mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Device 3 mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Device 3 Name mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Device 3 discarded reason mismatch.");
			});
		}
		[Test]
		public void Ctl1 ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (5), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (4), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.Devices [2].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Device 3 Name mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 3 OSVersion mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("DiscardedReason"), Is.Empty, "Device 3 discarded reason mismatch.");

				Assert.That (task.Devices [3].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 4 Name mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 4 OSVersion mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("DiscardedReason"), Is.Empty, "Device 4 discarded reason mismatch.");

				Assert.That (task.Devices [4].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Device 5 UDID mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Device 5 Name mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Device 5 OSVersion mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Device 5 UDID mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("DiscardedReason"), Is.Empty, "Device 5 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 1 Description mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 2 Description mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 3 Name mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Device 4 Name mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Device 4 discarded reason mismatch.");
			});
		}

		[Test]
		public void Ctl1_iPhone ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var appManifestXml =
			"""
			<?xml version="1.0" encoding="UTF-8"?>
			<plist>
				<dict>
					<key>UIDeviceFamily</key>
					<array>
						<integer>1</integer>
					</array>
				</dict>
			</plist>
			""";
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1, appManifestXml);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (5), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (4), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.Devices [2].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Device 3 Name mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 3 OSVersion mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("DiscardedReason"), Is.Empty, "Device 3 discarded reason mismatch.");

				Assert.That (task.Devices [3].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 4 Name mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 4 OSVersion mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("DiscardedReason"), Is.Empty, "Device 4 discarded reason mismatch.");

				Assert.That (task.Devices [4].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Device 5 UDID mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Device 5 Name mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Device 5 OSVersion mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Device 5 UDID mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [4].GetMetadata ("DiscardedReason"), Is.Empty, "Device 5 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 1 Description mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 2 Description mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 3 Name mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 4 Name mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Discarded Device 4 discarded reason mismatch.");
			});
		}

		[Test]
		public void Ctl1_iPad ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var appManifestXml =
			"""
			<?xml version="1.0" encoding="UTF-8"?>
			<plist>
				<dict>
					<key>UIMinimumOSVersion</key>
					<string>16.0</string>
					<key>UIDeviceFamily</key>
					<array>
						<integer>2</integer>
					</array>
				</dict>
			</plist>
			""";
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1, appManifestXml);


			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (2), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (7), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Device 1 Description mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 1 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Discarded Device 1 Description mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not an iPad, but the app only supports iPads"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Discarded Device 2 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Discarded Device 2 Description mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not an iPad, but the app only supports iPads"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 3 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 3 Description mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 4 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 4 Description mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 4 reason mismatch.");

				Assert.That (task.DiscardedDevices [4].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 5 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 5 Description mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 5 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 5 reason mismatch.");

				Assert.That (task.DiscardedDevices [5].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 6 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Discarded Device 6 Description mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 6 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 6 UDID mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 6 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not an iPad, but the app only supports iPads"), "Discarded Device 6 reason mismatch.");

				Assert.That (task.DiscardedDevices [6].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 7 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 7 Description mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 7 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 7 UDID mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 7 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Discarded Device 7 reason mismatch.");
			});
		}

		[Test]
		public void Ctl1_OSVersion ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var appManifestPath = Path.Combine (tmpdir, "Info.plist");
			var appManifestXml =
			"""
			<?xml version="1.0" encoding="UTF-8"?>
			<plist>
				<dict>
					<key>MinimumOSVersion</key>
					<string>26.0</string>
				</dict>
			</plist>
			""";
			File.WriteAllText (appManifestPath, appManifestXml);
			task.AppBundleManifestPath = appManifestPath;

			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (4), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (5), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.Devices [2].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Device 3 Name mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 3 OSVersion mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("DiscardedReason"), Is.Empty, "Device 3 discarded reason mismatch.");

				Assert.That (task.Devices [3].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 4 Name mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 4 OSVersion mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 4 UDID mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [3].GetMetadata ("DiscardedReason"), Is.Empty, "Device 4 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Discarded Device 1 Name mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device OS version '18.7.1' is lower than the app's minimum OS version '26.0'"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 2 Name mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 3 Name mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 4 Name mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 4 reason mismatch.");

				Assert.That (task.DiscardedDevices [4].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 5 Name mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 5 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Discarded Device 5 reason mismatch.");
			});
		}

		[Test]
		public void Ctl1_RuntimeIdentifier ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1);

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var appManifestPath = Path.Combine (tmpdir, "Info.plist");
			var appManifestXml =
			"""
			<?xml version="1.0" encoding="UTF-8"?>
			<plist>
				<dict>
				</dict>
			</plist>
			""";
			File.WriteAllText (appManifestPath, appManifestXml);
			task.AppBundleManifestPath = appManifestPath;
			task.RuntimeIdentifier = $"ios-arm64";

			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (3), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (6), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Device 2 Name mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Device 2 OSVersion mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Device 2 UDID mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("DiscardedReason"), Is.Empty, "Device 2 discarded reason mismatch.");

				Assert.That (task.Devices [2].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Device 3 Name mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Device 3 OSVersion mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Device 3 UDID mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [2].GetMetadata ("DiscardedReason"), Is.Empty, "Device 3 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 1 Name mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Discarded Device 2 Name mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'tvOS' does not match the requested platform 'iOS'"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 3 Name mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Discarded Device 4 Name mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device runtime identifier(s) 'iossimulator-arm64' incompatible with the requested runtime identifier 'ios-arm64'"), "Discarded Device 4 reason mismatch.");

				Assert.That (task.DiscardedDevices [4].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 5 Name mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 5 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device runtime identifier(s) 'iossimulator-arm64' incompatible with the requested runtime identifier 'ios-arm64'"), "Discarded Device 5 reason mismatch.");

				Assert.That (task.DiscardedDevices [5].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 6 UDID mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 6 Name mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 6 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 6 UDID mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 6 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Discarded Device 6 reason mismatch.");
			});
		}

		[Test]
		public void Ctl1_AppleTV ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64"); // because the set of available simulators is different on x64

			var platform = ApplePlatform.TVOS;
			var task = CreateTask (platform, SIMCTL_JSON_1, DEVICECTL_JSON_1);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (1), "Devices count mismatch.");
				Assert.That (task.DiscardedDevices.Count, Is.EqualTo (8), "Discarded device count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Device 1 ItemSpec mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("Apple TV - tvOS 26.1 (Shutdown)"), "Device 1 Description mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("60ED31BD-80CE-420A-B0CB-756D2CD38201"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("tvossimulator-arm64"), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");

				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("00008001-012301230123ABCD"), "Discarded Device 1 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPad Pro 3rd Gen - iOS 26.0"), "Discarded Device 1 Description mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("00008001-012301230123ABCD"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'iOS' does not match the requested platform 'tvOS'"), "Discarded Device 1 reason mismatch.");

				Assert.That (task.DiscardedDevices [1].ItemSpec, Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 2 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 13 - iOS 18.7.1"), "Discarded Device 2 Description mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("OSVersion"), Is.EqualTo ("18.7.1"), "Discarded Device 2 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("UDID"), Is.EqualTo ("00008002-012301230123ABCD"), "Discarded Device 2 UDID mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 2 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [1].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'iOS' does not match the requested platform 'tvOS'"), "Discarded Device 2 reason mismatch.");

				Assert.That (task.DiscardedDevices [2].ItemSpec, Is.EqualTo ("00008003-012301230123ABCD"), "Discarded Device 3 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("Description"), Is.EqualTo ("Rolf's iPhone 15 - iOS 26.1"), "Discarded Device 3 Description mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 3 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("UDID"), Is.EqualTo ("00008003-012301230123ABCD"), "Discarded Device 3 UDID mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("ios-arm64"), "Discarded Device 3 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [2].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'iOS' does not match the requested platform 'tvOS'"), "Discarded Device 3 reason mismatch.");

				Assert.That (task.DiscardedDevices [3].ItemSpec, Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 4 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("Description"), Is.EqualTo ("Rolf’s Apple Watch Series 7 - watchOS 11.5"), "Discarded Device 4 Description mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("OSVersion"), Is.EqualTo ("11.5"), "Discarded Device 4 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("UDID"), Is.EqualTo ("00008004-012301230123ABCD"), "Discarded Device 4 UDID mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 4 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [3].GetMetadata ("DiscardedReason"), Is.EqualTo ("'appleWatch' devices are not supported"), "Discarded Device 4 reason mismatch.");

				Assert.That (task.DiscardedDevices [4].ItemSpec, Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 5 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - 26.0 (Shutdown)"), "Discarded Device 5 Description mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("OSVersion"), Is.EqualTo ("26.0"), "Discarded Device 5 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("UDID"), Is.EqualTo ("D4D95709-144A-4CAA-8469-89566EC1C935"), "Discarded Device 5 UDID mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 5 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [4].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device is not available: runtime profile not found using \"System\" match policy"), "Discarded Device 5 reason mismatch.");

				Assert.That (task.DiscardedDevices [5].ItemSpec, Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 6 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Discarded Device 6 Description mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 6 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("UDID"), Is.EqualTo ("D40CE982-3E65-4756-8162-90EFE50AF7FA"), "Discarded Device 6 UDID mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 6 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [5].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'iOS' does not match the requested platform 'tvOS'"), "Discarded Device 6 reason mismatch.");

				Assert.That (task.DiscardedDevices [6].ItemSpec, Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Discarded Device 7 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 13-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 7 Description mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 7 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("UDID"), Is.EqualTo ("3F1C114D-FC3D-481A-9CA1-499EE1339390"), "Discarded Device 7 UDID mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 7 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [6].GetMetadata ("DiscardedReason"), Is.EqualTo ("Device platform 'iOS' does not match the requested platform 'tvOS'"), "Discarded Device 7 reason mismatch.");

				Assert.That (task.DiscardedDevices [7].ItemSpec, Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 8 ItemSpec mismatch.");
				Assert.That (task.DiscardedDevices [7].GetMetadata ("Description"), Is.EqualTo ("iPad Pro 11-inch (M5) - iOS 26.1 (Shutdown)"), "Discarded Device 8 Description mismatch.");
				Assert.That (task.DiscardedDevices [7].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Discarded Device 8 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [7].GetMetadata ("UDID"), Is.EqualTo ("F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4"), "Discarded Device 8 UDID mismatch.");
				Assert.That (task.DiscardedDevices [7].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded Device 8 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [7].GetMetadata ("DiscardedReason"), Is.EqualTo ("Unknown device type identifier 'com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB'"), "Discarded Device 8 reason mismatch.");
			});
		}

		[Test]
		[TestCase ("iossimulator-arm64", "iossimulator-arm64")]
		[TestCase ("", "iossimulator-arm64")]
		public void SimCtl_MultiArch_RuntimeIdentifier (string runtimeIdentifier, string expectedRid)
		{
			if (!GetAvailableDevices.CanRunArm64)
				Assert.Ignore ("arm64 simulators require an Apple Silicon Mac.");
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_MULTIARCH, "");
			task.RuntimeIdentifier = runtimeIdentifier;
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (1), "Devices count mismatch.");

				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"), "Device 1 mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 1 Name mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("OSVersion"), Is.EqualTo ("26.1"), "Device 1 OSVersion mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("UDID"), Is.EqualTo ("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"), "Device 1 UDID mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (expectedRid), "Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("DiscardedReason"), Is.Empty, "Device 1 discarded reason mismatch.");
			});
		}

		[Test]
		public void SimCtl_MultiArch_UnsupportedRuntimeIdentifier ()
		{
			var task = CreateTask (ApplePlatform.iOS, SIMCTL_JSON_MULTIARCH, "");
			task.RuntimeIdentifier = "iossimulator-x64";
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.That (task.Devices, Is.Empty, "Devices");
			Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo ("iossimulator-arm64"), "Discarded simulator RuntimeIdentifier");
			Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.AnyOf ("Device runtime identifier(s) 'iossimulator-arm64' incompatible with the requested runtime identifier 'iossimulator-x64'", "Can't run an arm64 simulator on an x86_64 macOS desktop."), "Discarded reason");
		}

		[Test]
		public void SimCtl_X64OnlySimulator ()
		{
			var task = CreateTask (ApplePlatform.iOS, SIMCTL_JSON_MULTIARCH.Replace ("\"arm64\",", ""), "");
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.That (task.Devices, Is.Empty, "Devices");
			Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("Simulator does not support arm64."), "Discarded reason");
		}

		[Test]
		public void DeviceCtl2_Mac ()
		{
			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, "", DEVICECTL_JSON_2);
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");

			Assert.Multiple (() => {
				Assert.That (task.DiscardedDevices [0].ItemSpec, Is.EqualTo ("12345678-1234-1234-ABCD-1234567980AB"), "Discarded Device 1 itemspec mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("Description"), Is.EqualTo (""), "Discarded Device 1 Name mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("OSVersion"), Is.EqualTo (""), "Discarded Device 1 OSVersion mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("UDID"), Is.EqualTo ("12345678-1234-1234-ABCD-1234567980AB"), "Discarded Device 1 UDID mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("RuntimeIdentifier"), Is.EqualTo (""), "Discarded Device 1 RuntimeIdentifier mismatch.");
				Assert.That (task.DiscardedDevices [0].GetMetadata ("DiscardedReason"), Is.EqualTo ("'mac' devices are not supported"), "Discarded Device 1 reason mismatch.");
			});
		}

		[Test]
		public void BootedSimulatorsFirst ()
		{
			if (!Configuration.CanRunArm64)
				Assert.Ignore ("This test currently only works on arm64");

			var platform = ApplePlatform.iOS;
			var task = CreateTask (platform, SIMCTL_JSON_BOOTED, "");
			Assert.That (task.Execute (), Is.True, "Task should have succeeded.");
			Assert.Multiple (() => {
				Assert.That (task.Devices.Count, Is.EqualTo (2), "Devices count mismatch.");

				// The booted simulator should come first, even though alphabetically "iPhone 11" < "iPhone 17 Pro"
				Assert.That (task.Devices [0].ItemSpec, Is.EqualTo ("22222222-2222-2222-2222-222222222222"), "Device 1 mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Description"), Is.EqualTo ("iPhone 17 Pro - iOS 26.1 (Booted)"), "Device 1 Description mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("Status"), Is.EqualTo ("Booted"), "Device 1 Status mismatch.");
				Assert.That (task.Devices [0].GetMetadata ("State"), Is.EqualTo ("Booted"), "Device 1 State mismatch.");

				Assert.That (task.Devices [1].ItemSpec, Is.EqualTo ("11111111-1111-1111-1111-111111111111"), "Device 2 mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Description"), Is.EqualTo ("iPhone 11 - iOS 26.1 (Shutdown)"), "Device 2 Description mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("Status"), Is.EqualTo ("Shutdown"), "Device 2 Status mismatch.");
				Assert.That (task.Devices [1].GetMetadata ("State"), Is.EqualTo ("Shutdown"), "Device 2 State mismatch.");
			});
		}

		const string DEVICECTL_JSON_VERSION_5 =
		"""
		{
			"info": {
				"jsonVersion": 5
			},
			"result": {
				"devices": [
					{
						"_deprecationNotice": {
							"deprecatedFields": [ "hardwareProperties", "deviceProperties", "connectionProperties" ],
							"replacement": "properties"
						},
						"identifier": "33333333-AAAA-BBBB-CCCC-DDDDDDDDDDDD",
						"properties": {
							"device": {
								"name": "Rolf's iPhone 15"
							},
							"hardware": {
								"cpuType": {
									"name": "arm64e"
								},
								"deviceType": "iPhone",
								"platform": "iOS",
								"udid": "00008003-012301230123ABCD"
							},
							"state": {
								"osVersionNumber": "27.0",
								"pairingState": "paired"
							}
						}
					}
				]
			}
		}
		""";

		const string DEVICECTL_JSON_1 =
		"""
		{
		"info" : {
			"arguments" : [
			"devicectl",
			"list",
			"devices",
			"--json-output",
			"x.json"
			],
			"commandType" : "devicectl.list.devices",
			"environment" : {
			"TERM" : "xterm-256color"
			},
			"jsonVersion" : 2,
			"outcome" : "success",
			"version" : "477.39"
		},
		"result" : {
			"devices" : [
			{
				"connectionProperties" : {
				"tunnelState" : "unavailable"
				},
				"deviceProperties" : {
				"name" : "Rolf's iPad Pro 3rd Gen",
				"osBuildUpdate" : "23A341",
				"osVersionNumber" : "26.0",
				"releaseType" : "Beta"
				},
				"hardwareProperties" : {
				"cpuType" : {
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
				},
				"deviceType" : "iPad",
				"ecid" : 18446744073709551615,
				"hardwareModel" : "J317AP",
				"internalStorageCapacity" : 64000000000,
				"isProductionFused" : true,
				"marketingName" : "iPad Pro (11-inch)",
				"platform" : "iOS",
				"productType" : "iPad8,1",
				"reality" : "physical",
				"serialNumber" : "SERIAL_1",
				"supportedCPUTypes" : [
					{
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
					},
					{
					"name" : "arm64",
					"subType" : 0,
					"type" : 16777228
					}
				],
				"supportedDeviceFamilies" : [
					1,
					2
				],
				"thinningProductType" : "iPad8,1",
				"udid" : "00008001-012301230123ABCD"
				},
				"identifier" : "11111111-AAAA-BBBB-CCCC-DDDDDDDDDDDD",
				"tags" : [

				],
				"visibilityClass" : "default"
			},
			{
				"connectionProperties" : {
				"pairingState" : "paired",
				},
				"deviceProperties" : {
				"bootState" : "booted",
				"name" : "Rolf's iPhone 13",
				"osBuildUpdate" : "22H31",
				"osVersionNumber" : "18.7.1"
				},
				"hardwareProperties" : {
				"cpuType" : {
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
				},
				"deviceType" : "iPhone",
				"ecid" : 0,
				"hardwareModel" : "D63AP",
				"internalStorageCapacity" : 128000000000,
				"isProductionFused" : true,
				"marketingName" : "iPhone 13 Pro",
				"platform" : "iOS",
				"productType" : "iPhone14,2",
				"reality" : "physical",
				"serialNumber" : "SERIAL_2",
				"supportedCPUTypes" : [
					{
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
					},
					{
					"name" : "arm64",
					"subType" : 0,
					"type" : 16777228
					},
					{
					"name" : "armv8",
					"subType" : 13,
					"type" : 12
					}
				],
				"supportedDeviceFamilies" : [
					1
				],
				"thinningProductType" : "iPhone14,2",
				"udid" : "00008002-012301230123ABCD"
				},
				"identifier" : "22222222-AAAA-BBBB-CCCC-DDDDDDDDDDDD",
				"tags" : [

				],
				"visibilityClass" : "default"
			},
			{
				"connectionProperties" : {
				"pairingState" : "paired",
				"tunnelState" : "unavailable"
				},
				"deviceProperties" : {
				"name" : "Rolf's iPhone 15",
				"osBuildUpdate" : "23B85",
				"osVersionNumber" : "26.1"
				},
				"hardwareProperties" : {
				"cpuType" : {
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
				},
				"deviceType" : "iPhone",
				"ecid" : 1,
				"hardwareModel" : "D83AP",
				"internalStorageCapacity" : 128000000000,
				"isProductionFused" : true,
				"marketingName" : "iPhone 15 Pro",
				"platform" : "iOS",
				"productType" : "iPhone16,1",
				"reality" : "physical",
				"serialNumber" : "SERIAL_3",
				"supportedCPUTypes" : [
					{
					"name" : "arm64e",
					"subType" : 2,
					"type" : 16777228
					},
					{
					"name" : "arm64",
					"subType" : 0,
					"type" : 16777228
					}
				],
				"supportedDeviceFamilies" : [
					1
				],
				"thinningProductType" : "iPhone16,1",
				"udid" : "00008003-012301230123ABCD"
				},
				"identifier" : "33333333-AAAA-BBBB-CCCC-DDDDDDDDDDDD",
				"tags" : [

				],
				"visibilityClass" : "default"
			},
			{
				"connectionProperties" : {
				"pairingState" : "paired",
				"transportType" : "localNetwork",
				"tunnelState" : "disconnected",
				"tunnelTransportProtocol" : "tcp"
				},
				"deviceProperties" : {
				"name" : "Rolf’s Apple Watch Series 7",
				"osBuildUpdate" : "22T572",
				"osVersionNumber" : "11.5",
				},
				"hardwareProperties" : {
				"cpuType" : {
					"name" : "arm64_32",
					"subType" : 1,
					"type" : 33554444
				},
				"deviceType" : "appleWatch",
				"ecid" : 2,
				"hardwareModel" : "N187sAP",
				"isProductionFused" : true,
				"marketingName" : "Apple Watch Series 7",
				"platform" : "watchOS",
				"productType" : "Watch6,6",
				"reality" : "physical",
				"serialNumber" : "SERIAL_4",
				"thinningProductType" : "Watch6,6",
				"udid" : "00008004-012301230123ABCD"
				},
				"identifier" : "44444444-AAAA-BBBB-CCCC-DDDDDDDDDDDD",
				"tags" : [

				],
				"visibilityClass" : "default"
			}
			]
		}
		}
		""";

		const string DEVICECTL_JSON_2 =
		"""
		{
			"result" : {
				"devices" : [
				{
					"connectionProperties" : {
					"isMobileDeviceOnly" : false,
					"pairingState" : "unsupported",
					"potentialHostnames" : [
						"12345678-1234-1234-ABCD-1234567980AB.coredevice.local"
					],
					"tunnelState" : "unavailable"
					},
					"deviceProperties" : {
					"bootState" : "booted",
					"ddiServicesAvailable" : false,
					"providerSpecificValues" : {
						"hasAMRestorableDeviceRef" : "true"
					}
					},
					"hardwareProperties" : {
					"deviceType" : "mac",
					"platform" : "macOS",
					"productType" : "MacBookPro17,1"
					},
					"identifier" : "12345678-1234-1234-ABCD-1234567980AB",
					"visibilityClass" : "default"
				}
				]
			}
		}
		""";

		const string SIMCTL_JSON_1 =
		"""
		{
			"devicetypes" : [
				{
				"productFamily" : "iPhone",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 11.simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
				"modelIdentifier" : "iPhone12,1",
				"minRuntimeVersionString" : "13.0.0",
				"minRuntimeVersion" : 851968,
				"name" : "iPhone 11"
				},
				{
				"productFamily" : "iPad",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPad Pro 13-inch (M5).simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPad-Pro-13-inch-M5-12GB",
				"modelIdentifier" : "iPad17,4",
				"minRuntimeVersionString" : "26.0.0",
				"minRuntimeVersion" : 1703936,
				"name" : "iPad Pro 13-inch (M5)"
				},
				{
				"productFamily" : "Apple TV",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/Apple TV 4K (3rd generation).simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-4K",
				"modelIdentifier" : "AppleTV14,1",
				"minRuntimeVersionString" : "16.1.0",
				"minRuntimeVersion" : 1048832,
				"name" : "Apple TV 4K (3rd generation)"
				},
			],
			"runtimes" : [
				{
				"isAvailable" : true,
				"version" : "26.1",
				"isInternal" : false,
				"buildversion" : "23B80",
				"supportedArchitectures" : [
					"arm64"
				],
				"supportedDeviceTypes" : [
					{
					"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 17 Pro.simdevicetype",
					"name" : "iPhone 17 Pro",
					"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro",
					"productFamily" : "iPhone"
					}
				],
				"identifier" : "com.apple.CoreSimulator.SimRuntime.iOS-26-1",
				"platform" : "iOS",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime",
				"runtimeRoot" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime\/Contents\/Resources\/RuntimeRoot",
				"lastUsage" : {
					"arm64" : "2025-11-11T18:49:51Z"
				},
				"name" : "iOS 26.1"
				},
				{
				"isAvailable" : true,
				"version" : "26.1",
				"isInternal" : false,
				"buildversion" : "23J579",
				"supportedArchitectures" : [
					"arm64"
				],
				"supportedDeviceTypes" : [
					{
					"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/Apple TV 4K (3rd generation).simdevicetype",
					"name" : "Apple TV 4K (3rd generation)",
					"identifier" : "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-4K",
					"productFamily" : "Apple TV"
					},
				],
				"identifier" : "com.apple.CoreSimulator.SimRuntime.tvOS-26-1",
				"platform" : "tvOS",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Volumes\/tvOS_23J579\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/tvOS 26.1.simruntime",
				"runtimeRoot" : "\/Library\/Developer\/CoreSimulator\/Volumes\/tvOS_23J579\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/tvOS 26.1.simruntime\/Contents\/Resources\/RuntimeRoot",
				"lastUsage" : {
					"arm64" : "2025-11-11T18:49:38Z"
				},
				"name" : "tvOS 26.1"
				}
			],
			"devices" : {
				"com.apple.CoreSimulator.SimRuntime.tvOS-26-1" : [
					{
						"lastBootedAt" : "2025-11-03T15:42:10Z",
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/60ED31BD-80CE-420A-B0CB-756D2CD38201\/data",
						"dataPathSize" : 1172033536,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/60ED31BD-80CE-420A-B0CB-756D2CD38201",
						"udid" : "60ED31BD-80CE-420A-B0CB-756D2CD38201",
						"isAvailable" : true,
						"logPathSize" : 266240,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.Apple-TV-4K-3rd-generation-4K",
						"state" : "Shutdown",
						"name" : "Apple TV - tvOS 26.1"
					}
				],
				"com.apple.CoreSimulator.SimRuntime.iOS-26-0" : [
					{
						"lastBootedAt" : "2025-11-04T08:20:55Z",
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/D4D95709-144A-4CAA-8469-89566EC1C935\/data",
						"dataPathSize" : 1880317952,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/D4D95709-144A-4CAA-8469-89566EC1C935",
						"udid" : "D4D95709-144A-4CAA-8469-89566EC1C935",
						"isAvailable" : false,
						"availabilityError" : "runtime profile not found using \"System\" match policy",
						"logPathSize" : 253952,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro",
						"state" : "Shutdown",
						"name" : "iPhone 17 Pro"
					},
				],
				"com.apple.CoreSimulator.SimRuntime.iOS-26-1" : [
					{
						"lastBootedAt" : "2025-11-06T12:53:03Z",
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/D40CE982-3E65-4756-8162-90EFE50AF7FA\/data",
						"dataPathSize" : 2274861056,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/D40CE982-3E65-4756-8162-90EFE50AF7FA",
						"udid" : "D40CE982-3E65-4756-8162-90EFE50AF7FA",
						"isAvailable" : true,
						"logPathSize" : 253952,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
						"state" : "Shutdown",
						"name" : "iPhone 11 - iOS 26.1"
					},
					{
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/3F1C114D-FC3D-481A-9CA1-499EE1339390\/data",
						"dataPathSize" : 18337792,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/3F1C114D-FC3D-481A-9CA1-499EE1339390",
						"udid" : "3F1C114D-FC3D-481A-9CA1-499EE1339390",
						"isAvailable" : true,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPad-Pro-13-inch-M5-12GB",
						"state" : "Shutdown",
						"name" : "iPad Pro 13-inch (M5)"
					},
					{
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4\/data",
						"dataPathSize" : 18337792,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4",
						"udid" : "F8BEDB0B-441A-4D05-AED6-E9724DEA6BF4",
						"isAvailable" : true,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPad-Pro-11-inch-M5-12GB",
						"state" : "Shutdown",
						"name" : "iPad Pro 11-inch (M5)"
					}
				]
			},
			"pairs" : {

			}
		}
		""";

		const string SIMCTL_JSON_MULTIARCH =
		"""
		{
			"devicetypes" : [
				{
				"productFamily" : "iPhone",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 11.simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
				"modelIdentifier" : "iPhone12,1",
				"minRuntimeVersionString" : "13.0.0",
				"minRuntimeVersion" : 851968,
				"name" : "iPhone 11"
				}
			],
			"runtimes" : [
				{
				"isAvailable" : true,
				"version" : "26.1",
				"isInternal" : false,
				"buildversion" : "23B80",
				"supportedArchitectures" : [
					"arm64",
					"x86_64"
				],
				"supportedDeviceTypes" : [
					{
					"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 11.simdevicetype",
					"name" : "iPhone 11",
					"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
					"productFamily" : "iPhone"
					}
				],
				"identifier" : "com.apple.CoreSimulator.SimRuntime.iOS-26-1",
				"platform" : "iOS",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime",
				"runtimeRoot" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime\/Contents\/Resources\/RuntimeRoot",
				"name" : "iOS 26.1"
				}
			],
			"devices" : {
				"com.apple.CoreSimulator.SimRuntime.iOS-26-1" : [
					{
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE\/data",
						"dataPathSize" : 2274861056,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE",
						"udid" : "AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE",
						"isAvailable" : true,
						"logPathSize" : 253952,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
						"state" : "Shutdown",
						"name" : "iPhone 11 - iOS 26.1"
					}
				]
			},
			"pairs" : {

			}
		}
		""";

		const string SIMCTL_JSON_BOOTED =
		"""
		{
			"devicetypes" : [
				{
				"productFamily" : "iPhone",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 11.simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
				"modelIdentifier" : "iPhone12,1",
				"minRuntimeVersionString" : "13.0.0",
				"minRuntimeVersion" : 851968,
				"name" : "iPhone 11"
				},
				{
				"productFamily" : "iPhone",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 17 Pro.simdevicetype",
				"maxRuntimeVersion" : 4294967295,
				"maxRuntimeVersionString" : "65535.255.255",
				"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro",
				"modelIdentifier" : "iPhone18,3",
				"minRuntimeVersionString" : "26.0.0",
				"minRuntimeVersion" : 1703936,
				"name" : "iPhone 17 Pro"
				}
			],
			"runtimes" : [
				{
				"isAvailable" : true,
				"version" : "26.1",
				"isInternal" : false,
				"buildversion" : "23B80",
				"supportedArchitectures" : [
					"arm64"
				],
				"supportedDeviceTypes" : [
					{
					"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 11.simdevicetype",
					"name" : "iPhone 11",
					"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
					"productFamily" : "iPhone"
					},
					{
					"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Profiles\/DeviceTypes\/iPhone 17 Pro.simdevicetype",
					"name" : "iPhone 17 Pro",
					"identifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro",
					"productFamily" : "iPhone"
					}
				],
				"identifier" : "com.apple.CoreSimulator.SimRuntime.iOS-26-1",
				"platform" : "iOS",
				"bundlePath" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime",
				"runtimeRoot" : "\/Library\/Developer\/CoreSimulator\/Volumes\/iOS_23B80\/Library\/Developer\/CoreSimulator\/Profiles\/Runtimes\/iOS 26.1.simruntime\/Contents\/Resources\/RuntimeRoot",
				"name" : "iOS 26.1"
				}
			],
			"devices" : {
				"com.apple.CoreSimulator.SimRuntime.iOS-26-1" : [
					{
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/11111111-1111-1111-1111-111111111111\/data",
						"dataPathSize" : 2274861056,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/11111111-1111-1111-1111-111111111111",
						"udid" : "11111111-1111-1111-1111-111111111111",
						"isAvailable" : true,
						"logPathSize" : 253952,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-11",
						"state" : "Shutdown",
						"name" : "iPhone 11"
					},
					{
						"lastBootedAt" : "2025-11-06T12:53:03Z",
						"dataPath" : "\/Users\/rolf\/Library\/Developer\/CoreSimulator\/Devices\/22222222-2222-2222-2222-222222222222\/data",
						"dataPathSize" : 2274861056,
						"logPath" : "\/Users\/rolf\/Library\/Logs\/CoreSimulator\/22222222-2222-2222-2222-222222222222",
						"udid" : "22222222-2222-2222-2222-222222222222",
						"isAvailable" : true,
						"logPathSize" : 253952,
						"deviceTypeIdentifier" : "com.apple.CoreSimulator.SimDeviceType.iPhone-17-Pro",
						"state" : "Booted",
						"name" : "iPhone 17 Pro"
					}
				]
			},
			"pairs" : {

			}
		}
		""";
	}
}
