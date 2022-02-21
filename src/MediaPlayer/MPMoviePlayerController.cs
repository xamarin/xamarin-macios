// Copyright 2014 Xamarin, Inc.

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;
#if HAS_IAD && !NET
using iAd;
#endif

#nullable enable

namespace MediaPlayer {

#if HAS_IAD && !NET
	public partial class MPMoviePlayerController {
		// This is a [Category] -> C# extension method (see adlib.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("tvos")]
#if IOS
		[Obsolete ("Starting with ios9.0 use 'AVPlayerViewController' (AVKit) instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
		[Obsolete ("Starting with ios15.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (7,0)]
		[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
		static public void PrepareForPrerollAds ()
		{
		}
	}
#endif // HAS_IAD && !NET
}
