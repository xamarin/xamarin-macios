using System.Collections.Generic;
using Xharness;
using Xharness.Collections;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Hardware {
	public interface IHardwareDevice : IDevice {
		string DeviceIdentifier { get; set; }
		string DeviceClass { get; set; }
		string CompanionIdentifier { get; set; }
		string BuildVersion { get; set; }
		string ProductVersion { get; set; }
		string ProductType { get; set; }
		string InterfaceType { get; set; }
		bool? IsUsableForDebugging { get; set; }
		bool IsLocked { get; set; }
		int DebugSpeed { get; }
		DevicePlatform DevicePlatform { get; }
		bool Supports64Bit { get; }
		bool Supports32Bit { get; }
		Architecture Architecture { get; }
		bool IsSupported (iOSTestProject project); // TODO: Move out of this class, since we are looking at a specific case for xamarin-macions
	}

	public interface IDeviceLoader : ILoadAsync {
		IProcessManager ProcessManager { get; set; }
		IEnumerable<IHardwareDevice> ConnectedDevices { get; }
		IEnumerable<IHardwareDevice> Connected64BitIOS { get; }
		IEnumerable<IHardwareDevice> Connected32BitIOS { get; }
		IEnumerable<IHardwareDevice> ConnectedTV { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch32_64 { get; }
		IHardwareDevice FindCompanionDevice (ILog log, IHardwareDevice device);
	}
}
