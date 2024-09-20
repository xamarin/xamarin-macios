//
// Unit tests for MPSNDArrayIdentity
//

#if HAS_METALPERFORMANCESHADERS
using System;

using Foundation;
using Metal;
using MetalPerformanceShaders;

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MPSNDArrayIdentityTest {
		[Test]
		public void ReshapeA ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Inconclusive ($"Metal does not exist on this device.");

			using var identity = new MPSNDArrayIdentity (device);
			using var sourceArray = new MPSNDArray (device, 3.14f);
			using var newArray = identity.Reshape (null, sourceArray, new nuint [] { 1 }, null);
			Assert.AreEqual (1, (int) newArray.NumberOfDimensions, "NumberOfDimensions");
			Assert.AreEqual (1, (int) newArray.GetLength (0), "Length #0");
		}

		[Test]
		public void ReshapeB ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Inconclusive ($"Metal does not exist on this device.");

			using var identity = new MPSNDArrayIdentity (device);
			using var sourceArray = new MPSNDArray (device, 3.14f);
			using var newArray = identity.Reshape (null, null, sourceArray, new nuint [] { 1 }, null);
			Assert.AreEqual (1, (int) newArray.NumberOfDimensions, "NumberOfDimensions");
			Assert.AreEqual (1, (int) newArray.GetLength (0), "Length #0");
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERS
