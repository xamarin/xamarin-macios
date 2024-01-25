#if !__WATCHOS__ && !__TVOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLVisibleFunctionTableDescriptorTest {

		MTLVisibleFunctionTableDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			descriptor = MTLVisibleFunctionTableDescriptor.Create ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void FunctionCountTest ()
		{
			nuint newFunctionCount = 10;
			nuint objFunctionCount = 0;
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.FunctionCount = newFunctionCount;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objFunctionCount = descriptor.FunctionCount;
			}, "Getter");
			Assert.AreEqual (newFunctionCount, objFunctionCount, "Value");
		}
	}
}
#endif
