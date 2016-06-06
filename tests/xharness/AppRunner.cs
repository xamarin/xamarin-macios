using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace xharness
{
	public class AppRunner
	{
		public Harness Harness;
		public string ProjectFile;

		XmlDocument simulator_data;

		string appName;
		string appPath;
		string launchAppPath;
		string bundle_identifier;
		string platform;
		bool isSimulator;

		string device_name;
		string companion_device_name;

		class IData {
			public XmlNode Node;
             
			public string udid { get { return Node.Attributes ["UDID"].Value; } }
		}

		class SimulatorData : IData {
			public string datapath { get { return Node.SelectSingleNode ("DataPath").InnerText; } }
			public string logpath { get { return Node.SelectSingleNode ("LogPath").InnerText; } }
			public string devicetype { get { return Node.SelectSingleNode ("SimDeviceType").InnerText; } }
			public string runtime { get { return Node.SelectSingleNode ("SimRuntime").InnerText; } }
			public string system_log { get { return Path.Combine (logpath, "system.log"); } }
			public string name { get { return Node.Attributes ["Name"].Value; } }
		}

		class DeviceData : IData {
			public string DeviceIdentifier { get { return Node.SelectSingleNode ("DeviceIdentifier")?.InnerText; } }
			public string DeviceClass { get { return Node.SelectSingleNode ("DeviceClass")?.InnerText; } }
			public string CompanionIdentifier { get { return Node.SelectSingleNode ("CompanionIdentifier")?.InnerText; } }
			public string Name { get { return Node.SelectSingleNode ("Name")?.InnerText; } }
		}

		// For watch apps we end up with 2 simulators, the watch simulator (the main one), and the iphone simulator (the companion one).
		SimulatorData[] simulators;
		SimulatorData simulator { get { return simulators [0]; } }
		SimulatorData companion_simulator { get { return simulators.Length == 2 ? simulators [1] : null; } }

		string mode;

		string SymbolicateCrashReport (string report)
		{
			var symbolicatecrash = Path.Combine (Harness.XcodeRoot, "Contents/SharedFrameworks/DTDeviceKitBase.framework/Versions/A/Resources/symbolicatecrash");
			if (!File.Exists (symbolicatecrash))
				symbolicatecrash = Path.Combine (Harness.XcodeRoot, "Contents/SharedFrameworks/DVTFoundation.framework/Versions/A/Resources/symbolicatecrash");
			
			if (!File.Exists (symbolicatecrash)) {
				Harness.Log ("Can't symbolicate {0} because the symbolicatecrash script {1} does not exist", report, symbolicatecrash);
				return report;
			}

			var output = new StringBuilder ();
			if (ExecuteCommand (symbolicatecrash, "\"" + report + "\"", true, captured_output: output, environment_variables: new Dictionary<string, string> { { "DEVELOPER_DIR", Path.Combine (Harness.XcodeRoot, "Contents", "Developer") }})) {
				File.WriteAllText (report + ".symbolicated", output.ToString ());
				Harness.Log ("Symbolicated {0} successfully.", report);
				return report + ".symbolicated";
			}

			Harness.Log ("Failed to symbolicate {0}:\n{1}", report, output.ToString ());

			return report;
		}

		void FindSimulator ()
		{
			string simulator_devicetype;
			string simulator_runtime;

			switch (Harness.Target) {
			case "ios-simulator-32":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case "ios-simulator-64":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5s";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case "ios-simulator":
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-4s";
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
				
			var tmpfile = Path.GetTempFileName ();
			try {
				ExecuteCommand (Harness.MlaunchPath, string.Format ("--sdkroot {0} --listsim {1}", Harness.XcodeRoot, tmpfile), output_verbosity_level: 1);
				simulator_data = new XmlDocument ();
				simulator_data.LoadWithoutNetworkAccess (tmpfile);
				SimulatorData candidate = null;
				simulators = null;
				foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevices/SimDevice")) {
					var data = new SimulatorData { Node = sim };
					if (data.runtime == simulator_runtime && data.devicetype == simulator_devicetype) {
						var udid = data.udid;
						var secondaryData = (SimulatorData) null;
						var nodeCompanion = simulator_data.SelectSingleNode ("/MTouch/Simulator/AvailableDevicePairs/SimDevicePair/Companion[text() = '" + udid + "']");
						var nodeGizmo = simulator_data.SelectSingleNode ("/MTouch/Simulator/AvailableDevicePairs/SimDevicePair/Gizmo[text() = '" + udid + "']");

						if (nodeCompanion != null) {
							var gizmo_udid = nodeCompanion.ParentNode.SelectSingleNode ("Gizmo").InnerText;
							var node =  simulator_data.SelectSingleNode ("/MTouch/Simulator/AvailableDevices/SimDevice[@UDID = '" + gizmo_udid + "']");
							secondaryData = new SimulatorData () { Node = node };
						} else if (nodeGizmo != null) {
							var companion_udid = nodeGizmo.ParentNode.SelectSingleNode ("Companion").InnerText;
							var node = simulator_data.SelectSingleNode ("/MTouch/Simulator/AvailableDevices/SimDevice[@UDID = '" + companion_udid + "']");
							secondaryData = new SimulatorData () { Node = node };
						}
						if (secondaryData != null) {
							simulators = new SimulatorData[] { data, secondaryData };
							break;
						} else {
							candidate = data;
						}
					}
				}
				if (simulators == null)
					simulators = new SimulatorData[] { candidate };
			} finally {
				File.Delete (tmpfile);
			}

			if (simulators == null)
				throw new Exception ("Could not find simulator");

			Harness.Log (1, "Found simulator: {0} {1}", simulators [0].name, simulators [0].udid);
			if (simulators.Length > 1)
				Harness.Log (1, "Found companion simulator: {0} {1}", simulators [1].name, simulators [1].udid);
		}

		void FindDevice ()
		{
			if (device_name != null)
				return;
			
			device_name = Environment.GetEnvironmentVariable ("DEVICE_NAME");
			if (!string.IsNullOrEmpty (device_name))
				return;
			
			var tmpfile = Path.GetTempFileName ();
			try {
				ExecuteCommand (Harness.MlaunchPath, string.Format ("--sdkroot {0} --listdev={1} --output-format=xml", Harness.XcodeRoot, tmpfile), output_verbosity_level: 1);
				var doc = new XmlDocument ();
				doc.LoadWithoutNetworkAccess (tmpfile);
				var nodes = new List<DeviceData> ();
				foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
					nodes.Add (new DeviceData { Node = dev });
				}

				if (nodes.Count == 0)
					throw new Exception ("No devices connected");

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

				var selected = nodes.Where ((v) => deviceClasses.Contains (v.DeviceClass));
				DeviceData selected_data;
				if (selected.Count () == 0) {
					throw new Exception ($"Could not find any applicable devices with device class(es): {string.Join (", ", deviceClasses)}");
				} else if (selected.Count () > 1) {
					selected_data = selected.First ();
					Harness.Log ("Found {0} devices for device class(es) {1}: {2}. Selected: '{3}'", selected.Count (), string.Join (", ", deviceClasses), string.Join (", ", selected.Select ((v) => v.Name).ToArray ()), selected_data.Name);
				} else {
					selected_data = selected.First ();
				}
				device_name = selected_data.Name;

				if (mode == "watchos") {
					var companion = nodes.Where ((v) => v.DeviceIdentifier == selected_data.CompanionIdentifier);
					if (companion.Count () == 0)
						throw new Exception ($"Could not find the companion device for '{selected_data.Name}'");
					else if (companion.Count () > 1)
						Harness.Log ("Found {0} companion devices for {1}?!?", companion.Count (), selected_data.Name);
					companion_device_name = companion.First ().Name;
				}
			} finally {
				File.Delete (tmpfile);
			}
		}

		void PrepareSimulator ()
		{
			if (SkipSimulatorSetup) {
				Harness.Log (0, "Simulator setup skipped.");
				return;
			}
			
			KillEverything ();
			ShowSimulatorList ();

			// We shutdown and erase all simulators.
			// We only fixup TCC.db on the main simulator.

			foreach (var sim in simulators) {
				var udid = sim.udid;
				// erase the simulator (make sure the device isn't running first)
				ExecuteXcodeCommand ("simctl", "shutdown " + udid, true, output_verbosity_level: 1, timeout: TimeSpan.FromMinutes (1));
				ExecuteXcodeCommand ("simctl", "erase " + udid, true, output_verbosity_level: 1, timeout: TimeSpan.FromMinutes (1));

				// boot & shutdown to make sure it actually works
				ExecuteXcodeCommand ("simctl", "boot " + udid, true, output_verbosity_level: 1, timeout: TimeSpan.FromMinutes (1));
				ExecuteXcodeCommand ("simctl", "shutdown " + udid, true, output_verbosity_level: 1, timeout: TimeSpan.FromMinutes (1));
			}

			// Edit the permissions to prevent dialog boxes in the test app
			var TCC_db = Path.Combine (simulator.datapath, "data", "Library", "TCC", "TCC.db");
			if (!File.Exists (TCC_db)) {
				Harness.Log ("Opening simulator to create TCC.db");
				var simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator.app");
				if (!Directory.Exists (simulator_app))
					simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "iOS Simulator.app");

				ExecuteCommand ("open", "-a \"" + simulator_app + "\" --args -CurrentDeviceUDID " + simulator.udid, output_verbosity_level: 1);

				var tcc_creation_timeout = 60;
				var watch = new Stopwatch ();
				watch.Start ();
				while (!File.Exists (TCC_db) && watch.Elapsed.TotalSeconds < tcc_creation_timeout) {
					Harness.Log ("Waiting for simulator to create TCC.db... {0}", (int) (tcc_creation_timeout - watch.Elapsed.TotalSeconds));
					Thread.Sleep (TimeSpan.FromSeconds (1));
				}
			}

			if (File.Exists (TCC_db)) {
				var sim_services = new string [] {
					"kTCCServiceAddressBook",
					"kTCCServicePhotos",
					"kTCCServiceUbiquity",
					"kTCCServiceWillow"
				};

				var failure = false;
				var tcc_edit_timeout = 5;
				var watch = new Stopwatch ();
				watch.Start ();
				do {
					failure = false;
					foreach (var service in sim_services) {
						if (!ExecuteCommand ("sqlite3", string.Format ("{0} \"DELETE FROM access WHERE service = '{1}' and client ='{2}';\"", TCC_db, service, bundle_identifier), true, output_verbosity_level: 1)) {
							failure = true;
						} else {
							if (!ExecuteCommand ("sqlite3", string.Format ("{0} \"INSERT INTO access VALUES('{1}','{2}',0,1,0,NULL,NULL);\"", TCC_db, service, bundle_identifier), true, output_verbosity_level: 1)) {
								failure = true;
							}
						}
					}
					if (failure) {
						if (watch.Elapsed.TotalSeconds > tcc_edit_timeout)
							break;
						Harness.Log ("Failed to edit TCC.db, trying again in 1 second... ", (int) (tcc_edit_timeout - watch.Elapsed.TotalSeconds));
						Thread.Sleep (TimeSpan.FromSeconds (1));
					}
				} while (failure);

				if (failure) {
					Harness.Log ("Failed to edit TCC.db, the test run might hang due to permission request dialogs");
				} else {
					Harness.Log ("Successfully edited TCC.db");
				}
			} else {
				Harness.Log ("No TCC.db found for the simulator {0} (SimRuntime={1} and SimDeviceType={1})", simulator.udid, simulator.runtime, simulator.devicetype);
			}
				
			foreach (var sim in simulators) {
				if (!File.Exists (sim.system_log)) {
					Harness.Log ("No system log found for SimRuntime={0} and SimDeviceType={1}", sim.runtime, sim.devicetype);
				} else {
					File.WriteAllText (sim.system_log, string.Format (" *** This log file was cleared out by Xamarin.iOS's test run at {0} **** \n", DateTime.Now.ToString ()));
				}
			}

			KillEverything ();

			ExecuteXcodeCommand ("simctl", "shutdown " + simulator.udid, true, output_verbosity_level: 1, timeout: TimeSpan.FromMinutes (1));
		}

		void Initialize ()
		{
			var csproj = new XmlDocument ();
			csproj.LoadWithoutNetworkAccess (ProjectFile);
			appName = csproj.GetAssemblyName ();
			var info_plist_path = csproj.GetInfoPListInclude ();
			var info_plist = new XmlDocument ();
			info_plist.LoadWithoutNetworkAccess (Path.Combine (Path.GetDirectoryName (ProjectFile), info_plist_path));
			bundle_identifier = info_plist.GetCFBundleIdentifier ();

			switch (Harness.Target) {
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

		public int Install ()
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

			var success = ExecuteCommand (Harness.MlaunchPath, args.ToString ());
			return success ? 0 : 1;
		}

		bool SkipSimulatorSetup {
			get {
				return !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("SKIP_SIMULATOR_SETUP"));
			}
		}

		public int Run ()
		{
			HashSet<string> start_crashes = null;
			string device_system_log = null;

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
			listener.LogPath = Path.GetDirectoryName (Harness.LogFile);
			listener.LogFile = Path.GetFileName (Harness.LogFile);
			listener.AutoExit = true;
			listener.Address = System.Net.IPAddress.Any;
			listener.Initialize ();

			args.AppendFormat (" -argument=-app-arg:-hostport:{0}", listener.Port);
			args.AppendFormat (" -setenv=NUNIT_HOSTPORT={0}", listener.Port);

			if (File.Exists (Harness.LogFile))
				File.Delete (Harness.LogFile);

			bool? success = null;
			bool timed_out = false;

			if (isSimulator) {
				FindSimulator ();

				Harness.Log ("*** Executing {0}/{1} in the simulator ***", appName, mode);

				PrepareSimulator ();

				args.Append (" --launchsim");
				args.AppendFormat (" \"{0}\" ", launchAppPath);
				args.Append (" --device=:v2:udid=").Append (simulator.udid).Append (" ");

				start_crashes = CreateCrashReportsSnapshot (true);

				listener.StartAsync ();
				Harness.Log ("Starting test run");
				var proc = new XProcess () {
					Harness = Harness,
					FileName = Harness.MlaunchPath,
					Arguments = args.ToString (),
					VerbosityLevel = 0,
				};
				proc.Start ();

				var launchState = 0; // 0: launching, 1: launch timed out, 2: run timed out, 3: completed
				var launchMutex = new Mutex ();
				var runCompleted = new ManualResetEvent (false);
				ThreadPool.QueueUserWorkItem ((v) => {
					if (!listener.WaitForConnection (TimeSpan.FromMinutes (Harness.LaunchTimeout))) {
						lock (launchMutex) {
							if (launchState == 0) {
								launchState = 1;
								runCompleted.Set ();
							}
						}
						Harness.Log ("Test launch timed out after {0} minute(s).", Harness.LaunchTimeout);
					} else {
						Harness.Log ("Test run started");
					}
				});
				ThreadPool.QueueUserWorkItem ((v) => {
					var rv = proc.WaitForExit (TimeSpan.FromMinutes (Harness.Timeout));

					lock (launchMutex) {
						if (launchState == 0)
							launchState = rv ? 3 : 2;
						runCompleted.Set ();
					}

					if (rv) {
						Harness.Log ("Test run completed");
					} else {
						Harness.Log ("Test run timed out after {0} minute(s).", Harness.Timeout);
					}
				});

				runCompleted.WaitOne ();
									
				switch (launchState) {
				case 1:
				case 2:
					success = false;
					timed_out = true;

					// find pid
					var pid = -1;
					var output = proc.ReadCurrentOutput ();
					foreach (var line in output.ToString ().Split ('\n')) {
						if (line.StartsWith ("Application launched. PID = ", StringComparison.Ordinal)) {
							var pidstr = line.Substring ("Application launched. PID = ".Length);
							if (!int.TryParse (pidstr, out pid))
								Harness.Log ("Could not parse pid: {0}", pidstr);
						} else if (line.Contains ("Xamarin.Hosting: Launched ") && line.Contains (" with pid ")) {
							var pidstr = line.Substring (line.LastIndexOf (' '));
							if (!int.TryParse (pidstr, out pid))
								Harness.Log ("Could not parse pid: {0}", pidstr);
						}
					}
					if (pid > 0) {
						KillPid (proc, pid, TimeSpan.FromSeconds (5), TimeSpan.FromMinutes (launchState == 1 ? Harness.LaunchTimeout : Harness.Timeout), launchState == 1 ? "Launch" : "Completion");
					} else {
						Harness.Log ("Could not find pid in mtouch output.");
					}
					// kill mtouch too
					kill (proc.Id, 9);
					break;
				case 3:
					// Success!
					break;
				case 0: // shouldn't happen ever
				default:
					throw new Exception ($"Invalid launch state: {launchState}");
				}

				listener.Cancel ();

				// cleanup after us
				KillEverything ();
			} else {
				FindDevice ();

				Harness.Log ("*** Executing {0}/{1} on device ***", appName, mode);

				args.Append (" --launchdev");
				args.AppendFormat (" \"{0}\" ", launchAppPath);
				
				AddDeviceName (args);

				device_system_log = Harness.LogFile + ".device.log";
				var logdev = new DeviceLogCapturer () {
					Harness =  Harness,
					LogPath = device_system_log,
					DeviceName = device_name,
				};
				logdev.StartCapture ();

				start_crashes = CreateCrashReportsSnapshot (false);

				listener.StartAsync ();
				Harness.Log ("Starting test run");
				ExecuteCommand (Harness.MlaunchPath, args.ToString ());
				if (listener.WaitForCompletion (TimeSpan.FromMinutes (Harness.Timeout))) {
					Harness.Log ("Test run completed");
				} else {
					Harness.Log ("Test run did not complete in {0} minutes.", Harness.Timeout);
					listener.Cancel ();
					success = false;
					timed_out = true;
				}

				logdev.StopCapture ();

				// Upload the system log
				if (File.Exists (device_system_log)) {
					Harness.Log (1, "A capture of the device log is: {0}", device_system_log);
					if (Harness.InWrench)
						Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", Path.GetFullPath (device_system_log));
				}
			}

			listener.Dispose ();

			// check the final status
			var crashed = false;
			if (File.Exists (Harness.LogFile)) {
				Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", Path.GetFullPath (Harness.LogFile));
				var log = File.ReadAllText (Harness.LogFile);
				if (log.Contains ("Tests run")) {
					var tests_run = string.Empty;
					var log_lines = File.ReadAllLines (Harness.LogFile);
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
						Harness.Log ("Test run failed");
					} else {
						Harness.LogWrench ("@MonkeyWrench: AddSummary: {0} succeeded: {1}<br/>", mode, tests_run);
						Harness.Log ("Test run succeeded");
						success = true;
					}
				} else if (timed_out) {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} timed out</i></b><br/>", mode);
				} else {
					Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed</i></b><br/>", mode);
					Harness.Log ("Test run crashed");
					crashed = true;
				}
			} else if (timed_out) {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} never launched</i></b><br/>", mode);
				Harness.Log ("Test run never launched");
			} else {
				Harness.LogWrench ("@MonkeyWrench: AddSummary: <b><i>{0} crashed at startup (no log)</i></b><br/>", mode);
				Harness.Log ("Test run crashed before it started (no log file produced)");
				crashed = true;
			}
				
			// Check for crash reports
			var crash_report_search_done = false;
			var crash_report_search_timeout = 5;
			var watch = new Stopwatch ();
			watch.Start ();
			do {
				var end_crashes = CreateCrashReportsSnapshot (isSimulator);
				end_crashes.ExceptWith (start_crashes);
				if (end_crashes.Count > 0) {
					Harness.Log ("Found {0} new crash report(s)", end_crashes.Count);
					if (!isSimulator) {
						// Download crash reports from the device. We put them in the project directory so that they're automatically deleted on wrench
						// (if we put them in /tmp, they'd never be deleted).
						var downloaded_crash_reports = new HashSet<string> ();
						foreach (var file in end_crashes) {
							var crash_report_target = Path.Combine (Path.GetDirectoryName (ProjectFile), Path.GetFileName (file));
							if (ExecuteCommand (Harness.MlaunchPath, "--download-crash-report=" + file + " --download-crash-report-to=" + crash_report_target + " --sdkroot " + Harness.XcodeRoot)) {
								Harness.Log ("Downloaded crash report {0} to {1}", file, crash_report_target);
								crash_report_target = SymbolicateCrashReport (crash_report_target);
								downloaded_crash_reports.Add (crash_report_target);
							} else {
								Harness.Log ("Could not download crash report {0}", file);
							}
						}
						end_crashes = downloaded_crash_reports;
					}
					foreach (var cp in end_crashes) {
						Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", Path.GetFullPath (cp));
						Harness.Log ("    {0}", cp);
					}
					crash_report_search_done = true;
				} else if (!crashed && !timed_out) {
					crash_report_search_done = true;
				} else {
					if (watch.Elapsed.TotalSeconds > crash_report_search_timeout) {
						crash_report_search_done = true;
					} else {
						Harness.Log ("No crash reports, waiting a second to see if the crash report service just didn't complete in time ({0})", (int) (crash_report_search_timeout - watch.Elapsed.TotalSeconds));
						Thread.Sleep (TimeSpan.FromSeconds (1));
					}
				}
			} while (!crash_report_search_done);

			if (!success.HasValue)
				success = false;

			if (isSimulator) {
				foreach (var sim in simulators) {
					// Upload the system log
					if (File.Exists (sim.system_log)) {
						Harness.Log (success.Value ? 1 : 0, "System log for the '{1}' simulator is: {0}", sim.system_log, sim.name);
						if (Harness.InWrench) {
							var syslog = Harness.LogFile + (sim == simulator ? ".system.log" : ".companion.system.log");
							File.Copy (sim.system_log, syslog, true);
							Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", Path.GetFullPath (syslog));
						}
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

		[DllImport ("/usr/lib/libc.dylib")]
		static extern void kill (int pid, int sig);

		void KillPid (XProcess proc, int pid, TimeSpan kill_separation, TimeSpan timeout, string type)
		{
			Harness.Log ("{2} timeout ({1} s) reached, will now send SIGQUIT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
			kill (pid, 3 /* SIGQUIT */); // print managed stack traces.
			if (!proc.WaitForExit (kill_separation /* wait for at most 5 seconds to see if something happens */)) {
				Harness.Log ("{2} timeout ({1} s) reached, will now send SIGABRT to the app (PID: {0})", pid, timeout.TotalSeconds, type);
				kill (pid, 6 /* SIGABRT */); // print native stack traces.
				if (!proc.WaitForExit (kill_separation /* wait another 5 seconds */)) {
					Harness.Log ("{2} timeout ({1} s) reached, will now send SIGKILL to the app (PID: {0})", pid, timeout.TotalSeconds, type);
					kill (pid, 9 /* SIGKILL */); // terminate unconditionally.
				}
			}
		}

		HashSet<string> CreateCrashReportsSnapshot (bool simulator)
		{
			HashSet<string> rv;

			if (simulator) {
				rv = new HashSet<string> (Directory.EnumerateFiles (Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Library", "Logs", "DiagnosticReports")));
			} else {
				var tmp = Path.GetTempFileName ();
				if (ExecuteCommand (Harness.MlaunchPath, "--list-crash-reports=" + tmp + " --sdkroot " + Harness.XcodeRoot, true)) {
					rv = new HashSet<string> (File.ReadAllLines (tmp));
				} else {
					rv = new HashSet<string> ();
				}
				File.Delete (tmp);
			}

			return rv;
		}

		void KillEverything ()
		{
			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService" };
			foreach (var k in to_kill)
				ExecuteCommand ("killall", "-9 \"" + k + "\"", true, output_verbosity_level: 1);
		}

		void ShowSimulatorList ()
		{
			if (Harness.Verbosity > 0)
				ExecuteXcodeCommand ("simctl", "list", ignore_errors: true, timeout: TimeSpan.FromSeconds (10));
		}

		bool ExecuteXcodeCommand (string executable, string args, bool ignore_errors = false, int output_verbosity_level = 1, TimeSpan? timeout = null)
		{
			return ExecuteCommand (Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "usr", "bin", executable), args, ignore_errors, output_verbosity_level, timeout: timeout);
		}

		bool ExecuteCommand (string filename, string args, bool ignore_errors = false, int output_verbosity_level = 1, StringBuilder captured_output = null, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null)
		{
			int exitcode;
			return ExecuteCommand (filename, args, out exitcode, ignore_errors, output_verbosity_level, captured_output, timeout, environment_variables);
		}

		bool ExecuteCommand (string filename, string args, out int exitcode, bool ignore_errors = false, int output_verbosity_level = 1, StringBuilder captured_output = null, TimeSpan? timeout = null, Dictionary<string, string> environment_variables = null)
		{
			if (captured_output == null)
				captured_output = new StringBuilder ();
			var streamEnds = new CountdownEvent (2);
			using (var p = new Process ()) {
				p.StartInfo.FileName = filename;
				p.StartInfo.Arguments = args;
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				if (environment_variables != null) {
					foreach (var kvp in environment_variables)
						p.StartInfo.EnvironmentVariables.Add (kvp.Key, kvp.Value);
				}
				p.OutputDataReceived += (object sender, DataReceivedEventArgs e) => 
				{
					if (e.Data == null) {
						streamEnds.Signal ();
					} else {
						lock (captured_output) {
							captured_output.AppendLine (e.Data);
							Harness.Log (output_verbosity_level, e.Data);
						}
					}
				};
				p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => 
				{
					if (e.Data == null) {
						streamEnds.Signal ();
					} else {
						lock (captured_output) {
							captured_output.AppendLine (e.Data);
							Harness.Log (output_verbosity_level, e.Data);
						}
					}
				};
				Harness.Log (output_verbosity_level, "{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
				p.Start ();
				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();
				if (p.WaitForExit (!timeout.HasValue ? int.MaxValue : (int) timeout.Value.TotalMilliseconds )) {
					streamEnds.Wait ();
					exitcode = p.ExitCode;
					if (p.ExitCode != 0 && !ignore_errors)
						throw new Exception (string.Format ("Failed to execute {0}:\n{1}", filename, captured_output.ToString ()));
					return p.ExitCode == 0;
				} else  {
					if (!ignore_errors)
						throw new Exception (string.Format ("Execution of {0} timed out after {2} minutes:\n{1}", filename, captured_output.ToString (), timeout.Value.TotalMinutes));
					else
						Harness.Log ("Execution of {0} timed out after {2} minutes:\n{1}", filename, captured_output.ToString (), timeout.Value.TotalMinutes);
					exitcode = 0;
					kill (p.Id, 9);
					return false;
				}
			}
		}
	}
}

