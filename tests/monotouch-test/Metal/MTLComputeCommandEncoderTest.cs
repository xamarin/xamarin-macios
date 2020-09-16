#if !__WATCHOS__

using System;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
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
			if (device == null)
				Assert.Inconclusive ("Metal is not supported");

			commandQ = device.CreateCommandQueue ();
			if (commandQ == null)  // this happens on a simulator
				Assert.Inconclusive ("Could not get the functions library for the device.");

			commandBuffer = commandQ.CommandBuffer ();
			if (commandBuffer == null) // happens on sim
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
			encoder?.Dispose ();
			encoder = null;
		}

		[Test]
		public void SetBuffers ()
		{
			Assert.Throws<ArgumentNullException> (() => {
#if XAMCORE_4_0
				encoder.SetBuffers (null, new nuint [0], new NSRange ());
#else
				encoder.SetManagedBuffers (null, new nuint [0], new NSRange ());
#endif
			}, "Null buffers should throw.");

			Assert.Throws<ArgumentNullException> (() => {
#if XAMCORE_4_0
				encoder.SetBuffers (new IMTLBuffer [0], null, new NSRange ());
#else
				encoder.SetManagedBuffers (new IMTLBuffer [0], null, new NSRange ());
#endif
			}, "Null offsets should throw.");

			// assert we do not crash or throw, we are testing the extension method
			Assert.DoesNotThrow (() => {
#if XAMCORE_4_0
				encoder.SetBuffers (new IMTLBuffer [0], new nuint [0], new NSRange ());
#else
				encoder.SetManagedBuffers (new IMTLBuffer [0], new nuint [0], new NSRange ());
#endif

			}, "Should not throw");
		}
	}
}
#endif
