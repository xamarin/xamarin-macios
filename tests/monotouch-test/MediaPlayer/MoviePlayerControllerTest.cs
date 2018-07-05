//
// Unit tests for MPMoviePlayerController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
using UIKit;
using iAd;
using ObjCRuntime;
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
		
		[Test]
		public void PreparePrerollAds_New ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0);

			// NSInvalidArgumentException +[MPMoviePlayerController preparePrerollAds]: unrecognized selector sent to class 0x109c46b48
			if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 10, 0))
				Assert.Ignore ("Broken on iOS 10 beta 3");
			
			MPMoviePlayerController.PrepareForPrerollAds ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
