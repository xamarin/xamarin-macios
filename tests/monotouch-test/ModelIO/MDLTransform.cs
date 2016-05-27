//
// MDLTransform Unit Tests
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
using UIKit;
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
using NUnit.Framework;

namespace MonoTouchFixtures.ModelIO {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class MDLTransformTest {
		[TestFixtureSetUp]
		public void Setup ()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0))
				Assert.Ignore ("Requires iOS9+");

			if (Runtime.Arch == Arch.SIMULATOR && IntPtr.Size == 4) {
				// There's a bug in the i386 version of objc_msgSend where it doesn't preserve SIMD arguments
				// when resizing the cache of method selectors for a type. So here we call all selectors we can
				// find, so that the subsequent tests don't end up producing any cache resize (radar #21630410).
				object dummy;
				using (var obj = new MDLTransform (Matrix4.Identity)) {
					dummy = obj.Matrix;
					dummy = obj.MaximumTime;
					dummy = obj.MinimumTime;
					dummy = obj.Rotation;
					obj.GetRotation (0);
					dummy = obj.Scale;
					obj.GetScale (0);
					obj.SetIdentity ();
					obj.SetLocalTransform (Matrix4.Identity);
					obj.SetRotation (Vector3.Zero, 0);
					obj.SetScale (Vector3.Zero, 0);
					obj.SetTranslation (Vector3.Zero, 0);
					dummy = obj.Translation;
					obj.GetTranslation (0);
				}
			}
		}

		[Test]
		public void Ctors ()
		{
			Matrix4 id = Matrix4.Identity;
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (id)) {
				Asserts.AreEqual (Vector3.Zero, obj.Translation, "Translation");
				Asserts.AreEqual (Vector3.One, obj.Scale, "Scale");
				Asserts.AreEqual (Vector3.Zero, obj.Rotation, "Rotation");
				Asserts.AreEqual (id, obj.Matrix, "Matrix");

				obj.Translation = V3;
				Asserts.AreEqual (V3, obj.Translation, "Translation 2");
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Scale = V3;
				Asserts.AreEqual (V3, obj.Scale, "Scale 2");
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Rotation = V3;
				Asserts.AreEqual (V3, obj.Rotation, "Rotation 2");
			}

			using (var obj = new MDLTransform (id)) {
				V3 *= 2;
				obj.Rotation = V3;
				Asserts.AreEqual (V3, obj.Rotation, "Rotation 2");
			}
		}
			
		[Test]
		public void ScaleAtTimeTest ()
		{
			var matrix = Matrix4.Identity;
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetScale (V3, 0);
				Asserts.AreEqual (V3, obj.GetScale (0), "ScaleAtTime");
			}
		}

		[Test]
		public void TranslationAtTimeTest ()
		{
			var matrix = Matrix4.Identity;
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetTranslation (V3, 0);
				Asserts.AreEqual (V3, obj.GetTranslation (0), "TranslationAtTime");
			}
		}

		[Test]
		public void RotationAtTimeTest ()
		{
			var matrix = Matrix4.Identity;
			var V3 = new Vector3 (1, 2, 3);

			using (var obj = new MDLTransform (matrix)) {
				obj.SetRotation (V3, 0);
				Asserts.AreEqual (V3, obj.GetRotation (0), "RotationAtTime");
			}
		}
	}
}

#endif // !__WATCHOS__
