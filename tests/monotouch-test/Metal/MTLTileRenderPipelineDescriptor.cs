#if __IOS__

using System;

#if XAMCORE_2_0
using Metal;
#else
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLTileRenderPipelineDescriptorTests {
		MTLTileRenderPipelineDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			descriptor = new MTLTileRenderPipelineDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor != null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void ColorAttachmentsTest ()
		{
			var attachments = descriptor.ColorAttachments;
			Assert.NotNull (attachments);
		}

		[Test]
		public void GetSetLabelTest ()
		{
			descriptor.Label = "Foo";
			Assert.AreEqual ("Foo", descriptor.Label);
		}

		[Test]
		public void GetSetRasterSampleCount ()
		{
			descriptor.RasterSampleCount = 2;
			Assert.AreEqual (2, descriptor.RasterSampleCount);
		}

		[Test]
		public void GetSetThreadgroupSizeMatchesTileSize ()
		{
			descriptor.ThreadgroupSizeMatchesTileSize = true;
			Assert.AreEqual (true, descriptor.ThreadgroupSizeMatchesTileSize);

			descriptor.ThreadgroupSizeMatchesTileSize = false;
			Assert.AreEqual (false, descriptor.ThreadgroupSizeMatchesTileSize);
		}

		[Test]
		public void GetTileBuffers ()
		{
			var buffers = descriptor.TileBuffers;
			Assert.NotNull (buffers);
		}

		[Test]
		public void GetSetMaxTotalThreadsPerThreadgroupTest ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			descriptor.MaxTotalThreadsPerThreadgroup = 10;
			Assert.AreEqual (10, descriptor.MaxTotalThreadsPerThreadgroup);
		}
	}
}

#endif // !__WATCHOS__