//
// Unit tests for UINavigationController
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	class NavigationControllerPoker : UINavigationController {
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NavigationControllerTest {
		[Test]
		public void ViewControllers_EmptyNull ()
		{
			using (UIViewController vc = new UIViewController ())
			using (var nc = new NavigationControllerPoker ()) {
				Assert.That (nc.ViewControllers, Is.Empty, "Empty");
				nc.SetViewControllers (new UIViewController [] { vc }, false);
				Assert.That (nc.ViewControllers, Is.Not.Empty, "!Empty");
				nc.SetViewControllers (null, false);
				Assert.That (nc.ViewControllers, Is.Empty, "null->Empty/SetMethod");
				nc.ViewControllers = null;
				Assert.That (nc.ViewControllers, Is.Empty, "null->Empty/Property");
			}
		}
	}
}

#endif // !__WATCHOS__
