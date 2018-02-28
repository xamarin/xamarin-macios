//
// MusicPlayer.cs: Bindings to the AudioToolbox's MusicPlayers APIs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2014 Xamarin Inc.
//
//

#if !WATCH

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.AudioToolbox {

	// untyped enum (used as an OSStatus in the API) -> MusicPlayer.h
	public enum MusicPlayerStatus {
		Success = 0,
		InvalidSequenceType = -10846,
		TrackIndexError = -10859,
		TrackNotFound = -10858,
		EndOfTrack = -10857,
		StartOfTrack = -10856,
		IllegalTrackDestination = -10855,
		NoSequence = -10854,
		InvalidEventType = -10853,
		InvalidPlayerState = -10852,
		CannotDoInCurrentContext = -10863,
		NoTrackDestination = -66720
	}

	// typedef UInt32 -> MusicPlayer.h
	public enum MusicEventType : uint {
		Null,
		ExtendedNote = 1,
		ExtendedTempo = 3,
		User = 4,
		Meta = 5,
		MidiNoteMessage = 6,
		MidiChannelMessage = 7,
		MidiRawData = 8,
		Parameter = 9,
		AUPreset = 10
	}

	// typedef UInt32 -> MusicPlayer.h
	[Flags]
	public enum MusicSequenceLoadFlags {
		PreserveTracks = 0,
		ChannelsToTracks = 1 << 0
	}

	// typedef UInt32 -> MusicPlayer.h
	public enum MusicSequenceFileTypeID : uint {
		Any = 0,
		Midi = 0x6d696469, // 'midi'
		iMelody = 0x696d656c, // 'imel'
	}

	// typedef UInt32 -> MusicPlayer.h
	[Flags]
	public enum MusicSequenceFileFlags {
		Default = 0,
		EraseFile = 1
	}

	// MusicPlayer.h
	public class MusicPlayer : INativeObject, IDisposable {
		IntPtr handle;

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus NewMusicPlayer (/* MusicPlayer* */ out IntPtr outPlayer);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus DisposeMusicPlayer (/* MusicPlayer */ IntPtr inPlayer);
							      
		private MusicPlayer (IntPtr handle) {
			this.handle = handle;
		}

		~MusicPlayer ()
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
			currentSequence = null;
			if (handle != IntPtr.Zero){
				DisposeMusicPlayer (handle);
				handle = IntPtr.Zero;
			}
		}

		public MusicPlayer ()
		{
			var result = NewMusicPlayer (out handle);
			if (result == MusicPlayerStatus.Success)
				return;
			throw new Exception ("Unable to create MusicPlayer: " + result);
		}

		static public MusicPlayer Create (out MusicPlayerStatus OSstatus)
		{
			IntPtr handle;
			OSstatus = NewMusicPlayer (out handle);
			if (OSstatus == 0)
				return new MusicPlayer (handle);
			return null;
		}

		MusicSequence currentSequence;
#if !COREBUILD
		// note: MusicTimeStamp -> Float64

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetTime (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp* */ out double outTime);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetTime (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp* */ double inTime);
		
		public double Time {
			get {
				double time;
				MusicPlayerGetTime (handle, out time);
				return time;
			}
			set {
				MusicPlayerSetTime (handle, value);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerPreroll (/* MusicPlayer */ IntPtr inPlayer);
		
		public MusicPlayerStatus Preroll ()
		{
			return MusicPlayerPreroll (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerStart (/* MusicPlayer */ IntPtr inPlayer);
		
		public MusicPlayerStatus Start ()
		{
			return MusicPlayerStart (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerStop (/* MusicPlayer */ IntPtr inPlayer);

		public MusicPlayerStatus Stop ()
		{
			return MusicPlayerStop (handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerIsPlaying (/* MusicPlayer */ IntPtr inPlayer, /* Boolean* */ out bool outIsPlaying);

		public bool IsPlaying {
			get {
				bool res;
				MusicPlayerIsPlaying (handle, out res);
				return res;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetPlayRateScalar (/* MusicPlayer */ IntPtr inPlayer, /* Float64 */ double inScaleRate);
		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetPlayRateScalar (/* MusicPlayer */ IntPtr inPlayer, /* Float64* */ out double outScaleRate);
		
		public double PlayRateScalar {
			get {
				double rate;
				MusicPlayerGetPlayRateScalar (handle, out rate);
				return rate;
			}
			set {
				MusicPlayerSetPlayRateScalar (handle, value);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetHostTimeForBeats (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp */ double inBeats, /* UInt64* */ out long outHostTime);
		
		public MusicPlayerStatus GetHostTimeForBeats (double beats, out long hostTime)
		{
			return MusicPlayerGetHostTimeForBeats (handle, beats, out hostTime);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetBeatsForHostTime (/* MusicPlayer */ IntPtr inPlayer, /* UInt64 */ long inHostTime, /* MusicTimeStamp* */ out double outBeats);
		
		public MusicPlayerStatus GetBeatsForHostTime (long hostTime, out double beats)
		{
			return MusicPlayerGetBeatsForHostTime (handle, hostTime, out beats);
		}

		
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetSequence (/* MusicPlayer */ IntPtr inPlayer, /* MusicSequence* */ out IntPtr outSequence);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetSequence (/* MusicPlayer */ IntPtr inPlayer, IntPtr inSequence);
		
		public MusicSequence MusicSequence {
			get {
				IntPtr seqHandle;
				if (MusicPlayerGetSequence (handle, out seqHandle) == MusicPlayerStatus.Success)
					return MusicSequence.Lookup (seqHandle);
				else
					return null;
			}
			set {
				currentSequence = value;
				MusicPlayerSetSequence (handle, value == null ? IntPtr.Zero : value.Handle);
			}
		}
#endif
	}
}

#endif // IOS || TVOS
