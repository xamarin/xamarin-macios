//
// Unit tests for CVMetalTextureCache
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using CoreVideo;
using Metal;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVMetalTextureCacheTests {

		[Test]
		public void CVMetalTextureCacheCtorTest ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (9, 0);

			var d = MTLDevice.SystemDefault;
			var cache = new CVMetalTextureCache (d, new CVMetalTextureAttributes {
				Usage = MTLTextureUsage.PixelFormatView
			});

			Assert.NotNull (cache);
		}

		[Test]
		public void FromDeviceTest ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (9, 0);

			var d = MTLDevice.SystemDefault;
			var cache = CVMetalTextureCache.FromDevice (d, new CVMetalTextureAttributes {
				Usage = MTLTextureUsage.PixelFormatView
			});

			Assert.NotNull (cache);
		}
	}
}
#endif
