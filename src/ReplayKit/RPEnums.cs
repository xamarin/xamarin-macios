//
// ReplayKit enums
//
// Copyright 2015-2016 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;

namespace ReplayKit {

	[MacCatalyst (13, 1)]
	[Native ("RPRecordingErrorCode")]
	[ErrorDomain ("RPRecordingErrorDomain")]
	public enum RPRecordingError : long {
		None = 0,
		Unknown = -5800,
		UserDeclined = -5801,
		Disabled = -5802,
		FailedToStart = -5803,
		Failed = -5804,
		InsufficientStorage = -5805,
		Interrupted = -5806,
		ContentResize = -5807,
		BroadcastInvalidSession = -5808,
		SystemDormancy = -5809,
		Entitlements = -5810,
		ActivePhoneCall = -5811,
		FailedToSave = -5812,
		CarPlay = -5813,
		FailedApplicationConnectionInvalid = -5814,
		FailedApplicationConnectionInterrupted = -5815,
		FailedNoMatchingApplicationContext = -5816,
		FailedMediaServicesFailure = -5817,
		VideoMixingFailure = -5818,
		BroadcastSetupFailed = -5819,
		FailedToObtainUrl = -5820,
		FailedIncorrectTimeStamps = -5821,
		FailedToProcessFirstSample = -5822,
		FailedAssetWriterFailedToSave = -5823,
		FailedNoAssetWriter = -5824,
		FailedAssetWriterInWrongState = -5825,
		FailedAssetWriterExportFailed = -5826,
		FailedToRemoveFile = -5827,
		FailedAssetWriterExportCanceled = -5828,
		AttemptToStopNonRecording = -5829,
		AttemptToStartInRecordingState = -5830,
		PhotoFailure = -5831,
		RecordingInvalidSession = -5832,
		FailedToStartCaptureStack = -5833,
		InvalidParameter = -5834,
		FilePermissions = -5835,
		ExportClipToUrlInProgress = -5836,
	}

	[NoiOS]
	[NoMac]
	[NoMacCatalyst]
	[Native]
	public enum RPPreviewViewControllerMode : long {
		Preview,
		Share
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum RPSampleBufferType : long {
		Video = 1,
		AudioApp,
		AudioMic
	}

	[Native]
	[NoTV]
	[MacCatalyst (13, 1)]
	public enum RPCameraPosition : long {
		Front = 1,
		Back,
	}
}
