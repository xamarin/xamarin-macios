//
// This file contains a class used to consume NSFastEnumeration
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2015, Xamarin Inc.
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.Foundation {
	[StructLayout (LayoutKind.Sequential)]
	internal struct NSFastEnumerationState {
		nint state;
		internal IntPtr itemsPtr;
		internal IntPtr mutationsPtr;
		nint extra1;
		nint extra2;
		nint extra3;
		nint extra4;
		nint extra5;
	}
}
