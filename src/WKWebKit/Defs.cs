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
		JavaScriptResultTypeIsUnsupported,
	}

	[Availability (Platform.Mac_10_10 | Platform.iOS_8_0)]
	[Native]
	public enum WKSelectionGranularity : nint {
		Dynamic, Character
	}
}
