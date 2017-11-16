//
// UIGestureRecognizer Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

#if !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class GestureRecognizerTest {

		[Test]
		public void Null ()
		{
			using (var gr = new UIGestureRecognizer (Null)) {
				// ensure documented null-friendly methods actually are before releasing them in the wild
				gr.LocationInView (null);
				// can't call LocationOfTouch, 0 is not valid if there's no touch event
				// gr.LocationOfTouch (0, null);
				gr.RemoveTarget (null, null);
			}
		}
	}
}

#endif // !__WATCHOS__
