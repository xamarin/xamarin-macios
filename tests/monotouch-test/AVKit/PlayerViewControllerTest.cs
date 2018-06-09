//
// Unit tests for AVPlayerViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using AVKit;
using Foundation;
using UIKit;
using iAd;
#else
using MonoTouch.AVKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.iAd;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AVKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayerViewControllerTest {

#if !XAMCORE_2_0
		[Test]
		public void PreparePrerollAds_Old ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8,0))
				Assert.Inconclusive ("Requires 8.0+");

			(null as AVPlayerViewController).PreparePrerollAds ();
		}
#endif
		//[Test]
		public void PreparePrerollAds_New ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (8,0))
				Assert.Inconclusive ("Requires 8.0+");

			AVPlayerViewController.PrepareForPrerollAds ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
