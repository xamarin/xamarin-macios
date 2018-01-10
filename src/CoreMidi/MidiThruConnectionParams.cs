// 
// MidiThruConnectionParams.cs: A C# wrapper around MidiThruConnectionParamsStruct
//
// Authors: Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

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

	[StructLayout (LayoutKind.Sequential)]
	public unsafe struct MidiValueMap {
		byte[] map_value;
		public byte[] Value {
			get {
				return map_value ?? (map_value = new byte [128]);
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));
				if (value.Length != 128)
					throw new ArgumentOutOfRangeException (nameof (value), "The length of the Value array must be 128");
				map_value = value;
			}
		}
	}

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
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
		public MidiThruConnectionEndpoint[] Sources;
		public uint NumDestinations;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 8)]
		public MidiThruConnectionEndpoint[] Destinations;

		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ChannelMap;
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
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] Reserved2;
		public byte FilterOutAllControls;

		public ushort NumControlTransforms; 
		public ushort NumMaps;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 4)]
		public ushort[] Reserved3;

		// FUN: structure is variably-sized. It contains numControlTransform instances of 
		// MidiControlTransform, followed by numMaps instances of MidiValueMap.
	}

#if !COREBUILD
	public class MidiThruConnectionParams {
		MidiThruConnectionParamsStruct connectionParams;

		[DllImport (Constants.CoreMidiLibrary)]
		extern static void MIDIThruConnectionParamsInitialize (out MidiThruConnectionParamsStruct inConnectionParams);

		public MidiThruConnectionParams()
		{
			// Always create a valid init point
			MIDIThruConnectionParamsInitialize (out connectionParams);
		}

		public MidiThruConnectionEndpoint[] Sources {
			get { return connectionParams.Sources; }
			set {
				if (value?.Length > 8)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 8 endpoints are allowed");
				connectionParams.Sources = value;
			}
		}

		public MidiThruConnectionEndpoint[] Destinations {
			get { return connectionParams.Destinations; }
			set {
				if (value?.Length > 8)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 8 endpoints are allowed");
				connectionParams.Destinations = value;
			}
		}

		public byte[] ChannelMap {
			get { return connectionParams.ChannelMap; }
			set {
				if (value?.Length > 16)
					throw new ArgumentOutOfRangeException (nameof (value), "A maximum of 16 channels are allowed");
				connectionParams.ChannelMap = value;
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
			set { connectionParams.FilterOutSysEx = value ? (byte)1 : (byte)0; }
		}

		public bool FilterOutMtc {
			get { return connectionParams.FilterOutMtc == 1; }
			set { connectionParams.FilterOutMtc = value ? (byte)1 : (byte)0; }
		}

		public bool FilterOutBeatClock {
			get { return connectionParams.FilterOutBeatClock == 1; }
			set { connectionParams.FilterOutBeatClock = value ? (byte)1 : (byte)0; }
		}

		public bool FilterOutTuneRequest {
			get { return connectionParams.FilterOutTuneRequest == 1; }
			set { connectionParams.FilterOutTuneRequest = value ? (byte)1 : (byte)0; }
		}

		public bool FilterOutAllControls {
			get { return connectionParams.FilterOutAllControls == 1; }
			set { connectionParams.FilterOutAllControls = value ? (byte)1 : (byte)0; }
		}

		public MidiControlTransform[] Controls { get; set; }
		public MidiValueMap[] Maps { get; set; }

		internal void ReadStruct (NSData data)
		{
			IntPtr buffer = data.Bytes;
			connectionParams = (MidiThruConnectionParamsStruct) Marshal.PtrToStructure (buffer, typeof (MidiThruConnectionParamsStruct));

			// Put ourselves at the end of the static struct in case we need to fetch the dynamic part of the struct
			IntPtr bufferEnd = IntPtr.Add (buffer, Marshal.SizeOf (typeof (MidiThruConnectionParamsStruct)));
			var controlsSize = Marshal.SizeOf (typeof (MidiControlTransform));

			if (connectionParams.NumControlTransforms == 0)
				Controls = null;
			else {
				Controls = new MidiControlTransform[connectionParams.NumControlTransforms];
				unsafe { // Lets use memcpy to avoid a for loop
					fixed (MidiControlTransform* arrAddr = &Controls[0])
						Runtime.memcpy ((IntPtr) arrAddr, bufferEnd, controlsSize * connectionParams.NumControlTransforms);
				}
			}

			if (connectionParams.NumMaps == 0)
				Maps = null;
			else {
				Maps = new MidiValueMap [connectionParams.NumMaps];
				bufferEnd = IntPtr.Add (bufferEnd, controlsSize * connectionParams.NumControlTransforms);
				unsafe { // Lets use memcpy to avoid a for loop
					for (int i = 0; i < connectionParams.NumMaps; i++) {
						Maps [i].Value = new byte [128];
						fixed (byte* arrAddr = &Maps[i].Value [0])
							Runtime.memcpy ((IntPtr) arrAddr, bufferEnd, 128);
					}
				}
			}
		}

		internal NSData WriteStruct ()
		{
			if (Sources?.Length > 0) {
				connectionParams.NumSources = (uint)Sources.Length;
				for (int i = 0; i < Sources.Length; i++)
					connectionParams.Sources[i] = Sources[i];
			}

			if (Destinations?.Length > 0) {
				connectionParams.NumDestinations = (uint)Destinations.Length;
				for (int i = 0; i < Destinations.Length; i++)
					connectionParams.Destinations[i] = Destinations[i];
			}

			if (ChannelMap?.Length > 0) {
				for (int i = 0; i < ChannelMap.Length; i++)
					connectionParams.ChannelMap[i] = ChannelMap[i];
			}

			connectionParams.NumControlTransforms = Controls != null ? (ushort) Controls.Length : (ushort) 0;
			connectionParams.NumMaps = Maps != null ? (ushort) Maps.Length : (ushort) 0;

			var paramsSize = Marshal.SizeOf (typeof (MidiThruConnectionParamsStruct));
			var controlsSize = Marshal.SizeOf (typeof (MidiControlTransform));
			// Get the full size of the struct, static + dynamic parts
			var fullSize = paramsSize +
				(Controls == null ? 0 : controlsSize * Controls.Length) +
				(Maps == null ? 0 : 128 * Maps.Length);
			var buffer = Marshal.AllocHGlobal (fullSize);
			var bufferEnd = IntPtr.Add (buffer, Marshal.SizeOf (typeof (MidiThruConnectionParamsStruct)));

			try {
				// Copy static sized struct
				Marshal.StructureToPtr (connectionParams, buffer, false);

				if (connectionParams.NumControlTransforms > 0) {
					unsafe { // Lets use memcpy to avoid a for loop
						fixed (MidiControlTransform* arrAddr = &Controls[0])
							Runtime.memcpy (bufferEnd, (IntPtr) arrAddr, controlsSize * connectionParams.NumControlTransforms);
					}
				}

				if (connectionParams.NumMaps > 0) {
					// Set the destination buffer after controls arr if any
					bufferEnd = IntPtr.Add (bufferEnd, controlsSize * connectionParams.NumControlTransforms);
					unsafe { // Lets use memcpy to avoid a for loop
						for (int i = 0; i < connectionParams.NumMaps; i++) {
							fixed (byte* arrAddr = &Maps[i].Value [0])
								Runtime.memcpy (bufferEnd, (IntPtr) arrAddr, 128);
						}
					}
				}

				var data = NSData.FromBytes (buffer, (nuint) fullSize);
				return data;
			}
			finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal (buffer);
			}
		}
	}
#endif // !COREBUILD
}

