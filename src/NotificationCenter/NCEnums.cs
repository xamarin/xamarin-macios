using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;

#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.NotificationCenter {

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NCUpdateResult : nuint {
		NewData,
		NoData,
		Failed
	}
}