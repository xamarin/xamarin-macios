// Unit tests for CVPixelBufferAttributes
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using CoreFoundation;
using Foundation;
using CoreVideo;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelBufferAttributesTest {

		[Test]
		public void Defaults ()
		{
			var options = new CVPixelBufferAttributes ();
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 0), "Count");
			Assert.Null (options.MemoryAllocator, "MemoryAllocator");
		}

		[Test]
		public void MemoryAllocator ()
		{
			var options = new CVPixelBufferAttributes () {
				MemoryAllocator = CFAllocator.MallocZone
			};
			Assert.That (options.Dictionary.Count, Is.EqualTo ((nuint) 1), "Count");
			Assert.That (options.MemoryAllocator.Handle, Is.EqualTo (CFAllocator.MallocZone.Handle), "MemoryAllocator");
		}
	}
}

#endif // !__WATCHOS__
