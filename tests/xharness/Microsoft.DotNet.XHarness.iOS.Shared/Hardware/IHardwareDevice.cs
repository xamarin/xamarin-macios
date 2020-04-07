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
}
