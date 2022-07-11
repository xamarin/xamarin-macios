//
// Unit tests for AVPlayerViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if HAS_IAD

using System;
using AVKit;
using Foundation;
using UIKit;
using iAd;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PlayerViewControllerTest {

		[Test]
		public void PreparePrerollAds_New ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			AVPlayerViewController.PrepareForPrerollAds ();
		}
	}
}

#endif // HAS_IAD
