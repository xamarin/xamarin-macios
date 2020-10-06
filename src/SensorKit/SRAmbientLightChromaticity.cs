using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

#nullable enable

namespace SensorKit {

	[NoWatch, NoTV, NoMac]
	[iOS (14,0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct SRAmbientLightChromaticity {
		public float X;
		public float Y;
	}
}
