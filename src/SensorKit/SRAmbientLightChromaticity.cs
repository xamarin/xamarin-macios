using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace SensorKit {

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (14, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct SRAmbientLightChromaticity {
		public float X;
		public float Y;
	}
}
