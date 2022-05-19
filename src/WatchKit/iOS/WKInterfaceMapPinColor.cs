#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace WatchKit {
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public enum WKInterfaceMapPinColor : long {
		Red = 0,
		Green = 1,
		Purple = 2,
	}
}
#endif // __IOS__ && !NET
