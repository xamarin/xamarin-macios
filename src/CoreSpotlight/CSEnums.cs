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
		/// <summary>An unknown error occured.</summary>
		UnknownError = -1,
		/// <summary>The index was not available.</summary>
		IndexUnavailableError = -1000,
		/// <summary>The search item was invalid.</summary>
		InvalidItemError = -1001,
		/// <summary>The search client was in an invalid state.</summary>
		InvalidClientStateError = -1002,
		/// <summary>A remote connection failed.</summary>
		RemoteConnectionError = -1003,
		/// <summary>The quota was exceeded.</summary>
		QuotaExceeded = -1004,
		/// <summary>The device does not support indexing.</summary>
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
		/// <summary>Indicates that an unknown error ocurred.</summary>
		Unknown = -2000,
		/// <summary>Indicates that the search index could not be reached.</summary>
		IndexUnreachable = -2001,
		/// <summary>Indicates that the query was invalid.</summary>
		InvalidQuery = -2002,
		/// <summary>Indicates that the search was canceled.</summary>
		Cancelled = -2003
	}

	/// <summary>Enumerates file protection options in calls to <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Core%20Spotlight%20CSSearchable%20Index%20From%20Name&amp;scope=Xamarin" title="M:CoreSpotlight.CSSearchableIndex.FromName*">M:CoreSpotlight.CSSearchableIndex.FromName*</a></format>.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	public enum CSFileProtection {
		/// <summary>The file is not protected.</summary>
		None,
		/// <summary>The file is encrypted and cannot be read until after booting and unlocking are completed.</summary>
		Complete,
		/// <summary>The file is encrypted. If it was  created when the device was locked, it cannot be accessed after it is closed until after the user unlocks the device.</summary>
		CompleteUnlessOpen,
		/// <summary>The file is encrypted and cannot be opened until the user unlocks the device.</summary>
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
