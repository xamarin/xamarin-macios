// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class TableViewCellTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UITableViewCell tvc = new UITableViewCell (frame)) {
				Assert.That (tvc.Frame.X, Is.EqualTo ((nfloat) 0.0f), "X");
				Assert.That (tvc.Frame.Y, Is.EqualTo ((nfloat) 0.0f), "Y");
				// whatever supplied value X and Y are set to 0.0 by the cell
				// Width and Height are set by the cell (e.g. 320 x 44 for the iPhone)
			}
		}
	}
}

#endif // !__WATCHOS__
