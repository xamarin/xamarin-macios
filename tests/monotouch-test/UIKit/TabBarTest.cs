// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
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
	public class TabBarTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UITabBar tb = new UITabBar (frame)) {
				Assert.That (tb.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void SelectedItem ()
		{
			using (UITabBarItem item = new UITabBarItem ())
			using (UITabBar tb = new UITabBar ()) {
				Assert.Null (tb.SelectedItem, "1a");
				
				tb.SelectedItem = item;
				// setter did not work because 'item' is not in Items
				Assert.Null (tb.SelectedItem, "2a");
				Assert.Null (tb.Items, "2b");

				tb.SelectedItem = null;
				Assert.Null (tb.SelectedItem, "3a");
			}
		}

		[Test]
		public void Items ()
		{
			using (UITabBarItem item = new UITabBarItem ())
			using (UITabBar tb = new UITabBar ()) {
				Assert.Null (tb.Items, "1a");
				Assert.Null (tb.SelectedItem, "1b");
				
				tb.Items = new UITabBarItem[] { item };
				Assert.NotNull (tb.Items, "2a");
				tb.SelectedItem = item;
				Assert.NotNull (tb.SelectedItem, "2b");
				
				tb.Items = null;
				Assert.Null (tb.Items, "3a");
				// Interaction between Items and SelectedItems -> backing fields!
				Assert.Null (tb.SelectedItem, "3b");
			}
		}

#if !__TVOS__
		[Test]
		public void Customizing ()
		{
			using (UITabBarItem item = new UITabBarItem ())
			using (UITabBar tb = new UITabBar ()) {
				Assert.False (tb.IsCustomizing, "IsCustomizing-1");
				
				tb.BeginCustomizingItems (new UITabBarItem[] { item });
				Assert.True (tb.IsCustomizing, "IsCustomizing-2");
#if XAMCORE_2_0
				Assert.False (tb.EndCustomizing (false), "End-1");
#else
				Assert.False (tb.EndCustomizingAnimated (false), "End-1");
#endif

				tb.BeginCustomizingItems (null);
#if XAMCORE_2_0
				Assert.False (tb.EndCustomizing (false), "End-2");
#else
				Assert.False (tb.EndCustomizingAnimated (false), "End-2");
#endif
				
				Assert.False (tb.IsCustomizing, "IsCustomizing-3");
			}
		}
#endif

		[Test]
		public void BackgroundImage ()
		{
			using (UIImage i = new UIImage ())
			using (UITabBar tb = new UITabBar ()) {
				Assert.Null (tb.BackgroundImage, "1");
				
				tb.BackgroundImage = i;
				Assert.NotNull (tb.BackgroundImage, "2");
				
				tb.BackgroundImage = null;
				Assert.Null (tb.BackgroundImage, "3");
			}
		}

		[Test]
		public void SelectionIndicatorImage ()
		{
			using (UIImage i = new UIImage ())
			using (UITabBar tb = new UITabBar ()) {
				Assert.Null (tb.SelectionIndicatorImage, "1");
				
				tb.SelectionIndicatorImage = i;
				Assert.NotNull (tb.SelectionIndicatorImage, "2");
				
				tb.SelectionIndicatorImage = null;
				Assert.Null (tb.SelectionIndicatorImage, "3");
			}
		}

		[Test]
		public void TintColor ()
		{
			using (UITabBar tb = new UITabBar ()) {
				// TintColor is inherited in iOS7 so it won't be null by default
				if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false))
					Assert.NotNull (tb.TintColor, "1");
				else
					Assert.Null (tb.TintColor, "1");
				
				tb.TintColor = UIColor.White;
				Assert.That (tb.TintColor, Is.EqualTo (UIColor.White), "2");

				tb.TintColor = null;
				if (TestRuntime.IsTVOS)
					Assert.That (tb.TintColor, Is.EqualTo (UIColor.White), "3");
				else if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false))
					Assert.That (tb.TintColor, Is.Not.EqualTo (UIColor.White), "3");
				else
					Assert.Null (tb.TintColor, "3");
			}
		}

#if !__TVOS__
		[Test]
		public void SelectedImageTintColor ()
		{
			using (UITabBar tb = new UITabBar ()) {
				Assert.Null (tb.SelectedImageTintColor, "1");
				
				tb.SelectedImageTintColor = UIColor.Black;
				if (!TestRuntime.CheckSystemVersion (PlatformName.iOS, 7, 1)) {
					// before 7.1 the tintColor would have been accepted
					Assert.NotNull (tb.SelectedImageTintColor, "2");
			
					tb.SelectedImageTintColor = null;
				}
				Assert.Null (tb.SelectedImageTintColor, "3");
			}
		}
#endif
	}
}

#endif // !__WATCHOS__
