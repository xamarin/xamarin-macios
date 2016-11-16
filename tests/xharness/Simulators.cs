using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace xharness
{
	public class Simulators
	{
		public Harness Harness;

		public List<SimRuntime> SupportedRuntimes = new List<SimRuntime> ();
		public List<SimDeviceType> SupportedDeviceTypes = new List<SimDeviceType> ();
		public List<SimDevice> AvailableDevices = new List<SimDevice> ();
		public List<SimDevicePair> AvailableDevicePairs = new List<SimDevicePair> ();

		public async Task LoadAsync (Log log)
		{
			if (SupportedRuntimes.Count > 0)
				return;
			
			var tmpfile = Path.GetTempFileName ();
			try {
				using (var process = new Process ()) {
					process.StartInfo.FileName = Harness.MlaunchPath;
					process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listsim {1}", Harness.XcodeRoot, tmpfile);
					log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
					var rv = await process.RunAsync (log, false);
					if (!rv.Succeeded)
						throw new Exception ("Failed to list simulators.");
					log.WriteLine ("Result:");
					log.WriteLine (File.ReadAllText (tmpfile));
					var simulator_data = new XmlDocument ();
					simulator_data.LoadWithoutNetworkAccess (tmpfile);
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedRuntimes/SimRuntime")) {
						SupportedRuntimes.Add (new SimRuntime ()
						{
							Name = sim.SelectSingleNode ("Name").InnerText,
							Identifier = sim.SelectSingleNode ("Identifier").InnerText,
							Version = long.Parse (sim.SelectSingleNode ("Version").InnerText),
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType")) {
						SupportedDeviceTypes.Add (new SimDeviceType ()
						{
							Name = sim.SelectSingleNode ("Name").InnerText,
							Identifier = sim.SelectSingleNode ("Identifier").InnerText,
							ProductFamilyId = sim.SelectSingleNode ("ProductFamilyId").InnerText,
							MinRuntimeVersion = long.Parse (sim.SelectSingleNode ("MinRuntimeVersion").InnerText),
							MaxRuntimeVersion = long.Parse (sim.SelectSingleNode ("MaxRuntimeVersion").InnerText),
							Supports64Bits = bool.Parse (sim.SelectSingleNode ("Supports64Bits").InnerText),
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevices/SimDevice")) {
						AvailableDevices.Add (new SimDevice ()
						{
							Harness = Harness,
							Name = sim.Attributes ["Name"].Value,
							UDID = sim.Attributes ["UDID"].Value,
							SimRuntime = sim.SelectSingleNode ("SimRuntime").InnerText,
							SimDeviceType = sim.SelectSingleNode ("SimDeviceType").InnerText,
							DataPath = sim.SelectSingleNode ("DataPath").InnerText,
							LogPath = sim.SelectSingleNode ("LogPath").InnerText,
						});
					}
					foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevicePairs/SimDevicePair")) {
						AvailableDevicePairs.Add (new SimDevicePair ()
						{
							UDID = sim.Attributes ["UDID"].Value,
							Companion = sim.SelectSingleNode ("Companion").InnerText,
							Gizmo = sim.SelectSingleNode ("Gizmo").InnerText,

						});
					}
				}
			} finally {
				File.Delete (tmpfile);
			}
		}

		public async Task<SimDevice []> FindAsync (AppRunnerTarget target, Log log)
		{
			SimDevice [] simulators = null;

			string [] simulator_devicetypes;
			string simulator_runtime;
			string [] companion_devicetypes = null;
			string companion_runtime = null;

			switch (target) {
			case AppRunnerTarget.Simulator_iOS32:
				simulator_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.iPhone-5" };
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_iOS64:
				simulator_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.iPhone-5s" };
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_iOS:
				simulator_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.iPhone-5" };
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_tvOS:
				simulator_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p" };
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.tvOS-" + Xamarin.SdkVersions.TVOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_watchOS:
				simulator_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-38mm", "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-2-38mm" };
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.watchOS-" + Xamarin.SdkVersions.WatchOS.Replace ('.', '-');
				companion_devicetypes = new string [] { "com.apple.CoreSimulator.SimDeviceType.iPhone-6s" };
				companion_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			default:
				throw new Exception (string.Format ("Unknown simulator target: {0}", Harness.Target));
			}

			var devices = AvailableDevices.Where ((SimDevice v) =>
			{
				if (v.SimRuntime != simulator_runtime)
					return false;

				if (!simulator_devicetypes.Contains (v.SimDeviceType))
					return false;

				if (target == AppRunnerTarget.Simulator_watchOS)
					return AvailableDevicePairs.Any ((SimDevicePair pair) => pair.Companion == v.UDID || pair.Gizmo == v.UDID);

				return true;
			});

			SimDevice candidate = null;

			foreach (var device in devices) {
				var data = device;
				var secondaryData = (SimDevice) null;
				var nodeCompanions = AvailableDevicePairs.Where ((SimDevicePair v) => v.Companion == device.UDID);
				var nodeGizmos = AvailableDevicePairs.Where ((SimDevicePair v) => v.Gizmo == device.UDID);

				if (nodeCompanions.Any ()) {
					var gizmo_udid = nodeCompanions.First ().Gizmo;
					var node = AvailableDevices.Where ((SimDevice v) => v.UDID == gizmo_udid);
					secondaryData = node.First ();
				} else if (nodeGizmos.Any ()) {
					var companion_udid = nodeGizmos.First ().Companion;
					var node = AvailableDevices.Where ((SimDevice v) => v.UDID == companion_udid);
					secondaryData = node.First ();
				}
				if (secondaryData != null) {
					simulators = new SimDevice [] { data, secondaryData };
					break;
				} else {
					candidate = data;
				}
			}

			if (simulators == null && candidate == null && target == AppRunnerTarget.Simulator_watchOS) {
				// We might be only missing device pairs to match phone + watch.
				var watchDevices = AvailableDevices.Where ((SimDevice v) => { return v.SimRuntime == simulator_runtime && simulator_devicetypes.Contains (v.SimDeviceType); });
				var companionDevices = AvailableDevices.Where ((SimDevice v) => { return v.SimRuntime == companion_runtime && companion_devicetypes.Contains (v.SimDeviceType); });
				if (!watchDevices.Any () || !companionDevices.Any ()) {
					log.WriteLine ($"Could not find both watch devices for <runtime={simulator_runtime} and device type={string.Join (";", simulator_devicetypes)}> and companion device for <runtime={companion_runtime} and device type {string.Join (";", companion_devicetypes)}>");
					return null;
				}
				var watchDevice = watchDevices.First ();
				var companionDevice = companionDevices.First ();

				log.WriteLine ($"Creating device pair for '{watchDevice.Name}' and '{companionDevice.Name}'");
				var rv = await Harness.ExecuteXcodeCommandAsync ("simctl", $"pair {watchDevice.UDID} {companionDevice.UDID}", log, TimeSpan.FromMinutes (1));
				if (!rv.Succeeded) {
					log.WriteLine ($"Could not create device pair, so could not find simulator for runtime={simulator_runtime} and device type={string.Join ("; ", simulator_devicetypes)}.");
					return null;
				}
				AvailableDevicePairs.Add (new SimDevicePair ()
				{
					Companion = companionDevice.UDID,
					Gizmo = watchDevice.UDID,
					UDID = $"<created for {companionDevice.UDID} and {watchDevice.UDID}",
				});
				simulators = new SimDevice [] { watchDevice, companionDevice };
			}

			if (simulators == null) {
				if (candidate == null) {
					log.WriteLine ($"Could not find simulator for runtime={simulator_runtime} and device type={string.Join (";", simulator_devicetypes)}.");
					return null;
				}
				simulators = new SimDevice [] { candidate };
			}

			if (simulators == null) {
				log.WriteLine ("Could not find simulator");
				return null;
			}

			log.WriteLine ("Found simulator: {0} {1}", simulators [0].Name, simulators [0].UDID);
			if (simulators.Length > 1)
				log.WriteLine ("Found companion simulator: {0} {1}", simulators [1].Name, simulators [1].UDID);

			return simulators;
		}
	}

	public class SimRuntime
	{
		public string Name;
		public string Identifier;
		public long Version;
	}

	public class SimDeviceType
	{
		public string Name;
		public string Identifier;
		public string ProductFamilyId;
		public long MinRuntimeVersion;
		public long MaxRuntimeVersion;
		public bool Supports64Bits;
	}

	public class SimDevice
	{
		public string UDID;
		public string Name;
		public string SimRuntime;
		public string SimDeviceType;
		public string DataPath;
		public string LogPath;

		public string SystemLog { get { return Path.Combine (LogPath, "system.log"); } }

		public Harness Harness;

		public bool IsWatchSimulator { get { return SimRuntime.StartsWith ("com.apple.CoreSimulator.SimRuntime.watchOS", StringComparison.Ordinal); } }

		public async Task EraseAsync (Log log)
		{
			// here we don't care if execution fails.
			// erase the simulator (make sure the device isn't running first)
			await Harness.ExecuteXcodeCommandAsync ("simctl", "shutdown " + UDID, log, TimeSpan.FromMinutes (1));
			await Harness.ExecuteXcodeCommandAsync ("simctl", "erase " + UDID, log, TimeSpan.FromMinutes (1));

			// boot & shutdown to make sure it actually works
			await Harness.ExecuteXcodeCommandAsync ("simctl", "boot " + UDID, log, TimeSpan.FromMinutes (1));
			await Harness.ExecuteXcodeCommandAsync ("simctl", "shutdown " + UDID, log, TimeSpan.FromMinutes (1));
		}

		public async Task ShutdownAsync (Log log)
		{
			await Harness.ExecuteXcodeCommandAsync ("simctl", "shutdown " + UDID, log, TimeSpan.FromMinutes (1));
		}

		public static async Task KillEverythingAsync (Log log)
		{
			await ProcessHelper.ExecuteCommandAsync ("launchctl", "remove com.apple.CoreSimulator.CoreSimulatorService", log, TimeSpan.FromSeconds (10));

			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService" };

			await ProcessHelper.ExecuteCommandAsync ("killall", "-9 " + string.Join (" ", to_kill.Select ((v) => Harness.Quote (v)).ToArray ()), log, TimeSpan.FromSeconds (10));

			foreach (var dir in new string [] {
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.watchsimulator.savedState"),
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library", "Saved Application State", "com.apple.iphonesimulator.savedState"),
			}) {
				try {
					if (Directory.Exists (dir))
						Directory.Delete (dir, true);
				} catch (Exception e) {
					log.WriteLine ("Could not delete the directory '{0}': {1}", dir, e.Message);
				}
			}
		}

		public async Task AgreeToPromptsAsync (Log log, params string[] bundle_identifiers)
		{
			if (bundle_identifiers == null || bundle_identifiers.Length == 0) {
				log.WriteLine ("No bundle identifiers given when requested permission editing.");
				return;
			}

			var TCC_db = Path.Combine (DataPath, "data", "Library", "TCC", "TCC.db");
			var sim_services = new string [] {
					"kTCCServiceAddressBook",
					"kTCCServicePhotos",
					"kTCCServiceMediaLibrary",
					"kTCCServiceUbiquity",
					"kTCCServiceWillow"
				};

			var failure = false;
			var tcc_edit_timeout = 5;
			var watch = new Stopwatch ();
			watch.Start ();

			do {
				if (failure) {
					log.WriteLine ("Failed to edit TCC.db, trying again in 1 second... ", (int) (tcc_edit_timeout - watch.Elapsed.TotalSeconds));
					await Task.Delay (TimeSpan.FromSeconds (1));
				}
				failure = false;
				foreach (var bundle_identifier in bundle_identifiers) {
					var sql = new System.Text.StringBuilder ();
					sql.Append (Harness.Quote (TCC_db));
					sql.Append (" \"");
					foreach (var service in sim_services) {
						sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL);", service, bundle_identifier);
						sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL);", service, bundle_identifier + ".watchkitapp");
					}
					sql.Append ("\"");
					var rv = await ProcessHelper.ExecuteCommandAsync ("sqlite3", sql.ToString (), log, TimeSpan.FromSeconds (5));
					if (!rv.Succeeded) {
						failure = true;
						break;
					}
				}
			} while (failure && watch.Elapsed.TotalSeconds <= tcc_edit_timeout);

			if (failure) {
				log.WriteLine ("Failed to edit TCC.db, the test run might hang due to permission request dialogs");
			} else {
				log.WriteLine ("Successfully edited TCC.db");
			}
		}

		async Task OpenSimulator (Log log)
		{
			string simulator_app;

			if (IsWatchSimulator) {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator (Watch).app");
			} else {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator.app");
				if (!Directory.Exists (simulator_app))
					simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "iOS Simulator.app");
			}

			await ProcessHelper.ExecuteCommandAsync ("open", "-a " + Harness.Quote (simulator_app) + " --args -CurrentDeviceUDID " + UDID, log, TimeSpan.FromSeconds (15));
		}

		public async Task PrepareSimulatorAsync (Log log, params string[] bundle_identifiers)
		{
			// Kill all existing processes
			await KillEverythingAsync (log);

			// We shutdown and erase all simulators.
			await EraseAsync (log);

			// Edit the permissions to prevent dialog boxes in the test app
			var TCC_db = Path.Combine (DataPath, "data", "Library", "TCC", "TCC.db");
			if (!File.Exists (TCC_db)) {
				log.WriteLine ("Opening simulator to create TCC.db");
				await OpenSimulator (log);

				var tcc_creation_timeout = 60;
				var watch = new Stopwatch ();
				watch.Start ();
				while (!File.Exists (TCC_db) && watch.Elapsed.TotalSeconds < tcc_creation_timeout) {
					log.WriteLine ("Waiting for simulator to create TCC.db... {0}", (int)(tcc_creation_timeout - watch.Elapsed.TotalSeconds));
					await Task.Delay (TimeSpan.FromSeconds (0.250));
				}
			}

			if (File.Exists (TCC_db)) {
				await AgreeToPromptsAsync (log, bundle_identifiers);
			} else {
				log.WriteLine ("No TCC.db found for the simulator {0} (SimRuntime={1} and SimDeviceType={1})", UDID, SimRuntime, SimDeviceType);
			}

			// Make sure we're in a clean state
			await KillEverythingAsync (log);

			// Make 100% sure we're shutdown
			await ShutdownAsync (log);
		}

	}

	public class SimDevicePair
	{
		public string UDID;
		public string Companion;
		public string Gizmo;
	}

	public class Devices
	{
		public Harness Harness;

		public List<Device> ConnectedDevices = new List<Device> ();

		public async Task LoadAsync (Log log)
		{
			if (ConnectedDevices.Count > 0)
				return;

			var tmpfile = Path.GetTempFileName ();
			try {
				using (var process = new Process ()) {
					process.StartInfo.FileName = Harness.MlaunchPath;
					process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listdev={1} --output-format=xml", Harness.XcodeRoot, tmpfile);
					await process.RunAsync (log, false);

					var doc = new XmlDocument ();
					doc.LoadWithoutNetworkAccess (tmpfile);

					foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
						ConnectedDevices.Add (new Device ()
						{
							DeviceIdentifier = dev.SelectSingleNode ("DeviceIdentifier")?.InnerText,
							DeviceClass = dev.SelectSingleNode ("DeviceClass")?.InnerText,
							CompanionIdentifier = dev.SelectSingleNode ("CompanionIdentifier")?.InnerText,
							Name = dev.SelectSingleNode ("Name")?.InnerText,
						});
					}
				}
			} finally {
				File.Delete (tmpfile);
			}
		}
	}

	public class Device
	{
		public string DeviceIdentifier;
		public string DeviceClass;
		public string CompanionIdentifier;
		public string Name;
	}
}

