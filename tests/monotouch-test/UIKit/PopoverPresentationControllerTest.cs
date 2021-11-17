// Copyright 2014 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PopoverPresentationControllerTest {

		class MyPopoverBackgroundView : UIPopoverBackgroundView {
		}

		[Test]
		public void PopoverBackgroundViewType ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 8, 0, throwIfOtherPlatform: false);

			using (var vc = new UIViewController ())
			using (var pc = new UIPopoverPresentationController (vc, null)) {
				Assert.Null (pc.PopoverBackgroundViewType, "PopoverBackgroundViewType");
				Type my = typeof (MyPopoverBackgroundView);
				pc.PopoverBackgroundViewType = my;
				Assert.That (pc.PopoverBackgroundViewType, Is.SameAs (my), "MyPopoverBackgroundView");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
