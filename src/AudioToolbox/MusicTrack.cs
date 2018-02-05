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

#if !WATCH

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
#if IOS
using CoreMidi;
#endif

using MidiEndpointRef = System.Int32;

namespace AudioToolbox {

	// MusicPlayer.h
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
	public abstract class _MidiData {
		protected int len, start;
		protected byte [] data;
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
	public class MidiRawData : _MidiData {
		public MidiRawData () {}

		internal override IntPtr ToUnmanaged ()
		{
			unsafe {
				// Length (UInt32) + length (UInt8 for each)
				var target = (byte *) Marshal.AllocHGlobal (4 + len);
				*((int *) target) = len;
				var rdata = target + 4;
				
				if (data != null)
					Marshal.Copy (data, start, (IntPtr) rdata, len);
				else
					Runtime.memcpy (rdata, (byte *) buffer, len);
				return (IntPtr) target;
			}
		}
	}

	public class MusicEventUserData : MidiRawData {
		public MusicEventUserData () {}

		internal MusicEventUserData (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException (nameof (handle));

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
	public class MidiMetaEvent : _MidiData {
		public byte MetaEventType;
		
		internal override IntPtr ToUnmanaged ()
		{
			unsafe {
				// MetaEventType (UInt8) + 3 x unused (UInt8) + length (UInt32) + length (UInt8 for each)
				var target = (byte *) Marshal.AllocHGlobal (8 + len);
				*target = MetaEventType;
				var plen = (int *)(target + 4);
				*plen = len;
				var rdata = target + 8;
				
				if (data != null)
					Marshal.Copy (data, start, (IntPtr) rdata, len);
				else
					Runtime.memcpy (rdata, (byte *) buffer, len);
				return (IntPtr) target;
			}
		}
	}

	// MusicPlayer.h
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
	
	public class MusicTrack : INativeObject
#if !COREBUILD
	, IDisposable
#endif
	{
#if !COREBUILD
		MusicSequence sequence;
		IntPtr handle;
		bool owns;

		internal MusicTrack (MusicSequence sequence, IntPtr handle, bool owns)
		{
			this.sequence = sequence;
			this.handle = handle;
			this.owns = owns;
		}

		~MusicTrack ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				if (owns)
					MusicSequenceDisposeTrack (sequence.Handle, handle);
				handle = IntPtr.Zero;
			}
			sequence = null;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceDisposeTrack (/* MusicSequence */ IntPtr inSequence, /* MusicTrack */ IntPtr inTrack);

		public static MusicTrack FromSequence (MusicSequence sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException ("sequence");
			return sequence.CreateTrack ();
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackGetSequence (/* MusicTrack */ IntPtr inTrack, /* MusicSequence* */ out IntPtr outSequence);

		public MusicSequence Sequence {
			get {
				IntPtr seqHandle;
				
				if (MusicTrackGetSequence (handle, out seqHandle) == MusicPlayerStatus.Success)
					return MusicSequence.Lookup (seqHandle);
				return null;
			}
		}
 
#if IOS
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetDestMIDIEndpoint (/* MusicTrack */ IntPtr inTrack, MidiEndpointRef inEndpoint);

		public MusicPlayerStatus SetDestMidiEndpoint (MidiEndpoint endpoint)
		{
			return MusicTrackSetDestMIDIEndpoint (handle, endpoint == null ? MidiObject.InvalidRef : endpoint.MidiHandle);
		}
#endif // IOS

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetDestNode (/* MusicTrack */ IntPtr inTrack, /* AUNode */ int inNode);

		public MusicPlayerStatus SetDestNode (int node)
		{
			return MusicTrackSetDestNode (handle, node);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static /* OSStatus */ MusicPlayerStatus MusicTrackSetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, void *inData, /* UInt32 */ int inLength);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackSetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, ref double inData, /* UInt32 */ int inLength);
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static /* OSStatus */ MusicPlayerStatus MusicTrackGetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, void *outData, /* UInt32* */ ref int ioLength);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackGetProperty (/* MusicTrack */ IntPtr inTrack, /* UInt32 */ SequenceTrackProperty propertyId, ref double outData, /* UInt32* */ ref int ioLength);

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
					MusicTrackGetProperty (handle, SequenceTrackProperty.MuteStatus, &val, ref len);
					return val != 0;
				}
			}
			set {
				unsafe {
					MusicTrackSetProperty (handle, SequenceTrackProperty.MuteStatus, &value, 1);
				}
			}
		}

		public bool SoloStatus {
			get {
				byte val;
				unsafe {
					int len = 1;
					MusicTrackGetProperty (handle, SequenceTrackProperty.SoloStatus, &val, ref len);
					return val != 0;
				}
			}
			set {
				unsafe {
					MusicTrackSetProperty (handle, SequenceTrackProperty.SoloStatus, &value, 1);
				}
			}
		}

		public double TrackLength {
			get {
				double value = 0;
				int len = sizeof (double);
				MusicTrackGetProperty (handle, SequenceTrackProperty.TrackLength, ref value, ref len);
				return value;
			}
			set {
				MusicTrackSetProperty (handle, SequenceTrackProperty.TrackLength, ref value, sizeof (double));	
			}
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDINoteEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, MidiNoteMessage *inMessage);

		public unsafe MusicPlayerStatus AddMidiNoteEvent (double timeStamp, MidiNoteMessage message)
		{
			return MusicTrackNewMIDINoteEvent (handle, timeStamp, &message);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDIChannelEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, MidiChannelMessage *inMessage);

		public unsafe MusicPlayerStatus AddMidiChannelEvent (double timestamp, MidiChannelMessage channelMessage)
		{
			return MusicTrackNewMIDIChannelEvent (handle, timestamp, &channelMessage);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMIDIRawDataEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimestamp, /* MIDIRawData* */ IntPtr inRawData);

		public MusicPlayerStatus AddMidiRawDataEvent (double timestamp, MidiRawData rawData)
		{
			if (rawData == null)
				throw new ArgumentNullException ("rawData");
			
			var native = rawData.ToUnmanaged ();
			var r = MusicTrackNewMIDIRawDataEvent (handle, timestamp, native);
			Marshal.FreeHGlobal (native);
			return r;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewExtendedNoteEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, ExtendedNoteOnEvent *inInfo);

		public MusicPlayerStatus AddNewExtendedNoteEvent (double timestamp, ExtendedNoteOnEvent evt)
		{
			unsafe {
				return MusicTrackNewExtendedNoteEvent (handle, timestamp, &evt);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewExtendedTempoEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* Float64 */ double bpm);

		public MusicPlayerStatus AddExtendedTempoEvent (double timestamp, double bmp)
		{
			return MusicTrackNewExtendedTempoEvent (handle, timestamp, bmp);
		}
			      
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewMetaEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* MIDIMetaEvent* */ IntPtr inMetaEvent);

		public MusicPlayerStatus AddMetaEvent (double timestamp, MidiMetaEvent metaEvent)
		{
			if (metaEvent == null)
				throw new ArgumentNullException ("metaEvent");
			
			var ptr = metaEvent.ToUnmanaged ();
			var ret = MusicTrackNewMetaEvent (handle, timestamp, ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackNewUserEvent (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inTimeStamp, /* MusicEventUserData* */ IntPtr inUserData);

		public MusicPlayerStatus AddUserEvent (double timestamp, MusicEventUserData userData)
		{
			if (userData == null)
				throw new ArgumentNullException ("userData");
			var ptr = userData.ToUnmanaged ();
			var ret = MusicTrackNewUserEvent (handle, timestamp, ptr);
			Marshal.FreeHGlobal (ptr);
			return ret;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackMoveEvents (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime, /* MusicTimeStamp */ double inMoveTime);

		public MusicPlayerStatus MoveEvents (double startTime, double endTime, double moveTime)
		{
			return MusicTrackMoveEvents (handle, startTime, endTime, moveTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackClear (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime);

		public MusicPlayerStatus Clear (double startTime, double endTime)
		{
			return MusicTrackClear (handle, startTime, endTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackCut (/* MusicTrack */ IntPtr inTrack, /* MusicTimeStamp */ double inStartTime, /* MusicTimeStamp */ double inEndTime);

		public MusicPlayerStatus Cut (double startTime, double endTime)
		{
			return MusicTrackCut (handle, startTime, endTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackCopyInsert (/* MusicTrack */ IntPtr inSourceTrack, /* MusicTimeStamp */ double inSourceStartTime, double /* MusicTimeStamp */ inSourceEndTime, /* MusicTrack */ IntPtr inDestTrack, /* MusicTimeStamp */ double inDestInsertTime);

		public MusicPlayerStatus CopyInsert (double sourceStartTime, double sourceEndTime, MusicTrack targetTrack, double targetInsertTime)
		{
			if (targetTrack == null)
				throw new ArgumentNullException ("targetTrack");
			return MusicTrackCopyInsert (handle, sourceStartTime, sourceEndTime, targetTrack.Handle, targetInsertTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicTrackMerge (/* MusicTrack */ IntPtr inSourceTrack, /* MusicTimeStamp */ double inSourceStartTime, double /* MusicTimeStamp */ inSourceEndTime, /* MusicTrack */ IntPtr inDestTrack, /* MusicTimeStamp */ double inDestInsertTime);

		public MusicPlayerStatus Merge (double sourceStartTime, double sourceEndTime, MusicTrack targetTrack, double targetInsertTime)
		{
			if (targetTrack == null)
				throw new ArgumentNullException ("targetTrack");
			return MusicTrackMerge (handle, sourceStartTime, sourceEndTime, targetTrack.Handle, targetInsertTime);
		}
#endif // !COREBUILD

#if false
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static MusicPlayerStatus
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static MusicPlayerStatus
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static MusicPlayerStatus
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static MusicPlayerStatus
#endif
	}
}

#endif // IOS || TVOS
