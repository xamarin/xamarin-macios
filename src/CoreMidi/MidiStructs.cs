#if !TVOS
#nullable enable

using System.Runtime.InteropServices;

using CoreFoundation;

using MidiObjectRef = System.Int32;
using MidiClientRef = System.Int32;
using MidiDeviceRef = System.Int32;
using MidiDeviceListRef = System.Int32;
using MidiPortRef = System.Int32;
using MidiEndpointRef = System.Int32;
using MidiEntityRef = System.Int32;

namespace CoreMidi {
	/// <summary>Represents a MIDI 2.0 device manufacturer identifier, consisting of a 3-byte SysEx ID.</summary>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	[NativeName ("MIDI2DeviceManufacturer")]
	public struct Midi2DeviceManufacturer {
		// Byte sysExIDByte[3]; // 1-byte SysEx IDs are padded with trailing zeroes
		byte sysExIdByte0;
		byte sysExIdByte1;
		byte sysExIdByte2;

		/// <summary>Gets or sets the 3-byte SysEx manufacturer ID. Single-byte SysEx IDs should be padded with trailing zeroes.</summary>
		/// <returns>A 3-element byte array with the SysEx manufacturer ID bytes.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the array length is not exactly 3.</exception>
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

	/// <summary>Represents a MIDI 2.0 device revision level, consisting of a 4-byte revision level identifier.</summary>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	[NativeName ("MIDI2DeviceRevisionLevel")]
	public struct Midi2DeviceRevisionLevel {
		// Byte revisionLevel[4];
		byte revisionLevel0;
		byte revisionLevel1;
		byte revisionLevel2;
		byte revisionLevel3;

		/// <summary>Gets or sets the 4-byte device revision level.</summary>
		/// <returns>A 4-element byte array with the device revision level.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the array length is not exactly 4.</exception>
		public byte [] RevisionLevel {
			get {
				return new byte [] { revisionLevel0, revisionLevel1, revisionLevel2, revisionLevel3 };
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				if (value.Length != 4)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), "Length must be 4");

				revisionLevel0 = value [0];
				revisionLevel1 = value [1];
				revisionLevel2 = value [2];
				revisionLevel3 = value [3];
			}
		}
	}

	/// <summary>Represents a standard MIDI-CI profile identifier with profile bank, number, version, and level fields.</summary>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	[NativeName ("MIDICIProfileIDStandard")]
	public struct MidiCIProfileIdStandard {
		/// <summary>The first byte of the standard profile identifier.</summary>
		public byte /* MIDIUInteger7 */ ProfileIdByte1;
		/// <summary>The profile bank number.</summary>
		public byte /* MIDIUInteger7 */ ProfileBank;
		/// <summary>The profile number within the bank.</summary>
		public byte /* MIDIUInteger7 */ ProfileNumber;
		/// <summary>The version of the profile.</summary>
		public byte /* MIDIUInteger7 */ ProfileVersion;
		/// <summary>The level of the profile.</summary>
		public byte /* MIDIUInteger7 */ ProfileLevel;
	}

	/// <summary>Represents a manufacturer-specific MIDI-CI profile identifier with SysEx ID and info fields.</summary>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	[NativeName ("MIDICIProfileIDManufacturerSpecific")]
	public struct MidiCIProfileIdManufacturerSpecific {
		/// <summary>The first byte of the manufacturer SysEx ID.</summary>
		public byte /* MIDIUInteger7 */ SysExId1;
		/// <summary>The second byte of the manufacturer SysEx ID.</summary>
		public byte /* MIDIUInteger7 */ SysExId2;
		/// <summary>The third byte of the manufacturer SysEx ID.</summary>
		public byte /* MIDIUInteger7 */ SysExId3;
		/// <summary>The first manufacturer-specific info byte.</summary>
		public byte /* MIDIUInteger7 */ Info1;
		/// <summary>The second manufacturer-specific info byte.</summary>
		public byte /* MIDIUInteger7 */ Info2;
	}

	/// <summary>Represents a MIDI-CI profile identifier, which can be either a standard or manufacturer-specific profile.</summary>
	/// <remarks>This is a union type. Access the <see cref="Standard" /> property for standard profiles or <see cref="ManufacturerSpecific" /> for manufacturer-specific profiles.</remarks>
	[SupportedOSPlatform ("ios18.0")]
	[SupportedOSPlatform ("maccatalyst18.0")]
	[SupportedOSPlatform ("macos15.0")]
	[UnsupportedOSPlatform ("tvos")]
	[NativeName ("MIDICIProfileID")]
	public struct MidiCIProfileId {
		// This is a union between MidiCIProfileIdStandard and MidiCIProfileIdManufacturerSpecific, each with the same size (5 bytes)
		// So declare a struct with 5 bytes, and then do some memory copies to convert to each element of the union.
		byte /* MIDIUInteger7 */ Value0;
		byte /* MIDIUInteger7 */ Value1;
		byte /* MIDIUInteger7 */ Value2;
		byte /* MIDIUInteger7 */ Value3;
		byte /* MIDIUInteger7 */ Value4;

		/// <summary>Gets or sets this profile ID interpreted as a standard MIDI-CI profile identifier.</summary>
		/// <returns>A <see cref="MidiCIProfileIdStandard" /> that represents this profile ID.</returns>
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

		/// <summary>Gets or sets this profile ID interpreted as a manufacturer-specific MIDI-CI profile identifier.</summary>
		/// <returns>A <see cref="MidiCIProfileIdManufacturerSpecific" /> that represents this profile ID.</returns>
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

	/// <summary>A struct that represents a request to transmit a single system-exclusive event.</summary>
	[NativeName ("MIDISysexSendRequest")]
	[StructLayout (LayoutKind.Sequential)]
	struct MidiSysexSendRequest {
		MidiEndpointRef destination;
		IntPtr /* const Byte * */                                                        data;
		uint bytesToSend;
		byte /* Boolean */                                                               complete;
		byte reserved1;
		byte reserved2;
		byte reserved3;
		unsafe delegate* unmanaged<MidiSysexSendRequest*, void> /* MIDICompletionProc */ completionProc;
		IntPtr /* void * __nullable */                                                   completionRefCon;

		/// <summary>The endpoint where the request is sent.</summary>
		public MidiEndpointRef Destination {
			get => destination;
			set => destination = value;
		}

		/// <summary>A pointer to the data to send.</summary>
		/// <remarks>The MIDI system will update this value as the request progresses.</remarks>
		public IntPtr Data {
			get => data;
			set => data = value;
		}

		/// <summary>The number of bytes to send.</summary>
		/// <remarks>The MIDI system will update this value as the request progresses.</remarks>
		public uint BytesToSend {
			get => bytesToSend;
			set => bytesToSend = value;
		}

		/// <summary>The client can set true to immediately stop the request. The MIDI system will set it to true when the request is complete.</summary>
		public bool Complete {
			get => complete != 0;
			set => complete = value.AsByte ();
		}

		/// <summary>The callback that is called when all the data has been sent and the request is complete.</summary>
		/// <remarks>Also called if the client sets <see cref="Complete" /> to true before the request is complete.</remarks>
		public unsafe delegate* unmanaged<MidiSysexSendRequest*, void> CompletionProcedure {
			get => completionProc;
			set => completionProc = value;
		}

		/// <summary>A context value that's passed to the <see cref="CompletionProcedure" /> callback.</summary>
		public IntPtr Context {
			get => completionRefCon;
			set => completionRefCon = value;
		}
	}


	/// <summary>A struct that represents a request to transmit a single UMP system-exclusive event.</summary>
	[NativeName ("MIDISysexSendRequestUMP")]
	[StructLayout (LayoutKind.Sequential)]
	struct MidiSysexSendRequestUmp {
		MidiEndpointRef destination;
		IntPtr /* UInt32* */ words;
		uint /* UInt32 */ wordsToSend;
		byte /* Boolean */ complete;
		unsafe delegate* unmanaged<MidiSysexSendRequestUmp*, void> /* MIDICompletionProcUMP */ completionProc;
		IntPtr /* void* __nullable */ completionRefCon;

		/// <summary>The endpoint where the request is sent.</summary>
		public MidiEndpointRef Destination {
			get => destination;
			set => destination = value;
		}

		/// <summary>A pointer to the 32-bit word(s) to send.</summary>
		/// <remarks>The MIDI system will update this value as the request progresses.</remarks>
		public IntPtr Words {
			get => words;
			set => words = value;
		}

		/// <summary>The number of 32-bit words to send.</summary>
		/// <remarks>The MIDI system will update this value as the request progresses.</remarks>
		public uint WordsToSend {
			get => wordsToSend;
			set => wordsToSend = value;
		}

		/// <summary>The client can set true to immediately stop the request. The MIDI system will set it to true when the request is complete.</summary>
		public bool Complete {
			get => complete != 0;
			set => complete = value.AsByte ();
		}

		/// <summary>The callback that is called when all the data has been sent and the request is complete.</summary>
		/// <remarks>Also called if the client sets <see cref="Complete" /> to true before the request is complete.</remarks>
		public unsafe delegate* unmanaged<MidiSysexSendRequestUmp*, void> CompletionProcedure {
			get => completionProc;
			set => completionProc = value;
		}

		/// <summary>A context value that's passed to the <see cref="CompletionProcedure" /> callback.</summary>
		public IntPtr Context {
			get => completionRefCon;
			set => completionRefCon = value;
		}
	}
}
#endif
