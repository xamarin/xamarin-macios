using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Moq;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class JenkinsDeviceLoadterTests {

		public class TestCasesData {
			public static IEnumerable GetDeviceTestCases {
				get {
					// set the mock expectations the expected results
					var simulators = new Mock<ISimulatorLoader> ();
					var aDevice = new Mock<IHardwareDevice> ();

					// no devices found
					var devices = new Mock<IHardwareDeviceLoader> ();
					devices.Setup (d => d.Connected32BitIOS).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.Connected64BitIOS).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.ConnectedTV).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.ConnectedWatch).Returns (Array.Empty<IHardwareDevice> ());

					yield return new TestCaseData (simulators.Object, devices.Object, $"Device Listing (ok - no devices found).");

					// iOS 32b
					devices = new Mock<IHardwareDeviceLoader> ();
					devices.Setup (d => d.Connected32BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.Connected64BitIOS).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.ConnectedTV).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.ConnectedWatch).Returns (Array.Empty<IHardwareDevice> ());

					yield return new TestCaseData (simulators.Object, devices.Object, $"Device Listing (ok). Devices types are: iOS 32 bit");

					devices = new Mock<IHardwareDeviceLoader> ();
					devices.Setup (d => d.Connected32BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.Connected64BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.ConnectedTV).Returns (Array.Empty<IHardwareDevice> ());
					devices.Setup (d => d.ConnectedWatch).Returns (Array.Empty<IHardwareDevice> ());

					yield return new TestCaseData (simulators.Object, devices.Object, $"Device Listing (ok). Devices types are: iOS 32 bit, iOS 64 bit");

					devices = new Mock<IHardwareDeviceLoader> ();
					devices.Setup (d => d.Connected32BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.Connected64BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.ConnectedTV).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.ConnectedWatch).Returns (Array.Empty<IHardwareDevice> ());
					yield return new TestCaseData (simulators.Object, devices.Object, $"Device Listing (ok). Devices types are: iOS 32 bit, iOS 64 bit, tvOS");

					devices = new Mock<IHardwareDeviceLoader> ();
					devices.Setup (d => d.Connected32BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.Connected64BitIOS).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.ConnectedTV).Returns (new IHardwareDevice [] { aDevice.Object });
					devices.Setup (d => d.ConnectedWatch).Returns (new IHardwareDevice [] { aDevice.Object });
					yield return new TestCaseData (simulators.Object, devices.Object, $"Device Listing (ok). Devices types are: iOS 32 bit, iOS 64 bit, tvOS, watchOS");
				}
			}

			public static IEnumerable GetSimulatorTestCases {
				get {
					var devices = new Mock<IHardwareDeviceLoader> ();
					var simulators = new Mock<ISimulatorLoader> ();
					var processManager = new Mock<IMlaunchProcessManager> ();
					var db = new Mock<ITCCDatabase> ();

					simulators.Setup (s => s.AvailableDevices).Returns (Array.Empty<SimulatorDevice> ());
					yield return new TestCaseData (simulators.Object, devices.Object, "Simulator Listing (ok - no simulators found).");

					simulators = new Mock<ISimulatorLoader> ();
					simulators.Setup (s => s.AvailableDevices).Returns (new SimulatorDevice [] { new SimulatorDevice (processManager.Object, db.Object) });
					yield return new TestCaseData (simulators.Object, devices.Object, $"Simulator Listing (ok - Found 1 simulators).");
				}
			}
		}

		Mock<ILogs> logs;
		Mock<IFileBackedLog> log;

		[SetUp]
		public void SetUp ()
		{
			logs = new Mock<ILogs> ();
			log = new Mock<IFileBackedLog> ();

			logs.Setup (l => l.Create (
				It.IsAny<string> (),
				It.Is<string> (s => s.Equals ("Simulator Listing", StringComparison.OrdinalIgnoreCase)),
				null)).Returns (log.Object);

			logs.Setup (l => l.Create (
				It.IsAny<string> (),
				It.Is<string> (s => s.Equals ("Device Listing", StringComparison.OrdinalIgnoreCase)),
				null)).Returns (log.Object);

			log.SetupSet (l => l.Description = It.IsAny<string> ()).Verifiable ();
		}

		[TearDown]
		public void TearDown ()
		{
			logs = null;
			log = null;
		}


		[Test, TestCaseSource (typeof (TestCasesData), "GetDeviceTestCases")]
		public async Task FoundDevicesTest (ISimulatorLoader simulators, IHardwareDeviceLoader devices, string expectedDescription)
		{
			var loader = new JenkinsDeviceLoader (simulators, devices, logs.Object);

			await loader.LoadDevicesAsync ();
			// validate that the log description will be set as expected
			log.VerifySet (l => l.Description = It.Is<string> (s => s.Equals (expectedDescription, StringComparison.OrdinalIgnoreCase)));
		}

		[Test, TestCaseSource (typeof (TestCasesData), "GetSimulatorTestCases")]
		public async Task FoundSimulatorsTest (ISimulatorLoader simulators, IHardwareDeviceLoader devices, string expectedDescription)
		{
			var loader = new JenkinsDeviceLoader (simulators, devices, logs.Object);

			await loader.LoadSimulatorsAsync ();
			// validate that the log description will be set as expected
			log.VerifySet (l => l.Description = It.Is<string> (s => s.Equals (expectedDescription, StringComparison.OrdinalIgnoreCase)));
		}

	}
}
