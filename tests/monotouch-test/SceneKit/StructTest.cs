//
// Unit tests for SCN* structures
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreAnimation;
using Foundation;
using SceneKit;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.SceneKit;
using MonoTouch.UIKit;
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

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StructTest {

		[Test]
		public void Matrix ()
		{
			var id = SCNMatrix4.Identity;
#if !XAMCORE_2_0
			var m = new SCNMatrix4 ();
			// that was possible in classic - but will now trigger a CS0198
			SCNMatrix4.Identity = m;
#else
			id.M11 = 2;
#endif
			Assert.That (SCNMatrix4.Identity, Is.Not.EqualTo (id), "Identity");
		}

		[Test]
		public void Quaternion ()
		{
			var id = SCNQuaternion.Identity;
#if !XAMCORE_2_0
			var q = new SCNQuaternion ();
			// that was possible in classic - but will now trigger a CS0198
			SCNQuaternion.Identity = q;
#else
			id.W = 2;
#endif
			Assert.That (SCNQuaternion.Identity, Is.Not.EqualTo (id), "Identity");
		}

		[Test]
		public void Vector ()
		{
			var v = new SCNVector4 ();
			var u = SCNVector4.UnitX;
#if !XAMCORE_2_0
			// that was possible in classic - but will now trigger a CS0198
			SCNVector4.UnitX = v;
#else
			u.X = 2;
#endif
			Assert.That (SCNVector4.UnitX, Is.Not.EqualTo (u), "UnitX");

			u = SCNVector4.UnitY;
#if !XAMCORE_2_0
			// that was possible in classic - but will now trigger a CS0198
			SCNVector4.UnitY = v;
#else
			u.Y = 2;
#endif
			Assert.That (SCNVector4.UnitY, Is.Not.EqualTo (u), "UnitY");

			u = SCNVector4.UnitZ;
#if !XAMCORE_2_0
			// that was possible in classic - but will now trigger a CS0198
			SCNVector4.UnitZ = v;
#else
			u.Z = 2;
#endif
			Assert.That (SCNVector4.UnitZ, Is.Not.EqualTo (u), "UnitZ");

			u = SCNVector4.UnitW;
#if !XAMCORE_2_0
			// that was possible in classic - but will now trigger a CS0198
			SCNVector4.UnitW = v;
#else
			u.W = 2;
#endif
			Assert.That (SCNVector4.UnitW, Is.Not.EqualTo (u), "UnitW");

			u = SCNVector4.Zero;
#if !XAMCORE_2_0
			// that was possible in classic - but will now trigger a CS0198
			SCNVector4.Zero = v;
			Assert.That (SCNVector4.Zero, Is.EqualTo (u), "Zero");
#else
			u.W = 2;
			Assert.That (SCNVector4.Zero, Is.Not.EqualTo (u), "Zero");
#endif
		}
	}
}

#endif // !__WATCHOS__
