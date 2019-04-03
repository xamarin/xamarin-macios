//
// Unit tests for MPSStateBatch
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2019 Microsoft Corporation.
//

#if !__WATCHOS__

using System;

using Foundation;
using Metal;
using MetalPerformanceShaders;

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	public class MPSStateBatchTests {

		IMTLDevice device;
		NSArray<MPSState> cache;

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");

			cache = NSArray<MPSState>.FromNSObjects (
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 220, 220, false)),
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 221, 221, false)),
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 222, 222, false)),
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 223, 223, false)),
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 224, 224, false)),
				new MPSState (device, MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA32Float, 225, 225, false))
			);
		}

		[Test]
		public void IncrementReadCountTest () => Assert.DoesNotThrow (() => MPSStateBatch.IncrementReadCount (cache, 5), "IncrementReadCount");

		[Test]
		public void MPSImageBatchResourceSizeTest ()
		{
			var size = MPSStateBatch.GetResourceSize (cache);
			Assert.That (size, Is.GreaterThan (0), "idx");
		}
	}
}
#endif // !__WATCHOS__
