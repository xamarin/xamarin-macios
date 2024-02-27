//
// Unit tests for CMMemoryPool
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc All rights reserved.
//
using System;
using Foundation;
using CoreMedia;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMMemoryPoolTest {
		[Test]
		public void Ctor ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			using (var mp = new CMMemoryPool ()) {
				var allocator = mp.GetAllocator ();
				var ptr = allocator.Allocate (55);
				Assert.AreNotEqual (IntPtr.Zero, ptr);
				allocator.Deallocate (ptr);
			}
		}

		[Test]
		public void CtorAgeOutPeriod ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacOSX, 10, 8, throwIfOtherPlatform: false);

			using (var mp = new CMMemoryPool (TimeSpan.FromSeconds (40))) {
				var allocator = mp.GetAllocator ();
				var ptr = allocator.Allocate (2);
				Assert.AreNotEqual (IntPtr.Zero, ptr);
				allocator.Deallocate (ptr);
			}
		}
	}
}
