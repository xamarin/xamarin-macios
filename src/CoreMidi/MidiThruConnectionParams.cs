#if !TVOS && !WATCH
// 
// MidiThruConnectionParams.cs: A C# wrapper around MidiThruConnectionParamsStruct
//
// Authors: Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2016 Xamarin Inc.
//

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using MidiEndpointRef = System.Int32;
using MidiUniqueID = System.Int32;

namespace CoreMidi {
	public enum MidiTransformType : ushort {
		None = 0,
		FilterOut = 1,
		MapControl = 2,
		Add = 8,
		Scale = 9,
		MinValue = 10,
		MaxValue = 11,
		MapValue = 12
	}

	public enum MidiTransformControlType : byte {
		SevenBit = 0,
		FourteenBit = 1,
		SevenBitRpn = 2,
		FourteenBitRpn = 3,
		SevenBitNRpn = 4,
		FourteenBitNRpn = 5
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[NativeName ("MIDITransform")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiTransform {
		public MidiTransformType Transform;
		public short Param;

		public MidiTransform (MidiTransformType transform, short param)
		{
			Transform = transform;
			Param = param;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[NativeName ("MIDIValueMap")]
	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiValueMap {
		byte [] map_value;
		public byte [] Value {
			get {
				return map_value ?? (map_value = new byte [128]);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				if (value.Length != 128)
					throw new ArgumentOutOfRangeException (nameof (value), "The length of the Value array must be 128");
				map_value = value;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[NativeName ("MIDIControlTransform")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiControlTransform {
		public MidiTransformControlType ControlType;
		public MidiTransformControlType RemappedControlType;
		public ushort ControlNumber;
		public MidiTransformType Transform;
		public short Param;

		public MidiControlTransform (MidiTransformControlType controlType,
									 MidiTransformControlType remappedControlType,
									 ushort controlNumber, MidiTransformType transform,
									 short param)
		{
			ControlType = controlType;
			RemappedControlType = remappedControlType;
			ControlNumber = controlNumber;
			Transform = transform;
			Param = param;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[NativeName ("MIDIThruConnectionEndpoint")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiThruConnectionEndpoint {
		public MidiEndpointRef EndpointRef;
		public MidiUniqueID UniqueID;

		public MidiThruConnectionEndpoint (MidiEndpointRef endpointRef, MidiUniqueID uniqueID)
		{
			EndpointRef = endpointRef;
			UniqueID = uniqueID;
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	struct MidiThruConnectionParamsStruct {
		public uint Version;
		public uint NumSources;

		MidiThruConnectionEndpoint Sources_0;
		MidiThruConnectionEndpoint Sources_1;
		MidiThruConnectionEndpoint Sources_2;
		MidiThruConnectionEndpoint Sources_3;
		MidiThruConnectionEndpoint Sources_4;
		MidiThruConnectionEndpoint Sources_5;
		MidiThruConnectionEndpoint Sources_6;
		MidiThruConnectionEndpoint Sources_7;
		public MidiThruConnectionEndpoint []? Sources {
			get {
				if (NumSources == 0)
					return null;

				var rv = new List<MidiThruConnectionEndpoint> ();
				if (NumSources > 0)
					rv.Add (Sources_0);
				if (NumSources > 1)
					rv.Add (Sources_1);
				if (NumSources > 2)
					rv.Add (Sources_2);
				if (NumSources > 3)
					rv.Add (Sources_3);
				if (NumSources > 4)
					rv.Add (Sources_4);
				if (NumSources > 5)
					rv.Add (Sources_5);
				if (NumSources > 6)
					rv.Add (Sources_6);
				if (NumSources > 7)
					rv.Add (Sources_7);
				return rv.ToArray ();
			}
			set {
				if (value?.Length > 8)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 8 endpoints are allowed");
				Sources_0 = value?.Length > 0 ? value [0] : default (MidiThruConnectionEndpoint);
				Sources_1 = value?.Length > 1 ? value [1] : default (MidiThruConnectionEndpoint);
				Sources_2 = value?.Length > 2 ? value [2] : default (MidiThruConnectionEndpoint);
				Sources_3 = value?.Length > 3 ? value [3] : default (MidiThruConnectionEndpoint);
				Sources_4 = value?.Length > 4 ? value [4] : default (MidiThruConnectionEndpoint);
				Sources_5 = value?.Length > 5 ? value [5] : default (MidiThruConnectionEndpoint);
				Sources_6 = value?.Length > 6 ? value [6] : default (MidiThruConnectionEndpoint);
				Sources_7 = value?.Length > 7 ? value [7] : default (MidiThruConnectionEndpoint);
				NumSources = (uint) (value?.Length ?? 0);
			}
		}
		public uint NumDestinations;

		MidiThruConnectionEndpoint Destinations_0;
		MidiThruConnectionEndpoint Destinations_1;
		MidiThruConnectionEndpoint Destinations_2;
		MidiThruConnectionEndpoint Destinations_3;
		MidiThruConnectionEndpoint Destinations_4;
		MidiThruConnectionEndpoint Destinations_5;
		MidiThruConnectionEndpoint Destinations_6;
		MidiThruConnectionEndpoint Destinations_7;
		public MidiThruConnectionEndpoint []? Destinations {
			get {
				if (NumDestinations == 0)
					return null;

				var rv = new List<MidiThruConnectionEndpoint> ();
				if (NumDestinations > 0)
					rv.Add (Destinations_0);
				if (NumDestinations > 1)
					rv.Add (Destinations_1);
				if (NumDestinations > 2)
					rv.Add (Destinations_2);
				if (NumDestinations > 3)
					rv.Add (Destinations_3);
				if (NumDestinations > 4)
					rv.Add (Destinations_4);
				if (NumDestinations > 5)
					rv.Add (Destinations_5);
				if (NumDestinations > 6)
					rv.Add (Destinations_6);
				if (NumDestinations > 7)
					rv.Add (Destinations_7);
				return rv.ToArray ();
			}
			set {
				if (value?.Length > 8)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 8 endpoints are allowed");
				Destinations_0 = value?.Length > 0 ? value [0] : default (MidiThruConnectionEndpoint);
				Destinations_1 = value?.Length > 1 ? value [1] : default (MidiThruConnectionEndpoint);
				Destinations_2 = value?.Length > 2 ? value [2] : default (MidiThruConnectionEndpoint);
				Destinations_3 = value?.Length > 3 ? value [3] : default (MidiThruConnectionEndpoint);
				Destinations_4 = value?.Length > 4 ? value [4] : default (MidiThruConnectionEndpoint);
				Destinations_5 = value?.Length > 5 ? value [5] : default (MidiThruConnectionEndpoint);
				Destinations_6 = value?.Length > 6 ? value [6] : default (MidiThruConnectionEndpoint);
				Destinations_7 = value?.Length > 7 ? value [7] : default (MidiThruConnectionEndpoint);
				NumDestinations = (uint) (value?.Length ?? 0);
			}
		}

		byte ChannelMap_00;
		byte ChannelMap_01;
		byte ChannelMap_02;
		byte ChannelMap_03;
		byte ChannelMap_04;
		byte ChannelMap_05;
		byte ChannelMap_06;
		byte ChannelMap_07;
		byte ChannelMap_08;
		byte ChannelMap_09;
		byte ChannelMap_10;
		byte ChannelMap_11;
		byte ChannelMap_12;
		byte ChannelMap_13;
		byte ChannelMap_14;
		byte ChannelMap_15;
		public byte[] ChannelMap {
			get {
				return new byte [] {
					ChannelMap_00, ChannelMap_01, ChannelMap_02, ChannelMap_03, ChannelMap_04, ChannelMap_05, ChannelMap_06, ChannelMap_07,
					ChannelMap_08, ChannelMap_09, ChannelMap_10, ChannelMap_11, ChannelMap_12, ChannelMap_13, ChannelMap_14, ChannelMap_15,
				};
			}
			set {
				if (value.Length > 16)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 16 channels are allowed");

				ChannelMap_00 = value.Length > 00 ? value [00] : (byte) 0;
				ChannelMap_01 = value.Length > 01 ? value [01] : (byte) 0;
				ChannelMap_02 = value.Length > 02 ? value [02] : (byte) 0;
				ChannelMap_03 = value.Length > 03 ? value [03] : (byte) 0;
				ChannelMap_04 = value.Length > 04 ? value [04] : (byte) 0;
				ChannelMap_05 = value.Length > 05 ? value [05] : (byte) 0;
				ChannelMap_06 = value.Length > 06 ? value [06] : (byte) 0;
				ChannelMap_07 = value.Length > 07 ? value [07] : (byte) 0;
				ChannelMap_08 = value.Length > 08 ? value [08] : (byte) 0;
				ChannelMap_09 = value.Length > 09 ? value [09] : (byte) 0;
				ChannelMap_10 = value.Length > 10 ? value [10] : (byte) 0;
				ChannelMap_11 = value.Length > 11 ? value [11] : (byte) 0;
				ChannelMap_12 = value.Length > 12 ? value [12] : (byte) 0;
				ChannelMap_13 = value.Length > 13 ? value [13] : (byte) 0;
				ChannelMap_14 = value.Length > 14 ? value [14] : (byte) 0;
				ChannelMap_15 = value.Length > 15 ? value [15] : (byte) 0;
			}
		}

		public byte LowVelocity;
		public byte HighVelocity;
		public byte LowNote;
		public byte HighNote;
		public MidiTransform NoteNumber;
		public MidiTransform Velocity;
		public MidiTransform KeyPressure;
		public MidiTransform ChannelPressure;
		public MidiTransform ProgramChange;
		public MidiTransform PitchBend;

		public byte FilterOutSysEx;
		public byte FilterOutMtc;
		public byte FilterOutBeatClock;
		public byte FilterOutTuneRequest;

		byte Reserved2_0;
		byte Reserved2_1;
		byte Reserved2_2;
		public byte FilterOutAllControls;

		public ushort NumControlTransforms;
		public ushort NumMaps;

		ushort Reserved3_0;
		ushort Reserved3_1;
		ushort Reserved3_2;
		ushort Reserved3_3;

		// FUN: structure is variably-sized. It contains numControlTransform instances of 
		// MidiControlTransform, followed by numMaps instances of MidiValueMap.
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public class MidiThruConnectionParams {
		MidiThruConnectionParamsStruct connectionParams;

		MidiControlTransform []? controls;
		MidiValueMap []? maps;

		[DllImport (Constants.CoreMidiLibrary)]
		unsafe extern static void MIDIThruConnectionParamsInitialize (MidiThruConnectionParamsStruct* inConnectionParams);

		public MidiThruConnectionParams ()
		{
			// Always create a valid init point
			MidiThruConnectionParamsStruct tmpStruct;
			unsafe {
				MIDIThruConnectionParamsInitialize (&tmpStruct);
			}
			connectionParams = tmpStruct;
		}

		public MidiThruConnectionEndpoint []? Sources {
			get { return connectionParams.Sources; }
			set { connectionParams.Sources = value;	}
		}

		public MidiThruConnectionEndpoint []? Destinations {
			get { return connectionParams.Destinations; }
			set { connectionParams.Destinations = value; }
		}

		public byte [] ChannelMap {
			get { return connectionParams.ChannelMap; }
			set {
				connectionParams.ChannelMap = value ?? new byte [16];
			}
		}

		public byte LowVelocity {
			get { return connectionParams.LowVelocity; }
			set { connectionParams.LowVelocity = value; }
		}

		public byte HighVelocity {
			get { return connectionParams.HighVelocity; }
			set { connectionParams.HighVelocity = value; }
		}

		public byte LowNote {
			get { return connectionParams.LowNote; }
			set { connectionParams.LowNote = value; }
		}

		public byte HighNote {
			get { return connectionParams.HighNote; }
			set { connectionParams.HighNote = value; }
		}

		public MidiTransform NoteNumber {
			get { return connectionParams.NoteNumber; }
			set { connectionParams.NoteNumber = value; }
		}

		public MidiTransform Velocity {
			get { return connectionParams.Velocity; }
			set { connectionParams.Velocity = value; }
		}

		public MidiTransform KeyPressure {
			get { return connectionParams.KeyPressure; }
			set { connectionParams.KeyPressure = value; }
		}

		public MidiTransform ChannelPressure {
			get { return connectionParams.ChannelPressure; }
			set { connectionParams.ChannelPressure = value; }
		}

		public MidiTransform ProgramChange {
			get { return connectionParams.ProgramChange; }
			set { connectionParams.ProgramChange = value; }
		}

		public MidiTransform PitchBend {
			get { return connectionParams.PitchBend; }
			set { connectionParams.PitchBend = value; }
		}

		public bool FilterOutSysEx {
			get { return connectionParams.FilterOutSysEx == 1; }
			set { connectionParams.FilterOutSysEx = value ? (byte) 1 : (byte) 0; }
		}

		public bool FilterOutMtc {
			get { return connectionParams.FilterOutMtc == 1; }
			set { connectionParams.FilterOutMtc = value ? (byte) 1 : (byte) 0; }
		}

		public bool FilterOutBeatClock {
			get { return connectionParams.FilterOutBeatClock == 1; }
			set { connectionParams.FilterOutBeatClock = value ? (byte) 1 : (byte) 0; }
		}

		public bool FilterOutTuneRequest {
			get { return connectionParams.FilterOutTuneRequest == 1; }
			set { connectionParams.FilterOutTuneRequest = value ? (byte) 1 : (byte) 0; }
		}

		public bool FilterOutAllControls {
			get { return connectionParams.FilterOutAllControls == 1; }
			set { connectionParams.FilterOutAllControls = value ? (byte) 1 : (byte) 0; }
		}

		public MidiControlTransform []? Controls {
			get => controls;
			set {
				if (value?.Length > ushort.MaxValue)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 65535 controls are allowed");
				controls = value;
			}
		}

		public MidiValueMap []? Maps {
			get => maps;
			set {
				if (value?.Length > ushort.MaxValue)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 65535 maps are allowed");
				maps = value;
			}
		}

		internal void ReadStruct (NSData data)
		{
			IntPtr buffer = data.Bytes;
			connectionParams = Marshal.PtrToStructure<MidiThruConnectionParamsStruct> (buffer)!;

			// Put ourselves at the end of the static struct in case we need to fetch the dynamic part of the struct
			IntPtr bufferEnd = IntPtr.Add (buffer, Marshal.SizeOf<MidiThruConnectionParamsStruct> ());
			var controlsSize = Marshal.SizeOf<MidiControlTransform> ();

			if (connectionParams.NumControlTransforms == 0)
				Controls = null;
			else {
				Controls = new MidiControlTransform [connectionParams.NumControlTransforms];
				unsafe {
					fixed (void* arrAddr = Controls)
						Buffer.MemoryCopy ((void*) bufferEnd, arrAddr, controlsSize * connectionParams.NumControlTransforms, controlsSize * connectionParams.NumControlTransforms);
				}
			}

			if (connectionParams.NumMaps == 0)
				Maps = null;
			else {
				Maps = new MidiValueMap [connectionParams.NumMaps];
				bufferEnd = IntPtr.Add (bufferEnd, controlsSize * connectionParams.NumControlTransforms);
				unsafe {
					for (int i = 0; i < connectionParams.NumMaps; i++) {
						Maps [i].Value = new byte [128];
						fixed (void* arrAddr = Maps [i].Value)
							Buffer.MemoryCopy ((void*) bufferEnd, arrAddr, 128, 128);
						bufferEnd += 128;
					}
				}
			}
		}

		internal NSData WriteStruct ()
		{
			connectionParams.NumControlTransforms = Controls is not null ? (ushort) Controls.Length : (ushort) 0;
			connectionParams.NumMaps = Maps is not null ? (ushort) Maps.Length : (ushort) 0;

			var paramsSize = Marshal.SizeOf<MidiThruConnectionParamsStruct> ();
			var controlsSize = Marshal.SizeOf<MidiControlTransform> ();
			// Get the full size of the struct, static + dynamic parts
			var fullSize = paramsSize +
				(Controls is null ? 0 : controlsSize * Controls.Length) +
				(Maps is null ? 0 : 128 * Maps.Length);
			var buffer = Marshal.AllocHGlobal (fullSize);
			var bufferEnd = IntPtr.Add (buffer, Marshal.SizeOf<MidiThruConnectionParamsStruct> ());

			try {
				// Copy static sized struct
				Marshal.StructureToPtr<MidiThruConnectionParamsStruct> (connectionParams, buffer, false);

				if (connectionParams.NumControlTransforms > 0) {
					unsafe {
						fixed (void* arrAddr = Controls)
							Buffer.MemoryCopy (arrAddr, (void*) bufferEnd, controlsSize * connectionParams.NumControlTransforms, controlsSize * connectionParams.NumControlTransforms);
					}
				}

				if (connectionParams.NumMaps > 0) {
					// Set the destination buffer after controls arr if any
					bufferEnd = IntPtr.Add (bufferEnd, controlsSize * connectionParams.NumControlTransforms);
					unsafe {
						for (int i = 0; i < connectionParams.NumMaps; i++) {
							fixed (void* arrAddr = Maps! [i].Value)
								Buffer.MemoryCopy (arrAddr, (void*) bufferEnd, 128, 128);
							bufferEnd += 128;
						}
					}
				}

				var data = NSData.FromBytes (buffer, (nuint) fullSize);
				return data;
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}
	}
#endif // !COREBUILD
}
#endif
