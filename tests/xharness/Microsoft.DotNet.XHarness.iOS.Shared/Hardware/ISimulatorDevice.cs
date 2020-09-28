using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

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
		Task Erase (ILog log);
		Task Shutdown (ILog log);
		Task<bool> PrepareSimulator (ILog log, params string [] bundle_identifiers);
		Task KillEverything (ILog log);
	}

	public interface ISimulatorLoader : IDeviceLoader {
		IEnumerable<SimRuntime> SupportedRuntimes { get; }
		IEnumerable<SimDeviceType> SupportedDeviceTypes { get; }
		IEnumerable<SimulatorDevice> AvailableDevices { get; }
		IEnumerable<SimDevicePair> AvailableDevicePairs { get; }
		Task<ISimulatorDevice []> FindSimulators (TestTarget target, ILog log, bool create_if_needed = true, bool min_version = false);
		ISimulatorDevice FindCompanionDevice (ILog log, ISimulatorDevice device);
		IEnumerable<ISimulatorDevice> SelectDevices (TestTarget target, ILog log, bool min_version);
	}
}
