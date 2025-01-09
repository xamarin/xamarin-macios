using Foundation;
using ObjCRuntime;
using System;

namespace StoreKit {

	// typedef NSInteger SKPaymentTransactionState;
	// StoreKit.framework/Headers/SKPaymentTransaction.h
	/// <summary>Transaction state.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKPaymentTransactionState : long {
		Purchasing,
		Purchased,
		Failed,
		Restored,
		[MacCatalyst (13, 1)]
		Deferred,
	}

	// untyped enum and not used in API - so it _could_ be an `int`
	// OTOH it's meant to be used with NSError.Code which is an NSInteger/nint
	// StoreKit.framework/Headers/SKError.h
	/// <summary>An enumeration whose values specify various errors relating to StoreKit.</summary>
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
	/// <summary>An enumeration whose values specify the state of an <see cref="T:StoreKit.SKDownload" /> object. Used with the <see cref="P:StoreKit.SKDownload.DownloadState" /> property.</summary>
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKDownloadState : long {
		Waiting,
		Active,
		Paused,
		Finished,
		Failed,
		Cancelled,
	}

	/// <summary>Enumerates authorization states for music store access.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKCloudServiceAuthorizationStatus : long {
		NotDetermined,
		Denied,
		Restricted,
		Authorized,
	}

	/// <summary>Enumerates flags that describe the user's local music library capabilities..</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKCloudServiceCapability : ulong {
		None = 0,
		MusicCatalogPlayback = 1 << 0,
		[MacCatalyst (13, 1)]
		MusicCatalogSubscriptionEligible = 1 << 1,
		AddToCloudMusicLibrary = 1 << 8
	}

	/// <summary>Enumerates visiblity settings for a promoted product.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKProductStorePromotionVisibility : long {
		Default,
		Show,
		Hide,
	}

	/// <summary>Defines the natural period for a subscription.</summary>
	[Watch (6, 2)]
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKProductPeriodUnit : ulong {
		Day,
		Week,
		Month,
		Year,
	}

	/// <summary>Enumerates how a discount price is applied.</summary>
	[Watch (6, 2)]
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.WatchOS, 11, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKProductDiscountPaymentMode : ulong {
		PayAsYouGo,
		PayUpFront,
		FreeTrial,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum SKOverlayPosition : long {
		SKOverlayPositionBottom = 0,
		Raised = 1,
	}

	[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
	public enum SKAdNetworkCoarseConversionValue {
		[Field ("SKAdNetworkCoarseConversionValueHigh")]
		High,
		[Field ("SKAdNetworkCoarseConversionValueMedium")]
		Medium,
		[Field ("SKAdNetworkCoarseConversionValueLow")]
		Low,
	}
}
