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
#if XAMCORE_2_0
using Foundation;
using UIKit;
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
	public class UIAlertControllerTest {
		
		[Test]
		public void InitWithNibNameTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UIAlertControllerTest is new in 8.0");
			
			UIAlertController ctrl = new UIAlertController (null, null);
			Assert.NotNull (ctrl, "UIAlertController ctor(String, NSBundle)");

			ctrl.AddAction (new UIAlertAction ());
			ctrl.AddAction (new UIAlertAction ());
			Assert.That (ctrl.Actions.Length, Is.EqualTo (2), "UIAlertController instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__
