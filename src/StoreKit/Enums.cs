using Foundation;
using ObjCRuntime;
using System;

namespace StoreKit {

	// typedef NSInteger SKPaymentTransactionState;
	// StoreKit.framework/Headers/SKPaymentTransaction.h
#if !NET
	[Watch (6, 2)]
#endif
	[Native]
	public enum SKPaymentTransactionState : long {
		Purchasing,
		Purchased,
		Failed,  
		Restored,
#if NET
		[SupportedOSPlatform ("ios8.0")]
#else
		[iOS (8,0)]
#endif
Deferred
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
#if !NET
	[Watch (6, 2)]
#endif
	[Native]
	public enum SKDownloadState : long {
		Waiting, Active, Paused, Finished, Failed, Cancelled
	}

#if NET
	[SupportedOSPlatform ("ios9.3")]
#else
	[Watch (7,0)]
	[iOS (9,3)]
#endif
	[Native]
	public enum SKCloudServiceAuthorizationStatus : long {
		NotDetermined,
		Denied,
		Restricted,
		Authorized
	}

#if NET
	[SupportedOSPlatform ("ios9.3")]
#else
	[Watch (7,0)]
	[iOS (9,3)]
#endif
	[Native]
	public enum SKCloudServiceCapability : ulong {
		None = 0,
		MusicCatalogPlayback = 1 << 0,
#if NET
		[SupportedOSPlatform ("ios10.1")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[NoTV]
		[iOS (10,1)]
#endif
		MusicCatalogSubscriptionEligible = 1 << 1,
		AddToCloudMusicLibrary = 1 << 8
	}

#if NET
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos11.0")]
#else
	[iOS (11,0)]
	[TV (11,0)]
	[Mac (11,0)]
	[NoWatch]
#endif
	[Native]
	public enum SKProductStorePromotionVisibility : long {
		Default,
		Show,
		Hide,
	}

#if NET
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("macos10.13.2")]
#else
	[Watch (6, 2)]
	[iOS (11,2)]
	[TV (11,2)]
	[Mac (10,13,2)]
#endif
	[Native]
	public enum SKProductPeriodUnit : ulong {
		Day,
		Week,
		Month,
		Year,
	}

#if NET
	[SupportedOSPlatform ("ios11.2")]
	[SupportedOSPlatform ("tvos11.2")]
	[SupportedOSPlatform ("macos10.13.2")]
#else
	[Watch (6, 2)]
	[iOS (11,2)]
	[TV (11,2)]
	[Mac (10,13,2)]
#endif
	[Native]
	public enum SKProductDiscountPaymentMode : ulong {
		PayAsYouGo,
		PayUpFront,
		FreeTrial,
	}

#if NET
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
#else
	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (14,0)]
	[MacCatalyst (14,0)]
#endif
	[Native]
	public enum SKOverlayPosition : long {
		SKOverlayPositionBottom = 0,
		Raised = 1,
	}
}
