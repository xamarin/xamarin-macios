//
// Unit tests for NSMutableData
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableDataTest {

		[Test]
		public void FromCapacity ()
		{
			Assert.Throws<ArgumentOutOfRangeException> (delegate
			{
				NSMutableData.FromCapacity (-1);
			}, "negative");
			using (var empty = NSMutableData.FromCapacity (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}

		[Test]
		public void FromLength ()
		{
			Assert.Throws<ArgumentOutOfRangeException> (delegate
			{
				NSMutableData.FromLength (-1);
			}, "negative");
			using (var empty = NSMutableData.FromLength (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}

		[Test]
		public void Constructor ()
		{
			// the bound constructor is for capacity (not length)
			TestDelegate check_capacity = delegate
			{
				uint capacity = (uint) Int32.MaxValue + 2;
				Console.WriteLine ("Trying to allocate {0} bytes, this may cause malloc errors", capacity);
				new NSMutableData (capacity).Dispose ();
			};

			if (IntPtr.Size == 4) {
				Assert.Throws<ArgumentOutOfRangeException> (check_capacity, "negative");
			} else {
				// this can either fail with an Exception due to not enough memory to allocate 2GB (typical on device), or it can succeed (usually in the sim).
				try {
					check_capacity ();
					// It worked, that's fine.
				} catch (Exception ex) {
					// Verify that the exception is an OOM (i.e. native code failed to init the object).
					Assert.AreSame (typeof (Exception), ex.GetType (), "exception type");
					Assert.That (ex.Message, Does.StartWith ("Could not initialize an instance of the type 'Foundation.NSMutableData': the native 'initWithCapacity:' method returned nil."), "OOM");
				}
			}

			using (var empty = new NSMutableData (0)) {
				Assert.That (empty.Length, Is.EqualTo ((nuint) 0), "Length");
			}
		}
	}
}
