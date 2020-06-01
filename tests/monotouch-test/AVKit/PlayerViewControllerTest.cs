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
using AVKit;
using Foundation;
using UIKit;
using iAd;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.AVKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayerViewControllerTest {

		[Test]
		public void PreparePrerollAds_New ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);

			AVPlayerViewController.PrepareForPrerollAds ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
