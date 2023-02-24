//
// WebKit/Defs.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014, 2016 Xamarin Inc. All rights reserved.
//

using System;

using ObjCRuntime;

#nullable enable

namespace WebKit {
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationType : long {
		LinkActivated,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other = -1
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationActionPolicy : long {
		Cancel,
		Allow,
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Download,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationResponsePolicy : long {
		Cancel,
		Allow,
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Download,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum WKUserScriptInjectionTime : long {
		AtDocumentStart,
		AtDocumentEnd
	}

	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("WKErrorDomain")]
	public enum WKErrorCode : long {
		None,
		Unknown,
		WebContentProcessTerminated,
		WebViewInvalidated,
		JavaScriptExceptionOccurred,
		JavaScriptResultTypeIsUnsupported,
		// Xcode 9
		ContentRuleListStoreCompileFailed,
		ContentRuleListStoreLookUpFailed,
		ContentRuleListStoreRemoveFailed,
		ContentRuleListStoreVersionMismatch,
		// Xcode 11
		AttributedStringContentFailedToLoad,
		AttributedStringContentLoadTimedOut,
		// Xcode 12
		JavaScriptInvalidFrameTarget,
		NavigationAppBoundDomain,
		JavaScriptAppBoundDomain,
		// Xcode 14
		DuplicateCredential,
		MalformedCredential,
		CredentialNotFound,
	}

#if NET
	[NoMac]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKSelectionGranularity : long {
		Dynamic, Character
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum WKDataDetectorTypes : ulong {
		None = 0,
		PhoneNumber = 1 << 0,
		Link = 1 << 1,
		Address = 1 << 2,
		CalendarEvent = 1 << 3,
		TrackingNumber = 1 << 4,
		FlightNumber = 1 << 5,
		LookupSuggestion = 1 << 6,
		SpotlightSuggestion = LookupSuggestion,
		All = UInt64.MaxValue
	}

	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum WKAudiovisualMediaTypes : ulong {
		None = 0,
		Audio = 1 << 0,
		Video = 1 << 1,
		All = UInt64.MaxValue
	}

	[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKMediaCaptureState : long {
		None,
		Active,
		Muted,
	}

	[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKMediaCaptureType : long {
		Camera,
		Microphone,
		CameraAndMicrophone,
	}

	[Mac (12, 0), iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKPermissionDecision : long {
		Prompt,
		Grant,
		Deny,
	}

}
