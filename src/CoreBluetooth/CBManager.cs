#if IOS || WATCH
using System;
using ObjCRuntime;

#nullable enable

namespace CoreBluetooth {
	public partial class CBManager {

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (13,0)]
		[Watch (6,0)]
#endif
		public static CBManagerAuthorization Authorization {
			get {
				// in iOS 13.1 / Watch 6.1 this is a static property, like other [tv|mac]OS
#if IOS
				if (SystemVersion.CheckiOS (13, 1)) {
#elif WATCH
				if (SystemVersion.CheckwatchOS (6, 1)) {
#endif
					return _SAuthorization;
				} else {
					// in iOS 13.0 this was, shortly (deprecated in 13.1), an instance property
					return new CBCentralManager ()._IAuthorization;
				}
			}
		}
	}
}
#endif
