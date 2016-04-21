//
// ReplayKit enums
//
// Copyright 2015-2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.UIKit;

namespace XamCore.ReplayKit {

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
	}
}