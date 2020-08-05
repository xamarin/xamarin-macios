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
using CoreGraphics;
using Foundation;
using ModelIO;
using ObjCRuntime;
using SceneKit;
#if !MONOMAC
using UIKit;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLObjectTest {
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

		[Test]
		public void ProtocolTest ()
		{
			using (var obj = new MDLObject ()) {
				var p = new Protocol (typeof (IMDLComponent));
				obj.SetComponent (new MDLTransform (), p);
				Assert.NotNull (obj.IsComponentConforming (p));
			}
		}
	}
}

#endif // !__WATCHOS__
