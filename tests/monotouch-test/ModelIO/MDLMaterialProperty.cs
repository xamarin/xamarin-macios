//
// MDLMaterialProperty Unit Tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
#if !__TVOS__
using MultipeerConnectivity;
#endif
using ModelIO;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
#if !__TVOS__
using MonoTouch.MultipeerConnectivity;
#endif
using MonoTouch.UIKit;
using MonoTouch.ModelIO;
using MonoTouch.ObjCRuntime;
#endif
using OpenTK;
using MatrixFloat2x2 = global::OpenTK.NMatrix2;
using MatrixFloat3x3 = global::OpenTK.NMatrix3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
using VectorFloat3 = global::OpenTK.NVector3;
using Bindings.Test;
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLMaterialPropertyTest {
		[TestFixtureSetUp]
		public void Setup ()
		{

			if (!TestRuntime.CheckXcodeVersion (7, 0))
				Assert.Ignore ("Requires iOS 9.0+ or macOS 10.11+");

			if (
#if !MONOMAC
				Runtime.Arch == Arch.SIMULATOR && 
# endif
				IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				object dummy;
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion)) {
					dummy = obj.Color;
					dummy = obj.Float2Value;
					dummy = obj.Float3Value;
					dummy = obj.Float4Value;
					dummy = obj.FloatValue;
					dummy = obj.Matrix4x4;
					dummy = obj.Name;
					dummy = obj.Semantic;
					obj.SetProperties (new MDLMaterialProperty ("foo", MDLMaterialSemantic.AmbientOcclusion));
					dummy = obj.StringValue;
					dummy = obj.TextureSamplerValue;
					dummy = obj.Type;
					dummy = obj.UrlValue;
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, Vector3.Zero)) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, new MDLTextureSampler ())) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, "string value")) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, new NSUrl ("http://foo.com"))) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, Matrix4.Identity)) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, Vector4.Zero)) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, UIColor.Black.CGColor)) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, Vector2.Zero)) {
				}
				using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, 1.23f)) {
				}
			}
		}


		[Test]
		public void Ctors ()
		{
			Vector2 V2;
			Vector3 V3;
			Vector4 V4;
			Matrix4 M4;
			MatrixFloat4x4 M4x4;
			MDLTextureSampler tsv;
			NSUrl url;

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion)) {
				Assert.AreEqual (MDLMaterialSemantic.AmbientOcclusion, obj.Semantic, "1 Semantic");
				Assert.IsNull (obj.Color, "1 Color");
				Asserts.AreEqual (Vector2.Zero, obj.Float2Value, "1 Float2Value");
				Asserts.AreEqual (Vector3.Zero, obj.Float3Value, "1 Float3Value");
				Asserts.AreEqual (Vector4.Zero, obj.Float4Value, "1 Float4Value");
				Assert.AreEqual (0.0f, obj.FloatValue, "1 FloatValue");
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix4x4, "1 Matrix4x4");
				Assert.AreEqual ("name", obj.Name, "1 Name");
				Assert.IsNull (obj.StringValue, "1 StringValue");
				Assert.IsNull (obj.TextureSamplerValue, "1 TextureSamplerValue");
				Assert.AreEqual (MDLMaterialPropertyType.Float, obj.Type, "1 Type");
				Assert.IsNull (obj.UrlValue, "1 UrlValue");

				V2 = new Vector2 (1, 2);
				V3 = new Vector3 (3, 4, 5);
				V4 = new Vector4 (6, 7, 8, 9);
				M4 = new Matrix4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
				M4x4 = new MatrixFloat4x4 (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
				tsv = new MDLTextureSampler ();
				url = new NSUrl ("http://xamarin.com");

				obj.Semantic = MDLMaterialSemantic.Anisotropic;
				Assert.AreEqual (MDLMaterialSemantic.Anisotropic, obj.Semantic, "2 Semantic");

				obj.Color = UIColor.Blue.CGColor;
				Assert.AreEqual (UIColor.Blue.CGColor.ToString (), obj.Color.ToString (), "2 Color");

				obj.Float2Value = V2;
				Asserts.AreEqual (V2, obj.Float2Value, "2 Float2Value");

				obj.Float3Value = V3;
				Asserts.AreEqual (V3, obj.Float3Value, "2 Float3Value");

				obj.Float4Value = V4;
				Asserts.AreEqual (V4, obj.Float4Value, "2 Float4Value");

				obj.FloatValue = 3.14f;
				Assert.AreEqual (3.14f, obj.FloatValue, "2 FloatValue");

				obj.Matrix4x4 = M4;
				// It looks like the Matrix4 setter is ignored, assigning a matrix
				// doesn't work in Xcode either.
				Asserts.AreEqual (Matrix4.Identity, obj.Matrix4x4, "2 Matrix4x4");

				obj.Name = "new name";
				Assert.AreEqual ("new name", obj.Name, "2 Name");

				obj.StringValue = "string value";
				Assert.AreEqual ("string value", obj.StringValue, "2 StringValue");

				obj.TextureSamplerValue = tsv;
				Assert.AreEqual (tsv.Handle, obj.TextureSamplerValue.Handle, "2 TextureSamplerValue");

				Assert.AreEqual (MDLMaterialPropertyType.Texture, obj.Type, "2 Type");

				// Looks like the URLValue can't change after construction
				obj.UrlValue = url;
				if (TestRuntime.CheckXcodeVersion (9, 0)) {
					Assert.AreSame (url, obj.UrlValue, "2 UrlValue");
				} else {
					Assert.IsNull (obj.UrlValue, "2 UrlValue");
				}
			}


			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, url)) {
				Assert.AreEqual (url.Handle, obj.UrlValue.Handle, "3 UrlValue");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, V3)) {
				Asserts.AreEqual (V3, obj.Float3Value, "4 Float3Value");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, tsv)) {
				Assert.AreEqual (tsv.Handle, obj.TextureSamplerValue.Handle, "5 TextureSamplerValue");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, "string value")) {
				Assert.AreEqual ("string value", obj.StringValue, "6 StringValue");
			}
				
			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, M4)) {
				Asserts.AreEqual (M4, obj.Matrix4x4, "7 Matrix4x4");
				Asserts.AreEqual (CFunctions.GetMatrixFloat4x4 (obj, "matrix4x4"), obj.MatrixFloat4x4, "7b MatrixFloat4x4");
				Asserts.AreEqual (MatrixFloat4x4.Transpose ((MatrixFloat4x4) M4), obj.MatrixFloat4x4, "7c MatrixFloat4x4");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, M4x4)) {
				Asserts.AreEqual (CFunctions.GetMatrixFloat4x4 (obj, "matrix4x4"), obj.MatrixFloat4x4, "7' MatrixFloat4x4");
				Asserts.AreEqual (M4x4, obj.MatrixFloat4x4, "7'b MatrixFloat4x4");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, V4)) {
				Asserts.AreEqual (V4, obj.Float4Value, "8 Float4Value");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, UIColor.Red.CGColor)) {
				Assert.AreEqual (UIColor.Blue.CGColor.ToString (), obj.Color.ToString (), "9 Color");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, V2)) {
				Asserts.AreEqual (V2, obj.Float2Value, "10 Float2Value");
			}

			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion, 3.1415f)) {
				Assert.AreEqual (3.1415f, obj.FloatValue, "11 FloatValue");
			}
		}

		[Test]
		public void Copy ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			using (var obj = new MDLMaterialProperty ("name", MDLMaterialSemantic.AmbientOcclusion)) {
				Assert.IsNotNull (obj.Copy ());
			}
		}
	}
}

#endif // !__WATCHOS__
