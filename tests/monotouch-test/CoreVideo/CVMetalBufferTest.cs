//
// Unit tests for CVMetalBuffer
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;

using CoreVideo;
using Foundation;
using Metal;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVMetalBufferTests {

		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			Assert.AreNotEqual (0, CVMetalBuffer.GetTypeId (), "GetTypeId");
		}

#if !MONOMAC
		[Test]
		[TestCase (CVPixelFormatType.CV32BGRA)]
		public void GetMetalBufferTest (CVPixelFormatType pft)
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			TestRuntime.AssertNotSimulator (); // metal api not supported in the simulator
			CVMetalBufferCacheTests.AssertSupported ();

			using var device = MTLDevice.SystemDefault;
			using var cache = new CVMetalBufferCache (device, (CVMetalBufferCacheAttributes) null);
			var dict = new CVPixelBufferAttributes () {
				MetalCompatibility = true,
			};
			using var image = new CVPixelBuffer (320, 320, pft, dict);
			using var buffer = cache.CreateBufferFromImage (image);
			Assert.IsNotNull (buffer, "Buffer");
			using var metalBuffer = buffer.GetMetalBuffer ();
			Assert.IsNotNull (metalBuffer, "GetMetalBuffer");
		}
#endif // !MONOMAC
	}
}
#endif
