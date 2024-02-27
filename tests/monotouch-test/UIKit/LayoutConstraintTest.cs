// Copyright 2012-2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LayoutConstraintTest {

		[Test]
		public void Create ()
		{
			using (var view = new UIView ()) {
				NSLayoutConstraint.Create (view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 5).Dispose ();
			}
		}

		[Test]
		public void FromVisualFormat_NullMetrics ()
		{
			using (var dict = new NSMutableDictionary ())
			using (var b0 = UIButton.FromType (UIButtonType.InfoDark))
			using (var b1 = UIButton.FromType (UIButtonType.InfoLight)) {
				dict ["button0"] = b0;
				dict ["button1"] = b1;
				var constaints = NSLayoutConstraint.FromVisualFormat ("[button0]-20-[button1]", NSLayoutFormatOptions.AlignAllBaseline, null, dict);
				Assert.That (constaints.Length, Is.EqualTo (2), "constaints");
			}
		}

		[Test]
		public void FromVisualFormat ()
		{
			using (var metrics = new NSMutableDictionary ())
			using (var dict = new NSMutableDictionary ())
			using (var b0 = UIButton.FromType (UIButtonType.InfoDark))
			using (var b1 = UIButton.FromType (UIButtonType.InfoLight)) {
				dict ["button0"] = b0;
				dict ["button1"] = b1;
				var constaints = NSLayoutConstraint.FromVisualFormat ("[button0]-20-[button1]", NSLayoutFormatOptions.AlignAllBaseline, metrics, dict);
				Assert.NotNull (constaints);
			}
		}
	}
}

#endif // !__WATCHOS__
