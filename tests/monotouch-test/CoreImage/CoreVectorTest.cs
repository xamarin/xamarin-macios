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
using System.Runtime.InteropServices;

using Foundation;
using CoreImage;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreImage {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CIVectorTest {
		
		[Test]
		public void CtorFloatArray ()
		{
			// Make sure these do not crash
			Assert.That (new CIVector (new nfloat [0]).Count, Is.EqualTo ((nint) 0), "0");
#if NO_NFLOAT_OPERATORS
			Assert.That (new CIVector (new nfloat [] {new NFloat (1)}).Count, Is.EqualTo ((nint) 1), "1");
			Assert.That (new CIVector (new nfloat [] {new NFloat (1),new NFloat (2)}).Count, Is.EqualTo ((nint) 2), "2'");
			Assert.That (new CIVector (new nfloat [] {new NFloat (1),new NFloat (2),new NFloat (3)}).Count, Is.EqualTo ((nint) 3), "3");
			Assert.That (new CIVector (new nfloat [] {new NFloat (1),new NFloat (2),new NFloat (3),new NFloat (4)}).Count, Is.EqualTo ((nint) 4), "4");
#else
			Assert.That (new CIVector (new nfloat [] {1}).Count, Is.EqualTo ((nint) 1), "1");
			Assert.That (new CIVector (new nfloat [] {1,2}).Count, Is.EqualTo ((nint) 2), "2'");
			Assert.That (new CIVector (new nfloat [] {1,2,3}).Count, Is.EqualTo ((nint) 3), "3");
			Assert.That (new CIVector (new nfloat [] {1,2,3,4}).Count, Is.EqualTo ((nint) 4), "4");
#endif

			Assert.Throws<ArgumentNullException> (() => new CIVector ((nfloat[]) null), "null");
		}

		[Test]
		public void CtorFloatArrayCount ()
		{
			Assert.That (new CIVector (new nfloat [0], 0).Count, Is.EqualTo ((nint) 0), "0");
#if NO_NFLOAT_OPERATORS
			Assert.That (new CIVector (new nfloat [] {new NFloat (1)}, 1).Count, Is.EqualTo ((nint) 1), "1");
			Assert.That (new CIVector (new nfloat [] {new NFloat (1),new NFloat (2)}, 2).Count, Is.EqualTo ((nint) 2), "2'");
			Assert.That (new CIVector (new nfloat [] {new NFloat (1),new NFloat (2),new NFloat (3),new NFloat (4)}, 2).Count, Is.EqualTo ((nint) 2), "4/2");
#else
			Assert.That (new CIVector (new nfloat [] {1}, 1).Count, Is.EqualTo ((nint) 1), "1");
			Assert.That (new CIVector (new nfloat [] {1,2}, 2).Count, Is.EqualTo ((nint) 2), "2'");
			Assert.That (new CIVector (new nfloat [] {1,2,3,4}, 2).Count, Is.EqualTo ((nint) 2), "4/2");
#endif

			Assert.Throws<ArgumentNullException> (() => new CIVector ((nfloat[]) null, 0), "null");
#if NO_NFLOAT_OPERATORS
			Assert.Throws<ArgumentOutOfRangeException> (() => new CIVector (new nfloat [] {new NFloat (1)}, 2), "out-of-range");
#else
			Assert.Throws<ArgumentOutOfRangeException> (() => new CIVector (new nfloat [] {1}, 2), "out-of-range");
#endif
		}

		[Test]
		public void CtorInts ()
		{
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
				Assert.That (v.Count, Is.EqualTo ((nint) 0), "Count/empty");
			}
		}
	}
}

#endif // !__WATCHOS__
