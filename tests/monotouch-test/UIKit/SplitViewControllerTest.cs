// Copyright 2012 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SplitViewControllerTest {

		[Test]
		public void Defaults ()
		{
			// UISplitViewController feature is only available on iPads
			// and we (presently) crash on devices when an objective-c exception is thrown
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				return;

			using (UIViewController v1 = new UIViewController ())
			using (UIViewController v2 = new UIViewController ())
			using (UISplitViewController svc = new UISplitViewController ()) {
				Assert.That (svc.ViewControllers, Is.Empty, "ViewControllers"); // not null, empty

				svc.ViewControllers = new UIViewController [] { v1, v2 };

				Assert.AreSame (v1, svc.ViewControllers [0], "vc0");
				Assert.AreSame (v2, svc.ViewControllers [1], "vc1");

				Assert.AreSame (v1, svc.ChildViewControllers [0], "cvc0");
				Assert.AreSame (v2, svc.ChildViewControllers [1], "cvc1");
			}
		}

		[Test]
		public void PresentsWithGesture ()
		{
			// UISplitViewController feature is only available on iPads
			// and we (presently) crash on devices when an objective-c exception is thrown
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				return;

			TestRuntime.IgnoreOnTVOS ();

			using (UISplitViewController svc = new UISplitViewController ()) {
				Assert.True (svc.PresentsWithGesture, "PresentsWithGesture/default");
			}
		}
	}
}

#endif // !__WATCHOS__
