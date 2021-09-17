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
using CoreAnimation;
using Foundation;
using SceneKit;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.SceneKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class StructTest {

		[Test]
		public void Matrix ()
		{
			var id = SCNMatrix4.Identity;
			id.M11 = 2;
			Assert.That (SCNMatrix4.Identity, Is.Not.EqualTo (id), "Identity");
		}

		[Test]
		public void Quaternion ()
		{
			var id = SCNQuaternion.Identity;
			id.W = 2;
			Assert.That (SCNQuaternion.Identity, Is.Not.EqualTo (id), "Identity");
		}

		[Test]
		public void Vector ()
		{
			var v = new SCNVector4 ();
			var u = SCNVector4.UnitX;
			u.X = 2;
			Assert.That (SCNVector4.UnitX, Is.Not.EqualTo (u), "UnitX");

			u = SCNVector4.UnitY;
			u.Y = 2;
			Assert.That (SCNVector4.UnitY, Is.Not.EqualTo (u), "UnitY");

			u = SCNVector4.UnitZ;
			u.Z = 2;
			Assert.That (SCNVector4.UnitZ, Is.Not.EqualTo (u), "UnitZ");

			u = SCNVector4.UnitW;
			u.W = 2;
			Assert.That (SCNVector4.UnitW, Is.Not.EqualTo (u), "UnitW");

			u = SCNVector4.Zero;
			u.W = 2;
			Assert.That (SCNVector4.Zero, Is.Not.EqualTo (u), "Zero");
		}
	}
}

#endif // !__WATCHOS__
