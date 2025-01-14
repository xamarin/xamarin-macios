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
	/// <summary>An enumeration whose values specify errors relating to the passes and passbook functionality.</summary>
	[MacCatalyst (13, 1)]
	[ErrorDomain ("PKPassKitErrorDomain")]
	[Native]
	public enum PKPassKitErrorCode : long {
		Unknown = -1,
		None = 0,
		InvalidData = 1,
		UnsupportedVersion,
		InvalidSignature,
		NotEntitled,
	}

	// NSInteger -> PKPassLibrary.h
	/// <summary>An enumeration whose values define possible results when passes are added.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPassLibraryAddPassesStatus : long {
		DidAddPasses,
		ShouldReviewPasses,
		DidCancelAddPasses,
	}

	/// <summary>Enumeration of pass types (whether a pass is a barcode or presents a payment card).</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPassType : ulong {
		Barcode,
		SecureElement,
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElement' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureElement' instead.")]
		Payment = SecureElement,
		Any = ulong.MaxValue,
	}

	/// <summary>Enumeration of results of authorization requests.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentAuthorizationStatus : long {
		Success,
		Failure,

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		InvalidBillingPostalAddress,

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		InvalidShippingPostalAddress,

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		InvalidShippingContact,

		[MacCatalyst (13, 1)]
		PinRequired,
		[MacCatalyst (13, 1)]
		PinIncorrect,
		[MacCatalyst (13, 1)]
		PinLockout,
	}

	/// <summary>Enumeration of valid states of a <see cref="T:PassKit.PKPaymentPass" /> payment card.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PKSecureElementPassActivationState' instead.")]
	[Native]
	public enum PKPaymentPassActivationState : ulong {
		Activated,
		RequiresActivation,
		Activating,
		Suspended,
		Deactivated,
	}

	[iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKSecureElementPassActivationState : long {
		Activated,
		RequiresActivation,
		Activating,
		Suspended,
		Deactivated,
	}

	/// <summary>Payment processing capabilities of a merchant.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKMerchantCapability : ulong {
		ThreeDS = 1 << 0,
		EMV = 1 << 1,
		Credit = 1 << 2,
		Debit = 1 << 3,
		[iOS (17, 0), Mac (14, 0), NoTV, MacCatalyst (17, 0)]
		InstantFundsOut = 1 << 7,
	}

	/// <summary>Holds address information for billing or shipping purposes.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'PKContactField' instead.")]
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
		All = PostalAddress | Phone | Email | Name,
	}

	/// <summary>Enumerates available styles for <see cref="T:PassKit.PKPaymentButton" /> objects.</summary>
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

	/// <summary>Enumerates the types of <see cref="T:PassKit.PKPaymentButton" /> objects.</summary>
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
		[MacCatalyst (13, 1)]
		Checkout,
		[MacCatalyst (13, 1)]
		Book,
#else
		[Obsolete ("Use 'Book2'.")]
		Book,
		[Obsolete ("Use 'Checkout2'.")]
		Checkout,
#endif // !NET
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
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		Continue = 16,
#if !NET
#pragma warning disable 0618 // warning CS0618: 'PKPaymentButtonType.[field]' is obsolete: 'Use '[replacement]'.'
		Book2 = Checkout,
		Checkout2 = Book,
#pragma warning restore
#endif // !NET
	}

	/// <summary>Enumerates shipping methods.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKShippingType : ulong {
		Shipping,
		Delivery,
		StorePickup,
		ServicePickup,
	}

	/// <summary>Enumerates error codes relating to PassKit operations.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPaymentPassError : long {
		Unsupported,
		UserCancelled,
		SystemCancelled,
	}

	/// <summary>Enumerates results that are used in calls to <see cref="M:PassKit.PKPassLibrary.RequestAutomaticPassPresentationSuppression(System.Action{PassKit.PKAutomaticPassPresentationSuppressionResult})" />.</summary>
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAutomaticPassPresentationSuppressionResult : ulong {
		NotSupported = 0,
		AlreadyPresenting,
		Denied,
		Cancelled,
		Success,
	}

	/// <summary>Enumerates the types of cards available to Apple Pay.</summary>
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

	/// <summary>Enumerates whether a payment associated with a <see cref="T:PassKit.PKShippingMethod" /> is pending or final.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKPaymentSummaryItemType : ulong {
		Final,
		Pending,
	}

	/// <summary>Enumerates Pass Button styles.</summary>
	[NoMac] // under `#if TARGET_OS_IOS`
	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPassButtonStyle : long {
		Black = 0,
		Outline,
	}

	/// <summary>Enumerates error conditions for payment operations.</summary>
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

	[MacCatalyst (13, 1)]
	[Native]
	public enum PKAddPaymentPassStyle : ulong {
		Payment,
		Access,
	}

	[iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("PKAddSecureElementPassErrorDomain")]
	[Native]
	public enum PKAddSecureElementPassErrorCode : long {
		GenericError = 0,
#if !XAMCORE_5_0
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'GenericError' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'GenericError' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'GenericError' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'GenericError' instead.")]
		UnknownError = GenericError,
#endif
		UserCanceledError,
		UnavailableError,
		InvalidConfigurationError,
		DeviceNotSupportedError,
		DeviceNotReadyError,
		OSVersionNotSupportedError,
	}

	[NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum PKIdentityButtonLabel : long {
		VerifyIdentity = 0,
		Verify,
		VerifyAge,
		Continue,
	}

	[NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum PKIdentityButtonStyle : long {
		Black = 0,
		Outline,
	}

	[NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
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
		RegionNotSupported = 8,
	}

	[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	[ErrorDomain ("PKShareSecureElementPassErrorDomain")]
	public enum PKShareSecureElementPassErrorCode : long {
		UnknownError,
		SetupError,
	}

	[iOS (16, 0), MacCatalyst (16, 0), NoTV, NoMac]
	[Native]
	public enum PKShareSecureElementPassResult : long {
		Canceled,
		Shared,
		Failed,
	}

	[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum PKVehicleConnectionErrorCode : long {
		Unknown = 0,
		SessionUnableToStart,
		SessionNotActive,
	}

	[iOS (16, 0), Mac (13, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum PKVehicleConnectionSessionConnectionState : long {
		Disconnected = 0,
		Connected,
		Connecting,
		FailedToConnect,
	}

	[iOS (17, 0), Mac (14, 0), TV (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum PKApplePayLaterAvailability : long {
		Available,
		UnavailableItemIneligible,
		UnavailableRecurringTransaction,
	}

	[NoTV, Mac (15, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	[ErrorDomain ("PKDisbursementErrorDomain")]
	public enum PKDisbursementErrorCode : long {
		UnknownError = -1,
		UnsupportedCardError = 1,
		RecipientContactInvalidError,
	}

	[NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[Native]
	public enum PKPayLaterAction : long {
		LearnMore = 0,
		Calculator,
	}

	[NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[Native]
	public enum PKPayLaterDisplayStyle : long {
		Standard = 0,
		Badge,
		Checkout,
		Price,
	}

	[Static]
	[Internal]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	interface PKMerchantCategoryCodeValues {
		[Field ("PKMerchantCategoryCodeNone")]
		short None { get; }
	}
}
