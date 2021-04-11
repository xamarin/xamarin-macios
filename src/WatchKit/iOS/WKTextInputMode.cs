#if __IOS__
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace WatchKit {
#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
#endif
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public enum WKTextInputMode : long {
		Plain = 0,
		AllowEmoji = 1,
		AllowAnimatedEmoji = 2,
	}
}
#endif // __IOS__
