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
	[Native]
	public enum GCExtendedGamepadSnapshotDataVersion : long
	{
		Version1 = 0x0100,
		Version2 = 0x0101,
	}
	
	[Native]
	public enum GCMicroGamepadSnapshotDataVersion : long
	{
		Version1 = 0x0100
	}
}
