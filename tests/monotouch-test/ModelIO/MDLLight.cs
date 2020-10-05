//
// MDLLight Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using CoreGraphics;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLLightTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0+ or macOS 10.11+");

			if (
#if !MONOMAC
				Runtime.Arch == Arch.SIMULATOR && 
#endif
				IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				using (var obj = new MDLLight ()) {
					obj.GetIrradiance (Vector3.Zero);
					obj.GetIrradiance (Vector3.Zero, CGColorSpace.CreateGenericRgb ());
				}
			}
		}

		[Test]
		public void IrradianceAtPointTest ()
		{
			using (var obj = new MDLLight ()) {
				var color = obj.GetIrradiance (new Vector3 (1, 2, 3));
				Assert.IsNotNull (color, "color 1");
			}

			using (var obj = new MDLLight ()) {
				var color = obj.GetIrradiance (new Vector3 (1, 2, 3), CGColorSpace.CreateGenericRgb ());
				Assert.IsNotNull (color, "color 2");
			}
		}
	}
}

#endif // !__WATCHOS__
