using System;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;

#if !MONOMAC
using UIKit;
#endif

namespace NotificationCenter {

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NCUpdateResult : ulong {
		NewData,
		NoData,
		Failed
	}

	[iOS (10,0)][NoMac]
	[Native]
	public enum NCWidgetDisplayMode : long {
		Compact,
		Expanded
	}
}