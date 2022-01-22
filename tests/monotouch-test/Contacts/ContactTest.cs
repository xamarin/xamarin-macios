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
	public class ContactTest {

		[SetUp]
		public void MinimumSdkCheck ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);
		}

		[Test]
		public void DescriptorForAllComparatorKeys ()
		{
			var keys = CNContact.GetDescriptorForAllComparatorKeys ();
			// while most input for ICNKeyDescriptor are done with NSString
			// the output is opaque and an internal type
			// note: this is not very robust - but I want to know if this changes during the next betas
			Assert.True (keys.Description.StartsWith ("<CNAggregateKeyDescriptor:", StringComparison.Ordinal), "type");
			Assert.True (keys.Description.Contains (" kind=Formatter "), "kind");
			Assert.True (keys.Description.Contains (" style: 100"), "style"); // 1002 before iOS 10, 1003 after
		}

		[Test]
		public void Ctor ()
		{
			using (var contact = new CNContact ()) {
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
			}
		}
	}
}

#endif // !__TVOS__
