#if !__WATCHOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;


namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLBlitPassDescriptorTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
		}

		[Test]
		public void TestSampleBufferAttachments ()
		{
			// need to be tested since it fails intro
			using var passDescripton = MTLBlitPassDescriptor.Create ();
			Assert.IsNotNull (passDescripton, "passDescriptor");
			Assert.DoesNotThrow (() => {
				using var attachments = passDescripton.SampleBufferAttachments; // don't care about the value, just that it works
			}, "Attachements");
		}
	}

}

#endif // !__WATCHOS__
