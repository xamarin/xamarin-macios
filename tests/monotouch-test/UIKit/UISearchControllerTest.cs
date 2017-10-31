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
	public class UISearchControllerTest {
		
		[Test]
		public void InitWithNibNameTest ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UISearchController is new in 8.0");

			UISearchController ctrl = new UISearchController (null, null);
			Assert.NotNull (ctrl, "UISearchController ctor(String, NSBundle)");

			ctrl.Delegate = new UISearchControllerDelegate ();
			Assert.NotNull (ctrl.Delegate, "UISearchController instance is not usable ");
		}
	}
}

#endif // !__WATCHOS__
