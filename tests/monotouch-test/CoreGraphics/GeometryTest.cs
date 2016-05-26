//
// Unit tests for CGGeometry (and related)
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using CoreGraphics;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
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
	public class GeometryTest {

		static public readonly IntPtr Handle = Dlfcn.dlopen (Constants.CoreGraphicsLibrary, 0);

		public static RectangleF GetRect (string symbol)
		{
			var indirect = Dlfcn.dlsym (Handle, symbol);
			if (indirect == IntPtr.Zero)
				return RectangleF.Empty;
			unsafe {
				nfloat *ptr = (nfloat *) indirect;
				return new RectangleF (ptr [0], ptr [1], ptr [2], ptr [3]);
			}
		}

		[Test]
		public void Infinite ()
		{
			var r = GetRect ("CGRectInfinite");
			Assert.False (r.IsEmpty, "IsEmpty");
			Assert.False (r.IsNull (), "IsNull");
			Assert.True (r.IsInfinite (), "IsInfinite");
		}

		[Test]
		public void Null ()
		{
			var r = GetRect ("CGRectNull");
			Assert.True (r.IsEmpty, "IsEmpty");
			Assert.True (r.IsNull (), "IsNull");
			Assert.False (r.IsInfinite (), "IsInfinite");
		}

		[Test]
		public void Zero ()
		{
			var r = GetRect ("CGRectZero");
			Assert.True (r.IsEmpty, "IsEmpty");
			Assert.False (r.IsNull (), "IsNull");
			Assert.False (r.IsInfinite (), "IsInfinite");
		}
	}
}