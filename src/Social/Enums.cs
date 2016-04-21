//
// Enums.cs: Enumerations for the Social framework
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2014 Xamarin Inc
//

#if XAMCORE_2_0 || !MONOMAC

using XamCore.ObjCRuntime;

namespace XamCore.Social {

	// NSInteger -> SLRequest.h
	[Native]
	public enum SLRequestMethod : nint {
		Get, Post, Delete, Put
	}

	// NSInteger -> SLComposeViewController.h
	[Native]
	public enum SLComposeViewControllerResult : nint {
		Cancelled, Done
	}

	// note: those are NSString in iOS/OSX that we expose as an enum (i.e. it's NOT a native enum)
	// when adding a value make sure to update SLRequest.KindToType method
	public enum SLServiceKind {
		Facebook, 
		Twitter, 
		SinaWeibo, 
		TencentWeibo, 
#if MONOMAC
		LinkedIn
#endif
	}
}
#endif
