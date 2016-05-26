//
// Unit tests for CIVector
//
// Authors:
//	Miguel de Icaza <miguel@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Foundation;
using CoreImage;
#else
using MonoTouch.CoreImage;
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

namespace MonoTouchFixtures.CoreImage {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CIVectorTest {
		
		[Test]
		public void Constructors ()
		{
			// Make sure these do not crash
			Assert.That (new CIVector (new nfloat [0]).Count, Is.EqualTo ((nint) 0));
			Assert.That (new CIVector (new nfloat [] {1}).Count, Is.EqualTo ((nint) 1));
			Assert.That (new CIVector (new nfloat [] {1,2}).Count, Is.EqualTo ((nint) 2));
			Assert.That (new CIVector (new nfloat [] {1,2,3}).Count, Is.EqualTo ((nint) 3));
			Assert.That (new CIVector (new nfloat [] {1,2,3,4}).Count, Is.EqualTo ((nint) 4));
			Assert.That (new CIVector (1).Count, Is.EqualTo ((nint) 1));
			Assert.That (new CIVector (1,2).Count, Is.EqualTo ((nint) 2));
			Assert.That (new CIVector (1,2,3).Count, Is.EqualTo ((nint) 3));
			Assert.That (new CIVector (1,2,3,4).Count, Is.EqualTo ((nint) 4));
		}

		[Test]
		public void FromValues ()
		{
			Assert.Throws<ArgumentNullException> (() => CIVector.FromValues (null), "null");
			using (var v = CIVector.FromValues (new nfloat [0])) {
				Assert.That (v.Count, Is.EqualTo (0), "Count/empty");
			}
		}
	}
}

#endif // !__WATCHOS__
