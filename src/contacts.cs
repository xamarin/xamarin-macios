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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Contacts {

	interface ICNKeyDescriptor { }

	[MacCatalyst (13, 1)]
	[Protocol]
	// Headers say "This protocol is reserved for Contacts framework usage.", so don't create a model
	interface CNKeyDescriptor : NSObjectProtocol, NSSecureCoding, NSCopying {
	}

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForContactsMatchingEmailAddress:")]
		NSPredicate GetPredicateForContactsMatchingEmailAddress (string emailAddress);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("predicateForContactsMatchingPhoneNumber:")]
		NSPredicate GetPredicateForContacts (CNPhoneNumber phoneNumber);

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

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNFetchRequest))]
	[DisableDefaultCtor] // using init raises an exception according to docs
	interface CNContactFetchRequest : NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithKeysToFetch:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		NativeHandle Constructor (NSArray keysToFetch);

		[NullAllowed]
		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; set; }

		[Export ("keysToFetch", ArgumentSemantic.Copy)]
		// we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		// cannot be exposed as NSString since they could be internalized types, like CNAggregateKeyDescriptor
		NSArray KeysToFetch { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("mutableObjects")]
		bool MutableObjects { get; set; }

		[Export ("unifyResults")]
		bool UnifyResults { get; set; }

		[Export ("sortOrder")]
		CNContactSortOrder SortOrder { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface CNContactFormatter : NSSecureCoding {

		[Static]
		[Export ("descriptorForRequiredKeysForStyle:")]
		ICNKeyDescriptor GetDescriptorForRequiredKeys (CNContactFormatterStyle style);

		[Static]
		[return: NullAllowed]
		[Export ("stringFromContact:style:")]
		string GetStringFrom (CNContact contact, CNContactFormatterStyle style);

		[Static]
		[return: NullAllowed]
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

		[return: NullAllowed]
		[Export ("stringFromContact:")]
		string GetString (CNContact contact);

		[return: NullAllowed]
		[Export ("attributedStringFromContact:defaultAttributes:")]
		NSAttributedString GetAttributedString (CNContact contact, [NullAllowed] NSDictionary attributes);

		[Field ("CNContactPropertyAttribute")]
		NSString ContactPropertyAttribute { get; }

		[Static]
		[iOS (14, 0), Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Export ("descriptorForRequiredKeysForDelimiter")]
		ICNKeyDescriptor RequiredKeysForDelimiter { get; }

		[Static]
		[iOS (14, 0), Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Export ("descriptorForRequiredKeysForNameOrder")]
		ICNKeyDescriptor RequiredKeysForNameOrder { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNContactRelation : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Static]
		[Export ("contactRelationWithName:")]
		CNContactRelation FromName (string name);

		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		[Export ("name")]
		string Name { get; }
	}

	[MacCatalyst (13, 1)]
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

		[Field ("CNLabelContactRelationSon")]
		[MacCatalyst (13, 1)]
		NSString Son { get; }

		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationDaughter")]
		NSString Daughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationColleague")]
		NSString Colleague { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationTeacher")]
		NSString Teacher { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSibling")]
		NSString Sibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerSibling")]
		NSString YoungerSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderSibling")]
		NSString ElderSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerSister")]
		NSString YoungerSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungestSister")]
		NSString YoungestSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderSister")]
		NSString ElderSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationEldestSister")]
		NSString EldestSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerBrother")]
		NSString YoungerBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungestBrother")]
		NSString YoungestBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderBrother")]
		NSString ElderBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationEldestBrother")]
		NSString EldestBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMaleFriend")]
		NSString MaleFriend { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFemaleFriend")]
		NSString FemaleFriend { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationWife")]
		NSString Wife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationHusband")]
		NSString Husband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMalePartner")]
		NSString MalePartner { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFemalePartner")]
		NSString FemalePartner { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGirlfriendOrBoyfriend")]
		NSString GirlfriendOrBoyfriend { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGirlfriend")]
		NSString Girlfriend { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBoyfriend")]
		NSString Boyfriend { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandparent")]
		NSString Grandparent { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandmother")]
		NSString Grandmother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandmotherMothersMother")]
		NSString GrandmotherMothersMother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandmotherFathersMother")]
		NSString GrandmotherFathersMother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandfather")]
		NSString Grandfather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandfatherMothersFather")]
		NSString GrandfatherMothersFather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandfatherFathersFather")]
		NSString GrandfatherFathersFather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandparent")]
		NSString GreatGrandparent { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandmother")]
		NSString GreatGrandmother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandfather")]
		NSString GreatGrandfather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandchild")]
		NSString Grandchild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGranddaughter")]
		NSString Granddaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGranddaughterDaughtersDaughter")]
		NSString GranddaughterDaughtersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGranddaughterSonsDaughter")]
		NSString GranddaughterSonsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandson")]
		NSString Grandson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandsonDaughtersSon")]
		NSString GrandsonDaughtersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandsonSonsSon")]
		NSString GrandsonSonsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandchild")]
		NSString GreatGrandchild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGranddaughter")]
		NSString GreatGranddaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandson")]
		NSString GreatGrandson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentInLaw")]
		NSString ParentInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMotherInLaw")]
		NSString MotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMotherInLawWifesMother")]
		NSString MotherInLawWifesMother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMotherInLawHusbandsMother")]
		NSString MotherInLawHusbandsMother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFatherInLaw")]
		NSString FatherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFatherInLawWifesFather")]
		NSString FatherInLawWifesFather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFatherInLawHusbandsFather")]
		NSString FatherInLawHusbandsFather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoParentInLaw")]
		NSString CoParentInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoMotherInLaw")]
		NSString CoMotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoFatherInLaw")]
		NSString CoFatherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSiblingInLaw")]
		NSString SiblingInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerSiblingInLaw")]
		NSString YoungerSiblingInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderSiblingInLaw")]
		NSString ElderSiblingInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLaw")]
		NSString SisterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerSisterInLaw")]
		NSString YoungerSisterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderSisterInLaw")]
		NSString ElderSisterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawSpousesSister")]
		NSString SisterInLawSpousesSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawWifesSister")]
		NSString SisterInLawWifesSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawHusbandsSister")]
		NSString SisterInLawHusbandsSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawBrothersWife")]
		NSString SisterInLawBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawYoungerBrothersWife")]
		NSString SisterInLawYoungerBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawElderBrothersWife")]
		NSString SisterInLawElderBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLaw")]
		NSString BrotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerBrotherInLaw")]
		NSString YoungerBrotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderBrotherInLaw")]
		NSString ElderBrotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawSpousesBrother")]
		NSString BrotherInLawSpousesBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawHusbandsBrother")]
		NSString BrotherInLawHusbandsBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawWifesBrother")]
		NSString BrotherInLawWifesBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawSistersHusband")]
		NSString BrotherInLawSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawYoungerSistersHusband")]
		NSString BrotherInLawYoungerSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawElderSistersHusband")]
		NSString BrotherInLawElderSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawWifesBrothersWife")]
		NSString SisterInLawWifesBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSisterInLawHusbandsBrothersWife")]
		NSString SisterInLawHusbandsBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawWifesSistersHusband")]
		NSString BrotherInLawWifesSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationBrotherInLawHusbandsSistersHusband")]
		NSString BrotherInLawHusbandsSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoSiblingInLaw")]
		NSString CoSiblingInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoSisterInLaw")]
		NSString CoSisterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCoBrotherInLaw")]
		NSString CoBrotherInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationChildInLaw")]
		NSString ChildInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationDaughterInLaw")]
		NSString DaughterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSonInLaw")]
		NSString SonInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousin")]
		NSString Cousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousin")]
		NSString YoungerCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousin")]
		NSString ElderCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMaleCousin")]
		NSString MaleCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFemaleCousin")]
		NSString FemaleCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinParentsSiblingsChild")]
		NSString CousinParentsSiblingsChild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinParentsSiblingsSon")]
		NSString CousinParentsSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinParentsSiblingsSon")]
		NSString YoungerCousinParentsSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinParentsSiblingsSon")]
		NSString ElderCousinParentsSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinParentsSiblingsDaughter")]
		NSString CousinParentsSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinParentsSiblingsDaughter")]
		NSString YoungerCousinParentsSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinParentsSiblingsDaughter")]
		NSString ElderCousinParentsSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinMothersSistersDaughter")]
		NSString CousinMothersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersSistersDaughter")]
		NSString YoungerCousinMothersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersSistersDaughter")]
		NSString ElderCousinMothersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinMothersSistersSon")]
		NSString CousinMothersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersSistersSon")]
		NSString YoungerCousinMothersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersSistersSon")]
		NSString ElderCousinMothersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinMothersBrothersDaughter")]
		NSString CousinMothersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersBrothersDaughter")]
		NSString YoungerCousinMothersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersBrothersDaughter")]
		NSString ElderCousinMothersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinMothersBrothersSon")]
		NSString CousinMothersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersBrothersSon")]
		NSString YoungerCousinMothersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersBrothersSon")]
		NSString ElderCousinMothersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinFathersSistersDaughter")]
		NSString CousinFathersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinFathersSistersDaughter")]
		NSString YoungerCousinFathersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinFathersSistersDaughter")]
		NSString ElderCousinFathersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinFathersSistersSon")]
		NSString CousinFathersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinFathersSistersSon")]
		NSString YoungerCousinFathersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinFathersSistersSon")]
		NSString ElderCousinFathersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinFathersBrothersDaughter")]
		NSString CousinFathersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinFathersBrothersDaughter")]
		NSString YoungerCousinFathersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinFathersBrothersDaughter")]
		NSString ElderCousinFathersBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinFathersBrothersSon")]
		NSString CousinFathersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinFathersBrothersSon")]
		NSString YoungerCousinFathersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinFathersBrothersSon")]
		NSString ElderCousinFathersBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinGrandparentsSiblingsChild")]
		NSString CousinGrandparentsSiblingsChild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinGrandparentsSiblingsDaughter")]
		NSString CousinGrandparentsSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinGrandparentsSiblingsSon")]
		NSString CousinGrandparentsSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersSiblingsSonOrFathersSistersSon")]
		NSString YoungerCousinMothersSiblingsSonOrFathersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersSiblingsSonOrFathersSistersSon")]
		NSString ElderCousinMothersSiblingsSonOrFathersSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationYoungerCousinMothersSiblingsDaughterOrFathersSistersDaughter")]
		NSString YoungerCousinMothersSiblingsDaughterOrFathersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationElderCousinMothersSiblingsDaughterOrFathersSistersDaughter")]
		NSString ElderCousinMothersSiblingsDaughterOrFathersSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSibling")]
		NSString ParentsSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsYoungerSibling")]
		NSString ParentsYoungerSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsElderSibling")]
		NSString ParentsElderSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingMothersSibling")]
		NSString ParentsSiblingMothersSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingMothersYoungerSibling")]
		NSString ParentsSiblingMothersYoungerSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingMothersElderSibling")]
		NSString ParentsSiblingMothersElderSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingFathersSibling")]
		NSString ParentsSiblingFathersSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingFathersYoungerSibling")]
		NSString ParentsSiblingFathersYoungerSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationParentsSiblingFathersElderSibling")]
		NSString ParentsSiblingFathersElderSibling { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAunt")]
		NSString Aunt { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntParentsSister")]
		NSString AuntParentsSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntParentsYoungerSister")]
		NSString AuntParentsYoungerSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntParentsElderSister")]
		NSString AuntParentsElderSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersSister")]
		NSString AuntFathersSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersYoungerSister")]
		NSString AuntFathersYoungerSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersElderSister")]
		NSString AuntFathersElderSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersBrothersWife")]
		NSString AuntFathersBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersYoungerBrothersWife")]
		NSString AuntFathersYoungerBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntFathersElderBrothersWife")]
		NSString AuntFathersElderBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntMothersSister")]
		NSString AuntMothersSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntMothersYoungerSister")]
		NSString AuntMothersYoungerSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntMothersElderSister")]
		NSString AuntMothersElderSister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationAuntMothersBrothersWife")]
		NSString AuntMothersBrothersWife { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandaunt")]
		NSString Grandaunt { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncle")]
		NSString Uncle { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleParentsBrother")]
		NSString UncleParentsBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleParentsYoungerBrother")]
		NSString UncleParentsYoungerBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleParentsElderBrother")]
		NSString UncleParentsElderBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleMothersBrother")]
		NSString UncleMothersBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleMothersYoungerBrother")]
		NSString UncleMothersYoungerBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleMothersElderBrother")]
		NSString UncleMothersElderBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleMothersSistersHusband")]
		NSString UncleMothersSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersBrother")]
		NSString UncleFathersBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersYoungerBrother")]
		NSString UncleFathersYoungerBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersElderBrother")]
		NSString UncleFathersElderBrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersSistersHusband")]
		NSString UncleFathersSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersYoungerSistersHusband")]
		NSString UncleFathersYoungerSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationUncleFathersElderSistersHusband")]
		NSString UncleFathersElderSistersHusband { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGranduncle")]
		NSString Granduncle { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSiblingsChild")]
		NSString SiblingsChild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNiece")]
		NSString Niece { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNieceSistersDaughter")]
		NSString NieceSistersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNieceBrothersDaughter")]
		NSString NieceBrothersDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNieceSistersDaughterOrWifesSiblingsDaughter")]
		NSString NieceSistersDaughterOrWifesSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNieceBrothersDaughterOrHusbandsSiblingsDaughter")]
		NSString NieceBrothersDaughterOrHusbandsSiblingsDaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephew")]
		NSString Nephew { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephewSistersSon")]
		NSString NephewSistersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephewBrothersSon")]
		NSString NephewBrothersSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephewBrothersSonOrHusbandsSiblingsSon")]
		NSString NephewBrothersSonOrHusbandsSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephewSistersSonOrWifesSiblingsSon")]
		NSString NephewSistersSonOrWifesSiblingsSon { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandniece")]
		NSString Grandniece { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandnieceSistersGranddaughter")]
		NSString GrandnieceSistersGranddaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandnieceBrothersGranddaughter")]
		NSString GrandnieceBrothersGranddaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandnephew")]
		NSString Grandnephew { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandnephewSistersGrandson")]
		NSString GrandnephewSistersGrandson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandnephewBrothersGrandson")]
		NSString GrandnephewBrothersGrandson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepparent")]
		NSString Stepparent { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepfather")]
		NSString Stepfather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepmother")]
		NSString Stepmother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepchild")]
		NSString Stepchild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepson")]
		NSString Stepson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepdaughter")]
		NSString Stepdaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepbrother")]
		NSString Stepbrother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationStepsister")]
		NSString Stepsister { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationMotherInLawOrStepmother")]
		NSString MotherInLawOrStepmother { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationFatherInLawOrStepfather")]
		NSString FatherInLawOrStepfather { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationDaughterInLawOrStepdaughter")]
		NSString DaughterInLawOrStepdaughter { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSonInLawOrStepson")]
		NSString SonInLawOrStepson { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationCousinOrSiblingsChild")]
		NSString CousinOrSiblingsChild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNieceOrCousin")]
		NSString NieceOrCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationNephewOrCousin")]
		NSString NephewOrCousin { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGrandchildOrSiblingsChild")]
		NSString GrandchildOrSiblingsChild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationGreatGrandchildOrSiblingsGrandchild")]
		NSString GreatGrandchildOrSiblingsGrandchild { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationDaughterInLawOrSisterInLaw")]
		NSString DaughterInLawOrSisterInLaw { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelContactRelationSonInLawOrBrotherInLaw")]
		NSString SonInLawOrBrotherInLaw { get; }

		[iOS (14, 0), Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Field ("CNLabelContactRelationGranddaughterOrNiece")]
		NSString GranddaughterOrNiece { get; }

		[iOS (14, 0), Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Field ("CNLabelContactRelationGrandsonOrNephew")]
		NSString GrandsonOrNephew { get; }

	}

	delegate void CNContactStoreRequestAccessHandler (bool granted, NSError error);
#if !NET
	delegate void CNContactStoreEnumerateContactsHandler (CNContact contact, bool stop);
#endif
	delegate void CNContactStoreListContactsHandler (CNContact contact, ref bool stop);

	interface ICNChangeHistoryEventVisitor { }

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface CNChangeHistoryEventVisitor {
		[Abstract]
		[Export ("visitDropEverythingEvent:")]
		void DropEverything (CNChangeHistoryDropEverythingEvent @event);

		[Abstract]
		[Export ("visitAddContactEvent:")]
		void AddContact (CNChangeHistoryAddContactEvent @event);

		[Abstract]
		[Export ("visitUpdateContactEvent:")]
		void UpdateContact (CNChangeHistoryUpdateContactEvent @event);

		[Abstract]
		[Export ("visitDeleteContactEvent:")]
		void DeleteContact (CNChangeHistoryDeleteContactEvent @event);

		[Export ("visitAddGroupEvent:")]
		void AddGroup (CNChangeHistoryAddGroupEvent @event);

		[Export ("visitUpdateGroupEvent:")]
		void UpdateGroup (CNChangeHistoryUpdateGroupEvent @event);

		[Export ("visitDeleteGroupEvent:")]
		void DeleteGroup (CNChangeHistoryDeleteGroupEvent @event);

		[Export ("visitAddMemberToGroupEvent:")]
		void AddMemberToGroup (CNChangeHistoryAddMemberToGroupEvent @event);

		[Export ("visitRemoveMemberFromGroupEvent:")]
		void RemoveMemberFromGroup (CNChangeHistoryRemoveMemberFromGroupEvent @event);

		[Export ("visitAddSubgroupToGroupEvent:")]
		void AddSubgroupToGroup (CNChangeHistoryAddSubgroupToGroupEvent @event);

		[Export ("visitRemoveSubgroupFromGroupEvent:")]
		void RemoveSubgroupFromGroup (CNChangeHistoryRemoveSubgroupFromGroupEvent @event);
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNChangeHistoryEvent : NSCopying, NSSecureCoding {
		[Export ("acceptEventVisitor:")]
		void AcceptEventVisitor (ICNChangeHistoryEventVisitor visitor);
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	interface CNChangeHistoryDropEverythingEvent { }

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryAddContactEvent {
		[Export ("contact", ArgumentSemantic.Strong)]
		CNContact Contact { get; }

		[NullAllowed, Export ("containerIdentifier", ArgumentSemantic.Strong)]
		string ContainerIdentifier { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryUpdateContactEvent {
		[Export ("contact", ArgumentSemantic.Strong)]
		CNContact Contact { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryDeleteContactEvent {
		[Export ("contactIdentifier", ArgumentSemantic.Strong)]
		string ContactIdentifier { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryAddGroupEvent {
		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }

		[Export ("containerIdentifier", ArgumentSemantic.Strong)]
		string ContainerIdentifier { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryUpdateGroupEvent {
		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryDeleteGroupEvent {
		[Export ("groupIdentifier", ArgumentSemantic.Strong)]
		string GroupIdentifier { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryAddMemberToGroupEvent {
		[Export ("member", ArgumentSemantic.Strong)]
		CNContact Member { get; }

		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryRemoveMemberFromGroupEvent {
		[Export ("member", ArgumentSemantic.Strong)]
		CNContact Member { get; }

		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryAddSubgroupToGroupEvent {
		[Export ("subgroup", ArgumentSemantic.Strong)]
		CNGroup Subgroup { get; }

		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNChangeHistoryEvent))]
	[DisableDefaultCtor]
	interface CNChangeHistoryRemoveSubgroupFromGroupEvent {
		[Export ("subgroup", ArgumentSemantic.Strong)]
		CNGroup Subgroup { get; }

		[Export ("group", ArgumentSemantic.Strong)]
		CNGroup Group { get; }
	}

	// this type is new in Xcode11 but is decorated with earlier versions since it's used as a
	// base type for older types (and that confuse the generator for 32bits availability)
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNFetchRequest { }

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNFetchRequest))]
	interface CNChangeHistoryFetchRequest : NSSecureCoding {
		[NullAllowed, Export ("startingToken", ArgumentSemantic.Copy)]
		NSData StartingToken { get; set; }

		[NullAllowed, Export ("additionalContactKeyDescriptors", ArgumentSemantic.Copy)]
		ICNKeyDescriptor [] AdditionalContactKeyDescriptors { get; set; }

		[Export ("shouldUnifyResults")]
		bool ShouldUnifyResults { get; set; }

		[Export ("mutableObjects")]
		bool MutableObjects { get; set; }

		[Export ("includeGroupChanges")]
		bool IncludeGroupChanges { get; set; }

		[NullAllowed]
		[Export ("excludedTransactionAuthors", ArgumentSemantic.Copy)]
		string [] ExcludedTransactionAuthors { get; set; }
	}

	[Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNFetchResult<T> {
		[Export ("value", ArgumentSemantic.Strong)]
		NSObject Value { get; }

		[Export ("currentHistoryToken", ArgumentSemantic.Copy)]
		NSData CurrentHistoryToken { get; }
	}

	[MacCatalyst (13, 1)]
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
		[return: NullAllowed]
		CNContact [] GetUnifiedContacts (NSPredicate predicate, NSArray keys, [NullAllowed] out NSError error);

		[Export ("unifiedContactWithIdentifier:keysToFetch:error:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		[return: NullAllowed]
		CNContact GetUnifiedContact (string identifier, NSArray keys, [NullAllowed] out NSError error);

		[NoiOS, NoWatch]
		[NoMacCatalyst]
		[Export ("unifiedMeContactWithKeysToFetch:error:")]
		[Protected] // we cannot use ICNKeyDescriptor as Apple (and others) can adopt it from categories
		[return: NullAllowed]
		NSObject GetUnifiedMeContact (NSArray keys, [NullAllowed] out NSError error);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("enumeratorForContactFetchRequest:error:")]
		[return: NullAllowed]
		CNFetchResult<NSEnumerator<CNContact>> GetEnumeratorForContact (CNContactFetchRequest request, [NullAllowed] out NSError error);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("enumeratorForChangeHistoryFetchRequest:error:")]
		[return: NullAllowed]
		CNFetchResult<NSEnumerator<CNChangeHistoryEvent>> GetEnumeratorForChangeHistory (CNChangeHistoryFetchRequest request, [NullAllowed] out NSError error);


#if !NET && !WATCH
		[Obsolete ("Use the overload that takes 'CNContactStoreListContactsHandler' instead.")]
		[Export ("enumerateContactsWithFetchRequest:error:usingBlock:")]
		bool EnumerateContacts (CNContactFetchRequest fetchRequest, out NSError error, CNContactStoreEnumerateContactsHandler handler);

		[Sealed]
#endif
		[Export ("enumerateContactsWithFetchRequest:error:usingBlock:")]
		bool EnumerateContacts (CNContactFetchRequest fetchRequest, [NullAllowed] out NSError error, CNContactStoreListContactsHandler handler);

		[Export ("groupsMatchingPredicate:error:")]
		[return: NullAllowed]
		CNGroup [] GetGroups ([NullAllowed] NSPredicate predicate, [NullAllowed] out NSError error);

		[Export ("containersMatchingPredicate:error:")]
		[return: NullAllowed]
		CNContainer [] GetContainers ([NullAllowed] NSPredicate predicate, [NullAllowed] out NSError error);

#if !WATCH
		[Export ("executeSaveRequest:error:")]
		[return: NullAllowed]
		bool ExecuteSaveRequest (CNSaveRequest saveRequest, [NullAllowed] out NSError error);
#endif
		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("currentHistoryToken", ArgumentSemantic.Copy)]
		NSData CurrentHistoryToken { get; }

		[Export ("defaultContainerIdentifier")]
		[NullAllowed]
		string DefaultContainerIdentifier { get; }

		[Notification]
		[Field ("CNContactStoreDidChangeNotification")]
		NSString NotificationDidChange { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNContactVCardSerialization {

		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor GetDescriptorFromRequiredKeys ();

		[Static]
		[return: NullAllowed]
		[Export ("dataWithContacts:error:")]
		NSData GetDataFromContacts (CNContact [] contacts, out NSError error);

		[Static]
		[return: NullAllowed]
		[Export ("contactsWithData:error:")]
		CNContact [] GetContactsFromData (NSData data, out NSError error);
	}

#if !NET
#pragma warning disable 0618 // warning CS0618: 'CategoryAttribute.CategoryAttribute(bool)' is obsolete: 'Inline the static members in this category in the category's class (and remove this obsolete once fixed)'
	[Category (allowStaticMembers: true)]
#pragma warning disable
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

	[MacCatalyst (13, 1)]
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
#if NET
		[Export ("predicateForContainersWithIdentifiers:")]
#else
		[Wrap ("CNContainer_PredicatesExtension.GetPredicateForContainers (null!, identifiers)")]
#endif
		NSPredicate CreatePredicateForContainers (string [] identifiers);

		[Static]
#if NET
		[Export ("predicateForContainerOfContactWithIdentifier:")]
#else
		[Wrap ("CNContainer_PredicatesExtension.GetPredicateForContainerOfContact (null!, contactIdentifier)")]
#endif
		NSPredicate CreatePredicateForContainerOfContact (string contactIdentifier);

		[Static]
#if NET
		[Export ("predicateForContainerOfGroupWithIdentifier:")]
#else
		[Wrap ("CNContainer_PredicatesExtension.GetPredicateForContainerOfGroup (null!, groupIdentifier)")]
#endif
		NSPredicate CreatePredicateForContainerOfGroup (string groupIdentifier);
		#endregion
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

#if !NET
#pragma warning disable 0618 // warning CS0618: 'CategoryAttribute.CategoryAttribute(bool)' is obsolete: 'Inline the static members in this category in the category's class (and remove this obsolete once fixed)'
	[Category (allowStaticMembers: true)]
#pragma warning disable
	[BaseType (typeof (CNGroup))]
	interface CNGroup_PredicatesExtension {

		[Obsolete ("Use 'CNGroup.CreatePredicateForGroups' instead.")]
		[Static]
		[Export ("predicateForGroupsWithIdentifiers:")]
		NSPredicate GetPredicateForGroups (string [] identifiers);

		[Obsolete ("Use 'CNGroup.CreatePredicateForSubgroupsInGroup' instead.")]
		[NoiOS]
		[NoWatch]
		[Static]
		[Export ("predicateForSubgroupsInGroupWithIdentifier:")]
		NSPredicate GetPredicateForSubgroupsInGroup (string parentGroupIdentifier);

		[Obsolete ("Use 'CNGroup.CreatePredicateForGroupsInContainer' instead.")]
		[Static]
		[Export ("predicateForGroupsInContainerWithIdentifier:")]
		NSPredicate GetPredicateForGroupsInContainer (string containerIdentifier);
	}
#endif

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNGroup : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("name")]
		string Name { get; }

		#region comes from CNGroup (Predicates) Category
		[Static]
#if NET
		[Export ("predicateForGroupsWithIdentifiers:")]
#else
		[Wrap ("CNGroup_PredicatesExtension.GetPredicateForGroups (null!, identifiers)")]
#endif
		NSPredicate CreatePredicateForGroups (string [] identifiers);

		[NoiOS]
		[NoWatch]
		[NoMacCatalyst]
		[Static]
#if NET
		[Export ("predicateForSubgroupsInGroupWithIdentifier:")]
#else
		[Wrap ("CNGroup_PredicatesExtension.GetPredicateForSubgroupsInGroup (null!, parentGroupIdentifier)")]
#endif
		NSPredicate CreatePredicateForSubgroupsInGroup (string parentGroupIdentifier);

		[Static]
#if NET
		[Export ("predicateForGroupsInContainerWithIdentifier:")]
#else
		[Wrap ("CNGroup_PredicatesExtension.GetPredicateForGroupsInContainer (null!, containerIdentifier)")]
#endif
		NSPredicate CreatePredicateForGroupsInContainer (string containerIdentifier);
		#endregion
	}

	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNGroupKey { // Can be used in KVO

		[Field ("CNGroupIdentifierKey")]
		NSString Identifier { get; }

		[Field ("CNGroupNameKey")]
		NSString Name { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNInstantMessageAddress : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("initWithUsername:service:")]
		NativeHandle Constructor (string username, string service);

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

	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNInstantMessageAddressKey { // Can be used in KVO

		[Field ("CNInstantMessageAddressUsernameKey")]
		NSString Username { get; }

		[Field ("CNInstantMessageAddressServiceKey")]
		NSString Service { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNLabeledValue<ValueType> : NSCopying, NSSecureCoding
		where ValueType : INSCopying, INSSecureCoding {

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
		NativeHandle Constructor ([NullAllowed] string label, ValueType value);

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

	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNLabelKey {

		[Field ("CNLabelHome")]
		NSString Home { get; }

		[Field ("CNLabelWork")]
		NSString Work { get; }

		[iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("CNLabelSchool")]
		NSString School { get; }

		[Field ("CNLabelOther")]
		NSString Other { get; }

		[Field ("CNLabelEmailiCloud")]
		NSString EmailiCloud { get; }

		[Field ("CNLabelURLAddressHomePage")]
		NSString UrlAddressHomePage { get; }

		[Field ("CNLabelDateAnniversary")]
		NSString DateAnniversary { get; }
	}

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNGroup))]
	interface CNMutableGroup {

		[New]
		[Export ("name")]
		string Name { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (CNPostalAddress))]
	interface CNMutablePostalAddress {

		[New]
		[Export ("street")]
		string Street { get; set; }

		[MacCatalyst (13, 1)]
		[New]
		[Export ("subLocality")]
		string SubLocality { get; set; }

		[New]
		[Export ("city")]
		string City { get; set; }

		[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Apple doc: no handle (nil) if no string (or nil string) is given
	interface CNPhoneNumber : NSCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Static, Export ("phoneNumberWithStringValue:")]
		[return: NullAllowed]
		CNPhoneNumber PhoneNumberWithStringValue (string stringValue);

		[Export ("initWithStringValue:")]
		NativeHandle Constructor (string stringValue);

		[Export ("stringValue")]
		string StringValue { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNLabelPhoneNumberKey {

		[Field ("CNLabelPhoneNumberiPhone")]
		NSString iPhone { get; }

		[Watch (7, 2), iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("CNLabelPhoneNumberAppleWatch")]
		NSString AppleWatch { get; }

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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CNPostalAddress : NSCopying, NSMutableCopying, NSSecureCoding, INSCopying, INSSecureCoding {

		[Export ("street")]
		string Street { get; }

		[MacCatalyst (13, 1)]
		[Export ("subLocality")]
		string SubLocality { get; }

		[Export ("city")]
		string City { get; }

		[MacCatalyst (13, 1)]
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
		[Wrap ("LocalizeProperty (option.GetConstant ()!)")]
		string LocalizeProperty (CNPostalAddressKeyOption option);
	}

#if !NET
	[Static]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	interface CNPostalAddressKey { // Can be used in KVO

		[Field ("CNPostalAddressStreetKey")]
		NSString Street { get; }

		[Field ("CNPostalAddressSubLocalityKey")]
		NSString SubLocality { get; }

		[Field ("CNPostalAddressCityKey")]
		NSString City { get; }

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

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("CNPostalAddressSubLocalityKey")]
		SubLocality,

		[MacCatalyst (13, 1)]
		[Field ("CNPostalAddressSubAdministrativeAreaKey")]
		SubAdministrativeArea,
	}

	[MacCatalyst (13, 1)]
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
	[MacCatalyst (13, 1)]
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
		[NoMacCatalyst]
		[Export ("addSubgroup:toGroup:")]
		void AddSubgroup (CNGroup subgroup, CNGroup group);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("removeSubgroup:fromGroup:")]
		void RemoveSubgroup (CNGroup subgroup, CNGroup group);

		[Export ("addMember:toGroup:")]
		void AddMember (CNContact contact, CNGroup group);

		[Export ("removeMember:fromGroup:")]
		void RemoveMember (CNContact contact, CNGroup group);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("transactionAuthor")]
		string TransactionAuthor { get; set; }

		[Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("shouldRefetchContacts")]
		bool ShouldRefetchContacts { get; set; }
	}
#endif // !WATCH

	[MacCatalyst (13, 1)]
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
		NativeHandle Constructor ([NullAllowed] string url, [NullAllowed] string username, [NullAllowed] string userIdentifier, [NullAllowed] string service);

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("localizedStringForKey:")]
		string LocalizeProperty (NSString key);

		[Static]
		[Wrap ("LocalizeProperty (key.GetConstant ()!)")]
		string LocalizeProperty (CNPostalAddressKeyOption key);

		[Static]
		[Export ("localizedStringForService:")]
		string LocalizeService (NSString service);
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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
}
