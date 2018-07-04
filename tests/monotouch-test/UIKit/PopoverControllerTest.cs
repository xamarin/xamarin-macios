// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
using MonoTouchException=Foundation.MonoTouchException;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MonoTouchException=MonoTouch.Foundation.MonoTouchException;
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
	public class PopoverControllerTest {
		[Test]
		public void Defaults ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				return;

			bool ios8 = TestRuntime.CheckSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);
			
			using (var vc = new UIViewController ())
			using (var pc = new UIPopoverController (vc)) {
				Assert.That (pc.ContentViewController, Is.SameAs (vc), "ContentViewController");
				Assert.Null (pc.PassthroughViews, "PassthroughViews");
				Assert.That (pc.PopoverArrowDirection, Is.EqualTo (UIPopoverArrowDirection.Unknown), "PopoverArrowDirection");
				Assert.That (pc.PopoverContentSize.IsEmpty, Is.EqualTo (ios8), "PopoverContentSize");
				Assert.That (pc.PopoverLayoutMargins.ToString (), Is.EqualTo (ios8 ? "{0, 0, 0, 0}" : "{30, 10, 10, 10}"), "PopoverLayoutMargins");
				Assert.False (pc.PopoverVisible, "PopoverVisible");
				Assert.Null (pc.ShouldDismiss, "ShouldDismiss");
			}
		}

		[Test]
		[ExpectedException (typeof (MonoTouchException))]
		public void PresentFromBarButtonItem_BadButton ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				Assert.Inconclusive ("Requires iPad");
			
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("ObjectiveC exception crash on devices - bug #3980");
			
			using (var vc = new UIViewController ())
			using (var bbi = new UIBarButtonItem (UIBarButtonSystemItem.Action))
			using (var pc = new UIPopoverController (vc)) {
				// UIBarButtonItem is itself 'ok' but it's not assigned to a view
				pc.PresentFromBarButtonItem (bbi, UIPopoverArrowDirection.Down, true);
				// fails with:
				// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[UIPopoverController presentPopoverFromBarButtonItem:permittedArrowDirections:animated:]: Popovers cannot be presented from a view which does not have a window.
			}
		}

		[Test]
		public void PresentFromRect ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				return;
			
			using (var vc = new UIViewController ())
			using (var bbi = new UIBarButtonItem (UIBarButtonSystemItem.Action))
			using (var pc = new UIPopoverController (vc)) {
				var view = UIApplication.SharedApplication.KeyWindow;
				pc.PresentFromRect (new RectangleF (10, 10, 100, 100), view, UIPopoverArrowDirection.Down, true);
				pc.Dismiss (true);
				// works (as long as we dismiss the popover before disposing)
			}
		}

		[Test]
		[ExpectedException (typeof (MonoTouchException))]
		public void PresentFromRect_BadView ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				Assert.Inconclusive ("Requires iPad");

			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("ObjectiveC exception crash on devices - bug #3980");
			
			using (var vc = new UIViewController ())
			using (var bbi = new UIBarButtonItem (UIBarButtonSystemItem.Action))
			using (var pc = new UIPopoverController (vc)) {
				// 'vc' has never been shown
				pc.PresentFromRect (new RectangleF (10, 10, 100, 100), vc.View, UIPopoverArrowDirection.Down, true);
				// fails with:
				// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[UIPopoverController presentPopoverFromRect:inView:permittedArrowDirections:animated:]: Popovers cannot be presented from a view which does not have a window.
			}
		}
		
		// note: not complete (won't work) - but enough for testing the property
		// full sample at http://stackoverflow.com/a/9312939/220643
		class MyPopoverBackgroundView : UIPopoverBackgroundView {
		}
		
		[Test]
		public void PopoverBackgroundViewType ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				return;
			
			using (var vc = new UIViewController ())
			using (var pc = new UIPopoverController (vc)) {
				Assert.Null (pc.PopoverBackgroundViewType, "PopoverBackgroundViewType");
				Type my = typeof (MyPopoverBackgroundView);
				pc.PopoverBackgroundViewType = my;
				Assert.That (pc.PopoverBackgroundViewType, Is.SameAs (my), "MyPopoverBackgroundView");
			}
		}
	}
}

#endif // !__WATCHOS__
