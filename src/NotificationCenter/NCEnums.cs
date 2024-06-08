using System;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;

#if !MONOMAC
using UIKit;
#endif

namespace NotificationCenter {

	/// <summary>Enumerates values that describe what happened after the application developer attempted to change the state of a widget by using the <see cref="M:NotificationCenter.NCWidgetProviding.WidgetPerformUpdate(System.Action{NotificationCenter.NCUpdateResult})" /> method.</summary>
	[Deprecated (PlatformName.iOS, 14, 0)]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
	[Native]
	public enum NCUpdateResult : ulong {
		NewData,
		NoData,
		Failed
	}

	/// <summary>Enumerates widget display modes.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 14, 0)]
	[Native]
	public enum NCWidgetDisplayMode : long {
		Compact,
		Expanded
	}
}
