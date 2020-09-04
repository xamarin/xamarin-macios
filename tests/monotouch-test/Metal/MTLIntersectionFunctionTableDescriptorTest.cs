#if !__WATCHOS__
#nullable enable

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLIntersectionFunctionTableDescriptorTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);
		}

		[Test]
		public void FunctionCountTest ()
		{
			using var descriptor = MTLIntersectionFunctionTableDescriptor.Create ();

			nuint newCount = 10;
			nuint objCount = 0;

			Assert.DoesNotThrow (() => {
				descriptor.FunctionCount = newCount;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objCount = descriptor.FunctionCount;
			}, "Getter");
			Assert.AreEqual (newCount, objCount, "Count");
		}
	}
}

#endif
