// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013, 2016 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FloatRangeTest {

		[Ignore ("https://github.com/xamarin/maccore/issues/1885")]
		[Test]
		public void ManagedVersusNative ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
			var uikit = Dlfcn.dlopen (Constants.UIKitLibrary, 0);
			try {
				var zero = Dlfcn.dlsym (uikit, "UIFloatRangeZero");
				var Zero = Marshal.PtrToStructure<UIFloatRange> (zero);
				Assert.True (UIFloatRange.Zero.Equals (Zero), "Zero");

				var infinite = Dlfcn.dlsym (uikit, "UIFloatRangeInfinite");
				var Infinite = Marshal.PtrToStructure<UIFloatRange> (infinite);
				Assert.True (Infinite.IsInfinite, "IsInfinite");
				Assert.False (UIFloatRange.Infinite.Equals (Infinite), "Infinite");
			} finally {
				Dlfcn.dlclose (uikit);
			}
		}

		[Test]
		public void IsInfinite ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
			Assert.True (UIFloatRange.Infinite.IsInfinite, "Infinite");
			Assert.False (UIFloatRange.Zero.IsInfinite, "Zero");
		}

		[Ignore ("https://github.com/xamarin/maccore/issues/1885")]
		[Test]
		public void Equals ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
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
