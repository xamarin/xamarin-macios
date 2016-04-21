//
// Enumcs.cs: Definitions for MonoTouch.NewsstandKit
//
// Copyright 2011, Xamarin, Inc.
//
// Author:
//  Miguel de Icaza
//

using XamCore.ObjCRuntime;

namespace XamCore.NewsstandKit {

	// untyped enum -> NKIssue.h
	// note: iOS 8 beta (2) has it defined as an NSInteger, filed as radar 17564957 with Apple
	[Native]
	public enum NKIssueContentStatus : nint {
		None, Downloading, Available
	}
}