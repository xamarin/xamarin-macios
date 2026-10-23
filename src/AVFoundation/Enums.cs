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
using System.ComponentModel;

namespace AVFoundation {

	/// <summary>Represents sample rate conversion quality used by audio encoder.</summary>
	/// <remarks>
	///     </remarks>
	[Native]
	// NSInteger - AVAudioSettings.h
	public enum AVAudioQuality : long {
		/// <summary>Minimum quality.</summary>
		Min = 0,
		/// <summary>Low quality encoding.</summary>
		Low = 0x20,
		/// <summary>Medium quality.</summary>
		Medium = 0x40,
		/// <summary>High quality encoding.</summary>
		High = 0x60,
		/// <summary>Maximum quality.</summary>
		Max = 0x7F,
	}

	/// <summary>Status flag of the export operation.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAssetExportSession.h
	public enum AVAssetExportSessionStatus : long {
		/// <summary>Unknown error or state.</summary>
		Unknown,
		/// <summary>The session is waiting to export more data.</summary>
		Waiting,
		/// <summary>Asset export in progress.</summary>
		Exporting,
		/// <summary>The export operation was completed.</summary>
		Completed,
		/// <summary>Asset export failed.</summary>
		Failed,
		/// <summary>The operation was cancelled.</summary>
		Cancelled,
	}

	/// <summary>An enumeration whose values specify the <see cref="AVFoundation.AVAssetReader" />'s status.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAssetReader.h
	public enum AVAssetReaderStatus : long {
		/// <summary>Indicates that reading has not yet begun.</summary>
		Unknown = 0,
		/// <summary>More sample buffers are ready for output.</summary>
		Reading,
		/// <summary>Reading completed successfully.</summary>
		Completed,
		/// <summary>Reading failed.</summary>
		Failed,
		/// <summary>Reading was cancelled.</summary>
		Cancelled,
	}

	/// <summary>An enumeration whose values represent the status of an <see cref="AVFoundation.AVAssetWriter" /> object.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAssetWriter.h
	public enum AVAssetWriterStatus : long {
		/// <summary>The state of the <see cref="AVFoundation.AVAssetWriter" /> cannot be determined.</summary>
		Unknown = 0,
		/// <summary>Assets are currently being written.</summary>
		Writing,
		/// <summary>The asset writing completed successfully.</summary>
		Completed,
		/// <summary>The asset writing failed while writing.</summary>
		Failed,
		/// <summary>The asset writing has been cancelled.</summary>
		Cancelled,
	}

	/// <summary>Video capture orientation.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureSession.h
	public enum AVCaptureVideoOrientation : long {
		/// <summary>Portrait</summary>
		Portrait = 1,
		/// <summary>Portrait, upside down.</summary>
		PortraitUpsideDown,
		/// <summary>Landscape, turned right.</summary>
		LandscapeRight,
		/// <summary>Landscape, turned left.</summary>
		LandscapeLeft,
	}

	/// <summary>Flash mode.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureFlashMode : long {
		/// <summary>Never use the flash.</summary>
		Off,
		/// <summary>Always use the flash.</summary>
		On,
		/// <summary>Automatic.</summary>
		Auto,
	}

	/// <summary>The capture device torch mode.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureTorchMode : long {
		/// <summary>Never uses the torch.</summary>
		Off,
		/// <summary>Always uses the torch.</summary>
		On,
		/// <summary>Uses the torch based on the available light measured.</summary>
		Auto,
	}

	/// <summary>Auto focus states.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureFocusMode : long {
		/// <summary>Focus that will not change automatically.</summary>
		Locked,
		/// <summary>Normal autofocus.</summary>
		AutoFocus,
		/// <summary>Autofocus that attempts to track the subject.</summary>
		ContinuousAutoFocus,
	}

	/// <summary>An enumeration whose values specify the position of a <see cref="AVFoundation.AVCaptureDevice" />.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureDevicePosition : long {
		/// <summary>The capturing hardware's location is unknown.</summary>
		Unspecified = 0,
		/// <summary>The capturing hardware is on the back of the device.</summary>
		Back = 1,
		/// <summary>The capturing hardware is on the front of the device.</summary>
		Front = 2,
	}

	/// <summary>An enumeration whose values specify options for varying exposure modes during capture.</summary>
	/// <remarks>
	///       <para />
	///     </remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureExposureMode : long {
		/// <summary>Exposure setting is locked.</summary>
		Locked,
		/// <summary>The camera performs auto expose.</summary>
		AutoExpose,
		/// <summary>Performs auto-expose and adjusts the setting continously. </summary>
		ContinuousAutoExposure,
		/// <summary>Exposure is limited by the <see cref="AVFoundation.AVCaptureDevice.ISO" /> and <see cref="AVFoundation.AVCaptureDevice.ExposureDuration" /> properties.</summary>
		[MacCatalyst (14, 0)]
		Custom,
	}

	/// <summary>Capture white balance mode.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureWhiteBalanceMode : long {
		/// <summary>Auto white balance has been locked.</summary>
		Locked,
		/// <summary>Automatic white balance, set it once.</summary>
		AutoWhiteBalance,
		/// <summary>Contimuously evaluate and set the white balance.</summary>
		ContinuousAutoWhiteBalance,
	}

	// Populated in NSError.Code, an NSInteger
	// anonymous enum - AVError.h
	/// <summary>An enumeration whose values define various audiovisual errors.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVError : long {
		/// <summary>Indicates that an unknown error occurred.</summary>
		Unknown = -11800,
		/// <summary>Indicates that system memory became full.</summary>
		OutOfMemory = -11801,
		/// <summary>Indicates that no data was received because the session was not running.</summary>
		SessionNotRunning = -11803,
		/// <summary>Indicates that the device was in use by another session.</summary>
		DeviceAlreadyUsedByAnotherSession = -11804,
		/// <summary>Indicates that no data was sent to the recording session.</summary>
		NoDataCaptured = -11805,
		/// <summary>Indicates that the recording session configuration was changed.</summary>
		SessionConfigurationChanged = -11806,
		/// <summary>Indicates that non-volatile storage was full.</summary>
		DiskFull = -11807,
		/// <summary>Indicates that the device was not connected.</summary>
		DeviceWasDisconnected = -11808,
		/// <summary>Indicates that the recording source media changed.</summary>
		MediaChanged = -11809,
		/// <summary>Indicates that the maximum allowable recording duration was reached.</summary>
		MaximumDurationReached = -11810,
		/// <summary>Indicates that the maximum allowable output file size was reached.</summary>
		MaximumFileSizeReached = -11811,
		/// <summary>Indicates that the source data was interrupted.</summary>
		MediaDiscontinuity = -11812,
		/// <summary>Indicates that the maximum number of samples was reached for the output file.</summary>
		MaximumNumberOfSamplesForFileFormatReached = -11813,
		/// <summary>Indicates that the device was not connected.</summary>
		DeviceNotConnected = -11814,
		/// <summary>Indicates that the device was in use by another application.</summary>
		DeviceInUseByAnotherApplication = -11815,
		/// <summary>Indicates that the device is locked for an update or for other configuration.</summary>
		DeviceLockedForConfigurationByAnotherProcess = -11817,
		/// <summary>Indicates that the recording sesion was interrupted.</summary>
		SessionWasInterrupted = -11818,
		/// <summary>Indicates that media services were reset.</summary>
		MediaServicesWereReset = -11819,
		/// <summary>Indicates that data export failed.</summary>
		ExportFailed = -11820,
		/// <summary>Indicates that the media data could not be decoded.</summary>
		DecodeFailed = -11821,
		/// <summary>Indicates that the source media was not valid.</summary>
		InvalidSourceMedia = -11822,
		/// <summary>Indicates that data could not be written because the output file already exists.</summary>
		FileAlreadyExists = -11823,
		/// <summary>Indicates that the source media contained gaps and could not be loaded.</summary>
		CompositionTrackSegmentsNotContiguous = -11824,
		/// <summary>Indicates that the source media's play duration on the device was not valid.</summary>
		InvalidCompositionTrackSegmentDuration = -11825,
		/// <summary>Indicates that the source media could not be opened because a track start time was not valid.</summary>
		InvalidCompositionTrackSegmentSourceStartTime = -11826,
		/// <summary>Indicates that the source media had no duration set.</summary>
		InvalidCompositionTrackSegmentSourceDuration = -11827,
		/// <summary>Indicates that the media was in an urecognizable format and could therefore not be opened.</summary>
		FormatNotRecognized = -11828,
		/// <summary>Indicates that the data could not be parsed.</summary>
		FailedToParse = -11829, // Should have been FileFailedToParse
		/// <summary>Indicates that the maximum number of still images was reached for the output file.</summary>
		MaximumStillImageCaptureRequestsExceeded = -11830,
		/// <summary>Indicates that the app is not authorized to play the content.</summary>
		ContentIsProtected = -11831,
		/// <summary>Indicates that playback failed because no image was present at a specified time.</summary>
		NoImageAtTime = -11832,
		/// <summary>Indicates that a decoder for the media data could not be found.</summary>
		DecoderNotFound = -11833,
		/// <summary>Indicates that a required data encoder was missing.</summary>
		EncoderNotFound = -11834,
		/// <summary>Indicates that the user is not authorized to play the content.</summary>
		ContentIsNotAuthorized = -11835,
		/// <summary>The application is not authorized.</summary>
		ApplicationIsNotAuthorized = -11836,
		/// <summary>Indicates that the device is not available while the app is backgrounded.</summary>
		DeviceIsNotAvailableInBackground = -11837,
		/// <summary>Indicates that an operation that was not supported on a specific asset or asset type was attempted.</summary>
		OperationNotSupportedForAsset = -11838,
		/// <summary>Indicates that the decoder for the media data was unavailable.</summary>
		DecoderTemporarilyUnavailable = -11839,
		/// <summary>Indicates that a required data encoder was unavailable.</summary>
		EncoderTemporarilyUnavailable = -11840,
		/// <summary>Indicates that a video composition operation was not valid.</summary>
		InvalidVideoComposition = -11841,
		/// <summary>Indicates that an attempt was made to follow a reference for which authorization was lacking.</summary>
		ReferenceForbiddenByReferencePolicy = -11842,
		/// <summary>Indicates that the ouptut path contained an invalid extension.</summary>
		InvalidOutputURLPathExtension = -11843,
		/// <summary>Indicates that the screen could not be captured.</summary>
		ScreenCaptureFailed = -11844,
		/// <summary>Indicates that the display was disabled.</summary>
		DisplayWasDisabled = -11845,
		/// <summary>Indicates that a torch level was unavailable, possibly due to overheating.</summary>
		TorchLevelUnavailable = -11846,
		/// <summary>Indicates that an operation was interrupted.</summary>
		OperationInterrupted = -11847,
		/// <summary>Indicates that the media format is not supported by the device.</summary>
		IncompatibleAsset = -11848,
		/// <summary>Indicates that the media data could not be loaded due to missing required data.</summary>
		FailedToLoadMediaData = -11849,
		/// <summary>Indicates that the media server is not correctly configured.</summary>
		ServerIncorrectlyConfigured = -11850,
		/// <summary>Indicates that the application was not authorized on the device.</summary>
		ApplicationIsNotAuthorizedToUseDevice = -11852,

		/// <summary>Indicates that the data could not be parsed.</summary>
		FailedToParse2 = -11853,
		/// <summary>Indicates that the file type does not support sample references.</summary>
		FileTypeDoesNotSupportSampleReferences = -11854,
		/// <summary>Indicates that the media data was corrupted.</summary>
		UndecodableMediaData = -11855,

		/// <summary>Indicates that playback failed because the controller lacked necessary internet access.</summary>
		AirPlayControllerRequiresInternet = -11856,
		/// <summary>Indicates that playback failed because the receiver lacked necessary internet access.</summary>
		AirPlayReceiverRequiresInternet = -11857,

		/// <summary>Indicates that a video compositor failed.</summary>
		VideoCompositorFailed = -11858,

		/// <summary>Indicates that an attempt was made to start a recording while another was in progress.</summary>
		RecordingAlreadyInProgress = -11859,
		CreateContentKeyRequestFailed = -11860,
		/// <summary>Indicates that the output settings were not supported.</summary>
		UnsupportedOutputSettings = -11861,
		/// <summary>Indicates that a disallowed operation was attempted.</summary>
		OperationNotAllowed = -11862,
		/// <summary>To be added.</summary>
		ContentIsUnavailable = -11863,
		/// <summary>To be added.</summary>
		FormatUnsupported = -11864,
		/// <summary>To be added.</summary>
		MalformedDepth = -11865,
		/// <summary>To be added.</summary>
		ContentNotUpdated = -11866,
		/// <summary>To be added.</summary>
		NoLongerPlayable = -11867,
		/// <summary>To be added.</summary>
		NoCompatibleAlternatesForExternalDisplay = -11868,
		/// <summary>To be added.</summary>
		NoSourceTrack = -11869,
		ExternalPlaybackNotSupportedForAsset = -11870,
		OperationNotSupportedForPreset = -11871,
		SessionHardwareCostOverage = -11872,
		UnsupportedDeviceActiveFormat = -11873,
		IncorrectlyConfigured = -11875,
		SegmentStartedWithNonSyncSample = -11876,
		RosettaNotInstalled = -11877,
		OperationCancelled = -11878,
		RequestCancelled = -11879,

		InvalidSampleCursor = -11880,
		FailedToLoadSampleData = -11881,
		AirPlayReceiverTemporarilyUnavailable = -11882,
		EncodeFailed = -11883,
		SandboxExtensionDenied = -11884,
		ToneMappingFailed = -11885,
		MediaExtensionDisabled = -11886,
		MediaExtensionConflict = -11887,
		ContentKeyRequestPlaybackDestinationDoesNotSupportDeviceIdentifierRandomization = -11888,
		ContentKeyInvalid = -11889,
		NoSmartFramingsEnabled = -11890,
		AutoWhiteBalanceNotLocked = -11891,
		FollowExternalSyncDeviceTimedOut = -11892,
		FollowExternalSyncFailed = -11894,
		ExternalSyncDeviceFrequencyHigherThanSpecified = -11895,
		ExternalSyncDeviceFrequencyLowerThanSpecified = -11896,
		/// <summary>Indicates that there is insufficient storage space to replenish Pro Video Storage.</summary>
		NotEnoughSpaceForProVideoStorageReplenishment = -11897,
	}

	/// <summary>An enumeration whose values specify the behavior of the player when it finishes playing.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVPlayer.h
	public enum AVPlayerActionAtItemEnd : long {
		/// <summary>Indicates the player should advance to the next item, if it exists.</summary>
		Advance,
		/// <summary>The player should pause.</summary>
		Pause,
		/// <summary>The player should do nothing.</summary>
		None,
	}

	/// <summary>An enumeration whose values specify the status of a <see cref="AVFoundation.AVPlayerItem" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVPlayerItem.h
	public enum AVPlayerItemStatus : long {
		/// <summary>The status of the item is not known.</summary>
		Unknown,
		/// <summary>The item is ready to play.</summary>
		ReadyToPlay,
		/// <summary>The item could not be played.</summary>
		Failed,
	}

	/// <summary>An enumeration whose values specify the load status of a given property.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAsynchronousKeyValueLoading.h
	public enum AVKeyValueStatus : long {
		/// <summary>The load attempt's state is unknown.</summary>
		Unknown,
		/// <summary>The load attempt is ongoing.</summary>
		Loading,
		/// <summary>The load attempt succeeded.</summary>
		Loaded,
		/// <summary>The load attempt failed.</summary>
		Failed,
		/// <summary>The load attempt was cancelled.</summary>
		Cancelled,
	}

	/// <summary>An enumeration whose values indicate the status of an <see cref="AVFoundation.AVPlayer" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVPlayer.h
	public enum AVPlayerStatus : long {
		/// <summary>The status of the player is unknown, generally because media has not been loaded.</summary>
		Unknown,
		/// <summary>The player is ready to play the <see cref="AVFoundation.AVPlayerItem" />.</summary>
		ReadyToPlay,
		/// <summary>The player can no longer play the <see cref="AVFoundation.AVPlayerItem" />.</summary>
		Failed,
	}

	/// <summary>An enumeration whose values define restrictions relating to a <see cref="AVFoundation.AVAsset" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	// NSUInteger - AVAsset.h
	public enum AVAssetReferenceRestrictions : ulong {
		/// <summary>There are no restrictions on references to media data.</summary>
		ForbidNone = 0,
		/// <summary>Remote assets cannot require a local media resource (e.g., an HTTP URL cannot require a client-side file).</summary>
		ForbidRemoteReferenceToLocal = (1 << 0),
		/// <summary>Local asset references to remote media are not allowed.</summary>
		ForbidLocalReferenceToRemote = (1 << 1),
		/// <summary>Remote asset references to a different site are not allowed.</summary>
		ForbidCrossSiteReference = (1 << 2),
		/// <summary>Local asset references to local media outside the container are not allowed.</summary>
		ForbidLocalReferenceToLocal = (1 << 3),
		/// <summary>Only references to media data stored within the asset's container are allowed.</summary>
		ForbidAll = 0xFFFF,
	}

	/// <summary>An enumeration whose values indicate the result of image generation.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAssetImageGenerator.h
	public enum AVAssetImageGeneratorResult : long {
		/// <summary>Image generation succeeded.</summary>
		Succeeded,
		/// <summary>Image generation failed.</summary>
		Failed,
		/// <summary>Image generation was cancelled.</summary>
		Cancelled,
	}

	/// <summary>An enumeration whose values specify whether a <see cref="AVFoundation.AVCaptureDevice" /> is playing or not.</summary>
	[NoiOS]
	[Unavailable (PlatformName.MacCatalyst)]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureDeviceTransportControlsPlaybackMode : long {
		/// <summary>To be added.</summary>
		NotPlaying,
		/// <summary>To be added.</summary>
		Playing,
	}

	/// <summary>An enumeration whose values specify how interlaced fields should be dealt with.</summary>
	///     
	///     <!-- TODO: This should be used with AVCaptureConnection.VideoFieldMode property, but that doesn't appear to exist  -->
	[NoiOS]
	[Unavailable (PlatformName.MacCatalyst)]
	[NoTV]
	[Native]
	// NSInteger - AVCaptureSession.h
	public enum AVVideoFieldMode : long {
		/// <summary>Both top and bottom interlaced video fields should be passed through.</summary>
		Both,
		/// <summary>Only the top interlaced video field should be passed through.</summary>
		TopOnly,
		/// <summary>Only the bottom interlaced video field should be passed through.</summary>
		BottomOnly,
		/// <summary>The top and bottom fields should be de-interlaced.</summary>
		Deinterlace,
	}

	/// <summary>An enumeration whose values specify optional audio behaviors.</summary>
	/// <remarks>To be added.</remarks>
	// Not deprecated in headers on macOS
	[Deprecated (PlatformName.iOS, 27, 0, message: "Use 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[Deprecated (PlatformName.TvOS, 27, 0, message: "Use 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionInterruptionOptions : ulong {
		/// <summary>Interupption has ended and the app can resumes its audio session.</summary>
		ShouldResume = 1,
	}

	/// <summary>An enumeration whose values define whether, after an audio session deactivates, previously interrupted audio sessions should or should not re-activate.</summary>
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionSetActiveOptions : ulong {
		/// <summary>Notifies other audio sessions that they can resume when this session deactivates.</summary>
		NotifyOthersOnDeactivation = 1,
	}

	/// <summary>An enumeration whose values define whether an audio session should override the audio port and output via the built-in speaker.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionPortOverride : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		[NoTV]
		[NoMac] // Removed in Xcode 12 GM
		[MacCatalyst (13, 1)]
		Speaker = 0x73706b72, // 'spkr'
	}

	/// <summary>An enumeration whose values specify why an audio route changed.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionRouteChangeReason : ulong {
		/// <summary>To be added.</summary>
		Unknown,
		/// <summary>To be added.</summary>
		NewDeviceAvailable,
		/// <summary>To be added.</summary>
		OldDeviceUnavailable,
		/// <summary>To be added.</summary>
		CategoryChange,
		/// <summary>To be added.</summary>
		Override,
		/// <summary>To be added.</summary>
		WakeFromSleep = 6,
		/// <summary>To be added.</summary>
		NoSuitableRouteForCategory = 7,
		/// <summary>To be added.</summary>
		RouteConfigurationChange = 8,
	}

	/// <summary>An enumeration whose values specify optional audio behaviors.</summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionCategoryOptions : ulong {
		/// <summary>Indicates that this session's output should mix in with output from other sessions.</summary>
		MixWithOthers = 1,
		/// <summary>Indicates that output volume from other audio sessions will be reduced when this one is playing.</summary>
		DuckOthers = 2,
		/// <summary>Indicates that Bluetooth is allowed.</summary>
		[NoMac] // Removed in Xcode 12 GM
		[NoTV]
		[MacCatalyst (13, 1)]
		AllowBluetooth = 4,
		/// <summary>Indicates that the system output should be used by default.</summary>
		[NoMac] // Removed in Xcode 12 GM
		[NoTV]
		[MacCatalyst (13, 1)]
		DefaultToSpeaker = 8,

		/// <summary>Indicates that the output is typically spoken word output that should pause output from other sessions.</summary>
		[NoMac] // Removed in Xcode 12 GM
		[MacCatalyst (13, 1)]
		InterruptSpokenAudioAndMixWithOthers = 17,
		/// <summary>Indicates that Bluetooth A2DP is allowed.</summary>
		[NoMac] // Removed in Xcode 12 GM
		[MacCatalyst (13, 1)]
		AllowBluetoothA2DP = 32,
		/// <summary>Indicates that AirPlay is allowed.</summary>
		[NoMac] // Removed in Xcode 12 GM
		[MacCatalyst (13, 1)]
		AllowAirPlay = 64,
		[NoMac]
		[NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		OverrideMutedMicrophoneInterruption = 128,
		[iOS (26, 0), NoTV, NoMacCatalyst, NoMac]
		BluetoothHighQualityRecording = 1uL << 19,
		[iOS (26, 2), NoTV, MacCatalyst (26, 2), NoMac]
		FarFieldInput = 1uL << 18,
	}

	/// <summary>An enumeration whose values specify the beginning and ending of an audio interruption.</summary>
	/// <remarks>To be added.</remarks>
	// Not deprecated in headers on macOS
	[Deprecated (PlatformName.iOS, 27, 0, message: "Use 'AVAudioSession.DidBecomeInactiveNotification' and 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[Deprecated (PlatformName.TvOS, 27, 0, message: "Use 'AVAudioSession.DidBecomeInactiveNotification' and 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use 'AVAudioSession.DidBecomeInactiveNotification' and 'AVAudioSession.ResumptionRecommendationNotification' instead.")]
	[MacCatalyst (13, 1)]
	[Native]
	// NSUInteger - AVAudioSession.h
	public enum AVAudioSessionInterruptionType : ulong {
		/// <summary>To be added.</summary>
		Ended,
		/// <summary>To be added.</summary>
		Began,
	}

	/// <summary>An enumeration whose values specify various errors relating to <see cref="AVFoundation.AVAudioSession" />s.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVAudioSession.h
	// typedef CF_ENUM(NSInteger, AVAudioSessionErrorCode) -> CoreAudioTypes.framework/Headers/AudioSessionTypes.h
	public enum AVAudioSessionErrorCode : long {
		/// <summary>Indicates that no error occurred. Success.</summary>
		None = 0,
		/// <summary>Indicates that Media Services has failed.</summary>
		MediaServicesFailed = 0x6D737276, // 'msrv'
		/// <summary>Indicates that an attempt was made to inactivate a busy session.</summary>
		IsBusy = 0x21616374, // '!act'
		/// <summary>Indicates that an operation that is incompatible with the current category was attempted.</summary>
		IncompatibleCategory = 0x21636174, // 'cat'
		/// <summary>Indicates that a session with nonmixable audio tried to activate while the app was in the background.</summary>
		CannotInterruptOthers = 0x21696e74, // 'int'
		/// <summary>Indicates that a required entitlement was not present.</summary>
		MissingEntitlement = 0x656e743f, // 'ent?'
		/// <summary>Indicates that Siri was recording when the operation was attempted.</summary>
		SiriIsRecording = 0x73697269, // 'siri'
		/// <summary>Indicates that audio playback was attempted when playback was not allowed.</summary>
		CannotStartPlaying = 0x21706c61, // '!pla'
		/// <summary>Indicates that audio recording was attempted and failed.</summary>
		CannotStartRecording = 0x21726563, // '!rec'
		/// <summary>Indicates that the app tried to set a parameter to a disallowed value.</summary>
		BadParam = -50,
		/// <summary>Indicates that another app with higher priority preempted the operation.</summary>
		InsufficientPriority = 0x21707269, // '!pri'
		/// <summary>Indicates that a required resource, such as an audio input, is not available on the device.</summary>
		ResourceNotAvailable = 0x21726573, // '!res'
		/// <summary>Indicates that an unspecified error occurred.</summary>
		Unspecified = 0x77686174, // 'what'
		ExpiredSession = 0x21736573, // '!ses'
		SessionNotActive = 0x696e6163, // 'inac'
	}

	/// <summary>An enumeration whose values specify hints to autofocus. Used with <see cref="AVCaptureDevice.AutoFocusRangeRestriction" />.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVCaptureAutoFocusRangeRestriction : long {
		/// <summary>Autofocus should try to focus on objects at any distance.</summary>
		None = 0,
		/// <summary>Autofocus should try to focus on closer objects.</summary>
		Near = 1,
		/// <summary>Autofocus should try to focus on distant objects.</summary>
		Far = 2,
	}

	// Convenience enum for native strings (defined in AVAudioSettings.h)
	/// <summary>An enumeration whose values specify the type of audio bit-rate. Used with <see cref="AVFoundation.AudioSettings.BitRateStrategy" /></summary>
	/// <remarks>To be added.</remarks>
	public enum AVAudioBitRateStrategy : int {
		/// <summary>To be added.</summary>
		Constant,
		/// <summary>To be added.</summary>
		LongTermAverage,
		/// <summary>To be added.</summary>
		VariableConstrained,
		/// <summary>To be added.</summary>
		Variable,
	}

	// Convenience enum for native strings (defined in AVAudioSettings.h)
	/// <summary>An enumeration whose values specify valid rate-converstion algorithms. Used with <see cref="AudioSettings.SampleRateConverterAlgorithm" />.</summary>
	/// <remarks>To be added.</remarks>
	public enum AVSampleRateConverterAlgorithm : int {
		/// <summary>To be added.</summary>
		Normal,
		/// <summary>To be added.</summary>
		Mastering,
	}

	/// <summary>An enumeration whose values specify whether a <see cref="AVFoundation.AVCaptureDevice" /> has been authorized by the user for use. Used with <see cref="AVFoundation.AVCaptureDevice.GetAuthorizationStatus(Foundation.NSString)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	// NSInteger - AVCaptureDevice.h
	public enum AVAuthorizationStatus : long {
		/// <summary>The user has not interacted with the permissions dialog.</summary>
		NotDetermined,
		/// <summary>The user is denied access and may not change permission.</summary>
		Restricted,
		/// <summary>The user has denied access.</summary>
		Denied,
		/// <summary>The user has granted access.</summary>
		Authorized,
	}

	/// <summary>An enumeration whose values specify whether the <see cref="AVFoundation.AVSpeechSynthesizer" /> should pause or stop immediately or complete an entire word.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	// NSInteger - AVSpeechSynthesis.h
	public enum AVSpeechBoundary : long {
		/// <summary>The speech should stop or pause immediately.</summary>
		Immediate,
		/// <summary>The speech should stop or pause after the current word.</summary>
		Word,
	}

	/// <summary>Enumerates formats for audio data (see <see cref="AVFoundation.AVAudioFormat.CommonFormat" />).</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioCommonFormat : ulong {
		/// <summary>To be added.</summary>
		Other = 0,
		/// <summary>To be added.</summary>
		PCMFloat32 = 1,
		/// <summary>To be added.</summary>
		PCMFloat64 = 2,
		/// <summary>To be added.</summary>
		PCMInt16 = 3,
		/// <summary>To be added.</summary>
		PCMInt32 = 4,
	}

	/// <summary>Enumerates valid 3D audio-rendering algorithms.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum AVAudio3DMixingRenderingAlgorithm : long {
		/// <summary>Pans the mixer bus into a stereo field.</summary>
		EqualPowerPanning = 0,
		/// <summary>An algorithm best used for headphones; lower quality but lower processor-intensity than <see cref="AVFoundation.AVAudio3DMixingRenderingAlgorithm.HRTF" />.</summary>
		SphericalHead = 1,
		/// <summary>A processor-intensive Head-Related Transfer Function algorithm best used for emulating 3D space with headphones. (See also <see cref="AVFoundation.AVAudio3DMixingRenderingAlgorithm.SphericalHead" />)</summary>
		HRTF = 2,
		/// <summary>Renders on all output channels, but weights towards an origin. Best used for ambient sounds. </summary>
		SoundField = 3,
		/// <summary>No localization is performed on the input data.</summary>
		StereoPassThrough = 5,
		[MacCatalyst (13, 1)]
		HrtfHQ = 6,
		[MacCatalyst (13, 1)]
		Auto = 7,
	}

#if XAMCORE_5_0
	[NoTV, NoMac]
#endif
	/// <summary>Enumerates valid permissions for <see cref="AVFoundation.AVAudioSession" />.</summary>
	/// <remarks>To be added.</remarks>
	[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'AVAudioApplicationRecordPermission' instead.")]
	[Deprecated (PlatformName.TvOS, 17, 0, message: "Use 'AVAudioApplicationRecordPermission' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'AVAudioApplicationRecordPermission' instead.")]
	[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'AVAudioApplicationRecordPermission' instead.")]
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioSessionRecordPermission : ulong {
		/// <summary>The user has neither nor denied permission, generally because they have not yet interacted with the permissions dialog.</summary>
		Undetermined = 1970168948 /*'undt'*/,
		/// <summary>The user denied permission.</summary>
		Denied = 1684369017 /*'deny'*/,
		/// <summary>The user granted permission.</summary>
		Granted = 1735552628 /*'grnt'*/,
	}

	/// <summary>Enumerates the valid values for <see cref="AVFoundation.AVAudioSessionSecondaryAudioHintEventArgs.Hint" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioSessionSilenceSecondaryAudioHintType : ulong {
		/// <summary>Another app's primary audio has started.</summary>
		Begin = 1,
		/// <summary>Another app's primary audio has ended.</summary>
		End = 0,
	}

	/// <summary>Flagging enumeration whose values are valid options in calls to <see cref="AVFoundation.AVAudioPlayerNode.ScheduleBufferAsync(AVFoundation.AVAudioPcmBuffer,AVFoundation.AVAudioTime,AVFoundation.AVAudioPlayerNodeBufferOptions,AVFoundation.AVAudioPlayerNodeCompletionCallbackType)" /></summary>
	/// <remarks>To be added.</remarks>
	[Flags]
	[Native]
	public enum AVAudioPlayerNodeBufferOptions : ulong {
		/// <summary>The buffer should loop indefinitely.</summary>
		Loops = 0x01,
		/// <summary>The buffer should interrupt any other buffer that is already playing.</summary>
		Interrupts = 0x02,
		/// <summary>The buffer should interrupt any other buffer at its loop point.</summary>
		InterruptsAtLoop = 0x04,
	}

	/// <summary>Filter types. Used with the <see cref="AVFoundation.AVAudioUnitEQFilterParameters.FilterType" /> property.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum AVAudioUnitEQFilterType : long {
		/// <summary>Parametric filter based on Butterworth analog prototype. Must have <c>frequency</c> for center, <c>bandwidth</c>, and <c>gain</c></summary>
		Parametric = 0,
		/// <summary>To be added.</summary>
		LowPass = 1,
		/// <summary>To be added.</summary>
		HighPass = 2,
		/// <summary>Low pass filter with resonance. Must have <c>frequency</c> (-3dB cutoff) and <c>bandwidth</c>.</summary>
		ResonantLowPass = 3,
		/// <summary>High pass filter with resonance. Must have <c>frequency</c> (-3dB cutoff) and <c>bandwidth</c>.</summary>
		ResonantHighPass = 4,
		/// <summary>Band pass filter. Must specify <c>frequency</c> and <c>bandwidth</c>.</summary>
		BandPass = 5,
		/// <summary>Band stop / notch filter. Must specify <c>frequency</c> and <c>bandwidth</c>.</summary>
		BandStop = 6,
		/// <summary>To be added.</summary>
		LowShelf = 7,
		/// <summary>To be added.</summary>
		HighShelf = 8,
		/// <summary>Low shelf filter with resonance. Must have <c>frequency</c>, <c>bandwidth</c>, and <c>gain</c>.</summary>
		ResonantLowShelf = 9,
		/// <summary>High shelf filter with resonance. Must have <c>frequency</c>, <c>bandwidth</c>, and <c>gain</c>.</summary>
		ResonantHighShelf = 10,
	}

	/// <summary>Enumerates constants describing the reverb presets.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum AVAudioUnitReverbPreset : long {
		/// <summary>To be added.</summary>
		SmallRoom = 0,
		/// <summary>To be added.</summary>
		MediumRoom = 1,
		/// <summary>To be added.</summary>
		LargeRoom = 2,
		/// <summary>To be added.</summary>
		MediumHall = 3,
		/// <summary>To be added.</summary>
		LargeHall = 4,
		/// <summary>To be added.</summary>
		Plate = 5,
		/// <summary>To be added.</summary>
		MediumChamber = 6,
		/// <summary>To be added.</summary>
		LargeChamber = 7,
		/// <summary>To be added.</summary>
		Cathedral = 8,
		/// <summary>To be added.</summary>
		LargeRoom2 = 9,
		/// <summary>To be added.</summary>
		MediumHall2 = 10,
		/// <summary>To be added.</summary>
		MediumHall3 = 11,
		/// <summary>To be added.</summary>
		LargeHall2 = 12,
		/// <summary>A reverb preset that simulates a general outdoor environment.</summary>
		[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		OutdoorGeneral = 24,
	}

	/// <summary>Enumerates valid values that can be passed to <see cref="AVFoundation.AVAudioUnitDistortion.LoadFactoryPreset(AVFoundation.AVAudioUnitDistortionPreset)" />.</summary>
	/// <remarks>To be added.</remarks>
	[Native]
	public enum AVAudioUnitDistortionPreset : long {
		/// <summary>To be added.</summary>
		DrumsBitBrush = 0,
		/// <summary>To be added.</summary>
		DrumsBufferBeats = 1,
		/// <summary>To be added.</summary>
		DrumsLoFi = 2,
		/// <summary>To be added.</summary>
		MultiBrokenSpeaker = 3,
		/// <summary>To be added.</summary>
		MultiCellphoneConcert = 4,
		/// <summary>To be added.</summary>
		MultiDecimated1 = 5,
		/// <summary>To be added.</summary>
		MultiDecimated2 = 6,
		/// <summary>To be added.</summary>
		MultiDecimated3 = 7,
		/// <summary>To be added.</summary>
		MultiDecimated4 = 8,
		/// <summary>To be added.</summary>
		MultiDistortedFunk = 9,
		/// <summary>To be added.</summary>
		MultiDistortedCubed = 10,
		/// <summary>To be added.</summary>
		MultiDistortedSquared = 11,
		/// <summary>To be added.</summary>
		MultiEcho1 = 12,
		/// <summary>To be added.</summary>
		MultiEcho2 = 13,
		/// <summary>To be added.</summary>
		MultiEchoTight1 = 14,
		/// <summary>To be added.</summary>
		MultiEchoTight2 = 15,
		/// <summary>To be added.</summary>
		MultiEverythingIsBroken = 16,
		/// <summary>To be added.</summary>
		SpeechAlienChatter = 17,
		/// <summary>To be added.</summary>
		SpeechCosmicInterference = 18,
		/// <summary>To be added.</summary>
		SpeechGoldenPi = 19,
		/// <summary>To be added.</summary>
		SpeechRadioTower = 20,
		/// <summary>To be added.</summary>
		SpeechWaves = 21,
	}

	/// <include file="../../docs/api/AVFoundation/AVAudioEnvironmentDistanceAttenuationModel.xml" path="/Documentation/Docs[@DocId='T:AVFoundation.AVAudioEnvironmentDistanceAttenuationModel']/*" />
	[Native]
	public enum AVAudioEnvironmentDistanceAttenuationModel : long {
		/// <summary>Gain = (Distance / ReferenceDistance)^(-RolloffFactor)</summary>
		Exponential = 1,
		/// <summary>Gain = ReferenceDistance / (ReferenceDistance + RolloffFactor * (Distance - ReferenceDistance))</summary>
		Inverse = 2,
		/// <summary>Gain = (1 - RolloffFactor * (Distance - ReferenceDistance) / (MaximumDistance - ReferenceDistance))</summary>
		Linear = 3,
	}

	/// <summary>Enumerates possible values of the <see cref="AVSampleBufferAudioRenderer.Status" />, <see cref="AVSampleBufferDisplayLayer.Status" /> and <see cref="AVSampleBufferVideoRenderer.Status" /> fields.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVQueuedSampleBufferRenderingStatus : long {
		/// <summary>No sample buffers have been enqueued yet.</summary>
		Unknown,
		/// <summary>At least one sample buffer is queued for rendering.</summary>
		Rendering,
		/// <summary>The player failed during or preparing for rendering.</summary>
		Failed,
	}

	/// <summary>Enumerates types of video stabilization supported by the device's format.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCaptureVideoStabilizationMode : long {
		/// <summary>No video stabilization should be applied.</summary>
		Off,
		/// <summary>The default video stabilization algorithm should be used.</summary>
		Standard = 1,
		/// <summary>The "cinematic stabilization" algorithm should be used.</summary>
		Cinematic = 2,
		[MacCatalyst (14, 0)]
		CinematicExtended = 3,
		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0), NoMac]
		PreviewOptimized = 4,
		[iOS (18, 0), MacCatalyst (18, 0), TV (18, 0), NoMac]
		CinematicExtendedEnhanced = 5,
		[iOS (26, 0), NoMacCatalyst, NoTV, NoMac]
		LowLatency = 6,
		/// <summary>The device determines the stabilization mode.</summary>
		Auto = -1,
	}

	/// <summary>Enumerates constants relating to the device's autofocus system.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCaptureAutoFocusSystem : long {
		/// <summary>Autofocus is not available.</summary>
		None,
		/// <summary>Autofocus by way of contrast detection, which requires a focus scan.</summary>
		ContrastDetection,
		/// <summary>Autofocus by way of phase detection, which often can be done without a focus scan.</summary>
		PhaseDetection,
	}

	/// <summary>Enumerates ways that a capture session can be interrupted.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	[NoMac]
	public enum AVCaptureSessionInterruptionReason : long {
		/// <summary>Indicates that the app was sent to the background while capturing.</summary>
		VideoDeviceNotAvailableInBackground = 1,
		/// <summary>Indicates that another client began using the audio capture device.</summary>
		AudioDeviceInUseByAnotherClient = 2,
		/// <summary>Indicates that another client began using the video capture device.</summary>
		VideoDeviceInUseByAnotherClient = 3,
		/// <summary>Indicates that the app was displayed in Slide Over, Split View, or PIP mode, which would result in resource contention and degraded capture quality.</summary>
		VideoDeviceNotAvailableWithMultipleForegroundApps = 4,
		/// <summary>To be added.</summary>
		[MacCatalyst (14, 0)]
		VideoDeviceNotAvailableDueToSystemPressure = 5,
		[MacCatalyst (26, 0), TV (26, 0), NoMac, iOS (26, 0)]
		SensitiveContentMitigationActivated = 6,
	}

	/// <summary>Enumerates the quality of speech synthesis.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVSpeechSynthesisVoiceQuality : long {
		/// <summary>To be added.</summary>
		Default = 1,
		/// <summary>To be added.</summary>
		Enhanced = 2,
		[iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		Premium = 3,
	}

	/// <summary>Enumerates the priming strategy for <see cref="AVFoundation.AVAudioConverterPrimeInfo" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioConverterPrimeMethod : long {
		/// <summary>To be added.</summary>
		Pre = 0,
		/// <summary>To be added.</summary>
		Normal = 1,
		/// <summary>To be added.</summary>
		None = 2,
	}

	/// <summary>Enumerates the state of the input stream.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioConverterInputStatus : long {
		/// <summary>To be added.</summary>
		HaveData = 0,
		/// <summary>To be added.</summary>
		NoDataNow = 1,
		/// <summary>To be added.</summary>
		EndOfStream = 2,
	}

	/// <summary>Enumerates the state of the output stream during audio conversion.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioConverterOutputStatus : long {
		/// <summary>To be added.</summary>
		HaveData = 0,
		/// <summary>To be added.</summary>
		InputRanDry = 1,
		/// <summary>To be added.</summary>
		EndOfStream = 2,
		/// <summary>To be added.</summary>
		Error = 3,
	}

	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum AVMusicSequenceLoadOptions : ulong {
		/// <summary>To be added.</summary>
		PreserveTracks = 0,
		/// <summary>To be added.</summary>
		ChannelsToTracks = (1 << 0),
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum AVMovieWritingOptions : ulong {
		/// <summary>To be added.</summary>
		AddMovieHeaderToDestination = 0,
		/// <summary>To be added.</summary>
		TruncateDestinationToMovieHeaderOnly = (1 << 0),
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Native]
	public enum AVContentAuthorizationStatus : long {
		/// <summary>To be added.</summary>
		Unknown,
		/// <summary>To be added.</summary>
		Completed,
		/// <summary>To be added.</summary>
		Cancelled,
		/// <summary>To be added.</summary>
		TimedOut,
		/// <summary>To be added.</summary>
		Busy,
		/// <summary>To be added.</summary>
		NotAvailable,
		/// <summary>To be added.</summary>
		NotPossible,
	}

	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVSampleBufferRequestDirection : long {
		/// <summary>To be added.</summary>
		Forward = 1,
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Reverse = -1,
	}

	[iOS (15, 0)]
	[TV (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVSampleBufferRequestMode : long {
		/// <summary>To be added.</summary>
		Immediate,
		/// <summary>To be added.</summary>
		Scheduled,
		[MacCatalyst (13, 1)]
		Opportunistic = 2,
	}

	/// <summary>Enumerates video capture color spaces.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCaptureColorSpace : long {
		/// <summary>Indicates an RGB color space.</summary>
		Srgb = 0,
		/// <summary>Indicates the DCI-P3 color space with a CIE D65 white point.</summary>
		P3D65 = 1,
		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[NoMac]
		HlgBT2020 = 2,
		[NoMac, NoiOS, NoMacCatalyst]
		AppleLog = 3,
		[NoMac, iOS (26, 0), MacCatalyst (26, 0), TV (26, 0)]
		AppleLog2 = 4,
	}

	/// <summary>Enumerates loop count limits.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVMusicTrackLoopCount : long {
		/// <summary>Loops the music track indefinitely.</summary>
		Forever = -1,
	}

	/// <summary>Enumerates allowable time <see cref="AVFoundation.AVPlayer.TimeControlStatus" /> values.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVPlayerTimeControlStatus : long {
		/// <summary>Indicates that the player is paused.</summary>
		Paused,
		/// <summary>Indicates that the player is waiting to play at a specified rate.</summary>
		WaitingToPlayAtSpecifiedRate,
		/// <summary>Indicates that the player is playing.</summary>
		Playing,
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioSessionIOType : long {
		/// <summary>To be added.</summary>
		NotSpecified = 0,
		/// <summary>To be added.</summary>
		Aggregated = 1,
	}

	/// <summary>Enumerates the states of an <see cref="AVFoundation.AVPlayerLooper" /> object.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVPlayerLooperStatus : long {
		/// <summary>The status of the looper is not currently available.</summary>
		Unknown,
		/// <summary>The looper is ready to play.</summary>
		Ready,
		/// <summary>Looping failed (see <see cref="AVFoundation.AVPlayerLooper.Error" />).</summary>
		Failed,
		/// <summary>Looping has been cancelled.</summary>
		Cancelled,
	}

	[NoiOS]
	[NoMac]
	[NoMacCatalyst]
	[Native]
	public enum AVContentProposalAction : long {
		Accept,
		Reject,
		Defer,
	}

	[NoiOS]
	[NoMac]
	[NoMacCatalyst]
	[Native]
	public enum AVPlayerViewControllerSkippingBehavior : long {
		Default = 0,
		SkipItem,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVContentKeyRequestStatus : long {
		/// <summary>To be added.</summary>
		Requesting,
		/// <summary>To be added.</summary>
		Received,
		/// <summary>To be added.</summary>
		Renewed,
		/// <summary>To be added.</summary>
		Retried,
		/// <summary>To be added.</summary>
		Cancelled,
		/// <summary>To be added.</summary>
		Failed,
	}

	[MacCatalyst (13, 1)]
	public enum AVContentKeyRequestRetryReason {
		/// <summary>To be added.</summary>
		[Field ("AVContentKeyRequestRetryReasonTimedOut")]
		TimedOut,
		/// <summary>To be added.</summary>
		[Field ("AVContentKeyRequestRetryReasonReceivedResponseWithExpiredLease")]
		ReceivedResponseWithExpiredLease,
		/// <summary>To be added.</summary>
		[Field ("AVContentKeyRequestRetryReasonReceivedObsoleteContentKey")]
		ReceivedObsoleteContentKey,
	}

	/// <summary>Enumerates delivery methods for content keys.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	public enum AVContentKeySystem {
		/// <summary>Indicates FairPlay.</summary>
		[Field ("AVContentKeySystemFairPlayStreaming")]
		FairPlayStreaming = 0,

		[MacCatalyst (13, 1)]
		[Field ("AVContentKeySystemClearKey")]
		ClearKey = 1,

		/// <summary>Indicates a clear system.</summary>
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'AVContentKeySystem.SystemClearKey' instead.")]
		AVContentKeySystemClearKey = ClearKey,

		[MacCatalyst (13, 1)]
		[Field ("AVContentKeySystemAuthorizationToken")]
		AuthorizationToken = 2,
	}

	// Convience enum for native string values 
	/// <summary>Enumerates presets for asset export sessions.</summary>
	[MacCatalyst (13, 1)]
	public enum AVAssetExportSessionPreset {
		/// <summary>Indicates a low quality QuickTime file.</summary>
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetLowQuality")]
		LowQuality = 0, // AVAssetExportPresetLowQuality

		/// <summary>Indicates a medium quality QuickTime file.</summary>
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetMediumQuality")]
		MediumQuality = 1, // AVAssetExportPresetMediumQuality

		/// <summary>Indicates the highest quality QuickTime file.</summary>
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHighestQuality")]
		HighestQuality = 2, // AVAssetExportPresetHighestQuality

		/// <summary>Indicates 640x480 output.</summary>
		[Field ("AVAssetExportPreset640x480")]
		Preset640x480 = 3, // AVAssetExportPreset640x480

		/// <summary>Indicates 960x540 output.</summary>
		[Field ("AVAssetExportPreset960x540")]
		Preset960x540 = 4, // AVAssetExportPreset960x540

		/// <summary>Indicates 1280x720 output.</summary>
		[Field ("AVAssetExportPreset1280x720")]
		Preset1280x720 = 5, // AVAssetExportPreset1280x720

		/// <summary>Indicates 1920x1080 output.</summary>
		[Field ("AVAssetExportPreset1920x1080")]
		Preset1920x1080 = 6, // AVAssetExportPreset1920x1080

		/// <summary>Indicates 3840x2160 output.</summary>
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPreset3840x2160")]
		Preset3840x2160 = 7, // AVAssetExportPreset3840x2160

		/// <summary>Indicates an audio-only m4a file with gapless playback data.</summary>
		[Field ("AVAssetExportPresetAppleM4A")]
		AppleM4A = 8, // AVAssetExportPresetAppleM4A

		/// <summary>Indicates a pass-through export, where possible.</summary>
		[Field ("AVAssetExportPresetPassthrough")]
		Passthrough = 9, // AVAssetExportPresetPassthrough

#if !XAMCORE_5_0
		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'AVOutputSettingsPreset.PresetHevc1920x1080' instead if you're looking for the 'AVOutputSettingsPresetHEVC1920x1080' constant, or 'Hevc1920x1080' if you're looking for the 'AVAssetExportPresetHEVC1920x1080 constant.")]
		[Field ("AVOutputSettingsPresetHEVC1920x1080")]
		PresetHevc1920x1080 = 11,

		[MacCatalyst (13, 1)]
		[Obsolete ("Use 'AVOutputSettingsPreset.PresetHevc3840x2160' instead if you're looking for the 'AVOutputSettingsPresetHEVC3840x2160' constant, or 'Hevc3840x2160' if you're looking for the 'AVAssetExportPresetHEVC3840x2160' constant.")]
		[Field ("AVOutputSettingsPresetHEVC3840x2160")]
		PresetHevc3840x2160 = 12,
#endif // !XAMCORE_5_0

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVCHighestQuality")]
		HevcHighestQuality,

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC3840x2160")]
		Hevc3840x2160,

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC1920x1080")]
		Hevc1920x1080,

		[NoTV, MacCatalyst (15, 0), iOS (15, 0)]
		[Field ("AVAssetExportPresetAppleProRes4444LPCM")]
		AppleProRes4444Lpcm,

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC1920x1080WithAlpha")]
		Hevc1920x1080WithAlpha,

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC3840x2160WithAlpha")]
		Hevc3840x2160WithAlpha,

		[NoTV, iOS (26, 0)]
		[MacCatalyst (26, 0)]
		[Field ("AVAssetExportPresetHEVC7680x4320")]
		Hevc7680x4320,

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVCHighestQualityWithAlpha")]
		HevcHighestQualityWithAlpha,

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("AVAssetExportPresetMVHEVC960x960")]
		MvHevc960x960,

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("AVAssetExportPresetMVHEVC1440x1440")]
		MvHevc1440x1440,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVAssetExportPresetHEVC4320x2160")]
		Hevc4320x2160,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVAssetExportPresetMVHEVC4320x4320")]
		MvHevc4320x4320,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVAssetExportPresetMVHEVC7680x7680")]
		MvHevc7680x7680,

	}

	[MacCatalyst (13, 1)]
	public enum AVOutputSettingsPreset {
		[Field ("AVOutputSettingsPreset640x480")]
		Preset640x480 = 0,

		[Field ("AVOutputSettingsPreset960x540")]
		Preset960x540 = 1,

		[Field ("AVOutputSettingsPreset1280x720")]
		Preset1280x720 = 2,

		[Field ("AVOutputSettingsPreset1920x1080")]
		Preset1920x1080 = 3,

		[MacCatalyst (13, 1)]
		[Field ("AVOutputSettingsPreset3840x2160")]
		Preset3840x2160 = 4,

		[MacCatalyst (13, 1)]
		[Field ("AVOutputSettingsPresetHEVC1920x1080")]
		PresetHevc1920x1080 = 11, // we added the wrong value in the export enum, we use the same so that they can be swap

		[MacCatalyst (13, 1)]
		[Field ("AVOutputSettingsPresetHEVC3840x2160")]
		PresetHevc3840x2160 = 12, // we added the wrong value in the export enum, we use the same so that they can be swap

		[MacCatalyst (13, 1)]
		[Field ("AVOutputSettingsPresetHEVC1920x1080WithAlpha")]
		PresetHevc1920x1080WithAlpha = 13,

		[MacCatalyst (13, 1)]
		[Field ("AVOutputSettingsPresetHEVC3840x2160WithAlpha")]
		PresetHevc3840x2160WithAlpha = 14,

		[NoTV]
		[iOS (26, 0)]
		[MacCatalyst (26, 0)]
		[Field ("AVOutputSettingsPresetHEVC7680x4320")]
		PresetHevc7680x4320 = 15,

		[TV (17, 0), MacCatalyst (17, 0), iOS (17, 0)]
		[Field ("AVOutputSettingsPresetMVHEVC960x960")]
		PresetMvHevc960x960 = 16,

		[TV (17, 0), MacCatalyst (17, 0), iOS (17, 0)]
		[Field ("AVOutputSettingsPresetMVHEVC1440x1440")]
		PresetMvHevc1440x1440 = 17,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVOutputSettingsPresetHEVC4320x2160")]
		Hevc4320x2160 = 18,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVOutputSettingsPresetMVHEVC4320x4320")]
		MvHevc4320x4320 = 19,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVOutputSettingsPresetMVHEVC7680x7680")]
		MvHevc7680x7680 = 20,
	}

	/// <summary>Enumerates depth data accuracy types.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Native]
	public enum AVDepthDataAccuracy : long {
		/// <summary>Indicates that the depth data indicate relative depths from depth to depth.</summary>
		Relative = 0,
		/// <summary>Indicates that the depth data indicate distances in the real world.</summary>
		Absolute = 1,
	}

	/// <summary>Enumerates whether manual rendering is done offline or under real-time constraints.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioEngineManualRenderingMode : long {
		/// <summary>To be added.</summary>
		Offline = 0,
		/// <summary>To be added.</summary>
		Realtime = 1,
	}

	/// <summary>Enumerates status of manual rendering.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioEngineManualRenderingStatus : long {
		/// <summary>To be added.</summary>
		Error = -1,
		/// <summary>To be added.</summary>
		Success = 0,
		/// <summary>To be added.</summary>
		InsufficientDataFromInputNode = 1,
		/// <summary>To be added.</summary>
		CannotDoInCurrentContext = 2,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioSessionRouteSharingPolicy : ulong {
		/// <summary>To be added.</summary>
		Default = 0,
#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'LongFormAudio' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'LongFormAudio' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'LongFormAudio' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 11, message: "Use 'LongFormAudio' instead.")]
		LongForm = 1,
#endif
		LongFormAudio = 1,
		/// <summary>To be added.</summary>
		Independent = 2,
		[NoTV]
		[NoMac]
		[MacCatalyst (14, 0)]
		LongFormVideo = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioPlayerNodeCompletionCallbackType : long {
		/// <summary>To be added.</summary>
		Consumed = 0,
		/// <summary>To be added.</summary>
		Rendered = 1,
		/// <summary>To be added.</summary>
		PlayedBack = 2,
	}

	/// <summary>Error codes for manual rendering errors.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	public enum AVAudioEngineManualRenderingError {
		/// <summary>To be added.</summary>
		InvalidMode = -80800,
		/// <summary>To be added.</summary>
		Initialized = -80801,
		/// <summary>To be added.</summary>
		NotRunning = -80802,
	}

	/// <summary>Enumerates states for physical image stabilization hardware.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCaptureLensStabilizationStatus : long {
		/// <summary>Indicates that stabilization hardware is not present.</summary>
		Unsupported = 0,
		/// <summary>Indicates that the stabilization hardware is off.</summary>
		Off = 1,
		/// <summary>Indicates that the stabilization hardware is active.</summary>
		Active = 2,
		/// <summary>Indicates that the stabilization hardware reached or exceeded the range that it can stabilize.</summary>
		OutOfRange = 3,
		/// <summary>Indicates that stabilization hardware is present but unavailable.</summary>
		Unavailable = 4,
	}

	/// <summary>Enumerates reasons for dropped capture data.</summary>
	/// <remarks>To be added.</remarks>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCaptureOutputDataDroppedReason : long {
		/// <summary>Indicates that no data was dropped.</summary>
		None = 0,
		/// <summary>Indicates that data was dropped because it were too late.</summary>
		LateData = 1,
		/// <summary>Indicates that buffer memory is full.</summary>
		OutOfBuffers = 2,
		/// <summary>Indicates that the device was too busy to capture data.</summary>
		Discontinuity = 3,
	}

	[MacCatalyst (13, 1)]
	public enum AVVideoApertureMode {
		/// <summary>To be added.</summary>
		[Field ("AVVideoApertureModeCleanAperture")]
		CleanAperture = 0,

		/// <summary>To be added.</summary>
		[Field ("AVVideoApertureModeProductionAperture")]
		ProductionAperture = 1,

		/// <summary>To be added.</summary>
		[Field ("AVVideoApertureModeEncodedPixels")]
		EncodedPixels = 2,
	}
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum AVAssetDownloadedAssetEvictionPriority {
		/// <summary>To be added.</summary>
		[Field ("AVAssetDownloadedAssetEvictionPriorityDefault")]
		Default = 0,

		/// <summary>To be added.</summary>
		[Field ("AVAssetDownloadedAssetEvictionPriorityImportant")]
		Important = 1,
	}

	[MacCatalyst (13, 1)]
	public enum AVAssetWriterInputMediaDataLocation {
		/// <summary>To be added.</summary>
		[Field ("AVAssetWriterInputMediaDataLocationInterleavedWithMainMediaData")]
		InterleavedWithMainMediaData = 0,

		/// <summary>To be added.</summary>
		[Field ("AVAssetWriterInputMediaDataLocationBeforeMainMediaDataNotInterleaved")]
		BeforeMainMediaDataNotInterleaved = 1,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("AVAssetWriterInputMediaDataLocationSparselyInterleavedWithMainMediaData")]
		SparselyInterleavedWithMainMediaData = 2,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	public enum AVAssetExportSessionResumptionFailureReason {
		[Field ("AVAssetExportSessionResumptionFailureReasonUnsupportedForPresetOnPlatform")]
		UnsupportedForPresetOnPlatform,

		[Field ("AVAssetExportSessionResumptionFailureReasonTemporaryDirectoryDoesNotExist")]
		TemporaryDirectoryDoesNotExist,

		[Field ("AVAssetExportSessionResumptionFailureReasonIncompatibleSessionSettings")]
		IncompatibleSessionSettings,

		[Field ("AVAssetExportSessionResumptionFailureReasonIncompatibleTemporaryDirectoryContents")]
		IncompatibleTemporaryDirectoryContents,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	public enum AVExternalStorageDeviceReasonNotRecommendedForCaptureUse {
		[Field ("AVExternalStorageDeviceReasonNotRecommendedForCaptureUseEncrypted")]
		Encrypted,

		[Field ("AVExternalStorageDeviceReasonNotRecommendedForCaptureUseUnsupportedFileSystem")]
		UnsupportedFileSystem,

		[Field ("AVExternalStorageDeviceReasonNotRecommendedForCaptureUseSlowWritingSpeed")]
		SlowWritingSpeed,

		[Field ("AVExternalStorageDeviceReasonNotRecommendedForCaptureUseUnknownWritingSpeed")]
		UnknownWritingSpeed,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	public enum AVProVideoStorageBusyReason {
		[Field ("AVProVideoStorageBusyReasonAdjustingCapacity")]
		AdjustingCapacity,

		[Field ("AVProVideoStorageBusyReasonReplenishing")]
		Replenishing,

		[Field ("AVProVideoStorageBusyReasonCapturing")]
		Capturing,
	}

	/// <summary>Identifies scene signals that can influence automatic exposure.</summary>
	[NoMac, iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
	[Flags]
	public enum AVCaptureDeviceExposureSignal {
		/// <summary>Subject motion that may require reducing motion blur.</summary>
		[Field ("AVCaptureDeviceExposureSignalSubjectMotion")]
		SubjectMotion = 1,

		/// <summary>Multiple faces that may require greater depth of field.</summary>
		[Field ("AVCaptureDeviceExposureSignalGroupPhoto")]
		GroupPhoto = 2,

		/// <summary>A document scene that may benefit from greater sharpness.</summary>
		[Field ("AVCaptureDeviceExposureSignalDocument")]
		Document = 4,

		/// <summary>Diffraction artifacts that may benefit from an aperture adjustment.</summary>
		[Field ("AVCaptureDeviceExposureSignalStarburst")]
		Starburst = 8,

		/// <summary>Artificial-light flicker that may require coordinating aperture and exposure duration.</summary>
		[Field ("AVCaptureDeviceExposureSignalFlicker")]
		Flicker = 16,
	}

	/// <summary>Constants for known video codecs.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (15, 0)]
	public enum AVVideoCodecType {
		/// <summary>To be added.</summary>
		[Field ("AVVideoCodecTypeH264")]
		H264 = 0,

		/// <summary>To be added.</summary>
		[Field ("AVVideoCodecTypeJPEG")]
		Jpeg = 1,

		/// <summary>To be added.</summary>
		[Field ("AVVideoCodecTypeAppleProRes422")]
		AppleProRes422 = 3,

		/// <summary>To be added.</summary>
		[Field ("AVVideoCodecTypeAppleProRes4444")]
		AppleProRes4444 = 4,

		/// <summary>To be added.</summary>
		[Field ("AVVideoCodecTypeHEVC")]
		Hevc = 5,

		[MacCatalyst (15, 0)]
		[Field ("AVVideoCodecTypeAppleProRes422HQ")]
		AppleProRes422HQ = 6,

		[MacCatalyst (15, 0)]
		[Field ("AVVideoCodecTypeAppleProRes422LT")]
		AppleProRes422LT = 7,

		[MacCatalyst (15, 0)]
		[Field ("AVVideoCodecTypeAppleProRes422Proxy")]
		AppleProRes422Proxy = 8,

		[MacCatalyst (15, 0)]
		[Field ("AVVideoCodecTypeHEVCWithAlpha")]
		HevcWithAlpha = 9,

		[TV (18, 0), MacCatalyst (18, 0), Mac (15, 0), iOS (18, 0)]
		[Field ("AVVideoCodecTypeAppleProRes4444XQ")]
		AppleProRes4444XQ = 10,

		[TV (18, 0), MacCatalyst (18, 0), Mac (15, 0), iOS (18, 0)]
		[Field ("AVVideoCodecTypeJPEGXL")]
		JpegXl = 11,

		[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("AVVideoCodecTypeAppleProResRAW")]
		AppleProResRaw = 12,

		[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("AVVideoCodecTypeAppleProResRAWHQ")]
		AppleProResRawHQ = 13,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Native]
	public enum AVDepthDataQuality : long {
		/// <summary>To be added.</summary>
		Low = 0,
		/// <summary>To be added.</summary>
		High = 1,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[NoMac]
	[Flags]
	[Native]
	public enum AVCaptureSystemPressureFactors : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		SystemTemperature = (1 << 0),
		/// <summary>To be added.</summary>
		PeakPower = (1 << 1),
		/// <summary>To be added.</summary>
		DepthModuleTemperature = (1 << 2),
		[iOS (17, 0), MacCatalyst (17, 0)]
		CameraTemperature = (1uL << 3),
		/// <summary>Indicates that the system is 30 seconds away from unexpected power off.</summary>
		[iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
		SystemStress = (1uL << 4),
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	[Deprecated (PlatformName.MacCatalyst, 26, 0, "Use 'AVPlayer.EligibleForHdrpPlayback' instead.")]
	[Deprecated (PlatformName.iOS, 26, 0, "Use 'AVPlayer.EligibleForHdrpPlayback' instead.")]
	[Deprecated (PlatformName.TvOS, 26, 0, "Use 'AVPlayer.EligibleForHdrpPlayback' instead.")]
	public enum AVPlayerHdrMode : long {
		/// <summary>To be added.</summary>
		Hlg = 0x1,
		/// <summary>To be added.</summary>
		Hdr10 = 0x2,
		/// <summary>To be added.</summary>
		DolbyVision = 0x4,
	}

	/// <summary>Specifies options for activating an audio session.</summary>
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum AVAudioSessionActivationOptions : ulong {
		/// <summary>No activation options are specified.</summary>
		None = 0x0,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[Flags]
	[Native]
	public enum AVAudioSessionDeactivationOptions : ulong {
		/// <summary>No options.</summary>
		None = 0,
		/// <summary>Notifies other apps that were interrupted that the interruption has ended and they may resume playback.</summary>
		[NoMac]
		NotifyOthersOnDeactivation = 1uL << 0,
	}

	[NoMac]
	[iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum AVAudioSessionDeactivationSource : long {
		/// <summary>The app requested deactivation.</summary>
		App = 1,
		/// <summary>The system deactivated the session.</summary>
		System = 2,
	}

	[NoMac]
	[iOS (27, 0), TV (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum AVAudioSessionResumptionRecommendation : long {
		/// <summary>The system recommends not resuming.</summary>
		ShouldNotResume = 0,
		/// <summary>The system recommends resuming.</summary>
		ShouldResume = 1,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	public enum AVAudioMixInputParametersTrackID : int {
		/// <summary>Indicates that the input parameters should be applied to the mix of all audio tracks rather than to a single specific audio track.</summary>
		TrackMix = 0,
	}

	[Native]
	public enum AVAudioSessionPromptStyle : ulong {
		/// <summary>To be added.</summary>
		None = 0x6e6f6e65, // 1852796517 - 'none'
		/// <summary>To be added.</summary>
		Short = 0x73687274, // 1936224884 - 'shrt'
		/// <summary>To be added.</summary>
		Normal = 0x6e726d6c, //1852992876 - 'nrml'
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVSpeechSynthesisVoiceGender : long {
		Unspecified,
		Male,
		Female,
	}
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[TV (17, 0)]
	[Native]
	public enum AVCapturePhotoQualityPrioritization : long {
		Speed = 1,
		Balanced = 2,
		Quality = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudioEnvironmentOutputType : long {
		Auto = 0,
		Headphones = 1,
		BuiltInSpeakers = 2,
		ExternalSpeakers = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudio3DMixingSourceMode : long {
		SpatializeIfMono = 0,
		Bypass = 1,
		PointSource = 2,
		AmbienceBed = 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum AVAudio3DMixingPointSourceInHeadMode : long {
		Mono = 0,
		Bypass = 1,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum AVAssetSegmentType : long {
		Initialization = 1,
		Separable = 2,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Flags]
	[Native]
	public enum AVAudioSpatializationFormats : ulong {
		None = 0,
		MonoAndStereo = 3,
		Multichannel = 4,
		MonoStereoAndMultichannel = 7,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum AVAudioStereoOrientation : long {
		None = 0,
		Portrait = 1,
		PortraitUpsideDown = 2,
		LandscapeRight = 3,
		LandscapeLeft = 4,
	}

	[TV (14, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum AVFileTypeProfile {
		[Field (null)]
		None = 0,
		[Field ("AVFileTypeProfileMPEG4AppleHLS")]
		Mpeg4AppleHls,
		[Field ("AVFileTypeProfileMPEG4CMAFCompliant")]
		Mpeg4CmafCompliant,
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVAudioRoutingArbitrationCategory : long {
		Playback = 0,
		PlayAndRecord = 1,
		PlayAndRecordVoice = 2,
	}

	[MacCatalyst (13, 1)]
	public enum AVContentKeyResponseDataType {
		FairPlayStreamingKeyResponseData,
		[MacCatalyst (13, 1)]
		AuthorizationTokenData,
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum AVDelegatingPlaybackCoordinatorRateChangeOptions : ulong {
		None = 0,
		PlayImmediately = (1uL << 0),
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum AVDelegatingPlaybackCoordinatorSeekOptions : ulong {
		None = 0,
		ResumeImmediately = (1uL << 0),
	}

	[TV (17, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVCaptureMicrophoneMode : long {
		Standard = 0,
		WideSpectrum = 1,
		VoiceIsolation = 2,
	}

	[TV (17, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVCaptureSystemUserInterface : long {
		VideoEffects = 1,
		MicrophoneModes = 2,
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[Native]
	public enum AVPlayerAudiovisualBackgroundPlaybackPolicy : long {
		Automatic = 1,
		Pauses = 2,
		ContinuesIfPossible = 3,
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum AVCoordinatedPlaybackSuspensionReason {

		[Field ("AVCoordinatedPlaybackSuspensionReasonAudioSessionInterrupted")]
		AudioSessionInterrupted,

		[Field ("AVCoordinatedPlaybackSuspensionReasonStallRecovery")]
		StallRecovery,

		[Field ("AVCoordinatedPlaybackSuspensionReasonPlayingInterstitial")]
		PlayingInterstitial,

		[Field ("AVCoordinatedPlaybackSuspensionReasonCoordinatedPlaybackNotPossible")]
		CoordinatedPlaybackNotPossible,

		[Field ("AVCoordinatedPlaybackSuspensionReasonUserActionRequired")]
		UserActionRequired,

		[Field ("AVCoordinatedPlaybackSuspensionReasonUserIsChangingCurrentTime")]
		UserIsChangingCurrentTime,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionAnimation : long {
		None = 0,
		CharacterReveal = 1,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionFontWeight : long {
		Unknown = 0,
		Normal = 1,
		Bold = 2,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionFontStyle : long {
		Unknown = 0,
		Normal = 1,
		Italic = 2,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Flags]
	[Native]
	public enum AVCaptionDecoration : ulong {
		None = 0x0,
		Underline = 1uL << 0,
		LineThrough = 1uL << 1,
		Overline = 1uL << 2,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionTextCombine : long {
		All = -1,
		None = 0,
		OneDigit = 1,
		TwoDigits = 2,
		ThreeDigits = 3,
		FourDigits = 4,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionTextAlignment : long {
		Start = 0,
		End = 1,
		Center = 2,
		Left = 3,
		Right = 4,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionRegionWritingMode : long {
		LeftToRightAndTopToBottom = 0,
		TopToBottomAndRightToLeft = 2,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionRegionScroll : long {
		None = 0,
		RollUp = 1,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionRegionDisplayAlignment : long {
		Before = 0,
		Center = 1,
		After = 2,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionRubyPosition : long {
		Before = 0,
		After = 1,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionRubyAlignment : long {
		Start = 0,
		Center = 1,
		DistributeSpaceBetween = 2,
		DistributeSpaceAround = 3,
	}

	[iOS (18, 0), MacCatalyst (18, 0)]
	[NoTV]
	[Native]
	public enum AVCaptionConversionValidatorStatus : long {
		Unknown = 0,
		Validating = 1,
		Completed = 2,
		Stopped = 3,
	}

	[TV (17, 0)]
	[MacCatalyst (15, 0)]
	[iOS (15, 0)]
	[Native]
	public enum AVCapturePrimaryConstituentDeviceSwitchingBehavior : long {
		Unsupported = 0,
		Auto = 1,
		Restricted = 2,
		Locked = 3,
	}

	[TV (17, 0)]
	[MacCatalyst (15, 0)]
	[iOS (15, 0)]
	[Flags]
	[Native]
	public enum AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions : ulong {
		None = 0x0,
		VideoZoomChanged = 1uL << 0,
		FocusModeChanged = 1uL << 1,
		ExposureModeChanged = 1uL << 2,
	}

	[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
	[Native]
	public enum AVAudioSessionRenderingMode : long {
		NotApplicable = 0,
		MonoStereo = 1,
		Surround = 2,
		SpatialAudio = 3,
		DolbyAudio = 4,
		DolbyAtmos = 5,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	[NativeName ("AVMIDIControlChangeMessageType")]
	public enum AVMidiControlChangeMessageType : long {
		BankSelect = 0,
		ModWheel = 1,
		Breath = 2,
		Foot = 4,
		PortamentoTime = 5,
		DataEntry = 6,
		Volume = 7,
		Balance = 8,
		Pan = 10,
		Expression = 11,
		Sustain = 64,
		Portamento = 65,
		Sostenuto = 66,
		Soft = 67,
		LegatoPedal = 68,
		Hold2Pedal = 69,
		FilterResonance = 71,
		ReleaseTime = 72,
		AttackTime = 73,
		Brightness = 74,
		DecayTime = 75,
		VibratoRate = 76,
		VibratoDepth = 77,
		VibratoDelay = 78,
		ReverbLevel = 91,
		ChorusLevel = 93,
		RpnLsb = 100,
		RpnMsb = 101,
		AllSoundOff = 120,
		ResetAllControllers = 121,
		AllNotesOff = 123,
		OmniModeOff = 124,
		OmniModeOn = 125,
		MonoModeOn = 126,
		MonoModeOff = 127,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	[NativeName ("AVMIDIMetaEventType")]
	public enum AVMidiMetaEventType : long {
		SequenceNumber = 0,
		Text = 1,
		Copyright = 2,
		TrackName = 3,
		Instrument = 4,
		Lyric = 5,
		Marker = 6,
		CuePoint = 7,
		MidiChannel = 32,
		MidiPort = 33,
		EndOfTrack = 47,
		Tempo = 81,
		SmpteOffset = 84,
		TimeSignature = 88,
		KeySignature = 89,
		ProprietaryEvent = 127,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum AVAudioVoiceProcessingSpeechActivityEvent : long {
		Started = 0,
		Ended = 1,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum AVSpeechSynthesisPersonalVoiceAuthorizationStatus : ulong {
		NotDetermined,
		Denied,
		Unsupported,
		Authorized,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum AVSpeechSynthesisVoiceTraits : ulong {
		None = 0x0,
		IsNoveltyVoice = 1uL << 0,
		IsPersonalVoice = 1uL << 1,
	}
	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native, Flags]
	public enum AVAssetTrackGroupOutputHandling : ulong {
		None = 0x0,
		PreserveAlternateTracks = (1uL << 0),
		DefaultPolicy = None,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum AVSpeechSynthesisMarkerMark : long {
		Phoneme,
		Word,
		Sentence,
		Paragraph,
		Bookmark,
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum AVAudioVoiceProcessingOtherAudioDuckingLevel : long {
		Default = 0,
		Min = 10,
		Mid = 20,
		Max = 30,
	}

	[TV (17, 0), MacCatalyst (17, 0), iOS (17, 0)]
	[Native]
	public enum AVCapturePhotoOutputCaptureReadiness : long {
		SessionNotRunning = 0,
		Ready = 1,
		NotReadyMomentarily = 2,
		NotReadyWaitingForCapture = 3,
		NotReadyWaitingForProcessing = 4,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum AVPlayerLooperItemOrdering : long {
		PrecedeExistingItems = 0,
		FollowExistingItems = 1,
	}

	[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum AVExternalContentProtectionStatus : long {
		Pending = 0,
		Sufficient = 1,
		Insufficient = 2,
	}

	[TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
	[Native]
	public enum AVPlayerInterstitialEventAssetListResponseStatus : long {
		Available = 0,
		Cleared = 1,
		Unavailable = 2,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	enum AVAssetPlaybackConfigurationOption {
		[Field ("AVAssetPlaybackConfigurationOptionStereoVideo")]
		StereoVideo,

		[Field ("AVAssetPlaybackConfigurationOptionStereoMultiviewVideo")]
		StereoMultiviewVideo,

		[MacCatalyst (18, 0), TV (18, 0), Mac (15, 0), iOS (18, 0)]
		[Field ("AVAssetPlaybackConfigurationOptionSpatialVideo")]
		SpatialVideo,

		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Field ("AVAssetPlaybackConfigurationOptionNonRectilinearProjection")]
		NonRectilinearProjection,

		[NoMacCatalyst, NoTV, Mac (26, 0), NoiOS]
		[Field ("AVAssetPlaybackConfigurationOptionAppleImmersiveVideo")]
		AppleImmersiveVideo,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	enum AVPlayerInterstitialEventCue {
		[Field ("AVPlayerInterstitialEventNoCue")]
		NoCue,

		[Field ("AVPlayerInterstitialEventJoinCue")]
		JoinCue,

		[Field ("AVPlayerInterstitialEventLeaveCue")]
		LeaveCue,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum AVPlayerInterstitialEventTimelineOccupancy : long {
		SinglePoint = 0,
		Fill = 1,
	}

	// Yes, this type is in AVFoundation, not CoreMedia
	[TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
	public enum CMTagCollectionVideoOutputPreset : uint {
		Monoscopic,
		Stereoscopic,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVPlayerNetworkResourcePriority : long {
		Default = 0,
		Low = 1,
		High = 2,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVPlayerInterstitialEventSkippableEventState : long {
		NotSkippable = 0,
		NotYetEligible = 1,
		Eligible = 2,
		NoLongerEligible = 3,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVAudioContentSource : long {
		Unspecified = -1,
		Reserved = 0,
		AppleCaptureTraditional = 1,
		AppleCaptureSpatial = 2,
		AppleCaptureSpatialEnhanced = 3,
		AppleMusicTraditional = 4,
		AppleMusicSpatial = 5,
		AppleAVTraditionalOffline = 6,
		AppleAVSpatialOffline = 7,
		AppleAVTraditionalLive = 8,
		AppleAVSpatialLive = 9,
		ApplePassthrough = 10,
		CaptureTraditional = 33,
		CaptureSpatial = 34,
		CaptureSpatial_Enhanced = 35,
		MusicTraditional = 36,
		MusicSpatial = 37,
		AVTraditionalOffline = 38,
		AVSpatialOffline = 39,
		AVTraditionalLive = 40,
		AVSpatialLive = 41,
		Passthrough = 42,
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVAudioDynamicRangeControlConfiguration : long {
		None = 0,
		Music = 1,
		Speech = 2,
		Movie = 3,
		Capture = 4,
	}

	[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
	public enum AVCaptureAspectRatio {
		[Field ("AVCaptureAspectRatio1x1")]
		OneByOne1x1,

		[Field ("AVCaptureAspectRatio16x9")]
		SixteenByNine16x9,

		[Field ("AVCaptureAspectRatio9x16")]
		NineBySixteen9x16,

		[Field ("AVCaptureAspectRatio4x3")]
		FourByThree4x3,

		[Field ("AVCaptureAspectRatio3x4")]
		ThreeByFour3x4,
	}

	[TV (26, 0), NoMac, MacCatalyst (26, 0), iOS (26, 0)]
	[BackingFieldType (typeof (AVCaptureWhiteBalanceTemperatureAndTintValues))]
	public enum AVCaptureWhiteBalanceTemperatureAndTintValue {
		[Field ("AVCaptureWhiteBalanceTemperatureAndTintValuesTungsten")]
		Tungsten,

		[Field ("AVCaptureWhiteBalanceTemperatureAndTintValuesFluorescent")]
		Fluorescent,

		[Field ("AVCaptureWhiteBalanceTemperatureAndTintValuesDaylight")]
		Daylight,

		[Field ("AVCaptureWhiteBalanceTemperatureAndTintValuesCloudy")]
		Cloudy,

		[Field ("AVCaptureWhiteBalanceTemperatureAndTintValuesShadow")]
		Shadow,
	}

	[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVExternalSyncDeviceStatus : long {
		Unavailable = 0,
		Ready = 1,
		Calibrating = 2,
		ActiveSync = 3,
		FreeRunSync = 4,
	}

	[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVCaptureTimecodeSourceType : long {
		FrameCount = 0,
		RealTimeClock = 1,
		External = 2,
	}

	[TV (26, 0), MacCatalyst (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVCaptureTimecodeGeneratorSynchronizationStatus : long {
		Unknown = 0,
		SourceSelected = 1,
		Synchronizing = 2,
		Synchronized = 3,
		TimedOut = 4,
		SourceUnavailable = 5,
		SourceUnsupported = 6,
		NotRequired = 7,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum AVMetricPlaybackMode : long {
		Local = 0,
		AirPlayVideo = 1,
	}

	[iOS (27, 0), TV (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[Native]
	public enum AVCaptureBroadcastVideoOutputDroppedFrameReplacementPolicy : long {
		RepeatPreviousFrame = 0,
		BlackFrame = 1,
	}
}
