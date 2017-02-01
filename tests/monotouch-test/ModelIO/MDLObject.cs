//
// MDLObject Unit Tests
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
using ModelIO;
using ObjCRuntime;
using SceneKit;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
using MonoTouch.SceneKit;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLObjectTest {
		[TestFixtureSetUp]
		public void Setup ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
				Assert.Ignore ("Requires iOS9+");

			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				using (var obj = new MDLObject ()) {
					object dummy;
					obj.SetComponent (new MDLTransform (), new Protocol ("MDLComponent"));
					obj.IsComponentConforming (new Protocol ("MDLComponent"));
					dummy = obj.Parent;
					dummy = obj.Transform;
					obj.AddChild (new MDLObject ());
					obj.GetBoundingBox (0);
				}
			}
		}

		[Test]
		public void GetBoundingBox ()
		{
			using (var obj = MDLObject.FromNode (SCNNode.FromGeometry (SCNBox.Create (1, 1, 1, 0)))) {
				var bb = obj.GetBoundingBox (0);
			}
		}
	}
}

#endif // !__WATCHOS__
