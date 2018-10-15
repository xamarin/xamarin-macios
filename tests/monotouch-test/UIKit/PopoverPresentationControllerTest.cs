// Copyright 2014 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
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
	public class PopoverPresentationControllerTest {

		class MyPopoverBackgroundView : UIPopoverBackgroundView {
		}

		[Test]
		public void PopoverBackgroundViewType ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 8, 0, throwIfOtherPlatform: false);

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
