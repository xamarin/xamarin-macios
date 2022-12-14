#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreMidi {

#if NET
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
	[Mac (10,14)]
	[iOS (12,0)]
#endif
	[NativeName ("MIDICIDeviceIdentification")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiCIDeviceIdentification {
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] Manufacturer;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] Family;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[] ModelNumber;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] RevisionLevel;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 5)]
		public byte[] Reserved;
	}
}
