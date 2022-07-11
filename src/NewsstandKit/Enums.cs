//
// Enumcs.cs: Definitions for MonoTouch.NewsstandKit
//
// Copyright 2011, Xamarin, Inc.
//
// Author:
//  Miguel de Icaza
//

using ObjCRuntime;

namespace NewsstandKit {

	// untyped enum -> NKIssue.h
	// note: iOS 8 beta (2) has it defined as an NSInteger, filed as radar 17564957 with Apple
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use the Remote Notifications Background Modes instead.")]
	[Native]
	public enum NKIssueContentStatus : long {
		None, Downloading, Available
	}
}
