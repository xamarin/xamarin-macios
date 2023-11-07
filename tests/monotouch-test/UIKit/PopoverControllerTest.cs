// Copyright 2011 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PopoverControllerTest {
		[Test]
		public void Defaults ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				return;

			bool ios8 = TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

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
		public void PresentFromBarButtonItem_BadButton ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				Assert.Inconclusive ("Requires iPad");

			TestRuntime.AssertNotDevice ("ObjectiveC exception crash on devices - bug #3980");

			using (var vc = new UIViewController ())
			using (var bbi = new UIBarButtonItem (UIBarButtonSystemItem.Action))
			using (var pc = new UIPopoverController (vc)) {
#if __MACCATALYST__
				pc.PresentFromBarButtonItem (bbi, UIPopoverArrowDirection.Down, true);
#else
				// UIBarButtonItem is itself 'ok' but it's not assigned to a view
#if NET
				Assert.Throws<ObjCException> (() => pc.PresentFromBarButtonItem (bbi, UIPopoverArrowDirection.Down, true));
#else
				Assert.Throws<MonoTouchException> (() => pc.PresentFromBarButtonItem (bbi, UIPopoverArrowDirection.Down, true));
#endif
#endif
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
				pc.PresentFromRect (new CGRect (10, 10, 100, 100), view, UIPopoverArrowDirection.Down, true);
				pc.Dismiss (true);
				// works (as long as we dismiss the popover before disposing)
			}
		}

		[Test]
		public void PresentFromRect_BadView ()
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad)
				Assert.Inconclusive ("Requires iPad");

			TestRuntime.AssertNotDevice ("ObjectiveC exception crash on devices - bug #3980");

			using (var vc = new UIViewController ())
			using (var bbi = new UIBarButtonItem (UIBarButtonSystemItem.Action))
			using (var pc = new UIPopoverController (vc)) {
				// 'vc' has never been shown
#if NET
				Assert.Throws<ObjCException> (() => pc.PresentFromRect (new CGRect (10, 10, 100, 100), vc.View, UIPopoverArrowDirection.Down, true));
#else
				Assert.Throws<MonoTouchException> (() => pc.PresentFromRect (new CGRect (10, 10, 100, 100), vc.View, UIPopoverArrowDirection.Down, true));
#endif
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
