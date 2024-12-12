//
// Unit tests for MPSNDArrayDescriptor
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
	public class MPSNDArrayDescriptorTest {
		[Test]
		public void PermuteWithDimensionOrderTest ()
		{
			var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Inconclusive ($"Metal does not exist on this device.");

			using var desc = MPSNDArrayDescriptor.Create (MPSDataType.Int32, new nuint [] { 10 });
			desc.PermuteWithDimensionOrder (new nuint [] { 0 });
			Assert.AreEqual (1, (int) desc.NumberOfDimensions, "NumberOfDimensions");
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERS
