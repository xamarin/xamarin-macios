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
	public class ProgressViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIProgressView pv = new UIProgressView (frame)) {
				Assert.That (pv.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (pv.Frame.Y, Is.EqualTo (frame.Y), "Y");
				Assert.That (pv.Frame.Width, Is.EqualTo (frame.Width), "Width");
				// Height is set by the ProgressView (e.g. 9 for the iPhone)
			}
		}
	}
}

#endif // !__WATCHOS__
