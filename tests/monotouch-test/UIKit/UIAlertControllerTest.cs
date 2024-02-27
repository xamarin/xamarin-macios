//
// Unit tests for UIAlertControllerTest
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
	public class UIAlertControllerTest {

		[Test]
		public void InitWithNibNameTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			UIAlertController ctrl = new UIAlertController (null, null);
			Assert.NotNull (ctrl, "UIAlertController ctor(String, NSBundle)");

			ctrl.AddAction (new UIAlertAction ());
			ctrl.AddAction (new UIAlertAction ());
			Assert.That (ctrl.Actions.Length, Is.EqualTo (2), "UIAlertController instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__
