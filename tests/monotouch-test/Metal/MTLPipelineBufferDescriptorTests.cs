#if !__WATCHOS__

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLPipelineBufferDescriptorTest {
		MTLPipelineBufferDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			descriptor = new MTLPipelineBufferDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor != null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetSetMutabilityTest ()
		{
			descriptor.Mutability = MTLMutability.Immutable;
			Assert.AreEqual (MTLMutability.Immutable, descriptor.Mutability);
		}
	}
}

#endif // !__WATCHOS__
