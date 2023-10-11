//
// SSEnums.cs: SafariServices framework enums
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, 2016 Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//

using System;

using Foundation;
using ObjCRuntime;

namespace SafariServices {

	// NSInteger -> SSReadingList.h
	[NoMac]
	[MacCatalyst (14, 0)]
	[Native ("SSReadingListErrorCode")]
	[ErrorDomain ("SSReadingListErrorDomain")]
	public enum SSReadingListError : long {
		UrlSchemeNotAllowed = 1
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'SFErrorCode' enum.")]
	[MacCatalyst (13, 4)]
	[Deprecated (PlatformName.MacCatalyst, 13, 4, message: "Use 'SFErrorCode' enum.")]
	[Native]
	[ErrorDomain ("SFContentBlockerErrorDomain")]
	public enum SFContentBlockerErrorCode : long {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Native]
	[ErrorDomain ("SFErrorDomain")]
	public enum SFErrorCode : long {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SFSafariViewControllerDismissButtonStyle : long {
		Done,
		Close,
		Cancel,
	}

	[NoMac]
	[Native]
	[ErrorDomain ("SFAuthenticationErrorDomain")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ASWebAuthenticationSessionErrorCode' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ASWebAuthenticationSessionErrorCode' instead.")]
	public enum SFAuthenticationError : long {
		CanceledLogin = 1,
	}

#if !NET
	[Obsolete ("Enum not used by any API.")]
	[NoiOS]
	[Native]
	public enum SFSafariServicesVersion : long {
		V10_0,
		V10_1,
		V11_0,
	}
#endif
}
