//
// ios5-twitter.cs: Twitter bindings
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2014 Xamarin Inc
//

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.Twitter {

	// untyped enum -> TWTweetComposeViewController.h where the values are equals to those of
	// SLComposeViewControllerResult, which is a NSInteger -> SLComposeViewController.h, but a 
	// sizeof(TWTweetComposeViewControllerResultDone) shows it's 4 bytes (on a 64 bits process)
	public enum TWTweetComposeViewControllerResult {
		Cancelled, Done
	}

	// untyped enum -> TWRequest.h where the values are equals to those of SLRequestMethod, 
	// which is a NSInteger -> SLRequest.h, but a sizeof(TWRequestMethodDELETE) shows it's
	// 4 bytes (on a 64 bits process)
	// note: the API (selectors) uses this as an NSInteger, e.g. from introspection tests
	// 	Return Value of selector: requestMethod, Type: Twitter.TWRequestMethod, Encoded as: q
	// which likely means it's internally used as a `SLRequestMethod`
	[Native]
	public enum TWRequestMethod : nint {
		Get, Post, Delete
	}
}