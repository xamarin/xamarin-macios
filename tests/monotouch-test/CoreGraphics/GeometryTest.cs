//
// Unit tests for CGGeometry (and related)
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GeometryTest {

		static public readonly IntPtr Handle = Dlfcn.dlopen (Constants.CoreGraphicsLibrary, 0);

		public static CGRect GetRect (string symbol)
		{
			var indirect = Dlfcn.dlsym (Handle, symbol);
			if (indirect == IntPtr.Zero)
				return CGRect.Empty;
			unsafe {
				nfloat* ptr = (nfloat*) indirect;
				return new CGRect (ptr [0], ptr [1], ptr [2], ptr [3]);
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
