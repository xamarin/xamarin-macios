//
// iAd.cs: definitions
//
// Author:
//   Miguel de Icaza
//
// Copyright 2011-2014, 2016 Xamarin Inc
//

#if !NET

using System;
using ObjCRuntime;

namespace iAd {

	// NSInteger -> ADBannerView.h
#if NET
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#if IOS
	[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	[Native]
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
#if NET
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#if IOS
	[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	[Native]
	public enum ADAdType : long {
		Banner, MediumRectangle
	}

	// NSInteger -> UIViewControlleriAdAdditions.h
#if NET
	[UnsupportedOSPlatform ("ios10.0")]
#if IOS
	[Obsolete ("Starting with ios10.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#if IOS
	[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 10, 0)]
	[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	[Native]
	public enum ADInterstitialPresentationPolicy : long {
		None = 0,
		Automatic,
		Manual,
	}

#if NET
	[SupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[iOS (9,0)]
	[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	[Native]
	[ErrorDomain ("ADClientErrorDomain")]
	public enum ADClientError : long {
		Unknown = 0,
		TrackingRestrictedOrDenied = 1,
		[Obsolete ("Use 'TrackingRestrictedOrDenied' instead.")]
		LimitAdTracking = TrackingRestrictedOrDenied,
		MissingData = 2,
		CorruptResponse = 3,
		RequestClientError = 4,
		RequestServerError = 5,
		RequestNetworkError = 6,
		UnsupportedPlatform = 7,
	}
}

#endif // !NET
