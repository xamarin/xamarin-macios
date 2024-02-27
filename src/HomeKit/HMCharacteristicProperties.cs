#nullable enable

using System;

using ObjCRuntime;
using Foundation;

namespace HomeKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class HMCharacteristicProperties {

		public bool SupportsChangeNumber { get; set; }

		public bool SupportsBonjourNotification { get; set; }

		public bool SupportsEventNotification { get; set; }

		public bool Readable { get; set; }

		public bool Writable { get; set; }
	}
}
