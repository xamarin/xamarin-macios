#if !__WATCHOS__

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLArgumentDescriptorTest {
		MTLArgumentDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			descriptor = new MTLArgumentDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetSetAccessTest ()
		{
			descriptor.Access = MTLArgumentAccess.ReadWrite;
			Assert.AreEqual (MTLArgumentAccess.ReadWrite, descriptor.Access);
		}

		[Test]
		public void GetSetArrayLengthTest ()
		{
			descriptor.ArrayLength = 1;
			Assert.AreEqual ((nuint) 1, descriptor.ArrayLength);
		}

		[Test]
		public void GetSetConstantBlockAlignmentTest ()
		{
			descriptor.ConstantBlockAlignment = 1;
			Assert.AreEqual ((nuint) 1, descriptor.ConstantBlockAlignment);
		}

		[Test]
		public void GetSetDataTypeTest ()
		{
			descriptor.DataType = MTLDataType.Half4;
			Assert.AreEqual (MTLDataType.Half4, descriptor.DataType);
		}

		[Test]
		public void GetSetIndexTest ()
		{
			descriptor.Index = 1;
			Assert.AreEqual ((nuint) 1, descriptor.Index);
		}

		[Test]
		public void GetSetTextureTypeTest ()
		{
			descriptor.TextureType = MTLTextureType.k2DArray;
			Assert.AreEqual (MTLTextureType.k2DArray, descriptor.TextureType);
		}
	}
}

#endif // !__WATCHOS__
