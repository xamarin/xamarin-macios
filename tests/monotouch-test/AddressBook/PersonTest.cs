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
using Foundation;
using UIKit;
using AddressBook;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AddressBook {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PersonTest {

		[SetUp]
		public void Setup ()
		{
			// The API here was introduced to Mac Catalyst later than for the other frameworks, so we have this additional check
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false);
			if (TestRuntime.CheckXcodeVersion (15, 0)) {
				Assert.Ignore ("The addressbook framework is deprecated in Xcode 15.0 and always returns null");
			}
		}

		[Test]
		public void UpdateAddressLine ()
		{
			TestRuntime.CheckAddressBookPermission ();

			NSError err;
			var ab = ABAddressBook.Create (out err);
			Assert.IsNotNull (ab, "#1");

			var people = ab.GetPeople ();
			if (people.Length < 1) {
				// TODO:
				return;
			}

			var p = people [0];

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
