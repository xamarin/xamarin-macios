#if !__WATCHOS__ && !__TVOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLResourceStatePassDescriptorTest {
		MTLResourceStatePassDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
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
