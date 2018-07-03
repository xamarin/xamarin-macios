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
	public class CollectionViewControllerTest {

		[Test]
		public void Ctor ()
		{
			using (var l = new UICollectionViewLayout ())
			// interesting ctor for the linker: a [PostSnippet] directly on the backing field is needed
			using (var c = new UICollectionViewController (l)) {
				// that's because Apple did not expose the init* argument as a property until 7.0
				if (TestRuntime.CheckiOSSystemVersion (7, 0, throwIfOtherPlatform: false))
					Assert.AreSame (l, c.Layout, "Layout");
			}
		}
	}
}

#endif // !__WATCHOS__
