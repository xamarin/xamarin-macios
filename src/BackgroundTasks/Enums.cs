//
// BackgroundTasks C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//
using System;
using Foundation;
using ObjCRuntime;

namespace BackgroundTasks {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("ios13.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[TV (13,0)]
	[NoWatch]
	[NoMac]
	[iOS (13,0)]
#endif
	[Native]
	[ErrorDomain ("BGTaskSchedulerErrorDomain")]
	public enum BGTaskSchedulerErrorCode : long {
		Unavailable = 1,
		TooManyPendingTaskRequests = 2,
		NotPermitted = 3,
	}
}
