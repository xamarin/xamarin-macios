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

#if !MACCORE
	// NSInteger -> SSReadingList.h
	[Native]
	public enum SSReadingListError : nint {
		UrlSchemeNotAllowed = 1
	}

	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10,0, message: "Use SFErrorCode enum")]
	[Native]
	[ErrorDomain ("SFContentBlockerErrorDomain")]
	public enum SFContentBlockerErrorCode : nint {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}
#endif

	[iOS (10,0)]
	[Native]
	[ErrorDomain ("SFErrorDomain")]
	public enum SFErrorCode : nint
	{
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}
}
