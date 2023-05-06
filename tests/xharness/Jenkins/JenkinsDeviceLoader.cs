using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins {
	class JenkinsDeviceLoader {
		static readonly string devicesName = "Device";
		static readonly string simulatorsName = "Simulator";

		readonly ISimulatorLoader simulators;
		readonly IHardwareDeviceLoader devices;
		public ILog SimulatorLoadLog { get; private set; }
		public ILog DeviceLoadLog { get; private set; }

		public JenkinsDeviceLoader (ISimulatorLoader simulators, IHardwareDeviceLoader devices, ILogs logs)
		{
			if (logs is null)
				throw new ArgumentNullException (nameof (logs));

			this.simulators = simulators ?? throw new ArgumentNullException (nameof (simulators));
			this.devices = devices ?? throw new ArgumentNullException (nameof (devices));
			SimulatorLoadLog = logs.Create ($"simulator-list-{Harness.Helpers.Timestamp}.log", $"Simulator Listing");
			DeviceLoadLog = logs.Create ($"device-list-{Harness.Helpers.Timestamp}.log", $"Device Listing");
		}

		static string BuildDevicesDescription (IHardwareDeviceLoader deviceLoader, string name)
		{
			var devicesTypes = new StringBuilder ();
			if (deviceLoader.Connected32BitIOS.Any ()) {
				devicesTypes.Append ("iOS 32 bit");
			}
			if (deviceLoader.Connected64BitIOS.Any ()) {
				devicesTypes.Append (devicesTypes.Length == 0 ? "iOS 64 bit" : ", iOS 64 bit");
			}
			if (deviceLoader.ConnectedTV.Any ()) {
				devicesTypes.Append (devicesTypes.Length == 0 ? "tvOS" : ", tvOS");
			}
			if (deviceLoader.ConnectedWatch.Any ()) {
				devicesTypes.Append (devicesTypes.Length == 0 ? "watchOS" : ", watchOS");
			}
			return (devicesTypes.Length == 0) ? $"{name} Listing (ok - no devices found)." : $"{name} Listing (ok). Devices types are: {devicesTypes}";
		}

		static string BuildSimulatorsDescription (ISimulatorLoader simulatorLoader, string name)
		{
			var simCount = simulatorLoader.AvailableDevices.Count ();
			return (simCount == 0) ? $"{name} Listing (ok - no simulators found)." : $"{name} Listing (ok - Found {simCount} simulators).";
		}

		Task LoadAsync (ILog log, IDeviceLoader deviceManager, string name)
		{
			log.Description = $"{name} Listing (in progress)";

			var capturedLog = log;
			return deviceManager.LoadDevices (capturedLog, includeLocked: false, forceRefresh: true).ContinueWith ((v) => {
				if (v.IsFaulted) {
					capturedLog.WriteLine ("Failed to load:");
					capturedLog.WriteLine (v.Exception.ToString ());
					capturedLog.Description = $"{name} Listing {v.Exception.Message})";
				} else if (v.IsCompleted) {
					capturedLog.Description = deviceManager switch {
						IHardwareDeviceLoader d => BuildDevicesDescription (d, name),
						ISimulatorLoader s => BuildSimulatorsDescription (s, name),
						_ => throw new NotImplementedException (),
					};
				}
			});
		}

		public Task LoadSimulatorsAsync ()
			=> LoadAsync (SimulatorLoadLog, simulators, simulatorsName);

		public Task LoadDevicesAsync ()
			=> LoadAsync (DeviceLoadLog, devices, devicesName);

		public Task LoadAllAsync ()
			=> Task.WhenAll (LoadDevicesAsync (), LoadSimulatorsAsync ());
	}
}
