// Copyright 2014 Xamarin Inc. All rights reserved

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
	public class CollectionViewTransitionLayoutTest {

		[Test]
		public void Ctor ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

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
