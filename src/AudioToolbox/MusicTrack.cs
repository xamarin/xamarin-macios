//
// MusicTrack.cs: Bindings to the AudioToolbox's MusicPlayers APIs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2014 Xamarin Inc.
//
// MISSING:
//       MusicTrackNewParameterEvent
//       MusicTrackNewAUPresetEvent
//

#nullable enable

#if !WATCH

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
#if IOS
using CoreMidi;
#endif

using MidiEndpointRef = System.Int32;
using System.Runtime.Versioning;

namespace AudioToolbox {

	// MusicPlayer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiNoteMessage {
		public byte Channel;
		public byte Note;
		public byte Velocity;
		public byte ReleaseVelocity;
		public /* Float32 */ float Duration;

		public MidiNoteMessage (byte channel, byte note, byte velocity, byte releaseVelocity, float duration)
		{
			Channel = channel;
			Note = note;
			Velocity = velocity;
			ReleaseVelocity = releaseVelocity;
			Duration = duration;
		}
	}

	// MusicPlayer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MidiChannelMessage {
		public byte Status;
		public byte Data1;
		public byte Data2;
		public byte Reserved;

		public MidiChannelMessage (byte status, byte data1, byte data2)
		{
			Status = status;
			Data1 = data1;
			Data2 = data2;
			Reserved = 0;
		}
	}

	//
	// Since we can not express this in the way that C does, we expose a
	// high level API, and we provide a ToUnmanaged that returns an allocated
	// IntPtr buffer with the data
	//
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class MidiData {
#else
	public abstract class _MidiData {
#endif
		protected int len, start;
		protected byte []? data;
		protected IntPtr buffer;

		public void SetData (byte [] Data)
		{
			len = Data.Length;
			start = 0;
			data = Data;
			buffer = IntPtr.Zero;
		}

		public void SetData (int len, int start, byte [] Data)
		{
			if (len + start > Data.Length)
				throw new ArgumentException ("len+start go beyond the end of Data");
			if (len < 0 || start < 0)
				throw new ArgumentException ("len||start are negative");
			this.len = len;
			this.start = start;
			this.data = Data;
			buffer = IntPtr.Zero;
		}

		public void SetData (int len, IntPtr buffer)
		{
			this.len = len;
			this.buffer = buffer;
			this.data = null;
		}

		//
		// Converts our high-level representations to a buffer that
		// we can pass to unmanaged functions that take a MidiRawData
		//
		internal abstract IntPtr ToUnmanaged ();
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class MidiRawData : MidiData {
#else
	public class MidiRawData : _MidiData {
#endif
		public MidiRawData () { }

		internal override IntPtr ToUnmanaged ()
		{
			unsafe {
				// Length (UInt32) + length (UInt8 for each)
				var target = (byte*) Marshal.AllocHGlobal (4 + len);
				*((int*) target) = len;
				var rdata = target + 4;

				if (data is not null)
					Marshal.Copy (data, start, (IntPtr) rdata, len);
				else
					Buffer.MemoryCopy ((void*) buffer, (void*) rdata, len, len);
				return (IntPtr) target;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class MusicEventUserData : MidiRawData {
		public MusicEventUserData () { }

		internal MusicEventUserData (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handle));

			int length = Marshal.ReadInt32 (handle);

			var buffer = new byte [length];
			Marshal.Copy (handle + 4, buffer, 0, length);

			len = length;
			data = buffer;
		}
	}

	//
	// Since we can not express this in the way that C does, we expose a
	// high level API, and we provide a ToUnmanaged that returns an allocated
	// IntPtr buffer with the data
	//
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class MidiMetaEvent : MidiData {
#else
	public class MidiMetaEvent : _MidiData {
#endif
		public byte MetaEventType;

		internal override IntPtr ToUnmanaged ()
		{
			unsafe {
				// MetaEventType (UInt8) + 3 x unused (UInt8) + length (UInt32) + length (UInt8 for each)
				var target = (byte*) Marshal.AllocHGlobal (8 + len);
				*target = MetaEventType;
				var plen = (int*) (target + 4);
				*plen = len;
				var rdata = target + 8;

				if (data is not null)
					Marshal.Copy (data, start, (IntPtr) rdata, len);
				else
					Buffer.MemoryCopy ((void*) buffer, (void*) rdata, len, len);
				return (IntPtr) target;
			}
		}
	}

	// MusicPlayer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct ExtendedNoteOnEvent {
		public /* MusicDeviceInstrumentID */ uint InstrumentID;
		public /* MusicDeviceGroupID */ uint DeviceGroupID;
		public /* Float32 */ float Duration;

		// MusicDeviceNoteParams extendedParams

		// Documented as having to be 2
#pragma warning disable 169
		int argCount;
#pragma warning restore 169
		public float Pitch;
		public float Velocity;
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class MusicTrack : DisposableObject {
#if !COREBUILD
		MusicSequence? sequence;

		internal MusicTrack (MusicSequence sequence, IntPtr handle, bool owns)
			: base (handle, owns)
		{
			this.sequence = sequence;
		}

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns) {
				if (sequence is not null)
					MusicSequenceDisposeTrack (sequence.Handle, Handle);
			}
			sequence = null;
			base.Dispose (disposing);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceDisposeTrack (/* MusicSequence */ IntPtr inSequence, /* MusicTrack */ IntPtr inTrack);

		public static MusicTrack? FromSequence (MusicSequence sequence)
		{
			if (sequence is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sequence));
			return sequence.CreateTrack ();
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackGetSequence (/* MusicTrack */ IntPtr inTrack, /* MusicSequence* */ IntPtr* outSequence);

		public MusicSequence? Sequence {
			get {
				IntPtr seqHandle;
				unsafe {
					if (MusicTrackGetSequence (Handle, &seqHandle) == MusicPlayerStatus.Success)
						return MusicSequence.Lookup (seqHandle);
				}
				return null;
			}
		}

#if IOS
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetDestMIDIEndpoint (/* MusicTrack */ IntPtr inTrack, MidiEndpointRef inEndpoint);

		public MusicPlayerStatus SetDestMidiEndpoint (MidiEndpoint endpoint)
		{
			return MusicTrackSetDestMIDIEndpoint (Handle, endpoint is null ? MidiObject.InvalidRef : endpoint.MidiHandle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackGetDestMIDIEndpoint (/* MusicTrack */ IntPtr inTrack, MidiEndpointRef* outEndpoint);

		public MusicPlayerStatus GetDestMidiEndpoint (out MidiEndpoint? outEndpoint)
		{
			MidiEndpointRef midiHandle;
			MusicPlayerStatus result;
			unsafe {
				result = MusicTrackGetDestMIDIEndpoint (Handle, &midiHandle);
			}
			outEndpoint = (result == MusicPlayerStatus.Success)? new MidiEndpoint (midiHandle): null;
			return result;
		}
#endif

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetDestNode (/* MusicTrack */ IntPtr inTrack, /* AUNode */ int inNode);

		public MusicPlayerStatus SetDestNode (int node)
		{
			return MusicTrackSetDestNode (Handle, node);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static /* OSStatus */ MusicPlayerStatus MusicTrackSetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, byte* inData, /* UInt32 */ int inLength);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, double* inData, /* UInt32 */ int inLength);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static /* OSStatus */ MusicPlayerStatus MusicTrackGetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, byte* outData, /* UInt32* */ int* ioLength);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackGetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, double* outData, /* UInt32* */ int* ioLength);

		// internal use only - it's a UInt32 in the API
		enum SequenceTrackProperty {
			LoopInfo,
			OffsetTime,
			MuteStatus,
			SoloStatus,
			AutomatedParameters,
			TrackLength,
			TimeResolution
		}

		public bool MuteStatus {
			get {
				byte val;
				unsafe {
					int len = 1;
					MusicTrackGetProperty (Handle, SequenceTrackProperty.MuteStatus, &val, &len);
					return val != 0;
				}
			}
			set {
				unsafe {
					var val = value ? (byte) 1 : (byte) 0;
					MusicTrackSetProperty (Handle, SequenceTrackProperty.MuteStatus, &val, 1);
				}
			}
		}

		public bool SoloStatus {
			get {
				byte val;
				unsafe {
					int len = 1;
					MusicTrackGetProperty (Handle, SequenceTrackProperty.SoloStatus, &val, &len);
					return val != 0;
				}
			}
			set {
				unsafe {
					var val = value ? (byte) 1 : (byte) 0;
					MusicTrackSetProperty (Handle, SequenceTrackProperty.SoloStatus, &val, 1);
				}
			}
		}

		public double TrackLength {
			get {
				double value = 0;
				int len = sizeof (double);
				unsafe {
					MusicTrackGetProperty (Handle, SequenceTrackProperty.TrackLength, &value, &len);
				}
				return value;
			}
			set {
				unsafe {
					MusicTrackSetProperty (Handle, SequenceTrackProperty.TrackLength, &value, sizeof (double));
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDINoteEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, MidiNoteMessage* inMessage);

		public unsafe MusicPlayerStatus AddMidiNoteEvent (double timeStamp, MidiNoteMessage message)
		{
			return MusicTrackNewMIDINoteEvent (Handle, timeStamp, &message);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDIChannelEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, MidiChannelMessage* inMessage);

		public unsafe MusicPlayerStatus AddMidiChannelEvent (double timestamp, MidiChannelMessage channelMessage)
		{
			return MusicTrackNewMIDIChannelEvent (Handle, timestamp, &channelMessage);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDIRawDataEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimestamp, /* MIDIRawData* */ IntPtr inRawData);

		public MusicPlayerStatus AddMidiRawDataEvent (double timestamp, MidiRawData rawData)
		{
			if (rawData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (rawData));

			var native = rawData.ToUnmanaged ();
			var r = MusicTrackNewMIDIRawDataEvent (Handle, timestamp, native);
			Marshal.FreeHGlobal (native);
			return r;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewExtendedNoteEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, ExtendedNoteOnEvent* inInfo);

		public MusicPlayerStatus AddNewExtendedNoteEvent (double timestamp, ExtendedNoteOnEvent evt)
		{
			unsafe {
				return MusicTrackNewExtendedNoteEvent (Handle, timestamp, &evt);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewExtendedTempoEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* Float64 */ double bpm);

		public MusicPlayerStatus AddExtendedTempoEvent (double timestamp, double bmp)
		{
			return MusicTrackNewExtendedTempoEvent (Handle, timestamp, bmp);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMetaEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* MIDIMetaEvent* */ IntPtr inMetaEvent);

		public MusicPlayerStatus AddMetaEvent (double timestamp, MidiMetaEvent metaEvent)
		{
			if (metaEvent is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (metaEvent));

			var ptr = metaEvent.ToUnmanaged ();
			var ret = MusicTrackNewMetaEvent (Handle, timestamp, ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewUserEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* MusicEventUserData* */ IntPtr inUserData);

		public MusicPlayerStatus AddUserEvent (double timestamp, MusicEventUserData userData)
		{
			if (userData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (userData));
			var ptr = userData.ToUnmanaged ();
			var ret = MusicTrackNewUserEvent (Handle, timestamp, ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackMoveEvents (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime, /* MusicTimeStamp */ double inMoveTime);

		public MusicPlayerStatus MoveEvents (double startTime, double endTime, double moveTime)
		{
			return MusicTrackMoveEvents (Handle, startTime, endTime, moveTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackClear (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime);

		public MusicPlayerStatus Clear (double startTime, double endTime)
		{
			return MusicTrackClear (Handle, startTime, endTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackCut (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime);

		public MusicPlayerStatus Cut (double startTime, double endTime)
		{
			return MusicTrackCut (Handle, startTime, endTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackCopyInsert (/* MusicTrack */ IntPtr inSourceTrack, /* MusicTimeStamp */ double inSourceStartTime, double /* MusicTimeStamp */ inSourceEndTime, /* MusicTrack */ IntPtr inDestTrack, /* MusicTimeStamp */ double inDestInsertTime);

		public MusicPlayerStatus CopyInsert (double sourceStartTime, double sourceEndTime, MusicTrack targetTrack, double targetInsertTime)
		{
			if (targetTrack is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetTrack));
			return MusicTrackCopyInsert (Handle, sourceStartTime, sourceEndTime, targetTrack.Handle, targetInsertTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackMerge (/* MusicTrack */ IntPtr inSourceTrack, /* MusicTimeStamp */ double inSourceStartTime, double /* MusicTimeStamp */ inSourceEndTime, /* MusicTrack */ IntPtr inDestTrack, /* MusicTimeStamp */ double inDestInsertTime);

		public MusicPlayerStatus Merge (double sourceStartTime, double sourceEndTime, MusicTrack targetTrack, double targetInsertTime)
		{
			if (targetTrack is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetTrack));
			return MusicTrackMerge (Handle, sourceStartTime, sourceEndTime, targetTrack.Handle, targetInsertTime);
		}
#endif // !COREBUILD
	}
}

#endif // IOS || TVOS
