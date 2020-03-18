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
		static readonly string projectFilePath = Path.Combine (sampleProjectPath, "SystemXunit.csproj");

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
		Mock<ICrashSnapshotReporter> snapshotReporter;
		Mock<ILogs> logs;
		Mock<ILog> mainLog;

		ISimulatorsLoaderFactory simulatorsFactory;
		IDeviceLoaderFactory devicesFactory;
		ISimpleListenerFactory listenerFactory;
		ICrashSnapshotReporterFactory snapshotReporterFactory;

		[SetUp]
		public void SetUp ()
		{
			logs = new Mock<ILogs> ();
			logs.SetupGet (x => x.Directory).Returns (Path.Combine (outputPath, "logs"));

			processManager = new Mock<IProcessManager> ();
			simulators = new Mock<ISimulatorsLoader> ();
			devices = new Mock<IDeviceLoader> ();
			simpleListener = new Mock<ISimpleListener> ();
			snapshotReporter = new Mock<ICrashSnapshotReporter> ();

			var mock1 = new Mock<ISimulatorsLoaderFactory> ();
			mock1.Setup (m => m.CreateLoader ()).Returns (simulators.Object);
			simulatorsFactory = mock1.Object;

			var mock2 = new Mock<IDeviceLoaderFactory> ();
			mock2.Setup (m => m.CreateLoader ()).Returns (devices.Object);
			devicesFactory = mock2.Object;

			var mock3 = new Mock<ISimpleListenerFactory> ();
			mock3
				.Setup (m => m.Create (It.IsAny<RunMode>(), It.IsAny<ILog>(), It.IsAny<ILog>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns ((ListenerTransport.Tcp, simpleListener.Object, "listener-temp-file"));
			listenerFactory = mock3.Object;
			simpleListener.SetupGet (x => x.Port).Returns (1020);

			var mock4 = new Mock<ICrashSnapshotReporterFactory> ();
			mock4.Setup (m => m.Create (It.IsAny<ILog>(), It.IsAny<ILogs>(), It.IsAny<bool>(), It.IsAny<string>())).Returns (snapshotReporter.Object);
			snapshotReporterFactory = mock4.Object;

			mainLog = new Mock<ILog> ();

			Directory.CreateDirectory (appPath);
		}

		[Test]
		public void InitializeTest ()
		{
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				snapshotReporterFactory,
				AppRunnerTarget.Simulator_iOS64,
				Mock.Of<IHarness> (),
				mainLog.Object,
				logs.Object,
				projectFilePath:projectFilePath,
				buildConfiguration: "Debug");
			
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
				snapshotReporterFactory,
				AppRunnerTarget.Simulator_iOS64,
				Mock.Of<IHarness> (),
				mainLog.Object,
				logs.Object,
				projectFilePath:projectFilePath,
				buildConfiguration: "Debug");

			var exception = Assert.ThrowsAsync<InvalidOperationException> (
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
				snapshotReporterFactory,
				AppRunnerTarget.Simulator_iOS64,
				Mock.Of<IHarness> (),
				mainLog.Object,
				logs.Object,
				projectFilePath:projectFilePath,
				buildConfiguration: "Debug");

			var exception = Assert.ThrowsAsync<InvalidOperationException> (
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
				snapshotReporterFactory,
				AppRunnerTarget.Device_iOS,
				Mock.Of<IHarness> (),
				mainLog.Object,
				logs.Object,
				projectFilePath:projectFilePath,
				buildConfiguration: "Debug");

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
				snapshotReporterFactory,
				AppRunnerTarget.Device_iOS,
				Mock.Of<IHarness> (),
				mainLog.Object,
				logs.Object,
				projectFilePath:projectFilePath,
				buildConfiguration: "Debug");

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
				mainLog.Object,
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
				snapshotReporterFactory,
				AppRunnerTarget.Device_iOS,
				harnessMock.Object,
				mainLog.Object,
				logs.Object,
				projectFilePath: Path.Combine (sampleProjectPath, "SystemXunit.csproj"),
				buildConfiguration: "Debug");

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
				mainLog.Object,
				TimeSpan.FromMinutes (1),
				null,
				null));
		}

		[Test]
		public async Task RunOnSimulatorTest ()
		{
			const string xcodePath = "/path/to/xcode";
			const string mlaunchPath = "/path/to/mlaunch";

			var harness = Mock.Of<IHarness> (x =>
				x.XcodeRoot == xcodePath &&
				x.MlaunchPath == mlaunchPath &&
				x.Verbosity == 1 &&
				x.LogDirectory == logs.Object.Directory &&
				x.InCI == false &&
				x.EnvironmentVariables == new Dictionary<string, string> () {
					{ "env1", "value1" },
					{ "env2", "value2" },
				} && 
				x.Timeout == 1d &&
				x.GetStandardErrorTty() == "tty1");

			devices.Setup (x => x.ConnectedDevices).Returns (mockDevices.Reverse());

			// Crash reporter
			var crashReporterFactory = new Mock<ICrashSnapshotReporterFactory> ();
			crashReporterFactory
				.Setup (x => x.Create (mainLog.Object, It.IsAny<ILogs> (), false, null))
				.Returns (snapshotReporter.Object);

			// Mock finding simulators
			simulators
				.Setup (x => x.LoadAsync (It.IsAny<ILog> (), false, false))
				.Returns (Task.CompletedTask);

			string simulatorLogPath = Path.Combine (Path.GetTempPath (), "simulator-logs");
			simulators
				.Setup (x => x.FindAsync (AppRunnerTarget.Simulator_iOS64, mainLog.Object, true, false))
				.ReturnsAsync (new ISimulatorDevice [] {
					new SimDevice() {
						Name = "Test iPhone simulator",
						UDID = "58F21118E4D34FD69EAB7860BB9B38A0",
						LogPath = simulatorLogPath,
					}
				});

			var listenerLogFile = new Mock<ILogFile> ();

			logs
				.Setup (x => x.Create (It.IsAny<string> (), "TestLog", It.IsAny<bool> ()))
				.Returns (listenerLogFile.Object);

			simpleListener.SetupGet (x => x.ConnectedTask).Returns (Task.CompletedTask);

			var processResult = new ProcessExecutionResult () { ExitCode = 1 };

			var expectedArgs = $"--sdkroot {xcodePath} -v -v -argument=-connection-mode -argument=none " +
				$"-argument=-app-arg:-autostart -setenv=NUNIT_AUTOSTART=true -argument=-app-arg:-autoexit " +
				$"-setenv=NUNIT_AUTOEXIT=true -argument=-app-arg:-enablenetwork -setenv=NUNIT_ENABLE_NETWORK=true " +
				$"-setenv=DISABLE_SYSTEM_PERMISSION_TESTS=1 -argument=-app-arg:-hostname:127.0.0.1 " +
				$"-setenv=NUNIT_HOSTNAME=127.0.0.1 -argument=-app-arg:-transport:Tcp -setenv=NUNIT_TRANSPORT=TCP " +
				$"-argument=-app-arg:-hostport:{simpleListener.Object.Port} -setenv=NUNIT_HOSTPORT={simpleListener.Object.Port} " +
				$"-setenv=env1=value1 -setenv=env2=value2 --launchsim {appPath}";

			processManager
				.Setup (x => x.ExecuteCommandAsync (
					mlaunchPath,
					It.Is<IList<string>> (args => string.Join(" ", args) == expectedArgs),
					mainLog.Object,
					TimeSpan.FromMinutes (harness.Timeout * 2),
					null,
					It.IsAny<CancellationToken> ()))
				.ReturnsAsync (new ProcessExecutionResult () { ExitCode = 0 });

			// Act
			var appRunner = new AppRunner (processManager.Object,
				simulatorsFactory,
				listenerFactory,
				devicesFactory,
				crashReporterFactory.Object,
				AppRunnerTarget.Simulator_iOS64,
				harness,
				mainLog.Object,
				logs.Object,
				projectFilePath: projectFilePath,
				buildConfiguration: "Debug",
				timeoutMultiplier: 2);

			var result = await appRunner.RunAsync ();

			// Verify
			Assert.AreEqual (0, result);

			mainLog.Verify (x => x.WriteLine ("Test run started"));

			simpleListener.Verify (x => x.Initialize (), Times.AtLeastOnce);
			simpleListener.Verify (x => x.StartAsync (), Times.AtLeastOnce);

			simulators.VerifyAll ();

			// 

			/*processManager.Verify (x => x.ExecuteCommandAsync (
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
				mainLog.Object,
				TimeSpan.FromMinutes (1),
				null,
				null));*/
		}
	}
}
