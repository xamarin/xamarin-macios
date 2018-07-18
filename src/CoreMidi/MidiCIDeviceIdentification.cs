using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreMidi {

	[NoWatch, NoTV, Mac (10,14, onlyOn64: true), iOS (12,0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiCIDeviceIdentification {
		public byte[] Manufacturer;
		public byte[] Family;
		public byte[] ModelNumber;
		public byte[] RevisionLevel;
		public byte[] Reserved;
	}
}