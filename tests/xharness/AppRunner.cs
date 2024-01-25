using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.DotNet.XHarness.Common.CLI;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.Common.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.XmlResults;

using Xharness.Jenkins.TestTasks;

namespace Xharness {

	public class AppRunner {
		readonly IMlaunchProcessManager processManager;
		readonly ISimulatorLoaderFactory simulatorsLoaderFactory;
		readonly ISimpleListenerFactory listenerFactory;
		readonly IDeviceLoaderFactory devicesLoaderFactory;
		readonly ICrashSnapshotReporterFactory snapshotReporterFactory;
		readonly ICaptureLogFactory captureLogFactory;
		readonly IDeviceLogCapturerFactory deviceLogCapturerFactory;
		readonly ITestReporterFactory testReporterFactory;
		readonly IAppBundleInformationParser appBundleInformationParser;

		readonly RunMode runMode;
		readonly bool isSimulator;
		readonly TestTarget target;
		readonly IHarness harness;
		readonly double timeoutMultiplier;
		readonly IBuildToolTask buildTask;
		readonly string variation;
		readonly string projectFilePath;
		readonly string buildConfiguration;

		string deviceName;
		string companionDeviceName;
		ISimulatorDevice simulator;
		ISimulatorDevice companionSimulator;

		bool ensureCleanSimulatorState = true;
		bool EnsureCleanSimulatorState {
			get => ensureCleanSimulatorState && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("SKIP_SIMULATOR_SETUP"));
			set => ensureCleanSimulatorState = value;
		}

		public AppBundleInformation AppInformation { get; private set; }

		bool IsExtension => AppInformation.Extension.HasValue;

		public TestExecutingResult Result { get; private set; }

		public string FailureMessage { get; private set; }

		public IFileBackedLog MainLog { get; set; }

		public ILogs Logs { get; }

		public AppRunner (IMlaunchProcessManager processManager,
						  IAppBundleInformationParser appBundleInformationParser,
						  ISimulatorLoaderFactory simulatorsFactory,
						  ISimpleListenerFactory simpleListenerFactory,
						  IDeviceLoaderFactory devicesFactory,
						  ICrashSnapshotReporterFactory snapshotReporterFactory,
						  ICaptureLogFactory captureLogFactory,
						  IDeviceLogCapturerFactory deviceLogCapturerFactory,
						  ITestReporterFactory reporterFactory,
						  TestTarget target,
						  IHarness harness,
						  IFileBackedLog mainLog,
						  ILogs logs,
						  string projectFilePath,
						  string buildConfiguration,
						  ISimulatorDevice simulator = null,
						  ISimulatorDevice companionSimulator = null,
						  string deviceName = null,
						  string companionDeviceName = null,
						  bool ensureCleanSimulatorState = false,
						  double timeoutMultiplier = 1,
						  string variation = null,
						  IBuildToolTask buildTask = null)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.simulatorsLoaderFactory = simulatorsFactory ?? throw new ArgumentNullException (nameof (simulatorsFactory));
			this.listenerFactory = simpleListenerFactory ?? throw new ArgumentNullException (nameof (simpleListenerFactory));
			this.devicesLoaderFactory = devicesFactory ?? throw new ArgumentNullException (nameof (devicesFactory));
			this.snapshotReporterFactory = snapshotReporterFactory ?? throw new ArgumentNullException (nameof (snapshotReporterFactory));
			this.captureLogFactory = captureLogFactory ?? throw new ArgumentNullException (nameof (captureLogFactory));
			this.deviceLogCapturerFactory = deviceLogCapturerFactory ?? throw new ArgumentNullException (nameof (deviceLogCapturerFactory));
			this.testReporterFactory = reporterFactory ?? throw new ArgumentNullException (nameof (testReporterFactory));
			this.appBundleInformationParser = appBundleInformationParser ?? throw new ArgumentNullException (nameof (appBundleInformationParser));
			this.harness = harness ?? throw new ArgumentNullException (nameof (harness));
			this.MainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.Logs = logs ?? throw new ArgumentNullException (nameof (logs));
			this.timeoutMultiplier = timeoutMultiplier;
			this.deviceName = deviceName;
			this.companionDeviceName = companionDeviceName;
			this.ensureCleanSimulatorState = ensureCleanSimulatorState;
			this.simulator = simulator;
			this.companionSimulator = companionSimulator;
			this.buildTask = buildTask;
			this.target = target;
			this.variation = variation;
			this.projectFilePath = projectFilePath;
			this.buildConfiguration = buildConfiguration;

			runMode = target.ToRunMode ();
			isSimulator = target.IsSimulator ();
		}

		public async Task InitializeAsync ()
		{
			AppInformation = await appBundleInformationParser.ParseFromProject2 (harness.AppBundleLocator, projectFilePath, target, buildConfiguration);
			AppInformation.Variation = variation;
		}

		async Task<bool> FindSimulatorAsync ()
		{
			if (simulator is not null)
				return true;

			var sims = simulatorsLoaderFactory.CreateLoader ();
			await sims.LoadDevices (Logs.Create ($"simulator-list-{Harness.Helpers.Timestamp}.log", "Simulator list"), false, false);
			(simulator, companionSimulator) = await sims.FindSimulators (target.GetTargetOs (false), MainLog);

			return simulator is not null;
		}

		async Task FindDevice ()
		{
			if (deviceName is not null)
				return;

			deviceName = Environment.GetEnvironmentVariable ("DEVICE_NAME");
			if (!string.IsNullOrEmpty (deviceName))
				return;

			var devs = devicesLoaderFactory.CreateLoader ();
			await devs.LoadDevices (MainLog, false, false);

			var device = await devs.FindDevice (runMode, MainLog, false, false);

			deviceName = device?.Name;

			if (runMode == RunMode.WatchOS)
				companionDeviceName = (await devs.FindCompanionDevice (MainLog, device)).Name;
		}

		public async Task<ProcessExecutionResult> InstallAsync (CancellationToken cancellation_token)
		{
			if (isSimulator) {
				// We reset the simulator when running, so a separate install step does not make much sense.
				throw new InvalidOperationException ("Installing to a simulator is not supported.");
			}

			await FindDevice ();

			if (string.IsNullOrEmpty (deviceName))
				throw new NoDeviceFoundException ();

			var args = new MlaunchArguments ();

			for (int i = -1; i < harness.Verbosity; i++)
				args.Add (new VerbosityArgument ());

			args.Add (new InstallAppOnDeviceArgument (AppInformation.AppPath));
			args.Add (new DeviceNameArgument (companionDeviceName ?? deviceName));

			if (runMode == RunMode.WatchOS) {
				args.Add (new DeviceArgument ("ios,watchos"));
			}

			var totalSize = Directory.GetFiles (AppInformation.AppPath, "*", SearchOption.AllDirectories).Select ((v) => new FileInfo (v).Length).Sum ();
			MainLog.WriteLine ($"Installing '{AppInformation.AppPath}' to '{companionDeviceName ?? deviceName}'. Size: {totalSize} bytes = {totalSize / 1024.0 / 1024.0:N2} MB");

			return await processManager.ExecuteCommandAsync (args, MainLog, TimeSpan.FromHours (1), cancellationToken: cancellation_token);
		}

		public async Task<ProcessExecutionResult> UninstallAsync ()
		{
			if (isSimulator)
				throw new InvalidOperationException ("Uninstalling from a simulator is not supported.");

			await FindDevice ();

			var args = new MlaunchArguments ();

			for (int i = -1; i < harness.Verbosity; i++)
				args.Add (new VerbosityArgument ());

			args.Add (new UninstallAppFromDeviceArgument (AppInformation.BundleIdentifier));
			args.Add (new DeviceNameArgument (companionDeviceName ?? deviceName));

			return await processManager.ExecuteCommandAsync (args, MainLog, TimeSpan.FromMinutes (1));
		}

		public async Task<int> RunAsync ()
		{
			if (!isSimulator) {
				await FindDevice ();

				if (deviceName is null)
					throw new NoDeviceFoundException ();
			}

			var args = new MlaunchArguments ();

			for (int i = -1; i < harness.Verbosity; i++)
				args.Add (new VerbosityArgument ());

			args.Add (new SetAppArgumentArgument ("-connection-mode"));
			args.Add (new SetAppArgumentArgument ("none")); // This will prevent the app from trying to connect to any IDEs
			args.Add (new SetAppArgumentArgument ("-autostart", true));
			args.Add (new SetEnvVariableArgument ("NUNIT_AUTOSTART", true));
			args.Add (new SetAppArgumentArgument ("-autoexit", true));
			args.Add (new SetEnvVariableArgument ("NUNIT_AUTOEXIT", true));
			args.Add (new SetAppArgumentArgument ("-enablenetwork", true));
			args.Add (new SetEnvVariableArgument ("NUNIT_ENABLE_NETWORK", true));
			// detect if we are using a jenkins bot.
			var useXmlOutput = harness.InCI;
			if (useXmlOutput) {
				args.Add (new SetEnvVariableArgument ("NUNIT_ENABLE_XML_OUTPUT", true));
				args.Add (new SetEnvVariableArgument ("NUNIT_ENABLE_XML_MODE", "wrapped"));
				args.Add (new SetEnvVariableArgument ("NUNIT_XML_VERSION", "nunitv3"));
			}

			if (harness.InCI) {
				// We use the 'BUILD_REVISION' variable to detect whether we're running CI or not.
				args.Add (new SetEnvVariableArgument ("BUILD_REVISION", Environment.GetEnvironmentVariable ("BUILD_REVISION")));
			}

			if (!harness.GetIncludeSystemPermissionTests (TestPlatform.iOS, !isSimulator))
				args.Add (new SetEnvVariableArgument ("DISABLE_SYSTEM_PERMISSION_TESTS", 1));

			if (isSimulator) {
				args.Add (new SetAppArgumentArgument ("-hostname:127.0.0.1", true));
				args.Add (new SetEnvVariableArgument ("NUNIT_HOSTNAME", "127.0.0.1"));
			} else if (!listenerFactory.UseTunnel) {  // if is not simulator AND we are not using the tunnel, if we use the tunnel this is not needed
				var ips = new StringBuilder ();
				var ipAddresses = System.Net.Dns.GetHostEntry (System.Net.Dns.GetHostName ()).AddressList;
				for (int i = 0; i < ipAddresses.Length; i++) {
					if (i > 0)
						ips.Append (',');
					ips.Append (ipAddresses [i].ToString ());
				}

				var ipArg = ips.ToString ();
				args.Add (new SetAppArgumentArgument ($"-hostname:{ipArg}", true));
				args.Add (new SetEnvVariableArgument ("NUNIT_HOSTNAME", ipArg));
			}

			var listener_log = Logs.Create ($"test-{runMode.ToString ().ToLowerInvariant ()}-{Harness.Helpers.Timestamp}.log", LogType.TestLog.ToString (), timestamp: !useXmlOutput);
			var (transport, listener, listenerTmpFile) = listenerFactory.Create (runMode, MainLog, listener_log, isSimulator, true, useXmlOutput);

			var listenerPort = listener.InitializeAndGetPort ();

			args.Add (new SetAppArgumentArgument ($"-transport:{transport}", true));
			args.Add (new SetEnvVariableArgument ("NUNIT_TRANSPORT", transport.ToString ().ToUpper ()));

			if (transport == ListenerTransport.File)
				args.Add (new SetEnvVariableArgument ("NUNIT_LOG_FILE", listenerTmpFile));

			args.Add (new SetAppArgumentArgument ($"-hostport:{listenerPort}", true));
			args.Add (new SetEnvVariableArgument ("NUNIT_HOSTPORT", listenerPort));

			if (listenerFactory.UseTunnel)
				args.Add (new SetEnvVariableArgument ("USE_TCP_TUNNEL", true));

			listener.StartAsync ();

			// object that will take care of capturing and parsing the results
			ICrashSnapshotReporter crashReporter = snapshotReporterFactory.Create (MainLog, Logs, isDevice: !isSimulator, deviceName);

			var testReporterTimeout = TimeSpan.FromMinutes (harness.Timeout * timeoutMultiplier);
			var testReporter = testReporterFactory.Create (MainLog,
				MainLog,
				Logs,
				crashReporter,
				listener,
				new XmlResultParser (),
				AppInformation,
				runMode,
				harness.XmlJargon,
				deviceName,
				testReporterTimeout,
				buildTask?.Logs?.Directory,
				(level, message) => harness.Log (level, message));

			listener.ConnectedTask
				.TimeoutAfter (TimeSpan.FromMinutes (harness.LaunchTimeout))
				.ContinueWith (testReporter.LaunchCallback)
				.DoNotAwait ();

			args.AddRange (harness.EnvironmentVariables.Select (kvp => new SetEnvVariableArgument (kvp.Key, kvp.Value)));

			if (IsExtension) {
				switch (AppInformation.Extension) {
				case Extension.TodayExtension:
					args.Add (isSimulator
						? (MlaunchArgument) new LaunchSimulatorExtensionArgument (AppInformation.LaunchAppPath, AppInformation.BundleIdentifier)
						: new LaunchDeviceExtensionArgument (AppInformation.LaunchAppPath, AppInformation.BundleIdentifier));
					break;
				case Extension.WatchKit2:
				default:
					throw new NotImplementedException ();
				}
			} else {
				args.Add (isSimulator
					? (MlaunchArgument) new LaunchSimulatorAppArgument (AppInformation.LaunchAppPath)
					: new LaunchDeviceArgument (AppInformation.LaunchAppPath));
			}
			if (!isSimulator)
				args.Add (new DisableMemoryLimitsArgument ());

			if (isSimulator) {
				if (!await FindSimulatorAsync ())
					return 1;

				var stdout_log = Logs.CreateFile ($"stdout-{Harness.Helpers.Timestamp}.log", "Standard output");
				var stderr_log = Logs.CreateFile ($"stderr-{Harness.Helpers.Timestamp}.log", "Standard error");
				args.Add (new SetStdoutArgument (stdout_log));
				args.Add (new SetStderrArgument (stderr_log));

				var simulators = new [] { simulator, companionSimulator }.Where (s => s is not null);
				var systemLogs = new List<ICaptureLog> ();
				foreach (var sim in simulators) {
					// Upload the system log
					MainLog.WriteLine ("System log for the '{1}' simulator is: {0}", sim.SystemLog, sim.Name);
					bool isCompanion = sim != simulator;

					var logDescription = isCompanion ? LogType.CompanionSystemLog.ToString () : LogType.SystemLog.ToString ();
					var log = captureLogFactory.Create (
						Path.Combine (Logs.Directory, sim.Name + ".log"),
						sim.SystemLog,
						harness.Action != HarnessAction.Jenkins,
						logDescription);

					log.StartCapture ();
					Logs.Add (log);
					systemLogs.Add (log);
					WrenchLog.WriteLine ("AddFile: {0}", log.FullPath);
				}

				MainLog.WriteLine ("*** Executing {0}/{1} in the simulator ***", AppInformation.AppName, runMode);

				if (EnsureCleanSimulatorState) {
					foreach (var sim in simulators) {
						using var tcclog = Logs.Create ($"prepare-simulator-{Harness.Helpers.Timestamp}.log", "Simulator preparation");
						var rv = await sim.PrepareSimulator (tcclog, AppInformation.BundleIdentifier);
						tcclog.Description += rv ? " ✅ " : " (failed) ⚠️";
					}
				}

				MainLog.WriteLine ("Enabling verbose logging");
				foreach (var sim in simulators) {
					var udid = sim.UDID;
					await sim.Boot (MainLog, new CancellationToken ());
					await processManager.ExecuteXcodeCommandAsync ("simctl", new string [] { "logverbose", udid, "enable" }, MainLog, TimeSpan.FromMinutes (5));
					await sim.Shutdown (MainLog);
				}
				MainLog.WriteLine ("Enabled verbose logging");

				args.Add (new SimulatorUDIDArgument (simulator.UDID));

				await crashReporter.StartCaptureAsync ();

				MainLog.WriteLine ("Starting test run");

				var testRunResult = await processManager.ExecuteCommandAsync (args, MainLog, testReporterTimeout, cancellationToken: testReporter.CancellationToken);
				await testReporter.CollectSimulatorResult (testRunResult);

				// cleanup after us
				if (EnsureCleanSimulatorState)
					await simulator.KillEverything (MainLog);

				foreach (var log in systemLogs)
					log.StopCapture ();

				MainLog.WriteLine ("Disabling verbose logging");
				foreach (var sim in simulators) {
					var udid = sim.UDID;
					await processManager.ExecuteXcodeCommandAsync ("simctl", new string [] { "logverbose", udid, "disable" }, MainLog, TimeSpan.FromMinutes (5));
				}
				MainLog.WriteLine ("Disabledverbose logging");
			} else {
				MainLog.WriteLine ("*** Executing {0}/{1} on device '{2}' ***", AppInformation.AppName, runMode, deviceName);

				if (runMode == RunMode.WatchOS) {
					args.Add (new AttachNativeDebuggerArgument ()); // this prevents the watch from backgrounding the app.
				} else {
					args.Add (new WaitForExitArgument ());
				}

				args.Add (new DeviceNameArgument (deviceName));

				var deviceSystemLog = Logs.Create ($"device-{deviceName}-{Harness.Helpers.Timestamp}.log", "Device log");
				var deviceLogCapturer = deviceLogCapturerFactory.Create (harness.HarnessLog, deviceSystemLog, deviceName);
				deviceLogCapturer.StartCapture ();

				try {
					await crashReporter.StartCaptureAsync ();

					MainLog.WriteLine ("Starting test run");

					if (transport == ListenerTransport.Tcp && listenerFactory.UseTunnel && listener is SimpleTcpListener tcpListener) {
						// create a new tunnel using the listener
						var tunnel = listenerFactory.TunnelBore.Create (deviceName, MainLog);
						tunnel.Open (deviceName, tcpListener, testReporterTimeout, MainLog);
						// wait until we started the tunnel
						await tunnel.Started;
					}

					// We need to check for MT1111 (which means that mlaunch won't wait for the app to exit).
					var aggregatedLog = Log.CreateAggregatedLog (testReporter.CallbackLog, MainLog);
					ProcessExecutionResult runTestResults = await processManager.ExecuteCommandAsync (
						args,
						aggregatedLog,
						testReporterTimeout,
						cancellationToken: testReporter.CancellationToken);

					await testReporter.CollectDeviceResult (runTestResults);
				} finally {
					deviceLogCapturer.StopCapture ();
					deviceSystemLog.Dispose ();
				}

				// Upload the system log
				if (File.Exists (deviceSystemLog.FullPath)) {
					MainLog.WriteLine ("A capture of the device log is: {0}", deviceSystemLog.FullPath);
					WrenchLog.WriteLine ("AddFile: {0}", deviceSystemLog.FullPath);
				}
			}

			if (!listener.StopAsync ().Wait (TimeSpan.FromSeconds (5))) {
				MainLog.WriteLine ("Failed to stop listener within 5 seconds. Will cancel it.");
				listener.Cancel ();
			}
			listener.Dispose ();

			// close a tunnel if it was created
			if (!isSimulator && listenerFactory.UseTunnel)
				await listenerFactory.TunnelBore.Close (deviceName);

			// check the final status, copy all the required data
			(Result, FailureMessage) = await testReporter.ParseResult ();

			return testReporter.Success.Value ? 0 : 1;
		}
	}
}
