using System.Collections.Generic;
using System.Threading.Tasks;
using Xharness;
using Xharness.Collections;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Hardware {
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
		public SimDevice Main;
		public SimDevice Companion; // the phone for watch devices
	}

	public interface ISimulatorDevice : IDevice {
		IProcessManager ProcessManager { get; set; }
		ITCCDatabase TCCDatabase { get; set; }
		string SimRuntime { get; set; }
		string SimDeviceType { get; set; }
		string DataPath { get; set; }
		string LogPath { get; set; }
		string SystemLog { get; }
		bool IsWatchSimulator { get; }
		Task EraseAsync (ILog log);
		Task ShutdownAsync (ILog log);
		Task PrepareSimulatorAsync (ILog log, params string [] bundle_identifiers);
	}

	public interface ISimulatorsLoader : ILoadAsync {
		IProcessManager ProcessManager { get; set; }
		IEnumerable<SimRuntime> SupportedRuntimes { get; }
		IEnumerable<SimDeviceType> SupportedDeviceTypes { get; }
		IEnumerable<SimDevice> AvailableDevices { get; }
		IEnumerable<SimDevicePair> AvailableDevicePairs { get; }
		Task<ISimulatorDevice []> FindAsync (AppRunnerTarget target, ILog log, bool create_if_needed = true, bool min_version = false);
		ISimulatorDevice FindCompanionDevice (ILog log, ISimulatorDevice device);
		IEnumerable<ISimulatorDevice> SelectDevices (AppRunnerTarget target, ILog log, bool min_version);
	}

}
