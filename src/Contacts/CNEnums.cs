//
// Contacts bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace Contacts {

	// NSInteger -> CNContact.h
	/// <summary>Enumerates whether a contact represents an individual or an organization.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactType : long {
		Person,
		Organization
	}

	// NSInteger -> CNContact.h
	/// <summary>Enumerates the manner in which contacts should be sorted.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactSortOrder : long {
		None,
		UserDefault,
		GivenName,
		FamilyName
	}

	// NSInteger -> CNContactFormatter.h
	/// <summary>Enumerates whether or not a contact name should be spelled out phonetically.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactFormatterStyle : long {
		FullName,
		PhoneticFullName
	}

	// NSInteger -> CNContactFormatter.h
	/// <summary>Enumerates how contacts should be sorted for display.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactDisplayNameOrder : long {
		UserDefault,
		GivenNameFirst,
		FamilyNameFirst
	}

	// NSInteger -> CNContactStore.h
	/// <summary>An enumeration whose only value (<see cref="F:Contacts.CNEntityType.Contacts" />) is used by some methods in <see cref="T:Contacts.CNContactStore" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNEntityType : long {
		Contacts
	}

	// NSInteger -> CNContactStore.h
	/// <summary>Enumerates the application's current authorization to access the <see cref="T:Contacts.CNContactStore" />.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
		[iOS (18, 0), NoMacCatalyst, NoMac]
		Limited,
	}

	// NSInteger -> CNContainer.h
	/// <summary>Enumerates known <see cref="T:Contacts.CNContainer" /> types.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContainerType : long {
		Unassigned = 0,
		Local,
		Exchange,
		CardDav
	}

	// NSInteger -> CNError.h
	/// <summary>Enumerates kinds of error encountered while working with contacts.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("CNErrorDomain")]
	public enum CNErrorCode : long {
		CommunicationError = 1,
		DataAccessError = 2,
		AuthorizationDenied = 100,
		NoAccessibleWritableContainers = 101,
		UnauthorizedKeys = 102,
		FeatureDisabledByUser = 103,
		FeatureNotAvailable = 104,
		RecordDoesNotExist = 200,
		InsertedRecordAlreadyExists = 201,
		ContainmentCycle = 202,
		ContainmentScope = 203,
		ParentRecordDoesNotExist = 204,
		RecordIdentifierInvalid = 205,
		RecordNotWritable = 206,
		ParentContainerNotWritable = 207,
		ValidationMultipleErrors = 300,
		ValidationTypeMismatch = 301,
		ValidationConfigurationError = 302,
		PredicateInvalid = 400,
		PolicyViolation = 500,
		ClientIdentifierInvalid = 600,
		ClientIdentifierDoesNotExist = 601,
		ClientIdentifierCollision = 602,
		ChangeHistoryExpired = 603,
		ChangeHistoryInvalidAnchor = 604,
		ChangeHistoryInvalidFetchRequest = 605,
		VCardMalformed = 700,
		VCardSummarizationError = 701,
	}

	// NSInteger -> CNPostalAddressFormatter.h
	/// <summary>Enumerates postal address formatter styles.</summary>
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNPostalAddressFormatterStyle : long {
		MailingAddress,
	}

	/// <summary>Flagging enumeration that specifies keys that can be checked with <see cref="M:Contacts.CNContact.IsKeyAvailable(Foundation.NSString)" /> and <see cref="M:Contacts.CNContact.AreKeysAvailable``1(``0[])" />.</summary>
	[MacCatalyst (13, 1)]
	[Flags]
	public enum CNContactOptions : long {
		None = 0,
		Nickname = 1 << 0,
		PhoneticGivenName = 1 << 1,
		PhoneticMiddleName = 1 << 2,
		PhoneticFamilyName = 1 << 3,
		OrganizationName = 1 << 4,
		DepartmentName = 1 << 5,
		JobTitle = 1 << 6,
		Birthday = 1 << 7,
		NonGregorianBirthday = 1 << 8,
		Note = 1 << 9,
#if !MONOMAC
		[NoMac]
		[MacCatalyst (13, 1)]
		ImageData = 1 << 10,
#endif
		ThumbnailImageData = 1 << 11,
#if !MONOMAC
		[NoMac]
		[MacCatalyst (13, 1)]
		ImageDataAvailable = 1 << 12,
#endif
		Type = 1 << 13,
		PhoneNumbers = 1 << 14,
		EmailAddresses = 1 << 15,
		PostalAddresses = 1 << 16,
		Dates = 1 << 17,
		UrlAddresses = 1 << 18,
		Relations = 1 << 19,
		SocialProfiles = 1 << 20,
		InstantMessageAddresses = 1 << 21,
	}
}
