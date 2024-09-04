//
// Unit tests for MPSNDArray
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
	public class MPSNDArrayTest {
		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Inconclusive ($"Metal does not exist on this device.");

			using var array = new MPSNDArray (device, 3.14);
			using var newArray = array.Create (1, new nuint [] { 1 }, new nuint [] { 1 });
			Assert.IsNull (newArray, "Create"); // creation probably fails because I don't know which arguments to pass.

			Assert.Throws<ArgumentOutOfRangeException> (() => array.Create (2, new nuint [1], new nuint [2]), "AOORE 1");
			Assert.Throws<ArgumentOutOfRangeException> (() => array.Create (2, new nuint [2], new nuint [1]), "AOORE 2");
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERS
