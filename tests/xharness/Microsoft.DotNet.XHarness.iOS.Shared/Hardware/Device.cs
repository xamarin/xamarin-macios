using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Hardware {
	public enum DeviceClass {
		Unknown = 0,
		iPhone,
		iPad,
		iPod,
		Watch,
		AppleTV,
	}

	public class Device : IHardwareDevice {
		public string DeviceIdentifier { get; set; }
		public DeviceClass DeviceClass { get; set; }
		public string CompanionIdentifier { get; set; }
		public string Name { get; set; }
		public string BuildVersion { get; set; }
		public string ProductVersion { get; set; }
		public string ProductType { get; set; }
		public string InterfaceType { get; set; }
		public bool? IsUsableForDebugging { get; set; }
		public bool IsLocked { get; set; }

		public string UDID { get { return DeviceIdentifier; } set { DeviceIdentifier = value; } }

		public string OSVersion => ProductVersion;

		// Add a speed property that can be used to sort a list of devices according to speed.
		public int DebugSpeed => InterfaceType?.ToLowerInvariant () switch
		{
			"usb" => 0, // fastest
			null => 1, // mlaunch doesn't know - not sure when this can happen, but wifi is quite slow, so maybe this faster
			"wifi" => 2, // wifi is quite slow
			_ => 3, // Anything else is probably slower than wifi (e.g. watch).
		};

		public DevicePlatform DevicePlatform => DeviceClass switch
		{
			_ when DeviceClass == DeviceClass.iPhone || DeviceClass == DeviceClass.iPod || DeviceClass == DeviceClass.iPad => DevicePlatform.iOS,
			_ when DeviceClass == DeviceClass.AppleTV => DevicePlatform.tvOS,
			_ when DeviceClass == DeviceClass.Watch => DevicePlatform.watchOS,
			_ => DevicePlatform.Unknown,
		};

		public bool Supports64Bit => Architecture == Architecture.ARM64;

		public bool Supports32Bit => DevicePlatform switch
		{
			DevicePlatform.iOS => Version.Parse (ProductVersion).Major < 11,
			DevicePlatform.tvOS => false,
			DevicePlatform.watchOS => true,
			_ => throw new NotImplementedException ()
		};

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
				if (model.StartsWith ("Watch", StringComparison.Ordinal)) {
					var identifier = model.Substring ("Watch".Length);
					var values = identifier.Split (',');
					switch (values [0]) {
					case "1": // Apple Watch (1st gen)
					case "2": // Apple Watch Series 1 and Series 2
					case "3": // Apple Watch Series 3
						return Architecture.ARMv7k;

					case "4": // Apple Watch Series 4
					default:
						return Architecture.ARM64_32;
					}
				}

				// https://www.theiphonewiki.com/wiki/List_of_Apple_TVs
				if (model.StartsWith ("AppleTV", StringComparison.Ordinal))
					return Architecture.ARM64;

				throw new NotImplementedException ();
			}
		}
	}
}
