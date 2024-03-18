#if !__WATCHOS__

using System;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLArgumentEncoderTest {
		IMTLDevice device;
		IMTLLibrary library;
		IMTLFunction function;
		IMTLArgumentEncoder encoder;

		[SetUp]
		public void SetUp ()
		{
			// The call to CreateArgumentEncoder below doesn't seem to work
			// on earlier versions of iOS (at least fails on iOS 12), so just
			// skip those versions for now.
			TestRuntime.AssertXcodeVersion (11, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			library = device.CreateDefaultLibrary ();
			if (library is null)  // this happens on a simulator
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
