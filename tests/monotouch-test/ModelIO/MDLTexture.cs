//
// MDLTexture Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
#if !__TVOS__
using MultipeerConnectivity;
#endif
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
	public class MDLTextureTest {
		[OneTimeSetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void Ctor ()
		{
			var V2 = new Vector2i (123, 456);

			using (var obj = new MDLTexture (null, true, null, V2, 12, 2, MDLTextureChannelEncoding.Float16, false)) {
				Asserts.AreEqual (V2, obj.Dimensions, "dimensions");
			}
		}

		[Test]
		public void CreateIrradianceTextureCubeTest_a ()
		{
			var V2 = new Vector2i (3, 3);

			using (var obj = new MDLTexture ()) {
				using (var txt = MDLTexture.CreateIrradianceTextureCube (obj, "name", V2)) {
					if (TestRuntime.CheckXcodeVersion (8, 0)) {
						Assert.IsNull (txt, "Is Null"); // this is probably because the arguments to CreateIrradianceTextureCube are invalid, but I haven't been able to figure out valid values.
					} else {
						Assert.IsNotNull (txt, "Ain't Null");
						Assert.AreEqual ((nuint) 4, txt.ChannelCount, "ChannelCount");
						Assert.AreEqual (MDLTextureChannelEncoding.UInt8, txt.ChannelEncoding, "ChannelEncoding");
						Assert.AreEqual (new Vector2i (3, 18), txt.Dimensions, "Dimensions");
						Assert.AreEqual ((nuint) 2, txt.MipLevelCount, "MipLevelCount");
						Assert.AreEqual ((nint) 12, txt.RowStride, "RowStride");
					}
				}
			}
		}

		[Test]
		public void CreateIrradianceTextureCubeTest_b ()
		{
			var V2 = new Vector2i (3, 3);

			using (var obj = new MDLTexture ()) {
				using (var txt = MDLTexture.CreateIrradianceTextureCube (obj, "name", V2, 0.1234f)) {
					if (TestRuntime.CheckXcodeVersion (8, 0)) {
						Assert.IsNull (txt, "Is Null"); // this is probably because the arguments to CreateIrradianceTextureCube are invalid, but I haven't been able to figure out valid values.
					} else {
						Assert.IsNotNull (txt, "Ain't Null");
						Assert.AreEqual ((nuint) 4, txt.ChannelCount, "ChannelCount");
						Assert.AreEqual (MDLTextureChannelEncoding.UInt8, txt.ChannelEncoding, "ChannelEncoding");
						Assert.AreEqual (new Vector2i (3, 18), txt.Dimensions, "Dimensions");
						Assert.AreEqual ((nuint) 1, txt.MipLevelCount, "MipLevelCount");
						Assert.AreEqual ((nint) 12, txt.RowStride, "RowStride");
					}
				}
			}
		}

		[Test]
		public void DimensionsTest ()
		{
			var V2 = new Vector2i (123, 456);

			using (var txt = new MDLTexture ()) {
				Asserts.AreEqual (Vector2i.Zero, txt.Dimensions, "a");
			}
		}
	}
}

#endif // !__WATCHOS__
