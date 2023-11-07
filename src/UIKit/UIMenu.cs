#if !WATCH
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace UIKit {
	public partial class UIMenu {
#if !XAMCORE_5_0

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[iOS (15, 0)]
		[TV (15, 0)]
#endif
		public virtual UIMenuElement [] SelectedElements {
			get {
				// check if we are on tvos earlier than 15, if so, return and empty array, else return
				// the correct value
#if TVOS
				if (SystemVersion.ChecktvOS (15, 0)) {
					return _SelectedElements;
				} else {
					return Array.Empty<UIMenuElement> ();
				}
#else
				return _SelectedElements;
#endif
			}
#endif
		}
	}
}
#endif
