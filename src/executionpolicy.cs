// Copyright 2019 Microsoft Corporation

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace ExecutionPolicy {

	[Mac (10,15)]
	[Native]
	public enum EPDeveloperToolStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[Mac (10,15)]
	[Native]
	[ErrorDomain ("EPErrorDomain")]
	public enum EPError : long {
		Generic = 1,
		NotADeveloperTool,
	}

	[Mac (10,15)]
	[BaseType (typeof (NSObject))]
	interface EPDeveloperTool {

		[Export ("authorizationStatus")]
		EPDeveloperToolStatus AuthorizationStatus { get; }

		[Export ("requestDeveloperToolAccessWithCompletionHandler:")]
		void RequestDeveloperToolAccess (Action<bool> handler);
	}

	[Mac (10,15)]
	[BaseType (typeof (NSObject))]
	interface EPExecutionPolicy {

		[Export ("addPolicyExceptionForURL:error:")]
		bool AddPolicyException (NSUrl url, [NullAllowed] out NSError error);
	}
}
