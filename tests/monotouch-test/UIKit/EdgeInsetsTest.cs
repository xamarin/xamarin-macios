//
// Unit tests for UIEdgeInsets
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class EdgeInsetsTest {

		[Test]
		public void FromString_Null ()
		{
			var e = UIEdgeInsets.FromString (null);
			Assert.That (e, Is.EqualTo (UIEdgeInsets.Zero), "roundtrip");
		}

		[Test]
		public void ToFromString_Zero ()
		{
			string s = UIEdgeInsets.Zero.ToString ();
			var e = UIEdgeInsets.FromString (s);
			Assert.That (e, Is.EqualTo (UIEdgeInsets.Zero), "roundtrip");
		}

		[Test]
		public void InsetRect_Zero ()
		{
			var r = UIEdgeInsets.Zero.InsetRect (CGRect.Empty);
			Assert.That (r, Is.EqualTo (CGRect.Empty), "InsetRect");
		}

		[Test]
		public void InsetRect ()
		{
			var i = new UIEdgeInsets (10, 20, 30, 40);
			var r = new CGRect (1, 2, 3, 4);
			r = i.InsetRect (r);
			Assert.That (r.X, Is.EqualTo ((nfloat) 21f), "X");
			Assert.That (r.Y, Is.EqualTo ((nfloat) 12f), "Y");
			Assert.That (r.Width, Is.EqualTo ((nfloat) (-57f)), "Width");
			Assert.That (r.Height, Is.EqualTo ((nfloat) (-36f)), "Height");

			Assert.False (i.Equals (UIEdgeInsets.Zero), "Equals(UIEdgeInsets)");
			Assert.False (UIEdgeInsets.Zero.Equals ((object) i), "Equals(object)");
		}

		[Test]
		public void Operators ()
		{
			var i1 = new UIEdgeInsets (10, 20, 30, 40);
			var i2 = new UIEdgeInsets (10, 10, 10, 10);

#pragma warning disable CS1718 // warning CS1718: Comparison made to same variable; did you mean to compare something else?
			Assert.True (i1 == i1, "i1 == i1");
			Assert.True (i2 == i2, "i1 == i1");
			Assert.True (i1 != i2, "i1 != i2");
			Assert.True (i2 != i1, "i2 != i1");
#pragma warning restore
		}
	}
}
#endif
