using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;

#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.NotificationCenter {

	[iOS (8,0)]
	[Native]
	public enum NCUpdateResult : nuint {
		NewData,
		NoData,
		Failed
	}
}