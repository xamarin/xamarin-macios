using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	partial class HMCharacteristic 
	{
		public bool SupportsEventNotification {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.SupportsEventNotification)
						return true;
				}
				return false;
			}
		}
		
		public bool Readable {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.Readable)
						return true;
				}
				return false;
			}
		}

		public bool Writable {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.Writable)
						return true;
				}
				return false;
			}
		}

#if NET
		[SupportedOSPlatform ("ios9.3")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,3)]
		[Watch (2,2)]
#endif
		public bool Hidden {
			get {
				foreach (var p in Properties) {
					if (p == HMCharacteristicPropertyInternal.Hidden)
						return true;
				}
				return false;
			}
		}
	}
}
