//
// BackgroundTasks C# bindings
//
// Authors:
//	Manuel de la Pena Saenz <mandel@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//
using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace BackgroundTasks {

	[Abstract]
	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (BGTaskRequest))]
	interface BGAppRefreshTaskRequest {
		[Export ("initWithIdentifier:")]
		IntPtr Constructor (string identifier);
	}

	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (BGTaskRequest))]
	interface BGProcessingTaskRequest {
		[Export ("initWithIdentifier:")]
		IntPtr Constructor (string identifier);

		[Export ("requiresNetworkConnectivity")]
		bool RequiresNetworkConnectivity { get; set; }

		[Export ("requiresExternalPower")]
		bool RequiresExternalPower { get; set; }
	}

	[Abstract]
	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BGTaskRequest : NSCopying {
		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("earliestBeginDate", ArgumentSemantic.Copy)]
		NSDate EarliestBeginDate { get; set; }
	}

	[Abstract]
	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BGTask
	{
		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("expirationHandler", ArgumentSemantic.Strong)]
		Action ExpirationHandler { get; set; }

		[Export ("setTaskCompletedWithSuccess:")]
		void SetTaskCompleted (bool success);
	}

	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (BGTask))]
	interface BGAppRefreshTask {
	}

	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (BGTask))]
	interface BGProcessingTask {
	}

	[TV (13,0), NoWatch, NoMac, iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BGTaskScheduler {
		[Static]
		[Export ("sharedScheduler", ArgumentSemantic.Strong)]
		BGTaskScheduler Shared { get; }

		[Advice ("This API is not available when using iOS App Extensions.")]
		[Export ("registerForTaskWithIdentifier:usingQueue:launchHandler:")]
		bool Register (string identifier, [NullAllowed] DispatchQueue queue, Action<BGTask> launchHandler);

		[Export ("submitTaskRequest:error:")]
		bool Submit (BGTaskRequest taskRequest, [NullAllowed] out NSError error);

		[Export ("cancelTaskRequestWithIdentifier:")]
		void Cancel (string identifier);

		[Export ("cancelAllTaskRequests")]
		void CancelAll ();

		[Async]
		[Export ("getPendingTaskRequestsWithCompletionHandler:")]
		void GetPending (Action<BGTaskRequest []> completionHandler);
	}

}