#if !TVOS && !WATCH
//
// MidiServices.cs: Implementation of the MidiObject base class and its derivates
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//
// TODO:
//   * Each MidiObject should be added to a static hashtable so we can always
//     obtain objects that have already been created from the handle, and avoid
//     having two managed objects referencing the same unmanaged object.
//
//     Currently a few lookup functions end up creating objects that might have
//     already been surfaced (new MidiEndpoint (handle) for example)
//
// MIDISendSysex
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using MidiObjectRef = System.Int32;
using MidiClientRef = System.Int32;
using MidiDeviceRef = System.Int32;
using MidiDeviceListRef = System.Int32;
using MidiPortRef = System.Int32;
using MidiEndpointRef = System.Int32;
using MidiEntityRef = System.Int32;

namespace CoreMidi {
#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	[NativeName ("MIDI2DeviceManufacturer")]
	public struct Midi2DeviceManufacturer {
		// Byte sysExIDByte[3]; // 1-byte SysEx IDs are padded with trailing zeroes
		byte sysExIdByte0;
		byte sysExIdByte1;
		byte sysExIdByte2;

		public byte [] SysExIdByte {
			get {
				return new byte [] { sysExIdByte0, sysExIdByte1, sysExIdByte2 };
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				if (value.Length != 3)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), "Length must be 3");

				sysExIdByte0 = value [0];
				sysExIdByte1 = value [1];
				sysExIdByte2 = value [2];
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	[NativeName ("MIDI2DeviceRevisionLevel")]
	public struct Midi2DeviceRevisionLevel {
		// Byte revisionLevel[4];
		byte revisionLevel0;
		byte revisionLevel1;
		byte revisionLevel2;
		byte revisionLevel3;

		public byte [] RevisionLevel {
			get {
				return new byte [] { revisionLevel0, revisionLevel1, revisionLevel2, revisionLevel3 };
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				if (value.Length != 3)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), "Length must be 4");

				revisionLevel0 = value [0];
				revisionLevel1 = value [1];
				revisionLevel2 = value [2];
				revisionLevel3 = value [3];
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	[NativeName ("MIDICIProfileIDStandard")]
	public struct MidiCIProfileIdStandard {
		public byte /* MIDIUInteger7 */ ProfileIDByte1;
		public byte /* MIDIUInteger7 */ ProfileBank;
		public byte /* MIDIUInteger7 */ ProfileNumber;
		public byte /* MIDIUInteger7 */ ProfileVersion;
		public byte /* MIDIUInteger7 */ ProfileLevel;
	}

#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	[NativeName ("MIDICIProfileIDManufacturerSpecific")]
	public struct MidiCIProfileIdManufacturerSpecific {
		public byte /* MIDIUInteger7 */ SysExId1;
		public byte /* MIDIUInteger7 */ SysExId2;
		public byte /* MIDIUInteger7 */ SysExId3;
		public byte /* MIDIUInteger7 */ Info1;
		public byte /* MIDIUInteger7 */ Info2;
	}

#if NET
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[SupportedOSPlatform ("tvos18.0")]
#else
	[Watch (11, 0), TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
	[NativeName ("MIDICIProfileID")]
	public struct MidiCIProfileId {
		// This is a union between MidiCIProfileIdStandard and MidiCIProfileIdManufacturerSpecific, each with the same size (5 bytes)
		// So declare a struct with 5 bytes, and then do some memory copies to convert to each element of the union.
		byte /* MIDIUInteger7 */ Value0;
		byte /* MIDIUInteger7 */ Value1;
		byte /* MIDIUInteger7 */ Value2;
		byte /* MIDIUInteger7 */ Value3;
		byte /* MIDIUInteger7 */ Value4;

		public unsafe MidiCIProfileIdStandard Standard {
			get {
				fixed (MidiCIProfileId* self = &this) {
					return *(MidiCIProfileIdStandard*) self;
				}
			}
			set {
				fixed (MidiCIProfileId* self = &this) {
					*self = *(MidiCIProfileId*) &value;
				}
			}
		}

		public unsafe MidiCIProfileIdManufacturerSpecific ManufacturerSpecific {
			get {
				fixed (MidiCIProfileId* self = &this) {
					return *(MidiCIProfileIdManufacturerSpecific*) self;
				}
			}
			set {
				fixed (MidiCIProfileId* self = &this) {
					*self = *(MidiCIProfileId*) &value;
				}
			}
		}
	}
}
#endif
