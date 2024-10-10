#if !TVOS && !WATCH
#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreMidi {

#if !XAMCORE_5_0
#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[NativeName ("MIDICIDeviceIdentification")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiCIDeviceIdentification {
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 3)]
		public byte [] Manufacturer;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
		public byte [] Family;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
		public byte [] ModelNumber;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 4)]
		public byte [] RevisionLevel;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 5)]
		public byte [] Reserved;
	}
#endif

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoWatch]
	[NoTV]
#endif
	[NativeName ("MIDICIDeviceIdentification")]
	[StructLayout (LayoutKind.Sequential)]
#if XAMCORE_5_0
	public struct MidiCIDeviceIdentification {
#elif COREBUILD
	public struct MidiCIDeviceIdentification_Blittable {
#else
	internal struct MidiCIDeviceIdentification_Blittable {
#endif
		byte manufacturer1;
		byte manufacturer2;
		byte manufacturer3;

		byte family1;
		byte family2;

		byte modelnumber1;
		byte modelnumber2;

		byte revisionLevel1;
		byte revisionLevel2;
		byte revisionLevel3;
		byte revisionLevel4;

		byte reserved1;
		byte reserved2;
		byte reserved3;
		byte reserved4;
		byte reserved5;

		public byte [] Manufacturer {
			get { return new byte [] { manufacturer1, manufacturer2, manufacturer3 }; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 3)
					throw new ArgumentOutOfRangeException (nameof (value), "Array length must be exactly 3");
				manufacturer1 = value [0];
				manufacturer2 = value [1];
				manufacturer3 = value [2];
			}
		}

		public byte [] Family {
			get { return new byte [] { family1, family2 }; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 2)
					throw new ArgumentOutOfRangeException (nameof (value), "Array length must be exactly 2");
				family1 = value [0];
				family2 = value [1];
			}
		}

		public byte [] ModelNumber {
			get { return new byte [] { modelnumber1, modelnumber2 }; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 2)
					throw new ArgumentOutOfRangeException (nameof (value), "Array length must be exactly 2");
				modelnumber1 = value [0];
				modelnumber2 = value [1];
			}
		}

		public byte [] RevisionLevel {
			get { return new byte [] { revisionLevel1, revisionLevel2, revisionLevel3, revisionLevel4 }; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 4)
					throw new ArgumentOutOfRangeException (nameof (value), "Array length must be exactly 4");
				revisionLevel1 = value [0];
				revisionLevel2 = value [1];
				revisionLevel3 = value [2];
				revisionLevel4 = value [3];
			}
		}

		public byte [] Reserved {
			get { return new byte [] { reserved1, reserved2, reserved3, reserved4, reserved5 }; }
			set {
				if (value is null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 5)
					throw new ArgumentOutOfRangeException (nameof (value), "Array length must be exactly 5");
				reserved1 = value [0];
				reserved2 = value [1];
				reserved3 = value [2];
				reserved4 = value [3];
				reserved5 = value [4];
			}
		}

#if !XAMCORE_5_0

		internal MidiCIDeviceIdentification ToMidiCIDeviceIdentification ()
		{
			var rv = new MidiCIDeviceIdentification ();
			rv.Manufacturer = Manufacturer;
			rv.Family = Family;
			rv.ModelNumber = ModelNumber;
			rv.RevisionLevel = RevisionLevel;
			rv.Reserved = Reserved;
			return rv;
		}
#endif
	}
}
#endif
