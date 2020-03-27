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
	[NoMac][iOS (7,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using UIKit on macOS.")]
	[Native]
	[ErrorDomain ("SSReadingListErrorDomain")]
	public enum SSReadingListError : long {
		UrlSchemeNotAllowed = 1
	}

	[NoMac]
	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10,0, message: "Use 'SFErrorCode' enum.")]
	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Native]
	[ErrorDomain ("SFContentBlockerErrorDomain")]
	public enum SFContentBlockerErrorCode : long {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}

	[iOS (10,0)]
	[Unavailable (PlatformName.MacCatalyst)][Advice ("This API is not available when using UIKit on macOS.")]
	[Native]
	[ErrorDomain ("SFErrorDomain")]
	public enum SFErrorCode : long
	{
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}

	[NoMac]
	[iOS (11,0)]
	[Native]
	public enum SFSafariViewControllerDismissButtonStyle : long {
		Done,
		Close,
		Cancel,
	}

	[NoMac]
	[iOS (11,0)]
	[Native]
	[ErrorDomain ("SFAuthenticationErrorDomain")]
	[Deprecated (PlatformName.iOS, 12,0, message: "Use 'ASWebAuthenticationSessionErrorCode' instead.")]
	public enum SFAuthenticationError : long {
		CanceledLogin = 1,
	}

#if !XAMCORE_4_0
	[Obsolete ("Enum not used by any API.")]
	[NoiOS]
	[Mac (10,12,4)]
	[Native]
	public enum SFSafariServicesVersion : long {
		V10_0,
		V10_1,
		[Mac (10,13)]
		V11_0,
	}
#endif
}
