#if !__WATCHOS__

using System;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class MTLArgumentEncoderTest {
		IMTLDevice device;
		IMTLLibrary library;
		IMTLFunction function;
		IMTLArgumentEncoder encoder;

		[SetUp]
		public void SetUp ()
		{
			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null)
				Assert.Inconclusive ("Metal is not supported");

			library = device.CreateDefaultLibrary ();
			if (library == null)  // this happens on a simulator
				Assert.Inconclusive ("Could not get the functions library for the device.");

			if (library.FunctionNames.Length == 0)
				Assert.Inconclusive ("Could not get functions for the pipeline.");

			function = library.CreateFunction (library.FunctionNames [0]);
			encoder = function.CreateArgumentEncoder (0);
		}

		[TearDown]
		public void TearDown ()
		{
			library?.Dispose ();
			library = null;
			function?.Dispose ();
			function = null;
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
