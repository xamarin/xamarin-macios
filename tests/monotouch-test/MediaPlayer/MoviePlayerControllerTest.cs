//
// Unit tests for MPMoviePlayerController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if HAS_IAD

using System;
using Foundation;
using MediaPlayer;
using UIKit;
using iAd;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MediaPlayer {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MoviePlayerControllerTest {
		
		[Test]
		public void PreparePrerollAds_New ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0);

			// NSInvalidArgumentException +[MPMoviePlayerController preparePrerollAds]: unrecognized selector sent to class 0x109c46b48
			if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 10, 0))
				Assert.Ignore ("Broken on iOS 10 beta 3");
			
			MPMoviePlayerController.PrepareForPrerollAds ();
		}
	}
}

#endif // HAS_IAD
