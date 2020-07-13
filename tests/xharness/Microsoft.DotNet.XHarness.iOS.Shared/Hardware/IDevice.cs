namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {

	public enum Architecture {
		ARMv6,
		ARMv7,
		ARMv7k,
		ARMv7s,
		ARM64,
		ARM64_32,
		i386,
		x86_64,
	}

	public enum DevicePlatform {
		Unknown,
		iOS,
		tvOS,
		watchOS,
		macOS,
	}

	public static class DevicePlatform_Extensions {
		public static string AsString (this DevicePlatform value)
		{
			switch (value) {
			case DevicePlatform.iOS:
				return "iOS";
			case DevicePlatform.tvOS:
				return "tvOS";
			case DevicePlatform.watchOS:
				return "watchOS";
			case DevicePlatform.macOS:
				return "macOS";
			default:
				throw new System.Exception ($"Unknown platform: {value}");
			}
		}
	}

	public interface IDevice {
		string Name { get; }
		string UDID { get; }
		string OSVersion { get; }
	}
}
