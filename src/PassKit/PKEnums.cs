// Copyright 2012-2014 Xamarin Inc. All rights reserved.

using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

	// untyped enum -> PKError.h
	// This never seemed to be deprecatd, yet in iOS8 it's obsoleted
#if NET
	[UnsupportedOSPlatform ("macos")]
#if IOS
	[Obsolete ("Starting with ios8.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Obsoleted (PlatformName.iOS, 8, 0)]
	[NoMac]
#endif
	public enum PKErrorCode {
		None = 0,
		Unknown = 1,
		NotEntitled,
		PermissionDenied, // new in iOS7
	}

	// NSInteger -> PKPass.h
#if NET
	[SupportedOSPlatform ("macos11.0")]
#else
	[Mac (11,0)]
#endif
	[ErrorDomain ("PKPassKitErrorDomain")]
	[Native]
	public enum PKPassKitErrorCode : long {
		Unknown = -1,
		None = 0,
		InvalidData = 1,
		UnsupportedVersion,
		InvalidSignature,
		NotEntitled
	}

	// NSInteger -> PKPassLibrary.h
#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (7,0)]
	[Mac (11,0)]
#endif
	[Native]
	public enum PKPassLibraryAddPassesStatus : long {
		DidAddPasses,
		ShouldReviewPasses,
		DidCancelAddPasses
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
#else
	[Mac (11,0)]
#endif
	[Native]
	public enum PKPassType : ulong {
		Barcode,
		SecureElement,
#if NET
		[UnsupportedOSPlatform ("ios13.4")]
#if IOS
		[Obsolete ("Starting with ios13.4 use 'SecureElement' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElement' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'SecureElement' instead.")]
#endif
		Payment = SecureElement,
		Any = ulong.MaxValue,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
#else
	[Mac (11,0)]
	[Watch (3,0)]
#endif
	[Native]
	public enum PKPaymentAuthorizationStatus : long {
		Success,
		Failure,

#if NET
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
#endif
		InvalidBillingPostalAddress,

#if NET
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
#endif
		InvalidShippingPostalAddress,

#if NET
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
#endif
		InvalidShippingContact,

#if NET
		[SupportedOSPlatform ("ios9.2")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (9,2)]
#endif
		PinRequired,
#if NET
		[SupportedOSPlatform ("ios9.2")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (9,2)]
#endif
		PinIncorrect,
#if NET
		[SupportedOSPlatform ("ios9.2")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (9,2)]
#endif
		PinLockout
	}

#if NET
	[UnsupportedOSPlatform ("ios13.4")]
#if IOS
	[Obsolete ("Starting with ios13.4 use 'PKSecureElementPassActivationState' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'PKSecureElementPassActivationState' instead.")]
#endif
	[Native]
	public enum PKPaymentPassActivationState : ulong {
		Activated, RequiresActivation, Activating, Suspended, Deactivated
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios13.4")]
#else
	[Mac (11,0)]
	[Watch (6,2)]
	[iOS (13,4)]
#endif
	[Native]
	public enum PKSecureElementPassActivationState : long {
		Activated,
		RequiresActivation,
		Activating,
		Suspended,
		Deactivated,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
#else
	[Mac (11,0)]
	[Watch (3,0)]
#endif
	[Native]
	public enum PKMerchantCapability : ulong {
		ThreeDS = 1 << 0,
		EMV = 1 << 1,
		Credit = 1 << 2,
		Debit = 1 << 3
	}

#if NET
	[UnsupportedOSPlatform ("ios11.0")]
#if IOS
	[Obsolete ("Starting with ios11.0 use 'PKContactField' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
	[UnsupportedOSPlatform ("macos")]
#else
	[NoMac]
	[Watch (3,0)]
	[Deprecated (PlatformName.iOS, 11,0, message: "Use 'PKContactField' instead.")]
	[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'PKContactField' instead.")]
#endif
	[Native]
	[Flags]
	public enum PKAddressField : ulong {
		None = 0,
		PostalAddress = 1 << 0,
		Phone = 1 << 1,
		Email = 1 << 2,
#if NET
		[SupportedOSPlatform ("ios8.3")]
		[UnsupportedOSPlatform ("ios11.0")]
#if IOS
		[Obsolete ("Starting with ios11.0 use 'PKContactField' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (8,3)]
#endif
		Name = 1 << 3,
		All = PostalAddress|Phone|Email|Name
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios8.3")]
#else
	[Mac (11,0)]
	[NoWatch]
	[iOS (8,3)]
#endif
	[Native]
	public enum PKPaymentButtonStyle : long {
		White,
		WhiteOutline,
		Black,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Automatic = 3,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios8.3")]
#else
	[Mac (11,0)]
	[NoWatch]
	[iOS (8,3)]
#endif
	[Native]
	public enum PKPaymentButtonType : long {
		Plain,
		Buy,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (9,0)]
#endif
		SetUp,
#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (10,0)]
#endif
		InStore,
#if NET
		[SupportedOSPlatform ("ios10.2")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (10,2)]
#endif
		Donate,
#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (12,0)]
#endif
		Book,
#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (12,0)]
#endif
		Checkout,
#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (12,0)]
#endif
		Subscribe,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Reload = 8,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		AddMoney = 9,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		TopUp = 10,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Order = 11,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Rent = 12,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Support = 13,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Contribute = 14,
#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("macos11.0")]
#else
		[iOS (14,0)]
#endif
		Tip = 15,
#if NET
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
#else
		[Mac (12,0)]
		[iOS (15,0)]
		[Watch (8,0)]
#endif
		Continue = 16,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios8.3")]
#else
	[Mac (11,0)]
	[Watch (3,0)]
	[iOS (8,3)]
#endif
	[Native]
	public enum PKShippingType : ulong {
		Shipping,
		Delivery,
		StorePickup,
		ServicePickup,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[Watch (6,0)]
	[iOS (9,0)]
	[Mac (11,0)]
#endif
	[Native]
	public enum PKAddPaymentPassError : long
	{
		Unsupported,
		UserCancelled,
		SystemCancelled
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios9.0")]
#else
	[Mac (11,0)]
	[NoWatch]
	[iOS (9,0)]
#endif
	[Native]
	public enum PKAutomaticPassPresentationSuppressionResult : ulong
	{
		NotSupported = 0,
		AlreadyPresenting,
		Denied,
		Cancelled,
		Success
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios9.0")]
#else
	[Mac (11,0)]
	[Watch (3,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum PKPaymentMethodType : ulong
	{
		Unknown = 0,
		Debit,
		Credit,
		Prepaid,
		Store,
		EMoney,
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios9.0")]
#else
	[Mac (11,0)]
	[Watch (3,0)]
	[iOS (9,0)]
#endif
	[Native]
	public enum PKPaymentSummaryItemType : ulong
	{
		Final,
		Pending
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoMac] // under `#if TARGET_OS_IOS`
	[iOS (9,0)]
#endif
	[Native]
	public enum PKAddPassButtonStyle : long {
		Black = 0,
		Outline
	}

#if NET
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios11.0")]
#else
	[Mac (11,0)]
	[Watch (4,0)]
	[iOS (11,0)]
#endif
	[ErrorDomain ("PKPaymentErrorDomain")]
	[Native]
	public enum PKPaymentErrorCode : long {
		Unknown = -1,
		ShippingContactInvalid = 1,
		BillingContactInvalid,
		ShippingAddressUnserviceable,
		CouponCodeInvalid,
		CouponCodeExpired,
	}

#if NET
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (12,0)]
	[Mac (11,0)]
	[NoWatch] // https://feedbackassistant.apple.com/feedback/6301809 https://github.com/xamarin/maccore/issues/1819
#endif
	[Native]
	public enum PKAddPaymentPassStyle : ulong {
		Payment,
		Access,
	}

#if NET
	[SupportedOSPlatform ("ios13.4")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[Watch (6,2)]
	[iOS (13,4)]
	[Mac (11,0)]
#endif
	[ErrorDomain ("PKAddSecureElementPassErrorDomain")]
	[Native]
	public enum PKAddSecureElementPassErrorCode : long {
		UnknownError,
		UserCanceledError,
		UnavailableError,
		InvalidConfigurationError,
		DeviceNotSupportedError,
		DeviceNotReadyError
	}
}
