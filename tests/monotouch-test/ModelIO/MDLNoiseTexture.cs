//
// MDLNoiseTexture Unit Tests
//
// Authors:
//	Vincent Dondain <vidondai@microsoft.com>
//
// Copyright 2016 Microsoft
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using ModelIO;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
#endif
using OpenTK;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO
{

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLNoiseTextureTest
	{
		[TestFixtureSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (8, 2);

#if !MONOMAC
			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				object dummy;
				using (var obj = new MDLNoiseTexture (1.0f, "texture", Vector2i.Zero, MDLTextureChannelEncoding.Float16)) {
					dummy = obj.ChannelCount;
					dummy = obj.ChannelEncoding;
					dummy = obj.Dimensions;
					dummy = obj.IsCube;
					dummy = obj.MipLevelCount;
					dummy = obj.Name;
					dummy = obj.RowStride;
					obj.GetTexelDataWithBottomLeftOrigin ();
					obj.GetTexelDataWithBottomLeftOrigin (1, false);
					obj.GetTexelDataWithTopLeftOrigin ();
					obj.GetTexelDataWithTopLeftOrigin (1, false);
				}
				using (var obj = new MDLTexture ()) {
				}
			}
#endif
		}

		[Test]
		public void Ctor ()
		{
			var V2 = new Vector2i (123, 456);

			using (var obj = new MDLNoiseTexture (1.0f, "texture", V2, MDLTextureChannelEncoding.Float16, MDLNoiseTextureType.Cellular)) {
				Asserts.AreEqual (V2, obj.Dimensions, "dimensions");
			}

			using (var obj = new MDLNoiseTexture (1.0f, "texture", V2, MDLTextureChannelEncoding.Float16, MDLNoiseTextureType.Vector)) {
				Asserts.AreEqual (V2, obj.Dimensions, "dimensions");
			}
		}
	}
}

#endif // !__WATCHOS__
