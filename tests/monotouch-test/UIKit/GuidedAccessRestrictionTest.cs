//
// Unit tests for UIGuidedAccessRestriction
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GuidedAccessRestrictionTest {

		[Test]
		public void GetState ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			Assert.That (UIGuidedAccessRestriction.GetState (null), Is.EqualTo (UIGuidedAccessRestrictionState.Allow), "null");
		}

#if !__TVOS__
		[Test]
		public void GuidedAccessConfigureAccessibilityFeaturesTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 2);

			var gotError = false;
			var didSuccess = true;
			var done = false;

			TestRuntime.RunAsync (TimeSpan.FromSeconds (30), async () => {
				try {
					var res = await UIGuidedAccessRestriction.ConfigureAccessibilityFeaturesAsync (UIGuidedAccessAccessibilityFeature.Zoom, true);
					gotError = res.Error is not null; // We expect an error back from the API call.
					didSuccess = res.Success; // We expect false since monotouch-tests app is not run in kiosk mode.
				} finally {
					done = true;
				}
			}, () => done);

			Assert.NotNull (gotError, "Error was null.");
			Assert.IsFalse (didSuccess, "Somehow this succeeded, are we running monotouch-tests app in kiosk mode?");
		}
#endif // !__TVOS__
	}
}

#endif // !__WATCHOS__
