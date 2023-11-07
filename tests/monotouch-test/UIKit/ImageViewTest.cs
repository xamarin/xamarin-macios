// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ImageViewTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UIImageView iv = new UIImageView (frame)) {
				Assert.That (iv.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void AnimationImages ()
		{
			using (var i1 = new UIImage ())
			using (var i2 = new UIImage ())
			using (var v = new UIImageView ()) {
				v.AnimationImages = new UIImage [] { i1, i2 };
				// no need for [PostGet] since it does not change other properties
				Assert.Null (v.Image, "Image");
				Assert.Null (v.HighlightedImage);
			}
		}

		[Test]
		public void HighlightedAnimationImages_BackingFields ()
		{
			using (var i1 = new UIImage ())
			using (var i2 = new UIImage ())
			using (var v = new UIImageView ()) {
				v.HighlightedAnimationImages = new UIImage [] { i1, i2 };
				// no need for [PostGet] since it does not change other properties
				Assert.Null (v.Image, "Image");
				Assert.Null (v.HighlightedImage);
			}
		}
	}
}

#endif // !__WATCHOS__
