// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012, 2014 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;

namespace AVFoundation {

	[Native]
	// NSInteger - AVAudioSettings.h
	public enum AVAudioQuality : long {
		Min = 0,
		Low = 0x20,
		Medium = 0x40,
		High = 0x60,
		Max = 0x7F
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVAssetExportSession.h
	public enum AVAssetExportSessionStatus : long {
		Unknown,
		Waiting,
		Exporting,
		Completed,
		Failed,
		Cancelled
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVAssetReader.h
	public enum AVAssetReaderStatus : long {
		Unknown = 0,
		Reading,
		Completed,
		Failed,
		Cancelled,
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVAssetWriter.h
	public enum AVAssetWriterStatus : long {
		Unknown = 0,
		Writing,
		Completed,
		Failed,
		Cancelled,
	}

	[NoTV, NoWatch]
	[Native]
	// NSInteger - AVCaptureSession.h
	public enum AVCaptureVideoOrientation : long {
		Portrait = 1,
		PortraitUpsideDown,
		LandscapeRight,
		LandscapeLeft,
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureFlashMode : long {
		Off, On, Auto
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureTorchMode : long {
		Off, On, Auto
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureFocusMode : long {
		Locked, AutoFocus, ContinuousAutoFocus,
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureDevicePosition : long {
		Unspecified = 0,
		Back = 1,
		Front = 2
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureExposureMode : long {
		Locked, AutoExpose, ContinuousAutoExposure, [iOS (8,0)] Custom
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureWhiteBalanceMode : long {
		Locked, AutoWhiteBalance, ContinuousAutoWhiteBalance
	}

#if !MONOMAC || !XAMCORE_4_0
	[Flags]
	[Native]
	[Deprecated (PlatformName.iOS, 6, 0)]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionInterruptionFlags : ulong {
		ShouldResume = 1
	}
#endif

	// Populated in NSError.Code, an NSInteger
	// anonymous enum - AVError.h
	[Watch (6,0)]
	[Native]
	public enum AVError : long {
		Unknown = -11800,
		OutOfMemory = -11801,
		SessionNotRunning = -11803,
		DeviceAlreadyUsedByAnotherSession = -11804,
		NoDataCaptured = -11805,
		SessionConfigurationChanged = -11806,
		DiskFull = -11807,
		DeviceWasDisconnected = -11808,
		MediaChanged = -11809,
		MaximumDurationReached = -11810,
		MaximumFileSizeReached = -11811,
		MediaDiscontinuity = -11812,
		MaximumNumberOfSamplesForFileFormatReached = -11813,
		DeviceNotConnected = -11814,
		DeviceInUseByAnotherApplication = -11815,
		DeviceLockedForConfigurationByAnotherProcess = -11817,
		SessionWasInterrupted = -11818,
		MediaServicesWereReset = -11819,
		ExportFailed = -11820,
		DecodeFailed = -11821,
		InvalidSourceMedia = -11822,
		FileAlreadyExists = -11823,
		CompositionTrackSegmentsNotContiguous = -11824,
		InvalidCompositionTrackSegmentDuration = -11825,
		InvalidCompositionTrackSegmentSourceStartTime = -11826,
		InvalidCompositionTrackSegmentSourceDuration = -11827,
		FormatNotRecognized = -11828,
		FailedToParse = -11829, // Should have been FileFailedToParse
		MaximumStillImageCaptureRequestsExceeded = -11830,
		ContentIsProtected = -11831,
		NoImageAtTime = -11832,
		DecoderNotFound = -11833,
		EncoderNotFound = -11834,
		ContentIsNotAuthorized = -11835,
		ApplicationIsNotAuthorized = -11836,
		DeviceIsNotAvailableInBackground = -11837,
		OperationNotSupportedForAsset = -11838,
		DecoderTemporarilyUnavailable = -11839,
		EncoderTemporarilyUnavailable = -11840,
		InvalidVideoComposition = -11841,
		ReferenceForbiddenByReferencePolicy = -11842,
		InvalidOutputURLPathExtension = -11843,
		ScreenCaptureFailed = -11844,
		DisplayWasDisabled = -11845,
		TorchLevelUnavailable = -11846,
		OperationInterrupted = -11847,
		IncompatibleAsset = -11848,
		FailedToLoadMediaData = -11849,
		ServerIncorrectlyConfigured = -11850,
		ApplicationIsNotAuthorizedToUseDevice = -11852,
			
		FailedToParse2 = -11853,
		FileTypeDoesNotSupportSampleReferences = -11854,
		UndecodableMediaData = -11855,

		AirPlayControllerRequiresInternet = -11856,
		AirPlayReceiverRequiresInternet = -11857,

		[iOS (9,0), Mac (10,11)]
		VideoCompositorFailed = -11858,

#if !MONOMAC
		[iOS (9,0)]
		RecordingAlreadyInProgress = -11859,
#endif
		[NoWatch, iOS (10,0), TV (10,0), Mac (10,12)]
		UnsupportedOutputSettings = -11861,
		[NoWatch, iOS (10,0), TV (10,0), Mac (10,12)]
		OperationNotAllowed = -11862,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		ContentIsUnavailable = -11863,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		FormatUnsupported = -11864,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		MalformedDepth = -11865,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		ContentNotUpdated = -11866,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		NoLongerPlayable = -11867,
		[NoWatch, iOS (11,0), TV (11,0), Mac (10,13)]
		NoCompatibleAlternatesForExternalDisplay = -11868,
		[NoWatch, iOS (11,2), TV (11,2), Mac (10,13,2)]
		NoSourceTrack = -11869,
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVPlayer.h
	public enum AVPlayerActionAtItemEnd : long {
		Advance,
		Pause,
		None
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVPlayerItem.h
	public enum AVPlayerItemStatus : long {
		Unknown, ReadyToPlay, Failed
	}

#if !MONOMAC || !XAMCORE_4_0
	[NoTV]
	[Flags]
	[Native]
	[Deprecated (PlatformName.iOS, 6, 0)]
	// declared as AVAudioSessionSetActiveOptions (NSUInteger) - AVAudioSession.h
	public enum AVAudioSessionFlags : ulong {
		NotifyOthersOnDeactivation = 1
	}
#endif

	[Watch (6,0)]
	[Native]
	// NSInteger - AVAsynchronousKeyValueLoading.h
	public enum AVKeyValueStatus : long {
		Unknown, Loading, Loaded, Failed, Cancelled
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVPlayer.h
	public enum AVPlayerStatus : long {
		Unknown,
		ReadyToPlay,
		Failed
	}

	[Watch (6,0)]
	[Native]
	// NSUInteger - AVAsset.h
	public enum AVAssetReferenceRestrictions : ulong {
		ForbidNone = 0,
		ForbidRemoteReferenceToLocal = (1 << 0),
		ForbidLocalReferenceToRemote = (1 << 1),
		ForbidCrossSiteReference     = (1 << 2),
		ForbidLocalReferenceToLocal  = (1 << 3),
		ForbidAll = 0xFFFF,
	}

	[Watch (6,0)]
	[Native]
	// NSInteger - AVAssetImageGenerator.h
	public enum AVAssetImageGeneratorResult : long {
		Succeeded, Failed, Cancelled
	}

#if !XAMCORE_3_0 || MONOMAC
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureDeviceTransportControlsPlaybackMode : long {
		NotPlaying, Playing
	}

	[NoTV, NoWatch]
	[Native]
	// NSInteger - AVCaptureSession.h
	public enum AVVideoFieldMode : long {
		Both, TopOnly, BottomOnly, Deinterlace
	}
#endif

#if !MONOMAC || !XAMCORE_4_0
	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionInterruptionOptions : ulong {
		ShouldResume = 1
	}

	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionSetActiveOptions : ulong {
		NotifyOthersOnDeactivation = 1
	}

	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionPortOverride : ulong {
		None = 0,
		[NoTV]
		Speaker = 0x73706b72 // 'spkr'
	}

	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionRouteChangeReason : ulong {
		Unknown,
		NewDeviceAvailable,
		OldDeviceUnavailable,
		CategoryChange,
		Override,
		WakeFromSleep = 6,
		NoSuitableRouteForCategory = 7,
		RouteConfigurationChange = 8
	}

	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionCategoryOptions : ulong {
		MixWithOthers = 1,
		DuckOthers = 2,
		[NoTV]
		AllowBluetooth = 4,
		[NoTV]
		DefaultToSpeaker = 8,

		[iOS (9,0)]
		InterruptSpokenAudioAndMixWithOthers = 17,
		[NoWatch, iOS (10,0), TV (10,0)]
		AllowBluetoothA2DP = 32,
		[NoWatch, iOS (10,0), TV (10,0)]
		AllowAirPlay = 64,
	}

	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionInterruptionType : ulong  {
		Ended, Began
	}

	[Native]
	// NSInteger - AVAudioSession.h
	// typedef CF_ENUM(NSInteger, AVAudioSessionErrorCode) -> CoreAudioTypes.framework/Headers/AudioSessionTypes.h
	public enum AVAudioSessionErrorCode : long {
		None = 0,
		MediaServicesFailed = 0x6D737276, // 'msrv'
		IsBusy = 0x21616374, // '!act'
		IncompatibleCategory = 0x21636174, // 'cat'
		CannotInterruptOthers = 0x21696e74, // 'int'
		MissingEntitlement = 0x656e743f, // 'ent?'
		SiriIsRecording = 0x73697269, // 'siri'
		CannotStartPlaying = 0x21706c61, // '!pla'
		CannotStartRecording = 0x21726563, // '!rec'
		BadParam = -50,
		InsufficientPriority = 0x21707269, // '!pri'
#if !XAMCORE_4_0
		[Obsolete ("Use 'ResourceNotAvailable' instead.")]
		CodeResourceNotAvailable = 0x21726573,
#endif
		ResourceNotAvailable = 0x21726573, // '!res'
		Unspecified = 0x77686174, // 'what'
		ExpiredSession = 0x21736573, // '!ses'
		SessionNotActive = 0x696e6163, // 'inac'
	}
#endif

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureAutoFocusRangeRestriction : long {
		None = 0,
		Near = 1,
		Far = 2
	}

	// Convenience enum for native strings (defined in AVAudioSettings.h)
	public enum AVAudioBitRateStrategy : int {
		Constant,
		LongTermAverage,
		VariableConstrained,
		Variable
	}

	// Convenience enum for native strings (defined in AVAudioSettings.h)
	public enum AVSampleRateConverterAlgorithm : int {
		Normal,
		Mastering
	}

	[NoWatch]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVAuthorizationStatus : long {
		NotDetermined, Restricted, Denied, Authorized
	}

#if !MONOMAC || !XAMCORE_4_0
	[iOS (7,0)]
	[Native]
	// NSInteger - AVSpeechSynthesis.h
	public enum AVSpeechBoundary : long {
		Immediate,
		Word
	}
#endif

	[iOS (8,0)]
	[Native]
	public enum AVAudioCommonFormat : ulong {
		Other = 0,
		PCMFloat32 = 1,
		PCMFloat64 = 2,
		PCMInt16 = 3,
		PCMInt32 = 4
	}

	[Native]
	public enum AVAudio3DMixingRenderingAlgorithm : long {
		EqualPowerPanning = 0,
		SphericalHead = 1,
		HRTF = 2,
		SoundField = 3,
		StereoPassThrough = 5
	}

#if !MONOMAC || !XAMCORE_4_0
	[NoTV, Watch (5,0)]
	[Native]
	public enum AVAudioSessionRecordPermission : ulong {
		Undetermined = 1970168948 /*'undt'*/,
		Denied = 1684369017 /*'deny'*/,
		Granted = 1735552628 /*'grnt'*/
	}

	[Native]
	public enum AVAudioSessionSilenceSecondaryAudioHintType : ulong {
		Begin = 1,
		End = 0
	}
#endif

	[Flags]
	[Native]
	public enum AVAudioPlayerNodeBufferOptions : ulong {
		Loops            = 0x01,
		Interrupts       = 0x02,
		InterruptsAtLoop = 0x04
	}

	[Native]
	public enum AVAudioUnitEQFilterType : long {
		Parametric = 0,
		LowPass = 1,
		HighPass = 2,
		ResonantLowPass = 3,
		ResonantHighPass = 4,
		BandPass = 5,
		BandStop = 6,
		LowShelf = 7,
		HighShelf = 8,
		ResonantLowShelf = 9,
		ResonantHighShelf = 10
	}
	
	[Native]
	public enum AVAudioUnitReverbPreset : long {
		SmallRoom = 0,
		MediumRoom = 1,
		LargeRoom = 2,
		MediumHall = 3,
		LargeHall = 4,
		Plate = 5,
		MediumChamber = 6,
		LargeChamber = 7,
		Cathedral = 8,
		LargeRoom2 = 9,
		MediumHall2 = 10,
		MediumHall3 = 11,
		LargeHall2 = 12
	}

	[Native]
	public enum AVAudioUnitDistortionPreset : long {
		DrumsBitBrush = 0,
		DrumsBufferBeats = 1,
		DrumsLoFi = 2,
		MultiBrokenSpeaker = 3,
		MultiCellphoneConcert = 4,
		MultiDecimated1 = 5,
		MultiDecimated2 = 6,
		MultiDecimated3 = 7,
		MultiDecimated4 = 8,
		MultiDistortedFunk = 9,
		MultiDistortedCubed = 10,
		MultiDistortedSquared = 11,
		MultiEcho1 = 12,
		MultiEcho2 = 13,
		MultiEchoTight1 = 14,
		MultiEchoTight2 = 15,
		MultiEverythingIsBroken = 16,
		SpeechAlienChatter = 17,
		SpeechCosmicInterference = 18,
		SpeechGoldenPi = 19,
		SpeechRadioTower = 20,
		SpeechWaves = 21
	}

	[Native]
	public enum AVAudioEnvironmentDistanceAttenuationModel : long {
		Exponential = 1,
		Inverse = 2,
		Linear = 3
	}

	[TV (10,2), Watch (6,0)]
	[Native]
	public enum AVQueuedSampleBufferRenderingStatus : long {
		Unknown, Rendering, Failed
	}

	[NoWatch]
	[NoTV]
	[Native]
	public enum AVCaptureVideoStabilizationMode : long {
		Off,
		Standard,
		Cinematic,
		[iOS (13,0)]
		CinematicExtended,
		Auto = -1
	}

#if XAMCORE_4_0
	[NoMac]
#endif
	[NoTV, NoWatch, iOS (8,0)]
	[Native]
	public enum AVCaptureAutoFocusSystem : long {
		None,
		ContrastDetection,
		PhaseDetection
	}

#if !MONOMAC
	[NoTV, NoWatch]
	[iOS (9,0)]
	[Native]
	public enum AVCaptureSessionInterruptionReason : long {
		VideoDeviceNotAvailableInBackground = 1,
		AudioDeviceInUseByAnotherClient = 2,
		VideoDeviceInUseByAnotherClient = 3,
		VideoDeviceNotAvailableWithMultipleForegroundApps = 4,
		[iOS (11, 1)]
		VideoDeviceNotAvailableDueToSystemPressure = 5,
	}

#endif

	[iOS (9,0), Mac (10,15)]
	[Native]
	public enum AVSpeechSynthesisVoiceQuality : long {
		Default = 1,
		Enhanced
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum AVAudioConverterPrimeMethod : long {
		Pre = 0,
		Normal = 1,
		None = 2
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum AVAudioConverterInputStatus : long {
		HaveData = 0,
		NoDataNow = 1,
		EndOfStream = 2
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum AVAudioConverterOutputStatus : long {
		HaveData = 0,
		InputRanDry = 1,
		EndOfStream = 2,
		Error = 3
	}

	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum AVMusicSequenceLoadOptions : ulong {
		PreserveTracks = 0,
		ChannelsToTracks = (1 << 0)
	}
			
	[NoTV]
	[iOS (13,0)]
	[Watch (6,0)]
	[Mac (10,11)]
	[Native]
	public enum AVMovieWritingOptions : ulong
	{
		AddMovieHeaderToDestination = 0,
		TruncateDestinationToMovieHeaderOnly = (1 << 0)
	}

	[NoiOS][NoTV][NoWatch]
	[Native]
	public enum AVContentAuthorizationStatus : long {
		Unknown,
		Completed,
		Cancelled,
		TimedOut,
		Busy,
		NotAvailable,
		NotPossible,
	}

	[NoiOS][NoTV][NoWatch]
	[Mac (10,10)]
	[Native]
	public enum AVSampleBufferRequestDirection : long {
		Forward = 1,
		None = 0,
		Reverse = -1,
	}

	[NoiOS][NoTV][NoWatch]
	[Mac (10,10)]
	[Native]
	public enum AVSampleBufferRequestMode : long {
		Immediate,
		Scheduled,
	}

	[NoTV, NoWatch, Mac (10,15), iOS (10,0)]
	[Native]
	public enum AVCaptureColorSpace : long
	{
		Srgb = 0,
		P3D65 = 1
	}

	[iOS (8,0)]
	[Native]
	public enum AVMusicTrackLoopCount : long
	{
		Forever = -1
	}

	[Watch (6,0), iOS (10,0), TV (10,0), Mac (10,12)]
	[Native]
	public enum AVPlayerTimeControlStatus : long
	{
		Paused,
		WaitingToPlayAtSpecifiedRate,
		Playing
	}

	[NoWatch, NoTV, iOS (10,0), Mac (10,15)]
	[Native]
	public enum AVAudioSessionIOType : long
	{
		NotSpecified = 0,
		Aggregated = 1,
	}

	[Watch (6,0), iOS (10,0), TV (10,0), Mac (10,12)]
	[Native]
	public enum AVPlayerLooperStatus : long {
		Unknown,
		Ready,
		Failed,
		Cancelled
	}
	
	[NoiOS, TV (10,0), NoWatch, NoMac]
	[Native]
	public enum AVContentProposalAction : long {
		Accept,
		Reject,
		Defer
	}

	[NoiOS, TV (10,0), NoWatch, NoMac]
	[Native]
	public enum AVPlayerViewControllerSkippingBehavior : long {
		Default = 0,
		SkipItem
	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[Native]
	public enum AVContentKeyRequestStatus : long {
		Requesting,
		Received,
		Renewed,
		Retried,
		Cancelled,
		Failed
	}

	[TV (10, 2), Mac (10, 12, 4), iOS (10, 3), NoWatch]
	public enum AVContentKeyRequestRetryReason {
		[Field ("AVContentKeyRequestRetryReasonTimedOut")]
		TimedOut,
		[Field ("AVContentKeyRequestRetryReasonReceivedResponseWithExpiredLease")]
		ReceivedResponseWithExpiredLease,
		[Field ("AVContentKeyRequestRetryReasonReceivedObsoleteContentKey")]
		ReceivedObsoleteContentKey,
	}

	[TV (10, 2), Mac (10, 12, 4), iOS (10, 3), NoWatch]
	public enum AVContentKeySystem {
		[Field ("AVContentKeySystemFairPlayStreaming")]
		FairPlayStreaming = 0,

		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		[Field ("AVContentKeySystemClearKey")]
		ClearKey = 1,

		[Obsolete ("Use 'AVContentKeySystem.SystemClearKey' instead.")]
		AVContentKeySystemClearKey = ClearKey,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVContentKeySystemAuthorizationToken")]
		AuthorizationToken = 2,
	}

	// Convience enum for native string values 
	[NoWatch]
	public enum AVAssetExportSessionPreset {
		[Mac(10,11)]
		[Field ("AVAssetExportPresetLowQuality")]
		LowQuality = 0, // AVAssetExportPresetLowQuality
		[Mac(10,11)]
		[Field ("AVAssetExportPresetMediumQuality")]
		MediumQuality = 1, // AVAssetExportPresetMediumQuality
		[Mac(10,11)]
		[Field ("AVAssetExportPresetHighestQuality")]
		HighestQuality = 2, // AVAssetExportPresetHighestQuality
		[Field ("AVAssetExportPreset640x480")]
		Preset640x480 = 3, // AVAssetExportPreset640x480
		[Field ("AVAssetExportPreset960x540")]
		Preset960x540 = 4, // AVAssetExportPreset960x540
		[Field ("AVAssetExportPreset1280x720")]
		Preset1280x720 = 5, // AVAssetExportPreset1280x720
		[Field ("AVAssetExportPreset1920x1080")]
		Preset1920x1080 = 6, // AVAssetExportPreset1920x1080

		[iOS (9,0)]
		[Mac (10,10)]
		[Field ("AVAssetExportPreset3840x2160")]
		Preset3840x2160 = 7, // AVAssetExportPreset3840x2160

		[Field ("AVAssetExportPresetAppleM4A")]
		AppleM4A = 8, // AVAssetExportPresetAppleM4A
		[Field ("AVAssetExportPresetPassthrough")]
		Passthrough = 9, // AVAssetExportPresetPassthrough

		[Obsolete ("Use 'AVOutputSettingsPreset.PresetHevc1920x1080' instead.")]
		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVOutputSettingsPresetHEVC1920x1080")]
		PresetHevc1920x1080 = 11, 

		[Obsolete ("Use 'AVOutputSettingsPreset.PresetHevc3840x2160' instead.")]
		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVOutputSettingsPresetHEVC3840x2160")]
		PresetHevc3840x2160 = 12,
	}

	[TV (9,0), NoWatch, Mac (10,9), iOS (7,0)]
	public enum AVOutputSettingsPreset {
		[Field ("AVOutputSettingsPreset640x480")]
		Preset640x480 = 0,

		[Field ("AVOutputSettingsPreset960x540")]
		Preset960x540 = 1,

		[Field ("AVOutputSettingsPreset1280x720")]
		Preset1280x720 = 2,

		[Field ("AVOutputSettingsPreset1920x1080")]
		Preset1920x1080 = 3,

		[Mac (10,10), iOS (9,0)]
		[Field ("AVOutputSettingsPreset3840x2160")]
		Preset3840x2160 = 4,

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("AVOutputSettingsPresetHEVC1920x1080")]
		PresetHevc1920x1080 = 11, // we added the wrong value in the export enum, we use the same so that they can be swap

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("AVOutputSettingsPresetHEVC3840x2160")]
		PresetHevc3840x2160 = 12, // we added the wrong value in the export enum, we use the same so that they can be swap

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVOutputSettingsPresetHEVC1920x1080WithAlpha")]
		PresetHevc1920x1080WithAlpha = 13,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVOutputSettingsPresetHEVC3840x2160WithAlpha")]
		PresetHevc3840x2160WithAlpha = 14,
	}

	[TV (11,0), NoWatch, iOS (11,0)]
	[Native]
	public enum AVDepthDataAccuracy : long {
		Relative = 0,
		Absolute = 1,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum AVAudioEngineManualRenderingMode : long {
		Offline = 0,
		Realtime = 1
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum AVAudioEngineManualRenderingStatus : long {
		Error = -1,
		Success = 0,
		InsufficientDataFromInputNode = 1,
		CannotDoInCurrentContext = 2
	}

	[TV (11,0), Watch (5,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum AVAudioSessionRouteSharingPolicy : ulong {
		Default = 0,
		LongForm = 1,
		Independent = 2
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum AVAudioPlayerNodeCompletionCallbackType : long {
		Consumed = 0,
		Rendered = 1,
		PlayedBack = 2
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	public enum AVAudioEngineManualRenderingError
	{
		InvalidMode = -80800,
		Initialized = -80801,
		NotRunning = -80802,
	}

	[NoTV, iOS (11,0)]
	[NoWatch]
	[Native]
	public enum AVCaptureLensStabilizationStatus : long
	{
		Unsupported = 0,
		Off = 1,
		Active = 2,
		OutOfRange = 3,
		Unavailable = 4
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Native]
	public enum AVCaptureOutputDataDroppedReason : long
	{
		None = 0,
		LateData = 1,
		OutOfBuffers = 2,
		Discontinuity = 3,
	}

	[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
	public enum AVVideoApertureMode {
		[Field ("AVVideoApertureModeCleanAperture")]
		CleanAperture = 0,

		[Field ("AVVideoApertureModeProductionAperture")]
		ProductionAperture = 1,

		[Field ("AVVideoApertureModeEncodedPixels")]
		EncodedPixels = 2,
	}

	[NoWatch, NoTV, NoMac, iOS (11, 0)]
	public enum AVAssetDownloadedAssetEvictionPriority {
		[Field ("AVAssetDownloadedAssetEvictionPriorityDefault")]
		Default = 0,

		[Field ("AVAssetDownloadedAssetEvictionPriorityImportant")]
		Important = 1,
	}

	[NoWatch, iOS (11, 0), TV (11,0), Mac (10,13)]
	public enum AVAssetWriterInputMediaDataLocation {
		[Field ("AVAssetWriterInputMediaDataLocationInterleavedWithMainMediaData")]
		InterleavedWithMainMediaData = 0,

		[Field ("AVAssetWriterInputMediaDataLocationBeforeMainMediaDataNotInterleaved")]
		BeforeMainMediaDataNotInterleaved = 1,
	}

	[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
	public enum AVVideoCodecType {
		[Field ("AVVideoCodecTypeH264")]
		H264 = 0,
		
		[Field ("AVVideoCodecTypeJPEG")]
		Jpeg = 1,

		[Field ("AVVideoCodecTypeAppleProRes422")]
		AppleProRes422 = 3,

		[Field ("AVVideoCodecTypeAppleProRes4444")]
		AppleProRes4444 = 4,

		[Field ("AVVideoCodecTypeHEVC")]
		Hevc = 5,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVVideoCodecTypeAppleProRes422HQ")]
		AppleProRes422HQ = 6,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVVideoCodecTypeAppleProRes422LT")]
		AppleProRes422LT = 7,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVVideoCodecTypeAppleProRes422Proxy")]
		AppleProRes422Proxy = 8,

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVVideoCodecTypeHEVCWithAlpha")]
		HevcWithAlpha = 9,
	}

	[TV (11,0), NoWatch, iOS (11,0)]
	[Native]
	public enum AVDepthDataQuality : long {
		Low = 0,
		High = 1
	}

	[NoWatch, NoTV, NoMac, iOS (11,1)]
	[Flags]
	[Native] 
	public enum AVCaptureSystemPressureFactors : ulong
	{
		None = 0,
		SystemTemperature = (1 << 0),
		PeakPower = (1 << 1),
		DepthModuleTemperature = (1 << 2)
	}

	[TV (11,2), NoWatch, NoMac, iOS (11,2)]
	[Native]
	[Flags]
	public enum AVPlayerHdrMode : long {
		Hlg = 0x1,
		Hdr10 = 0x2,
		DolbyVision = 0x4,
	}

	[Flags]
	[Native]
	[Watch (5,0), TV (12,0), iOS (12,0), Mac (10,15)]
	public enum AVAudioSessionActivationOptions : ulong
	{
		None = 0x0,
	}

	[Native]
	public enum AVAudioSessionPromptStyle : ulong
	{
		None = 0x6e6f6e65, // 1852796517 - 'none'
		Short = 0x73687274, // 1936224884 - 'shrt'
		Normal = 0x6e726d6c, //1852992876 - 'nrml'
	}

	[Watch (6,0), iOS (13,0), TV (13,0), Mac (10,15)]
	[Native]
	public enum AVSpeechSynthesisVoiceGender : long {
		Unspecified,
		Male,
		Female,
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[Native]
	public enum AVCapturePhotoQualityPrioritization : long {
		Speed = 1,
		Balanced = 2,
		Quality = 3,
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	public enum AVAudioEnvironmentOutputType : long {
		Auto = 0,
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	public enum AVAudio3DMixingSourceMode : long {
		SpatializeIfMono = 0,
		Bypass = 1,
		PointSource = 2,
		AmbienceBed = 3,
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	public enum AVAudio3DMixingPointSourceInHeadMode : long {
		Mono = 0,
		Bypass = 1,
	}

}
