//
// SSEnums.cs: SafariServices framework enums
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, 2016 Xamarin Inc.
//

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.SafariServices {

	// NSInteger -> SSReadingList.h
	[NoMac][iOS (7,0)]
	[Native]
	[ErrorDomain ("SSReadingListErrorDomain")]
	public enum SSReadingListError : nint {
		UrlSchemeNotAllowed = 1
	}

	[NoMac]
	[iOS (9,0)]
	[Deprecated (PlatformName.iOS, 10,0, message: "Use 'SFErrorCode' enum.")]
	[Native]
	[ErrorDomain ("SFContentBlockerErrorDomain")]
	public enum SFContentBlockerErrorCode : nint {
		Ok = 0,
		NoExtensionFound = 1,
		NoAttachmentFound = 2,
		LoadingInterrupted = 3
	}

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

	[NoMac]
	[iOS (11,0)]
	[Native]
	public enum SFSafariViewControllerDismissButtonStyle : nint {
		Done,
		Close,
		Cancel,
	}

	[NoMac]
	[iOS (11,0)]
	[Native]
	[ErrorDomain ("SFAuthenticationErrorDomain")]
	public enum SFAuthenticationError : nint {
		CanceledLogin = 1,
	}

	[NoiOS]
	[Mac (10,12,4, onlyOn64: true)]
	[Native]
	public enum SFSafariServicesVersion : nint {
		V10_0,
		V10_1,
		[Mac (10,13, onlyOn64: true)]
		V11_0,
	}
}
