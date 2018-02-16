//
// Enums.cs: Enumerations for the Social framework
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2014 Xamarin Inc
//

#if XAMCORE_2_0 || !MONOMAC

using ObjCRuntime;

namespace Social {

	// NSInteger -> SLRequest.h
	[Native]
	public enum SLRequestMethod : long {
		Get, Post, Delete, Put
	}

#if !MONOMAC || !XAMCORE_4_0
	// NSInteger -> SLComposeViewController.h
	[Native]
	public enum SLComposeViewControllerResult : long {
		Cancelled, Done
	}
#endif

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
