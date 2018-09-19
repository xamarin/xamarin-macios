//
// Unit tests for UIStackViewTest
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
	public class A_UIStackViewTest {

		[Test]
		public void InitWithFrameTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 9, 0, throwIfOtherPlatform: false);

			UIStackView stack = new UIStackView (new RectangleF (0, 0, 10, 10));
			Assert.NotNull (stack, "UIStackView ctor(CGRect)");

			stack.AddArrangedSubview (new UIImageView ());
			stack.AddArrangedSubview (new UIView ());

			Assert.That (stack.Subviews.Length, Is.EqualTo (2), "UIStackView instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__