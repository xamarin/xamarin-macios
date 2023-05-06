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
	public class MTLTileRenderPipelineColorAttachmentDescriptorTests {
		MTLTileRenderPipelineColorAttachmentDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
			descriptor = new MTLTileRenderPipelineColorAttachmentDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
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
