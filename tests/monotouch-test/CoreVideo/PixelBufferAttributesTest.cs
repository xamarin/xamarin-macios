// Unit tests for CVPixelBufferAttributes
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using CoreVideo;
using ObjCRuntime;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreVideo;
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

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelBufferAttributesTest {

		[Test]
		public void Defaults ()
		{
			var options = new CVPixelBufferAttributes ();
			Assert.That (options.Dictionary.Count, Is.EqualTo (0), "Count");
			Assert.Null (options.MemoryAllocator, "MemoryAllocator");
		}

		[Test]
		public void MemoryAllocator ()
		{
			var options = new CVPixelBufferAttributes () {
				MemoryAllocator = CFAllocator.MallocZone
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo (1), "Count");
			Assert.That (options.MemoryAllocator.Handle, Is.EqualTo (CFAllocator.MallocZone.Handle), "MemoryAllocator");
		}
	}
}

#endif // !__WATCHOS__
