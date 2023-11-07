//
// Unit tests for UIViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Reflection;
using Foundation;
using UIKit;
using ObjCRuntime;
#if HAS_IAD
using iAd;
#endif
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ViewControllerTest {

#if !__TVOS__
		[Test]
		public void Bug3489 ()
		{
			using (UIViewController a = new UIViewController ())
			using (UIViewController b = new UIViewController ())
			using (UIViewController c = new UIViewController ()) {
				a.PresentModalViewController (b, true);
				b.PresentModalViewController (c, true);

				b.DismissModalViewController (true);
				a.DismissModalViewController (true); //error
			}
		}
#endif

#if !__TVOS__
		[Test]
		public void Bug3189 ()
		{
			using (UIViewController a = new UIViewController ())
			using (UIViewController b = new UIViewController ())
			using (UIViewController c = new UIViewController ())
			using (UIViewController wb = new UINavigationController (b))
			using (UIViewController wc = new UINavigationController (c)) {
				a.PresentModalViewController (wb, true);
				b.PresentModalViewController (wc, true);

				c.DismissModalViewController (true); //error
			}
		}
#endif

		[Test]
		public void NonModal ()
		{
			using (UIViewController a = new UIViewController ())
			using (UIViewController b = new UIViewController ())
			using (UIViewController c = new UIViewController ())
			using (UIViewController wb = new UINavigationController (b))
			using (UIViewController wc = new UINavigationController (c)) {
				// interesting [PreSnippet] for the linker (wrt backing field elimitation)
				a.PresentViewController (wb, true, null);
				b.PresentViewController (wc, true, null);

				// interesting [PostSnippet] for the linker (wrt backing field elimitation)
				c.DismissViewController (true, null);
			}
		}

		[Test]
		public void NSAction_Null ()
		{
			using (var vc = new UIViewController ())
			using (var child = new UIViewController ()) {
				vc.PresentViewController (child, false, null);
				child.DismissViewController (false, null);
			}
		}

		[Test]
		public void Defaults ()
		{
			using (var vc = new UIViewController ()) {
				Assert.That (vc.ChildViewControllers, Is.Empty, "ChildViewControllers");
				Assert.False (vc.DefinesPresentationContext, "DefinesPresentationContext");
				Assert.False (vc.DisablesAutomaticKeyboardDismissal, "DisablesAutomaticKeyboardDismissal");
				Assert.False (vc.Editing, "Editing");
				Assert.False (vc.IsBeingDismissed, "IsBeingDismissed");
				Assert.False (vc.IsBeingPresented, "IsBeingPresented");
				Assert.False (vc.IsMovingFromParentViewController, "IsMovingFromParentViewController");
				Assert.False (vc.IsMovingToParentViewController, "IsMovingToParentViewController");
				Assert.False (vc.IsViewLoaded, "IsViewLoaded");
				Assert.False (vc.ModalInPopover, "ModalInPopover");
				Assert.Null (vc.NavigationController, "NavigationController");
				Assert.NotNull (vc.NibBundle, "NibBundle");
				Assert.Null (vc.NibName, "NibName");
				Assert.Null (vc.ParentViewController, "ParentViewController");
				Assert.Null (vc.PresentedViewController, "PresentedViewController");
				Assert.Null (vc.PresentingViewController, "PresentingViewController");
				Assert.False (vc.ProvidesPresentationContextTransitionStyle, "ProvidesPresentationContextTransitionStyle");
#if !__TVOS__
				Assert.True (vc.AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers, "AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers");
				Assert.False (vc.HidesBottomBarWhenPushed, "HidesBottomBarWhenPushed");
				Assert.Null (vc.ModalViewController, "ModalViewController");
				Assert.Null (vc.RotatingFooterView, "RotatingFooterView");
				Assert.Null (vc.RotatingHeaderView, "RotatingHeaderView");
#if !__MACCATALYST__
				Assert.Null (vc.SearchDisplayController, "SearchDisplayController");
#endif
				Assert.False (vc.WantsFullScreenLayout, "WantsFullScreenLayout");
#endif
				Assert.Null (vc.SplitViewController, "SplitViewController");
				Assert.Null (vc.Storyboard, "Storyboard");
				Assert.Null (vc.TabBarController, "TabBarController");
				Assert.NotNull (vc.TabBarItem, "TabBarItem");
				Assert.Null (vc.Title, "Title");
				Assert.Null (vc.ToolbarItems, "ToolbarItems");
				Assert.NotNull (vc.View, "View");
			}
		}

		[Test]
		public void Toolbars_Null ()
		{
			using (var undo = new UIBarButtonItem (UIBarButtonSystemItem.Undo))
			using (var redo = new UIBarButtonItem (UIBarButtonSystemItem.Redo))
			using (var vc = new UIViewController ()) {
				var buttons = new UIBarButtonItem [] { undo, redo };
				vc.ToolbarItems = buttons;
				Assert.That (vc.ToolbarItems.Length, Is.EqualTo (2), "1");
				vc.ToolbarItems = null;
				Assert.Null (vc.ToolbarItems, "2");
#if !__TVOS__
				vc.SetToolbarItems (buttons, true);
				Assert.That (vc.ToolbarItems.Length, Is.EqualTo (2), "3");
				vc.SetToolbarItems (null, false);
				Assert.Null (vc.ToolbarItems, "4");
#endif
			}
		}

		[Test]
		public void View_Null ()
		{
			using (var vc = new UIViewController ()) {
				// even if the default is null <quote>The default value of this property is nil.</quote>
				// we'll never see it as such as it will be loaded (loadView)
				Assert.NotNull (vc.View, "View-a");
				// OTOH we can set it to null ourself
				// or the controller can do it if iOS runs out of memory
				vc.View = null;
				// but again, accessing it will load the view
				Assert.NotNull (vc.View, "View-b");
			}
		}

		[Test]
		public void AppearanceTransition ()
		{
			// this was retroactively documented as available in 5.0 (officially added in 6.0)
			// but respondToSelector return false
			using (var vc = new UIViewController ()) {
				vc.BeginAppearanceTransition (true, true);
				vc.EndAppearanceTransition ();
			}
		}

#if HAS_IAD
		[Test]
		public void InterstitialAds_New ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);
			
			UIViewController.PrepareForInterstitialAds ();
		}
#endif // HAS_IAD
	}
}

#endif // !__WATCHOS__
