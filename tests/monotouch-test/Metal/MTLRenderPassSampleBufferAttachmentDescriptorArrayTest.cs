#if !__WATCHOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLRenderPassSampleBufferAttachmentDescriptorArrayTest {
		MTLRenderPassSampleBufferAttachmentDescriptorArray array;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			array = new MTLRenderPassSampleBufferAttachmentDescriptorArray ();
		}

		[TearDown]
		public void TearDown ()
		{
			array?.Dispose ();
			array = null;
		}

		[Test]
		public void IndexerTest ()
		{

			var obj = new MTLRenderPassSampleBufferAttachmentDescriptor ();
			MTLRenderPassSampleBufferAttachmentDescriptor dupe = null;
			Assert.DoesNotThrow (() => {
				array [0] = obj;
			});
			Assert.DoesNotThrow (() => {
				dupe = array [0];
			});
			Assert.IsNotNull (dupe, "Dupe");
			Assert.AreNotEqual (IntPtr.Zero, dupe.Handle, "Dupe");
		}
	}
}
#endif
