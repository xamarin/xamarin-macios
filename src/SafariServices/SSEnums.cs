//
// SSEnums.cs: SafariServices framework enums
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, 2016 Xamarin Inc.
//

using XamCore.ObjCRuntime;

namespace XamCore.SafariServices {

	// NSInteger -> SSReadingList.h
	[Native]
	public enum SSReadingListError : nint {
		UrlSchemeNotAllowed = 1
	}

	[Native]
	[ErrorDomain ("SFContentBlockerErrorDomain")]
	public enum SFContentBlockerErrorCode : nint {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}
}
