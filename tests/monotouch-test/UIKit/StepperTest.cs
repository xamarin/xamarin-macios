// Copyright 2011-2012 Xamarin Inc. All rights reserved

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
	public class StepperTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIStepper s = new UIStepper (frame)) {
				Assert.That (s.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (s.Frame.Y, Is.EqualTo (frame.Y), "Y");
				// Width and Height are set by the Slider (e.g. 94 x 27 for the iPhone)
			}
		}

		[Test]
		public void BackgroundImage ()
		{
			using (var s = new UIStepper ()) {
				s.SetBackgroundImage (null, UIControlState.Application);
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
