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

namespace xharness
{
	public class AppRunner
	{
		public Harness Harness;
		public string ProjectFile;

		public TestExecutingResult Result { get; private set; }

		string appName;
		string appPath;
		string launchAppPath;
		string bundle_identifier;
		string platform;
		bool isSimulator;

		string device_name;
		string companion_device_name;

		// For watch apps we end up with 2 simulators, the watch simulator (the main one), and the iphone simulator (the companion one).
		SimDevice[] simulators;
		SimDevice simulator { get { return simulators [0]; } }
		SimDevice companion_simulator { get { return simulators.Length == 2 ? simulators [1] : null; } }

		string target;
		public string Target {
			get { return target ?? Harness.Target; }
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

		void FindSimulator ()
		{
			if (simulators != null)
				return;
			
			string simulator_devicetype;
			string simulator_runtime;

			switch (Target) {
			case "ios-simulator-32":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case "ios-simulator-64":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5s";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case "ios-simulator":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case "tvos-simulator":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.tvOS-" + Xamarin.SdkVersions.TVOS.Replace ('.', '-');
				break;
			case "watchos-simulator":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-38mm";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.watchOS-" + Xamarin.SdkVersions.WatchOS.Replace ('.', '-');
				break;
			default:
				throw new Exception (string.Format ("Unknown simulator target: {0}", Harness.Target));
			}

			var sims = new Simulators () {
				Harness = Harness,
			};
			Task.Run (async () =>
			{
				await sims.LoadAsync (Logs.CreateStream (LogDirectory, "simulator-list.log", "Simulator list"));
			}).Wait ();

			var devices = sims.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == simulator_runtime && v.SimDeviceType == simulator_devicetype);
			SimDevice candidate = null;
			simulators = null;
			foreach (var device in devices) {
				var data = device;
				var secondaryData = (SimDevice) null;
				var nodeCompanions = sims.AvailableDevicePairs.Where ((SimDevicePair v) => v.Companion == device.UDID);
				var nodeGizmos = sims.AvailableDevicePairs.Where ((SimDevicePair v) => v.Gizmo == device.UDID);

				if (nodeCompanions.Any ()) {
					var gizmo_udid = nodeCompanions.First ().Gizmo;
					var node = sims.AvailableDevices.Where ((SimDevice v) => v.UDID == gizmo_udid);
					secondaryData = node.First ();
				} else if (nodeGizmos.Any ()) {
					var companion_udid = nodeGizmos.First ().Companion;
					var node = sims.AvailableDevices.Where ((SimDevice v) => v.UDID == companion_udid);
					secondaryData = node.First ();
				}
				if (secondaryData != null) {
					simulators = new SimDevice [] { data, secondaryData };
					break;
				} else {
					candidate = data;
				}
			}
			if (simulators == null) {
				if (candidate == null)
					throw new Exception ($"Could not find simulator for runtime={simulator_runtime} and device type={simulator_devicetype}.");
				simulators = new SimDevice [] { candidate };
			}

			if (simulators == null)
				throw new Exception ("Could not find simulator");

			main_log.WriteLine ("Found simulator: {0} {1}", simulators [0].Name, simulators [0].UDID);
			if (simulators.Length > 1)
				main_log.WriteLine ("Found companion simulator: {0} {1}", simulators [1].Name, simulators [1].UDID);
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
				selected_data = selected.First ();
				main_log.WriteLine ("Found {0} devices for device class(es) {1}: {2}. Selected: '{3}'", selected.Count (), string.Join (", ", deviceClasses), string.Join (", ", selected.Select ((v) => v.Name).ToArray ()), selected_data.Name);
			} else {
				selected_data = selected.First ();
			}
			device_name = selected_data.Name;

			if (mode == "watchos") {
				var companion = devs.ConnectedDevices.Where ((v) => v.DeviceIdentifier == selected_data.CompanionIdentifier);
				if (companion.Count () == 0)
					throw new Exception ($"Could not find the companion device for '{selected_data.Name}'");
				else if (companion.Count () > 1)
					main_log.WriteLine ("Found {0} companion devices for {1}?!?", companion.Count (), selected_data.Name);
				companion_device_name = companion.First ().Name;
			}
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
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Path.GetDirectoryName (ProjectFile), info_plist_path));
			bundle_identifier = info_plist.GetCFBundleIdentifier ();

			switch (Target) {
			case "ios-simulator-32":
				mode = "sim32";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case "ios-simulator-64":
				mode = "sim64";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case "ios-simulator":
				mode = "classic";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case "ios-device":
				mode = "ios";
				platform = "iPhone";
				isSimulator = false;
				break;
			case "tvos-simulator":
				mode = "tvos";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case "tvos-device":
				mode = "tvos";
				platform = "iPhone";
				isSimulator = false;
				break;
			case "watchos-simulator":
				mode = "watchos";
				platform = "iPhoneSimulator";
				isSimulator = true;
				break;
			case "watchos-device":
				mode = "watchos";
				platform = "iPhone";
				isSimulator = false;
				break;
			default:
				throw new Exception (string.Format ("Unknown target: {0}", Harness.Target));
			}

			appPath = Path.Combine (Path.GetDirectoryName (ProjectFile), csproj.GetOutputPath (platform, Harness.Configuration).Replace ('\\', '/'), appName + ".app");
			if (!Directory.Exists (appPath))
				throw new Exception (string.Format ("The app directory {0} does not exist. This is probably a bug in the test harness.", appPath));

			if (mode == "watchos") {
				launchAppPath = Directory.GetDirectories (Path.Combine (appPath, "Watch"), "*.app") [0];
			} else {
				launchAppPath = appPath;
			}
		}

		public int Install (Log log)
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

			var rv = ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), log, TimeSpan.FromHours (1)).Result;
			return rv.Succeeded ? 0 : 1;
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

		public async Task<int> RunAsync ()
		{
			CrashReportSnapshot crash_reports = new CrashReportSnapshot () { Device = !isSimulator, Harness = Harness, Log = main_log, Logs = Logs, LogDirectory = LogDirectory };
			LogStream device_system_log = null;
			LogStream listener_log = null;
			Log run_log = main_log;

			Initialize ();

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
			listener_log = Logs.CreateStream (LogDirectory, string.Format ("test-{0:yyyyMMdd_HHmmss}.log", DateTime.Now), "Test log");
			listener.TestLog = listener_log;
			listener.Log = main_log;
			listener.AutoExit = true;
			listener.Address = System.Net.IPAddress.Any;
			listener.Initialize ();

			args.AppendFormat (" -argument=-app-arg:-hostport:{0}", listener.Port);
			args.AppendFormat (" -setenv=NUNIT_HOSTPORT={0}", listener.Port);

			bool? success = null;
			bool timed_out = false;

			if (isSimulator) {
				FindSimulator ();

				var systemLogs = new List<CaptureLog> ();
				foreach (var sim in simulators) {
					// Upload the system log
					main_log.WriteLine ("System log for the '{1}' simulator is: {0}", sim.SystemLog, sim.Name);
					bool isCompanion = sim != simulator;

					var log = new CaptureLog (sim.SystemLog) {
						Path = Path.Combine (LogDirectory, sim.UDID + ".log"),
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

				args.Append (" --launchsim");
				args.AppendFormat (" \"{0}\" ", launchAppPath);
				args.Append (" --device=:v2:udid=").Append (simulator.UDID).Append (" ");

				await crash_reports.StartCaptureAsync ();

				listener.StartAsync ();
				main_log.WriteLine ("Starting test run");

				var cancellation_source = new CancellationTokenSource ();
				ThreadPool.QueueUserWorkItem ((v) => {
					if (!listener.WaitForConnection (TimeSpan.FromMinutes (Harness.LaunchTimeout))) {
						cancellation_source.Cancel ();
						main_log.WriteLine ("Test launch timed out after {0} minute(s).", Harness.LaunchTimeout);
					} else {
						main_log.WriteLine ("Test run started");
					}
				});
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
							}
						}
					}
					if (pid > 0) {
						var launchTimedout = cancellation_source.IsCancellationRequested;
						await KillPidAsync (main_log, pid, TimeSpan.FromSeconds (5), TimeSpan.FromMinutes (launchTimedout ? Harness.LaunchTimeout : Harness.Timeout), launchTimedout ? "Launch" : "Completion");
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
				FindDevice ();

				main_log.WriteLine ("*** Executing {0}/{1} on device ***", appName, mode);

				args.Append (" --launchdev");
				args.AppendFormat (" \"{0}\" ", launchAppPath);
				
				AddDeviceName (args);

				device_system_log = Logs.CreateStream (LogDirectory, "device.log", "Device log");
				var logdev = new DeviceLogCapturer () {
					Harness =  Harness,
					Log = device_system_log,
					DeviceName = device_name,
				};
				logdev.StartCapture ();

				await crash_reports.StartCaptureAsync ();

				listener.StartAsync ();
				main_log.WriteLine ("Starting test run");
				// This will not wait for app completion
				await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, args.ToString (), main_log, TimeSpan.FromMinutes (1));
				if (listener.WaitForCompletion (TimeSpan.FromMinutes (Harness.Timeout))) {
					main_log.WriteLine ("Test run completed");
				} else {
					main_log.WriteLine ("Test run did not complete in {0} minutes.", Harness.Timeout);
					listener.Cancel ();
					success = false;
					timed_out = true;
				}

				logdev.StopCapture ();

				// Upload the system log
				if (File.Exists (device_system_log.FullPath)) {
					main_log.WriteLine ("A capture of the device log is: {0}", device_system_log.FullPath);
					if (Harness.InWrench)
						Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", device_system_log.FullPath);
				}
			}

			listener.Dispose ();

			// check the final status
			var crashed = false;
			if (File.Exists (listener_log.FullPath)) {
				Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", listener_log.FullPath);
				string log;
				using (var reader = listener_log.GetReader ())
					log = reader.ReadToEnd ();
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
						success = false;
					} else {
						Harness.LogWrench ("@MonkeyWrench: AddSummary: {0} succeeded: {1}<br/>", mode, tests_run);
						main_log.WriteLine ("Test run succeeded");
						success = true;
					}
				} else if (timed_out) {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} timed out</i></b><br/>", mode);
				} else {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed</i></b><br/>", mode);
					main_log.WriteLine ("Test run crashed");
					crashed = true;
				}
			} else if (timed_out) {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} never launched</i></b><br/>", mode);
				main_log.WriteLine ("Test run never launched");
			} else {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed at startup (no log)</i></b><br/>", mode);
				main_log.WriteLine ("Test run crashed before it started (no log file produced)");
				crashed = true;
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

		[DllImport ("/usr/lib/libc.dylib")]
		static extern void kill (int pid, int sig);

		async Task KillPidAsync (Log log, int pid, TimeSpan kill_separation, TimeSpan timeout, string type)
		{
			log.WriteLine ("{2} timeout ({1} s) reached, will now send SIGQUIT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
			kill (pid, 3 /* SIGQUIT */); // print managed stack traces.
			if (await ProcessHelper.PollForExitAsync (pid, kill_separation /* wait for at most 5 seconds to see if something happens */))
				return;
			
			log.WriteLine ("{2} timeout ({1} s) reached, will now send SIGABRT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
			kill (pid, 6 /* SIGABRT */); // print native stack traces.
			if (await ProcessHelper.PollForExitAsync (pid, kill_separation /* wait another 5 seconds */))
				return;
			
			log.WriteLine ("{2} timeout ({1} s) reached, will now send SIGKILL to the app (PID: {0})", pid, timeout.TotalSeconds, type);
			kill (pid, 9 /* SIGKILL */); // terminate unconditionally.
		}
	}
}

