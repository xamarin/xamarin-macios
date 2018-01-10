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
using ObjCRuntime;
using Foundation;


namespace Contacts {

#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)

	interface ICNKeyDescriptor {}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Protocol]
	// Headers say "This protocol is reserved for Contacts framework usage.", so don't create a model
	interface CNKeyDescriptor : NSObjectProtocol, NSSecureCoding, NSCopying {
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNContact : NSCopying, NSMutableCopying, NSSecureCoding, NSItemProviderReading, NSItemProviderWriting {

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

		[iOS (10,0)][Mac (10,12, onlyOn64: true)]
		[Watch (3,0)]
		[Export ("phoneticOrganizationName")]
		string PhoneticOrganizationName { get; }

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

		[Mac (10,12)]
		[Export ("imageDataAvailable")]
		bool ImageDataAvailable { get; }

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
	interface CNContactKey {

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

		[iOS (10,0)][Mac (10,12, onlyOn64: true)]
		[Watch (3,0)]
		[Field ("CNContactPhoneticOrganizationNameKey")]
		NSString PhoneticOrganizationName { get; }

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

		[Field ("CNContactImageDataKey")]
		NSString ImageData { get; }

		[Mac (10,12)]
		[Field ("CNContactImageDataAvailableKey")]
		NSString ImageDataAvailable { get; }

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
	interface CNContactFetchRequest : NSSecureCoding {

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

		[iOS (10,0)][Mac (10,12)] // API existed previously ? maybe it was not working before now ?
		[Export ("mutableObjects")]
		bool MutableObjects { get; set; }

		[Export ("unifyResults")]
		bool UnifyResults { get; set; }

		[Export ("sortOrder")]
		CNContactSortOrder SortOrder { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSFormatter))]
	interface CNContactFormatter : NSSecureCoding {

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
	interface CNContactProperty : NSCopying, NSSecureCoding {

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
	interface CNContactRelation : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

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
	interface CNLabelContactRelationKey {

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

		[iOS (11,0), Mac (10,13, onlyOn64: true)]
		[Field ("CNLabelContactRelationSon")]
		NSString Son { get; }

		[iOS (11,0), Mac (10,13, onlyOn64: true)]
		[Field ("CNLabelContactRelationDaughter")]
		NSString Daughter { get; }
	}

	delegate void CNContactStoreRequestAccessHandler (bool granted, NSError error);
#if !XAMCORE_4_0
	delegate void CNContactStoreEnumerateContactsHandler (CNContact contact, bool stop);
#endif
	delegate void CNContactStoreListContactsHandler (CNContact contact, ref bool stop);

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNContactStore {

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

#if !XAMCORE_4_0 && !WATCH
		[Obsolete ("Use the overload that takes 'CNContactStoreListContactsHandler' instead.")]
		[Export ("enumerateContactsWithFetchRequest:error:usingBlock:")]
		bool EnumerateContacts (CNContactFetchRequest fetchRequest, out NSError error, CNContactStoreEnumerateContactsHandler handler);

		[Sealed] // We will introduce breaking changes anyways if XAMCORE_4_0 happens
#endif
		[Export ("enumerateContactsWithFetchRequest:error:usingBlock:")]
		bool EnumerateContacts (CNContactFetchRequest fetchRequest, out NSError error, CNContactStoreListContactsHandler handler);

		[Export ("groupsMatchingPredicate:error:")]
		CNGroup [] GetGroups ([NullAllowed] NSPredicate predicate, out NSError error);

		[Export ("containersMatchingPredicate:error:")]
		CNContainer [] GetContainers ([NullAllowed] NSPredicate predicate, out NSError error);

#if !WATCH
		[Export ("executeSaveRequest:error:")]
		bool ExecuteSaveRequest (CNSaveRequest saveRequest, out NSError error);
#endif

		[Export ("defaultContainerIdentifier")]
		[NullAllowed]
		string DefaultContainerIdentifier { get; }

		[Notification]
		[Field ("CNContactStoreDidChangeNotification")]
		NSString NotificationDidChange { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe (false)]
	interface CNContactsUserDefaults {

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
	interface CNContactVCardSerialization {

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

#if !XAMCORE_4_0
	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Category (allowStaticMembers: true)]
	[BaseType (typeof (CNContainer))]
	interface CNContainer_PredicatesExtension {

		[Obsolete ("Use 'CNContainer.CreatePredicateForContainers' instead.")]
		[Static]
		[Export ("predicateForContainersWithIdentifiers:")]
		NSPredicate GetPredicateForContainers (string [] identifiers);

		[Obsolete ("Use 'CNContainer.CreatePredicateForContainerOfContact' instead.")]
		[Static]
		[Export ("predicateForContainerOfContactWithIdentifier:")]
		NSPredicate GetPredicateForContainerOfContact (string contactIdentifier);

		[Obsolete ("Use 'CNContainer.CreatePredicateForContainerOfGroup' instead.")]
		[Static]
		[Export ("predicateForContainerOfGroupWithIdentifier:")]
		NSPredicate GetPredicateForContainerOfGroup (string groupIdentifier);
	}
#endif

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNContainer : NSCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("type", ArgumentSemantic.Assign)]
		CNContainerType ContainerType { get; }

#region comes from CNContainer (Predicates) Category
		[Static]
#if XAMCORE_4_0
		[Export ("predicateForContainersWithIdentifiers:")]
#else
		[Wrap ("(null as CNContainer).GetPredicateForContainers (identifiers)")]
#endif
		NSPredicate CreatePredicateForContainers (string [] identifiers);

		[Static]
#if XAMCORE_4_0
		[Export ("predicateForContainerOfContactWithIdentifier:")]
#else
		[Wrap ("(null as CNContainer).GetPredicateForContainerOfContact (contactIdentifier)")]
#endif
		NSPredicate CreatePredicateForContainerOfContact (string contactIdentifier);

		[Static]
#if XAMCORE_4_0
		[Export ("predicateForContainerOfGroupWithIdentifier:")]
#else
		[Wrap ("(null as CNContainer).GetPredicateForContainerOfGroup (groupIdentifier)")]
#endif
		NSPredicate CreatePredicateForContainerOfGroup (string groupIdentifier);
#endregion
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNContainerKey { // Can be used in KVO

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
	interface CNErrorUserInfoKey {

		[Field ("CNErrorUserInfoAffectedRecordsKey")]
		NSString AffectedRecords { get; }

		[Field ("CNErrorUserInfoAffectedRecordIdentifiersKey")]
		NSString AffectedRecordIdentifiers { get; }

		[Field ("CNErrorUserInfoValidationErrorsKey")]
		NSString ValidationErrors { get; }

		[Field ("CNErrorUserInfoKeyPathsKey")]
		NSString KeyPaths { get; }
	}

#if !XAMCORE_4_0
	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Category (allowStaticMembers: true)]
	[BaseType (typeof (CNGroup))]
	interface CNGroup_PredicatesExtension {

		[Obsolete ("Use 'CNGroup.CreatePredicateForGroups' instead.")]
		[Static]
		[Export ("predicateForGroupsWithIdentifiers:")]
		NSPredicate GetPredicateForGroups (string [] identifiers);

		[Obsolete ("Use 'CNGroup.CreatePredicateForSubgroupsInGroup' instead.")]
		[NoiOS][NoWatch]
		[Static]
		[Export ("predicateForSubgroupsInGroupWithIdentifier:")]
		NSPredicate GetPredicateForSubgroupsInGroup (string parentGroupIdentifier);

		[Obsolete ("Use 'CNGroup.CreatePredicateForGroupsInContainer' instead.")]
		[Static]
		[Export ("predicateForGroupsInContainerWithIdentifier:")]
		NSPredicate GetPredicateForGroupsInContainer (string containerIdentifier);
	}
#endif

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNGroup : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("name")]
		string Name { get; }

#region comes from CNGroup (Predicates) Category
		[Static]
#if XAMCORE_4_0
		[Export ("predicateForGroupsWithIdentifiers:")]
#else
		[Wrap ("(null as CNGroup).GetPredicateForGroups (identifiers)")]
#endif
		NSPredicate CreatePredicateForGroups (string [] identifiers);

		[NoiOS][NoWatch]
		[Static]
#if XAMCORE_4_0
		[Export ("predicateForSubgroupsInGroupWithIdentifier:")]
#else
		[Wrap ("(null as CNGroup).GetPredicateForSubgroupsInGroup (parentGroupIdentifier)")]
#endif
		NSPredicate CreatePredicateForSubgroupsInGroup (string parentGroupIdentifier);

		[Static]
#if XAMCORE_4_0
		[Export ("predicateForGroupsInContainerWithIdentifier:")]
#else
		[Wrap ("(null as CNGroup).GetPredicateForGroupsInContainer (containerIdentifier)")]
#endif
		NSPredicate CreatePredicateForGroupsInContainer (string containerIdentifier);
#endregion
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNGroupKey { // Can be used in KVO

		[Field ("CNGroupIdentifierKey")]
		NSString Identifier { get; }

		[Field ("CNGroupNameKey")]
		NSString Name { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNInstantMessageAddress : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

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
	interface CNInstantMessageAddressKey { // Can be used in KVO

		[Field ("CNInstantMessageAddressUsernameKey")]
		NSString Username { get; }

		[Field ("CNInstantMessageAddressServiceKey")]
		NSString Service { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNInstantMessageServiceKey {

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
	interface CNLabeledValue<ValueType> : NSCopying, NSSecureCoding
		where ValueType : INSCopying, INSSecureCoding
	{

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed]
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
	interface CNLabelKey {

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
	interface CNMutableContact {

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

		[iOS (10,0)][Mac (10,12, onlyOn64: true)]
		[Watch (3,0)]
		[New]
		[Export ("phoneticOrganizationName")]
		string PhoneticOrganizationName { get; set; }

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
	interface CNMutableGroup {

		[New]
		[Export ("name")]
		string Name { get; set; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (CNPostalAddress))]
	interface CNMutablePostalAddress {

		[New]
		[Export ("street")]
		string Street { get; set; }

		[iOS (10,3), Mac (10,12,4, onlyOn64: true)]
		[New]
		[Export ("subLocality")]
		string SubLocality { get; set; }

		[New]
		[Export ("city")]
		string City { get; set; }

		[iOS (10,3), Mac (10,12,4, onlyOn64: true)]
		[New]
		[Export ("subAdministrativeArea")]
		string SubAdministrativeArea { get; set; }

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
	interface CNPhoneNumber : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Static, Export ("phoneNumberWithStringValue:")]
		[return: NullAllowed]
		CNPhoneNumber PhoneNumberWithStringValue (string stringValue);

		[Export ("initWithStringValue:")]
		IntPtr Constructor (string stringValue);

		[Export ("stringValue")]
		string StringValue { get; }
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNLabelPhoneNumberKey {

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
	interface CNPostalAddress : NSCopying, NSMutableCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("street")]
		string Street { get; }

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Export ("subLocality")]
		string SubLocality { get; }

		[Export ("city")]
		string City { get; }

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Export ("subAdministrativeArea")]
		string SubAdministrativeArea { get; }

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

		[Static]
		[Wrap ("LocalizeProperty (option.GetConstant ())")]
		string LocalizeProperty (CNPostalAddressKeyOption option);
	}

#if !XAMCORE_4_0
	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNPostalAddressKey { // Can be used in KVO

		[Field ("CNPostalAddressStreetKey")]
		NSString Street { get; }

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Field ("CNPostalAddressSubLocalityKey")]
		NSString SubLocality { get; }

		[Field ("CNPostalAddressCityKey")]
		NSString City { get; }

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Field ("CNPostalAddressSubAdministrativeAreaKey")]
		NSString SubAdministrativeArea { get; }

		[Field ("CNPostalAddressStateKey")]
		NSString State { get; }

		[Field ("CNPostalAddressPostalCodeKey")]
		NSString PostalCode { get; }

		[Field ("CNPostalAddressCountryKey")]
		NSString Country { get; }

		[Field ("CNPostalAddressISOCountryCodeKey")]
		NSString IsoCountryCode { get; }
	}
#endif

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	public enum CNPostalAddressKeyOption {
		[Field ("CNPostalAddressStreetKey")]
		Street,
		[Field ("CNPostalAddressCityKey")]
		City,
		[Field ("CNPostalAddressStateKey")]
		State,
		[Field ("CNPostalAddressPostalCodeKey")]
		PostalCode,
		[Field ("CNPostalAddressCountryKey")]
		Country,
		[Field ("CNPostalAddressISOCountryCodeKey")]
		IsoCountryCode,

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Field ("CNPostalAddressSubLocalityKey")]
		SubLocality,

		[iOS (10,3)] [Mac (10,12,4, onlyOn64: true)]
		[Field ("CNPostalAddressSubAdministrativeAreaKey")]
		SubAdministrativeArea,
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSFormatter))]
	interface CNPostalAddressFormatter {

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
	interface CNSaveRequest {

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
	interface CNSocialProfile : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

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
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString key);

		[Static]
		[Wrap ("LocalizeProperty (key.GetConstant ())")]
		string LocalizeProperty (CNPostalAddressKeyOption key);

		[Static]
		[Export ("localizedStringForService:")]
		string LocalizeService (NSString service);
	}

	[iOS (9,0), Mac (10,11, onlyOn64: true)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNSocialProfileKey { // Can be used in KVO

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
	interface CNSocialProfileServiceKey {

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


