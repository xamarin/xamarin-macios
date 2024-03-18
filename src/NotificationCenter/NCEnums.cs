using System;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;

#if !MONOMAC
using UIKit;
#endif

namespace NotificationCenter {

	[Deprecated (PlatformName.iOS, 14, 0)]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
	[Native]
	public enum NCUpdateResult : ulong {
		NewData,
		NoData,
		Failed
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 14, 0)]
	[Native]
	public enum NCWidgetDisplayMode : long {
		Compact,
		Expanded
	}
}
