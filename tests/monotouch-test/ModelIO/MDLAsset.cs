//
// MDLAsset Unit Tests
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2017 Microsoft.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreGraphics;
using Foundation;
using UIKit;
using ModelIO;
using ObjCRuntime;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
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
	public class MDLAssetTest {
		[TestFixtureSetUp]
		public void Setup ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
				Assert.Ignore ("Requires iOS9+");

			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				using (var obj = new MDLAsset ()) {
					object dummy;
					obj.SetComponent (new MDLTransform (), new Protocol ("MDLComponent"));
					obj.IsComponentConforming (new Protocol ("MDLComponent"));
					obj.GetBoundingBox (0);
					dummy = obj.StartTime;
					dummy = obj.EndTime;
					dummy = obj.Url;
					dummy = obj.BufferAllocator;
					dummy = obj.VertexDescriptor;
					dummy = obj.Count;
					dummy = obj.Masters;
				}
			}
		}

		[Test]
		public void IndexerTest ()
		{
			using (var obj = new MDLAsset ()) {
				var key = new Protocol ("MDLComponent");
				var container = new MDLObjectContainer ();
				// Use the indexer's setter
				obj [key] = container;
				// Assert with the indexer's getter
				Assert.AreEqual (container, obj [key], "#1");
			}
		}
	}
}

#endif // !__WATCHOS__
