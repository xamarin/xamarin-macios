//
// Unit tests for CNContactStore
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using Contacts;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Contacts {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ContactStoreTest {

		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

#if !MONOMAC // Unlike XI, XM does not have infrastructure yet to disable prompts
		[Test]
#endif
		public void GetUnifiedContacts ()
		{
			string identifier = null;

			TestRuntime.CheckContactsPermission ();

			var fetchKeys = new [] { CNContactKey.Identifier, CNContactKey.GivenName, CNContactKey.FamilyName };
			NSError error;
			using (var predicate = CNContact.GetPredicateForContacts ("Appleseed"))
			using (var store = new CNContactStore ()) {
				var contacts = store.GetUnifiedContacts (predicate, fetchKeys, out error);
				// we can't be sure what's on devices, so check there's no error is the only thing we do
				// but it's in the default simulator build (but not the watchOS simulator)
#if !MONOMAC && !__WATCHOS__ && !__MACCATALYST__
				if ((error is null) && (Runtime.Arch == Arch.SIMULATOR)) {
					Assert.That (contacts.Length, Is.EqualTo (1), "Length");
					identifier = contacts [0].Identifier;
				}
#endif
			}

			// if we can't find the previous contact then we don't have an identifier for the GetUnifiedContact API
			// and we can't hardcode one as each simulator instance has a different identifier...
			if (identifier is null)
				return;

			using (var store = new CNContactStore ()) {
				var contact = store.GetUnifiedContact (identifier, fetchKeys, out error);
				// it's in the default simulator build
				if (TestRuntime.IsSimulatorOrDesktop) {
					// it fails on some bots (watchOS 4.2 on jenkins) so we cannot assume it always work
					if (error is not null)
						return;
					Assert.NotNull (contact, "contact");
					Assert.False (contact.AreKeysAvailable (CNContactOptions.OrganizationName | CNContactOptions.Note), "AreKeysAvailable-1");
					Assert.True (contact.AreKeysAvailable (CNContactOptions.None), "AreKeysAvailable-2");
					Assert.True (contact.AreKeysAvailable (fetchKeys), "AreKeysAvailable-3");
				}
			}
		}
	}
}

#endif // !__TVOS__
