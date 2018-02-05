// Copyright 2014 Xamarin, Inc.

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
#if IOS
using iAd;
#endif

namespace AVKit {
#if IOS
	public partial class AVPlayerViewController {

		// This is a [Category] -> C# extension method (see adlib.cs) but it targets on static selector
		// the resulting syntax does not look good in user code so we provide a better looking API
		// https://trello.com/c/iQpXOxCd/227-category-and-static-methods-selectors
		// note: we cannot reuse the same method name - as it would break compilation of existing apps
		[iOS (8,0)]
		static public void PrepareForPrerollAds ()
		{
			(null as AVPlayerViewController).PreparePrerollAds ();
		}
	}
#endif
}