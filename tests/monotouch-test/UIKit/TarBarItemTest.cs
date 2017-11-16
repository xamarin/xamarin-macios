//
// Unit tests for UITabBarItem
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
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
	public class TabBarItemTest {
		[Test]
		public void Ctor_Defaults ()
		{
			using (UITabBarItem tbi = new UITabBarItem ()) {
				Assert.Null (tbi.BadgeValue, "BadgeValue");
				Assert.True (tbi.Enabled, "Enabled");
#if !__TVOS__
				Assert.Null (tbi.FinishedSelectedImage, "FinishedSelectedImage");
				Assert.Null (tbi.FinishedUnselectedImage, "FinishedUnselectedImage");
#endif
				Assert.Null (tbi.Image, "Image");
				Assert.That (tbi.ImageInsets, Is.EqualTo (UIEdgeInsets.Zero), "ImageInsets");
				Assert.That (tbi.Tag, Is.EqualTo ((nint) 0), "Tag");
				Assert.Null (tbi.Title, "Title");
				Assert.That (tbi.TitlePositionAdjustment.Horizontal, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Horizontal");
				Assert.That (tbi.TitlePositionAdjustment.Vertical, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Vertical");
			}
		}

		[Test]
		public void Ctor_2 ()
		{
			using (UITabBarItem tbi = new UITabBarItem (UITabBarSystemItem.Bookmarks, nint.MaxValue)) {
				Assert.Null (tbi.BadgeValue, "BadgeValue");
				Assert.True (tbi.Enabled, "Enabled");
#if !__TVOS__
				Assert.Null (tbi.FinishedSelectedImage, "FinishedSelectedImage");
				Assert.Null (tbi.FinishedUnselectedImage, "FinishedUnselectedImage");
#endif
				Assert.Null (tbi.Image, "Image");
				Assert.That (tbi.ImageInsets, Is.EqualTo (UIEdgeInsets.Zero), "ImageInsets");
				Assert.That (tbi.Tag, Is.EqualTo (nint.MaxValue), "Tag");
				Assert.Null (tbi.Title, "Title");
				Assert.That (tbi.TitlePositionAdjustment.Horizontal, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Horizontal");
				Assert.That (tbi.TitlePositionAdjustment.Vertical, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Vertical");
			}
		}
		
		[Test]
		public void Ctor_3 ()
		{
			using (UIImage img = new UIImage ())
			using (UITabBarItem tbi = new UITabBarItem ("title", img, nint.MinValue)) {
				Assert.Null (tbi.BadgeValue, "BadgeValue");
				Assert.True (tbi.Enabled, "Enabled");
#if !__TVOS__
				Assert.Null (tbi.FinishedSelectedImage, "FinishedSelectedImage");
				Assert.Null (tbi.FinishedUnselectedImage, "FinishedUnselectedImage");
#endif
				Assert.AreSame (tbi.Image, img, "Image");
				Assert.That (tbi.ImageInsets, Is.EqualTo (UIEdgeInsets.Zero), "ImageInsets");
				Assert.That (tbi.Tag, Is.EqualTo (nint.MinValue), "Tag");
				Assert.That (tbi.Title, Is.EqualTo ("title"), "Title");
				Assert.That (tbi.TitlePositionAdjustment.Horizontal, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Horizontal");
				Assert.That (tbi.TitlePositionAdjustment.Vertical, Is.EqualTo ((nfloat) 0f), "TitlePositionAdjustment.Vertical");
			}
		}

		[Test]
		public void Ctor_3a_Null ()
		{
			using (UIImage img = new UIImage ()) {
				using (UITabBarItem tbi1 = new UITabBarItem (null, img, nint.MinValue)) {
					Assert.Null (tbi1.Title, "Title-1a");
					Assert.AreSame (tbi1.Image, img, "Image-1a");
					tbi1.Title = "title";
					tbi1.Image = null;
					Assert.That (tbi1.Title, Is.EqualTo ("title"), "Title-1b");
					Assert.IsNull (tbi1.Image, "Image-1b");
				}
				using (UITabBarItem tbi2 = new UITabBarItem ("title", null, nint.MaxValue)) {
					Assert.That (tbi2.Title, Is.EqualTo ("title"), "Title-2a");
					Assert.Null (tbi2.Image, "Image-2a");
					tbi2.Title = null;
					tbi2.Image = img;
					Assert.Null (tbi2.Title, "Title-2b");
					Assert.AreSame (tbi2.Image, img, "Image-2b");
				}
				using (UITabBarItem tbi3 = new UITabBarItem (null, null, 0)) {
					Assert.Null (tbi3.Title, "Title-3a");
					Assert.Null (tbi3.Image, "Image-3a");
					tbi3.Title = "title";
					tbi3.Image = img;
					Assert.That (tbi3.Title, Is.EqualTo ("title"), "Title-3b");
					Assert.AreSame (tbi3.Image, img, "Image-3b");
				}
			}
		}

		[Test]
		public void Ctor_3b_Null ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("Requires iOS7 or later");
				
			using (UIImage img = new UIImage ()) {
				using (UITabBarItem tbi1 = new UITabBarItem (null, null, null)) {
					Assert.Null (tbi1.Title, "Title-1a");
					Assert.Null (tbi1.Image, "Image-1a");
					Assert.Null (tbi1.SelectedImage, "SelectedImage-1a");
				}
				using (UITabBarItem tbi2 = new UITabBarItem ("title", img, null)) {
					Assert.That (tbi2.Title, Is.EqualTo ("title"), "Title-2a");
					Assert.AreSame (tbi2.Image, img, "Image-2a");
					// if not supplied Image is reused
					Assert.AreSame (tbi2.SelectedImage, img, "SelectedImage-2a");
				}
				using (UITabBarItem tbi3 = new UITabBarItem (null, null, img)) {
					Assert.Null (tbi3.Title, "Title-3a");
					Assert.Null (tbi3.Image, "Image-3a");
					// looks like a select-only image is not something allowed on 7.1
					if (UIDevice.CurrentDevice.CheckSystemVersion (7,1))
						Assert.Null (tbi3.SelectedImage, "SelectedImage-3a");
					else
						Assert.AreSame (tbi3.SelectedImage, img, "SelectedImage-3a");
				}
			}
		}

		[Test]
		public void SelectedImage_7a ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("Requires iOS7 or later");

			using (UIImage i1 = new UIImage ())
			using (UITabBarItem tbi = new UITabBarItem ("title", i1, null)) {
				Assert.AreSame (i1, tbi.Image, "Image");
				Assert.AreSame (i1, tbi.SelectedImage, "SelectedImage");
#if !__TVOS__
				Assert.Null (tbi.FinishedSelectedImage, "FinishedSelectedImage");
				Assert.Null (tbi.FinishedUnselectedImage, "FinishedSelectedImage");
#endif
				// null does a reset, in this case i1 can be reused
				tbi.SelectedImage = null;
				Assert.AreSame (i1, tbi.SelectedImage, "SelectedImage2");
			}
		}

		[Test]
		public void SelectedImage_7b ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7,0))
				Assert.Inconclusive ("Requires iOS7 or later");

			using (UIImage i1 = new UIImage ())
			using (UIImage i2 = new UIImage ())
			using (UITabBarItem tbi = new UITabBarItem ("title", i1, i2)) {
				Assert.AreSame (i1, tbi.Image, "Image");
				Assert.AreSame (i2, tbi.SelectedImage, "SelectedImage");
#if !__TVOS__
				Assert.Null (tbi.FinishedSelectedImage, "FinishedSelectedImage");
				Assert.Null (tbi.FinishedUnselectedImage, "FinishedSelectedImage");
#endif
				tbi.SelectedImage = null;
				// null does a reset, in this case i2 is removed and i1 gets used
				Assert.AreSame (i1, tbi.SelectedImage, "SelectedImage2");
			}
		}
	}
}

#endif // !__WATCHOS__