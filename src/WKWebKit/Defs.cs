//
// WKWebKit/Defs.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2014, 2016 Xamarin Inc. All rights reserved.
//

using System;

using XamCore.ObjCRuntime;

namespace XamCore.WebKit
{
	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum WKNavigationType : nint {
		LinkActivated,
		FormSubmitted,
		BackForward,
		Reload,
		FormResubmitted,
		Other = -1
	}

	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum WKNavigationActionPolicy : nint {
		Cancel,
		Allow
	}

	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum WKNavigationResponsePolicy : nint {
		Cancel,
		Allow
	}

	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum WKUserScriptInjectionTime : nint {
		AtDocumentStart,
		AtDocumentEnd
	}

	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	[ErrorDomain ("WKErrorDomain")]
	public enum WKErrorCode : nint {
		None,
		Unknown,
		WebContentProcessTerminated,
		WebViewInvalidated,
		JavaScriptExceptionOccurred,
		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		JavaScriptResultTypeIsUnsupported,
	}

#if !MONOMAC || !XAMCORE_4_0
	[Availability (Platform.iOS_8_0)]
	[Native]
	public enum WKSelectionGranularity : nint {
		Dynamic, Character
	}
#endif

	[iOS (10,0)][NoMac]
	[Native]
	[Flags]
	public enum WKDataDetectorTypes : nuint {
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

	[iOS (10,0)][Mac (10,12, only64: true)]
	[Native]
	[Flags]
	public enum WKAudiovisualMediaTypes : nuint	{
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
