//
// Unit tests for CVMetalBufferCache
//

#if !__WATCHOS__

using System;

using CoreVideo;
using Foundation;
using Metal;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVMetalBufferCacheTests {

		[Test]
		public void GetTypeIdTest ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			Assert.AreNotEqual (0, CVMetalBufferCache.GetTypeId (), "GetTypeId");
		}

		[Test]
		public void CtorTest_NSDictionary ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Ignore ("Metal is not supported on this device.");

			using var cache = new CVMetalBufferCache (device, (NSDictionary) null);
			Assert.IsNotNull (cache);
		}

		[Test]
		public void CtorTest_CVMetalBufferCacheAttributes ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Ignore ("Metal is not supported on this device.");

			using var cache = new CVMetalBufferCache (device, (CVMetalBufferCacheAttributes) null);
			Assert.IsNotNull (cache);
		}

		[Test]
		[TestCase (CVPixelFormatType.CV32BGRA)]
		public void CreateBufferFromImageTest (CVPixelFormatType pft)
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			TestRuntime.AssertNotSimulator (); // metal api not supported in the simulator

			using var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Ignore ("Metal is not supported on this device.");

			using var cache = new CVMetalBufferCache (device, (CVMetalBufferCacheAttributes) null);
			var dict = new CVPixelBufferAttributes () {
				MetalCompatibility = true,
			};
			using var image = new CVPixelBuffer (320, 320, pft, dict);
			using var buffer = cache.CreateBufferFromImage (image);
			Assert.IsNotNull (buffer, "Buffer");
		}

		[Test]
		public void FlushTest ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var device = MTLDevice.SystemDefault;
			if (device is null)
				Assert.Ignore ("Metal is not supported on this device.");

			using var cache = new CVMetalBufferCache (device, (CVMetalBufferCacheAttributes) null);
			cache.Flush ();
			cache.Flush (CVOptionFlags.None);
		}
	}
}
#endif
