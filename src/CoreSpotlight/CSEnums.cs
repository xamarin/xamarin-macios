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

#nullable enable

namespace CoreSpotlight {
	// NSInteger -> CNContact.h
	/// <summary>Enumerates possible errors associated with using Core Spotlight and searching.</summary>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("CSIndexErrorDomain")]
	public enum CSIndexErrorCode : long {
		UnknownError = -1,
		IndexUnavailableError = -1000,
		InvalidItemError = -1001,
		InvalidClientStateError = -1002,
		RemoteConnectionError = -1003,
		QuotaExceeded = -1004,
		IndexingUnsupported = -1005,
		MismatchedClientState = -1006,
	}

	/// <summary>Enumerates errors that can occur while running a Core Spotlight query with <see cref="M:CoreSpotlight.CSSearchQuery.Start" />.</summary>
	///     <remarks>Developers can use the <see cref="M:CoreSpotlight.CSSearchQueryErrorCodeExtensions.GetDomain(CoreSpotlight.CSSearchQueryErrorCode)" /> extension method to get the error domain.</remarks>
	[NoTV]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("CSSearchQueryErrorDomain")]
	[Native]
	public enum CSSearchQueryErrorCode : long {
		Unknown = -2000,
		IndexUnreachable = -2001,
		InvalidQuery = -2002,
		Cancelled = -2003
	}

	/// <summary>Enumerates file protection options in calls to <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Core%20Spotlight%20CSSearchable%20Index%20From%20Name&amp;scope=Xamarin" title="M:CoreSpotlight.CSSearchableIndex.FromName*">M:CoreSpotlight.CSSearchableIndex.FromName*</a></format>.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	public enum CSFileProtection {
		None,
		Complete,
		CompleteUnlessOpen,
		CompleteUntilFirstUserAuthentication,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum CSUserInteraction : long {
		Select,
		Default = Select,
		Focus,
	}
}
