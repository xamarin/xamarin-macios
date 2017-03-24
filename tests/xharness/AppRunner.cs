using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace xharness
{
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

	public class AppRunner
	{
		public Harness Harness;
		public string ProjectFile;
		public string AppPath;

		public TestExecutingResult Result { get; private set; }
		public string FailureMessage { get; private set; }

		string appName;
		string appPath;
		string launchAppPath;
		string bundle_identifier;
		string platform;
		Extension? extension;
		bool isSimulator;

		string device_name;
		string companion_device_name;

		string configuration;
		public string Configuration {
			get { return configuration ?? Harness.Configuration; }
			set { configuration = value; }
		}

		public string DeviceName {
			get { return device_name; }
			set { device_name = value; }
		}

		public string CompanionDeviceName {
			get { return companion_device_name; }
			set { companion_device_name = value; }
		}

		public bool isExtension {
			get {
				return extension.HasValue;
			}
		}

		// For watch apps we end up with 2 simulators, the watch simulator (the main one), and the iphone simulator (the companion one).
		SimDevice[] simulators;
		SimDevice simulator { get { return simulators [0]; } }
		SimDevice companion_simulator { get { return simulators.Length == 2 ? simulators [1] : null; } }

		AppRunnerTarget target;
		public AppRunnerTarget Target {
			get { return target == AppRunnerTarget.None ? Harness.Target : target; }
			set { target = value; }
		}

		string log_directory;
		public string LogDirectory {
			get { return log_directory ?? Harness.LogDirectory; }
			set { log_directory = value; }
		}

		Log main_log;
		public Logs Logs = new Logs ();

		public Log MainLog {
			get { return main_log; }
			set { main_log = value; }
		}

		public SimDevice [] Simulators {
			get { return simulators; }
			set { simulators = value; }
		}

		public string BundleIdentifier {
			get {
				return bundle_identifier;
			}
		}

		string mode;

		async Task<bool> FindSimulatorAsync ()
		{
			if (simulators != null)
				return true;
			
			var sims = new Simulators () {
				Harness = Harness,
			};
			await sims.LoadAsync (Logs.CreateStream (LogDirectory, "simulator-list.log", "Simulator list"));
			simulators = await sims.FindAsync (Target, main_log);

			return simulators != null;
		}

		void FindDevice ()
		{
			if (device_name != null)
				return;
			
			device_name = Environment.GetEnvironmentVariable ("DEVICE_NAME");
			if (!string.IsNullOrEmpty (device_name))
				return;

			var devs = new Devices () {
				Harness = Harness,
			};
			Task.Run (async () =>
			{
				await devs.LoadAsync (main_log);
			}).Wait ();

			string [] deviceClasses;
			switch (mode) {
			case "ios":
				deviceClasses = new string [] { "iPhone", "iPad" };
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

			var selected = devs.ConnectedDevices.Where ((v) => deviceClasses.Contains (v.DeviceClass));
			Device selected_data;
			if (selected.Count () == 0) {
				throw new Exception ($"Could not find any applicable devices with device class(es): {string.Join (", ", deviceClasses)}");
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
				main_log.WriteLine ("Found {0} devices for device class(es) '{1}': '{2}'. Selected: '{3}' (because it has the lowest version).", selected.Count (), string.Join ("', '", deviceClasses), string.Join ("', '", selected.Select ((v) => v.Name).ToArray ()), selected_data.Name);
			} else {
				selected_data = selected.First ();
			}
			device_name = selected_data.Name;

			if (mode == "watchos")
				companion_device_name = devs.FindCompanionDevice (main_log, selected_data).Name;
		}

		bool initialized;
		public void Initialize ()
		{
			if (initialized)
				return;
			initialized = true;

			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (ProjectFile);
			appName = csproj.GetAssemblyName ();
			var info_plist_path = csproj.GetInfoPListInclude ();
			var info_plist = new XmlDocument ();
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Path.GetDirectoryName (ProjectFile), info_plist_path.Replace ('\\', '/')));
			bundle_identifier = info_plist.GetCFBundleIdentifier ();

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

			appPath = Path.Combine (Path.GetDirectoryName (ProjectFile), csproj.GetOutputPath (platform, Configuration).Replace ('\\', '/'), appName + (isExtension ? ".appex" : ".app"));
			if (!Directory.Exists (appPath))
				throw new Exception (string.Format ("The app directory {0} does not exist. This is probably a bug in the test harness.", appPath));

			if (mode == "watchos") {
				launchAppPath = Directory.GetDirectories (Path.Combine (appPath, "Watch"), "*.app") [0];
			} else {
				launchAppPath = appPath;
			}
		}

		public async Task<ProcessExecutionResult> InstallAsync ()
		{
			Initialize ();

			if (isSimulator) {
				// We reset the simulator when running, so a separate install step does not make much sense.
				throw new Exception ("Installing to a simulator is not supported.");
			}

			FindDevice ();

			var args = new StringBuilder ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot))
				args.Append (" --sdkroot ").Append (Harness.XcodeRoot);
			for (int i = -1; i < Harness.Verbosity; i++)
				args.Append (" -v ");
			
			args.Append (" --installdev");
			args.AppendFormat (" \"{0}\" ", appPath);
			AddDeviceName (args, companion_device_name ?? device_name);

			if (mode == "watchos")
				args.Append (" --device ios,watchos");

			return await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), main_log, TimeSpan.FromMinutes (mode == "watchos" ? 15 : 3));
		}

		public async Task<ProcessExecutionResult> UninstallAsync ()
		{
			Initialize ();

			if (isSimulator)
				throw new Exception ("Uninstalling from a simulator is not supported.");

			FindDevice ();

			var args = new StringBuilder ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot))
				args.Append (" --sdkroot ").Append (Harness.XcodeRoot);
			for (int i = -1; i < Harness.Verbosity; i++)
				args.Append (" -v ");

			args.Append (" --uninstalldevbundleid");
			args.AppendFormat (" \"{0}\" ", bundle_identifier);
			AddDeviceName (args, companion_device_name ?? device_name);

			return await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), main_log, TimeSpan.FromMinutes (1));
		}

		bool ensure_clean_simulator_state = true;
		public bool EnsureCleanSimulatorState {
			get {
				return ensure_clean_simulator_state && string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("SKIP_SIMULATOR_SETUP"));
			}
			set {
				ensure_clean_simulator_state = value;
			}
		}

		void GenerateHumanReadableLogs (string finalPath, string logHeader, XmlDocument doc)
		{
			// load the resource that contains the xslt and apply it to the doc and write the logs
			if (File.Exists (finalPath)) {
				// if the file does exist, remove it
				File.Delete (finalPath);
			}

			using (var strm = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("xharness.nunit-summary.xslt"))
			using (var xsltReader = XmlReader.Create (strm)) 
			using (var xmlReader = new XmlNodeReader (doc)) 
			using (var writer = new StreamWriter (finalPath)) {
				writer.Write (logHeader);
 	   			var xslt = new XslCompiledTransform ();
				xslt.Load (xsltReader);
				xslt.Transform (xmlReader, null, writer);
				writer.Flush ();
			}
		}

		public bool TestsSucceeded (LogStream listener_log, bool timed_out, bool crashed)
		{
			string log;
			using (var reader = listener_log.GetReader ())
				log = reader.ReadToEnd ();
			// parsing the result is different if we are in jenkins or nor.
			if (Harness.InJenkins) {
				// we have to parse the xml result
				crashed = false;
				if (log.Contains ("test-results")) {
					// remove any possible extra info
					var index = log.IndexOf ("<test-results");
					var header = log.Substring(0, log.IndexOf ('<'));
					log = log.Remove (0, index - 1);
					var testsResults = new XmlDocument ();
					testsResults.LoadXml (log);

					var mainResultNode = testsResults.SelectSingleNode("test-results");
					if (mainResultNode == null) {
						Harness.LogWrench ($"Node is null.");
						crashed = true;
						return false;
					}
					// update the information of the main node to add information about the mode and the test that is excuted. This will later create
					// nicer reports in jenkins
					mainResultNode.Attributes["name"].Value = Target.AsString ();
					// store a clean version of the logs, later this will be used by the bots to show results in github/web
					var path = listener_log.FullPath;
					path = path.Replace (".log", ".xml");
					testsResults.Save (path);
					Logs.Add (new LogFile ("Test xml", path));
					// we want to keep the old TestResult page,
					GenerateHumanReadableLogs (listener_log.FullPath, header, testsResults);
					
					int ignored = Convert.ToInt16(mainResultNode.Attributes["ignored"].Value);
					int invalid = Convert.ToInt16(mainResultNode.Attributes["invalid"].Value);
					int inconclusive = Convert.ToInt16(mainResultNode.Attributes["inconclusive"].Value);
					int errors = Convert.ToInt16(mainResultNode.Attributes["errors"].Value);
					int failures = Convert.ToInt16(mainResultNode.Attributes["failures"].Value);
					int totalTests = Convert.ToInt16(mainResultNode.Attributes["total"].Value);

					// generate human readable logs
					var failed = errors != 0 || failures != 0;
					if (failed) {
						Harness.LogWrench ($"@MonkeyWrench: AddSummary: <b>{mode} failed: Test run: {totalTests} Passed: {totalTests - invalid - inconclusive - ignored} Inconclusive: {inconclusive} Failed: {errors + failures} Ignored: {ignored}</b><br/>");
						main_log.WriteLine ("Test run failed");
						return false;
					} else {
						Harness.LogWrench ($"@MonkeyWrench: AddSummary: {mode} succeeded: Test run: {totalTests} Passed: {totalTests - invalid - inconclusive - ignored} Inconclusive: {inconclusive} Failed: 0 Ignored: {ignored}<br/>");
						main_log.WriteLine ("Test run succeeded");
						return true;
					}
				} else if (timed_out) {
					Harness.LogWrench ($"@MonkeyWrench: AddSummary: <b><i>{mode} timed out</i></b><br/>");
					return false;
				} else {
					Harness.LogWrench ($"@MonkeyWrench: AddSummary: <b><i>{mode} crashed</i></b><br/>");
					main_log.WriteLine ("Test run crashed");
					crashed = true;
					return false;
				}
			} else {
				// parsing the human readable results
				if (log.Contains ("Tests run")) {
					var tests_run = string.Empty;
					var log_lines = log.Split ('\n');
					var failed = false;
					foreach (var line in log_lines) {
						if (line.Contains ("Tests run:")) {
							Console.WriteLine (line);
							tests_run = line.Replace ("Tests run: ", "");
							break;
						} else if (line.Contains ("FAIL")) {
							Console.WriteLine (line);
							failed = true;
						}
					}

					if (failed) {
						Harness.LogWrench ("@MonkeyWrench: AddSummary: <b>{0} failed: {1}</b><br/>", mode, tests_run);
						main_log.WriteLine ("Test run failed");
						return false;
					} else {
						Harness.LogWrench ("@MonkeyWrench: AddSummary: {0} succeeded: {1}<br/>", mode, tests_run);
						main_log.WriteLine ("Test run succeeded");
						return true;
					}
				} else if (timed_out) {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} timed out</i></b><br/>", mode);
					return false;
				} else {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed</i></b><br/>", mode);
					main_log.WriteLine ("Test run crashed");
					crashed = true;
					return false;
				}
			}
		}

		[DllImport ("/usr/lib/libc.dylib")]
		extern static IntPtr ttyname (int filedes);

		public async Task<int> RunAsync ()
		{
			CrashReportSnapshot crash_reports;
			LogStream device_system_log = null;
			LogStream listener_log = null;
			Log run_log = main_log;

			Initialize ();

			if (!isSimulator)
				FindDevice ();

			crash_reports = new CrashReportSnapshot ()
			{
				Device = !isSimulator,
				DeviceName = device_name,
				Harness = Harness,
				Log = main_log,
				Logs = Logs,
				LogDirectory = LogDirectory,
			};

			var args = new StringBuilder ();
			if (!string.IsNullOrEmpty (Harness.XcodeRoot))
				args.Append (" --sdkroot ").Append (Harness.XcodeRoot);
			for (int i = -1; i < Harness.Verbosity; i++)
				args.Append (" -v ");
			args.Append (" -argument=-connection-mode -argument=none"); // This will prevent the app from trying to connect to any IDEs
			args.Append (" -argument=-app-arg:-autostart");
			args.Append (" -setenv=NUNIT_AUTOSTART=true");
			args.Append (" -argument=-app-arg:-autoexit");
			args.Append (" -setenv=NUNIT_AUTOEXIT=true");
			args.Append (" -argument=-app-arg:-enablenetwork");
			args.Append (" -setenv=NUNIT_ENABLE_NETWORK=true");
			// detect if we are using a jenkins bot.
			if (Harness.InJenkins) 
				args.Append (" -setenv=NUNIT_ENABLE_XML_OUTPUT=true");

			if (isSimulator) {
				args.Append (" -argument=-app-arg:-hostname:127.0.0.1");
				args.Append (" -setenv=NUNIT_HOSTNAME=127.0.0.1");
			} else {
				var ips = new StringBuilder ();
				var ipAddresses = System.Net.Dns.GetHostEntry (System.Net.Dns.GetHostName ()).AddressList;
				for (int i = 0; i < ipAddresses.Length; i++) {
					if (i > 0)
						ips.Append (',');
					ips.Append (ipAddresses [i].ToString ());
				}

				args.AppendFormat (" -argument=-app-arg:-hostname:{0}", ips.ToString ());
				args.AppendFormat (" -setenv=NUNIT_HOSTNAME={0}", ips.ToString ());
			}
			var transport = mode == "watchos" ? "HTTP" : "TCP";
			args.AppendFormat (" -argument=-app-arg:-transport:{0}", transport);
			args.AppendFormat (" -setenv=NUNIT_TRANSPORT={0}", transport);

			SimpleListener listener;
			switch (transport) {
			case "HTTP":
				listener = new SimpleHttpListener ();
				break;
			case "TCP":
				listener = new SimpleTcpListener ();
				break;
			default:
				throw new NotImplementedException ();
			}
			listener_log = Logs.CreateStream (LogDirectory, string.Format ("test-{0}-{1:yyyyMMdd_HHmmss}.log", mode, DateTime.Now), "Test log");
			listener.TestLog = listener_log;
			listener.Log = main_log;
			listener.AutoExit = true;
			listener.Address = System.Net.IPAddress.Any;
			listener.Initialize ();

			args.AppendFormat (" -argument=-app-arg:-hostport:{0}", listener.Port);
			args.AppendFormat (" -setenv=NUNIT_HOSTPORT={0}", listener.Port);

			listener.StartAsync ();

			var cancellation_source = new CancellationTokenSource ();
			var timed_out = false;

			ThreadPool.QueueUserWorkItem ((v) =>
			{
				if (!listener.WaitForConnection (TimeSpan.FromMinutes (Harness.LaunchTimeout))) {
					cancellation_source.Cancel ();
					main_log.WriteLine ("Test launch timed out after {0} minute(s).", Harness.LaunchTimeout);
					timed_out = true;
				} else {
					main_log.WriteLine ("Test run started");
				}
			});

			foreach (var kvp in Harness.EnvironmentVariables)
				args.AppendFormat (" -setenv={0}={1}", kvp.Key, kvp.Value);

			bool? success = null;
			bool launch_failure = false;

			if (isExtension) {
				switch (extension) {
				case Extension.TodayExtension:
					args.Append (isSimulator ? " --launchsimbundleid" : " --launchdevbundleid");
					args.Append (" todayviewforextensions:");
					args.Append (BundleIdentifier);
					args.Append (" --observe-extension ");
					args.Append (Harness.Quote (launchAppPath));
					break;
				case Extension.WatchKit2:
				default:
					throw new NotImplementedException ();
				}
			} else {
				args.Append (isSimulator ? " --launchsim " : " --launchdev ");
				args.Append (Harness.Quote (launchAppPath));
			}

			if (isSimulator) {
				if (!await FindSimulatorAsync ())
					return 1;

				if (mode != "watchos") {
					var stderr_tty = Marshal.PtrToStringAuto (ttyname (2));
					if (!string.IsNullOrEmpty (stderr_tty)) {
						args.Append (" --stdout=").Append (Harness.Quote (stderr_tty));
						args.Append (" --stderr=").Append (Harness.Quote (stderr_tty));
					} else {
						var stdout_log = Logs.CreateFile ("Standard output", Path.Combine (LogDirectory, "stdout.log"));
						var stderr_log = Logs.CreateFile ("Standard error", Path.Combine (LogDirectory, "stderr.log"));
						args.Append (" --stdout=").Append (Harness.Quote (stdout_log.FullPath));
						args.Append (" --stderr=").Append (Harness.Quote (stderr_log.FullPath));
					}
				}

				var systemLogs = new List<CaptureLog> ();
				foreach (var sim in simulators) {
					// Upload the system log
					main_log.WriteLine ("System log for the '{1}' simulator is: {0}", sim.SystemLog, sim.Name);
					bool isCompanion = sim != simulator;

					var log = new CaptureLog (sim.SystemLog, entire_file: Harness.Action != HarnessAction.Jenkins)
					{
						Path = Path.Combine (LogDirectory, sim.Name + ".log"),
						Description = isCompanion ? "System log (companion)" : "System log",
					};
					log.StartCapture ();
					Logs.Add (log);
					systemLogs.Add (log);
					Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", log.Path);
				}

				main_log.WriteLine ("*** Executing {0}/{1} in the simulator ***", appName, mode);

				if (EnsureCleanSimulatorState) {
					foreach (var sim in simulators)
						await sim.PrepareSimulatorAsync (main_log, bundle_identifier);
				}

				args.Append (" --device=:v2:udid=").Append (simulator.UDID).Append (" ");

				await crash_reports.StartCaptureAsync ();

				main_log.WriteLine ("Starting test run");

				var result = await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), run_log, TimeSpan.FromMinutes (Harness.Timeout), cancellation_token: cancellation_source.Token);
				if (result.TimedOut) {
					timed_out = true;
					success = false;
					main_log.WriteLine ("Test run timed out after {0} minute(s).", Harness.Timeout);
				} else if (result.Succeeded) {
					main_log.WriteLine ("Test run completed");
					success = true;
				} else {
					main_log.WriteLine ("Test run failed");
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
									main_log.WriteLine ("Could not parse pid: {0}", pidstr);
							} else if (line.Contains ("Xamarin.Hosting: Launched ") && line.Contains (" with pid ")) {
								var pidstr = line.Substring (line.LastIndexOf (' '));
								if (!int.TryParse (pidstr, out pid))
									main_log.WriteLine ("Could not parse pid: {0}", pidstr);
							} else if (line.Contains ("error MT1008")) {
								launch_failure = true;
							}
						}
					}
					if (pid > 0) {
						var launchTimedout = cancellation_source.IsCancellationRequested;
						var timeoutType = launchTimedout ? "Launch" : "Completion";
						var timeoutValue = launchTimedout ? Harness.LaunchTimeout : Harness.Timeout;
						main_log.WriteLine ($"{timeoutType} timed out after {timeoutValue}");
						await Process_Extensions.KillTreeAsync (pid, main_log, true);
					} else {
						main_log.WriteLine ("Could not find pid in mtouch output.");
					}
				}

				listener.Cancel ();

				// cleanup after us
				if (EnsureCleanSimulatorState)
					await SimDevice.KillEverythingAsync (main_log);

				foreach (var log in systemLogs)
					log.StopCapture ();
				
			} else {
				main_log.WriteLine ("*** Executing {0}/{1} on device '{2}' ***", appName, mode, device_name);

				if (mode == "watchos") {
					args.Append (" --attach-native-debugger"); // this prevents the watch from backgrounding the app.
				} else {
					args.Append (" --wait-for-exit");
				}
				
				AddDeviceName (args);

				device_system_log = Logs.CreateStream (LogDirectory, $"device-{device_name}-{DateTime.Now:yyyyMMdd_HHmmss}.log", "Device log");
				var logdev = new DeviceLogCapturer () {
					Harness =  Harness,
					Log = device_system_log,
					DeviceName = device_name,
				};
				logdev.StartCapture ();

				await crash_reports.StartCaptureAsync ();

				main_log.WriteLine ("Starting test run");

				var result = await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), main_log, TimeSpan.FromMinutes (Harness.Timeout), cancellation_token: cancellation_source.Token);
				if (result.TimedOut) {
					timed_out = true;
					success = false;
					main_log.WriteLine ("Test run timed out after {0} minute(s).", Harness.Timeout);
				} else if (result.Succeeded) {
					main_log.WriteLine ("Test run completed");
					success = true;
				} else {
					main_log.WriteLine ("Test run failed");
					success = false;
				}

				logdev.StopCapture ();

				// Upload the system log
				if (File.Exists (device_system_log.FullPath)) {
					main_log.WriteLine ("A capture of the device log is: {0}", device_system_log.FullPath);
					Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", device_system_log.FullPath);
				}
			}

			listener.Dispose ();

			// check the final status
			var crashed = false;
			if (File.Exists (listener_log.FullPath)) {
				Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", listener_log.FullPath);
				success = TestsSucceeded (listener_log, timed_out, crashed);
			} else if (timed_out) {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} never launched</i></b><br/>", mode);
				main_log.WriteLine ("Test run never launched");
				success = false;
			} else if (launch_failure) {
 				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} failed to launch</i></b><br/>", mode);
 				main_log.WriteLine ("Test run failed to launch");
 				success = false;
			} else {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed at startup (no log)</i></b><br/>", mode);
				main_log.WriteLine ("Test run crashed before it started (no log file produced)");
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
							using (var log_reader = main_log.GetReader ()) {
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
						if (crash_reason != null)
							break;
					} catch (Exception e) {
						Console.WriteLine ("Failed to process crash report {1}: {0}", e.Message, crash.Description);
					}
				}
				if (!string.IsNullOrEmpty (crash_reason)) {
					if (crash_reason == "per-process-limit") {
						FailureMessage = "Killed due to using too much memory (per-process-limit).";
					} else {
						FailureMessage = $"Killed by the OS ({crash_reason})";
					}
				}
			}

			return success.Value ? 0 : 1;
		}

		public void AddDeviceName (StringBuilder args)
		{
			AddDeviceName (args, device_name);
		}

		public static void AddDeviceName (StringBuilder args, string device_name)
		{
			if (!string.IsNullOrEmpty (device_name)) {
				args.Append (" --devname ");
				args.Append (Harness.Quote (device_name));
			}
		}
	}
}
