//
// Unit tests for CNContact
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__TVOS__

using System;

using Contacts;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Contacts {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MutableContactTest {

		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void Properties ()
		{
			using (var contact = new CNMutableContact ()) {
				Assert.IsNull (contact.Birthday, "Birthday");
				Assert.AreEqual (0, contact.ContactRelations.Length, "ContactRelations");
				Assert.AreEqual (CNContactType.Person, contact.ContactType, "ContactType");
				Assert.AreEqual (0, contact.Dates.Length, "Dates");
				Assert.AreEqual (string.Empty, contact.DepartmentName, "DepartmentName");
				Assert.AreEqual (0, contact.EmailAddresses.Length, "EmailAddresses");
				Assert.AreEqual (string.Empty, contact.FamilyName, "FamilyName");
				Assert.AreEqual (string.Empty, contact.GivenName, "GivenName");
				Assert.AreNotEqual (string.Empty, contact.Identifier, "Identifier");
				Assert.IsNull (contact.ImageData, "ImageData");
				Assert.IsFalse (contact.ImageDataAvailable, "ImageDataAvailable");
				Assert.AreEqual (0, contact.InstantMessageAddresses.Length, "InstantMessageAddresses");
				Assert.AreEqual (string.Empty, contact.JobTitle, "JobTitle");
				Assert.AreEqual (string.Empty, contact.MiddleName, "MiddleName");
				Assert.AreEqual (string.Empty, contact.NamePrefix, "NamePrefix");
				Assert.AreEqual (string.Empty, contact.NameSuffix, "NameSuffix");
				Assert.AreEqual (string.Empty, contact.Nickname, "Nickname");
				Assert.IsNull (contact.NonGregorianBirthday, "NonGregorianBirthday");
				Assert.AreEqual (string.Empty, contact.Note, "Note");
				Assert.AreEqual (string.Empty, contact.OrganizationName, "OrganizationName");
				Assert.AreEqual (0, contact.PhoneNumbers.Length, "PhoneNumbers");
				Assert.AreEqual (string.Empty, contact.PhoneticFamilyName, "PhoneticFamilyName");
				Assert.AreEqual (string.Empty, contact.PhoneticGivenName, "PhoneticGivenName");
				Assert.AreEqual (string.Empty, contact.PhoneticMiddleName, "PhoneticMiddleName");
				Assert.AreEqual (0, contact.PostalAddresses.Length, "PostalAddresses");
				Assert.AreEqual (string.Empty, contact.PreviousFamilyName, "PreviousFamilyName");
				Assert.AreEqual (0, contact.SocialProfiles.Length, "SocialProfiles");
				Assert.IsNull (contact.ThumbnailImageData, "ThumbnailImageData");
				Assert.AreEqual (0, contact.UrlAddresses.Length, "UrlAddresses");

				contact.Birthday = new NSDateComponents () {
					Year = 1980
				};
				Assert.AreEqual ((nint) 1980, contact.Birthday.Year, "Birthday");

				contact.ContactRelations = new CNLabeledValue<CNContactRelation> [] {
					new CNLabeledValue<CNContactRelation> ("label", new CNContactRelation ("relation"))
				};
				Assert.AreEqual (1, contact.ContactRelations.Length, "ContactRelations");

				contact.ContactType = CNContactType.Organization;
				Assert.AreEqual (CNContactType.Organization, contact.ContactType, "ContactType");

				contact.Dates = new CNLabeledValue<NSDateComponents> [] {
					new CNLabeledValue<NSDateComponents> ("label", new NSDateComponents () {
						Month = 6
					})
				};
				Assert.AreEqual (1, contact.Dates.Length, "Dates");

				contact.DepartmentName = "department";
				Assert.AreEqual ("department", contact.DepartmentName, "DepartmentName");

				contact.EmailAddresses = new CNLabeledValue<NSString> [] {
					new CNLabeledValue<NSString> ("label", (NSString) "foo@bar.com")
				};
				Assert.AreEqual (1, contact.EmailAddresses.Length, "EmailAddresses");

				contact.FamilyName = "familyName";
				Assert.AreEqual ("familyName", contact.FamilyName, "FamilyName");

				contact.GivenName = "givenName";
				Assert.AreEqual ("givenName", contact.GivenName, "GivenName");

				Assert.AreNotEqual (string.Empty, contact.Identifier, "Identifier");

				contact.ImageData = new NSData ();
				Assert.IsNotNull (contact.ImageData, "ImageData-2");
				// iOS 10 (beta 1) fixed this bug (if not null then it's available)
				var avail = TestRuntime.CheckXcodeVersion (8, 0);
				Assert.That (contact.ImageDataAvailable, Is.EqualTo (avail), "ImageDataAvailable-2");

				contact.InstantMessageAddresses = new CNLabeledValue<CNInstantMessageAddress> [] {
					new CNLabeledValue<CNInstantMessageAddress> ("label", new CNInstantMessageAddress ("user", "service")),
				};
				Assert.AreEqual (1, contact.InstantMessageAddresses.Length, "InstantMessageAddresses");

				contact.JobTitle = "title";
				Assert.AreEqual ("title", contact.JobTitle, "JobTitle");

				contact.MiddleName = "middleName";
				Assert.AreEqual ("middleName", contact.MiddleName, "MiddleName");

				contact.NamePrefix = "namePrefix";
				Assert.AreEqual ("namePrefix", contact.NamePrefix, "NamePrefix");

				contact.NameSuffix = "nameSuffix";
				Assert.AreEqual ("nameSuffix", contact.NameSuffix, "NameSuffix");

				contact.Nickname = "nickname";
				Assert.AreEqual ("nickname", contact.Nickname, "Nickname");

				contact.NonGregorianBirthday = new NSDateComponents () {
					Year = 2099,
				};
				Assert.AreEqual ((nint) 2099, contact.NonGregorianBirthday.Year, "NonGregorianBirthday");

				contact.Note = "note";
				Assert.AreEqual ("note", contact.Note, "Note");

				contact.OrganizationName = "organizationName";
				Assert.AreEqual ("organizationName", contact.OrganizationName, "OrganizationName");

				contact.PhoneNumbers = new CNLabeledValue<CNPhoneNumber> [] {
					new CNLabeledValue<CNPhoneNumber> ("label", new CNPhoneNumber ("123-345-456"))
				};
				Assert.AreEqual (1, contact.PhoneNumbers.Length, "PhoneNumbers");

				contact.PhoneticFamilyName = "phoneticFamilyName";
				Assert.AreEqual ("phoneticFamilyName", contact.PhoneticFamilyName, "PhoneticFamilyName");

				contact.PhoneticGivenName = "phoneticGivenName";
				Assert.AreEqual ("phoneticGivenName", contact.PhoneticGivenName, "PhoneticGivenName");

				contact.PhoneticMiddleName = "phoneticMiddleName";
				Assert.AreEqual ("phoneticMiddleName", contact.PhoneticMiddleName, "PhoneticMiddleName");

				contact.PostalAddresses = new CNLabeledValue<CNPostalAddress> [] {
					new CNLabeledValue<CNPostalAddress> ("label", new CNMutablePostalAddress ()
						{
							Street = "my Street",
						})
				};
				Assert.AreEqual (1, contact.PostalAddresses.Length, "PostalAddresses");

				contact.PreviousFamilyName = "previousFamilyName";
				Assert.AreEqual ("previousFamilyName", contact.PreviousFamilyName, "PreviousFamilyName");

				contact.SocialProfiles = new CNLabeledValue<CNSocialProfile> [] {
					new CNLabeledValue<CNSocialProfile> ("label", new CNSocialProfile ("url", "username", "useridentifier", "service"))
				};
				Assert.AreEqual (1, contact.SocialProfiles.Length, "SocialProfiles");

				contact.UrlAddresses = new CNLabeledValue<NSString> [] {
					new CNLabeledValue<NSString> ("label", (NSString) "url@address.com")
				};
				Assert.AreEqual (1, contact.UrlAddresses.Length, "UrlAddresses");
			}
		}
	}
}

#endif // !__TVOS__
