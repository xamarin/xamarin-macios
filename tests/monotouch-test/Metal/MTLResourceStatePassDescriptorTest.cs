#if !__WATCHOS__ && !__TVOS__
#nullable enable

using System;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLResourceStatePassDescriptorTest {
		MTLResourceStatePassDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
#if __MACOS__
			TestRuntime.AssertXcodeVersion (12, 2);
#else
			TestRuntime.AssertXcodeVersion (12, 0);
#endif
			descriptor = MTLResourceStatePassDescriptor.Create ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null; 
		}

		[Test]
		public void SampleBufferAttachments ()
		{
			Assert.DoesNotThrow (() => {
				using var attachments = descriptor.SampleBufferAttachments; // testing the selector, dont care about the value.
			}, "Getter");
		}
	}
}

#endif
