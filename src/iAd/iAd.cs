//
// iAd.cs: definitions
//
// Author:
//   Miguel de Icaza
//
// Copyright 2011-2014, 2016 Xamarin Inc
//

using ObjCRuntime;

namespace iAd {

	// NSInteger -> ADBannerView.h
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Native]
	[ErrorDomain ("ADErrorDomain")]
	public enum ADError : long {
		Unknown,
		ServerFailure,
		LoadingThrottled,
		InventoryUnavailable,
		ConfigurationError,
		BannerVisibleWithoutContent,
		ApplicationInactive,
		AdUnloaded,
		AssetLoadFailure,
		AdResponseValidateFailure,
		AdAssetLoadPending,
	}

	// NSInteger -> ADBannerView.h
	[Deprecated (PlatformName.iOS, 10, 0)]
	[iOS (6,0)] 
	[Native]
	public enum ADAdType : long {
		Banner, MediumRectangle
	}

	// NSInteger -> UIViewControlleriAdAdditions.h
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Native]
	public enum ADInterstitialPresentationPolicy : long {
		None = 0,
		Automatic,
		Manual,
	}

	[iOS (9,0)]
	[Native]
	[ErrorDomain ("ADClientErrorDomain")]
	public enum ADClientError : long {
		Unknown = 0,
		LimitAdTracking = 1
	}
}