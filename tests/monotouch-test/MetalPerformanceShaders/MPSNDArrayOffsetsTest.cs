//
// Unit tests for MPSNDArrayOffsets
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
	public class MPSNDArrayOffsetsTest {
		[Test]
		public void Dimensions ()
		{
			var value = default (MPSNDArrayOffsets);
			CollectionAssert.AreEqual (new nint [16], value.Dimensions, "A");

			var array = new nint [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			value.Dimensions = array;
			CollectionAssert.AreEqual (array, value.Dimensions, "B");

			Assert.Throws<ArgumentNullException> (() => value.Dimensions = null, "C");
			Assert.Throws<ArgumentOutOfRangeException> (() => value.Dimensions = new nint [15], "D");
		}
	}
}
#endif // HAS_METALPERFORMANCESHADERS
