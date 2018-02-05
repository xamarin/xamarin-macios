//
// CoreSpotlight enums
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015, 2016 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;

namespace CoreSpotlight {
	// NSInteger -> CNContact.h
	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0), Mac (10,11)]
	[Mac (10,13, onlyOn64: true)]
	[Native]
	[ErrorDomain ("CSIndexErrorDomain")]
	public enum CSIndexErrorCode : long {
		UnknownError =	-1,
		IndexUnavailableError = -1000,
		InvalidItemError = -1001,
		InvalidClientStateError = -1002,
		RemoteConnectionError = -1003,
		QuotaExceeded = -1004,
		IndexingUnsupported = -1005,
	}

	[NoTV][iOS (10,0)]
	[Mac (10,13, onlyOn64: true)]
	[ErrorDomain ("CSSearchQueryErrorDomain")]
	[Native]
	public enum CSSearchQueryErrorCode : long {
		Unknown = -2000,
		IndexUnreachable = -2001,
		InvalidQuery = -2002,
		Cancelled = -2003
	}
}
