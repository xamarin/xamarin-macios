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

namespace WebKit
{
	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum WKNavigationType : long {
		LinkActivated,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other = -1
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum WKNavigationActionPolicy : long {
		Cancel,
		Allow
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum WKNavigationResponsePolicy : long {
		Cancel,
		Allow
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	public enum WKUserScriptInjectionTime : long {
		AtDocumentStart,
		AtDocumentEnd
	}

	[Mac (10, 10)]
	[iOS (8, 0)]
	[Native]
	[ErrorDomain ("WKErrorDomain")]
	public enum WKErrorCode : long {
		None,
		Unknown,
		WebContentProcessTerminated,
		WebViewInvalidated,
		JavaScriptExceptionOccurred,
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		JavaScriptResultTypeIsUnsupported,
		[iOS (11,0)][Mac (10,13, onlyOn64 : true)]
		ContentRuleListStoreCompileFailed,
		[iOS (11,0)][Mac (10,13, onlyOn64 : true)]
		ContentRuleListStoreLookUpFailed,
		[iOS (11,0)][Mac (10,13, onlyOn64 : true)]
		ContentRuleListStoreRemoveFailed,
		[iOS (11,0)][Mac (10,13, onlyOn64 : true)]
		ContentRuleListStoreVersionMismatch
	}

#if !MONOMAC || !XAMCORE_4_0
	[iOS (8, 0)]
	[Native]
	public enum WKSelectionGranularity : long {
		Dynamic, Character
	}
#endif

	[iOS (10,0)][NoMac]
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
#if XAMCORE_2_0
		All = UInt64.MaxValue
#else
		All = UInt32.MaxValue
#endif
	}

	[iOS (10,0)][Mac (10,12, onlyOn64: true)]
	[Native]
	[Flags]
	public enum WKAudiovisualMediaTypes : ulong	{
		None = 0,
		Audio = 1 << 0,
		Video = 1 << 1,
#if XAMCORE_2_0
		All = UInt64.MaxValue
#else
		All = UInt32.MaxValue
#endif
	}
}
