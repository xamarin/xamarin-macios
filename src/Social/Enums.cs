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

	// note: those are NSString in iOS/OSX that we expose as an enum (i.e. it's NOT a native enum)
	// when adding a value make sure to update SLRequest.KindToType method
	/// <summary>Enumeration with the various kinds of social services that can be used.</summary>
	///     <remarks>This enumeration is used to map into the underlying set of services offered by the social framework.   It is intended to assist code completion while developing and take the gueswork out of using the framework in some entry points that take an NSString as a parameter.</remarks>
	public enum SLServiceKind {
		Facebook,
		Twitter,
		SinaWeibo,
		TencentWeibo,
#if MONOMAC
		LinkedIn,
#endif
	}
}
