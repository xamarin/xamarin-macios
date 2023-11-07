#if !__WATCHOS__
#nullable enable

using System;

using Foundation;
using Metal;

using NUnit.Framework;


namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLBlitPassSampleBufferAttachmentDescriptorTest {
		MTLBlitPassSampleBufferAttachmentDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			descriptor = new MTLBlitPassSampleBufferAttachmentDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void SampleBufferTest ()
		{
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.SampleBuffer = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				using var buffer = descriptor.SampleBuffer;
			}, "Getter");
		}

		[Test]
		public void StartOfEncoderSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.StartOfEncoderSampleIndex = newIndex; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.StartOfEncoderSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Value");
		}

		[Test]
		public void EndOfEncoderSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.EndOfEncoderSampleIndex = newIndex; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.EndOfEncoderSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Value");
		}
	}

}

#endif // !__WATCHOS__
