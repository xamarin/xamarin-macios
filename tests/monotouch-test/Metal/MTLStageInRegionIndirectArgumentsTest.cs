#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Metal;
#else
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLStageInRegionIndirectArgumentsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void SizeOfMTLStageInRegionIndirectArgumentsTest ()
		{
			unsafe {
				Assert.AreEqual (sizeof (MTLStageInRegionIndirectArguments), 24); // 24 is the size of the native struct
			}
		}
	}
}

#endif // !__WATCHOS__
