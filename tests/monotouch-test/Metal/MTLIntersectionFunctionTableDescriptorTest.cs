#if !__WATCHOS__ && !__TVOS__
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
#if __MACOS__
			TestRuntime.AssertXcodeVersion (12, 2);
#else
			TestRuntime.AssertXcodeVersion (12, 0);
#endif
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
