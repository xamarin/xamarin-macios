using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {
	public interface IHardwareDevice : IDevice {
		string DeviceIdentifier { get; }
		DeviceClass DeviceClass { get; }
		string CompanionIdentifier { get; }
		string BuildVersion { get; }
		string ProductVersion { get; }
		string ProductType { get; }
		string InterfaceType { get; }
		bool? IsUsableForDebugging { get; }
		bool IsLocked { get; }
		int DebugSpeed { get; }
		DevicePlatform DevicePlatform { get; }
		bool Supports64Bit { get; }
		bool Supports32Bit { get; }
		Architecture Architecture { get; }
	}

	public interface IDeviceLoader : ILoadAsync {
		IEnumerable<IHardwareDevice> ConnectedDevices { get; }
		IEnumerable<IHardwareDevice> Connected64BitIOS { get; }
		IEnumerable<IHardwareDevice> Connected32BitIOS { get; }
		IEnumerable<IHardwareDevice> ConnectedTV { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch { get; }
		IEnumerable<IHardwareDevice> ConnectedWatch32_64 { get; }
		IHardwareDevice FindCompanionDevice (ILog log, IHardwareDevice device);
	}
}
