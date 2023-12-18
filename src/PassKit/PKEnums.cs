// Copyright 2012-2014 Xamarin Inc. All rights reserved.

using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

#if !NET
	// untyped enum -> PKError.h
	// This never seemed to be deprecatd, yet in iOS8 it's obsoleted
	// this enum does not show up in the headers anymore
	[Obsoleted (PlatformName.iOS, 8, 0)]
	[NoMac]
	public enum PKErrorCode {
		None = 0,
		Unknown = 1,
		NotEntitled,
		PermissionDenied, // new in iOS7
	}
#endif

	// NSInteger -> PKPass.h
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
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
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPassLibraryAddPassesStatus : long {
		DidAddPasses,
		ShouldReviewPasses,
		DidCancelAddPasses
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPassType : ulong {
		Barcode,
		SecureElement,
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElement' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'SecureElement' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureElement' instead.")]
		Payment = SecureElement,
		Any = ulong.MaxValue,
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentAuthorizationStatus : long {
		Success,
		Failure,

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		InvalidBillingPostalAddress,

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		InvalidShippingPostalAddress,

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		InvalidShippingContact,

		[MacCatalyst (13, 1)]
		PinRequired,
		[MacCatalyst (13, 1)]
		PinIncorrect,
		[MacCatalyst (13, 1)]
		PinLockout
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[Native]
	public enum PKPaymentPassActivationState : ulong {
		Activated, RequiresActivation, Activating, Suspended, Deactivated
	}

	[Mac (11, 0)]
	[Watch (6, 2), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKSecureElementPassActivationState : long {
		Activated,
		RequiresActivation,
		Activating,
		Suspended,
		Deactivated,
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKMerchantCapability : ulong {
		ThreeDS = 1 << 0,
		EMV = 1 << 1,
		Credit = 1 << 2,
		Debit = 1 << 3,
		[iOS (17, 0), Mac (14, 0), Watch (10, 0), NoTV, MacCatalyst (17, 0)]
		InstantFundsOut = 1 << 7,
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'PKContactField' instead.")]
	[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'PKContactField' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PKContactField' instead.")]
	[Native]
	[Flags]
	public enum PKAddressField : ulong {
		None = 0,
		PostalAddress = 1 << 0,
		Phone = 1 << 1,
		Email = 1 << 2,
		[MacCatalyst (13, 1)]
		Name = 1 << 3,
		All = PostalAddress | Phone | Email | Name
	}

	[Mac (11, 0)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentButtonStyle : long {
		White,
		WhiteOutline,
		Black,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Automatic = 3,
	}

	[Mac (11, 0)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentButtonType : long {
		Plain,
		Buy,
		[MacCatalyst (13, 1)]
		SetUp,
		[MacCatalyst (13, 1)]
		InStore,
		[MacCatalyst (13, 1)]
		Donate,
#if NET
		[iOS (12,0)]
		[MacCatalyst (13, 1)]
		Checkout,
		[iOS (12,0)]
		[MacCatalyst (13, 1)]
		Book,
#else
		[iOS (12, 0)]
		[Obsolete ("Use 'Book2'.")]
		Book,
		[iOS (12, 0)]
		[Obsolete ("Use 'Checkout2'.")]
		Checkout,
#endif // !NET
		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		Subscribe,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Reload = 8,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		AddMoney = 9,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		TopUp = 10,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Order = 11,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Rent = 12,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Support = 13,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Contribute = 14,
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		Tip = 15,
		[Mac (12, 0), iOS (15, 0), Watch (8, 0)]
		[MacCatalyst (15, 0)]
		Continue = 16,
#if !NET
#pragma warning disable 0618 // warning CS0618: 'PKPaymentButtonType.[field]' is obsolete: 'Use '[replacement]'.'
		[iOS (12, 0)]
		Book2 = Checkout,
		[iOS (12, 0)]
		Checkout2 = Book,
#pragma warning restore
#endif // !NET
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKShippingType : ulong {
		Shipping,
		Delivery,
		StorePickup,
		ServicePickup,
	}

	[Watch (6, 0)]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPaymentPassError : long {
		Unsupported,
		UserCancelled,
		SystemCancelled
	}

	[Mac (11, 0)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAutomaticPassPresentationSuppressionResult : ulong {
		NotSupported = 0,
		AlreadyPresenting,
		Denied,
		Cancelled,
		Success
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentMethodType : ulong {
		Unknown = 0,
		Debit,
		Credit,
		Prepaid,
		Store,
		EMoney,
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentSummaryItemType : ulong {
		Final,
		Pending
	}

	[NoWatch]
	[NoMac] // under `#if TARGET_OS_IOS`
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPassButtonStyle : long {
		Black = 0,
		Outline
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
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

	[iOS (12, 0)]
	[Mac (11, 0)]
	[NoWatch] // https://feedbackassistant.apple.com/feedback/6301809 https://github.com/xamarin/maccore/issues/1819
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPaymentPassStyle : ulong {
		Payment,
		Access,
	}

	[Watch (6, 2), iOS (13, 4)]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("PKAddSecureElementPassErrorDomain")]
	[Native]
	public enum PKAddSecureElementPassErrorCode : long {
		UnknownError,
		UserCanceledError,
		UnavailableError,
		InvalidConfigurationError,
		DeviceNotSupportedError,
		DeviceNotReadyError,
		OSVersionNotSupportedError,
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum PKIdentityButtonLabel : long {
		VerifyIdentity = 0,
		Verify,
		VerifyAge,
		Continue,
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum PKIdentityButtonStyle : long {
		Black = 0,
		Outline,
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	[ErrorDomain ("PKIdentityErrorDomain")]
	public enum PKIdentityError : long {
		Unknown = 0,
		NotSupported = 1,
		Cancelled = 2,
		NetworkUnavailable = 3,
		NoElementsRequested = 4,
		RequestAlreadyInProgress = 5,
		InvalidNonce = 6,
		InvalidElement = 7,
	}

	[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	[ErrorDomain ("PKShareSecureElementPassErrorDomain")]
	public enum PKShareSecureElementPassErrorCode : long {
		UnknownError,
		SetupError,
	}

	[iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV, NoMac]
	[Native]
	public enum PKShareSecureElementPassResult : long {
		Canceled,
		Shared,
		Failed,
	}

	[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum PKVehicleConnectionErrorCode : long {
		Unknown = 0,
		SessionUnableToStart,
		SessionNotActive,
	}

	[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum PKVehicleConnectionSessionConnectionState : long {
		Disconnected = 0,
		Connected,
		Connecting,
		FailedToConnect,
	}

	[iOS (17, 0), Mac (14, 0), Watch (10, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum PKApplePayLaterAvailability : long {
		Available,
		UnavailableItemIneligible,
		UnavailableRecurringTransaction,
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	[ErrorDomain ("PKDisbursementErrorDomain")]
	public enum PKDisbursementErrorCode : long {
		UnknownError = -1,
		UnsupportedCardError = 1,
		RecipientContactInvalidError,
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[Native]
	public enum PKPayLaterAction : long {
		LearnMore = 0,
		Calculator,
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[Native]
	public enum PKPayLaterDisplayStyle : long {
		Standard = 0,
		Badge,
		Checkout,
		Price,
	}
}
