#if !WATCH
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace UIKit {
	public partial class UIDeferredMenuElement {
#if !XAMCORE_5_0

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15, 0)]
		[TV (15,0)]
#endif
		public static UIDeferredMenuElement CreateUncached (UIDeferredMenuElementProviderHandler elementProvider)
		{
				// check if we are on tvos earlier than 15, if so, return and empty array, else return
				// the correct value
#if TVOS
				if (SystemVersion.ChecktvOS (15, 0)) {
					return _CreateUncached (elementProvider);
				} else {
					return null;
				}
#else
				return _CreateUncached (elementProvider);
#endif
		}
#endif
	}
}
#endif
