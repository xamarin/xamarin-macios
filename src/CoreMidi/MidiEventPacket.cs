// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace CoreMidi {
	/// <summary>Represents the native CoreMIDI <c>MIDIEventPacket</c> variable-sized C struct.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[NativeName ("MIDIEventPacket")]
	[StructLayout (LayoutKind.Sequential, Pack = 4)]
	public struct MidiEventPacket {
		ulong /* MIDITimeStamp */ timeStamp;
		uint /* UInt32 */ wordCount;

		/* UInt32 words[64]; */
		internal uint word_00;
		uint word_01;
		uint word_02;
		uint word_03;
		uint word_04;
		uint word_05;
		uint word_06;
		uint word_07;
		uint word_08;
		uint word_09;
		uint word_10;
		uint word_11;
		uint word_12;
		uint word_13;
		uint word_14;
		uint word_15;
		uint word_16;
		uint word_17;
		uint word_18;
		uint word_19;
		uint word_20;
		uint word_21;
		uint word_22;
		uint word_23;
		uint word_24;
		uint word_25;
		uint word_26;
		uint word_27;
		uint word_28;
		uint word_29;
		uint word_30;
		uint word_31;
		uint word_32;
		uint word_33;
		uint word_34;
		uint word_35;
		uint word_36;
		uint word_37;
		uint word_38;
		uint word_39;
		uint word_40;
		uint word_41;
		uint word_42;
		uint word_43;
		uint word_44;
		uint word_45;
		uint word_46;
		uint word_47;
		uint word_48;
		uint word_49;
		uint word_50;
		uint word_51;
		uint word_52;
		uint word_53;
		uint word_54;
		uint word_55;
		uint word_56;
		uint word_57;
		uint word_58;
		uint word_59;
		uint word_60;
		uint word_61;
		uint word_62;
		uint word_63;

		/// <summary>The timestamp for this packet.</summary>
		/// <returns>The timestamp for this packet.</returns>
		public ulong Timestamp {
			get => timeStamp;
			set => timeStamp = value;
		}

		/// <summary>The number of 32-bit Midi words in this packet.</summary>
		/// <returns>The number of 32-bit Midi words in this packet.</returns>
		public uint WordCount {
			get => wordCount;
			set {
				if (value > 64)
					throw new ArgumentOutOfRangeException (nameof (value), "WordCount can't be higher than 64.");
				wordCount = value;
			}
		}

		/// <summary>All the 32-bit Midi words in this packet.</summary>
		/// <returns>All the 32-bit Midi words in this packet.</returns>
		public uint [] Words {
			get {
				var wc = wordCount;
				if (wc > 64)
					throw new ArgumentOutOfRangeException (nameof (WordCount), "WordCount can't be higher than 64.");
				var rv = new uint [wc];
				unsafe {
					fixed (uint* destination = rv) {
						fixed (uint* source = &word_00) {
							Buffer.MemoryCopy (source, destination, rv.Length * sizeof (uint), wc * sizeof (uint));
						}
					}
				}
				return rv;
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				if (value.Length > 64)
					throw new ArgumentOutOfRangeException (nameof (value), "WordCount can't be higher than 64.");
				wordCount = (uint) value.Length;
				unsafe {
					fixed (uint* destination = &word_00) {
						fixed (uint* source = value) {
							Buffer.MemoryCopy (source, destination, 64 * sizeof (uint), value.Length * sizeof (uint));
						}
					}
				}
			}
		}

		/// <summary>An indexer for the 32-bit Midi words in this packet.</summary>
		/// <param name="index">The index of the 32-bit Midi word to set or get.</param>
		/// <returns>The 32-bit Midi words for specified index.</returns>
		public uint this [int index] {
			get {
				if (index < 0)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be positive.");
				if (index >= 64)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be less than 64.");
				if (index + 1 > wordCount)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be less than WordCount.");
				unsafe {
					fixed (uint* firstWord = &word_00)
						return firstWord [index];
				}
			}
			set {
				if (index < 0)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be positive.");
				if (index >= 64)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be less than 64.");
				if (index + 1 > wordCount)
					throw new ArgumentOutOfRangeException (nameof (index), "index must be less than WordCount.");
				unsafe {
					fixed (uint* firstWord = &word_00)
						firstWord [index] = value;
				}
			}
		}

#if !__TVOS__

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static OSStatus MIDIEventPacketSysexBytesForGroup (MidiEventPacket* pkt, byte /* UInt8 */ groupIndex, IntPtr* /* CFDataRef __nullable * __mononull */ outData);

		/// <summary>Get MIDI 1.0 sysex bytes on the specified group.</summary>
		/// <param name="groupIndex">The index of the target group.</param>
		/// <param name="status">A status code that describes the result of the operation. This will be <see cref="MidiError.Ok" /> in case of success.</param>
		/// <returns>An <see cref="NSData" /> that contains the requested byte stream.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
		public unsafe NSData? GetSysexBytes (byte groupIndex, out MidiError status)
		{
			var handle = default (IntPtr);

			fixed (MidiEventPacket* self = &this) {
				status = (MidiError) MIDIEventPacketSysexBytesForGroup (self, groupIndex, &handle);
			}
			if (handle == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSData> (handle, true);
		}
#endif // !__TVOS__
	}
}
