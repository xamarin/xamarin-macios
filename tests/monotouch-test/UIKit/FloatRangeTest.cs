// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013, 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
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
	public class FloatRangeTest {

		[Test]
		public void ManagedVersusNative ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9,0))
				Assert.Ignore ("Requires iOS 9+");
			var uikit = Dlfcn.dlopen (Constants.UIKitLibrary, 0);
			try {
				var zero = Dlfcn.dlsym (uikit, "UIFloatRangeZero");
				var Zero = (UIFloatRange)Marshal.PtrToStructure (zero, typeof (UIFloatRange));
				Assert.True (UIFloatRange.Zero.Equals (Zero), "Zero");

				var infinite = Dlfcn.dlsym (uikit, "UIFloatRangeInfinite");
				var Infinite = (UIFloatRange)Marshal.PtrToStructure (infinite, typeof (UIFloatRange));
				Assert.True (Infinite.IsInfinite, "IsInfinite");
				Assert.False (UIFloatRange.Infinite.Equals (Infinite), "Infinite");
			} finally {
				Dlfcn.dlclose (uikit);
			}
		}

		[Test]
		public void IsInfinite ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9,0))
				Assert.Ignore ("Requires iOS 9+");
			Assert.True (UIFloatRange.Infinite.IsInfinite, "Infinite");
			Assert.False (UIFloatRange.Zero.IsInfinite, "Zero");
		}

		[Test]
		public void Equals ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9,0))
				Assert.Ignore ("Requires iOS 9+");
			Assert.True (UIFloatRange.Zero.Equals (UIFloatRange.Zero), "Zero-Zero");
			var one = new UIFloatRange (1f, 1f);
			Assert.False (one.Equals (UIFloatRange.Zero), "one-Zero");
			Assert.False (UIFloatRange.Zero.Equals ((object) one), "Zero-one");
			Assert.True (one.Equals (one), "one-one");

			Assert.False (UIFloatRange.Infinite.Equals (UIFloatRange.Infinite), "Infinite-Infinite");
			Assert.False (UIFloatRange.Infinite.Equals (UIFloatRange.Zero), "Infinite-Zero");
			Assert.False (UIFloatRange.Zero.Equals (UIFloatRange.Infinite), "Zero-Infinite");
		}
	}
}

#endif // !__WATCHOS__
