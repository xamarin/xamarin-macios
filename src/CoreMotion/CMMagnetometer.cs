//
// CMMagnetometer.cs: Support classes
//
// Copyright 2011-2014, Xamarin Inc.
//
// Authors:
//   Miguel de Icaza 
//
using System.Runtime.InteropServices;

namespace CoreMotion {

	// CMMagnetometer.h
#if NET
	[SupportedOSPlatform ("macos10.15")]
#else
	[Mac (10,15)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CMMagneticField {
		public double X, Y, Z;

		public override string ToString ()
		{
			return string.Format ("({0},{1},{2})", X, Y, Z);
		}
	}
}
