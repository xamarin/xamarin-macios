using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Utils;

namespace xharness
{
	public class Simulators
	{
		public Harness Harness;

		bool loaded;
		SemaphoreSlim semaphore = new SemaphoreSlim (1);

		BlockingEnumerableCollection<SimRuntime> supported_runtimes = new BlockingEnumerableCollection<SimRuntime> ();
		BlockingEnumerableCollection<SimDeviceType> supported_device_types = new BlockingEnumerableCollection<SimDeviceType> ();
		BlockingEnumerableCollection<SimDevice> available_devices = new BlockingEnumerableCollection<SimDevice> ();
		BlockingEnumerableCollection<SimDevicePair> available_device_pairs = new BlockingEnumerableCollection<SimDevicePair> ();

		public IEnumerable<SimRuntime> SupportedRuntimes => supported_runtimes;
		public IEnumerable<SimDeviceType> SupportedDeviceTypes => supported_device_types;
		public IEnumerable<SimDevice> AvailableDevices => available_devices;
		public IEnumerable<SimDevicePair> AvailableDevicePairs => available_device_pairs;

		public async Task LoadAsync (Log log, bool force = false)
		{
			await semaphore.WaitAsync ();
			if (loaded) {
				if (!force) {
					semaphore.Release ();
					return;
				}
				supported_runtimes.Reset ();
				supported_device_types.Reset ();
				available_devices.Reset ();
				available_device_pairs.Reset ();
			}
			loaded = true;

			await Task.Run (async () =>
			{
				var tmpfile = Path.GetTempFileName ();
				try {
					using (var process = new Process ()) {
						process.StartInfo.FileName = Harness.MlaunchPath;
						process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listsim {1}", Harness.XcodeRoot, tmpfile);
						log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
						var rv = await process.RunAsync (log, false, timeout: TimeSpan.FromSeconds (30));
						if (!rv.Succeeded)
							throw new Exception ("Failed to list simulators.");
						log.WriteLine ("Result:");
						log.WriteLine (File.ReadAllText (tmpfile));
						var simulator_data = new XmlDocument ();
						simulator_data.LoadWithoutNetworkAccess (tmpfile);
						foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedRuntimes/SimRuntime")) {
							supported_runtimes.Add (new SimRuntime ()
							{
								Name = sim.SelectSingleNode ("Name").InnerText,
								Identifier = sim.SelectSingleNode ("Identifier").InnerText,
								Version = long.Parse (sim.SelectSingleNode ("Version").InnerText),
							});
						}

						foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType")) {
							supported_device_types.Add (new SimDeviceType ()
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
							available_devices.Add (new SimDevice ()
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
							available_device_pairs.Add (new SimDevicePair ()
							{
								UDID = sim.Attributes ["UDID"].Value,
								Companion = sim.SelectSingleNode ("Companion").InnerText,
								Gizmo = sim.SelectSingleNode ("Gizmo").InnerText,

							});
						}
					}
				} finally {
					supported_runtimes.SetCompleted ();
					supported_device_types.SetCompleted ();
					available_devices.SetCompleted ();
					available_device_pairs.SetCompleted ();
					File.Delete (tmpfile);
					semaphore.Release ();
				}
			});
		}

		string CreateName (string devicetype, string runtime)
		{
			var runtime_name = supported_runtimes?.Where ((v) => v.Identifier == runtime).FirstOrDefault ()?.Name ?? Path.GetExtension (runtime).Substring (1);
			var device_name = supported_device_types?.Where ((v) => v.Identifier == devicetype).FirstOrDefault ()?.Name ?? Path.GetExtension (devicetype).Substring (1);
			return $"{device_name} ({runtime_name}) - created by xharness";
		}

		async Task<IEnumerable<SimDevice>> FindOrCreateDevicesAsync (Log log, string runtime, string devicetype)
		{
			if (runtime == null || devicetype == null)
				return null;
			
			var devices = AvailableDevices.Where ((SimDevice v) => v.SimRuntime == runtime && v.SimDeviceType == devicetype);
			if (devices.Any ())
				return devices;
			
			var rv = await Harness.ExecuteXcodeCommandAsync ("simctl", $"create {StringUtils.Quote (CreateName (devicetype, runtime))} {devicetype} {runtime}", log, TimeSpan.FromMinutes (1));
			if (!rv.Succeeded) {
				log.WriteLine ($"Could not create device for runtime={runtime} and device type={devicetype}.");
				return null;
			}

			await LoadAsync (log, force: true);

			devices = AvailableDevices.Where ((SimDevice v) => v.SimRuntime == runtime && v.SimDeviceType == devicetype);
			if (!devices.Any ()) {
				log.WriteLine ($"No devices loaded after creating it? runtime={runtime} device type={devicetype}.");
				return null;
			}

			return devices;
		}

		async Task<SimDevicePair> FindOrCreateDevicePairAsync (Log log, IEnumerable<SimDevice> devices, IEnumerable<SimDevice> companion_devices)
		{
			// Check if we already have a device pair with the specified devices
			var pairs = AvailableDevicePairs.Where ((SimDevicePair pair) => {
				if (!devices.Any ((v) => v.UDID == pair.Gizmo))
					return false;
				if (!companion_devices.Any ((v) => v.UDID == pair.Companion))
					return false;
				return true;
			});

			if (!pairs.Any ()) {
				// No device pair. Create one.
				var device = devices.First ();
				var companion_device = companion_devices.First ();
				log.WriteLine ($"Creating device pair for '{device.Name}' and '{companion_device.Name}'");
				var rv = await Harness.ExecuteXcodeCommandAsync ("simctl", $"pair {device.UDID} {companion_device.UDID}", log, TimeSpan.FromMinutes (1));
				if (!rv.Succeeded) {
					log.WriteLine ($"Could not create device pair for '{device.Name}' ({device.UDID}) and '{companion_device.Name}' ({companion_device.UDID})");
					return null;
				}
				await LoadAsync (log, force: true);

				pairs = AvailableDevicePairs.Where ((SimDevicePair pair) => {
					if (!devices.Any ((v) => v.UDID == pair.Gizmo))
						return false;
					if (!companion_devices.Any ((v) => v.UDID == pair.Companion))
						return false;
					return true;
				});
			}

			return pairs.FirstOrDefault ();
		}

		public async Task<SimDevice []> FindAsync (AppRunnerTarget target, Log log, bool create_if_needed = true)
		{
			SimDevice [] simulators = null;

			string simulator_devicetype;
			string simulator_runtime;
			string companion_devicetype = null;
			string companion_runtime = null;

			switch (target) {
			case AppRunnerTarget.Simulator_iOS32:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-10-3";
				break;
			case AppRunnerTarget.Simulator_iOS64:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-X";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_iOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_tvOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.tvOS-" + Xamarin.SdkVersions.TVOS.Replace ('.', '-');
				break;
			case AppRunnerTarget.Simulator_watchOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-Watch-Series-3-38mm";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.watchOS-" + Xamarin.SdkVersions.WatchOS.Replace ('.', '-');
				companion_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-X";
				companion_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + Xamarin.SdkVersions.iOS.Replace ('.', '-');
				break;
			default:
				throw new Exception (string.Format ("Unknown simulator target: {0}", target));
			}

			var devices = await FindOrCreateDevicesAsync (log, simulator_runtime, simulator_devicetype);
			var companion_devices = await FindOrCreateDevicesAsync (log, companion_runtime, companion_devicetype);

			if (devices?.Any () != true) {
				log.WriteLine ($"Could not find or create devices runtime={simulator_runtime} and device type={simulator_devicetype}.");
				return null;
			}

			if (companion_runtime == null) {
				simulators = new SimDevice [] { devices.First () };
			} else {
				if (companion_devices?.Any () != true) {
					log.WriteLine ($"Could not find or create companion devices runtime={companion_runtime} and device type={companion_devicetype}.");
					return null;
				}

				var pair = await FindOrCreateDevicePairAsync (log, devices, companion_devices);
				if (pair == null) {
					log.WriteLine ($"Could not find or create device pair runtime={companion_runtime} and device type={companion_devicetype}.");
					return null;
				}

				simulators = new SimDevice [] {
					devices.First ((v) => v.UDID == pair.Gizmo), 
					companion_devices.First ((v) => v.UDID == pair.Companion),
				};
			}

			if (simulators == null) {
				log.WriteLine ($"Could not find simulator for runtime={simulator_runtime} and device type={simulator_devicetype}.");
				return null;
			}

			log.WriteLine ("Found simulator: {0} {1}", simulators [0].Name, simulators [0].UDID);
			if (simulators.Length > 1)
				log.WriteLine ("Found companion simulator: {0} {1}", simulators [1].Name, simulators [1].UDID);

			return simulators;
		}

		public SimDevice FindCompanionDevice (Log log, SimDevice device)
		{
			var pair = available_device_pairs.Where ((v) => v.Gizmo == device.UDID).Single ();
			return available_devices.Single ((v) => v.UDID == pair.Companion);
		}

		public IEnumerable<SimDevice> SelectDevices (AppRunnerTarget target, Log log)
		{
			return new SimulatorEnumerable
			{
				Simulators = this,
				Target = target,
				Log = log,
			};
		}

		class SimulatorEnumerable : IEnumerable<SimDevice>, IAsyncEnumerable
		{
			public Simulators Simulators;
			public AppRunnerTarget Target;
			public Log Log;
			object lock_obj = new object ();
			Task<SimDevice []> findTask;

			public IEnumerator<SimDevice> GetEnumerator ()
			{
				return new Enumerator ()
				{
					Enumerable = this,
				};
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return GetEnumerator ();
			}

			Task<SimDevice []> Find ()
			{
				lock (lock_obj) {
					if (findTask == null)
						findTask = Simulators.FindAsync (Target, Log);
					return findTask;
				}
			}

			public Task ReadyTask {
				get { return Find (); }
			}

			class Enumerator : IEnumerator<SimDevice>
			{
				internal SimulatorEnumerable Enumerable;
				SimDevice [] devices;
				bool moved;

				public SimDevice Current {
					get {
						return devices [0];
					}
				}

				object IEnumerator.Current {
					get {
						return Current;
					}
				}

				public void Dispose ()
				{
				}

				public bool MoveNext ()
				{
					if (moved)
						return false;
					if (devices == null)
						devices = Enumerable.Find ()?.Result?.ToArray (); // Create a copy of the list of devices, so we can have enumerator-specific current index.
					moved = true;
					return devices?.Length > 0;
				}

				public void Reset ()
				{
					moved = false;
				}
			}
		}
	}

	public interface IAsyncEnumerable
	{
		Task ReadyTask { get; }
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

	public class SimDevice : IDevice
	{
		public string UDID { get; set; }
		public string Name { get; set; }
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

			var to_kill = new string [] { "iPhone Simulator", "iOS Simulator", "Simulator", "Simulator (Watch)", "com.apple.CoreSimulator.CoreSimulatorService", "ibtoold" };

			await ProcessHelper.ExecuteCommandAsync ("killall", "-9 " + string.Join (" ", to_kill.Select ((v) => StringUtils.Quote (v)).ToArray ()), log, TimeSpan.FromSeconds (10));

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
			var tcc_edit_timeout = 30;
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
					sql.Append (StringUtils.Quote (TCC_db));
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

			if (IsWatchSimulator && Harness.XcodeVersion.Major < 9) {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator (Watch).app");
			} else {
				simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "Simulator.app");
				if (!Directory.Exists (simulator_app))
					simulator_app = Path.Combine (Harness.XcodeRoot, "Contents", "Developer", "Applications", "iOS Simulator.app");
			}

			await ProcessHelper.ExecuteCommandAsync ("open", "-a " + StringUtils.Quote (simulator_app) + " --args -CurrentDeviceUDID " + UDID, log, TimeSpan.FromSeconds (15));
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

	public class SimDeviceSpecification
	{
		public SimDevice Main;
		public SimDevice Companion; // the phone for watch devices
	}

	public class Devices
	{
		public Harness Harness;

		bool loaded;

		BlockingEnumerableCollection<Device> connected_devices = new BlockingEnumerableCollection<Device> ();
		public IEnumerable<Device> ConnectedDevices {
			get {
				return connected_devices;
			}
		}

		public async Task LoadAsync (Log log, bool extra_data = false, bool removed_locked = false, bool force = false)
		{
			if (loaded) {
				if (!force)
					return;
				connected_devices.Reset ();
			}

			loaded = true;

			await Task.Run (async () =>
			{
				var tmpfile = Path.GetTempFileName ();
				try {
					using (var process = new Process ()) {
						process.StartInfo.FileName = Harness.MlaunchPath;
						process.StartInfo.Arguments = string.Format ("--sdkroot {0} --listdev={1} {2} --output-format=xml", Harness.XcodeRoot, tmpfile, extra_data ? "--list-extra-data" : string.Empty);
						log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
						var rv = await process.RunAsync (log, false, timeout: TimeSpan.FromSeconds (120));
						if (!rv.Succeeded)
							throw new Exception ("Failed to list devices.");
						log.WriteLine ("Result:");
						log.WriteLine (File.ReadAllText (tmpfile));

						var doc = new XmlDocument ();
						doc.LoadWithoutNetworkAccess (tmpfile);

						foreach (XmlNode dev in doc.SelectNodes ("/MTouch/Device")) {
							var usable = dev.SelectSingleNode ("IsUsableForDebugging")?.InnerText;
							Device d = new Device
							{
								DeviceIdentifier = dev.SelectSingleNode ("DeviceIdentifier")?.InnerText,
								DeviceClass = dev.SelectSingleNode ("DeviceClass")?.InnerText,
								CompanionIdentifier = dev.SelectSingleNode ("CompanionIdentifier")?.InnerText,
								Name = dev.SelectSingleNode ("Name")?.InnerText,
								BuildVersion = dev.SelectSingleNode ("BuildVersion")?.InnerText,
								ProductVersion = dev.SelectSingleNode ("ProductVersion")?.InnerText,
								ProductType = dev.SelectSingleNode ("ProductType")?.InnerText,
								InterfaceType = dev.SelectSingleNode ("InterfaceType")?.InnerText,
								IsUsableForDebugging = usable == null ? (bool?) null : ((bool?) (usable == "True")),
							};
							bool.TryParse (dev.SelectSingleNode ("IsLocked")?.InnerText, out d.IsLocked);
							if (removed_locked && d.IsLocked) {
								log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's locked.");
								continue;
							}
							if (d.IsUsableForDebugging.HasValue && !d.IsUsableForDebugging.Value) {
								log.WriteLine ($"Skipping device {d.Name} ({d.DeviceIdentifier}) because it's not usable for debugging.");
								continue;
							}
							connected_devices.Add (d);
						}
					}
				} finally {
					connected_devices.SetCompleted ();
					File.Delete (tmpfile);
				}
			});
		}

		public Device FindCompanionDevice (Log log, Device device)
		{
			var companion = ConnectedDevices.Where ((v) => v.DeviceIdentifier == device.CompanionIdentifier);
			if (companion.Count () == 0)
				throw new Exception ($"Could not find the companion device for '{device.Name}'");

			if (companion.Count () > 1)
				log.WriteLine ("Found {0} companion devices for {1}?!?", companion.Count (), device.Name);

			return companion.First ();
		}
	}

	public enum Architecture
	{
		ARMv6,
		ARMv7,
		ARMv7k,
		ARMv7s,
		ARM64,
		i386,
		x86_64,
	}

	public enum DevicePlatform
	{
		Unknown,
		iOS,
		tvOS,
		watchOS,
	}

	public class Device : IDevice
	{
		public string DeviceIdentifier;
		public string DeviceClass;
		public string CompanionIdentifier;
		public string Name { get; set; }
		public string BuildVersion;
		public string ProductVersion;
		public string ProductType;
		public string InterfaceType;
		public bool? IsUsableForDebugging;
		public bool IsLocked;

		public string UDID { get { return DeviceIdentifier; } set { DeviceIdentifier = value; } }

		// Add a speed property that can be used to sort a list of devices according to speed.
		public int DebugSpeed {
			get {
				var itype = InterfaceType?.ToLowerInvariant ();
				if (itype == "usb")
					return 0; // fastest

				if (itype == null)
					return 1; // mlaunch doesn't know - not sure when this can happen, but wifi is quite slow, so maybe this faster

				if (itype == "wifi")
					return 2; // wifi is quite slow

				return 3; // Anything else is probably slower than wifi (e.g. watch).
			}
		}

		public DevicePlatform DevicePlatform {
			get {
				switch (DeviceClass) {
				case "iPhone":
				case "iPod":
				case "iPad":
					return DevicePlatform.iOS;
				case "AppleTV":
					return DevicePlatform.tvOS;
				case "Watch":
					return DevicePlatform.watchOS;
				default:
					return DevicePlatform.Unknown;
				}
			}
		}
		
		public bool Supports64Bit {
			get { return Architecture == Architecture.ARM64; }
		}

		public bool Supports32Bit {
			get {
				switch (DevicePlatform) {
				case DevicePlatform.iOS:
					return Version.Parse (ProductVersion).Major < 11;
				case DevicePlatform.tvOS:
					return false;
				case DevicePlatform.watchOS:
					return true;
				default:
					throw new NotImplementedException ();
				}
			}
		}

		public Architecture Architecture {
			get {
				var model = ProductType;

				// https://www.theiphonewiki.com/wiki/Models
				if (model.StartsWith ("iPhone", StringComparison.Ordinal)) {
					var identifier = model.Substring ("iPhone".Length);
					var values = identifier.Split (',');

					switch (values [0]) {
					case "1": // iPhone (1) and iPhone 3G (2)
						return Architecture.ARMv6;
					case "2": // iPhone 3GS (1)
					case "3": // iPhone 4 (1-3)
					case "4": // iPhone 4S (1)
						return Architecture.ARMv7;
					case "5": // iPhone 5 (1-2) and iPhone 5c (3-4)
						return Architecture.ARMv7s;
					case "6": // iPhone 5s (1-2)
					case "7": // iPhone 6+ (1) and iPhone 6 (2)
					case "8": // iPhone 6s (1), iPhone 6s+ (2), iPhoneSE (4)
					case "9": // iPhone 7 (1,3) and iPhone 7+ (2,4)
					default:
						return Architecture.ARM64;
					}
				}

				// https://www.theiphonewiki.com/wiki/List_of_iPads
				if (model.StartsWith ("iPad", StringComparison.Ordinal)) {
					var identifier = model.Substring ("iPad".Length);
					var values = identifier.Split (',');

					switch (values [0]) {
					case "1": // iPad (1)
					case "2": // iPad 2 (1-4) and iPad Mini (5-7)
					case "3": // iPad 3 (1-3) and iPad 4 (4-6)
						return Architecture.ARMv7;
					case "4": // iPad Air (1-3), iPad Mini 2 (4-6) and iPad Mini 3 (7-9)
					case "5": // iPad Air 2 (3-4)
					case "6": // iPad Pro 9.7-inch (3-4), iPad Pro 12.9-inch (7-8)
					default:
						return Architecture.ARM64;
					}
				}

				// https://www.theiphonewiki.com/wiki/List_of_iPod_touches
				if (model.StartsWith ("iPod", StringComparison.Ordinal)) {
					var identifier = model.Substring ("iPod".Length);
					var values = identifier.Split (',');

					switch (values [0]) {
					case "1": // iPod touch (1)
					case "2": // iPod touch 2G (1)
						return Architecture.ARMv6;
					case "3": // iPod touch 3G (1)
					case "4": // iPod touch 4G (1)
					case "5": // iPod touch 5G (1)
						return Architecture.ARMv7;
					case "7": // iPod touch 6G (1)
					default:
						return Architecture.ARM64;
					}
				}

				// https://www.theiphonewiki.com/wiki/List_of_Apple_Watches
				if (model.StartsWith ("Watch", StringComparison.Ordinal))
					return Architecture.ARMv7k;

				// https://www.theiphonewiki.com/wiki/List_of_Apple_TVs
				if (model.StartsWith ("AppleTV", StringComparison.Ordinal))
					return Architecture.ARM64;

				throw new NotImplementedException ();
			}
		}
	}

	interface IDevice
	{
		string Name { get; set; }
		string UDID { get; set; }
	}


	// This is a collection whose enumerator will wait enumerating until 
	// the collection has been marked as completed (but the enumerator can still
	// be created; this allows the creation of linq queries whose execution is
	// delayed until later).
	internal class BlockingEnumerableCollection<T> : IEnumerable<T> where T : class
	{
		List<T> list = new List<T> ();
		TaskCompletionSource<bool> completed = new TaskCompletionSource<bool> ();

		public int Count {
			get {
				WaitForCompletion ();
				return list.Count;
			}
		}

		public void Add (T device)
		{
			if (completed.Task.IsCompleted)
				Console.WriteLine ("Adding to completed collection!");
			list.Add (device);
		}

		public void SetCompleted ()
		{
			completed.TrySetResult (true);
		}

		void WaitForCompletion ()
		{
			completed.Task.Wait ();
		}

		public void Reset ()
		{
			completed = new TaskCompletionSource<bool> ();
			list.Clear ();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return new Enumerator (this);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		class Enumerator : IEnumerator<T>
		{
			BlockingEnumerableCollection<T> collection;
			IEnumerator<T> enumerator;

			public Enumerator (BlockingEnumerableCollection<T> collection)
			{
				this.collection = collection;
			}

			public T Current {
				get {
					return enumerator.Current;
				}
			}

			object IEnumerator.Current {
				get {
					return enumerator.Current;
				}
			}

			public void Dispose ()
			{
				enumerator.Dispose ();
			}

			public bool MoveNext ()
			{
				collection.WaitForCompletion ();
				if (enumerator == null)
					enumerator = collection.list.GetEnumerator ();
				return enumerator.MoveNext ();
			}

			public void Reset ()
			{
				enumerator.Reset ();
			}
		}
	}
}

