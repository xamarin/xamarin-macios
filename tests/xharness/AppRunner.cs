using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xharness.Execution;
using Xharness.Jenkins.TestTasks;
using Xharness.Listeners;
using Xharness.Logging;
using Xharness.Utilities;

namespace Xharness {
	public enum AppRunnerTarget
	{
		None,
		Simulator_iOS,
		Simulator_iOS32,
		Simulator_iOS64,
		Simulator_tvOS,
		Simulator_watchOS,
		Device_iOS,
		Device_tvOS,
		Device_watchOS,
	}

	public enum Extension
	{
		WatchKit2,
		TodayExtension,
	}

	class AppRunner
	{
		string appPath;
		string launchAppPath;
		string platform;
		Extension? extension;
		bool isSimulator;
		string configuration;
		string mode;

		public Harness Harness;
		public string ProjectFile;
		public string AppPath;
		public string Variation;
		public BuildToolTask BuildTask;
		public ISimpleListenerFactory ListenerFactory = new SimpleListenerFactory ();

		public TestExecutingResult Result { get; private set; }

		public string FailureMessage { get; private set; }

		public string DeviceName { get; set; }

		public string CompanionDeviceName { get; set; }

		public bool IsExtension => extension.HasValue;

		public string AppName { get; private set; }

		public double TimeoutMultiplier { get; set; } = 1;

		ILogs logs;
		public ILogs Logs => logs ?? (logs = new Logs (LogDirectory));

		public ILog MainLog { get; set; }

		public SimDevice [] Simulators { get; set; }
		SimDevice simulator => Simulators [0];

		public string BundleIdentifier { get; private set; }

		public IProcessManager ProcessManager { get; set; } = new ProcessManager ();

		AppRunnerTarget target;
		public AppRunnerTarget Target {
			get => target == AppRunnerTarget.None ? Harness.Target : target;
			set => target = value;
		}

		string log_directory;
		public string LogDirectory {
			get => log_directory ?? Harness.LogDirectory;
			set => log_directory = value;
		}

		bool ensure_clean_simulator_state = true;
		public bool EnsureCleanSimulatorState {
			get => ensure_clean_simulator_state && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("SKIP_SIMULATOR_SETUP"));
			set => ensure_clean_simulator_state = value;
		}

		public string Configuration {
			get => configuration ?? Harness.Configuration;
			set => configuration = value;
		}

		async Task<bool> FindSimulatorAsync ()
		{
			if (Simulators != null)
				return true;
			
			var sims = new Simulators () {
				Harness = Harness,
			};
			await sims.LoadAsync (Logs.Create ($"simulator-list-{Harness.Timestamp}.log", "Simulator list"));
			Simulators = await sims.FindAsync (Target, MainLog);

			return Simulators != null;
		}

		void FindDevice ()
		{
			if (DeviceName != null)
				return;
			
			DeviceName = Environment.GetEnvironmentVariable ("DEVICE_NAME");
			if (!string.IsNullOrEmpty (DeviceName))
				return;

			var devs = new Devices () {
				Harness = Harness,
			};
			Task.Run (async () =>
			{
				await devs.LoadAsync (MainLog);
			}).Wait ();

			string [] deviceClasses;
			switch (mode) {
			case "ios":
				deviceClasses = new string [] { "iPhone", "iPad", "iPod" };
				break;
			case "watchos":
				deviceClasses = new string [] { "Watch" };
				break;
			case "tvos":
				deviceClasses = new string [] { "AppleTV" }; // Untested
				break;
			default:
				throw new Exception ($"unknown mode: {mode}");
			}

			var selected = devs.ConnectedDevices.Where ((v) => deviceClasses.Contains (v.DeviceClass) && v.IsUsableForDebugging != false);
			Device selected_data;
			if (selected.Count () == 0) {
				throw new Exception ($"Could not find any applicable devices with device class(es): {string.Join (", ", deviceClasses)}");
			} else if (selected.Count () > 1) {
				selected_data = selected
					.OrderBy ((dev) =>
					{
						if (Version.TryParse (dev.ProductVersion, out Version v))
							return v;
						return new Version ();
					})
					.First ();
				MainLog.WriteLine ("Found {0} devices for device class(es) '{1}': '{2}'. Selected: '{3}' (because it has the lowest version).", selected.Count (), string.Join ("', '", deviceClasses), string.Join ("', '", selected.Select ((v) => v.Name).ToArray ()), selected_data.Name);
			} else {
				selected_data = selected.First ();
			}
			DeviceName = selected_data.Name;

			if (mode == "watchos")
				CompanionDeviceName = devs.FindCompanionDevice (MainLog, selected_data).Name;
		}

		bool initialized;
		public void Initialize ()
		{
			if (initialized)
				return;
			initialized = true;

			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (ProjectFile);
			AppName = csproj.GetAssemblyName ();
			var info_plist_path = csproj.GetInfoPListInclude ();
			var info_plist = new XmlDocument ();
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Path.GetDirectoryName (ProjectFile), info_plist_path.Replace ('\\', '/')));
			BundleIdentifier = info_plist.GetCFBundleIdentifier ();

			var extensionPointIdentifier = info_plist.GetNSExtensionPointIdentifier ();
			if (!string.IsNullOrEmpty (extensionPointIdentifier))
				extension = extensionPointIdentifier.ParseFromNSExtensionPointIdentifier ();

			switch (Target) {
			case AppRunnerTarget.Simulator_iOS32:
				mode = "sim32";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case AppRunnerTarget.Simulator_iOS64:
				mode = "sim64";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case AppRunnerTarget.Simulator_iOS:
				mode = "classic";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case AppRunnerTarget.Device_iOS:
				mode = "ios";
				platform = "iPhone";
				isSimulator = false;
				break;
			case AppRunnerTarget.Simulator_tvOS:
				mode = "tvos";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case AppRunnerTarget.Device_tvOS:
				mode = "tvos";
				platform = "iPhone";
				isSimulator = false;
				break;
			case AppRunnerTarget.Simulator_watchOS:
				mode = "watchos";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case AppRunnerTarget.Device_watchOS:
				mode = "watchos";
				platform = "iPhone";
				isSimulator = false;
				break;
			default:
				throw new Exception (string.Format ("Unknown target: {0}", Harness.Target));
			}

			appPath = Path.Combine (Path.GetDirectoryName (ProjectFile), csproj.GetOutputPath (platform, Configuration).Replace ('\\', '/'), AppName + (IsExtension ? ".appex" : ".app"));
			if (!Directory.Exists (appPath))
				throw new Exception (string.Format ("The app directory {0} does not exist. This is probably a bug in the test harness.", appPath));

			if (mode == "watchos") {
				launchAppPath = Directory.GetDirectories (Path.Combine (appPath, "Watch"), "*.app") [0];
			} else {
				launchAppPath = appPath;
			}
		}

		public async Task<ProcessExecutionResult> InstallAsync (CancellationToken cancellation_token)
		{
			Initialize ();

			if (isSimulator) {
				// We reset the simulator when running, so a separate install step does not make much sense.
				throw new Exception ("Installing to a simulator is not supported.");
			}

			FindDevice ();

			var args = new List<string> ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (Harness.XcodeRoot);
			}
			for (int i = -1; i < Harness.Verbosity; i++)
				args.Add ("-v");
			
			args.Add ("--installdev");
			args.Add (appPath);
			AddDeviceName (args, CompanionDeviceName ?? DeviceName);

			if (mode == "watchos") {
				args.Add ("--device");
				args.Add ("ios,watchos");
			}

			var totalSize = Directory.GetFiles (appPath, "*", SearchOption.AllDirectories).Select ((v) => new FileInfo (v).Length).Sum ();
			MainLog.WriteLine ($"Installing '{appPath}' to '{CompanionDeviceName ?? DeviceName}'. Size: {totalSize} bytes = {totalSize / 1024.0 / 1024.0:N2} MB");

			return await ProcessManager.ExecuteCommandAsync (Harness.MlaunchPath, args, MainLog, TimeSpan.FromHours (1), cancellation_token: cancellation_token);
		}

		public async Task<ProcessExecutionResult> UninstallAsync ()
		{
			Initialize ();

			if (isSimulator)
				throw new Exception ("Uninstalling from a simulator is not supported.");

			FindDevice ();

			var args = new List<string> ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (Harness.XcodeRoot);
			}
			for (int i = -1; i < Harness.Verbosity; i++)
				args.Add ("-v");

			args.Add ("--uninstalldevbundleid");
			args.Add (BundleIdentifier);
			AddDeviceName (args, CompanionDeviceName ?? DeviceName);

			return await ProcessManager.ExecuteCommandAsync (Harness.MlaunchPath, args, MainLog, TimeSpan.FromMinutes (1));
		}

		(string resultLine, bool failed, bool crashed) ParseResult (string test_log_path, bool timed_out, out bool crashed)
		{
			crashed = false;
			if (!File.Exists (test_log_path)) {
				crashed = true;
				return (null, false, true); // if we do not have a log file, the test crashes
			}
			// parsing the result is different if we are in jenkins or not.
			// When in Jenkins, Touch.Unit produces an xml file instead of a console log (so that we can get better test reporting).
			// However, for our own reporting, we still want the console-based log. This log is embedded inside the xml produced
			// by Touch.Unit, so we need to extract it and write it to disk. We also need to re-save the xml output, since Touch.Unit
			// wraps the NUnit xml output with additional information, which we need to unwrap so that Jenkins understands it.
			// 
			// On the other hand, the nunit and xunit do not have that data and have to be parsed.
			// 
			// This if statement has a small trick, we found out that internet sharing in some of the bots (VSTS) does not work, in
			// that case, we cannot do a TCP connection to xharness to get the log, this is a problem since if we did not get the xml
			// from the TCP connection, we are going to fail when trying to read it and not parse it. Therefore, we are not only
			// going to check if we are in CI, but also if the listener_log is valid.
			var path = Path.ChangeExtension (test_log_path, "xml");
			XmlResultParser.CleanXml (test_log_path, path);

			if (Harness.InCI && XmlResultParser.IsValidXml (path, out var xmlType)) {
				(string resultLine, bool failed, bool crashed) parseResult = (null, false, false);
				crashed = false;
				try {
					var newFilename = XmlResultParser.GetXmlFilePath (path, xmlType);

					// at this point, we have the test results, but we want to be able to have attachments in vsts, so if the format is
					// the right one (NUnitV3) add the nodes. ATM only TouchUnit uses V3.
					var testRunName = $"{AppName} {Variation}";
					if (xmlType == XmlResultJargon.NUnitV3) {
						var logFiles = new List<string> ();
						// add our logs AND the logs of the previous task, which is the build task
						logFiles.AddRange (Directory.GetFiles (Logs.Directory));
						if (BuildTask != null) // when using the run command, we do not have a build task, ergo, there are no logs to add.
							logFiles.AddRange (Directory.GetFiles (BuildTask.LogDirectory));
						// add the attachments and write in the new filename
						// add a final prefix to the file name to make sure that the VSTS test uploaded just pick
						// the final version, else we will upload tests more than once
						newFilename = XmlResultParser.GetVSTSFilename (newFilename);
						XmlResultParser.UpdateMissingData (path, newFilename, testRunName, logFiles);
					} else {
						// rename the path to the correct value
						File.Move (path, newFilename);
					}
					path = newFilename;

					// write the human readable results in a tmp file, which we later use to step on the logs
					var tmpFile = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
					(parseResult.resultLine, parseResult.failed) = XmlResultParser.GenerateHumanReadableResults (path, tmpFile, xmlType);
					File.Copy (tmpFile, test_log_path, true);
					File.Delete (tmpFile);

					// we do not longer need the tmp file
					Logs.AddFile (path, LogType.XmlLog.ToString ());
					return parseResult;

				} catch (Exception e) {
					MainLog.WriteLine ("Could not parse xml result file: {0}", e);
					// print file for better debugging
					MainLog.WriteLine ("File data is:");
					MainLog.WriteLine (new string ('#', 10));
					using (var stream = new StreamReader (path)) {
						string line;
						while ((line = stream.ReadLine ()) != null) {
							MainLog.WriteLine (line);
						}
					}
					MainLog.WriteLine (new string ('#', 10));
					MainLog.WriteLine ("End of xml results.");
					if (timed_out) {
						Harness.LogWrench ($"@MonkeyWrench: AddSummary: <b><i>{mode} timed out</i></b><br/>");
						return parseResult;
					} else {
						Harness.LogWrench ($"@MonkeyWrench: AddSummary: <b><i>{mode} crashed</i></b><br/>");
						MainLog.WriteLine ("Test run crashed");
						crashed = true;
						parseResult.crashed = true;
						return parseResult;
					}
				}

			}               // delete not needed copy
			File.Delete (path);
			// not the most efficient way but this just happens when we run
			// the tests locally and we usually do not run all tests, we are
			// more interested to be efficent on the bots
			string resultLine = null;
			using (var reader = new StreamReader (test_log_path)) {
				string line = null;
				bool failed = false;
				while ((line = reader.ReadLine ()) != null) {
					if (line.Contains ("Tests run:")) {
						Console.WriteLine (line);
						resultLine = line;
						break;
					} else if (line.Contains ("[FAIL]")) {
						Console.WriteLine (line);
						failed = true;
					}
				}
				return (resultLine, failed, false);
			}
		}

		public bool TestsSucceeded (string test_log_path, bool timed_out, out bool crashed)
		{
			var (resultLine, failed, crashed_out) = ParseResult (test_log_path, timed_out, out crashed);
			// read the parsed logs in a human readable way
			if (resultLine != null) {
				var tests_run = resultLine.Replace ("Tests run: ", "");
				if (failed) {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b>{0} failed: {1}</b><br/>", mode, tests_run);
					MainLog.WriteLine ("Test run failed");
					return false;
				} else {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: {0} succeeded: {1}<br/>", mode, tests_run);
					MainLog.WriteLine ("Test run succeeded");
					return true;
				}
			} else if (timed_out) {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} timed out</i></b><br/>", mode);
				return false;
			} else {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed</i></b><br/>", mode);
				MainLog.WriteLine ("Test run crashed");
				crashed = true;
				return false;
			}
		}

		[DllImport ("/usr/lib/libc.dylib")]
		extern static IntPtr ttyname (int filedes);

		public async Task<int> RunAsync ()
		{
			CrashReportSnapshot crash_reports;
			ILog device_system_log = null;
			ILog listener_log = null;
			ILog run_log = MainLog;

			Initialize ();

			if (!isSimulator)
				FindDevice ();

			crash_reports = new CrashReportSnapshot () {
				Device = !isSimulator,
				DeviceName = DeviceName,
				Harness = Harness,
				Log = MainLog,
				Logs = Logs,
				LogDirectory = LogDirectory,
			};

			var args = new List<string> ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot)) {
				args.Add ("--sdkroot");
				args.Add (Harness.XcodeRoot);
			}
			for (int i = -1; i < Harness.Verbosity; i++)
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
			var useXmlOutput = Harness.InCI;
			if (useXmlOutput) {
				args.Add ("-setenv=NUNIT_ENABLE_XML_OUTPUT=true");
				args.Add ("-setenv=NUNIT_ENABLE_XML_MODE=wrapped");
				args.Add ("-setenv=NUNIT_XML_VERSION=nunitv3");
			}

			if (Harness.InCI) {
				// We use the 'BUILD_REVISION' variable to detect whether we're running CI or not.
				args.Add ($"-setenv=BUILD_REVISION=${Environment.GetEnvironmentVariable ("BUILD_REVISION")}");
			}

			if (!Harness.GetIncludeSystemPermissionTests (TestPlatform.iOS, !isSimulator))
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

			listener_log = Logs.Create ($"test-{mode}-{Harness.Timestamp}.log", LogType.TestLog.ToString (), timestamp: !useXmlOutput);
			var transport = ListenerFactory.Create (mode, listener_log, isSimulator, out var listener, out var fn);
			
			args.Add ($"-argument=-app-arg:-transport:{transport}");
			args.Add ($"-setenv=NUNIT_TRANSPORT={transport.ToString ().ToUpper ()}");

			if (transport == ListenerTransport.File)
				args.Add ($"-setenv=NUNIT_LOG_FILE={fn}");

			
			listener.TestLog = listener_log;
			listener.Log = MainLog;
			listener.AutoExit = true;
			listener.Address = System.Net.IPAddress.Any;
			listener.XmlOutput = useXmlOutput;
			listener.Initialize ();

			args.Add ($"-argument=-app-arg:-hostport:{listener.Port}");
			args.Add ($"-setenv=NUNIT_HOSTPORT={listener.Port}");

			listener.StartAsync ();

			var cancellation_source = new CancellationTokenSource ();
			var timed_out = false;

			listener.ConnectedTask
				.TimeoutAfter (Harness.LaunchTimeout)
				.ContinueWith ((v) => {
					if (v.IsFaulted) {
						MainLog.WriteLine ("Test launch failed: {0}", v.Exception);
					} else if (v.IsCanceled) {
						MainLog.WriteLine ("Test launch was cancelled.");
					} else if (v.Result) {
						MainLog.WriteLine ("Test run started");
					} else {
						cancellation_source.Cancel ();
						MainLog.WriteLine ("Test launch timed out after {0} minute(s).", Harness.LaunchTimeout);
						timed_out = true;
					}
				}).DoNotAwait ();

			foreach (var kvp in Harness.EnvironmentVariables)
				args.Add ($"-setenv={kvp.Key}={kvp.Value}");

			bool? success = null;
			bool launch_failure = false;

			if (IsExtension) {
				switch (extension) {
				case Extension.TodayExtension:
					args.Add (isSimulator ? "--launchsimbundleid" : "--launchdevbundleid");
					args.Add ("todayviewforextensions:" + BundleIdentifier);
					args.Add ("--observe-extension");
					args.Add (launchAppPath);
					break;
				case Extension.WatchKit2:
				default:
					throw new NotImplementedException ();
				}
			} else {
				args.Add (isSimulator ? "--launchsim" : "--launchdev");
				args.Add (launchAppPath);
			}
			if (!isSimulator)
				args.Add ("--disable-memory-limits");

			var timeout = TimeSpan.FromMinutes (Harness.Timeout * TimeoutMultiplier);
			if (isSimulator) {
				if (!await FindSimulatorAsync ())
					return 1;

				if (mode != "watchos") {
					var stderr_tty = Marshal.PtrToStringAuto (ttyname (2));
					if (!string.IsNullOrEmpty (stderr_tty)) {
						args.Add ($"--stdout={stderr_tty}");
						args.Add ($"--stderr={stderr_tty}");
					} else {
						var stdout_log = Logs.CreateFile ($"stdout-{Harness.Timestamp}.log", "Standard output");
						var stderr_log = Logs.CreateFile ($"stderr-{Harness.Timestamp}.log", "Standard error");
						args.Add ($"--stdout={stdout_log}");
						args.Add ($"--stderr={stderr_log}");
					}
				}

				var systemLogs = new List<CaptureLog> ();
				foreach (var sim in Simulators) {
					// Upload the system log
					MainLog.WriteLine ("System log for the '{1}' simulator is: {0}", sim.SystemLog, sim.Name);
					bool isCompanion = sim != simulator;

					var log = new CaptureLog (Logs, Path.Combine (LogDirectory, sim.Name + ".log"), sim.SystemLog, entire_file: Harness.Action != HarnessAction.Jenkins)
					{
						Description = isCompanion ? LogType.CompanionSystemLog.ToString () : LogType.SystemLog.ToString (),
					};
					log.StartCapture ();
					Logs.Add (log);
					systemLogs.Add (log);
					Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", log.Path);
				}

				MainLog.WriteLine ("*** Executing {0}/{1} in the simulator ***", AppName, mode);

				if (EnsureCleanSimulatorState) {
					foreach (var sim in Simulators)
						await sim.PrepareSimulatorAsync (MainLog, BundleIdentifier);
				}

				args.Add ($"--device=:v2:udid={simulator.UDID}");

				await crash_reports.StartCaptureAsync ();

				MainLog.WriteLine ("Starting test run");

				var result = await ProcessManager.ExecuteCommandAsync (Harness.MlaunchPath, args, run_log, timeout, cancellation_token: cancellation_source.Token);
				if (result.TimedOut) {
					timed_out = true;
					success = false;
					MainLog.WriteLine ("Test run timed out after {0} minute(s).", timeout);
				} else if (result.Succeeded) {
					MainLog.WriteLine ("Test run completed");
					success = true;
				} else {
					MainLog.WriteLine ("Test run failed");
					success = false;
				}

				if (!success.Value) {
					// find pid
					var pid = -1;
					using (var reader = run_log.GetReader ()) {
						while (!reader.EndOfStream) {
							var line = reader.ReadLine ();
							if (line.StartsWith ("Application launched. PID = ", StringComparison.Ordinal)) {
								var pidstr = line.Substring ("Application launched. PID = ".Length);
								if (!int.TryParse (pidstr, out pid))
									MainLog.WriteLine ("Could not parse pid: {0}", pidstr);
							} else if (line.Contains ("Xamarin.Hosting: Launched ") && line.Contains (" with pid ")) {
								var pidstr = line.Substring (line.LastIndexOf (' '));
								if (!int.TryParse (pidstr, out pid))
									MainLog.WriteLine ("Could not parse pid: {0}", pidstr);
							} else if (line.Contains ("error MT1008")) {
								launch_failure = true;
							}
						}
					}
					if (pid > 0) {
						var launchTimedout = cancellation_source.IsCancellationRequested;
						var timeoutType = launchTimedout ? "Launch" : "Completion";
						var timeoutValue = launchTimedout ? Harness.LaunchTimeout.TotalSeconds : timeout.TotalSeconds;
						MainLog.WriteLine ($"{timeoutType} timed out after {timeoutValue} seconds");
						await ProcessManager.KillTreeAsync (pid, MainLog, true);
					} else {
						MainLog.WriteLine ("Could not find pid in mtouch output.");
					}
				}


				// cleanup after us
				if (EnsureCleanSimulatorState)
					await SimDevice.KillEverythingAsync (MainLog);

				foreach (var log in systemLogs)
					log.StopCapture ();
				
			} else {
				MainLog.WriteLine ("*** Executing {0}/{1} on device '{2}' ***", AppName, mode, DeviceName);

				if (mode == "watchos") {
					args.Add ("--attach-native-debugger"); // this prevents the watch from backgrounding the app.
				} else {
					args.Add ("--wait-for-exit");
				}
				
				AddDeviceName (args);

				device_system_log = Logs.Create ($"device-{DeviceName}-{Harness.Timestamp}.log", "Device log");
				var logdev = new DeviceLogCapturer () {
					Harness =  Harness,
					Log = device_system_log,
					DeviceName = DeviceName,
				};
				logdev.StartCapture ();

				try {
					await crash_reports.StartCaptureAsync ();

					MainLog.WriteLine ("Starting test run");

					bool waitedForExit = true;
					// We need to check for MT1111 (which means that mlaunch won't wait for the app to exit).
					var callbackLog = new CallbackLog ((line) => {
						// MT1111: Application launched successfully, but it's not possible to wait for the app to exit as requested because it's not possible to detect app termination when launching using gdbserver
						waitedForExit &= line?.Contains ("MT1111: ") != true;
						if (line?.Contains ("error MT1007") == true)
							launch_failure = true;
					});
					var runLog = Log.CreateAggregatedLog (callbackLog, MainLog);
					var timeoutWatch = Stopwatch.StartNew ();
					var result = await ProcessManager.ExecuteCommandAsync (Harness.MlaunchPath, args, runLog, timeout, cancellation_token: cancellation_source.Token);

					if (!waitedForExit && !result.TimedOut) {
						// mlaunch couldn't wait for exit for some reason. Let's assume the app exits when the test listener completes.
						MainLog.WriteLine ("Waiting for listener to complete, since mlaunch won't tell.");
						if (!await listener.CompletionTask.TimeoutAfter (timeout - timeoutWatch.Elapsed)) {
							result.TimedOut = true;
						}
					}

					if (result.TimedOut) {
						timed_out = true;
						success = false;
						MainLog.WriteLine ("Test run timed out after {0} minute(s).", timeout.TotalMinutes);
					} else if (result.Succeeded) {
						MainLog.WriteLine ("Test run completed");
						success = true;
					} else {
						MainLog.WriteLine ("Test run failed");
						success = false;
					}
				} finally {
					logdev.StopCapture ();
					device_system_log.Dispose ();
				}

				// Upload the system log
				if (File.Exists (device_system_log.FullPath)) {
					MainLog.WriteLine ("A capture of the device log is: {0}", device_system_log.FullPath);
					Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", device_system_log.FullPath);
				}
			}

			listener.Cancel ();
			listener.Dispose ();

			// check the final status
			var crashed = false;
			if (File.Exists (listener_log.FullPath)) {
				Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", listener_log.FullPath);
				success = TestsSucceeded (listener_log.FullPath, timed_out, out crashed);
			} else if (timed_out) {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} never launched</i></b><br/>", mode);
				MainLog.WriteLine ("Test run never launched");
				success = false;
			} else if (launch_failure) {
 				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} failed to launch</i></b><br/>", mode);
 				MainLog.WriteLine ("Test run failed to launch");
 				success = false;
			} else {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed at startup (no log)</i></b><br/>", mode);
				MainLog.WriteLine ("Test run crashed before it started (no log file produced)");
				crashed = true;
				success = false;
			}
				
			if (!success.HasValue)
				success = false;

			await crash_reports.EndCaptureAsync (TimeSpan.FromSeconds (success.Value ? 0 : 5));

			if (timed_out) {
				Result = TestExecutingResult.TimedOut;
			} else if (crashed) {
				Result = TestExecutingResult.Crashed;
			} else if (success.Value) {
				Result = TestExecutingResult.Succeeded;
			} else {
				Result = TestExecutingResult.Failed;
			}

			// Check crash reports to see if any of them explains why the test run crashed.
			if (!success.Value) {
				int pid = 0;
				string crash_reason = null;
				foreach (var crash in crash_reports.Logs) {
					try {
						if (pid == 0) {
							// Find the pid
							using (var log_reader = MainLog.GetReader ()) {
								string line;
								while ((line = log_reader.ReadLine ()) != null) {
									const string str = "was launched with pid '";
									var idx = line.IndexOf (str, StringComparison.Ordinal);
									if (idx > 0) {
										idx += str.Length;
										var next_idx = line.IndexOf ('\'', idx);
										if (next_idx > idx)
											int.TryParse (line.Substring (idx, next_idx - idx), out pid);
									}
									if (pid != 0)
										break;
								}
							}
						}

						using (var crash_reader = crash.GetReader ()) {
							var text = crash_reader.ReadToEnd ();

							var reader = System.Runtime.Serialization.Json.JsonReaderWriterFactory.CreateJsonReader (Encoding.UTF8.GetBytes (text), new XmlDictionaryReaderQuotas ());
							var doc = new XmlDocument ();
							doc.Load (reader);
							foreach (XmlNode node in doc.SelectNodes ($"/root/processes/item[pid = '" + pid + "']")) {
								Console.WriteLine (node?.InnerXml);
								Console.WriteLine (node?.SelectSingleNode ("reason")?.InnerText);
								crash_reason = node?.SelectSingleNode ("reason")?.InnerText;
							}
						}
						if (crash_reason != null) {
							// if in CI, do write an xml error that will be picked as a failure by VSTS
							if (Harness.InCI)
								XmlResultParser.GenerateFailure (Logs, "crash", AppName, Variation, "AppCrash", $"App crashed {crash_reason}.", crash_reports.Log.FullPath, Harness.XmlJargon);
							break;
						}
					} catch (Exception e) {
						Harness.Log (2, "Failed to process crash report '{1}': {0}", e.Message, crash.Description);
					}
				}
				if (!string.IsNullOrEmpty (crash_reason)) {
					if (crash_reason == "per-process-limit") {
						FailureMessage = "Killed due to using too much memory (per-process-limit).";
					} else {
						FailureMessage = $"Killed by the OS ({crash_reason})";
					}
					if (Harness.InCI)
						XmlResultParser.GenerateFailure (Logs, "crash", AppName, Variation, "AppCrash", $"App crashed: {FailureMessage}", crash_reports.Log.FullPath, Harness.XmlJargon);
				} else if (launch_failure) {
					// same as with a crash
					FailureMessage = $"Launch failure";
					if (Harness.InCI)
						XmlResultParser.GenerateFailure (Logs, "launch", AppName, Variation, $"AppLaunch on {DeviceName}", $"{FailureMessage} on {DeviceName}", MainLog.FullPath, XmlResultJargon.NUnitV3);
				} else if (!isSimulator && crashed && string.IsNullOrEmpty (crash_reason) && Harness.InCI) {
					// this happens more that what we would like on devices, the main reason most of the time is that we have had netwoking problems and the
					// tcp connection could not be stablished. We are going to report it as an error since we have not parsed the logs, evne when the app might have
					// not crashed. We need to check the main_log to see if we do have an tcp issue or not
					var isTcp = false;
					using (var reader = new StreamReader (MainLog.FullPath)) {
						string line;
						while ((line = reader.ReadLine ()) != null) {
							if (line.Contains ("Couldn't establish a TCP connection with any of the hostnames")) {
								isTcp = true;
								break;
							}
						}
					}
					if (isTcp)
						XmlResultParser.GenerateFailure (Logs, "tcp-connection", AppName, Variation, $"TcpConnection on {DeviceName}", $"Device {DeviceName} could not reach the host over tcp.", MainLog.FullPath, Harness.XmlJargon);
				} else if (timed_out && Harness.InCI) {
					XmlResultParser.GenerateFailure (Logs, "timeout", AppName, Variation, "AppTimeout", $"Test run timed out after {timeout.TotalMinutes} minute(s).", MainLog.FullPath, Harness.XmlJargon);
				}
			}

			return success.Value ? 0 : 1;
		}

		public void AddDeviceName (IList<string> args)
		{
			AddDeviceName (args, DeviceName);
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
