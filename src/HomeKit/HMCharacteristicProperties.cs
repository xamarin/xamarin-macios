#nullable enable

using System;

using ObjCRuntime;
using Foundation;

namespace HomeKit {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos10.0")]
#else
	[iOS (8,0)]
	[TV (10,0)]
#endif
	public class HMCharacteristicProperties {

		public bool SupportsChangeNumber { get; set; }

		public bool SupportsBonjourNotification { get; set; }

		public bool SupportsEventNotification { get; set; }

		public bool Readable { get; set; }

		public bool Writable { get; set; }
	}
}
