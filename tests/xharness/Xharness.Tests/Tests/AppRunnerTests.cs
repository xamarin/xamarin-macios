using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xharness.Execution;
using Xharness.Hardware;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness.Tests {
	[TestFixture]
	public class AppRunnerTests {

		const string appName = "com.xamarin.bcltests.SystemXunit";

		static readonly string outputPath = Path.GetDirectoryName (Assembly.GetAssembly (typeof(AppRunnerTests)).Location);
		static readonly string sampleProjectPath = Path.Combine (outputPath, "Samples", "TestProject");
		static readonly string appPath = Path.Combine (sampleProjectPath, "bin", appName + ".app");

		static readonly IHardwareDevice [] mockDevices = new IHardwareDevice [] {
			new Device() {
				BuildVersion = "17A577",
				DeviceClass = DeviceClass.iPhone,
				DeviceIdentifier = "8A450AA31EA94191AD6B02455F377CC1",
				InterfaceType = "Usb",
				IsUsableForDebugging = true,
				Name = "Test iPhone",
				ProductType = "iPhone12,1",
				ProductVersion = "13.0",
				UDID = "58F21118E4D34FD69EAB7860BB9B38A0",
			},
			new Device() {
				BuildVersion = "13G36",
				DeviceClass = DeviceClass.iPad,
				DeviceIdentifier = "E854B2C3E7C8451BAF8053EC4DAAEE49",
				InterfaceType = "Usb",
				IsUsableForDebugging = true,
				Name = "Test iPad",
				ProductType = "iPad2,1",
				ProductVersion = "9.3.5",
				UDID = "51F3354D448D4814825D07DC5658C19B",
			}
		};

		Mock<IProcessManager> processManager;
		Mock<ISimulatorsLoader> simulators;
		Mock<IDeviceLoader> devices;
		Mock<ISimpleListener> simpleListener;

		ILog mainLog;
		ISimulatorsLoaderFactory simulatorsFactory;
		IDeviceLoaderFactory devicesFactory;
		ISimpleListenerFactory listenerFactory;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			simulators = new Mock<ISimulatorsLoader> ();
			devices = new Mock<IDeviceLoader> ();
			simpleListener = new Mock<ISimpleListener> ();

			var mock1 = new Mock<ISimulatorsLoaderFactory> ();
			mock1.Setup (m => m.CreateLoader ()).Returns (simulators.Object);
			simulatorsFactory = mock1.Object;

			var mock2 = new Mock<IDeviceLoaderFactory> ();
			mock2.Setup (m => m.CreateLoader ()).Returns (devices.Object);
			devicesFactory = mock2.Object;

			var mock3 = new Mock<ISimpleListenerFactory> ();
			mock3.Setup (m => m.Create (It.IsAny<RunMode>(), It.IsAny<ILog>(), It.IsAny<bool>())).Returns ((ListenerTransport.Tcp, simpleListener.Object, null));
			listenerFactory = mock3.Object;

			mainLog = new Mock<ILog> ().Object;

			Directory.CreateDirectory (appPath);
		}

		[Test]
		public void InitializeTest ()
		{
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Simulator_iOS64,
				new Mock<IHarness> ().Object,
				new Mock<ILog>().Object,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));
			
			Assert.AreEqual (appName, appRunner.AppInformation.AppName);
			Assert.AreEqual (appPath, appRunner.AppInformation.AppPath);
			Assert.AreEqual (appPath, appRunner.AppInformation.LaunchAppPath);
			Assert.AreEqual (appName, appRunner.AppInformation.BundleIdentifier);
		}

		[Test]
		public void InstallToSimulatorTest ()
		{
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Simulator_iOS64,
				new Mock<IHarness> ().Object,
				new Mock<ILog>().Object,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));

			var exception = Assert.ThrowsAsync<UnsupportedOperationException> (
				async () => await appRunner.InstallAsync (new CancellationToken ()),
				"Install should not be allowed on a simulator");
		}

		[Test]
		public void UninstallToSimulatorTest ()
		{
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Simulator_iOS64,
				new Mock<IHarness> ().Object,
				new Mock<ILog>().Object,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));

			var exception = Assert.ThrowsAsync<UnsupportedOperationException> (
				async () => await appRunner.UninstallAsync (),
				"Uninstall should not be allowed on a simulator");
		}

		[Test]
		public void InstallWhenNoDevicesTest ()
		{
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Device_iOS,
				new Mock<IHarness> ().Object,
				new Mock<ILog>().Object,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));

			devices.Setup (x => x.ConnectedDevices).Returns (new IHardwareDevice [0]);

			var exception = Assert.ThrowsAsync<NoDeviceFoundException> (
				async () => await appRunner.InstallAsync (new CancellationToken ()),
				"Install requires a connected devices");
		}

		[Test]
		public async Task InstallOnDeviceTest ()
		{
			Mock<IHarness> harnessMock = new Mock<IHarness> ();
			harnessMock.SetupGet (x => x.XcodeRoot).Returns ("/path/to/xcode");
			harnessMock.SetupGet (x => x.MlaunchPath).Returns ("/path/to/mlaunch");
			harnessMock.SetupGet (x => x.Verbosity).Returns (2);

			var processResult = new ProcessExecutionResult () { ExitCode = 1, TimedOut = false };

			processManager
				.Setup (x => x.ExecuteCommandAsync (
					 It.IsAny<string> (),
					 It.IsAny<IList<string>> (),
					 It.IsAny<ILog> (),
					 It.IsAny<TimeSpan> (),
					 It.IsAny<Dictionary<string, string>> (),
					 It.IsAny<CancellationToken> ()))
				.ReturnsAsync(processResult);

			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Device_iOS,
				harnessMock.Object,
				mainLog,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));

			devices.Setup (x => x.ConnectedDevices).Returns (mockDevices);

			CancellationToken cancellationToken = new CancellationToken ();
			var result = await appRunner.InstallAsync (cancellationToken);

			Assert.AreEqual (1, result.ExitCode);
			
			processManager.Verify (x => x.ExecuteCommandAsync (
				"/path/to/mlaunch",
				new List<string> () {
					"--sdkroot",
					"/path/to/xcode",
					"-v",
					"-v",
					"-v",
					"--installdev",
					appPath,
					"--devname",
					"Test iPad"
				},
				mainLog,
				TimeSpan.FromHours (1),
				null,
				cancellationToken));
		}

		[Test]
		public async Task UninstallFromDeviceTest ()
		{
			Mock<IHarness> harnessMock = new Mock<IHarness> ();
			harnessMock.SetupGet (x => x.XcodeRoot).Returns ("/path/to/xcode");
			harnessMock.SetupGet (x => x.MlaunchPath).Returns ("/path/to/mlaunch");
			harnessMock.SetupGet (x => x.Verbosity).Returns (1);

			var processResult = new ProcessExecutionResult () { ExitCode = 3, TimedOut = false };

			processManager
				.Setup (x => x.ExecuteCommandAsync (
					 It.IsAny<string> (),
					 It.IsAny<IList<string>> (),
					 It.IsAny<ILog> (),
					 It.IsAny<TimeSpan> (),
					 null,
					 null))
				.ReturnsAsync(processResult);

			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				AppRunnerTarget.Device_iOS,
				harnessMock.Object,
				mainLog,
				Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				"Debug",
				Path.Combine (outputPath, "logs"));

			devices.Setup (x => x.ConnectedDevices).Returns (mockDevices.Reverse());

			var result = await appRunner.UninstallAsync ();

			Assert.AreEqual (3, result.ExitCode);
			
			processManager.Verify (x => x.ExecuteCommandAsync (
				"/path/to/mlaunch",
				new List<string> () {
					"--sdkroot",
					"/path/to/xcode",
					"-v",
					"-v",
					"--uninstalldevbundleid",
					appName,
					"--devname",
					"Test iPad"
				},
				mainLog,
				TimeSpan.FromMinutes (1),
				null,
				null));
		}
	}
}
