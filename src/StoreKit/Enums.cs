using ObjCRuntime;

namespace StoreKit {

	// typedef NSInteger SKPaymentTransactionState;
	// StoreKit.framework/Headers/SKPaymentTransaction.h
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
	[Native]
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
		Revoked,
		// iOS 12.2
		InvalidOfferIdentifier,
		InvalidSignature,
		MissingOfferParams,
		InvalidOfferPrice,
		PrivacyAcknowledgementRequired,
	}

	// typedef NSInteger SKDownloadState;
	// StoreKit.framework/Headers/SKDownload.h 
	[Native]
	public enum SKDownloadState : long {
		Waiting, Active, Paused, Finished, Failed, Cancelled
	}

#if !MONOMAC || !XAMCORE_4_0
	[iOS (9,3)]
	[Native]
	public enum SKCloudServiceAuthorizationStatus : long {
		NotDetermined,
		Denied,
		Restricted,
		Authorized
	}

	[iOS (9,3)]
	[Native]
	public enum SKCloudServiceCapability : ulong {
		None = 0,
		MusicCatalogPlayback = 1 << 0,
		[NoTV, iOS (10,1)]
		MusicCatalogSubscriptionEligible = 1 << 1,
		AddToCloudMusicLibrary = 1 << 8
	}

	[iOS (11,0)][TV (11,0)][NoMac]
	[Native]
	public enum SKProductStorePromotionVisibility : long {
		Default,
		Show,
		Hide,
	}
#endif
	[iOS (11,2), TV (11,2), Mac (10,13,2)]
	[Native]
	public enum SKProductPeriodUnit : ulong {
		Day,
		Week,
		Month,
		Year,
	}

	[iOS (11,2), TV (11,2), Mac (10,13,2)]
	[Native]
	public enum SKProductDiscountPaymentMode : ulong {
		PayAsYouGo,
		PayUpFront,
		FreeTrial,
	}
}
