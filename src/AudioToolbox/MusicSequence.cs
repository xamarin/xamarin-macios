//
// MusicPlayer.cs: Bindings to the AudioToolbox's MusicPlayers APIs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2014 Xamarin Inc.
//

#if !WATCH

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
using XamCore.Foundation;
#if !COREBUILD
using XamCore.CoreAnimation;
#if !TVOS
using XamCore.CoreMidi;
#endif
using XamCore.AudioUnit;
#endif

using MidiEndpointRef = System.Int32;

namespace XamCore.AudioToolbox {

#if !COREBUILD
	public delegate void MusicSequenceUserCallback (MusicTrack track, double inEventTime, MusicEventUserData inEventData, double inStartSliceBeat, double inEndSliceBeat);

	delegate void MusicSequenceUserCallbackProxy (/* void * */ IntPtr inClientData, /* MusicSequence* */ IntPtr inSequence, /* MusicTrack* */ IntPtr inTrack, /* MusicTimeStamp */ double inEventTime, /* MusicEventUserData* */ IntPtr inEventData, /* MusicTimeStamp */ double inStartSliceBeat, /* MusicTimeStamp */ double inEndSliceBeat);
#endif

	// MusicPlayer.h
	public class MusicSequence : INativeObject
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
		IntPtr handle;
		internal MusicSequence (IntPtr handle) {
			this.handle = handle;
		}

		static Dictionary <IntPtr, MusicSequenceUserCallback> userCallbackHandles = new Dictionary <IntPtr, MusicSequenceUserCallback> (Runtime.IntPtrEqualityComparer);

		static MusicSequenceUserCallbackProxy userCallbackProxy = new MusicSequenceUserCallbackProxy (UserCallbackProxy);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus NewMusicSequence (/* MusicSequence* */ out IntPtr outSequence);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus DisposeMusicSequence (/* MusicSequence */ IntPtr inSequence);
		
		public MusicSequence ()
		{
			NewMusicSequence (out handle);
			lock (sequenceMap)
				sequenceMap [handle] = new WeakReference (this);
		}
		
		~MusicSequence ()
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

				lock (userCallbackHandles)
					userCallbackHandles.Remove (handle);

				// Remove native user callback
				MusicSequenceSetUserCallback (handle, null, IntPtr.Zero);

				DisposeMusicSequence (handle);
				lock (sequenceMap){
					sequenceMap.Remove (handle);
				}
				handle = IntPtr.Zero;
			}
		}

		static readonly Dictionary<IntPtr,WeakReference> sequenceMap = new Dictionary<IntPtr,WeakReference> (Runtime.IntPtrEqualityComparer);
	       
		internal static MusicSequence Lookup (IntPtr handle)
		{
			lock (sequenceMap){
				WeakReference weakRef;
				
				if (sequenceMap.TryGetValue (handle, out weakRef)){
					var target = weakRef.Target;
					if (target != null){
						return (MusicSequence) target;
					}
					sequenceMap.Remove (handle);
				}
				var ms = new MusicSequence (handle);
				sequenceMap [handle] = new WeakReference (ms);
				return ms;
			}
		}


		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceSetAUGraph (/* MusicSequence */ IntPtr inSequence, /* AUGraph */ IntPtr inGraph);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetAUGraph (/* MusicSequence */ IntPtr inSequence, /* AUGraph* */ out IntPtr outGraph);

		public AUGraph AUGraph {
			get {
				IntPtr h;
				if (MusicSequenceGetAUGraph (handle, out h) != MusicPlayerStatus.Success)
					return null;

				return new AUGraph (h);
			}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");

				MusicSequenceSetAUGraph (handle, value.Handle);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceSetSequenceType (/* MusicSequence */ IntPtr inSequence, MusicSequenceType inType);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetSequenceType (/* MusicSequence */ IntPtr inSequence, out MusicSequenceType outType);

		public MusicSequenceType SequenceType {
			get {
				MusicSequenceType type;
				MusicSequenceGetSequenceType (handle, out type);
				return type;
			}
			set {
				MusicSequenceSetSequenceType (handle, value);
			}
		}

		public void GetSmpteResolution (short resolution, out sbyte fps, out byte ticks)
		{
			// MusicSequenceGetSMPTEResolution is CF_INLINE -> can't be pinvoke'd (it's not part of the library)
			fps = (sbyte) ((resolution & 0xFF00) >> 8);
			ticks = (byte) (resolution & 0x007F);
		}
		
		public short SetSmpteResolution (sbyte fps, byte ticks)
		{
			// MusicSequenceSetSMPTEResolution is CF_INLINE -> can't be pinvoke'd (it's not part of the library)
			if (fps > 0)
				fps = (sbyte) -fps; 
			return (short) ((fps << 8) + ticks);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* CFDictionaryRef */ IntPtr MusicSequenceGetInfoDictionary (/* MusicSequence */ IntPtr inSequence);

		public NSDictionary GetInfoDictionary ()
		{
			return Runtime.GetNSObject<NSDictionary> (MusicSequenceGetInfoDictionary (handle));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceNewTrack (/* MusicSequence */ IntPtr inSequence, /* MusicTrack* */ out IntPtr outTrack);

		public MusicTrack CreateTrack ()
		{
			IntPtr trackHandle;
			if (MusicSequenceNewTrack (handle, out trackHandle) == MusicPlayerStatus.Success)
				return new MusicTrack (this, trackHandle, owns: true);
			else
				return null;
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetTrackCount (/* MusicSequence */ IntPtr inSequence, /* UInt32* */ out int outNumberOfTracks);

		// an `uint` but we keep `int` for compatibility (should be enough tracks)
		public int TrackCount {
			get {
				int count;
				
				if (MusicSequenceGetTrackCount (handle, out count) == MusicPlayerStatus.Success)
					return count;
				return 0;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetIndTrack (/* MusicSequence */ IntPtr inSequence, /* Uint32 */ int inTrackIndex, /* MusicTrack* */ out IntPtr outTrack);
			
		public MusicTrack GetTrack (int trackIndex)
		{
			IntPtr outTrack;
			
			if (MusicSequenceGetIndTrack (handle, trackIndex, out outTrack) == MusicPlayerStatus.Success)
				return new MusicTrack (this, outTrack, owns: false);
			else
				return null;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetTrackIndex (/* MusicSequence */ IntPtr inSequence, /* MusicTrack */ IntPtr inTrack, /* UInt32* */ out int outTrackIndex);

#if !XAMCORE_2_0
		[Obsolete ("Use GetTrackIndex(MusicTrack, out int) to distinguish between the track or an error code")]
		public int GetTrackIndex (MusicTrack track)
		{
			int idx;
			if (track == null)
				throw new ArgumentNullException ("track");
			var status = MusicSequenceGetTrackIndex (handle, track.Handle, out idx);
			if (status == MusicPlayerStatus.Success)
				return idx;
			// that's never clear if the return value is the track or an error code
			return (int) status;
		}
#endif
		public MusicPlayerStatus GetTrackIndex (MusicTrack track, out int index)
		{
			if (track == null)
				throw new ArgumentNullException ("track");

			return MusicSequenceGetTrackIndex (handle, track.Handle, out index);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetTempoTrack (/* MusicSequence */ IntPtr sequence, /* MusicTrack */ out IntPtr outTrack);

		public MusicTrack GetTempoTrack ()
		{
			IntPtr outTrack;

			if (MusicSequenceGetTempoTrack (handle, out outTrack) == MusicPlayerStatus.Success)
				return new MusicTrack (this, outTrack, owns: false);
			else
				return null;
		}

#if IOS
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceSetMIDIEndpoint (/* MusicSequence */ IntPtr inSequence, MidiEndpointRef inEndpoint);

		public MusicPlayerStatus SetMidiEndpoint (MidiEndpoint endpoint)
		{
			if (endpoint == null)
				throw new ArgumentNullException ("endpoint");
			return MusicSequenceSetMIDIEndpoint (handle, endpoint.handle);
		}
#endif // IOS

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetSecondsForBeats (/* MusicSequence */ IntPtr inSequence, /* MusicTimeStamp */ double inBeats, /* Float64* */ out double outSeconds);

		public double GetSecondsForBeats (double beats)
		{
			double sec;
			if (MusicSequenceGetSecondsForBeats (handle, beats, out sec) == MusicPlayerStatus.Success)
				return sec;
			return 0;
		}
			
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceGetBeatsForSeconds (/* MusicSequence */ IntPtr inSequence, /* Float64 */ double inSeconds, /* MusicTimeStamp* */ out double outBeats);

		public double GetBeatsForSeconds (double seconds)
		{
			double beats;
			if (MusicSequenceGetBeatsForSeconds (handle, seconds, out beats) == MusicPlayerStatus.Success)
				return beats;
			return 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceSetUserCallback (/* MusicSequence */ IntPtr inSequence, MusicSequenceUserCallbackProxy inCallback, /* void * */ IntPtr inClientData);

		public void SetUserCallback (MusicSequenceUserCallback callback)
		{
			lock (userCallbackHandles)
				userCallbackHandles [handle] = callback;

			MusicSequenceSetUserCallback (handle, userCallbackProxy, IntPtr.Zero);
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (MusicSequenceUserCallbackProxy))]
#endif
		static void UserCallbackProxy (IntPtr inClientData, IntPtr inSequence, IntPtr inTrack, double inEventTime, IntPtr inEventData, double inStartSliceBeat, double inEndSliceBeat)
		{
			MusicSequenceUserCallback userCallback;
			lock (userCallbackHandles)
				userCallbackHandles.TryGetValue (inSequence, out userCallback);

			if (userCallback != null) {
				var userEventData = new MusicEventUserData (inEventData);
				var musicSequence = MusicSequence.Lookup (inSequence);
				var musicTrack = new MusicTrack (musicSequence, inTrack, false);

				userCallback (musicTrack, inEventTime, userEventData, inStartSliceBeat, inEndSliceBeat);
			}
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceBeatsToBarBeatTime (/* MusicSequence */ IntPtr inSequence, /* MusicTimeStamp */ double inBeats, /* UInt32 */ int inSubbeatDivisor, out CABarBeatTime outBarBeatTime);

		public MusicPlayerStatus BeatsToBarBeatTime (double beats, int subbeatDivisor, out CABarBeatTime barBeatTime)
		{
			return MusicSequenceBeatsToBarBeatTime (handle, beats, subbeatDivisor, out barBeatTime);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceBarBeatTimeToBeats (/* MusicSequence */ IntPtr inSequence, CABarBeatTime inBarBeatTime, /* MusicTimeStamp*/ out double outBeats);
#if !XAMCORE_2_0
		[Obsolete ("Use overload with an out 'double beats'")]
		public MusicPlayerStatus BarBeatTimeToBeats (CABarBeatTime barBeatTime)
		{
			double d;
			return MusicSequenceBarBeatTimeToBeats (handle, barBeatTime, out d);
		}
#endif
		public MusicPlayerStatus BarBeatTimeToBeats (CABarBeatTime barBeatTime, out double beats)
		{
			return MusicSequenceBarBeatTimeToBeats (handle, barBeatTime, out beats);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceReverse (/* MusicSequence */ IntPtr inSequence);

		public MusicPlayerStatus Reverse ()
		{
			return MusicSequenceReverse (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceFileLoad (/* MusicSequence */ IntPtr inSequence, /* CFURLRef */ IntPtr inFileRef, MusicSequenceFileTypeID inFileTypeHint, MusicSequenceLoadFlags inFlags);

		public MusicPlayerStatus LoadFile (NSUrl url, MusicSequenceFileTypeID fileTypeId, MusicSequenceLoadFlags loadFlags = 0)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			
			return MusicSequenceFileLoad (handle, url.Handle, fileTypeId, loadFlags);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceFileLoadData (/* MusicSequence */ IntPtr inSequence, /* CFDataRef */ IntPtr inData,  MusicSequenceFileTypeID inFileTypeHint, MusicSequenceLoadFlags inFlags);

		public MusicPlayerStatus LoadData (NSData data, MusicSequenceFileTypeID fileTypeId, MusicSequenceLoadFlags loadFlags = 0)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			return MusicSequenceFileLoadData (handle, data.Handle, fileTypeId, loadFlags);
		}
			
			
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceFileCreate (/* MusicSequence */ IntPtr inSequence, /* CFURLRef */ IntPtr inFileRef, MusicSequenceFileTypeID inFileType, MusicSequenceFileFlags inFlags, /* SInt16 */ ushort resolution);

		// note: resolution should be short instead of ushort
		public MusicPlayerStatus CreateFile (NSUrl url, MusicSequenceFileTypeID fileType, MusicSequenceFileFlags flags = 0, ushort resolution = 0)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			
			return MusicSequenceFileCreate (handle, url.Handle, fileType, flags, resolution);
		}
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicSequenceFileCreateData (/* MusicSequence */ IntPtr inSequence, MusicSequenceFileTypeID inFileType, MusicSequenceFileFlags inFlags, /* SInt16 */ ushort resolution, /* CFDataRef* */ out IntPtr outData);

		// note: resolution should be short instead of ushort
		public NSData CreateData (MusicSequenceFileTypeID fileType, MusicSequenceFileFlags flags = 0, ushort resolution = 0)
		{
			IntPtr theData;
			if (MusicSequenceFileCreateData (handle, fileType, flags, resolution, out theData) == MusicPlayerStatus.Success)
				return Runtime.GetNSObject<NSData> (theData);
			return null;
		}
#endif // !COREBUILD
	}

	// typedef UInt32 -> MusicPlayer.h
	public enum MusicSequenceType : uint {
		Beats = 0x62656174, 	// 'beat'
		Seconds = 0x73656373,	// 'secs'
		Samples = 0x73616d70    // 'samp'
	}
}

#endif // IOS || TVOS
