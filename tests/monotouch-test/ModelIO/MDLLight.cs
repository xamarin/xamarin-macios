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
#if XAMCORE_2_0
using CoreGraphics;
using Foundation;
using UIKit;
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#if !__TVOS__
using MonoTouch.MultipeerConnectivity;
#endif
using MonoTouch.UIKit;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLLightTest {
		[TestFixtureSetUp]
		public void Setup ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
				Assert.Ignore ("Requires iOS9+");
			
			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
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
				if (Runtime.Arch == Arch.SIMULATOR && Environment.OSVersion.Version.Major < 15) {
					Assert.IsNull (color, "color 1");
				} else {
					Assert.IsNotNull (color, "color 1");
				}
			}

			using (var obj = new MDLLight ()) {
				var color = obj.GetIrradiance (new Vector3 (1, 2, 3), CGColorSpace.CreateGenericRgb ());
				if (Runtime.Arch == Arch.SIMULATOR && Environment.OSVersion.Version.Major < 15) {
					Assert.IsNull (color, "color 2");
				} else {
					Assert.IsNotNull (color, "color 2");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
