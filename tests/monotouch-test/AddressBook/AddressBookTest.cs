//
// Unit tests for ABSource
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using AddressBook;
using ObjCRuntime;
#else
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.AddressBook {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AddressBookTest {
		
		// very general ABSource related tests (works on both simulator and devices)
		
		[Test]
		public void GetAllSources ()
		{
			TestRuntime.CheckAddressBookPermission ();
			ABAddressBook ab = new ABAddressBook ();
			var sources = ab.GetAllSources ();
			int value = Runtime.Arch == Arch.DEVICE || TestRuntime.CheckiOSSystemVersion (7, 0, throwIfOtherPlatform: false) ? 0 : 1;
			Assert.That (sources.Length, Is.GreaterThanOrEqualTo (value), "GetAllSources");
		}
		
		[Test]
		public void GetDefaultSource ()
		{
			TestRuntime.CheckAddressBookPermission ();
			ABAddressBook ab = new ABAddressBook ();
			Assert.NotNull (ab.GetDefaultSource (), "GetDefaultSource");
		}

		[Test]
		public void GetSource ()
		{
			TestRuntime.CheckAddressBookPermission ();
			ABAddressBook ab = new ABAddressBook ();
			Assert.Null (ab.GetSource (-1), "-1");
			// GetSource(0) is not reliable across device/simulator and iOS versions
			Assert.Null (ab.GetSource (Int32.MaxValue), "MaxValue");
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
