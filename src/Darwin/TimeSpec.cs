using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace Darwin {
	[StructLayout (LayoutKind.Sequential)]
	[NativeName ("timespec")]
	public struct TimeSpec {
		public nint Seconds;
		public nint NanoSeconds;
	}
}
