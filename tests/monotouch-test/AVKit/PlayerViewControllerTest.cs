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

		//[Test]
		public void PreparePrerollAds_New ()
		{
			TestRuntime.AssertiOSSystemVersion (8, 0, throwIfOtherPlatform: false);

			AVPlayerViewController.PrepareForPrerollAds ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
