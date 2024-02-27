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
	public class LabelTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UILabel l = new UILabel (frame)) {
				Assert.That (l.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void HighlightedTextColor ()
		{
			UILabel label = new UILabel ();
			Assert.Null (label.HighlightedTextColor, "HighlightedTextColor/default");
			label.HighlightedTextColor = UIColor.Blue;
			Assert.That (label.HighlightedTextColor, Is.EqualTo (UIColor.Blue), "HighlightedTextColor/blue");
			label.HighlightedTextColor = null;
			Assert.Null (label.HighlightedTextColor, "HighlightedTextColor/null");
		}
	}
}

#endif // !__WATCHOS__
