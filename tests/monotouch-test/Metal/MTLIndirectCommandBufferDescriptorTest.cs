#if !__WATCHOS__

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLIndirectCommandBufferDescriptorTest {
		MTLIndirectCommandBufferDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			descriptor = new MTLIndirectCommandBufferDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetSetCommandTypesTest ()
		{
			descriptor.CommandTypes = MTLIndirectCommandType.Draw;
			Assert.AreEqual (MTLIndirectCommandType.Draw, descriptor.CommandTypes);
		}

#if MONOMAC
		[Test]
		public void GetSetInheritPipelineStateTest ()
		{
			descriptor.InheritPipelineState = true;
			Assert.AreEqual (true, descriptor.InheritPipelineState);
		}
#endif

		[Test]
		public void GetSetInheritBuffersTest ()
		{
			descriptor.InheritBuffers = true;
			Assert.AreEqual (true, descriptor.InheritBuffers);
		}

		[Test]
		public void GetSetMaxVertexBufferBindCountTest ()
		{
			descriptor.MaxVertexBufferBindCount = 1;
			Assert.AreEqual ((nuint) 1, descriptor.MaxVertexBufferBindCount);
		}

		[Test]
		public void GetSetMaxFragmentBufferBindCountTest ()
		{
			descriptor.MaxFragmentBufferBindCount = 1;
			Assert.AreEqual ((nuint) 1, descriptor.MaxFragmentBufferBindCount);
		}
	}
}

#endif // !__WATCHOS__
