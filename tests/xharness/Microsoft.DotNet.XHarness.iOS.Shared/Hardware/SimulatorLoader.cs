using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public class SimulatorLoader : ISimulatorLoader {
		readonly SemaphoreSlim semaphore = new SemaphoreSlim (1);
		readonly BlockingEnumerableCollection<SimRuntime> supported_runtimes = new BlockingEnumerableCollection<SimRuntime> ();
		readonly BlockingEnumerableCollection<SimDeviceType> supported_device_types = new BlockingEnumerableCollection<SimDeviceType> ();
		readonly BlockingEnumerableCollection<SimulatorDevice> available_devices = new BlockingEnumerableCollection<SimulatorDevice> ();
		readonly BlockingEnumerableCollection<SimDevicePair> available_device_pairs = new BlockingEnumerableCollection<SimDevicePair> ();
		readonly IProcessManager processManager;

		bool loaded;

		public IEnumerable<SimRuntime> SupportedRuntimes => supported_runtimes;
		public IEnumerable<SimDeviceType> SupportedDeviceTypes => supported_device_types;
		public IEnumerable<SimulatorDevice> AvailableDevices => available_devices;
		public IEnumerable<SimDevicePair> AvailableDevicePairs => available_device_pairs;

		public SimulatorLoader (IProcessManager processManager)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		public async Task LoadDevices (ILog log, bool includeLocked = false, bool forceRefresh = false, bool listExtraData = false)
		{
			await semaphore.WaitAsync ();
			if (loaded) {
				if (!forceRefresh) {
					semaphore.Release ();
					return;
				}
				supported_runtimes.Reset ();
				supported_device_types.Reset ();
				available_devices.Reset ();
				available_device_pairs.Reset ();
			}

			await Task.Run (async () => {
				var tmpfile = Path.GetTempFileName ();
				try {
					using (var process = new Process ()) {
						var arguments = new MlaunchArguments (
							new ListSimulatorsArgument (tmpfile),
							new XmlOutputFormatArgument ());

						var task = processManager.RunAsync (process, arguments, log, timeout: TimeSpan.FromSeconds (30));
						log.WriteLine ("Launching {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

						var result = await task;

						if (!result.Succeeded)
							throw new Exception ("Failed to list simulators.");

						log.WriteLine ("Result:");
						log.WriteLine (File.ReadAllText (tmpfile));
						var simulator_data = new XmlDocument ();
						simulator_data.LoadWithoutNetworkAccess (tmpfile);
						foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedRuntimes/SimRuntime")) {
							supported_runtimes.Add (new SimRuntime () {
								Name = sim.SelectSingleNode ("Name").InnerText,
								Identifier = sim.SelectSingleNode ("Identifier").InnerText,
								Version = long.Parse (sim.SelectSingleNode ("Version").InnerText),
							});
						}

						foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/SupportedDeviceTypes/SimDeviceType")) {
							supported_device_types.Add (new SimDeviceType () {
								Name = sim.SelectSingleNode ("Name").InnerText,
								Identifier = sim.SelectSingleNode ("Identifier").InnerText,
								ProductFamilyId = sim.SelectSingleNode ("ProductFamilyId").InnerText,
								MinRuntimeVersion = long.Parse (sim.SelectSingleNode ("MinRuntimeVersion").InnerText),
								MaxRuntimeVersion = long.Parse (sim.SelectSingleNode ("MaxRuntimeVersion").InnerText),
								Supports64Bits = bool.Parse (sim.SelectSingleNode ("Supports64Bits").InnerText),
							});
						}

						foreach (XmlNode sim in simulator_data.SelectNodes ("/MTouch/Simulator/AvailableDevices/SimDevice")) {
							available_devices.Add (new SimulatorDevice (processManager, new TCCDatabase (processManager)) {
								Name = sim.Attributes ["Name"].Value,
								UDID = sim.Attributes ["UDID"].Value,
								SimRuntime = sim.SelectSingleNode ("SimRuntime").InnerText,
								SimDeviceType = sim.SelectSingleNode ("SimDeviceType").InnerText,
								DataPath = sim.SelectSingleNode ("DataPath").InnerText,
								LogPath = sim.SelectSingleNode ("LogPath").InnerText,
							});
						}


						var sim_device_pairs = simulator_data.
							SelectNodes ("/MTouch/Simulator/AvailableDevicePairs/SimDevicePair").
							Cast<XmlNode> ().
							// There can be duplicates, so remove those.
							Distinct (new SimulatorXmlNodeComparer ());
						foreach (XmlNode sim in sim_device_pairs) {
							available_device_pairs.Add (new SimDevicePair () {
								UDID = sim.Attributes ["UDID"].Value,
								Companion = sim.SelectSingleNode ("Companion").InnerText,
								Gizmo = sim.SelectSingleNode ("Gizmo").InnerText,
							});
						}
					}

					loaded = true;
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

		// Will return all devices that match the runtime + devicetype (even if a new device was created, any other devices will also be returned)
		async Task<IEnumerable<ISimulatorDevice>> FindOrCreateDevicesAsync (ILog log, string runtime, string devicetype, bool force = false)
		{
			if (runtime == null || devicetype == null)
				return null;

			IEnumerable<ISimulatorDevice> devices = null;

			if (!force) {
				devices = AvailableDevices.Where (v => v.SimRuntime == runtime && v.SimDeviceType == devicetype);
				if (devices.Any ())
					return devices;
			}

			var rv = await processManager.ExecuteXcodeCommandAsync ("simctl", new [] { "create", CreateName (devicetype, runtime), devicetype, runtime }, log, TimeSpan.FromMinutes (1));
			if (!rv.Succeeded) {
				log.WriteLine ($"Could not create device for runtime={runtime} and device type={devicetype}.");
				return null;
			}

			await LoadDevices (log, forceRefresh: true);

			devices = AvailableDevices.Where ((ISimulatorDevice v) => v.SimRuntime == runtime && v.SimDeviceType == devicetype);
			if (!devices.Any ()) {
				log.WriteLine ($"No devices loaded after creating it? runtime={runtime} device type={devicetype}.");
				return null;
			}

			return devices;
		}

		async Task<bool> CreateDevicePair (ILog log, ISimulatorDevice device, ISimulatorDevice companion_device, string runtime, string devicetype, bool create_device)
		{
			if (create_device) {
				// watch device is already paired to some other phone. Create a new watch device
				var matchingDevices = await FindOrCreateDevicesAsync (log, runtime, devicetype, force: true);
				var unPairedDevices = matchingDevices.Where ((v) => !AvailableDevicePairs.Any ((p) => { return p.Gizmo == v.UDID; }));
				if (device != null)                     // If we're creating a new watch device, assume that the one we were given is not usable.
					unPairedDevices = unPairedDevices.Where ((v) => v.UDID != device.UDID);
				if (unPairedDevices?.Any () != true)
					return false;
				device = unPairedDevices.First ();
			}

			log.WriteLine ($"Creating device pair for '{device.Name}' and '{companion_device.Name}'");

			var capturedLog = new StringBuilder ();
			var pairLog = new CallbackLog ((value) => {
				log.Write (value);
				capturedLog.Append (value);
			});
			var rv = await processManager.ExecuteXcodeCommandAsync ("simctl", new [] { "pair", device.UDID, companion_device.UDID }, pairLog, TimeSpan.FromMinutes (1));
			if (!rv.Succeeded) {
				if (!create_device) {
					var try_creating_device = false;
					var captured_log = capturedLog.ToString ();
					try_creating_device |= captured_log.Contains ("At least one of the requested devices is already paired with the maximum number of supported devices and cannot accept another pairing.");
					try_creating_device |= captured_log.Contains ("The selected devices are already paired with each other.");
					if (try_creating_device) {
						log.WriteLine ($"Could not create device pair for '{device.Name}' ({device.UDID}) and '{companion_device.Name}' ({companion_device.UDID}), but will create a new watch device and try again.");
						return await CreateDevicePair (log, device, companion_device, runtime, devicetype, true);
					}
				}

				log.WriteLine ($"Could not create device pair for '{device.Name}' ({device.UDID}) and '{companion_device.Name}' ({companion_device.UDID})");
				return false;
			}
			return true;
		}

		async Task<SimDevicePair> FindOrCreateDevicePairAsync (ILog log, IEnumerable<ISimulatorDevice> devices, IEnumerable<ISimulatorDevice> companion_devices)
		{
			// Check if we already have a device pair with the specified devices
			var pairs = AvailableDevicePairs.Where ((pair) => {
				if (!devices.Any ((v) => v.UDID == pair.Gizmo))
					return false;
				if (!companion_devices.Any ((v) => v.UDID == pair.Companion))
					return false;
				return true;
			});

			if (!pairs.Any ()) {
				// No device pair. Create one.
				// First check if the watch is already paired
				var unPairedDevices = devices.Where ((v) => !AvailableDevicePairs.Any ((p) => { return p.Gizmo == v.UDID; }));
				var unpairedDevice = unPairedDevices.FirstOrDefault ();
				var companion_device = companion_devices.First ();
				var device = devices.First ();
				if (!await CreateDevicePair (log, unpairedDevice, companion_device, device.SimRuntime, device.SimDeviceType, unpairedDevice == null))
					return null;

				await LoadDevices (log, forceRefresh: true);

				pairs = AvailableDevicePairs.Where ((pair) => {
					if (!devices.Any ((v) => v.UDID == pair.Gizmo))
						return false;
					if (!companion_devices.Any ((v) => v.UDID == pair.Companion))
						return false;
					return true;
				});
			}

			return pairs.FirstOrDefault ();
		}

		public async Task<ISimulatorDevice []> FindSimulators (TestTarget target, ILog log, bool create_if_needed = true, bool min_version = false)
		{
			ISimulatorDevice [] simulators = null;

			string simulator_devicetype;
			string simulator_runtime;
			string companion_devicetype = null;
			string companion_runtime = null;

			switch (target) {
			case TestTarget.Simulator_iOS32:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + (min_version ? SdkVersions.MiniOSSimulator : "10-3").Replace ('.', '-');
				break;
			case TestTarget.Simulator_iOS64:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType." + (min_version ? "iPhone-6" : "iPhone-X");
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + (min_version ? SdkVersions.MiniOSSimulator : SdkVersions.MaxiOSSimulator).Replace ('.', '-');
				break;
			case TestTarget.Simulator_iOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.iPhone-5";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + (min_version ? SdkVersions.MiniOSSimulator : SdkVersions.MaxiOSSimulator).Replace ('.', '-');
				break;
			case TestTarget.Simulator_tvOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType.Apple-TV-1080p";
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.tvOS-" + (min_version ? SdkVersions.MinTVOSSimulator : SdkVersions.MaxTVOSSimulator).Replace ('.', '-');
				break;
			case TestTarget.Simulator_watchOS:
				simulator_devicetype = "com.apple.CoreSimulator.SimDeviceType." + (min_version ? "Apple-Watch-38mm" : "Apple-Watch-Series-3-38mm");
				simulator_runtime = "com.apple.CoreSimulator.SimRuntime.watchOS-" + (min_version ? SdkVersions.MinWatchOSSimulator : SdkVersions.MaxWatchOSSimulator).Replace ('.', '-');
				companion_devicetype = "com.apple.CoreSimulator.SimDeviceType." + (min_version ? "iPhone-6" : "iPhone-X");
				companion_runtime = "com.apple.CoreSimulator.SimRuntime.iOS-" + (min_version ? SdkVersions.MinWatchOSCompanionSimulator : SdkVersions.MaxWatchOSCompanionSimulator).Replace ('.', '-');
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
				simulators = new ISimulatorDevice [] { devices.First () };
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

				simulators = new ISimulatorDevice [] {
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

		public ISimulatorDevice FindCompanionDevice (ILog log, ISimulatorDevice device)
		{
			var pair = available_device_pairs.Where ((v) => v.Gizmo == device.UDID).Single ();
			return available_devices.Single ((v) => v.UDID == pair.Companion);
		}

		public IEnumerable<ISimulatorDevice> SelectDevices (TestTarget target, ILog log, bool min_version)
		{
			return new SimulatorEnumerable {
				Simulators = this,
				Target = target,
				MinVersion = min_version,
				Log = log,
			};
		}

		class SimulatorXmlNodeComparer : IEqualityComparer<XmlNode> {
			public bool Equals (XmlNode a, XmlNode b)
			{
				return a ["Gizmo"].InnerText == b ["Gizmo"].InnerText && a ["Companion"].InnerText == b ["Companion"].InnerText;
			}

			public int GetHashCode (XmlNode node)
			{
				return node ["Gizmo"].InnerText.GetHashCode () ^ node ["Companion"].InnerText.GetHashCode ();
			}
		}

		class SimulatorEnumerable : IEnumerable<ISimulatorDevice>, IAsyncEnumerable {
			public SimulatorLoader Simulators;
			public TestTarget Target;
			public bool MinVersion;
			public ILog Log;
			readonly object lock_obj = new object ();
			Task<ISimulatorDevice []> findTask;

			public override string ToString ()
			{
				return $"Simulators for {Target} (MinVersion: {MinVersion})";
			}

			public IEnumerator<ISimulatorDevice> GetEnumerator ()
			{
				return new Enumerator () {
					Enumerable = this,
				};
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return GetEnumerator ();
			}

			Task<ISimulatorDevice []> Find ()
			{
				lock (lock_obj) {
					if (findTask == null)
						findTask = Simulators.FindSimulators (Target, Log, min_version: MinVersion);
					return findTask;
				}
			}

			public Task ReadyTask {
				get { return Find (); }
			}

			class Enumerator : IEnumerator<ISimulatorDevice> {
				internal SimulatorEnumerable Enumerable;
				ISimulatorDevice [] devices;
				bool moved;

				public ISimulatorDevice Current {
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
}
