//
// Unit tests for MPSImageBatch
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
	public class MPSImageBatchTests {

		IMTLDevice device;
		NSArray<MPSImage> cache;

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");

			cache = NSArray<MPSImage>.FromNSObjects (
				new MPSImage (device, MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float32, 224, 224, 3)),
				new MPSImage (device, MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float32, 224, 224, 3)),
				new MPSImage (device, MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float32, 224, 224, 3)),
				new MPSImage (device, MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float32, 224, 224, 3)),
				new MPSImage (device, MPSImageDescriptor.GetImageDescriptor (MPSImageFeatureChannelFormat.Float32, 224, 224, 3))
			);
		}

		[Test]
		public void IncrementReadCountTest () => Assert.DoesNotThrow (() => MPSImageBatch.IncrementReadCount (cache, 5), "IncrementReadCount");

		[Test]
		public void MPSImageBatchResourceSizeTest ()
		{
			var size = MPSImageBatch.GetResourceSize (cache);
			Assert.That (size, Is.GreaterThan (0), "idx");
		}

		// TODO: Enable once'MPSImageBatch.Iterate' is enabled, rdar://47282304.
		//[Ignore ("'DMPSImageBatchIterator' is not in the native library")]
		//[Test]
		//public void IteratorTest ()
		//{
		//	bool hit = false;
		//	var idx = MPSImageBatch.Iterate (cache, (image, index) => {
		//		hit = true;
		//		return (nint) index;
		//	});

		//	Assert.That (idx, Is.EqualTo (5), "idx");
		//	Assert.IsTrue (hit, "hit");
		//}
	}
}
#endif // !__WATCHOS__
