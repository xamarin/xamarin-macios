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
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class A_UIStackViewTest {

		[Test]
		public void InitWithFrameTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 9, 0, throwIfOtherPlatform: false);

			UIStackView stack = new UIStackView (new CGRect (0, 0, 10, 10));
			Assert.NotNull (stack, "UIStackView ctor(CGRect)");

			stack.AddArrangedSubview (new UIImageView ());
			stack.AddArrangedSubview (new UIView ());

			Assert.That (stack.Subviews.Length, Is.EqualTo (2), "UIStackView instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__
