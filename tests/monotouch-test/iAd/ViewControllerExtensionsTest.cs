//
// Unit tests for iAd's UIViewController extensions methods
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;

#if XAMCORE_2_0
using Foundation;
using UIKit;
using iAd;
#else
using MonoTouch.Foundation;
using MonoTouch.iAd;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreImage {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ViewControllerExtensionsTest {

		[Test]
		[Ignore ("got the original error back, needs more investigation")]
		public void PresentationPolicy ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);

			using (var vc = new UIViewController ()) {
				// without calling PrepareInterstitialAds we get a *unrecognized selector* native exception
				// http://forums.xamarin.com/discussion/9160/setinterstitialpresentationpolicy-gives-me-nsinvalidargumentexception
				UIViewController.PrepareForInterstitialAds ();

				Assert.That (vc.GetInterstitialPresentationPolicy (), Is.EqualTo (ADInterstitialPresentationPolicy.None), "default");

				vc.SetInterstitialPresentationPolicy (ADInterstitialPresentationPolicy.Automatic);
				Assert.That (vc.GetInterstitialPresentationPolicy (), Is.EqualTo (ADInterstitialPresentationPolicy.Automatic), "Automatic");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
