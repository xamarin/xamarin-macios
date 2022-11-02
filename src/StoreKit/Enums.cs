using Foundation;
using ObjCRuntime;
using System;

namespace StoreKit {

	// typedef NSInteger SKPaymentTransactionState;
	// StoreKit.framework/Headers/SKPaymentTransaction.h
	[Watch (6, 2)]
	[Native]
	public enum SKPaymentTransactionState : long {
		Purchasing,
		Purchased,
		Failed,  
		Restored,
		[iOS (8,0)]Deferred
	}

	// untyped enum and not used in API - so it _could_ be an `int`
	// OTOH it's meant to be used with NSError.Code which is an NSInteger/nint
	// StoreKit.framework/Headers/SKError.h
	[Native ("SKErrorCode")]
	[ErrorDomain ("SKErrorDomain")]
	public enum SKError : long {
		Unknown,
		ClientInvalid,
		PaymentCancelled,
		PaymentInvalid,
		PaymentNotAllowed,
		ProductNotAvailable,
		// iOS 9.3
		CloudServicePermissionDenied,
		CloudServiceNetworkConnectionFailed,
		// iOS 10.3
		CloudServiceRevoked,
#if !NET
		[Obsolete ("Use 'SKError.CloudServiceRevoked' instead.")]
		Revoked = CloudServiceRevoked,
#endif
		// iOS 12.2
		PrivacyAcknowledgementRequired,
		UnauthorizedRequestData,
		InvalidOfferIdentifier,
		InvalidSignature,
		MissingOfferParams,
		InvalidOfferPrice,
		OverlayCancelled = 15,

		// iOS 14
		OverlayInvalidConfiguration = 16,
		OverlayTimeout = 17,
		IneligibleForOffer = 18,
		UnsupportedPlatform = 19,
		// iOS 14.5
		OverlayPresentedInBackgroundScene = 20,
	}

	// typedef NSInteger SKDownloadState;
	// StoreKit.framework/Headers/SKDownload.h
	[Watch (6, 2)]
	[Native]
	public enum SKDownloadState : long {
		Waiting, Active, Paused, Finished, Failed, Cancelled
	}

	[Watch (7,0)]
	[iOS (9,3)]
	[Native]
	public enum SKCloudServiceAuthorizationStatus : long {
		NotDetermined,
		Denied,
		Restricted,
		Authorized
	}

	[Watch (7,0)]
	[iOS (9,3)]
	[Native]
	public enum SKCloudServiceCapability : ulong {
		None = 0,
		MusicCatalogPlayback = 1 << 0,
		[NoTV, iOS (10,1)]
		MusicCatalogSubscriptionEligible = 1 << 1,
		AddToCloudMusicLibrary = 1 << 8
	}

	[iOS (11,0)][TV (11,0)][Mac (11,0)][NoWatch]
	[Native]
	public enum SKProductStorePromotionVisibility : long {
		Default,
		Show,
		Hide,
	}

	[Watch (6, 2), iOS (11,2), TV (11,2), Mac (10,13,2)]
	[Native]
	public enum SKProductPeriodUnit : ulong {
		Day,
		Week,
		Month,
		Year,
	}

	[Watch (6, 2), iOS (11,2), TV (11,2), Mac (10,13,2)]
	[Native]
	public enum SKProductDiscountPaymentMode : ulong {
		PayAsYouGo,
		PayUpFront,
		FreeTrial,
	}

	[NoWatch, NoTV, NoMac, iOS (14,0)]
	[MacCatalyst (14,0)]
	[Native]
	public enum SKOverlayPosition : long {
		SKOverlayPositionBottom = 0,
		Raised = 1,
	}
}
