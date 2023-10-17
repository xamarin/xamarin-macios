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
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactType : long {
		Person,
		Organization
	}

	// NSInteger -> CNContact.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactSortOrder : long {
		None,
		UserDefault,
		GivenName,
		FamilyName
	}

	// NSInteger -> CNContactFormatter.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactFormatterStyle : long {
		FullName,
		PhoneticFullName
	}

	// NSInteger -> CNContactFormatter.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContactDisplayNameOrder : long {
		UserDefault,
		GivenNameFirst,
		FamilyNameFirst
	}

	// NSInteger -> CNContactStore.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNEntityType : long {
		Contacts
	}

	// NSInteger -> CNContactStore.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized
	}

	// NSInteger -> CNContainer.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNContainerType : long {
		Unassigned = 0,
		Local,
		Exchange,
		CardDav
	}

	// NSInteger -> CNError.h
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
	[MacCatalyst (13, 1)]
	[Native]
	public enum CNPostalAddressFormatterStyle : long {
		MailingAddress,
	}

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
