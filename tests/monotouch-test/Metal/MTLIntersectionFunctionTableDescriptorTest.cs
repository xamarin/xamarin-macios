#if !__WATCHOS__ && !__TVOS__
#nullable enable

using System;

using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLIntersectionFunctionTableDescriptorTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
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
