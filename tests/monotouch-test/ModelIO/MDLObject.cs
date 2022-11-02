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
#if NET
				Assert.NotNull (obj.GetComponent (p));
#else
				Assert.NotNull (obj.IsComponentConforming (p));
#endif
			}
		}
	}
}

#endif // !__WATCHOS__
