using System;
using System.Runtime.InteropServices;

#if !XAMCORE_2_0
#if MONOMAC
using MonoMac.Foundation;
#else
using MonoTouch.Foundation;
#endif
#else
using Foundation;
#endif

namespace AudioUnit {
	[StructLayout (LayoutKind.Sequential)]
	public struct AURecordedParameterEvent
	{
		public ulong hostTime;
		public ulong address;
		public float value;
	}
}
