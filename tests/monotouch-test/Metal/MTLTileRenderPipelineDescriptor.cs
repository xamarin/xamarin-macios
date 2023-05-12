#if __IOS__

using System;

using Foundation;
using Metal;
using ObjCRuntime;

using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLTileRenderPipelineDescriptorTests {
		MTLTileRenderPipelineDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
			descriptor = new MTLTileRenderPipelineDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
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
			Assert.AreEqual ((nuint) 2, descriptor.RasterSampleCount);
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
			Assert.AreEqual ((nuint) 10, descriptor.MaxTotalThreadsPerThreadgroup);
		}
	}
}

#endif // !__WATCHOS__
