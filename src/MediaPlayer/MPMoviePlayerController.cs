// Copyright 2014 Xamarin, Inc.

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
#if HAS_IAD && !XAMCORE_4_0
using iAd;
#endif

#nullable enable

namespace MediaPlayer {

#if HAS_IAD && !XAMCORE_4_0
	public partial class MPMoviePlayerController {
		// This is a [Category] -> C# extension method (see adlib.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
#if !NET
		[iOS (7,0)]
#endif
		[Obsoleted (PlatformName.iOS, 15,0, PlatformArchitecture.None, Constants.iAdRemoved)]
		static public void PrepareForPrerollAds ()
		{
		}
	}
#endif
}
