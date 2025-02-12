//
// Enums.cs: Enumerations for the Social framework
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2014 Xamarin Inc
//

using Foundation;
using ObjCRuntime;

namespace Social {

	// NSInteger -> SLRequest.h
	/// <summary>The HTTP verb associated with a social service request.</summary>
	[Native]
	public enum SLRequestMethod : long {
		Get,
		Post,
		Delete,
		Put,
	}

	// NSInteger -> SLComposeViewController.h
#if NET
	/// <summary>An enumeration whose values specify whether composition in a <see cref="T:Social.SLComposeViewController" /> was completed or cancelled.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
#endif
	[Native]
	public enum SLComposeViewControllerResult : long {
		Cancelled,
		Done,
	}
}
