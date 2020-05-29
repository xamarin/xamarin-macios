#if __IOS__

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLTileRenderPipelineColorAttachmentDescriptorTests {
		MTLTileRenderPipelineColorAttachmentDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			descriptor = new MTLTileRenderPipelineColorAttachmentDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor != null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetSetPixelFormat ()
		{
			descriptor.PixelFormat = MTLPixelFormat.RGBA8Snorm;
			Assert.AreEqual (MTLPixelFormat.RGBA8Snorm, descriptor.PixelFormat);
		}
	}
}

#endif // !__WATCHOS__