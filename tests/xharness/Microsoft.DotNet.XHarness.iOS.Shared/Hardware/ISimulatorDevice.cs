using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {
	public class SimRuntime {
		public string Name;
		public string Identifier;
		public long Version;
	}

	public class SimDeviceType {
		public string Name;
		public string Identifier;
		public string ProductFamilyId;
		public long MinRuntimeVersion;
		public long MaxRuntimeVersion;
		public bool Supports64Bits;
	}

	public class SimDevicePair {
		public string UDID;
		public string Companion;
		public string Gizmo;
	}

	public class SimDeviceSpecification {
		public SimulatorDevice Main;
		public SimulatorDevice Companion; // the phone for watch devices
	}

	public interface ISimulatorDevice : IDevice {
		string SimRuntime { get; set; }
		string SimDeviceType { get; set; }
		string DataPath { get; set; }
		string LogPath { get; set; }
		string SystemLog { get; }
		bool IsWatchSimulator { get; }
		Task EraseAsync (ILog log);
		Task ShutdownAsync (ILog log);
		Task PrepareSimulatorAsync (ILog log, params string [] bundle_identifiers);
		Task KillEverythingAsync (ILog log);
	}

	public interface ISimulatorsLoader : ILoadAsync {
		IEnumerable<SimRuntime> SupportedRuntimes { get; }
		IEnumerable<SimDeviceType> SupportedDeviceTypes { get; }
		IEnumerable<SimulatorDevice> AvailableDevices { get; }
		IEnumerable<SimDevicePair> AvailableDevicePairs { get; }
		Task<ISimulatorDevice []> FindAsync (TestTarget target, ILog log, bool create_if_needed = true, bool min_version = false);
		ISimulatorDevice FindCompanionDevice (ILog log, ISimulatorDevice device);
		IEnumerable<ISimulatorDevice> SelectDevices (TestTarget target, ILog log, bool min_version);
	}

}
