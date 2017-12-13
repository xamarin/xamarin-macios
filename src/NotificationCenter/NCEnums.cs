using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

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

	[iOS (10,0)][NoMac]
	[Native]
	public enum NCWidgetDisplayMode : nint {
		Compact,
		Expanded
	}
}