// Copyright 2012-2014 Xamarin Inc. All rights reserved.

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.PassKit {

	// untyped enum -> PKError.h
	// This never seemed to be deprecatd, yet in iOS8 it's obsoleted
	[Availability (Obsoleted = Platform.iOS_8_0)]
	[iOS (6,0)]
	public enum PKErrorCode {
		None = 0,
		Unknown = 1,
		NotEntitled,
		PermissionDenied, // new in iOS7
	}

	// NSInteger -> PKPass.h
	[iOS (6,0)]
	[Native]
	public enum PKPassKitErrorCode : nint {
		Unknown = -1,
		None = 0,
		InvalidData = 1,
		UnsupportedVersion,
#if XAMCORE_2_0
		InvalidSignature,
#else
		[Obsolete ("renamed to InvalidSignature")] // after betas?
		CertificateRevoked,
		InvalidSignature = CertificateRevoked,
#endif
		[iOS (8,0)]
		NotEntitled
	}

	// NSInteger -> PKPassLibrary.h
	[iOS (7,0)]
	[Native]
	public enum PKPassLibraryAddPassesStatus : nint {
		DidAddPasses,
		ShouldReviewPasses,
		DidCancelAddPasses
	}

	[Native]
	public enum PKPassType : nuint {
		Barcode, Payment,
		// Any = ~0
	}

	[Watch (3,0)]
	[Native]
	public enum PKPaymentAuthorizationStatus : nint {
		Success,
		Failure,
		InvalidBillingPostalAddress,
		InvalidShippingPostalAddress,
		InvalidShippingContact,
		[iOS (9,2)]
		PinRequired,
		[iOS (9,2)]
		PinIncorrect,
		[iOS (9,2)]
		PinLockout
	}

	[Native]
	public enum PKPaymentPassActivationState : nuint {
		Activated, RequiresActivation, Activating, Suspended, Deactivated
	}

	[Watch (3,0)]
	[Native]
	public enum PKMerchantCapability : nuint {
		ThreeDS = 1 << 0,
		EMV = 1 << 1,
		Credit = 1 << 2,
		Debit = 1 << 3
	}

	[Watch (3,0)]
	[Native]
	[Flags]
	public enum PKAddressField : nuint {
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
	public enum PKPaymentButtonStyle : nint {
		White,
		WhiteOutline,
		Black,
	}

	[NoWatch]
	[iOS (8,3)]
	[Native]
	public enum PKPaymentButtonType : nint {
		Plain,
		Buy,
		[iOS (9,0)]
		SetUp,
		[iOS (10,0)]
		InStore,
	}

	[Watch (3,0)]
	[iOS (8,3)]
	[Native]
	public enum PKShippingType : nuint {
		Shipping,
		Delivery,
		StorePickup,
		ServicePickup,
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAddPaymentPassError : nint
	{
		Unsupported,
		UserCancelled,
		SystemCancelled
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAutomaticPassPresentationSuppressionResult : nuint
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
	public enum PKPaymentMethodType : nuint
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
	public enum PKPaymentSummaryItemType : nuint
	{
		Final,
		Pending
	}

	[NoWatch]
	[iOS (9,0)]
	[Native]
	public enum PKAddPassButtonStyle : nint {
		Black = 0,
		Outline
	}
}
