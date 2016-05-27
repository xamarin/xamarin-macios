//
// Unit tests for PKObject
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using PassKit;
#else
using MonoTouch.Foundation;
using MonoTouch.PassKit;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ObjectTest {
		[Test]
		public void Constructor ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (6, 0))
				Assert.Inconclusive ("PassKit is new in 6.0");

			if (TestRuntime.CheckSystemAndSDKVersion (8, 0)) {
				Assert.DoesNotThrow (() => new PKObject ());
			} else {
				Assert.Throws<Exception> (() => new PKObject ());
			}
		}
	}
}

#endif // !__TVOS__
