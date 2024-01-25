#if !__WATCHOS__

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLAttributeDescriptorTest {
		MTLAttributeDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			descriptor = new MTLAttributeDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetSetFormatTest ()
		{
			descriptor.Format = MTLAttributeFormat.Invalid;
			Assert.AreEqual (MTLAttributeFormat.Invalid, descriptor.Format);
		}

		[Test]
		public void GetSetOffsetTest ()
		{
			uint offset = 0; // must be 0, other value will crash the test.
			descriptor.Offset = offset;
			Assert.AreEqual ((nuint) offset, descriptor.Offset);
		}

		[Test]
		public void GetSetBufferIndexTest ()
		{
			uint index = 0; // must be 0, other value will crash the test.
			descriptor.BufferIndex = index;
			Assert.AreEqual ((nuint) index, descriptor.BufferIndex);
		}
	}
}

#endif // !__WATCHOS__
