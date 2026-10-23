// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#nullable enable

namespace CoreMidi {
	/// <summary>A representation of all possible messages stored in a Universal MIDI packet (UMP).</summary>
	/// <remarks>
	///   <para>This is the managed representation of the native <c>MIDIUniversalMessage</c> struct. The active variant is determined by the <see cref="Type" /> property: only the union member matching the message type contains valid data.</para>
	/// </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiUniversalMessage {
		MidiMessageType type;
		byte group;
		byte reserved0;
		byte reserved1;
		byte reserved2;
		// 20 bytes of union storage, starting at offset 8.
		uint storage0;
		uint storage1;
		uint storage2;
		uint storage3;
		uint storage4;

		/// <summary>The message type. Determines which variant in the union is valid.</summary>
		public MidiMessageType Type => type;

		/// <summary>The 4-bit MIDI group this message belongs to.</summary>
		public byte Group => group;

		/// <summary>The utility message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.Utility" />.</summary>
		public MidiUniversalMessageUtility Utility {
			get {
				var self = this;
				return *(MidiUniversalMessageUtility*) ((byte*) &self + 8);
			}
		}

		/// <summary>The system message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.System" />.</summary>
		public MidiUniversalMessageSystem System {
			get {
				var self = this;
				return *(MidiUniversalMessageSystem*) ((byte*) &self + 8);
			}
		}

		/// <summary>The MIDI 1.0 channel voice message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.ChannelVoice1" />.</summary>
		public MidiUniversalMessageChannelVoice1 ChannelVoice1 {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice1*) ((byte*) &self + 8);
			}
		}

		/// <summary>The system exclusive (SysEx) message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.SysEx" />.</summary>
		public MidiUniversalMessageSysEx SysEx {
			get {
				var self = this;
				return *(MidiUniversalMessageSysEx*) ((byte*) &self + 8);
			}
		}

		/// <summary>The MIDI 2.0 channel voice message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.ChannelVoice2" />.</summary>
		public MidiUniversalMessageChannelVoice2 ChannelVoice2 {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2*) ((byte*) &self + 8);
			}
		}

		/// <summary>The 128-bit data message data. Valid when <see cref="Type" /> is <see cref="MidiMessageType.Data128" />.</summary>
		public MidiUniversalMessageData128 Data128 {
			get {
				var self = this;
				return *(MidiUniversalMessageData128*) ((byte*) &self + 8);
			}
		}

		/// <summary>The raw words of an unknown message. Valid when <see cref="Type" /> is not a recognized message type.</summary>
		public MidiUniversalMessageUnknown Unknown {
			get {
				var self = this;
				return *(MidiUniversalMessageUnknown*) ((byte*) &self + 8);
			}
		}
	}

	/// <summary>A utility message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageUtility {
		MidiUtilityStatus status;
		ushort union0;

		/// <summary>The status determining which value is valid.</summary>
		public MidiUtilityStatus Status => status;

		/// <summary>The jitter reduction clock. Valid when <see cref="Status" /> is <see cref="MidiUtilityStatus.JitterReductionClock" />.</summary>
		public ushort JitterReductionClock => union0;

		/// <summary>The jitter reduction timestamp. Valid when <see cref="Status" /> is <see cref="MidiUtilityStatus.JitterReductionTimestamp" />.</summary>
		public ushort JitterReductionTimestamp => union0;
	}

	/// <summary>A system message in a <see cref="MidiUniversalMessage" />.</summary>
	/// <remarks>
	///   <para>As of this writing, CoreMIDI's native <c>MIDIEventListForEachEvent</c> function only successfully decodes UMP words carrying <see cref="MidiSystemStatus.Mtc" />, <see cref="MidiSystemStatus.SongPosPointer" /> or <see cref="MidiSystemStatus.SongSelect" /> as <see cref="MidiMessageType.System" />. Every other <see cref="MidiSystemStatus" /> value — including <see cref="MidiSystemStatus.TuneRequest" /> and all System Real-Time statuses (<see cref="MidiSystemStatus.TimingClock" />, <see cref="MidiSystemStatus.Start" />, <see cref="MidiSystemStatus.Continue" />, <see cref="MidiSystemStatus.Stop" />, <see cref="MidiSystemStatus.ActiveSending" />, <see cref="MidiSystemStatus.SystemReset" />) — is reported as <see cref="MidiMessageType.Invalid" /> instead, regardless of caller. This is native CoreMIDI framework behavior, not a limitation of this binding.</para>
	/// </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageSystem {
		MidiSystemStatus status;
		ushort union0;

		/// <summary>The status determining which value is valid.</summary>
		public MidiSystemStatus Status => status;

		/// <summary>The MIDI time code. Valid when <see cref="Status" /> is <see cref="MidiSystemStatus.Mtc" />.</summary>
		public byte TimeCode => (byte) union0;

		/// <summary>The song position pointer. Valid when <see cref="Status" /> is <see cref="MidiSystemStatus.SongPosPointer" />.</summary>
		public ushort SongPositionPointer => union0;

		/// <summary>The selected song. Valid when <see cref="Status" /> is <see cref="MidiSystemStatus.SongSelect" />.</summary>
		public byte SongSelect => (byte) union0;
	}

	/// <summary>The note data of a MIDI 1.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice1Note {
		byte number;
		byte velocity;

		/// <summary>The 7-bit note number.</summary>
		public byte Number => number;

		/// <summary>The 7-bit note velocity.</summary>
		public byte Velocity => velocity;
	}

	/// <summary>The poly pressure data of a MIDI 1.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice1PolyPressure {
		byte noteNumber;
		byte pressure;

		/// <summary>The 7-bit note number.</summary>
		public byte NoteNumber => noteNumber;

		/// <summary>The 7-bit poly pressure data.</summary>
		public byte Pressure => pressure;
	}

	/// <summary>The control change data of a MIDI 1.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice1ControlChange {
		byte index;
		byte data;

		/// <summary>The 7-bit index of the control parameter.</summary>
		public byte Index => index;

		/// <summary>The 7-bit value of the control parameter.</summary>
		public byte Data => data;
	}

	/// <summary>A MIDI 1.0 channel voice message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiUniversalMessageChannelVoice1 {
		MidiCVStatus status;
		byte channel;
		byte reserved0;
		byte reserved1;
		byte reserved2;
		ushort union0;

		/// <summary>The status determining which value is valid.</summary>
		public MidiCVStatus Status => status;

		/// <summary>The MIDI channel (0-15).</summary>
		public byte Channel => channel;

		/// <summary>The note data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.NoteOff" /> or <see cref="MidiCVStatus.NoteOn" />.</summary>
		public MidiUniversalMessageChannelVoice1Note Note {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice1Note*) ((byte*) &self + 8);
			}
		}

		/// <summary>The poly pressure data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PolyPressure" />.</summary>
		public MidiUniversalMessageChannelVoice1PolyPressure PolyPressure {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice1PolyPressure*) ((byte*) &self + 8);
			}
		}

		/// <summary>The control change data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ControlChange" />.</summary>
		public MidiUniversalMessageChannelVoice1ControlChange ControlChange {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice1ControlChange*) ((byte*) &self + 8);
			}
		}

		/// <summary>The 7-bit program number. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ProgramChange" />.</summary>
		public byte Program => (byte) union0;

		/// <summary>The 7-bit channel pressure. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ChannelPressure" />.</summary>
		public byte ChannelPressure => (byte) union0;

		/// <summary>The pitch bend value. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PitchBend" />.</summary>
		public ushort PitchBend => union0;
	}

	/// <summary>A system exclusive (SysEx) message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageSysEx {
		MidiSysExStatus status;
		byte channel;
		byte data0;
		byte data1;
		byte data2;
		byte data3;
		byte data4;
		byte data5;
		byte reserved;

		/// <summary>The status determining how the message should be interpreted.</summary>
		public MidiSysExStatus Status => status;

		/// <summary>The MIDI channel (0-15).</summary>
		public byte Channel => channel;

		/// <summary>The SysEx data (6 bytes, 7-bit values each).</summary>
		/// <returns>A 6-element array with the SysEx data.</returns>
		public byte [] Data => new byte [] { data0, data1, data2, data3, data4, data5 };
	}

	/// <summary>The note data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2Note {
		byte number;
		MidiNoteAttribute attributeType;
		ushort velocity;
		ushort attribute;

		/// <summary>The 7-bit note number.</summary>
		public byte Number => number;

		/// <summary>The attribute type.</summary>
		public MidiNoteAttribute AttributeType => attributeType;

		/// <summary>The note velocity.</summary>
		public ushort Velocity => velocity;

		/// <summary>The attribute data.</summary>
		public ushort Attribute => attribute;
	}

	/// <summary>The poly pressure data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2PolyPressure {
		byte noteNumber;
		byte reserved;
		uint pressure;

		/// <summary>The 7-bit note number.</summary>
		public byte NoteNumber => noteNumber;

		/// <summary>The pressure value.</summary>
		public uint Pressure => pressure;
	}

	/// <summary>The control change data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2ControlChange {
		byte index;
		byte reserved;
		uint data;

		/// <summary>The 7-bit controller number.</summary>
		public byte Index => index;

		/// <summary>The controller value.</summary>
		public uint Data => data;
	}

	/// <summary>The program change data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2ProgramChange {
		MidiProgramChangeOptions options;
		byte program;
		byte reserved0;
		byte reserved1;
		ushort bank;

		/// <summary>The program change options.</summary>
		public MidiProgramChangeOptions Options => options;

		/// <summary>The 7-bit program number.</summary>
		public byte Program => program;

		/// <summary>The 14-bit bank.</summary>
		public ushort Bank => bank;
	}

	/// <summary>The channel pressure data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2ChannelPressure {
		uint data;
		byte reserved0;
		byte reserved1;

		/// <summary>The channel pressure data.</summary>
		public uint Data => data;
	}

	/// <summary>The pitch bend data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2PitchBend {
		uint data;
		byte reserved0;
		byte reserved1;

		/// <summary>The pitch bend data.</summary>
		public uint Data => data;
	}

	/// <summary>The per-note controller data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2PerNoteController {
		byte noteNumber;
		byte index;
		uint data;

		/// <summary>The 7-bit note number.</summary>
		public byte NoteNumber => noteNumber;

		/// <summary>The 7-bit controller number.</summary>
		public byte Index => index;

		/// <summary>The controller data.</summary>
		public uint Data => data;
	}

	/// <summary>The registered/assignable controller data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2Controller {
		byte bank;
		byte index;
		uint data;

		/// <summary>The 7-bit bank.</summary>
		public byte Bank => bank;

		/// <summary>The 7-bit controller number.</summary>
		public byte Index => index;

		/// <summary>The controller data.</summary>
		public uint Data => data;
	}

	/// <summary>The per-note pitch bend data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2PerNotePitchBend {
		byte noteNumber;
		byte reserved;
		uint bend;

		/// <summary>The 7-bit note number.</summary>
		public byte NoteNumber => noteNumber;

		/// <summary>The per-note pitch bend value.</summary>
		public uint Bend => bend;
	}

	/// <summary>The per-note management data of a MIDI 2.0 channel voice message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageChannelVoice2PerNoteManagement {
		byte note;
		MidiPerNoteManagementOptions options;
		byte reserved0;
		byte reserved1;
		byte reserved2;
		byte reserved3;

		/// <summary>The 7-bit note number.</summary>
		public byte Note => note;

		/// <summary>The per-note management options.</summary>
		public MidiPerNoteManagementOptions Options => options;
	}

	/// <summary>A MIDI 2.0 channel voice message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiUniversalMessageChannelVoice2 {
		MidiCVStatus status;
		byte channel;
		byte reserved0;
		byte reserved1;
		byte reserved2;
		uint union0;
		uint union1;

		/// <summary>The status determining which value is valid.</summary>
		public MidiCVStatus Status => status;

		/// <summary>The MIDI channel.</summary>
		public byte Channel => channel;

		/// <summary>The note data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.NoteOff" /> or <see cref="MidiCVStatus.NoteOn" />.</summary>
		public MidiUniversalMessageChannelVoice2Note Note {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2Note*) ((byte*) &self + 8);
			}
		}

		/// <summary>The poly pressure data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PolyPressure" />.</summary>
		public MidiUniversalMessageChannelVoice2PolyPressure PolyPressure {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2PolyPressure*) ((byte*) &self + 8);
			}
		}

		/// <summary>The control change data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ControlChange" />.</summary>
		public MidiUniversalMessageChannelVoice2ControlChange ControlChange {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2ControlChange*) ((byte*) &self + 8);
			}
		}

		/// <summary>The program change data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ProgramChange" />.</summary>
		public MidiUniversalMessageChannelVoice2ProgramChange ProgramChange {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2ProgramChange*) ((byte*) &self + 8);
			}
		}

		/// <summary>The channel pressure data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.ChannelPressure" />.</summary>
		public MidiUniversalMessageChannelVoice2ChannelPressure ChannelPressure {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2ChannelPressure*) ((byte*) &self + 8);
			}
		}

		/// <summary>The pitch bend data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PitchBend" />.</summary>
		public MidiUniversalMessageChannelVoice2PitchBend PitchBend {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2PitchBend*) ((byte*) &self + 8);
			}
		}

		/// <summary>The per-note controller data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.RegisteredPnc" /> or <see cref="MidiCVStatus.AssignablePnc" />.</summary>
		public MidiUniversalMessageChannelVoice2PerNoteController PerNoteController {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2PerNoteController*) ((byte*) &self + 8);
			}
		}

		/// <summary>The registered/assignable controller data. Valid when <see cref="Status" /> is one of <see cref="MidiCVStatus.RegisteredControl" />, <see cref="MidiCVStatus.AssignableControl" />, <see cref="MidiCVStatus.RelRegisteredControl" /> or <see cref="MidiCVStatus.RelAssignableControl" />.</summary>
		public MidiUniversalMessageChannelVoice2Controller Controller {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2Controller*) ((byte*) &self + 8);
			}
		}

		/// <summary>The per-note pitch bend data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PerNotePitchBend" />.</summary>
		public MidiUniversalMessageChannelVoice2PerNotePitchBend PerNotePitchBend {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2PerNotePitchBend*) ((byte*) &self + 8);
			}
		}

		/// <summary>The per-note management data. Valid when <see cref="Status" /> is <see cref="MidiCVStatus.PerNoteMgmt" />.</summary>
		public MidiUniversalMessageChannelVoice2PerNoteManagement PerNoteManagement {
			get {
				var self = this;
				return *(MidiUniversalMessageChannelVoice2PerNoteManagement*) ((byte*) &self + 8);
			}
		}
	}

	/// <summary>The 8-bit system exclusive (SysEx8) data of a 128-bit data message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageSysEx8 {
		byte byteCount;
		byte streamID;
		byte data0;
		byte data1;
		byte data2;
		byte data3;
		byte data4;
		byte data5;
		byte data6;
		byte data7;
		byte data8;
		byte data9;
		byte data10;
		byte data11;
		byte data12;
		byte reserved;

		/// <summary>The byte count of the data including the stream ID (1-14 bytes).</summary>
		public byte ByteCount => byteCount;

		/// <summary>The stream ID.</summary>
		public byte StreamId => streamID;

		/// <summary>The SysEx8 data (13 bytes).</summary>
		/// <returns>A 13-element array with the SysEx8 data.</returns>
		public byte [] Data => new byte [] { data0, data1, data2, data3, data4, data5, data6, data7, data8, data9, data10, data11, data12 };
	}

	/// <summary>The mixed data set of a 128-bit data message.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageMixedDataSet {
		byte mdsID;
		byte data0;
		byte data1;
		byte data2;
		byte data3;
		byte data4;
		byte data5;
		byte data6;
		byte data7;
		byte data8;
		byte data9;
		byte data10;
		byte data11;
		byte data12;
		byte data13;
		byte reserved;

		/// <summary>The mixed data set ID.</summary>
		public byte MixedDataSetId => mdsID;

		/// <summary>The mixed data set data (14 bytes).</summary>
		/// <returns>A 14-element array with the mixed data set data.</returns>
		public byte [] Data => new byte [] { data0, data1, data2, data3, data4, data5, data6, data7, data8, data9, data10, data11, data12, data13 };
	}

	/// <summary>A 128-bit data message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiUniversalMessageData128 {
		MidiSysExStatus status;
		uint union0;
		uint union1;
		uint union2;
		uint union3;

		/// <summary>The status determining which value is valid.</summary>
		public MidiSysExStatus Status => status;

		/// <summary>The SysEx8 data. Valid when <see cref="Status" /> is one of <see cref="MidiSysExStatus.Complete" />, <see cref="MidiSysExStatus.Start" />, <see cref="MidiSysExStatus.Continue" /> or <see cref="MidiSysExStatus.End" />.</summary>
		public MidiUniversalMessageSysEx8 SysEx8 {
			get {
				var self = this;
				return *(MidiUniversalMessageSysEx8*) ((byte*) &self + 4);
			}
		}

		/// <summary>The mixed data set. Valid when <see cref="Status" /> is <see cref="MidiSysExStatus.MixedDataSetHeader" /> or <see cref="MidiSysExStatus.MixedDataSetPayload" />.</summary>
		public MidiUniversalMessageMixedDataSet MixedDataSet {
			get {
				var self = this;
				return *(MidiUniversalMessageMixedDataSet*) ((byte*) &self + 4);
			}
		}
	}

	/// <summary>The raw words of an unknown message in a <see cref="MidiUniversalMessage" />.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiUniversalMessageUnknown {
		uint word0;
		uint word1;
		uint word2;
		uint word3;

		/// <summary>The raw words of the message (up to four 32-bit words).</summary>
		/// <returns>A 4-element array with the raw words.</returns>
		public uint [] Words => new uint [] { word0, word1, word2, word3 };
	}
}
