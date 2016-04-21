//
// Contacts bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.Contacts {

#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)

	public interface ICNKeyDescriptor {}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Protocol]
	// Headers say "This protocol is reserved for Contacts framework usage.", so don't create a model
	public interface CNKeyDescriptor : NSObjectProtocol, NSSecureCoding, NSCopying {
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContact : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("contactType")]
		CNContactType ContactType { get; }

		[Export ("namePrefix")]
		string NamePrefix { get; }

		[Export ("givenName")]
		string GivenName { get; }

		[Export ("middleName")]
		string MiddleName { get; }

		[Export ("familyName")]
		string FamilyName { get; }

		[Export ("previousFamilyName")]
		string PreviousFamilyName { get; }

		[Export ("nameSuffix")]
		string NameSuffix { get; }

		[Export ("nickname")]
		string Nickname { get; }

		[Export ("phoneticGivenName")]
		string PhoneticGivenName { get; }

		[Export ("phoneticMiddleName")]
		string PhoneticMiddleName { get; }

		[Export ("phoneticFamilyName")]
		string PhoneticFamilyName { get; }

		[Export ("organizationName")]
		string OrganizationName { get; }

		[Export ("departmentName")]
		string DepartmentName { get; }

		[Export ("jobTitle")]
		string JobTitle { get; }

		[Export ("note")]
		string Note { get; }

		[NullAllowed]
		[Export ("imageData", ArgumentSemantic.Copy)]
		NSData ImageData { get; }

		[NullAllowed]
		[Export ("thumbnailImageData", ArgumentSemantic.Copy)]
		NSData ThumbnailImageData { get; }

#if !MONOMAC
		[Export ("imageDataAvailable")]
		bool ImageDataAvailable { get; }
#endif

		[Export ("phoneNumbers", ArgumentSemantic.Copy)]
		CNLabeledValue<CNPhoneNumber> [] PhoneNumbers { get; }

		[Export ("emailAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<NSString> [] EmailAddresses { get; }

		[Export ("postalAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<CNPostalAddress> [] PostalAddresses { get; }

		[Export ("urlAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<NSString> [] UrlAddresses { get; }

		[Export ("contactRelations", ArgumentSemantic.Copy)]
		CNLabeledValue<CNContactRelation> [] ContactRelations { get; }

		[Export ("socialProfiles", ArgumentSemantic.Copy)]
		CNLabeledValue<CNSocialProfile> [] SocialProfiles { get; }

		[Export ("instantMessageAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<CNInstantMessageAddress> [] InstantMessageAddresses { get; }

		[NullAllowed]
		[Export ("birthday", ArgumentSemantic.Copy)]
		NSDateComponents Birthday { get; }

		[NullAllowed]
		[Export ("nonGregorianBirthday", ArgumentSemantic.Copy)]
		NSDateComponents NonGregorianBirthday { get; }

		[Export ("dates", ArgumentSemantic.Copy)]
		CNLabeledValue<NSDateComponents> [] Dates { get; }

		[Export ("isKeyAvailable:")]
		bool IsKeyAvailable (NSString contactKey);

		// - (BOOL)areKeysAvailable:(NSArray <id<CNKeyDescriptor>>*)keyDescriptors;
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		[Export ("areKeysAvailable:")]
		bool AreKeysAvailable (NSArray keyDescriptors);

		[Static]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString contactKey);

		[Static]
		[Export ("comparatorForNameSortOrder:")] // Using func because names in ObjC block are obj1, obj2
		Func<NSObject, NSObject, NSComparisonResult> ComparatorForName (CNContactSortOrder sortOrder);

		[Static]
		[Export ("descriptorForAllComparatorKeys")]
		ICNKeyDescriptor GetDescriptorForAllComparatorKeys ();

		[Export ("isUnifiedWithContactWithIdentifier:")]
		bool IsUnifiedWithContact (string contactIdentifier);

		[Field ("CNContactPropertyNotFetchedExceptionName")]
		NSString PropertyNotFetchedExceptionName { get; }

#if !XAMCORE_3_0
		// now exposed with the corresponding CNErrorCode enum
		[Field ("CNErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		// CNContact_PredicatesExtension - they should be in a [Category] but it makes
		// [Static] API hard (and ugly) to use since they become extension methods (and
		// do not look static anymore.
		// ref: https://trello.com/c/2z8FHb95/522-generator-static-members-in-category

		[Static]
		[Export ("predicateForContactsMatchingName:")]
		NSPredicate GetPredicateForContacts (string matchingName);

		[Static]
		[Export ("predicateForContactsWithIdentifiers:")]
		NSPredicate GetPredicateForContacts (string [] identifiers);

		[Static]
		[Export ("predicateForContactsInGroupWithIdentifier:")]
		NSPredicate GetPredicateForContactsInGroup (string groupIdentifier);

		[Static]
		[Export ("predicateForContactsInContainerWithIdentifier:")]
		NSPredicate GetPredicateForContactsInContainer (string containerIdentifier);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNContactKey {

		[Field ("CNContactIdentifierKey")]
		NSString Identifier { get; }

		[Field ("CNContactNamePrefixKey")]
		NSString NamePrefix { get; }

		[Field ("CNContactGivenNameKey")]
		NSString GivenName { get; }

		[Field ("CNContactMiddleNameKey")]
		NSString MiddleName { get; }

		[Field ("CNContactFamilyNameKey")]
		NSString FamilyName { get; }

		[Field ("CNContactPreviousFamilyNameKey")]
		NSString PreviousFamilyName { get; }

		[Field ("CNContactNameSuffixKey")]
		NSString NameSuffix { get; }

		[Field ("CNContactNicknameKey")]
		NSString Nickname { get; }

		[Field ("CNContactPhoneticGivenNameKey")]
		NSString PhoneticGivenName { get; }

		[Field ("CNContactPhoneticMiddleNameKey")]
		NSString PhoneticMiddleName { get; }

		[Field ("CNContactPhoneticFamilyNameKey")]
		NSString PhoneticFamilyName { get; }

		[Field ("CNContactOrganizationNameKey")]
		NSString OrganizationName { get; }

		[Field ("CNContactDepartmentNameKey")]
		NSString DepartmentName { get; }

		[Field ("CNContactJobTitleKey")]
		NSString JobTitle { get; }

		[Field ("CNContactBirthdayKey")]
		NSString Birthday { get; }

		[Field ("CNContactNonGregorianBirthdayKey")]
		NSString NonGregorianBirthday { get; }

		[Field ("CNContactNoteKey")]
		NSString Note { get; }

#if !MONOMAC
		[Field ("CNContactImageDataKey")]
		NSString ImageData { get; }

		[Field ("CNContactImageDataAvailableKey")]
		NSString ImageDataAvailable { get; }
#endif

		[Field ("CNContactThumbnailImageDataKey")]
		NSString ThumbnailImageData { get; }

		[Field ("CNContactTypeKey")]
		NSString Type { get; }

		[Field ("CNContactPhoneNumbersKey")]
		NSString PhoneNumbers { get; }

		[Field ("CNContactEmailAddressesKey")]
		NSString EmailAddresses { get; }

		[Field ("CNContactPostalAddressesKey")]
		NSString PostalAddresses { get; }

		[Field ("CNContactDatesKey")]
		NSString Dates { get; }

		[Field ("CNContactUrlAddressesKey")]
		NSString UrlAddresses { get; }

		[Field ("CNContactRelationsKey")]
		NSString Relations { get; }

		[Field ("CNContactSocialProfilesKey")]
		NSString SocialProfiles { get; }

		[Field ("CNContactInstantMessageAddressesKey")]
		NSString InstantMessageAddresses { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // using init raises an exception according to docs
	public interface CNContactFetchRequest {

		[DesignatedInitializer]
		[Export ("initWithKeysToFetch:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		IntPtr Constructor (NSArray keysToFetch);

		[NullAllowed]
		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; set; }

		[Export ("keysToFetch", ArgumentSemantic.Copy)]
		// we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		// cannot be exposed as NSString since they could be internalized types, like CNAggregateKeyDescriptor
		NSArray KeysToFetch { get; set; }

		[Export ("mutableObjects")]
		bool MutableObjects { get; set; }

		[Export ("unifyResults")]
		bool UnifyResults { get; set; }

		[Export ("sortOrder")]
		CNContactSortOrder SortOrder { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSFormatter))]
	public interface CNContactFormatter {

		[Static]
		[Export ("descriptorForRequiredKeysForStyle:")]
		ICNKeyDescriptor GetDescriptorForRequiredKeys (CNContactFormatterStyle style);

		[Static]
		[Export ("stringFromContact:style:")]
		string GetStringFrom (CNContact contact, CNContactFormatterStyle style);

		[Static]
		[Export ("attributedStringFromContact:style:defaultAttributes:")]
		NSAttributedString GetAttributedStringFrom (CNContact contact, CNContactFormatterStyle style, [NullAllowed] NSDictionary attributes);

		[Static]
		[Export ("nameOrderForContact:")]
		CNContactDisplayNameOrder GetNameOrderFor (CNContact contact);

		[Static]
		[Export ("delimiterForContact:")]
		string GetDelimiterFor (CNContact contact);

		[Export ("style")]
		CNContactFormatterStyle Style { get; set; }

		[Export ("stringFromContact:")]
		string GetString (CNContact contact);

		[Export ("attributedStringFromContact:defaultAttributes:")]
		NSAttributedString GetAttributedString (CNContact contact, [NullAllowed] NSDictionary attributes);

		[Field ("CNContactPropertyAttribute")]
		NSString ContactPropertyAttribute { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContactProperty : NSCopying, NSSecureCoding {

		[Export ("contact", ArgumentSemantic.Copy)]
		CNContact Contact { get; }

		[Export ("key")]
		string Key { get; }

		[NullAllowed]
		[Export ("value")]
		NSObject Value { get; }

		[NullAllowed]
		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed]
		[Export ("label")]
		string Label { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContactRelation : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Static]
		[Export ("contactRelationWithName:")]
		CNContactRelation FromName (string name);

		[Export ("initWithName:")]
		IntPtr Constructor (string name);

		[Export ("name")]
		string Name { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNLabelContactRelationKey {

		[Field ("CNLabelContactRelationFather")]
		NSString Father { get; }

		[Field ("CNLabelContactRelationMother")]
		NSString Mother { get; }

		[Field ("CNLabelContactRelationParent")]
		NSString Parent { get; }

		[Field ("CNLabelContactRelationBrother")]
		NSString Brother { get; }

		[Field ("CNLabelContactRelationSister")]
		NSString Sister { get; }

		[Field ("CNLabelContactRelationChild")]
		NSString Child { get; }

		[Field ("CNLabelContactRelationFriend")]
		NSString Friend { get; }

		[Field ("CNLabelContactRelationSpouse")]
		NSString Spouse { get; }

		[Field ("CNLabelContactRelationPartner")]
		NSString Partner { get; }

		[Field ("CNLabelContactRelationAssistant")]
		NSString Assistant { get; }

		[Field ("CNLabelContactRelationManager")]
		NSString Manager { get; }
	}

	public delegate void CNContactStoreRequestAccessHandler (bool granted, NSError error);
	public delegate void CNContactStoreEnumerateContactsHandler (CNContact contact, bool stop);

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContactStore {

		[Static]
		[Export ("authorizationStatusForEntityType:")]
		CNAuthorizationStatus GetAuthorizationStatus (CNEntityType entityType);

		[Async]
		[Export ("requestAccessForEntityType:completionHandler:")]
		void RequestAccess (CNEntityType entityType, CNContactStoreRequestAccessHandler completionHandler);

		[Export ("unifiedContactsMatchingPredicate:keysToFetch:error:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		CNContact[] GetUnifiedContacts (NSPredicate predicate, NSArray keys, out NSError error);

		[Export ("unifiedContactWithIdentifier:keysToFetch:error:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		CNContact GetUnifiedContact (string identifier, NSArray keys, out NSError error);

		[NoiOS, NoWatch]
		[Export ("unifiedMeContactWithKeysToFetch:error:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		NSObject GetUnifiedMeContact (NSArray keys, out NSError error);

		[Export ("enumerateContactsWithFetchRequest:error:usingBlock:")]
		bool EnumerateContacts (CNContactFetchRequest fetchRequest, out NSError error, CNContactStoreEnumerateContactsHandler handler);

		[Export ("groupsMatchingPredicate:error:")]
		CNGroup [] GetGroups ([NullAllowed] NSPredicate predicate, out NSError error);

		[Export ("containersMatchingPredicate:error:")]
		CNContainer [] GetContainers ([NullAllowed] NSPredicate predicate, out NSError error);

#if !WATCH
		[Export ("executeSaveRequest:error:")]
		bool ExecuteSaveRequest (CNSaveRequest saveRequest, out NSError error);
#endif

		[Export ("defaultContainerIdentifier")]
		string DefaultContainerIdentifier { get; }

		[Notification]
		[Field ("CNContactStoreDidChangeNotification")]
		NSString NotificationDidChange { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContactsUserDefaults {

		[Static]
		[Export ("sharedDefaults")]
		CNContactsUserDefaults GetSharedDefaults ();

		[Export ("sortOrder")]
		CNContactSortOrder SortOrder { get; }

		[Export ("countryCode")]
		string CountryCode { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContactVCardSerialization {

		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor GetDescriptorFromRequiredKeys ();

		[Static]
		[Export ("dataWithContacts:error:")]
		NSData GetDataFromContacts (CNContact [] contacts, out NSError error);

		[Static]
		[Export ("contactsWithData:error:")]
		CNContact [] GetContactsFromData (NSData data, out NSError error);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Category]
	[BaseType (typeof (CNContainer))]
	public interface CNContainer_PredicatesExtension {

		[Static]
		[Export ("predicateForContainersWithIdentifiers:")]
		NSPredicate GetPredicateForContainers (string [] identifiers);

		[Static]
		[Export ("predicateForContainerOfContactWithIdentifier:")]
		NSPredicate GetPredicateForContainerOfContact (string contactIdentifier);

		[Static]
		[Export ("predicateForContainerOfGroupWithIdentifier:")]
		NSPredicate GetPredicateForContainerOfGroup (string groupIdentifier);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNContainer : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("type", ArgumentSemantic.Assign)]
		CNContainerType ContainerType { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNContainerKey { // Can be used in KVO

		[Field ("CNContainerIdentifierKey")]
		NSString Identifier { get; }

		[Field ("CNContainerNameKey")]
		NSString Name { get; }

		[Field ("CNContainerTypeKey")]
		NSString Type { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNErrorUserInfoKey {

		[Field ("CNErrorUserInfoAffectedRecordsKey")]
		NSString AffectedRecords { get; }

		[Field ("CNErrorUserInfoAffectedRecordIdentifiersKey")]
		NSString AffectedRecordIdentifiers { get; }

		[Field ("CNErrorUserInfoValidationErrorsKey")]
		NSString ValidationErrors { get; }

		[Field ("CNErrorUserInfoKeyPathsKey")]
		NSString KeyPaths { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Category]
	[BaseType (typeof (CNGroup))]
	public interface CNGroup_PredicatesExtension {

		[Static]
		[Export ("predicateForGroupsWithIdentifiers:")]
		NSPredicate GetPredicateForGroups (string [] identifiers);

		[NoiOS][NoWatch]
		[Static]
		[Export ("predicateForSubgroupsInGroupWithIdentifier:")]
		NSPredicate GetPredicateForSubgroupsInGroup (string parentGroupIdentifier);

		[Static]
		[Export ("predicateForGroupsInContainerWithIdentifier:")]
		NSPredicate GetPredicateForGroupsInContainer (string containerIdentifier);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNGroup : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("name")]
		string Name { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNGroupKey { // Can be used in KVO

		[Field ("CNGroupIdentifierKey")]
		NSString Identifier { get; }

		[Field ("CNGroupNameKey")]
		NSString Name { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNInstantMessageAddress : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("initWithUsername:service:")]
		IntPtr Constructor (string username, string service);

		[Export ("username")]
		string Username { get; }

		[Export ("service")]
		string Service { get; }

		[Static]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString propertyKey);

		[Static]
		[Export ("localizedStringForService:")]
		string LocalizeService (NSString service);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNInstantMessageAddressKey { // Can be used in KVO

		[Field ("CNInstantMessageAddressUsernameKey")]
		NSString Username { get; }

		[Field ("CNInstantMessageAddressServiceKey")]
		NSString Service { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNInstantMessageServiceKey {

		[Field ("CNInstantMessageServiceAIM")]
		NSString Aim { get; }

		[Field ("CNInstantMessageServiceFacebook")]
		NSString Facebook { get; }

		[Field ("CNInstantMessageServiceGaduGadu")]
		NSString GaduGadu { get; }

		[Field ("CNInstantMessageServiceGoogleTalk")]
		NSString GoogleTalk { get; }

		[Field ("CNInstantMessageServiceICQ")]
		NSString Icq { get; }

		[Field ("CNInstantMessageServiceJabber")]
		NSString Jabber { get; }

		[Field ("CNInstantMessageServiceMSN")]
		NSString Msn { get; }

		[Field ("CNInstantMessageServiceQQ")]
		NSString QQ { get; }

		[Field ("CNInstantMessageServiceSkype")]
		NSString Skype { get; }

		[Field ("CNInstantMessageServiceYahoo")]
		NSString Yahoo { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNLabeledValue<ValueType> : NSCopying, NSSecureCoding
		where ValueType : INSCopying, INSSecureCoding
	{

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("label")]
		string Label { get; }

		[Export ("value", ArgumentSemantic.Copy)]
		ValueType Value { get; }

		[Static]
		[Export ("labeledValueWithLabel:value:")]
		ValueType FromLabel ([NullAllowed] string label, ValueType value);

		[Export ("initWithLabel:value:")]
		IntPtr Constructor ([NullAllowed] string label, ValueType value);

		[Export ("labeledValueBySettingLabel:")]
		ValueType GetLabeledValue ([NullAllowed] string label);

		[Export ("labeledValueBySettingValue:")]
		ValueType GetLabeledValue (ValueType value);

		[Export ("labeledValueBySettingLabel:value:")]
		ValueType GetLabeledValue ([NullAllowed] string label, ValueType value);

		// TODO: Enumify this method, it seems to accept CNLabelKey, CNLabelContactRelationKey and CNLabelPhoneNumberKey unsure if it takes random user values
		[Static]
		[Export ("localizedStringForLabel:")]
		string LocalizeLabel (NSString labelKey);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNLabelKey {

		[Field ("CNLabelHome")]
		NSString Home { get; }

		[Field ("CNLabelWork")]
		NSString Work { get; }

		[Field ("CNLabelOther")]
		NSString Other { get; }

		[Field ("CNLabelEmailiCloud")]
		NSString EmailiCloud { get; }

		[Field ("CNLabelURLAddressHomePage")]
		NSString UrlAddressHomePage { get; }

		[Field ("CNLabelDateAnniversary")]
		NSString DateAnniversary { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (CNContact))]
	public interface CNMutableContact {

		[New]
		[Export ("contactType")]
		CNContactType ContactType { get; set; }

		[New]
		[Export ("namePrefix")]
		string NamePrefix { get; set; }

		[New]
		[Export ("givenName")]
		string GivenName { get; set; }

		[New]
		[Export ("middleName")]
		string MiddleName { get; set; }

		[New]
		[Export ("familyName")]
		string FamilyName { get; set; }

		[New]
		[Export ("previousFamilyName")]
		string PreviousFamilyName { get; set; }

		[New]
		[Export ("nameSuffix")]
		string NameSuffix { get; set; }

		[New]
		[Export ("nickname")]
		string Nickname { get; set; }

		[New]
		[Export ("phoneticGivenName")]
		string PhoneticGivenName { get; set; }

		[New]
		[Export ("phoneticMiddleName")]
		string PhoneticMiddleName { get; set; }

		[New]
		[Export ("phoneticFamilyName")]
		string PhoneticFamilyName { get; set; }

		[New]
		[Export ("organizationName")]
		string OrganizationName { get; set; }

		[New]
		[Export ("departmentName")]
		string DepartmentName { get; set; }

		[New]
		[Export ("jobTitle")]
		string JobTitle { get; set; }

		[New]
		[Export ("note")]
		string Note { get; set; }

		[New]
		[NullAllowed]
		[Export ("imageData", ArgumentSemantic.Copy)]
		NSData ImageData { get; set; }

		[New]
		[Export ("phoneNumbers", ArgumentSemantic.Copy)]
		CNLabeledValue<CNPhoneNumber> [] PhoneNumbers { get; set; }

		[New]
		[Export ("emailAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<NSString> [] EmailAddresses { get; set; }

		[New]
		[Export ("postalAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<CNPostalAddress> [] PostalAddresses { get; set; }

		[New]
		[Export ("urlAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<NSString> [] UrlAddresses { get; set; }

		[New]
		[Export ("contactRelations", ArgumentSemantic.Copy)]
		CNLabeledValue<CNContactRelation> [] ContactRelations { get; set; }

		[New]
		[Export ("socialProfiles", ArgumentSemantic.Copy)]
		CNLabeledValue<CNSocialProfile> [] SocialProfiles { get; set; }

		[New]
		[Export ("instantMessageAddresses", ArgumentSemantic.Copy)]
		CNLabeledValue<CNInstantMessageAddress> [] InstantMessageAddresses { get; set; }

		[New]
		[NullAllowed]
		[Export ("birthday", ArgumentSemantic.Copy)]
		NSDateComponents Birthday { get; set; }

		[New]
		[NullAllowed]
		[Export ("nonGregorianBirthday", ArgumentSemantic.Copy)]
		NSDateComponents NonGregorianBirthday { get; set; }

		[New]
		[Export ("dates", ArgumentSemantic.Copy)]
		CNLabeledValue<NSDateComponents> [] Dates { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (CNGroup))]
	public interface CNMutableGroup {

		[New]
		[Export ("name")]
		string Name { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (CNPostalAddress))]
	public interface CNMutablePostalAddress {

		[New]
		[Export ("street")]
		string Street { get; set; }

		[New]
		[Export ("city")]
		string City { get; set; }

		[New]
		[Export ("state")]
		string State { get; set; }

		[New]
		[Export ("postalCode")]
		string PostalCode { get; set; }

		[New]
		[Export ("country")]
		string Country { get; set; }

		[New]
		[Export ("ISOCountryCode")]
		string IsoCountryCode { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Apple doc: no handle (nil) if no string (or nil string) is given
	public interface CNPhoneNumber : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		// @required + (instancetype)phoneNumberWithStringValue:(NSString *)stringValue;
		[Static, Export ("phoneNumberWithStringValue:")]
		CNPhoneNumber PhoneNumberWithStringValue (string stringValue);

		// @required - (instancetype)initWithStringValue:(NSString *)string;
		[Export ("initWithStringValue:")]
		IntPtr Constructor (string stringValue);

		// @property (readonly, copy, nonatomic) NSString * stringValue;
		[Export ("stringValue")]
		string StringValue { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNLabelPhoneNumberKey {

		[Field ("CNLabelPhoneNumberiPhone")]
		NSString iPhone { get; }

		[Field ("CNLabelPhoneNumberMobile")]
		NSString Mobile { get; }

		[Field ("CNLabelPhoneNumberMain")]
		NSString Main { get; }

		[Field ("CNLabelPhoneNumberHomeFax")]
		NSString HomeFax { get; }

		[Field ("CNLabelPhoneNumberWorkFax")]
		NSString WorkFax { get; }

		[Field ("CNLabelPhoneNumberOtherFax")]
		NSString OtherFax { get; }

		[Field ("CNLabelPhoneNumberPager")]
		NSString Pager { get; }
	}

	[iOS (9,0)] [Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNPostalAddress : NSCopying, NSMutableCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("street")]
		string Street { get; }

		[Export ("city")]
		string City { get; }

		[Export ("state")]
		string State { get; }

		[Export ("postalCode")]
		string PostalCode { get; }

		[Export ("country")]
		string Country { get; }

		[Export ("ISOCountryCode")]
		string IsoCountryCode { get; }

		[Static]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString property);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNPostalAddressKey { // Can be used in KVO

		[Field ("CNPostalAddressStreetKey")]
		NSString Street { get; }

		[Field ("CNPostalAddressCityKey")]
		NSString City { get; }

		[Field ("CNPostalAddressStateKey")]
		NSString State { get; }

		[Field ("CNPostalAddressPostalCodeKey")]
		NSString PostalCode { get; }

		[Field ("CNPostalAddressCountryKey")]
		NSString Country { get; }

		[Field ("CNPostalAddressISOCountryCodeKey")]
		NSString IsoCountryCode { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSFormatter))]
	public interface CNPostalAddressFormatter {

		[Static]
		[Export ("stringFromPostalAddress:style:")]
		string GetStringFrom (CNPostalAddress postalAddress, CNPostalAddressFormatterStyle style);

		[Static]
		[Export ("attributedStringFromPostalAddress:style:withDefaultAttributes:")]
		NSAttributedString GetAttributedStringFrom (CNPostalAddress postalAddress, CNPostalAddressFormatterStyle style, NSDictionary attributes);

		[Export ("stringFromPostalAddress:")]
		string GetStringFromPostalAddress (CNPostalAddress postalAddress);

		[Export ("attributedStringFromPostalAddress:withDefaultAttributes:")]
		NSAttributedString GetAttributedStringFromPostalAddress (CNPostalAddress postalAddress, NSDictionary attributes);

		[Export ("style")]
		CNPostalAddressFormatterStyle Style { get; set; }

		[Field ("CNPostalAddressPropertyAttribute")]
		NSString PropertyAttribute { get; }

		[Field ("CNPostalAddressLocalizedPropertyNameAttribute")]
		NSString LocalizedPropertyNameAttribute { get; }
	}

#if !WATCH
	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNSaveRequest {

		[Export ("addContact:toContainerWithIdentifier:")]
		void AddContact (CNMutableContact contact, [NullAllowed] string identifier);

		[Export ("updateContact:")]
		void UpdateContact (CNMutableContact contact);

		[Export ("deleteContact:")]
		void DeleteContact (CNMutableContact contact);

		[Export ("addGroup:toContainerWithIdentifier:")]
		void AddGroup (CNMutableGroup group, [NullAllowed] string identifier);

		[Export ("updateGroup:")]
		void UpdateGroup (CNMutableGroup group);

		[Export ("deleteGroup:")]
		void DeleteGroup (CNMutableGroup group);

		[NoiOS]
		[Export ("addSubgroup:toGroup:")]
		void AddSubgroup (CNGroup subgroup, CNGroup group);

		[NoiOS]
		[Export ("removeSubgroup:fromGroup:")]
		void RemoveSubgroup (CNGroup subgroup, CNGroup group);

		[Export ("addMember:toGroup:")]
		void AddMember (CNContact contact, CNGroup group);

		[Export ("removeMember:fromGroup:")]
		void RemoveMember (CNContact contact, CNGroup group);
	}
#endif // !WATCH

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	public interface CNSocialProfile : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("urlString")]
		string UrlString { get; }

		[Export ("username")]
		string Username { get; }

		[Export ("userIdentifier")]
		string UserIdentifier { get; }

		[Export ("service")]
		string Service { get; }

		[Export ("initWithUrlString:username:userIdentifier:service:")]
		IntPtr Constructor ([NullAllowed] string url, [NullAllowed] string username, [NullAllowed] string userIdentifier, [NullAllowed] string service);

		[Static]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString key);

		[Static]
		[Export ("localizedStringForService:")]
		string LocalizeService (NSString service);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNSocialProfileKey { // Can be used in KVO

		[Field ("CNSocialProfileURLStringKey")]
		NSString UrlString { get; }

		[Field ("CNSocialProfileUsernameKey")]
		NSString Username { get; }

		[Field ("CNSocialProfileUserIdentifierKey")]
		NSString UserIdentifier { get; }

		[Field ("CNSocialProfileServiceKey")]
		NSString Service { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	public interface CNSocialProfileServiceKey {

		[Field ("CNSocialProfileServiceFacebook")]
		NSString Facebook { get; }

		[Field ("CNSocialProfileServiceFlickr")]
		NSString Flickr { get; }

		[Field ("CNSocialProfileServiceLinkedIn")]
		NSString LinkedIn { get; }

		[Field ("CNSocialProfileServiceMySpace")]
		NSString MySpace { get; }

		[Field ("CNSocialProfileServiceSinaWeibo")]
		NSString SinaWeibo { get; }

		[Field ("CNSocialProfileServiceTencentWeibo")]
		NSString TencentWeibo { get; }

		[Field ("CNSocialProfileServiceTwitter")]
		NSString Twitter { get; }

		[Field ("CNSocialProfileServiceYelp")]
		NSString Yelp { get; }

		[Field ("CNSocialProfileServiceGameCenter")]
		NSString GameCenter { get; }
	}
#endif // XAMCORE_2_0
}


