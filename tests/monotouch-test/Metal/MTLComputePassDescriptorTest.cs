#if !__WATCHOS__
#nullable enable

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLComputePassDescriptorTest {
		MTLComputePassDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			descriptor = MTLComputePassDescriptor.Create ();
		}

		[TearDown]
		public void TearDown ()
		{
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void DispatchTypeTest ()
		{
			MTLDispatchType newType = MTLDispatchType.Concurrent;
			MTLDispatchType objType = MTLDispatchType.Serial;
			// we want to make sure we do not crash because intro fails 
			Assert.DoesNotThrow (() => {
				descriptor.DispatchType = newType; // we are testing if the property works, so setting to null does test the selector
			}, "Setter");
			Assert.DoesNotThrow (() => {
				objType = descriptor.DispatchType;
			}, "Getter");
			Assert.AreEqual (newType, objType, "Type");
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

#endif // !__WATCHOS__
