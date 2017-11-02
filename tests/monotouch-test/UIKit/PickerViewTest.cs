// Copyright 2011,2015 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PickerViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
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
