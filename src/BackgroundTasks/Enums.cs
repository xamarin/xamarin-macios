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

#nullable enable

namespace BackgroundTasks {

	[TV (13, 0), NoWatch, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("BGTaskSchedulerErrorDomain")]
	public enum BGTaskSchedulerErrorCode : long {
		Unavailable = 1,
		TooManyPendingTaskRequests = 2,
		NotPermitted = 3,
	}
}
