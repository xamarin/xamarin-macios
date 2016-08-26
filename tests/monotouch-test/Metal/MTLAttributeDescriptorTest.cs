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
	public class MTLAttributeDescriptorTest {
		MTLAttributeDescriptor descriptor = null;
		
		[SetUp]
		public void SetUp ()
		{
			descriptor = new MTLAttributeDescriptor  ();
		}
		
		[TearDown]
		public void TearDown ()
		{
			if (descriptor != null)
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
			Assert.AreEqual (offset, descriptor.Offset);
		}

		[Test]
		public void GetSetBufferIndexTest ()
		{
			uint index = 0; // must be 0, other value will crash the test.
			descriptor.BufferIndex = index;
			Assert.AreEqual (index, descriptor.BufferIndex);
		}
	}
}

#endif // !__WATCHOS__
