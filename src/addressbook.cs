
namespace AddressBook {

	[NoMac, NoTV]
	[Partial]
	interface ABAddressBook {
#if !XAMCORE_5_0
		[Internal]
		[Field ("ABAddressBookErrorDomain")]
		NSString _ErrorDomain { get; }
#endif
#if XAMCORE_5_0
		/// <summary>Identifies the error domain under which Address Book errors are grouped.</summary>
		/// <remarks>
		///   <para>
		///     When an <see cref="CoreFoundation.CFException" /> is thrown from an <see cref="ABAddressBook" /> method,
		///     its <see cref="CoreFoundation.CFException.Domain" /> is equal to this value.
		///   </para>
		/// </remarks>
		[Field ("ABAddressBookErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

	[NoMac, NoTV]
	[Internal]
	[Static, Partial]
	interface ABGroupProperty {
		[Field ("kABGroupNameProperty")]
		int Name { get; }
	}

	[NoMac, NoTV]
	[Internal]
	[Static, Partial]
	interface ABPersonPropertyId {
		[Field ("kABPersonAddressProperty")]
		int Address { get; }

		[Field ("kABPersonBirthdayProperty")]
		int Birthday { get; }

		[Field ("kABPersonCreationDateProperty")]
		int CreationDate { get; }

		[Field ("kABPersonDateProperty")]
		int Date { get; }

		[Field ("kABPersonDepartmentProperty")]
		int Department { get; }

		[Field ("kABPersonEmailProperty")]
		int Email { get; }

		[Field ("kABPersonFirstNameProperty")]
		int FirstName { get; }

		[Field ("kABPersonFirstNamePhoneticProperty")]
		int FirstNamePhonetic { get; }

		[Field ("kABPersonInstantMessageProperty")]
		int InstantMessage { get; }

		[Field ("kABPersonJobTitleProperty")]
		int JobTitle { get; }

		[Field ("kABPersonKindProperty")]
		int Kind { get; }

		[Field ("kABPersonLastNameProperty")]
		int LastName { get; }

		[Field ("kABPersonLastNamePhoneticProperty")]
		int LastNamePhonetic { get; }

		[Field ("kABPersonMiddleNameProperty")]
		int MiddleName { get; }

		[Field ("kABPersonMiddleNamePhoneticProperty")]
		int MiddleNamePhonetic { get; }

		[Field ("kABPersonModificationDateProperty")]
		int ModificationDate { get; }

		[Field ("kABPersonNicknameProperty")]
		int Nickname { get; }

		[Field ("kABPersonNoteProperty")]
		int Note { get; }

		[Field ("kABPersonOrganizationProperty")]
		int Organization { get; }

		[Field ("kABPersonPhoneProperty")]
		int Phone { get; }

		[Field ("kABPersonPrefixProperty")]
		int Prefix { get; }

		[Field ("kABPersonRelatedNamesProperty")]
		int RelatedNames { get; }

		[Field ("kABPersonSuffixProperty")]
		int Suffix { get; }

		[Field ("kABPersonURLProperty")]
		int Url { get; }

		[Field ("kABPersonSocialProfileProperty")]
		int SocialProfile { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonAddressKey {
		/// <summary>Gets the key for the city component of an address.</summary>
		[Field ("kABPersonAddressCityKey")]
		[NullAllowed]
		NSString City { get; }

		/// <summary>Gets the key for the country/region component of an address.</summary>
		[Field ("kABPersonAddressCountryKey")]
		[NullAllowed]
		NSString Country { get; }

		/// <summary>Gets the key for the country/region code component of an address.</summary>
		[Field ("kABPersonAddressCountryCodeKey")]
		[NullAllowed]
		NSString CountryCode { get; }

		/// <summary>Gets the key for the state or province component of an address.</summary>
		[Field ("kABPersonAddressStateKey")]
		[NullAllowed]
		NSString State { get; }

		/// <summary>Gets the key for the street component of an address.</summary>
		[Field ("kABPersonAddressStreetKey")]
		[NullAllowed]
		NSString Street { get; }

		/// <summary>Gets the key for the postal-code component of an address.</summary>
		[Field ("kABPersonAddressZIPKey")]
		[NullAllowed]
		NSString Zip { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonDateLabel {
		/// <summary>Gets the label used for anniversary dates.</summary>
		[Field ("kABPersonAnniversaryLabel")]
		[NullAllowed]
		NSString Anniversary { get; }
	}

	[NoMac, NoTV]
	[Internal]
	[Static, Partial]
	interface ABPersonKindId {
		[Field ("kABPersonKindOrganization")]
		NSNumber Organization { get; }

		[Field ("kABPersonKindPerson")]
		NSNumber Person { get; }
	}

	[NoMac, NoTV]
	[Internal]
	[Static, Partial]
	interface ABPersonSocialProfile {
		[Field ("kABPersonSocialProfileURLKey")]
		NSString URLKey { get; }

		[Field ("kABPersonSocialProfileServiceKey")]
		NSString ServiceKey { get; }

		[Field ("kABPersonSocialProfileUsernameKey")]
		NSString UsernameKey { get; }

		[Field ("kABPersonSocialProfileUserIdentifierKey")]
		NSString UserIdentifierKey { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonSocialProfileService {
#if !XAMCORE_5_0
		[Internal]
		[Field ("kABPersonSocialProfileServiceTwitter")]
		NSString _Twitter { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceGameCenter")]
		NSString _GameCenter { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceFacebook")]
		NSString _Facebook { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceMyspace")]
		NSString _Myspace { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceLinkedIn")]
		NSString _LinkedIn { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceFlickr")]
		NSString _Flickr { get; }

		[Internal]
		[Field ("kABPersonSocialProfileServiceSinaWeibo")]
		NSString _SinaWeibo { get; }
#endif
#if XAMCORE_5_0
		/// <summary>Identifies the Twitter social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceTwitter")]
		[NullAllowed]
		NSString Twitter { get; }

		/// <summary>Identifies the Game Center social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceGameCenter")]
		[NullAllowed]
		NSString GameCenter { get; }

		/// <summary>Identifies the Facebook social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceFacebook")]
		[NullAllowed]
		NSString Facebook { get; }

		/// <summary>Identifies the Myspace social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceMyspace")]
		[NullAllowed]
		NSString Myspace { get; }

		/// <summary>Identifies the LinkedIn social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceLinkedIn")]
		[NullAllowed]
		NSString LinkedIn { get; }

		/// <summary>Identifies the Flickr social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceFlickr")]
		[NullAllowed]
		NSString Flickr { get; }

		/// <summary>Identifies the Sina Weibo social-profile service.</summary>
		[Field ("kABPersonSocialProfileServiceSinaWeibo")]
		[NullAllowed]
		NSString SinaWeibo { get; }
#endif
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonPhoneLabel {
		/// <summary>Gets the label used for a home fax number.</summary>
		[Field ("kABPersonPhoneHomeFAXLabel")]
		[NullAllowed]
		NSString HomeFax { get; }

		/// <summary>Gets the label used for an iPhone number.</summary>
		[Field ("kABPersonPhoneIPhoneLabel")]
		[NullAllowed]
		NSString iPhone { get; }

		/// <summary>Gets the label used for a main phone number.</summary>
		[Field ("kABPersonPhoneMainLabel")]
		[NullAllowed]
		NSString Main { get; }

		/// <summary>Gets the label used for a mobile phone number.</summary>
		[Field ("kABPersonPhoneMobileLabel")]
		[NullAllowed]
		NSString Mobile { get; }

		/// <summary>Gets the label used for a pager number.</summary>
		[Field ("kABPersonPhonePagerLabel")]
		[NullAllowed]
		NSString Pager { get; }

		/// <summary>Gets the label used for a work fax number.</summary>
		[Field ("kABPersonPhoneWorkFAXLabel")]
		[NullAllowed]
		NSString WorkFax { get; }

		/// <summary>Gets the label used for another fax number.</summary>
		[Field ("kABPersonPhoneOtherFAXLabel")]
		[NullAllowed]
		NSString OtherFax { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonInstantMessageService {
		/// <summary>Gets the AIM instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceAIM")]
		[NullAllowed]
		NSString Aim { get; }

		/// <summary>Gets the ICQ instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceICQ")]
		[NullAllowed]
		NSString Icq { get; }

		/// <summary>Gets the Jabber instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceJabber")]
		[NullAllowed]
		NSString Jabber { get; }

		/// <summary>Gets the MSN instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceMSN")]
		[NullAllowed]
		NSString Msn { get; }

		/// <summary>Gets the Yahoo instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceYahoo")]
		[NullAllowed]
		NSString Yahoo { get; }

		/// <summary>Gets the QQ instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceQQ")]
		[NullAllowed]
		NSString QQ { get; }

		/// <summary>Gets the Google Talk instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceGoogleTalk")]
		[NullAllowed]
		NSString GoogleTalk { get; }

		/// <summary>Gets the Skype instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceSkype")]
		[NullAllowed]
		NSString Skype { get; }

		/// <summary>Gets the Facebook instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceFacebook")]
		[NullAllowed]
		NSString Facebook { get; }

		/// <summary>Gets the Gadu-Gadu instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceGaduGadu")]
		[NullAllowed]
		NSString GaduGadu { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonInstantMessageKey {
		/// <summary>Gets the key for an instant-messaging service identifier.</summary>
		[Field ("kABPersonInstantMessageServiceKey")]
		[NullAllowed]
		NSString Service { get; }

		/// <summary>Gets the key for an instant-messaging user name.</summary>
		[Field ("kABPersonInstantMessageUsernameKey")]
		[NullAllowed]
		NSString Username { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonUrlLabel {
		/// <summary>Gets the label used for a home-page URL.</summary>
		[Field ("kABPersonHomePageLabel")]
		[NullAllowed]
		NSString HomePage { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABPersonRelatedNamesLabel {
		/// <summary>Gets the label used for an assistant.</summary>
		[Field ("kABPersonAssistantLabel")]
		[NullAllowed]
		NSString Assistant { get; }

		/// <summary>Gets the label used for a brother.</summary>
		[Field ("kABPersonBrotherLabel")]
		[NullAllowed]
		NSString Brother { get; }

		/// <summary>Gets the label used for a child.</summary>
		[Field ("kABPersonChildLabel")]
		[NullAllowed]
		NSString Child { get; }

		/// <summary>Gets the label used for a father.</summary>
		[Field ("kABPersonFatherLabel")]
		[NullAllowed]
		NSString Father { get; }

		/// <summary>Gets the label used for a friend.</summary>
		[Field ("kABPersonFriendLabel")]
		[NullAllowed]
		NSString Friend { get; }

		/// <summary>Gets the label used for a manager.</summary>
		[Field ("kABPersonManagerLabel")]
		[NullAllowed]
		NSString Manager { get; }

		/// <summary>Gets the label used for a mother.</summary>
		[Field ("kABPersonMotherLabel")]
		[NullAllowed]
		NSString Mother { get; }

		/// <summary>Gets the label used for a parent.</summary>
		[Field ("kABPersonParentLabel")]
		[NullAllowed]
		NSString Parent { get; }

		/// <summary>Gets the label used for a partner.</summary>
		[Field ("kABPersonPartnerLabel")]
		[NullAllowed]
		NSString Partner { get; }

		/// <summary>Gets the label used for a sister.</summary>
		[Field ("kABPersonSisterLabel")]
		[NullAllowed]
		NSString Sister { get; }

		/// <summary>Gets the label used for a spouse.</summary>
		[Field ("kABPersonSpouseLabel")]
		[NullAllowed]
		NSString Spouse { get; }
	}

	[NoMac, NoTV]
	[Static, Partial]
	interface ABLabel {
		/// <summary>Gets the generic home label.</summary>
		[Field ("kABHomeLabel")]
		[NullAllowed]
		NSString Home { get; }

		/// <summary>Gets the generic other label.</summary>
		[Field ("kABOtherLabel")]
		[NullAllowed]
		NSString Other { get; }

		/// <summary>Gets the generic work label.</summary>
		[Field ("kABWorkLabel")]
		[NullAllowed]
		NSString Work { get; }
	}

	[NoMac, NoTV]
	[Internal]
	[Static, Partial]
	interface ABSourcePropertyId {
		[Field ("kABSourceNameProperty")]
		int Name { get; }

		[Field ("kABSourceTypeProperty")]
		int Type { get; }
	}
}
