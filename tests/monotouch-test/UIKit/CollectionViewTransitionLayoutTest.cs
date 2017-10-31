// Copyright 2014 Xamarin Inc. All rights reserved

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
	[Preserve (AllMembers = true)]
	public class CollectionViewTransitionLayoutTest {

		[Test]
		public void Ctor ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Inconclusive ("requires iOS 7.0+");

			using (var l1 = new UICollectionViewLayout ())
			using (var l2 = new UICollectionViewLayout ())
			using (var tl = new UICollectionViewTransitionLayout (l1, l2)) {
				// interesting ctor for the linker (two [PostGet])
				Assert.AreSame (tl.CurrentLayout, l1, "CurrentLayout");
				Assert.AreSame (tl.NextLayout, l2, "NextLayout");
			}
		}
	}
}

#endif // !__WATCHOS__
