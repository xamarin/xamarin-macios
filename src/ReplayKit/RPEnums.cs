//
// ReplayKit enums
//
// Copyright 2015-2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;
using XamCore.UIKit;

namespace XamCore.ReplayKit {

	[iOS (9,0)]
	[TV (10,0)]
	[Native]
	[ErrorDomain ("RPRecordingErrorDomain")]
	public enum RPRecordingError : nint {
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
		[iOS (11,2), TV (11,2)]
		FailedApplicationConnectionInvalid = -5814,
		[iOS (11,2), TV (11,2)]
		FailedApplicationConnectionInterrupted = -5815,
		[iOS (11,2), TV (11,2)]
		FailedNoMatchingApplicationContext = -5816,
		[iOS (11,2), TV (11,2)]
		FailedMediaServicesFailure = -5817,
		[iOS (11,2), TV (11,2)]
		VideoMixingFailure = -5818,
	}

	[NoiOS]
	[TV (10,0)]
	[Native]
	public enum RPPreviewViewControllerMode : nint {
		Preview,
		Share
	}

	[Native]
	[iOS (10,0)]
	[TV (10,0)]
	public enum RPSampleBufferType : nint {
		Video = 1,
		AudioApp,
		AudioMic
	}

	[Native]
	[iOS (11,0)]
	[NoTV]
	public enum RPCameraPosition : nint {
		Front = 1,
		Back,
	}
}