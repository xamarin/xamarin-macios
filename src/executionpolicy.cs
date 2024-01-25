// Copyright 2019 Microsoft Corporation

using System;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

namespace ExecutionPolicy {

	[MacCatalyst (16, 0)]
	[Native]
	public enum EPDeveloperToolStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[MacCatalyst (16, 0)]
	[Native]
	[ErrorDomain ("EPErrorDomain")]
	public enum EPError : long {
		Generic = 1,
		NotADeveloperTool,
	}

	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface EPDeveloperTool {

		[Export ("authorizationStatus")]
		EPDeveloperToolStatus AuthorizationStatus { get; }

		[Export ("requestDeveloperToolAccessWithCompletionHandler:")]
		void RequestDeveloperToolAccess (Action<bool> handler);
	}

	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface EPExecutionPolicy {

		[Export ("addPolicyExceptionForURL:error:")]
		bool AddPolicyException (NSUrl url, [NullAllowed] out NSError error);
	}
}
