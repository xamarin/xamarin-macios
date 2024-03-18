//
// MusicPlayer.cs: Bindings to the AudioToolbox's MusicPlayers APIs
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012-2014 Xamarin Inc.
//
//

#nullable enable

#if !WATCH

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioToolbox {

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


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// MusicPlayer.h
	public class MusicPlayer : DisposableObject {
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus NewMusicPlayer (/* MusicPlayer* */ IntPtr* outPlayer);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus DisposeMusicPlayer (/* MusicPlayer */ IntPtr inPlayer);

		[Preserve (Conditional = true)]
		MusicPlayer (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		protected override void Dispose (bool disposing)
		{
			currentSequence = null;
			if (Owns && Handle != IntPtr.Zero)
				DisposeMusicPlayer (Handle);
			base.Dispose (disposing);
		}

		unsafe static IntPtr Create ()
		{
			IntPtr handle;
			var result = NewMusicPlayer (&handle);
			if (result == MusicPlayerStatus.Success)
				return handle;
			throw new Exception ("Unable to create MusicPlayer: " + result);
		}

		public MusicPlayer ()
			: base (Create (), true)
		{
		}

		static public MusicPlayer? Create (out MusicPlayerStatus OSstatus)
		{
			IntPtr handle;
			unsafe {
				OSstatus = NewMusicPlayer (&handle);
			}
			if (OSstatus == 0)
				return new MusicPlayer (handle, true);
			return null;
		}

		MusicSequence? currentSequence;
#if !COREBUILD
		// note: MusicTimeStamp -> Float64

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetTime (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp* */ double* outTime);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetTime (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp* */ double inTime);

		public double Time {
			get {
				double time;
				unsafe {
					MusicPlayerGetTime (Handle, &time);
				}
				return time;
			}
			set {
				MusicPlayerSetTime (Handle, value);
			}
		}

		public MusicPlayerStatus GetTime (out double time)
		{
			time = 0;
			unsafe {
				return MusicPlayerGetTime (Handle, (double*) Unsafe.AsPointer<double> (ref time));
			}
		}

		public MusicPlayerStatus SetTime (double time)
		{
			return MusicPlayerSetTime (Handle, time);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerPreroll (/* MusicPlayer */ IntPtr inPlayer);

		public MusicPlayerStatus Preroll ()
		{
			return MusicPlayerPreroll (Handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerStart (/* MusicPlayer */ IntPtr inPlayer);

		public MusicPlayerStatus Start ()
		{
			return MusicPlayerStart (Handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerStop (/* MusicPlayer */ IntPtr inPlayer);

		public MusicPlayerStatus Stop ()
		{
			return MusicPlayerStop (Handle);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerIsPlaying (/* MusicPlayer */ IntPtr inPlayer, /* Boolean* */ byte* outIsPlaying);

		public bool IsPlaying {
			get {
				byte res;
				unsafe {
					MusicPlayerIsPlaying (Handle, &res);
				}
				return res != 0;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetPlayRateScalar (/* MusicPlayer */ IntPtr inPlayer, /* Float64 */ double inScaleRate);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetPlayRateScalar (/* MusicPlayer */ IntPtr inPlayer, /* Float64* */ double* outScaleRate);

		public double PlayRateScalar {
			get {
				double rate;
				unsafe {
					MusicPlayerGetPlayRateScalar (Handle, &rate);
				}
				return rate;
			}
			set {
				MusicPlayerSetPlayRateScalar (Handle, value);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetHostTimeForBeats (/* MusicPlayer */ IntPtr inPlayer, /* MusicTimeStamp */ double inBeats, /* UInt64* */ long* outHostTime);

		public MusicPlayerStatus GetHostTimeForBeats (double beats, out long hostTime)
		{
			hostTime = 0;
			unsafe {
				return MusicPlayerGetHostTimeForBeats (Handle, beats, (long*) Unsafe.AsPointer<long> (ref hostTime));
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetBeatsForHostTime (/* MusicPlayer */ IntPtr inPlayer, /* UInt64 */ long inHostTime, /* MusicTimeStamp* */ double* outBeats);

		public MusicPlayerStatus GetBeatsForHostTime (long hostTime, out double beats)
		{
			beats = 0;
			unsafe {
				return MusicPlayerGetBeatsForHostTime (Handle, hostTime, (double*) Unsafe.AsPointer<double> (ref beats));
			}
		}


		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ MusicPlayerStatus MusicPlayerGetSequence (/* MusicPlayer */ IntPtr inPlayer, /* MusicSequence* */ IntPtr* outSequence);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static /* OSStatus */ MusicPlayerStatus MusicPlayerSetSequence (/* MusicPlayer */ IntPtr inPlayer, IntPtr inSequence);

		public MusicSequence? MusicSequence {
			get {
				IntPtr seqHandle;
				unsafe {
					if (MusicPlayerGetSequence (Handle, &seqHandle) == MusicPlayerStatus.Success)
						return MusicSequence.Lookup (seqHandle);
				}

				return null;
			}
			set {
				currentSequence = value;
				MusicPlayerSetSequence (Handle, value.GetHandle ());
			}
		}
#endif
	}
}

#endif // IOS || TVOS
