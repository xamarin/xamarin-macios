#if !__WATCHOS__
#nullable enable

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLComputePassSampleBufferAttachmentDescriptorArrayTest {
		MTLComputePassSampleBufferAttachmentDescriptorArray array;

		[SetUp]
		public void SetUp ()
		{
#if __MACOS__
			TestRuntime.AssertXcodeVersion (12, 2);
#else
			TestRuntime.AssertXcodeVersion (12, 0);
#endif 
			array = new MTLComputePassSampleBufferAttachmentDescriptorArray ();
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

			var obj = new MTLComputePassSampleBufferAttachmentDescriptor ();
			MTLComputePassSampleBufferAttachmentDescriptor dupe = null;
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
