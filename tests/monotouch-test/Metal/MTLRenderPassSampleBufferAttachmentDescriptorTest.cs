#if !__WATCHOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLRenderPassSampleBufferAttachmentDescriptorTest {
		MTLRenderPassSampleBufferAttachmentDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			descriptor = new MTLRenderPassSampleBufferAttachmentDescriptor ();
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

			Assert.DoesNotThrow (() => {
				descriptor.SampleBuffer = null; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				using var f = descriptor.SampleBuffer;
			}, "Getter");
		}

		[Test]
		public void StartOfVertexSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;

			Assert.DoesNotThrow (() => {
				descriptor.StartOfVertexSampleIndex = newIndex;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.StartOfVertexSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Index");
		}

		[Test]
		public void EndOfVertexSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;

			Assert.DoesNotThrow (() => {
				descriptor.EndOfVertexSampleIndex = newIndex;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.EndOfVertexSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Index");
		}

		[Test]
		public void StartOfFragmentSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;

			Assert.DoesNotThrow (() => {
				descriptor.StartOfFragmentSampleIndex = newIndex;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.StartOfFragmentSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Index");
		}

		[Test]
		public void EndOfFragmentSampleIndexTest ()
		{
			nuint newIndex = 10;
			nuint objIndex = 0;

			Assert.DoesNotThrow (() => {
				descriptor.EndOfFragmentSampleIndex = newIndex;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objIndex = descriptor.EndOfFragmentSampleIndex;
			}, "Getter");
			Assert.AreEqual (newIndex, objIndex, "Index");
		}
	}
}
#endif
