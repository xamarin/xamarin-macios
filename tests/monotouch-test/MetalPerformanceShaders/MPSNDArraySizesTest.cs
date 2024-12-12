//
// Unit tests for MPSNDArraySizes
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
	public class MPSNDArraySizesTest {
		[Test]
		public void Dimensions ()
		{
			var value = default (MPSNDArraySizes);
			CollectionAssert.AreEqual (new nuint [16], value.Dimensions, "A");

			var array = new nuint [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			value.Dimensions = array;
			CollectionAssert.AreEqual (array, value.Dimensions, "B");

			Assert.Throws<ArgumentNullException> (() => value.Dimensions = null, "C");
			Assert.Throws<ArgumentOutOfRangeException> (() => value.Dimensions = new nuint [15], "D");
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERS
