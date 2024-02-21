#if !__WATCHOS__

using System;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLComputeCommandEncoderTest {
		IMTLDevice device;
		IMTLCommandQueue commandQ;
		IMTLCommandBuffer commandBuffer;
		IMTLComputeCommandEncoder encoder;

		[SetUp]
		public void SetUp ()
		{
			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			commandQ = device.CreateCommandQueue ();
			if (commandQ is null)  // this happens on a simulator
				Assert.Inconclusive ("Could not get the functions library for the device.");

			commandBuffer = commandQ.CommandBuffer ();
			if (commandBuffer is null) // happens on sim
				Assert.Inconclusive ("Could not get the command buffer for the device.");

			encoder = commandBuffer.ComputeCommandEncoder;
		}

		[TearDown]
		public void TearDown ()
		{
			commandQ?.Dispose ();
			commandQ = null;
			commandBuffer?.Dispose ();
			commandBuffer = null;
			encoder?.EndEncoding ();
			encoder?.Dispose ();
			encoder = null;
		}

#if NET
		[Test]
		public void SetBuffers ()
		{
			Assert.Throws<ArgumentNullException> (() => {
				encoder.SetBuffers (null, new nuint [0], new NSRange ());
			}, "Null buffers should throw.");

			Assert.Throws<ArgumentNullException> (() => {
				encoder.SetBuffers (new IMTLBuffer [0], null, new NSRange ());
			}, "Null offsets should throw.");

			// assert we do not crash or throw, we are testing the extension method
			Assert.DoesNotThrow (() => {
				encoder.SetBuffers (new IMTLBuffer [0], new nuint [0], new NSRange ());

			}, "Should not throw");
		}
#endif
	}
}
#endif
