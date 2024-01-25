#if !__WATCHOS__

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLStageInputOutputDescriptorTest {
		MTLStageInputOutputDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			descriptor = MTLStageInputOutputDescriptor.Create ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetLayoutsTest ()
		{
			Assert.NotNull (descriptor.Layouts); // default value
		}

		[Test]
		public void GetAttributesTest ()
		{
			Assert.NotNull (descriptor.Attributes); // default value
		}

		[Test]
		public void GetSetIndexType ()
		{
			descriptor.IndexType = MTLIndexType.UInt32;
			Assert.AreEqual (MTLIndexType.UInt32, descriptor.IndexType);
		}

		[Test]
		public void GetSetIndexBufferTest ()
		{
			uint index = 5;
			descriptor.IndexBufferIndex = 5;
			Assert.AreEqual (descriptor.IndexBufferIndex, (nuint) index);
		}
	}
}

#endif // !__WATCHOS__
