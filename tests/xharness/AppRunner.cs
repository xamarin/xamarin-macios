using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xharness.Hardware;
using Xharness.Execution;
using Xharness.Jenkins.TestTasks;
using Xharness.Listeners;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness {

	class AppRunner : IAppRunner
	{
		readonly IProcessManager processManager;
		readonly ISimulatorsLoaderFactory simulatorsLoaderFactory;
		readonly ISimpleListenerFactory listenerFactory;
		readonly IDeviceLoaderFactory devicesLoaderFactory;
		readonly ICrashSnapshotReporterFactory snapshotReporterFactory;
		readonly ICaptureLogFactory captureLogFactory;
		readonly IDeviceLogCapturerFactory deviceLogCapturerFactory;
		readonly ITestReporterFactory testReporterFactory;

		readonly RunMode runMode;
		readonly bool isSimulator;
		readonly TestTarget target;
		readonly IHarness harness;
		readonly string variation;
		readonly double timeoutMultiplier;
		readonly string logDirectory;

		string deviceName;
		string companionDeviceName;
		ISimulatorDevice [] simulators;
		ISimulatorDevice simulator => simulators [0];

		bool ensureCleanSimulatorState = true;
		bool EnsureCleanSimulatorState {
			get => ensureCleanSimulatorState && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("SKIP_SIMULATOR_SETUP"));
			set => ensureCleanSimulatorState = value;
		}

		public BuildToolTask BuildTask { get; private set; }

		public RunMode RunMode { get; private set; }

		bool IsExtension => AppInformation.Extension.HasValue;
		
		public AppBundleInformation AppInformation { get; }

		public TestExecutingResult Result { get; private set; }

		public string FailureMessage { get; private set; }

		public ILog MainLog { get; set; }

		public ILogs Logs { get; }

		public XmlResultJargon XmlJargon => harness.XmlJargon;
		public double LaunchTimeout => harness.LaunchTimeout;

		public AppRunner (IProcessManager processManager,
						  IAppBundleInformationParser appBundleInformationParser,
						  ISimulatorsLoaderFactory simulatorsFactory,
						  ISimpleListenerFactory simpleListenerFactory,
						  IDeviceLoaderFactory devicesFactory,
						  ICrashSnapshotReporterFactory snapshotReporterFactory,
						  ICaptureLogFactory captureLogFactory,
						  IDeviceLogCapturerFactory deviceLogCapturerFactory,
						  ITestReporterFactory reporterFactory,
						  TestTarget target,
						  IHarness harness,
						  ILog mainLog,
						  ILogs logs,
						  string projectFilePath,
						  string buildConfiguration,
						  ISimulatorDevice [] simulators = null,
						  string deviceName = null,
						  string companionDeviceName = null,
						  bool ensureCleanSimulatorState = false,
						  double timeoutMultiplier = 1,
						  string variation = null,
						  BuildToolTask buildTask = null)
		{
			if (appBundleInformationParser is null)
				throw new ArgumentNullException (nameof (appBundleInformationParser));

			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.simulatorsLoaderFactory = simulatorsFactory ?? throw new ArgumentNullException (nameof (simulatorsFactory));
			this.listenerFactory = simpleListenerFactory ?? throw new ArgumentNullException (nameof (simpleListenerFactory));
			this.devicesLoaderFactory = devicesFactory ?? throw new ArgumentNullException (nameof (devicesFactory));
			this.snapshotReporterFactory = snapshotReporterFactory ?? throw new ArgumentNullException (nameof (snapshotReporterFactory));
			this.captureLogFactory = captureLogFactory ?? throw new ArgumentNullException (nameof (captureLogFactory));
			this.deviceLogCapturerFactory = deviceLogCapturerFactory ?? throw new ArgumentNullException (nameof (deviceLogCapturerFactory));
			this.testReporterFactory = reporterFactory ?? throw new ArgumentNullException (nameof (testReporterFactory));
			this.harness = harness ?? throw new ArgumentNullException (nameof (harness));
			this.MainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.Logs = logs ?? throw new ArgumentNullException (nameof (logs));
			this.timeoutMultiplier = timeoutMultiplier;
			this.deviceName = deviceName;
			this.companionDeviceName = companionDeviceName;
			this.ensureCleanSimulatorState = ensureCleanSimulatorState;
			this.simulators = simulators;
			this.variation = variation;
			this.BuildTask = buildTask;
			this.target = target;

			RunMode = target.ToRunMode ();
			isSimulator = target.IsSimulator ();
			AppInformation = appBundleInformationParser.ParseFromProject (projectFilePath, target, buildConfiguration);
			AppInformation.Variation = variation;
		}

		async Task<bool> FindSimulatorAsync ()
		{
			if (simulators != null)
				return true;
			
			var sims = simulatorsLoaderFactory.CreateLoader();
			await sims.LoadAsync (Logs.Create ($"simulator-list-{Helpers.Timestamp}.log", "Simulator list"), false, false);
			simulators = await sims.FindAsync (target, MainLog);

			return simulators != null;
		}

		void FindDevice ()
		{
			if (deviceName != null)
				return;
			
			deviceName = Environment.GetEnvironmentVariable ("DEVICE_NAME");
			if (!string.IsNullOrEmpty (deviceName))
				return;
			
			var devs = devicesLoaderFactory.CreateLoader ();
			Task.Run (async () =>
			{
				await devs.LoadAsync (MainLog, false, false);
			}).Wait ();

			DeviceClass [] deviceClasses;
			switch (RunMode) {
			case RunMode.iOS:
				deviceClasses = new [] { DeviceClass.iPhone, DeviceClass.iPad, DeviceClass.iPod };
				break;
			case RunMode.WatchOS:
				deviceClasses = new [] { DeviceClass.Watch };
				break;
			case RunMode.TvOS:
				deviceClasses = new [] { DeviceClass.AppleTV }; // Untested
				break;
			default:
				throw new ArgumentException (nameof(RunMode));
			}

			var selected = devs.ConnectedDevices.Where ((v) => deviceClasses.Contains (v.DeviceClass) && v.IsUsableForDebugging != false);
			IHardwareDevice selected_data;
			if (selected.Count () == 0) {
				throw new NoDeviceFoundException ($"Could not find any applicable devices with device class(es): {string.Join (", ", deviceClasses)}");
			} else if (selected.Count () > 1) {
				selected_data = selected
					.OrderBy ((dev) =>
					{
						Version v;
						if (Version.TryParse (dev.ProductVersion, out v))
							return v;
						return new Version ();
					})
					.First ();
				MainLog.WriteLine ("Found {0} devices for device class(es) '{1}': '{2}'. Selected: '{3}' (because it has the lowest version).", selected.Count (), string.Join ("', '", deviceClasses), string.Join ("', '", selected.Select ((v) => v.Name).ToArray ()), selected_data.Name);
			} else {
				selected_data = selected.First ();
			}

			deviceName = selected_data.Name;

			if (RunMode == RunMode.WatchOS)
				companionDeviceName = devs.FindCompanionDevice (MainLog, selected_data).Name;
		}

		public async Task<ProcessExecutionResult> InstallAsync (CancellationToken cancellation_token)
		{
			if (isSimulator) {
				// We reset the simulator when running, so a separate install step does not make much sense.
				throw new InvalidOperationException ("Installing to a simulator is not supported.");
			}

			FindDevice ();

			var args = new List<string> ();
			if (!string.IsNullOrEmpty (harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (harness.XcodeRoot);
			}
			for (int i = -1; i < harness.Verbosity; i++)
				args.Add ("-v");
			
			args.Add ("--installdev");
			args.Add (AppInformation.AppPath);
			AddDeviceName (args, companionDeviceName ?? deviceName);

			if (RunMode == RunMode.WatchOS) {
				args.Add ("--device");
				args.Add ("ios,watchos");
			}

			var totalSize = Directory.GetFiles (AppInformation.AppPath, "*", SearchOption.AllDirectories).Select ((v) => new FileInfo (v).Length).Sum ();
			MainLog.WriteLine ($"Installing '{AppInformation.AppPath}' to '{companionDeviceName ?? deviceName}'. Size: {totalSize} bytes = {totalSize / 1024.0 / 1024.0:N2} MB");

			return await processManager.ExecuteCommandAsync (harness.MlaunchPath, args, MainLog, TimeSpan.FromHours (1), cancellation_token: cancellation_token);
		}

		public async Task<ProcessExecutionResult> UninstallAsync ()
		{
			if (isSimulator)
				throw new InvalidOperationException ("Uninstalling from a simulator is not supported.");

			FindDevice ();

			var args = new List<string> ();
			if (!string.IsNullOrEmpty (harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (harness.XcodeRoot);
			}
			for (int i = -1; i < harness.Verbosity; i++)
				args.Add ("-v");

			args.Add ("--uninstalldevbundleid");
			args.Add (AppInformation.BundleIdentifier);
			AddDeviceName (args, companionDeviceName ?? deviceName);

			return await processManager.ExecuteCommandAsync (harness.MlaunchPath, args, MainLog, TimeSpan.FromMinutes (1));
		}

		public TimeSpan GetNewTimeout () => TimeSpan.FromMinutes (harness.Timeout * timeoutMultiplier);

		public void LogException (int minLevel, string message, params object [] args) => harness.Log (minLevel, message, args);

		public async Task<int> RunAsync ()
		{
			if (!isSimulator)
				FindDevice ();

			var args = new List<string> ();

			if (!string.IsNullOrEmpty (harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (harness.XcodeRoot);
			}

			for (int i = -1; i < harness.Verbosity; i++)
				args.Add ("-v");
			args.Add ("-argument=-connection-mode");
			args.Add ("-argument=none"); // This will prevent the app from trying to connect to any IDEs
			args.Add ("-argument=-app-arg:-autostart");
			args.Add ("-setenv=NUNIT_AUTOSTART=true");
			args.Add ("-argument=-app-arg:-autoexit");
			args.Add ("-setenv=NUNIT_AUTOEXIT=true");
			args.Add ("-argument=-app-arg:-enablenetwork");
			args.Add ("-setenv=NUNIT_ENABLE_NETWORK=true");
			// detect if we are using a jenkins bot.
			var useXmlOutput = harness.InCI;
			if (useXmlOutput) {
				args.Add ("-setenv=NUNIT_ENABLE_XML_OUTPUT=true");
				args.Add ("-setenv=NUNIT_ENABLE_XML_MODE=wrapped");
				args.Add ("-setenv=NUNIT_XML_VERSION=nunitv3");
			}

			if (harness.InCI) {
				// We use the 'BUILD_REVISION' variable to detect whether we're running CI or not.
				args.Add ($"-setenv=BUILD_REVISION=${Environment.GetEnvironmentVariable ("BUILD_REVISION")}");
			}

			if (!harness.GetIncludeSystemPermissionTests (TestPlatform.iOS, !isSimulator))
				args.Add ("-setenv=DISABLE_SYSTEM_PERMISSION_TESTS=1");

			if (isSimulator) {
				args.Add ("-argument=-app-arg:-hostname:127.0.0.1");
				args.Add ("-setenv=NUNIT_HOSTNAME=127.0.0.1");
			} else {
				var ips = new StringBuilder ();
				var ipAddresses = System.Net.Dns.GetHostEntry (System.Net.Dns.GetHostName ()).AddressList;
				for (int i = 0; i < ipAddresses.Length; i++) {
					if (i > 0)
						ips.Append (',');
					ips.Append (ipAddresses [i].ToString ());
				}

				args.Add ($"-argument=-app-arg:-hostname:{ips}");
				args.Add ($"-setenv=NUNIT_HOSTNAME={ips}");
			}

			var listener_log = Logs.Create ($"test-{RunMode.ToString().ToLower()}-{Helpers.Timestamp}.log", LogType.TestLog.ToString (), timestamp: !useXmlOutput);
			var (transport, listener, listenerTmpFile) = listenerFactory.Create (RunMode, MainLog, listener_log, isSimulator, true, useXmlOutput);
			
			args.Add ($"-argument=-app-arg:-transport:{transport}");
			args.Add ($"-setenv=NUNIT_TRANSPORT={transport.ToString ().ToUpper ()}");

			if (transport == ListenerTransport.File)
				args.Add ($"-setenv=NUNIT_LOG_FILE={listenerTmpFile}");

			listener.Initialize ();

			args.Add ($"-argument=-app-arg:-hostport:{listener.Port}");
			args.Add ($"-setenv=NUNIT_HOSTPORT={listener.Port}");

			listener.StartAsync ();

			// object that will take care of capturing and parsing the results
			ILog run_log = MainLog;
			var crashLogs = new Logs (Logs.Directory);
			ICrashSnapshotReporter crashReporter = snapshotReporterFactory.Create (MainLog, crashLogs, isDevice: !isSimulator, deviceName);
			var testResult = testReporterFactory.Create (this, deviceName, listener, run_log, crashReporter);

			listener.ConnectedTask
				.TimeoutAfter (TimeSpan.FromMinutes (harness.LaunchTimeout))
				.ContinueWith (testResult.LaunchCallback)
				.DoNotAwait ();

			foreach (var kvp in harness.EnvironmentVariables)
				args.Add ($"-setenv={kvp.Key}={kvp.Value}");

			if (IsExtension) {
				switch (AppInformation.Extension) {
				case Extension.TodayExtension:
					args.Add (isSimulator ? "--launchsimbundleid" : "--launchdevbundleid");
					args.Add ("todayviewforextensions:" + AppInformation.BundleIdentifier);
					args.Add ("--observe-extension");
					args.Add (AppInformation.LaunchAppPath);
					break;
				case Extension.WatchKit2:
				default:
					throw new NotImplementedException ();
				}
			} else {
				args.Add (isSimulator ? "--launchsim" : "--launchdev");
				args.Add (AppInformation.LaunchAppPath);
			}
			if (!isSimulator)
				args.Add ("--disable-memory-limits");

			if (isSimulator) {
				if (!await FindSimulatorAsync ())
					return 1;

				if (RunMode != RunMode.WatchOS) {
					var stderr_tty = harness.GetStandardErrorTty ();
					if (!string.IsNullOrEmpty (stderr_tty)) {
						args.Add ($"--stdout={stderr_tty}");
						args.Add ($"--stderr={stderr_tty}");
					} else {
						var stdout_log = Logs.CreateFile ($"stdout-{Helpers.Timestamp}.log", "Standard output");
						var stderr_log = Logs.CreateFile ($"stderr-{Helpers.Timestamp}.log", "Standard error");
						args.Add ($"--stdout={stdout_log}");
						args.Add ($"--stderr={stderr_log}");
					}
				}

				var systemLogs = new List<ICaptureLog> ();
				foreach (var sim in simulators) {
					// Upload the system log
					MainLog.WriteLine ("System log for the '{1}' simulator is: {0}", sim.SystemLog, sim.Name);
					bool isCompanion = sim != simulator;

					var logDescription = isCompanion ? LogType.CompanionSystemLog.ToString () : LogType.SystemLog.ToString ();
					var log = captureLogFactory.Create (Logs,
						Path.Combine (Logs.Directory, sim.Name + ".log"),
						sim.SystemLog,
						harness.Action != HarnessAction.Jenkins,
						logDescription);

					log.StartCapture ();
					Logs.Add (log);
					systemLogs.Add (log);
					WrenchLog.WriteLine ("AddFile: {0}", log.FullPath);
				}

				MainLog.WriteLine ("*** Executing {0}/{1} in the simulator ***", AppInformation.AppName, RunMode);

				if (EnsureCleanSimulatorState) {
					foreach (var sim in simulators)
						await sim.PrepareSimulatorAsync (MainLog, AppInformation.BundleIdentifier);
				}

				args.Add ($"--device=:v2:udid={simulator.UDID}");

				await crashReporter.StartCaptureAsync ();

				MainLog.WriteLine ("Starting test run");

				await testResult.CollectSimulatorResult (
					processManager.ExecuteCommandAsync (harness.MlaunchPath, args, run_log, testResult.Timeout, cancellation_token: testResult.CancellationToken));

				// cleanup after us
				if (EnsureCleanSimulatorState)
					await simulator.KillEverythingAsync (MainLog);

				foreach (var log in systemLogs)
					log.StopCapture ();

			} else {
				MainLog.WriteLine ("*** Executing {0}/{1} on device '{2}' ***", AppInformation.AppName, RunMode, deviceName);

				if (RunMode == RunMode.WatchOS) {
					args.Add ("--attach-native-debugger"); // this prevents the watch from backgrounding the app.
				} else {
					args.Add ("--wait-for-exit");
				}

				AddDeviceName (args);

				var deviceSystemLog = Logs.Create ($"device-{deviceName}-{Helpers.Timestamp}.log", "Device log");
				var deviceLogCapturer = deviceLogCapturerFactory.Create (harness.HarnessLog, deviceSystemLog, deviceName);
				deviceLogCapturer.StartCapture ();

				try {
					await crashReporter.StartCaptureAsync ();

					MainLog.WriteLine ("Starting test run");

					// We need to check for MT1111 (which means that mlaunch won't wait for the app to exit).
					var runLog = Log.CreateAggregatedLog (testResult.CallbackLog, MainLog);
					testResult.TimeoutWatch.Start ();
					await testResult.CollectDeviceResult (processManager.ExecuteCommandAsync (harness.MlaunchPath, args, runLog, testResult.Timeout, cancellation_token: testResult.CancellationToken));
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

			listener.Cancel ();
			listener.Dispose ();

			// check the final status, copy all the required data
			(Result, FailureMessage) = await testResult.ParseResult ();
			return testResult.Success.Value ? 0 : 1;
		}

		public void AddDeviceName (IList<string> args)
		{
			AddDeviceName (args, deviceName);
		}

		public static void AddDeviceName (IList<string> args, string device_name)
		{
			if (!string.IsNullOrEmpty (device_name)) {
				args.Add ("--devname");
				args.Add (device_name);
			}
		}
	}
}
