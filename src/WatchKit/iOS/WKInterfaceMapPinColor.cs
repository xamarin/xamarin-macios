#if __IOS__
using System;
using System.ComponentModel;

using ObjCRuntime;

namespace WatchKit {
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public enum WKInterfaceMapPinColor : long {
		Red = 0,
		Green = 1,
		Purple = 2,
	}
}
#endif // __IOS__
