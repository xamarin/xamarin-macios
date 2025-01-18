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
	/// <summary>Enumerates the types of action that can cause navigation.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationType : long {
		LinkActivated,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other = -1,
	}

	/// <summary>Contains values that enumerate whether to cancel or allow navigation actions.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationActionPolicy : long {
		Cancel,
		Allow,
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Download,
	}

	/// <summary>Contains values that enumerate whether the response delegate should cancel or allow navigation.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKNavigationResponsePolicy : long {
		Cancel,
		Allow,
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		Download,
	}

	/// <summary>Enumerates values that indicate when to inject a script.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKUserScriptInjectionTime : long {
		AtDocumentStart,
		AtDocumentEnd,
	}

	/// <summary>Enumerates WebKit errors.</summary>
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
	/// <summary>Enumerates values the controls how selections are created.</summary>
	[NoMac]
#endif
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKSelectionGranularity : long {
		Dynamic,
		Character,
	}

	/// <summary>Enumerates the kinds of data that are detected and converted to links.</summary>
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
		All = UInt64.MaxValue,
	}

	/// <summary>Enumerates media types.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum WKAudiovisualMediaTypes : ulong {
		None = 0,
		Audio = 1 << 0,
		Video = 1 << 1,
		All = UInt64.MaxValue,
	}

	[iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKMediaCaptureState : long {
		None,
		Active,
		Muted,
	}

	[iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKMediaCaptureType : long {
		Camera,
		Microphone,
		CameraAndMicrophone,
	}

	[iOS (15, 0), NoTV, MacCatalyst (15, 0)]
	[Native]
	public enum WKPermissionDecision : long {
		Prompt,
		Grant,
		Deny,
	}

}
