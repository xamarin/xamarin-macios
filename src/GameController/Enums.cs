//
// Enums.cs
//
// Authors:
//   Chris Hamons (chris.hamons@xamarin.com)
//   Whitney Schmidt (whschm@microsoft.com)
//
// Copyright 2019, 2020 Microsoft Corporation

using System;

using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

namespace GameController {
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
#else
	[UnsupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("tvos13.0")]
	[UnsupportedOSPlatform ("macos10.15")]
#if IOS
	[Obsolete ("Starting with ios13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
	[Obsolete ("Starting with tvos13.0 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif MONOMAC
	[Obsolete ("Starting with macos10.15 use 'GCController.GetMicroGamepadController()' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	[Native]
	public enum GCExtendedGamepadSnapshotDataVersion : long
	{
		Version1 = 0x0100,
		Version2 = 0x0101,
	}

	[Native]
	public enum GCMicroGamepadSnapshotDataVersion : long
	{
		Version1 = 0x0100,
	}

#if !NET
	[TV (14,0), Mac (11,0), iOS (14,0)]
#else
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#endif
	[Native]
	public enum GCTouchState : long
	{
		Up,
		Down,
		Moving,
	}

#if !NET
	[TV (14,0), Mac (11,0), iOS (14,0)]
#else
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#endif
	[Native]
	public enum GCDeviceBatteryState : long
	{
		Unknown = -1,
		Discharging,
		Charging,
		Full,
	}

#if !NET
	[TV (14,0), Mac (11,0), iOS (14,0)]
#else
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
#endif
	[Native]
	public enum GCSystemGestureState : long
	{
		Enabled = 0,
		AlwaysReceive,
		Disabled,
	}

#if !NET
	[iOS (9,0)][Mac (10,11)]
#endif
	[Native]
	public enum GCControllerPlayerIndex : long {
		Unset = -1,
		Index1 = 0,
		Index2,
		Index3,
		Index4,
	}
}
