//
// This file contains a class used to consume NSFastEnumeration
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2015, Xamarin Inc.
//

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {
	[StructLayout (LayoutKind.Sequential)]
	internal struct NSFastEnumerationState {
		nint state;
		unsafe internal IntPtr* itemsPtr;
		unsafe internal IntPtr* mutationsPtr;
		nint extra1;
		nint extra2;
		nint extra3;
		nint extra4;
		nint extra5;

		// An array where the enumerator might store stuff.
		// This isn't part of the native declaration of NSFastEnumerationState,
		// we've added it to simplify our enumeration code.
		internal const int ArrayLength = 16;
		internal IntPtr array1;
		IntPtr array2;
		IntPtr array3;
		IntPtr array4;
		IntPtr array5;
		IntPtr array6;
		IntPtr array7;
		IntPtr array8;
		IntPtr array9;
		IntPtr array10;
		IntPtr array11;
		IntPtr array12;
		IntPtr array13;
		IntPtr array14;
		IntPtr array15;
		IntPtr array16;
	}
}
