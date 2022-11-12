// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UITableView tv = new UITableView (frame)) {
				Assert.That (tv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // !__WATCHOS__
