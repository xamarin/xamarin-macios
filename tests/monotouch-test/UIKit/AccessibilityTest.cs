//
// Unit tests for UIAccessibility
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
	public class AccessibilityTest {

		[Test]
		public void RequestGuidedAccessSession ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			// should not affect execution since it needs to be a "supervised" device (and allowed in MDM)
			UIAccessibility.RequestGuidedAccessSession (true, delegate (bool didSuccess)
			{
				Assert.False (didSuccess, "devices are not supervised by default");
			});
			UIAccessibility.RequestGuidedAccessSession (false, null);
		}

		[Test]
		public void ButtonShapesEnabled ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			Assert.False (UIAccessibility.ButtonShapesEnabled);
		}

		[Test]
		public void PrefersCrossFadeTransitions ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			Assert.False (UIAccessibility.PrefersCrossFadeTransitions);
		}
	}
}

#endif // !__WATCHOS__
