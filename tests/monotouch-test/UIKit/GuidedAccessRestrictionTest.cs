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
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GuidedAccessRestrictionTest {

		[Test]
		public void GetState ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("Requires iOS7 or later");

			Assert.That (UIGuidedAccessRestriction.GetState (null), Is.EqualTo (UIGuidedAccessRestrictionState.Allow), "null");
		}
	}
}

#endif // !__WATCHOS__
