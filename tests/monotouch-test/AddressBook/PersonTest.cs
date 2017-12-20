//
// Unit tests for ABPerson
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
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
	public class PersonTest {
		
		[Test]
		public void UpdateAddressLine ()
		{
			TestRuntime.CheckAddressBookPermission ();
			if (!TestRuntime.CheckSystemAndSDKVersion (6,0))
				Assert.Inconclusive ("System.EntryPointNotFoundException : ABAddressBookCreateWithOptions before 6.0");

			NSError err;
			var ab = ABAddressBook.Create (out err);
			Assert.IsNotNull (ab, "#1");

			var people = ab.GetPeople ();
			if (people.Length < 1) {
				// TODO:
				return;
			}

			var p = people[0];

			var all = p.GetAllAddresses ();
			var mutable = all.ToMutableMultiValue ();
			if (mutable.Count < 1) {
				// TODO:
				return;
			}

			var multi = mutable [0];
			var addr = multi.Value;
			addr.Zip = "78972";
			multi.Value = addr;
			p.SetAddresses (mutable);
			
			Assert.IsTrue (ab.HasUnsavedChanges);
			ab.Save ();
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
