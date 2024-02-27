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
using Foundation;
using ModelIO;
using ObjCRuntime;
using NUnit.Framework;

#if NET
using System.Numerics;
using Vector2i = global::CoreGraphics.NVector2i;
#else
using OpenTK;
#endif

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLNoiseTextureTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (8, 2);
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
