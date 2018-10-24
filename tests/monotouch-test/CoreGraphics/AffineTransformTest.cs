//
// Unit tests for CGAffineTransform
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
#endif

using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AffineTransformTest {
		[Test]
		public void Ctor ()
		{
			var transform = new CGAffineTransform ();
			Assert.AreEqual ((nfloat) 0, transform.xx);
			Assert.AreEqual ((nfloat) 0, transform.yx);
			Assert.AreEqual ((nfloat) 0, transform.xy);
			Assert.AreEqual ((nfloat) 0, transform.yy);
			Assert.AreEqual ((nfloat) 0, transform.x0);
			Assert.AreEqual ((nfloat) 0, transform.y0);

			transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			Assert.AreEqual ((nfloat) 1, transform.xx);
			Assert.AreEqual ((nfloat) 2, transform.yx);
			Assert.AreEqual ((nfloat) 3, transform.xy);
			Assert.AreEqual ((nfloat) 4, transform.yy);
			Assert.AreEqual ((nfloat) 5, transform.x0);
			Assert.AreEqual ((nfloat) 6, transform.y0);
		}

		[Test]
		public void MakeIdentity ()
		{
			var transform = CGAffineTransform.MakeIdentity ();

			Assert.AreEqual ((nfloat) 1, transform.xx, "xx");
			Assert.AreEqual ((nfloat) 0, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 0, transform.xy, "xy");
			Assert.AreEqual ((nfloat) 1, transform.yy, "yy");
			Assert.AreEqual ((nfloat) 0, transform.x0, "x0");
			Assert.AreEqual ((nfloat) 0, transform.y0, "y0");

			Assert.IsTrue (transform.IsIdentity, "identity");
		}

		[Test]
		public void MakeRotation ()
		{
			var transform = CGAffineTransform.MakeRotation ((nfloat) Math.PI);

			Assert.AreEqual ((nfloat) (-1), transform.xx, "xx");
			Assert.That ((double) 0, Is.EqualTo ((double) transform.yx).Within (0.0000001), "yx");
			Assert.That ((double) 0, Is.EqualTo ((double) transform.xy).Within (0.0000001), "xy");
			Assert.AreEqual ((nfloat) (-1), transform.yy, "yy");
			Assert.That ((double) 0, Is.EqualTo ((double) transform.x0).Within (0.0000001), "x0");
			Assert.That ((double) 0, Is.EqualTo ((double) transform.y0).Within (0.0000001), "y0");
		}

		[Test]
		public void MakeScale ()
		{
			var transform = CGAffineTransform.MakeScale (314, 413);
			Assert.AreEqual ((nfloat) 314, transform.xx);
			Assert.AreEqual ((nfloat) 0, transform.yx);
			Assert.AreEqual ((nfloat) 0, transform.xy);
			Assert.AreEqual ((nfloat) 413, transform.yy);
			Assert.AreEqual ((nfloat) 0, transform.x0);
			Assert.AreEqual ((nfloat) 0, transform.y0);
		}

		[Test]
		public void MakeTranslation ()
		{
			var transform = CGAffineTransform.MakeTranslation (12, 23);

			Assert.AreEqual ((nfloat) 1, transform.xx, "xx");
			Assert.AreEqual ((nfloat) 0, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 0, transform.xy, "xy");
			Assert.AreEqual ((nfloat) 1, transform.yy, "yy");
			Assert.AreEqual ((nfloat) 12, transform.x0, "x0");
			Assert.AreEqual ((nfloat) 23, transform.y0, "y0");
		}

		[Test]
		public void Multiply ()
		{
			var a = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			var transform = new CGAffineTransform (9, 8, 7, 6, 5, 4);
			transform.Multiply (a);

			Assert.AreEqual ((nfloat) 33, transform.xx, "xx");
			Assert.AreEqual ((nfloat) 50, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 25, transform.xy, "xy");
			Assert.AreEqual ((nfloat) 38, transform.yy, "yy");
			Assert.AreEqual ((nfloat) 22, transform.x0, "x0");
			Assert.AreEqual ((nfloat) 32, transform.y0, "y0");
		}

		[Test]
		public void StaticMultiply ()
		{
			var a = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			var b = new CGAffineTransform (9, 8, 7, 6, 5, 4);
			var transform = CGAffineTransform.Multiply (a, b);

			Assert.AreEqual ((nfloat) 23, transform.xx, "xx");
			Assert.AreEqual ((nfloat) 20, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 55, transform.xy, "xy");
			Assert.AreEqual ((nfloat) 48, transform.yy, "yy");
			Assert.AreEqual ((nfloat) 92, transform.x0, "x0");
			Assert.AreEqual ((nfloat) 80, transform.y0, "y0");
		}
		[Test]
		public void Scale ()
		{
			var transform1 = CGAffineTransform.MakeTranslation (1, 2);
			// t' = t * [ sx 0 0 sy 0 0 ]
			transform1.Scale (3, 4); // MatrixOrder.Append by default

			Assert.AreEqual ((nfloat) 3, transform1.xx);
			Assert.AreEqual ((nfloat) 0, transform1.yx);
			Assert.AreEqual ((nfloat) 0, transform1.xy);
			Assert.AreEqual ((nfloat) 4, transform1.yy);
			Assert.AreEqual ((nfloat) 3, transform1.x0);
			Assert.AreEqual ((nfloat) 8, transform1.y0);

			var transform2 = CGAffineTransform.MakeTranslation (1, 2);
			// t' = [ sx 0 0 sy 0 0 ] * t – Swift equivalent
			transform2.Scale (3, 4, MatrixOrder.Prepend);

			Assert.AreEqual ((nfloat)3, transform2.xx);
			Assert.AreEqual ((nfloat)0, transform2.yx);
			Assert.AreEqual ((nfloat)0, transform2.xy);
			Assert.AreEqual ((nfloat)4, transform2.yy);
			Assert.AreEqual ((nfloat)1, transform2.x0);
			Assert.AreEqual ((nfloat)2, transform2.y0);
		}

		[Test]
		public void StaticScale ()
		{
			var transformM = CGAffineTransform.Scale (CGAffineTransform.MakeTranslation (0, 200), 1, -1);
			var transformN = CGAffineTransformScale (CGAffineTransform.MakeTranslation (0, 200), 1, -1);

			Assert.IsTrue (transformM == transformN, "1");

			transformM = CGAffineTransform.Scale (CGAffineTransform.MakeTranslation (1, 2), -3, -4);
			transformN = CGAffineTransformScale (CGAffineTransform.MakeTranslation (1, 2), -3, -4);

			Assert.IsTrue (transformM == transformN, "2");
		}

		[DllImport (global::ObjCRuntime.Constants.CoreGraphicsLibrary)]
		public extern static CGAffineTransform CGAffineTransformScale (CGAffineTransform t, nfloat sx, nfloat sy);

		[Test]
		public void Translate ()
		{
			var transform = CGAffineTransform.MakeIdentity ();
			transform.Translate (1, -1); // MatrixOrder.Append by default

			Assert.AreEqual ((nfloat) 1, transform.xx, "xx");
			Assert.AreEqual ((nfloat) 0, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 0, transform.xy, "xy");
			Assert.AreEqual ((nfloat) 1, transform.yy, "yy");
			Assert.AreEqual ((nfloat) 1, transform.x0, "x0");
			Assert.AreEqual ((nfloat) (-1), transform.y0, "y0");

			transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			transform.Translate (2, -3);

			Assert.AreEqual ((nfloat)1, transform.xx, "xx");
			Assert.AreEqual ((nfloat)2, transform.yx, "yx");
			Assert.AreEqual ((nfloat)3, transform.xy, "xy");
			Assert.AreEqual ((nfloat)4, transform.yy, "yy");
			Assert.AreEqual ((nfloat)7, transform.x0, "x0");
			Assert.AreEqual ((nfloat)3, transform.y0, "y0");

			transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			transform.Translate (2, -3, MatrixOrder.Prepend);

			Assert.AreEqual ((nfloat)1, transform.xx, "xx");
			Assert.AreEqual ((nfloat)2, transform.yx, "yx");
			Assert.AreEqual ((nfloat)3, transform.xy, "xy");
			Assert.AreEqual ((nfloat)4, transform.yy, "yy");
			Assert.AreEqual ((nfloat)(-2), transform.x0, "x0");
			Assert.AreEqual ((nfloat)(-2), transform.y0, "y0");
		}

		[Test]
		public void StaticTranslate ()
		{
			var origin = CGAffineTransform.MakeIdentity ();
			var transformM = CGAffineTransform.Translate (origin, 1, -1);
			var transformN = CGAffineTransformTranslate (origin, 1, -1);

			Assert.AreEqual ((nfloat) 1, transformM.xx, "xx");
			Assert.AreEqual ((nfloat) 0, transformM.yx, "yx");
			Assert.AreEqual ((nfloat) 0, transformM.xy, "xy");
			Assert.AreEqual ((nfloat) 1, transformM.yy, "yy");
			Assert.AreEqual ((nfloat) 1, transformM.x0, "x0");
			Assert.AreEqual ((nfloat) (-1), transformM.y0, "y0");
			Assert.IsTrue (transformN == transformM);

			origin = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			transformM = CGAffineTransform.Translate (origin, 2, -3);
			transformN = CGAffineTransformTranslate (origin, 2, -3);

			Assert.AreEqual ((nfloat) 1, transformM.xx, "xx");
			Assert.AreEqual ((nfloat) 2, transformM.yx, "yx");
			Assert.AreEqual ((nfloat) 3, transformM.xy, "xy");
			Assert.AreEqual ((nfloat) 4, transformM.yy, "yy");
			Assert.AreEqual ((nfloat) (-2), transformM.x0, "x0");
			Assert.AreEqual ((nfloat) (-2), transformM.y0, "y0");
			Assert.IsTrue (transformN == transformM);
		}

		[DllImport (global::ObjCRuntime.Constants.CoreGraphicsLibrary)]
		public extern static CGAffineTransform CGAffineTransformTranslate (CGAffineTransform t, nfloat sx, nfloat sy);

		[Test]
		public void Rotate ()
		{
			var transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			transform.Rotate ((nfloat) Math.PI); // MatrixOrder.Append by default

			Assert.That ((double) (-1), Is.EqualTo ((double) transform.xx).Within (0.000001), "xx");
			Assert.That ((double) (-2), Is.EqualTo ((double) transform.yx).Within (0.000001), "yx");
			Assert.That ((double) (-3), Is.EqualTo ((double) transform.xy).Within (0.000001), "xy");
			Assert.That ((double) (-4), Is.EqualTo ((double) transform.yy).Within (0.000001), "yy");
			Assert.That ((double) (-5), Is.EqualTo ((double) transform.x0).Within (0.000001), "x0");
			Assert.That ((double) (-6), Is.EqualTo ((double) transform.y0).Within (0.000001), "y0");

			transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			transform.Rotate ((nfloat)Math.PI, MatrixOrder.Prepend);

			Assert.That ((double)(-1), Is.EqualTo ((double)transform.xx).Within (0.000001), "xx");
			Assert.That ((double)(-2), Is.EqualTo ((double)transform.yx).Within (0.000001), "yx");
			Assert.That ((double)(-3), Is.EqualTo ((double)transform.xy).Within (0.000001), "xy");
			Assert.That ((double)(-4), Is.EqualTo ((double)transform.yy).Within (0.000001), "yy");
			Assert.That ((double)5, Is.EqualTo ((double)transform.x0).Within (0.000001), "x0");
			Assert.That ((double)6, Is.EqualTo ((double)transform.y0).Within (0.000001), "y0");
		}

		[Test]
		public void StaticRotate ()
		{
			var transformM = CGAffineTransform.Rotate (new CGAffineTransform (1, 2, 3, 4, 5, 6), (nfloat) Math.PI);
			var transformN = CGAffineTransformRotate (new CGAffineTransform (1, 2, 3, 4, 5, 6), (nfloat) Math.PI);

			Assert.That ((double) (-1), Is.EqualTo ((double) transformM.xx).Within (0.000001), "xx");
			Assert.That ((double) (-2), Is.EqualTo ((double) transformM.yx).Within (0.000001), "yx");
			Assert.That ((double) (-3), Is.EqualTo ((double) transformM.xy).Within (0.000001), "xy");
			Assert.That ((double) (-4), Is.EqualTo ((double) transformM.yy).Within (0.000001), "yy");
			Assert.That ((double) 5, Is.EqualTo ((double) transformM.x0).Within (0.000001), "x0");
			Assert.That ((double) 6, Is.EqualTo ((double) transformM.y0).Within (0.000001), "y0");

			Assert.That ((double) transformN.xx, Is.EqualTo ((double) transformM.xx).Within (0.000001), "xx");
			Assert.That ((double) transformN.yx, Is.EqualTo ((double) transformM.yx).Within (0.000001), "yx");
			Assert.That ((double) transformN.xy, Is.EqualTo ((double) transformM.xy).Within (0.000001), "xy");
			Assert.That ((double) transformN.yy, Is.EqualTo ((double) transformM.yy).Within (0.000001), "yy");
			Assert.That ((double) 5, Is.EqualTo ((double) transformM.x0).Within (0.000001), "x0");
			Assert.That ((double) 6, Is.EqualTo ((double) transformM.y0).Within (0.000001), "y0");
		}

		[DllImport (global::ObjCRuntime.Constants.CoreGraphicsLibrary)]
		public extern static CGAffineTransform CGAffineTransformRotate (CGAffineTransform t, nfloat angle);

		[Test]
		public void IsIdentity ()
		{
			Assert.IsTrue (CGAffineTransform.MakeIdentity ().IsIdentity, "MakeIdentity");
			Assert.IsFalse (new CGAffineTransform (1, 2, 3, 4, 5, 6).IsIdentity, "123456");
		}

		[Test]
		public void TransformPoint ()
		{
			var transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			var point = transform.TransformPoint (new PointF (4, 5));

			Assert.AreEqual ((nfloat) 24, point.X, "X");
			Assert.AreEqual ((nfloat) 34, point.Y, "Y");
		}

		[Test]
		public void TransformRect ()
		{
			var transform = new CGAffineTransform (1, 2, 3, 4, 5, 6);
			var rect = transform.TransformRect (new RectangleF (4, 5, 6, 7));

			Assert.AreEqual ((nfloat) 24, rect.X, "X");
			Assert.AreEqual ((nfloat) 34, rect.Y, "Y");
			Assert.AreEqual ((nfloat) 27, rect.Width, "Width");
			Assert.AreEqual ((nfloat) 40, rect.Height, "Height");
		}

		[Test]
		public void Invert ()
		{
			var transform = new CGAffineTransform (1, 2, 3, 4, 5, 6).Invert ();

			Assert.AreEqual ((nfloat) (-2), transform.xx, "xx");
			Assert.AreEqual ((nfloat) 1, transform.yx, "yx");
			Assert.AreEqual ((nfloat) 1.5, transform.xy, "xy");
			Assert.AreEqual ((nfloat) (-0.5), transform.yy, "yy");
			Assert.AreEqual ((nfloat) 1.0, transform.x0, "x0");
			Assert.AreEqual ((nfloat) (-2.0), transform.y0, "y0");
		}
	}


}
