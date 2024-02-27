// Copyright 2011,2015 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PickerViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIPickerView pv = new UIPickerView (frame)) {
				Assert.That (pv.Frame.X, Is.EqualTo (frame.X), "X");
				Assert.That (pv.Frame.Y, Is.EqualTo (frame.Y), "Y");
				Assert.That (pv.Frame.Width, Is.EqualTo (frame.Width), "Width");
				// Height is set by Picker (e.g. 162 for the iPhone)
			}
		}

		[Test]
		public void ConformsTo ()
		{
			using (UIPickerView pv = new UIPickerView ()) {
				Assert.True (pv.ConformsToProtocol (Protocol.GetHandle ("UITableViewDataSource")), "UITableViewDataSource");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
