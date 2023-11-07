// Copyright 2015 Xamarin Inc. All rights reserved.

#if !__WATCHOS__

using System;
using Foundation;
using Metal;
using ObjCRuntime;
using SceneKit;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GeometrySourceTest {

		[Test]
		public void FromMetalBuffer ()
		{
			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0 or macOS 10.11");

			TestRuntime.AssertDevice ();

			var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Inconclusive ("Device does not support Metal");

			using (var buffer = device.CreateBuffer (1024, MTLResourceOptions.CpuCacheModeDefault)) {
				using (var source = SCNGeometrySource.FromMetalBuffer (buffer, MTLVertexFormat.Char2, SCNGeometrySourceSemantic.Vertex, 36, 0, 0)) {
					// the fact that it works means the lack of respondToSelector (in introspection tests) is no 
					// big deal and that the API really exists
					Assert.NotNull (source);
				}
			}
		}
	}
}

#endif // !__WATCHOS__
