using System;
using System.Runtime.InteropServices;

using Foundation;

namespace AudioUnit {
	[StructLayout (LayoutKind.Sequential)]
	public struct AURecordedParameterEvent {
		public ulong hostTime;
		public ulong address;
		public float value;
	}
}
