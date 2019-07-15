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
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "GCMicroGamepadSnapshot has been deprecated, use [GCController controllerWithMicroGamepad] instead")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "GCMicroGamepadSnapshot has been deprecated, use [GCController controllerWithMicroGamepad] instead")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "GCMicroGamepadSnapshot has been deprecated, use [GCController controllerWithMicroGamepad] instead")]
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
}
