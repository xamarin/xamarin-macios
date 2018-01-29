//
// iAd.cs: definitions
//
// Author:
//   Miguel de Icaza
//
// Copyright 2011-2014, 2016 Xamarin Inc
//

using XamCore.ObjCRuntime;

namespace XamCore.iAd {

	// NSInteger -> ADBannerView.h
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Native]
	[ErrorDomain ("ADErrorDomain")]
	public enum ADError : nint {
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
	public enum ADAdType : nint {
		Banner, MediumRectangle
	}

	// NSInteger -> UIViewControlleriAdAdditions.h
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Native]
	public enum ADInterstitialPresentationPolicy : nint {
		None = 0,
		Automatic,
		Manual,
	}

	[iOS (9,0)]
	[Native]
	[ErrorDomain ("ADClientErrorDomain")]
	public enum ADClientError : nint {
		Unknown = 0,
		LimitAdTracking = 1,
		[iOS (11,3)]
		MissingData = 2,
		[iOS (11,3)]
		CorruptResponse = 3,
	}
}