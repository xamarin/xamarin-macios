//
// WKWebKit/Defs.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014, 2016 Xamarin Inc. All rights reserved.
//

using System;

using ObjCRuntime;

#nullable enable

namespace WebKit
{
#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum WKNavigationType : long {
		LinkActivated,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other = -1
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum WKNavigationActionPolicy : long {
		Cancel,
		Allow,
#if NET
		[SupportedOSPlatform ("macos11.3")]
		[SupportedOSPlatform ("ios14.5")]
#else
		[Mac (11,3)]
		[iOS (14,5)]
#endif
		Download,
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum WKNavigationResponsePolicy : long {
		Cancel,
		Allow,
#if NET
		[SupportedOSPlatform ("macos11.3")]
		[SupportedOSPlatform ("ios14.5")]
#else
		[Mac (11,3)]
		[iOS (14,5)]
#endif
		Download,
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
	[Native]
	public enum WKUserScriptInjectionTime : long {
		AtDocumentStart,
		AtDocumentEnd
	}

#if NET
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[Mac (10, 10)]
	[iOS (8, 0)]
#endif
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
	}

#if NET
	[SupportedOSPlatform ("ios8.0")]
#else
	[iOS (8, 0)]
#endif
	[Native]
	public enum WKSelectionGranularity : long {
		Dynamic, Character
	}

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[iOS (10,0)]
	[NoMac]
#endif
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

#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.12")]
#else
	[iOS (10,0)]
	[Mac (10,12)]
#endif
	[Native]
	[Flags]
	public enum WKAudiovisualMediaTypes : ulong	{
		None = 0,
		Audio = 1 << 0,
		Video = 1 << 1,
		All = UInt64.MaxValue
	}

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (12,0)]
	[iOS (15,0)]
	[NoTV]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum WKMediaCaptureState : long {
		None,
		Active,
		Muted,
	}

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (12,0)]
	[iOS (15,0)]
	[NoTV]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum WKMediaCaptureType : long {
		Camera,
		Microphone,
		CameraAndMicrophone,
	}

#if NET
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[Mac (12,0)]
	[iOS (15,0)]
	[NoTV]
	[MacCatalyst (15,0)]
#endif
	[Native]
	public enum WKPermissionDecision : long {
		Prompt,
		Grant,
		Deny,
	}

}
