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

		public static Task KillEverythingAsync (Log log)
		{
			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService" };

			return ProcessHelper.ExecuteCommandAsync ("killall", "-9 " + string.Join (" ", to_kill.Select ((v) => Harness.Quote (v)).ToArray ()), log, TimeSpan.FromSeconds (10));
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
					if (!rv.Succeeded)
						failure = true;
					if (failure) {
						if (watch.Elapsed.TotalSeconds > tcc_edit_timeout)
							break;
						log.WriteLine ("Failed to edit TCC.db, trying again in 1 second... ", (int) (tcc_edit_timeout - watch.Elapsed.TotalSeconds));
						await Task.Delay (TimeSpan.FromSeconds (1));
					}
				}
			} while (failure);

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

