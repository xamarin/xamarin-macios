// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SwitchTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UISwitch s = new UISwitch (frame)) {
				Assert.That (s.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (s.Frame.Y, Is.EqualTo (frame.Y), "Y");
				// Width and Height are set by the Switch (e.g. 79 x 27 for the iPhone)
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
