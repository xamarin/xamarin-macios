// Copyright 2012-2014 Xamarin Inc. All rights reserved.

using System;
using Foundation;
using ObjCRuntime;

namespace PassKit {

	// untyped enum -> PKError.h
	// This never seemed to be deprecatd, yet in iOS8 it's obsoleted
	[Obsoleted (PlatformName.iOS, 8, 0)]
	[iOS (6,0)]
	public enum PKErrorCode {
		None = 0,
		Unknown = 1,
		NotEntitled,
		PermissionDenied, // new in iOS7
	}

	// NSInteger -> PKPass.h
	[iOS (6,0)]
	[ErrorDomain ("PKPassKitErrorDomain")]
	[Native]
	public enum PKPassKitErrorCode : long {
		Unknown = -1,
		None = 0,
		InvalidData = 1,
		UnsupportedVersion,
		InvalidSignature,
#if !XAMCORE_2_0
		[Obsolete ("renamed to InvalidSignature")] // after betas?
		CertificateRevoked = InvalidSignature,
#endif
		[iOS (8,0)]
		NotEntitled
	}

	// NSInteger -> PKPassLibrary.h
	[iOS (7,0)]
	[Native]
	public enum PKPassLibraryAddPassesStatus : long {
		DidAddPasses,
		ShouldReviewPasses,
		DidCancelAddPasses
	}

	[Native]
	public enum PKPassType : ulong {
		Barcode, Payment,
		// Any = ~0
	}

	[Watch (3,0)]
	[Native]
	public enum PKPaymentAuthorizationStatus : long {
		Success,
		Failure,

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentBillingAddressInvalidError'.")]
		InvalidBillingPostalAddress,

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentShippingAddressInvalidError'.")]
		InvalidShippingPostalAddress,

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'Failure' and 'PKPaymentRequest.CreatePaymentContactInvalidError'.")]
		InvalidShippingContact,

		[iOS (9,2)]
		PinRequired,
		[iOS (9,2)]
		PinIncorrect,
		[iOS (9,2)]
		PinLockout
	}

	[Native]
	public enum PKPaymentPassActivationState : ulong {
		Activated, RequiresActivation, Activating, Suspended, Deactivated
	}

	[Watch (3,0)]
	[Native]
	public enum PKMerchantCapability : ulong {
		ThreeDS = 1 << 0,
		EMV = 1 << 1,
		Credit = 1 << 2,
		Debit = 1 << 3
	}

	[Watch (3,0)]
	[Deprecated (PlatformName.iOS, 11,0, message: "Use 'PKContactField' instead.")]
	[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'PKContactField' instead.")]
	[Native]
	[Flags]
	public enum PKAddressField : ulong {
		None = 0,
		PostalAddress = 1 << 0,
		Phone = 1 << 1,
		Email = 1 << 2,
		[iOS (8,3)]
		Name = 1 << 3,
		All = PostalAddress|Phone|Email|Name
	}

	[NoWatch]
	[iOS (8,3)]
	[Native]
	public enum PKPaymentButtonStyle : long {
		White,
		WhiteOutline,
		Black,
	}

	[NoWatch]
	[iOS (8,3)]
	[Native]
	public enum PKPaymentButtonType : long {
		Plain,
		Buy,
		[iOS (9,0)]
		SetUp,
		[iOS (10,0)]
		InStore,
		[iOS (10,2)]
		Donate,
	}

	[Watch (3,0)]
	[iOS (8,3)]
	[Native]
	public enum PKShippingType : ulong {
		Shipping,
		Delivery,
		StorePickup,
		ServicePickup,
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAddPaymentPassError : long
	{
		Unsupported,
		UserCancelled,
		SystemCancelled
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAutomaticPassPresentationSuppressionResult : ulong
	{
		NotSupported = 0,
		AlreadyPresenting,
		Denied,
		Cancelled,
		Success
	}

	[Watch (3,0)]
	[iOS (9,0)]
	[Native]
	public enum PKPaymentMethodType : ulong
	{
		Unknown = 0,
		Debit,
		Credit,
		Prepaid,
		Store
	}

	[Watch (3,0)]
	[iOS (9,0)]
	[Native]
	public enum PKPaymentSummaryItemType : ulong
	{
		Final,
		Pending
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAddPassButtonStyle : long {
		Black = 0,
		Outline
	}

	[Watch (4,0)][iOS (11,0)]
	[ErrorDomain ("PKPaymentErrorDomain")]
	[Native]
	public enum PKPaymentErrorCode : long {
		Unknown = -1,
		ShippingContactInvalid = 1,
		BillingContactInvalid,
		ShippingAddressUnserviceable,
	}

	[iOS (12,0)]
	[NoWatch]
	[Native]
	public enum PKAddPaymentPassStyle : ulong {
		Payment,
		Access,
	}
}
