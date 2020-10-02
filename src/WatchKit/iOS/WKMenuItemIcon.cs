#if __IOS__
using System;
using System.ComponentModel;

using ObjCRuntime;

namespace WatchKit {
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public enum WKMenuItemIcon : long {
		Accept = 0,
		Add = 1,
		Block = 2,
		Decline = 3,
		Info = 4,
		Maybe = 5,
		More = 6,
		Mute = 7,
		Pause = 8,
		Play = 9,
		Repeat = 10,
		Resume = 11,
		Share = 12,
		Shuffle = 13,
		Speaker = 14,
		Trash = 15,
	}
}
#endif // __IOS__
