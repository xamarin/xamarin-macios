//
// Contacts bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)

namespace XamCore.Contacts {

	// NSInteger -> CNContact.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNContactType : nint {
		Person,
		Organization
	}

	// NSInteger -> CNContact.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNContactSortOrder : nint {
		None,
		UserDefault,
		GivenName,
		FamilyName
	}

	// NSInteger -> CNContactFormatter.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNContactFormatterStyle : nint {
		FullName,
		PhoneticFullName
	}

	// NSInteger -> CNContactFormatter.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNContactDisplayNameOrder : nint {
		UserDefault,
		GivenNameFirst,
		FamilyNameFirst
	}

	// NSInteger -> CNContactStore.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNEntityType : nint {
		Contacts
	}

	// NSInteger -> CNContactStore.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNAuthorizationStatus : nint {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized
	}

	// NSInteger -> CNContainer.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNContainerType : nint {
		Unassigned = 0,
		Local,
		Exchange,
		CardDav
	}

	// NSInteger -> CNError.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	[ErrorDomain ("CNErrorDomain")]
	public enum CNErrorCode : nint {
		CommunicationError = 1,
		DataAccessError = 2,
		AuthorizationDenied = 100,
		NoAccessibleWritableContainers = 101,
		RecordDoesNotExist = 200,
		InsertedRecordAlreadyExists = 201,
		ContainmentCycle = 202,
		ContainmentScope = 203,
		ParentRecordDoesNotExist = 204,
		ValidationMultipleErrors = 300,
		ValidationTypeMismatch = 301,
		ValidationConfigurationError = 302,
		PredicateInvalid = 400,
		PolicyViolation = 500,
		[iOS (11,0), Mac (10,13)]
		ClientIdentifierInvalid = 600,
		[iOS (11,0), Mac (10,13)]
		ClientIdentifierDoesNotExist = 601,
		[iOS (11,0), Mac (10,13)]
		VCardMalformed = 700,
	}

	// NSInteger -> CNPostalAddressFormatter.h
	[iOS (9,0), Mac (10,11)]
	[Native]
	public enum CNPostalAddressFormatterStyle : nint {
		MailingAddress,
	}
}

#endif // XAMCORE_2_0


