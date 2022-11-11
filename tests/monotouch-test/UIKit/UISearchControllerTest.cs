//
// Unit tests for UISearchControllerTest
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class UISearchControllerTest {

		[Test]
		public void InitWithNibNameTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			UISearchController ctrl = new UISearchController (null, null);
			Assert.NotNull (ctrl, "UISearchController ctor(String, NSBundle)");

			ctrl.Delegate = new UISearchControllerDelegate ();
			Assert.NotNull (ctrl.Delegate, "UISearchController instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__
