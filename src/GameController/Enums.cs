//
// Enums.cs
//
// Authors:
//   Chris Hamons (chris.hamons@xamarin.com)
//
// Copyright 2019 Microsoft Corporation

using System;

using ObjCRuntime;
using Foundation;

namespace GameController {
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GCController.GetMicroGamepadController()' instead.")]
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

	[TV (14,0), Mac (11,0), iOS (14,0)]
	[Native]
	public enum GCTouchState : long
	{
		Up,
		Down,
		Moving,
	}

	[TV (14,0), Mac (11,0), iOS (14,0)]
	[Native]
	public enum GCDeviceBatteryState : long
	{
		Unknown = -1,
		Discharging,
		Charging,
		Full,
	}
}
