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
	[Mac (10,15)]
	[StructLayout (LayoutKind.Sequential)]
	public struct CMMagneticField {
		public double X, Y, Z;

		public override string ToString ()
		{
			return string.Format ("({0},{1},{2})", X, Y, Z);
		}
	}
}
