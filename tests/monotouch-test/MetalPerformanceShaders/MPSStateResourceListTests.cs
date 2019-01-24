//
// Unit tests for MPSStateResourceList
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2019 Microsoft Corporation.
//

#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Foundation;
using Metal;
using MetalPerformanceShaders;
#else
using MonoTouch.Foundation;
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	public class MPSStateResourceListTests {

		IMTLDevice device;

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");
		}

		[Test]
		public void CreateTest ()
		{
			var resList = MPSStateResourceList.Create ();
			Assert.NotNull (resList, "Create");
		}

		[Test]
		public void MTLTextureDescriptorCreateTest ()
		{
			var arr = new MTLTextureDescriptor [10];
			for (nuint i = 0; i < 10; i++)
				arr [i] = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.Depth32Float, 50 + i, 50 + i, false);

			var resList = MPSStateResourceList.Create (arr[0], arr [1], arr [2], arr [3], arr [4], arr [5], arr [6], arr [7], arr [8], arr [9]);
			Assert.NotNull (resList, "resList");

			var state = new MPSState (device, resList);
			Assert.That (state.ResourceCount, Is.EqualTo (10), "ResourceCount");
		}

		[Test]
		public void SizesCreateTest ()
		{
			var resList = MPSStateResourceList.Create (1, 2, 3, 4, 5, 241);
			Assert.NotNull (resList, "resList");

			var state = new MPSState (device, resList);
			Assert.That (state.ResourceCount, Is.EqualTo (6), "ResourceCount");

			Assert.That (state.GetBufferSize (5), Is.EqualTo (241), "resList[5] = 241");
			Assert.That (state.GetBufferSize (2), Is.EqualTo (3), "resList[2] = 3");
		}
	}
}
#endif // !__WATCHOS__
