//
// Unit tests for MPMoviePlayerController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using UIKit;
using iAd;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using MonoTouch.iAd;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MoviePlayerControllerTest {

#if !XAMCORE_2_0
		[Test]
		public void PreparePrerollAds_Old ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7,0))
				Assert.Inconclusive ("Requires 7.0+");

			MPMoviePlayerController mpc = null;
			mpc.PreparePrerollAds ();
		}
#endif

		[Test]
		public void PreparePrerollAds_New ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (7,0))
				Assert.Inconclusive ("Requires 7.0+");

			MPMoviePlayerController.PrepareForPrerollAds ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
