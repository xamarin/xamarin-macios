//
// Unit tests for CNContactFetchRequest
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__
#if XAMCORE_2_0 // The Contacts framework is Unified only

using System;
#if XAMCORE_2_0
using Contacts;
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Contacts;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Contacts {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ContactFetchRequestTest {

		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void Ctor_ICNKeyDescriptorArray ()
		{
			var keys = new [] { CNContactVCardSerialization.GetDescriptorFromRequiredKeys () };
			using (var cfr = new CNContactFetchRequest (keys)) {
				Assert.That (keys [0].Description, Is.EqualTo (cfr.KeysToFetch.GetItem<NSObject> (0).Description), "KeysToFetch");
			}
		}

		[Test]
		public void Ctor_NSString ()
		{
			var keys = new [] { CNContactKey.GivenName };
			using (var cfr = new CNContactFetchRequest (keys)) {
				Assert.That (keys [0].Description, Is.EqualTo (cfr.KeysToFetch.GetItem<NSString> (0).Description), "KeysToFetch");
			}
		}

		[Test]
		public void Ctor_Mixed ()
		{
			var keys = new INativeObject [] { CNContactKey.GivenName, CNContactVCardSerialization.GetDescriptorFromRequiredKeys () };
			using (var cfr = new CNContactFetchRequest (keys)) {
				Assert.That (2, Is.EqualTo (cfr.KeysToFetch.Count), "KeysToFetch");
			}
		}
	}
}

#endif // XAMCORE_2_0
#endif // !__TVOS__
