// Copyright 2015 Xamarin Inc. All rights reserved.

#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Metal;
using MetalPerformanceShaders;
using UIKit;
#else
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {

	[TestFixture]
	public class KernelTest {

		[Test]
		public void RectNoClip ()
		{
			TestRuntime.AssertXcodeVersion (7,0);

			var d = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (d == null)
				Assert.Inconclusive ("Metal is not supported");
			
			var r = MPSKernel.RectNoClip;
			var o = r.Origin;
			Assert.That (o.X, Is.EqualTo (0), "X");
			Assert.That (o.Y, Is.EqualTo (0), "Y");
			Assert.That (o.Z, Is.EqualTo (0), "Z");
			var s = r.Size;
			Assert.That (s.Depth, Is.EqualTo (-1), "Depth");
			Assert.That (s.Height, Is.EqualTo (-1), "Height");
			Assert.That (s.Width, Is.EqualTo (-1), "Width");
		}
	}
}

#endif // !__WATCHOS__
