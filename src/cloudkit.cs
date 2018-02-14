using System;
using ObjCRuntime;
using Foundation;
using CoreLocation;
#if !TVOS
using Contacts;
#endif

namespace CloudKit {

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKAsset initWithFileURL:] or -[CKAsset initWithData:]
	[BaseType (typeof (NSObject))]
	interface CKAsset : NSCoding, NSSecureCoding, CKRecordValue {

		[Export ("initWithFileURL:")]
		IntPtr Constructor (NSUrl fileUrl);

		[Export ("fileURL", ArgumentSemantic.Copy)]
		NSUrl FileUrl { get; }
	}
	
	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CKUserIdentityLookupInfo : NSSecureCoding, NSCopying
	{
		[Internal, Export ("initWithEmailAddress:")]
		IntPtr _FromEmail (string emailAddress);

		[Internal, Export ("initWithPhoneNumber:")]
		IntPtr _FromPhoneNumber (string phoneNumber);

		[Export ("initWithUserRecordID:")]
		CKUserIdentityLookupInfo Constructor (CKRecordID userRecordID);

		[Static]
		[Export ("lookupInfosWithEmails:")]
		CKUserIdentityLookupInfo[] GetLookupInfosWithEmails (string[] emails);

		[Static]
		[Export ("lookupInfosWithPhoneNumbers:")]
		CKUserIdentityLookupInfo[] GetLookupInfosWithPhoneNumbers (string[] phoneNumbers);

		[Static]
		[Export ("lookupInfosWithRecordIDs:")]
		CKUserIdentityLookupInfo[] GetLookupInfos (CKRecordID[] recordIDs);

		[NullAllowed, Export ("emailAddress")]
		string EmailAddress { get; }

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; }

		[NullAllowed, Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordID { get; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CKUserIdentity : NSSecureCoding, NSCopying
	{
		[NullAllowed, Export ("lookupInfo", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo LookupInfo { get; }

		[NullAllowed, Export ("nameComponents", ArgumentSemantic.Copy)]
		NSPersonNameComponents NameComponents { get; }

		[NullAllowed, Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordID { get; }

		[Export ("hasiCloudAccount")]
		bool HasICloudAccount { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[Export ("contactIdentifiers", ArgumentSemantic.Copy)]
		string[] ContactIdentifiers { get; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface CKShareMetadata : NSCopying, NSSecureCoding
	{
		[Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("share", ArgumentSemantic.Strong)]
		CKShare Share { get; }

		[Export ("rootRecordID", ArgumentSemantic.Copy)]
		CKRecordID RootRecordID { get; }

		[Export ("participantType", ArgumentSemantic.Assign)]
		CKShareParticipantType Type { get; }

		[Export ("participantStatus", ArgumentSemantic.Assign)]
		CKShareParticipantAcceptanceStatus Status { get; }

		[Export ("participantPermission", ArgumentSemantic.Assign)]
		CKShareParticipantPermission Permission { get; }

		[Export ("ownerIdentity", ArgumentSemantic.Strong)]
		CKUserIdentity OwnerIdentity { get; }

		[NullAllowed, Export ("rootRecord", ArgumentSemantic.Strong)]
		CKRecord RootRecord { get; }
	}
	
	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKRecord))]
	[DisableDefaultCtor]
	interface CKShare
	{
		[Export ("initWithRootRecord:")]
		IntPtr Constructor (CKRecord rootRecord);

		[Export ("initWithRootRecord:shareID:")]
		[DesignatedInitializer]
		IntPtr Constructor (CKRecord rootRecord, CKRecordID shareID);

		[Export ("publicPermission", ArgumentSemantic.Assign)]
		CKShareParticipantPermission PublicPermission { get; set; }

		[NullAllowed]
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("participants", ArgumentSemantic.Strong)]
		CKShareParticipant[] Participants { get; }

		[Export ("owner", ArgumentSemantic.Strong)]
		CKShareParticipant Owner { get; }

		[NullAllowed, Export ("currentUserParticipant", ArgumentSemantic.Strong)]
		CKShareParticipant CurrentUser { get; }

		[Export ("addParticipant:")]
		void Add (CKShareParticipant participant);

		[Export ("removeParticipant:")]
		void Remove (CKShareParticipant participant);
	}

	[Static]
	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	partial interface CKShareKeys {
		
		[Field ("CKShareTitleKey")]
		NSString Title { get; }

		[Field ("CKShareThumbnailImageDataKey")]
		NSString ThumbnailImageData { get; }

		[Field ("CKShareTypeKey")]
		NSString Type { get; }
	}
	
	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface CKShareParticipant : NSSecureCoding, NSCopying
	{
		[Export ("userIdentity", ArgumentSemantic.Strong)]
		CKUserIdentity UserIdentity { get; }

		[Export ("type", ArgumentSemantic.Assign)]
		CKShareParticipantType Type { get; set; }

		[Export ("acceptanceStatus", ArgumentSemantic.Assign)]
		CKShareParticipantAcceptanceStatus AcceptanceStatus { get; }

		[Export ("permission", ArgumentSemantic.Assign)]
		CKShareParticipantPermission Permission { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use +[CKContainer privateCloudDatabase] or +[CKContainer publicCloudDatabase] instead of creating your own
	[BaseType (typeof (NSObject))]
	interface CKContainer {

		[NoWatch]
		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CurrentUserDefaultName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CurrentUserDefaultName' instead.")]
		[Field ("CKOwnerDefaultName")]
		NSString OwnerDefaultName { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Field ("CKCurrentUserDefaultName")]
		NSString CurrentUserDefaultName { get; }

		[Static]
		[Export ("defaultContainer")]
		CKContainer DefaultContainer { get; }

		[Static]
		[Export ("containerWithIdentifier:")]
		CKContainer FromIdentifier (string containerIdentifier);

		[NullAllowed, Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("addOperation:")]
		void AddOperation (CKOperation operation);

		[Export ("privateCloudDatabase")]
		CKDatabase PrivateCloudDatabase { get; }

		[Export ("publicCloudDatabase")]
		CKDatabase PublicCloudDatabase { get; }

		[iOS (10, 0)][Mac (10,12)]
		[Export ("sharedCloudDatabase")]
		CKDatabase SharedCloudDatabase { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("databaseWithDatabaseScope:")]
		CKDatabase GetDatabase (CKDatabaseScope databaseScope);

		[Export ("accountStatusWithCompletionHandler:")]
		[Async]
		void GetAccountStatus (Action<CKAccountStatus,NSError> completionHandler);

		[Export ("statusForApplicationPermission:completionHandler:")]
		[Async]
		void StatusForApplicationPermission (CKApplicationPermissions applicationPermission, Action<CKApplicationPermissionStatus,NSError> completionHandler);

		[Export ("requestApplicationPermission:completionHandler:")]
		[Async]
		void RequestApplicationPermission (CKApplicationPermissions applicationPermission, Action<CKApplicationPermissionStatus,NSError> completionHandler);

		[Export ("fetchUserRecordIDWithCompletionHandler:")]
		[Async]
		void FetchUserRecordId (Action<CKRecordID, NSError> completionHandler);

		[iOS (10,0)][Mac (10,12)]
		[NoTV]
		[Export ("discoverAllIdentitiesWithCompletionHandler:")]
		[Async]
		void DiscoverAllIdentities (Action<CKUserIdentity[], NSError> completionHandler);
		
		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'DiscoverAllIdentities' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'DiscoverAllIdentities' instead.")]
		[NoWatch]
		[NoTV]
		[Export ("discoverAllContactUserInfosWithCompletionHandler:")]
		[Async]
		void DiscoverAllContactUserInfos (Action<CKDiscoveredUserInfo[], NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("discoverUserIdentityWithEmailAddress:completionHandler:")]
		[Async]
		void DiscoverUserIdentityWithEmailAddress (string email, Action<CKUserIdentity, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("discoverUserIdentityWithPhoneNumber:completionHandler:")]
		[Async]
		void DiscoverUserIdentityWithPhoneNumber (string phoneNumber, Action<CKUserIdentity, NSError> completionHandler);
		
		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'DiscoverUserIdentityWithEmailAddress' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'DiscoverUserIdentityWithEmailAddress' instead.")]
		[NoWatch]
		[Export ("discoverUserInfoWithEmailAddress:completionHandler:")]
		[Async]
		void DiscoverUserInfo (string email, Action<CKDiscoveredUserInfo, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("discoverUserIdentityWithUserRecordID:completionHandler:")]
		[Async]
		void DiscoverUserIdentity (CKRecordID userRecordID, Action<CKUserIdentity, NSError> completionHandler);
	
		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'DiscoverUserIdentity' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'DiscoverUserIdentity' instead.")]
		[NoWatch]
		[Export ("discoverUserInfoWithUserRecordID:completionHandler:")]
		[Async]
		void DiscoverUserInfo (CKRecordID userRecordId, Action<CKDiscoveredUserInfo, NSError> completionHandler);

		[iOS (9,0)][Mac (10,11)]
		[Field ("CKAccountChangedNotification")]
		[Notification]
		NSString AccountChangedNotification { get; }

		[iOS (9,3)][Mac (10,11,4)]
		[NoTV] // does not answer on devices
		[Export ("fetchAllLongLivedOperationIDsWithCompletionHandler:")]
		[Async]
		void FetchAllLongLivedOperationIDs (Action<NSDictionary<NSString,NSOperation>, NSError> completionHandler);

		[iOS (9,3)][Mac (10,11,4)]
		[NoTV] // does not answer on devices
		[Export ("fetchLongLivedOperationWithID:completionHandler:")]
		[Async]
		void FetchLongLivedOperation (string[] operationID, Action<NSDictionary<NSString,NSOperation>, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("fetchShareParticipantWithEmailAddress:completionHandler:")]
		[Async]
		void FetchShareParticipantWithEmailAddress (string emailAddress, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("fetchShareParticipantWithPhoneNumber:completionHandler:")]
		[Async]
		void FetchShareParticipantWithPhoneNumber (string phoneNumber, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("fetchShareParticipantWithUserRecordID:completionHandler:")]
		[Async]
		void FetchShareParticipant (CKRecordID userRecordID, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("fetchShareMetadataWithURL:completionHandler:")]
		[Async]
		void FetchShareMetadata (NSUrl url, Action<CKShareMetadata, NSError> completionHandler);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("acceptShareMetadata:completionHandler:")]
		[Async]
		void AcceptShareMetadata (CKShareMetadata metadata, Action<CKShare, NSError> completionHandler);
	}

	delegate void CKDatabaseDeleteSubscriptionHandler (string subscriptionId, NSError error);

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // *** Assertion failure in -[CKDatabase init]
	[BaseType (typeof (NSObject))]
	interface CKDatabase {
		[Export ("addOperation:")]
		void AddOperation (CKDatabaseOperation operation);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }

		[Export ("fetchRecordWithID:completionHandler:")]
		[Async]
		void FetchRecord (CKRecordID recordId, Action<CKRecord, NSError> completionHandler);

		[Export ("saveRecord:completionHandler:")]
		[Async]
		void SaveRecord (CKRecord record, Action<CKRecord, NSError> completionHandler);

		[Export ("deleteRecordWithID:completionHandler:")]
		[Async]
		void DeleteRecord (CKRecordID recordId, Action<CKRecordID, NSError> completionHandler);

		[Export ("performQuery:inZoneWithID:completionHandler:")]
		[Async]
		void PerformQuery (CKQuery query, [NullAllowed] CKRecordZoneID zoneId, Action<CKRecord[], NSError> completionHandler);

		[Export ("fetchAllRecordZonesWithCompletionHandler:")]
		[Async]
		void FetchAllRecordZones (Action<CKRecordZone[], NSError> completionHandler);

		[Export ("fetchRecordZoneWithID:completionHandler:")]
		[Async]
		void FetchRecordZone (CKRecordZoneID zoneId, Action<CKRecordZone, NSError> completionHandler);

		[Export ("saveRecordZone:completionHandler:")]
		[Async]
		void SaveRecordZone (CKRecordZone zone, Action<CKRecordZone, NSError> completionHandler);

		[Export ("deleteRecordZoneWithID:completionHandler:")]
		[Async]
		void DeleteRecordZone (CKRecordZoneID zoneId, Action<CKRecordZoneID, NSError> completionHandler);

		[Export ("fetchSubscriptionWithID:completionHandler:")]
		[NoWatch]
		[Async]
		void FetchSubscription (string subscriptionId, Action<CKSubscription, NSError> completionHandler);

		[NoWatch]
		[Export ("fetchAllSubscriptionsWithCompletionHandler:")]
		[Async]
		void FetchAllSubscriptions (Action<CKSubscription[], NSError> completionHandler);

		[NoWatch]
		[Export ("saveSubscription:completionHandler:")]
		[Async]
		void SaveSubscription (CKSubscription subscription, Action<CKSubscription, NSError> completionHandler);

		[Export ("deleteSubscriptionWithID:completionHandler:")]
		[NoWatch]
		[Async]
		void DeleteSubscription (string subscriptionID, CKDatabaseDeleteSubscriptionHandler completionHandler);
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor]
#if XAMCORE_4_0 || WATCH
	[Abstract] // as per docs
#endif
	interface CKDatabaseOperation {

		[Export ("database", ArgumentSemantic.Retain)] [NullAllowed]
		CKDatabase Database { get; set; }
	}

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverAllContactsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[NullAllowed] // by default this property is null
		[Export ("discoverAllContactsCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKDiscoveredUserInfo[], NSError> DiscoverAllContactsHandler { get; set; }
	}

	[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CKUserIdentity' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CKUserIdentity' instead.")]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface CKDiscoveredUserInfo : NSCoding, NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordId { get; }

		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'DisplayContact.GivenName'.")]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'DisplayContact.GivenName'.")]
		[Export ("firstName", ArgumentSemantic.Copy)]
		string FirstName { get; }

		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'DisplayContact.FamilyName'.")]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'DisplayContact.FamilyName'.")]
		[Export ("lastName", ArgumentSemantic.Copy)]
		string LastName { get; }


#if XAMCORE_2_0 // The Contacts framework (CNContact) uses generics heavily, which is only supported in Unified (for now at least)
#if MONOMAC || IOS
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("displayContact", ArgumentSemantic.Copy)]
		CNContact DisplayContact { get; }
#endif // MONOMAC || IOS
#endif // XAMCORE_2_0
	}

	[NoWatch]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKDiscoverUserInfosCompletionHandler (NSDictionary emailsToUserInfos, NSDictionary userRecordIdsToUserInfos, NSError operationError);

	[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CKDiscoverUserIdentitiesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CKDiscoverUserIdentitiesOperation' instead.")]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverUserInfosOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithEmailAddresses:userRecordIDs:")]
		IntPtr Constructor (string [] emailAddresses, CKRecordID [] userRecordIDs);

		[NullAllowed] // by default this property is null
		[Export ("emailAddresses", ArgumentSemantic.Copy)]
		string [] EmailAddresses { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("userRecordIDs", ArgumentSemantic.Copy)]
		CKRecordID [] UserRecordIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("discoverUserInfosCompletionBlock", ArgumentSemantic.Copy)]
		CKDiscoverUserInfosCompletionHandler Completed {
			get;
			set; 
		}
	}

	// CKError.h Fields
	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Static]
	interface CKErrorFields {
#if !XAMCORE_3_0
		// now exposed with the corresponding CKErrorCode enum
		[Field ("CKErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Field ("CKPartialErrorsByItemIDKey")]
		NSString PartialErrorsByItemIdKey { get; }

		[Field ("CKRecordChangedErrorAncestorRecordKey")]
		NSString RecordChangedErrorAncestorRecordKey { get; }

		[Field ("CKRecordChangedErrorServerRecordKey")]
		NSString RecordChangedErrorServerRecordKey { get; }

		[Field ("CKRecordChangedErrorClientRecordKey")]
		NSString RecordChangedErrorClientRecordKey { get; }

		[Field ("CKErrorRetryAfterKey")]
		NSString ErrorRetryAfterKey { get; }
	}

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[DisableDefaultCtor] // designated
	interface CKFetchNotificationChangesOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithPreviousServerChangeToken:")]
		IntPtr Constructor ([NullAllowed] CKServerChangeToken previousServerChangeToken);

		[NullAllowed] // by default this property is null
		[Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit")]
		nuint ResultsLimit { get; set; }

		[Export ("moreComing")]
		bool MoreComing { get; }

		[NullAllowed] // by default this property is null
		[Export ("notificationChangedBlock", ArgumentSemantic.Copy)]
		Action<CKNotification> NotificationChanged {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("fetchNotificationChangesCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKServerChangeToken, NSError> Completed {
			get;
			set; 
		}
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: CKException Reason: You can't call init on CKServerChangeToken
	[BaseType (typeof (NSObject))]
	interface CKServerChangeToken : NSCopying, NSSecureCoding {
	
	}

	[NoWatch]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchRecordChangesHandler (CKServerChangeToken serverChangeToken, NSData clientChangeTokenData, NSError operationError);

	[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchRecordChangesOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithRecordZoneID:previousServerChangeToken:")]
		IntPtr Constructor (CKRecordZoneID recordZoneID, [NullAllowed] CKServerChangeToken previousServerChangeToken);

		[NullAllowed] // by default this property is null
		[Export ("recordZoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID RecordZoneId { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit", ArgumentSemantic.UnsafeUnretained)]
		nuint ResultsLimit { get; set; }

		[Export ("desiredKeys", ArgumentSemantic.Copy)] [NullAllowed]
		string [] DesiredKeys { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordChangedBlock", ArgumentSemantic.Copy)]
		Action<CKRecord> RecordChanged {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("recordWithIDWasDeletedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID> RecordDeleted {
			get;
			set; 
		}

		[Export ("moreComing")]
		bool MoreComing { get; }

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordChangesCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchRecordChangesHandler AllChangesReported {
			get;
			set; 
		}
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10, 12, onlyOn64 : true)]
	delegate void CKFetchRecordZoneChangesWithIDWasDeletedHandler (CKRecordID recordID, NSString recordType);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void CKFetchRecordZoneChangesTokensUpdatedHandler (CKRecordZoneID recordZoneID, CKServerChangeToken serverChangeToken, NSData clientChangeTokenData);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void CKFetchRecordZoneChangesFetchCompletedHandler (CKRecordZoneID recordZoneID, CKServerChangeToken serverChangeToken, NSData clientChangeTokenData, bool moreComing, NSError recordZoneError);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchRecordZoneChangesOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithRecordZoneIDs:optionsByRecordZoneID:")]
		IntPtr Constructor (CKRecordZoneID[] recordZoneIDs, [NullAllowed] NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesOptions> optionsByRecordZoneID);

		[NullAllowed]
		[Export ("recordZoneIDs", ArgumentSemantic.Copy)]
		CKRecordZoneID[] RecordZoneIDs { get; set; }

		[NullAllowed, Export ("optionsByRecordZoneID", ArgumentSemantic.Copy)]
		NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesOptions> OptionsByRecordZoneID { get; set; }

		[Export ("fetchAllChanges")]
		bool FetchAllChanges { get; set; }

		[NullAllowed, Export ("recordChangedBlock", ArgumentSemantic.Copy)]
		Action<CKRecord> RecordChanged { get; set; }

		[NullAllowed, Export ("recordWithIDWasDeletedBlock", ArgumentSemantic.Copy)]
		CKFetchRecordZoneChangesWithIDWasDeletedHandler RecordWithIDWasDeleted { get; set; }

		[NullAllowed, Export ("recordZoneChangeTokensUpdatedBlock", ArgumentSemantic.Copy)]
		CKFetchRecordZoneChangesTokensUpdatedHandler RecordZoneChangeTokensUpdated { get; set; }

		[NullAllowed, Export ("recordZoneFetchCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchRecordZoneChangesFetchCompletedHandler FetchCompleted { get; set; }

		[NullAllowed, Export ("fetchRecordZoneChangesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> ChangesCompleted { get; set; }
	}
	
	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface CKFetchRecordZoneChangesOptions : NSSecureCoding, NSCopying
	{
		[NullAllowed, Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit")]
		nuint ResultsLimit { get; set; }

		[NullAllowed, Export ("desiredKeys", ArgumentSemantic.Copy)]
		string[] DesiredKeys { get; set; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchRecordsCompletedHandler (NSDictionary recordsByRecordId, NSError error);

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // designated
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchRecordsOperation {

#if !WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();
#endif

		[Export ("initWithRecordIDs:")]
		IntPtr Constructor (CKRecordID [] recordIds);

		[NullAllowed] // by default this property is null
		[Export ("recordIDs", ArgumentSemantic.Copy)]
		CKRecordID [] RecordIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("perRecordProgressBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID,double> PerRecordProgress {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("perRecordCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKRecord,CKRecordID,NSError> PerRecordCompletion {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordsCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchRecordsCompletedHandler Completed {
			get;
			set; 
		}

		[Static]
		[Export ("fetchCurrentUserRecordOperation")]
		CKFetchRecordsOperation FetchCurrentUserRecordOperation ();
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKRecordZoneCompleteHandler (NSDictionary recordZonesByZoneId, NSError operationError);

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchRecordZonesOperation {

#if !WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();
#endif

		[Export ("initWithRecordZoneIDs:")]
		IntPtr Constructor (CKRecordZoneID [] zoneIds);

		[NullAllowed] // by default this property is null
		[Export ("recordZoneIDs", ArgumentSemantic.Copy)]
		CKRecordZoneID [] RecordZoneIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordZonesCompletionBlock", ArgumentSemantic.Copy)]
		CKRecordZoneCompleteHandler Completed {
			get;
			set; 
		}

		[Static]
		[Export ("fetchAllRecordZonesOperation")]
		CKFetchRecordZonesOperation FetchAllRecordZonesOperation ();
	}

	[NoWatch]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchSubscriptionsCompleteHandler (NSDictionary subscriptionsBySubscriptionId, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchSubscriptionsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithSubscriptionIDs:")]
		IntPtr Constructor (string [] subscriptionIds);

		[NullAllowed] // by default this property is null
		[Export ("subscriptionIDs", ArgumentSemantic.Copy)]
		string [] SubscriptionIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fetchSubscriptionCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchSubscriptionsCompleteHandler Completed {
			get;
			set; 
		}

		[Static]
		[Export ("fetchAllSubscriptionsOperation")]
		CKFetchSubscriptionsOperation FetchAllSubscriptionsOperation ();
	}

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
#if XAMCORE_4_0 || WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
	[DisableDefaultCtor]
#endif
	[BaseType (typeof (NSSortDescriptor))]
	interface CKLocationSortDescriptor : NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithKey:relativeLocation:")]
		IntPtr Constructor (string key, CLLocation relativeLocation);

		[Export ("relativeLocation", ArgumentSemantic.Copy)]
		CLLocation RelativeLocation { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKMarkNotificationsReadHandler (CKNotificationID[] notificationIDsMarkedRead, NSError operationError);

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKMarkNotificationsReadOperation initWithNotificationIDsToMarkRead:]
	[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	interface CKMarkNotificationsReadOperation {

		[DesignatedInitializer]
		[Export ("initWithNotificationIDsToMarkRead:")]
		IntPtr Constructor (CKNotificationID [] notificationIds);

		[NullAllowed]
		[Export ("notificationIDs", ArgumentSemantic.Copy)]
		CKNotificationID [] NotificationIds { get; set; }

		[Export ("markNotificationsReadCompletionBlock", ArgumentSemantic.Copy)]
		CKMarkNotificationsReadHandler Completed {
			get;
			set; 
		}
	}

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
	[BaseType (typeof (CKOperation))]
	[Deprecated (PlatformName.iOS, 11, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 13)]
	[Deprecated (PlatformName.WatchOS, 4, 0)]
	[Deprecated (PlatformName.TvOS, 11, 0)]
	interface CKModifyBadgeOperation {

		[Export ("initWithBadgeValue:")]
		IntPtr Constructor (nuint badgeValue);

		[Export ("badgeValue", ArgumentSemantic.UnsafeUnretained)]
		nuint BadgeValue { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifyBadgeCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed {
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true), Watch (3,0)]
	delegate void CKModifyRecordsOperationHandler (CKRecord [] savedRecords, CKRecordID [] deletedRecordIds, NSError operationError);

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordsOperation {

		[Export ("initWithRecordsToSave:recordIDsToDelete:")]
		IntPtr Constructor ([NullAllowed] CKRecord [] recordsToSave, [NullAllowed] CKRecordID [] recordsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("recordsToSave", ArgumentSemantic.Copy)]
		CKRecord [] RecordsToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordIDsToDelete", ArgumentSemantic.Copy)]
		CKRecordID [] RecordIdsToDelete { get; set; }

		[Export ("savePolicy", ArgumentSemantic.Assign)]
		CKRecordSavePolicy SavePolicy { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("clientChangeTokenData", ArgumentSemantic.Copy)]
		NSData ClientChangeTokenData { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("perRecordProgressBlock", ArgumentSemantic.Copy)]
		Action<CKRecord,double> PerRecordProgress {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("perRecordCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKRecord, NSError> PerRecordCompletion {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("modifyRecordsCompletionBlock", ArgumentSemantic.Copy)]
		CKModifyRecordsOperationHandler Completed {
			get;
			set; 
		}

		[Export ("atomic")]
		bool Atomic { get; set; }
		
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKModifyRecordZonesHandler (CKRecordZone [] savedRecordZones, CKRecordZoneID [] deletedRecordZoneIds, NSError operationError);

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordZonesOperation {

		[Export ("initWithRecordZonesToSave:recordZoneIDsToDelete:")]
		IntPtr Constructor ([NullAllowed] CKRecordZone [] recordZonesToSave, [NullAllowed] CKRecordZoneID [] recordZoneIdsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("recordZonesToSave", ArgumentSemantic.Copy)]
		CKRecordZone [] RecordZonesToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordZoneIDsToDelete", ArgumentSemantic.Copy)]
		CKRecordZoneID [] RecordZoneIdsToDelete { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifyRecordZonesCompletionBlock", ArgumentSemantic.Copy)]
		CKModifyRecordZonesHandler Completed {
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKModifySubscriptionsHandler (CKSubscription [] savedSubscriptions, string [] deletedSubscriptionIds, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKModifySubscriptionsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithSubscriptionsToSave:subscriptionIDsToDelete:")]
		IntPtr Constructor ([NullAllowed] CKSubscription [] subscriptionsToSave, [NullAllowed] string [] subscriptionIdsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("subscriptionsToSave", ArgumentSemantic.Copy)]
		CKSubscription [] SubscriptionsToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("subscriptionIDsToDelete", ArgumentSemantic.Copy)]
		string [] SubscriptionIdsToDelete { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifySubscriptionsCompletionBlock", ArgumentSemantic.Copy)]
		CKModifySubscriptionsHandler Completed {
			get;
			set; 
		}
	}

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // doc: <quote>You do not create notification IDs directly.</quote>
	[BaseType (typeof (NSObject))]
	interface CKNotificationID : NSCopying, NSSecureCoding, NSCoding {

	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKNotification is not meant for direct instantiation
	[BaseType (typeof (NSObject))]
#if XAMCORE_4_0 || WATCH
	[Abstract] // as per doc
#endif
	interface CKNotification : NSSecureCoding {

		[Export ("notificationType", ArgumentSemantic.Assign)]
		CKNotificationType NotificationType { get; }

		[Export ("notificationID", ArgumentSemantic.Copy)]
		CKNotificationID NotificationId { get; }

		[NullAllowed, Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("isPruned", ArgumentSemantic.UnsafeUnretained)]
		bool IsPruned { get; }

		[NoTV]
		[NullAllowed, Export ("alertBody")]
		string AlertBody { get; }

		[NoTV]
		[NullAllowed, Export ("alertLocalizationKey")]
		string AlertLocalizationKey { get; }

		[NoTV]
		[NullAllowed, Export ("alertLocalizationArgs", ArgumentSemantic.Copy)]
		string [] AlertLocalizationArgs { get; }

		[NoTV]
		[NullAllowed, Export ("alertActionLocalizationKey")]
		string AlertActionLocalizationKey { get; }

		[NoTV]
		[NullAllowed, Export ("alertLaunchImage")]
		string AlertLaunchImage { get; }

		[TV (10, 0)]
		[NullAllowed, Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; }

		[NoTV]
		[NullAllowed, Export ("soundName")]
		string SoundName { get; }

		[Static]
		[Export ("notificationFromRemoteNotificationDictionary:")]
		CKNotification FromRemoteNotificationDictionary (NSDictionary notificationDictionary);

		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("subscriptionID")]
		string SubscriptionID { get; }

		[NoTV]
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("category")]
		string Category { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("title")]
		string Title { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("titleLocalizationKey")]
		string TitleLocalizationKey { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("titleLocalizationArgs", ArgumentSemantic.Copy)]
		string[] TitleLocalizationArgs { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitleLocalizationKey")]
		string SubtitleLocalizationKey { get; }

		[Watch (4, 0), NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitleLocalizationArgs", ArgumentSemantic.Copy)]
		string[] SubtitleLocalizationArgs { get; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKQueryNotification is not meant for direct instantiation
	[BaseType (typeof (CKNotification))]
	interface CKQueryNotification : NSCoding, NSSecureCoding {

		[Export ("queryNotificationReason", ArgumentSemantic.Assign)]
		CKQueryNotificationReason QueryNotificationReason { get; }

		[NullAllowed, Export ("recordFields", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		NSDictionary<NSString, NSObject> RecordFields { get; }
#else
		NSDictionary RecordFields { get; }
#endif

		[NullAllowed, Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }

		[iOS (8, 0)]
		[Mac (10, 10)]
		[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'DatabaseScope' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'DatabaseScope' instead.")]
		[Export ("isPublicDatabase", ArgumentSemantic.UnsafeUnretained)]
		bool IsPublicDatabase { get; }
		
		[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // objc_exception_throw on CKNotification init
	[BaseType (typeof (CKNotification))]
	interface CKRecordZoneNotification : NSCoding, NSSecureCoding {

		[Export ("recordZoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID RecordZoneId { get; }

		[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[DisableDefaultCtor] // objc_exception_throw on CKNotification init
	[BaseType (typeof(CKNotification))]
	interface CKDatabaseNotification
	{
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64 : true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface CKOperationConfiguration : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("container", ArgumentSemantic.Strong)]
		CKContainer Container { get; set; }

		[Export ("qualityOfService", ArgumentSemantic.Assign)]
		NSQualityOfService QualityOfService { get; set; }

		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }

		[Export ("longLived")]
		bool LongLived { [Bind ("isLongLived")] get; set; }

		[Export ("timeoutIntervalForRequest")]
		double TimeoutIntervalForRequest { get; set; }

		[Export ("timeoutIntervalForResource")]
		double TimeoutIntervalForResource { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSOperation))]
	[DisableDefaultCtor] // Assertion failure in -[CKOperation init], /SourceCache/CloudKit/CloudKit-175.3/Framework/Operations/CKOperation.m:65
#if XAMCORE_4_0 || WATCH
	[Abstract] // as per docs
#endif
	interface CKOperation {

		// Apple removed, without deprecation, this property in iOS 9.3 SDK
		// [Mac (10,11), iOS (9,0)]
		// [Export ("activityStart")]
		// ulong ActivityStart ();

		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[NullAllowed, Export ("container", ArgumentSemantic.Retain)]
		CKContainer Container { get; set; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 9,0, message: "Use 'QualityOfService' property.")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use 'QualityOfService' property.")]
		[Export ("usesBackgroundSession", ArgumentSemantic.UnsafeUnretained)]
		bool UsesBackgroundSession { get; set; }

		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Export ("allowsCellularAccess", ArgumentSemantic.UnsafeUnretained)]
		bool AllowsCellularAccess { get; set; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[Export ("operationID")]
		string OperationID { get; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[Export ("longLived")]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		bool LongLived { [Bind ("isLongLived")] get; set; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("timeoutIntervalForRequest")]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		double TimeoutIntervalForRequest { get; set; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("timeoutIntervalForResource")]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message : "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'CKOperationConfiguration' instead.")]
		double TimeoutIntervalForResource { get; set; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[NullAllowed]
		[Export ("longLivedOperationWasPersistedBlock", ArgumentSemantic.Strong)]
		Action LongLivedOperationWasPersistedCallback { get; set; }		

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[Export ("configuration", ArgumentSemantic.Copy)]
		CKOperationConfiguration Configuration { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("group", ArgumentSemantic.Strong)]
		CKOperationGroup Group { get; set; }
	}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64 : true), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] 
	interface CKOperationGroup : NSSecureCoding {

		[Export ("operationGroupID")]
		string OperationGroupId { get; }

		[Export ("defaultConfiguration", ArgumentSemantic.Copy)]
		CKOperationConfiguration DefaultConfiguration { get; set; }

		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[Export ("quantity")]
		nuint Quantity { get; set; }

		[Export ("expectedSendSize", ArgumentSemantic.Assign)]
		CKOperationGroupTransferSize ExpectedSendSize { get; set; }

		[Export ("expectedReceiveSize", ArgumentSemantic.Assign)]
		CKOperationGroupTransferSize ExpectedReceiveSize { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKQuery initWithRecordType:predicate:sortDescriptors:]
	[BaseType (typeof (NSObject))]
	interface CKQuery : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("initWithRecordType:predicate:")]
		IntPtr Constructor (string recordType, NSPredicate predicate);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[NullAllowed, Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; set; }
	}

	[iOS (8,0), Watch (3,0), TV (10,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKQueryOperation {

#if !WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();
#endif

		[Field ("CKQueryOperationMaximumResults")][Internal]
		IntPtr _MaximumResults { get; set; }

		[Export ("initWithQuery:")]
		IntPtr Constructor (CKQuery query);

		[Export ("initWithCursor:")]
		IntPtr Constructor (CKQueryCursor cursor);

		[NullAllowed] // by default this property is null
		[Export ("query", ArgumentSemantic.Copy)]
		CKQuery Query { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("cursor", ArgumentSemantic.Copy)]
		CKQueryCursor Cursor { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneId { get; set; }

		[Export ("resultsLimit", ArgumentSemantic.UnsafeUnretained)]
		nuint ResultsLimit { get; set; }

		[Export ("desiredKeys", ArgumentSemantic.Copy)][NullAllowed]
		string [] DesiredKeys { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordFetchedBlock", ArgumentSemantic.Copy)]
		Action<CKRecord> RecordFetched {
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("queryCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKQueryCursor, NSError> Completed {
			get;
			set; 
		}
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CKRecordValue {

	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Crashes [CKRecord init] objc_exception_throw
	[BaseType (typeof (NSObject))]
	interface CKRecord : NSSecureCoding, NSCopying {

		[Field ("CKRecordTypeUserRecord")]
		NSString TypeUserRecord { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Field ("CKRecordParentKey")]
		NSString ParentKey { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Field ("CKRecordShareKey")]
		NSString ShareKey { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Field ("CKRecordTypeShare")]
		NSString TypeShare { get; }

		[Export ("initWithRecordType:")]
		IntPtr Constructor (string recordType);

		[Export ("initWithRecordType:recordID:")]
		IntPtr Constructor (string recordType, CKRecordID recordId);

		[Export ("initWithRecordType:zoneID:")]
		IntPtr Constructor (string recordType, CKRecordZoneID zoneId);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		CKRecordID Id { get; }
#else
		[Obsolete ("Use 'Id' instead.")]
		CKRecordID RecordId { get; }
#endif

		[NullAllowed, Export ("recordChangeTag")]
		string RecordChangeTag { get; }

		[NullAllowed, Export ("creatorUserRecordID", ArgumentSemantic.Copy)]
		CKRecordID CreatorUserRecordId { get; }

		[NullAllowed, Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; }

		[NullAllowed, Export ("lastModifiedUserRecordID", ArgumentSemantic.Copy)]
		CKRecordID LastModifiedUserRecordId { get; }

		[NullAllowed, Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get; }

		[Export ("objectForKey:")] [Internal]
		NSObject _ObjectForKey (string key);

		[Export ("setObject:forKey:")] [Internal]
		void _SetObject (IntPtr obj, string key);

		[Export ("allKeys")]
		string [] AllKeys ();

		[Export ("allTokens")]
		string [] AllTokens ();

		// No need for this ones
//		[Export ("objectForKeyedSubscript:")]
//		NSObject ObjectForKeyedSubscript (string key);
//
//		[Export ("setObject:forKeyedSubscript:")]
//		void SetObject (CKRecordValue obj, string key);

		[Export ("changedKeys")]
		string [] ChangedKeys ();

		[Export ("encodeSystemFieldsWithCoder:")]
		void EncodeSystemFields (NSCoder coder);
		
		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[NullAllowed, Export ("share", ArgumentSemantic.Copy)]
		CKReference Share { get; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[NullAllowed, Export ("parent", ArgumentSemantic.Copy)]
		CKReference Parent { get; set; }

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("setParentReferenceFromRecord:")]
		void SetParent ([NullAllowed] CKRecord parentRecord);

		[iOS (10,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
		[Export ("setParentReferenceFromRecordID:")]
		void SetParent ([NullAllowed] CKRecordID parentRecordID);
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException You must call -[CKRecordID initWithRecordName:] or -[CKRecordID initWithRecordName:zoneID:]
	interface CKRecordID : NSSecureCoding, NSCopying {

		[Export ("initWithRecordName:")]
		IntPtr Constructor (string recordName);

		[DesignatedInitializer]
		[Export ("initWithRecordName:zoneID:")]
		IntPtr Constructor (string recordName, CKRecordZoneID zoneId);

		[Export ("recordName", ArgumentSemantic.Retain)]
		string RecordName { get; }

		[Export ("zoneID", ArgumentSemantic.Retain)]
		CKRecordZoneID ZoneId { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
#if XAMCORE_4_0 || WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
	[DisableDefaultCtor]
#endif
	[BaseType (typeof (NSObject))]
	interface CKRecordZone : NSSecureCoding, NSCopying {

		[Field ("CKRecordZoneDefaultName")]
		NSString DefaultName { get; }

		[Export ("initWithZoneName:")]
		IntPtr Constructor (string zoneName);

		[Export ("initWithZoneID:")]
		IntPtr Constructor (CKRecordZoneID zoneId);

		[Export ("zoneID", ArgumentSemantic.Retain)]
		CKRecordZoneID ZoneId { get; }

		[Export ("capabilities", ArgumentSemantic.UnsafeUnretained)]
		CKRecordZoneCapabilities Capabilities { get; }

		[Static]
		[Export ("defaultRecordZone")]
		CKRecordZone DefaultRecordZone ();
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException You must call -[CKRecordZoneID initWithZoneName:ownerName:]
	interface CKRecordZoneID : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("initWithZoneName:ownerName:")]
		IntPtr Constructor (string zoneName, string ownerName);

		[Export ("zoneName", ArgumentSemantic.Retain)]
		string ZoneName { get; }

		[Export ("ownerName", ArgumentSemantic.Retain)]
		string OwnerName { get; }
	}

	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKReference initWithRecordID:] or -[CKReference initWithRecord:] or -[CKReference initWithAsset:]
	[BaseType (typeof (NSObject))]
	interface CKReference : NSSecureCoding, NSCopying, CKRecordValue {

		[DesignatedInitializer]
		[Export ("initWithRecordID:action:")]
		IntPtr Constructor (CKRecordID recordId, CKReferenceAction action);

		[Export ("initWithRecord:action:")]
		IntPtr Constructor (CKRecord record, CKReferenceAction action);

		[Export ("referenceAction", ArgumentSemantic.Assign)]
		CKReferenceAction ReferenceAction { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }
	}

	[NoWatch]
	[iOS (10,0)][TV (10,0), Mac (10,12, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof(CKSubscription))]
	interface CKQuerySubscription : NSSecureCoding, NSCopying
	{
		[Export ("initWithRecordType:predicate:options:")]
		IntPtr Constructor (string recordType, NSPredicate predicate, CKQuerySubscriptionOptions querySubscriptionOptions);

		[Export ("initWithRecordType:predicate:subscriptionID:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (string recordType, NSPredicate predicate, string subscriptionID, CKQuerySubscriptionOptions querySubscriptionOptions);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[NullAllowed, Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneID { get; set; }

		[Export ("querySubscriptionOptions", ArgumentSemantic.Assign)]
		CKQuerySubscriptionOptions SubscriptionOptions { get; }
	}

	[NoWatch]
	[iOS (10,0)][TV (10,0), Mac (10,12, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof(CKSubscription))]
	interface CKRecordZoneSubscription : NSSecureCoding, NSCopying
	{
		[Export ("initWithZoneID:")]
		IntPtr Constructor (CKRecordZoneID zoneID);

		[Export ("initWithZoneID:subscriptionID:")]
		[DesignatedInitializer]
		IntPtr Constructor (CKRecordZoneID zoneID, string subscriptionID);

		[Export ("zoneID", ArgumentSemantic.Copy)]
		// we need the setter since it was bound in the base type
		CKRecordZoneID ZoneID { get; [NotImplemented] set; }

		[NullAllowed, Export ("recordType")]
		string RecordType { get; set; }
	}

	[NoWatch]
	[iOS (10,0)][TV (10,0), Mac (10,12, onlyOn64 : true)]
	[DisableDefaultCtor]
	[BaseType (typeof(CKSubscription))]
	interface CKDatabaseSubscription : NSSecureCoding, NSCopying
	{
		[Export ("initWithSubscriptionID:")]
		[DesignatedInitializer]
		IntPtr Constructor (string subscriptionID);

		[NullAllowed, Export ("recordType")]
		string RecordType { get; set; }
	}

	[NoWatch]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // objc_exception_throw on [CKSubscription init]
	[BaseType (typeof (NSObject))]
	interface CKSubscription : NSSecureCoding, NSCopying {

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKQuerySubscription'.")]
		[Export ("initWithRecordType:predicate:options:")]
		IntPtr Constructor (string recordType, NSPredicate predicate, CKSubscriptionOptions subscriptionOptions);

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKQuerySubscription'.")]
		[Export ("initWithRecordType:predicate:subscriptionID:options:")]
		IntPtr Constructor (string recordType, NSPredicate predicate, string subscriptionId, CKSubscriptionOptions subscriptionOptions);

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKRecordZoneSubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKRecordZoneSubscription'.")]
		[Export ("initWithZoneID:options:")]
		IntPtr Constructor (CKRecordZoneID zoneId, CKSubscriptionOptions subscriptionOptions);

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKRecordZoneSubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKRecordZoneSubscription'.")]
		[Export ("initWithZoneID:subscriptionID:options:")]
		IntPtr Constructor (CKRecordZoneID zoneId, string subscriptionId, CKSubscriptionOptions subscriptionOptions);

		[Export ("subscriptionID")]
		string SubscriptionId { get; }

		[Export ("subscriptionType", ArgumentSemantic.UnsafeUnretained)]
		CKSubscriptionType SubscriptionType { get; }

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKQuerySubscription'.")]
		[Export ("recordType")]
		string RecordType { get; }

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKQuerySubscription'.")]
		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKQuerySubscriptionOptions'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKQuerySubscriptionOptions'.")]
		[Export ("subscriptionOptions", ArgumentSemantic.UnsafeUnretained)]
		CKSubscriptionOptions SubscriptionOptions { get; }

		[TV (10,0)]
		[Export ("notificationInfo", ArgumentSemantic.Copy)]
		CKNotificationInfo NotificationInfo { get; set; }

		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'CKRecordZoneSubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10,12, message: "Use 'CKRecordZoneSubscription'.")]
		[Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneID { get; set; }
	}

	[NoWatch]
	[iOS (8,0)][TV (10,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface CKNotificationInfo : NSSecureCoding, NSCopying, NSCoding {

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("alertBody")]
		string AlertBody { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("alertLocalizationKey")]
		string AlertLocalizationKey { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("alertLocalizationArgs", ArgumentSemantic.Copy)]
		string [] AlertLocalizationArgs { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("alertActionLocalizationKey")]
		string AlertActionLocalizationKey { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("alertLaunchImage")]
		string AlertLaunchImage { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("soundName")]
		string SoundName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }

		[TV (10, 0)]
		[Export ("shouldBadge", ArgumentSemantic.UnsafeUnretained)]
		bool ShouldBadge { get; set; }

		[Export ("shouldSendContentAvailable")]
		bool ShouldSendContentAvailable { get; set; }

		[NoTV]
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("titleLocalizationKey")]
		string TitleLocalizationKey { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("titleLocalizationArgs", ArgumentSemantic.Copy)]
		string[] TitleLocalizationArgs { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitleLocalizationKey")]
		string SubtitleLocalizationKey { get; set; }

		[NoTV, Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("subtitleLocalizationArgs", ArgumentSemantic.Copy)]
		string[] SubtitleLocalizationArgs { get; set; }

		[Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[Export ("shouldSendMutableContent")]
		bool ShouldSendMutableContent { get; set; }

		[Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("collapseIDKey")]
		string CollapseIdKey { get; set; }
	}
	
	[Watch (3,0)]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Name: CKException Reason: You can't call init on CKQueryCursor
	[BaseType (typeof (NSObject))]
	interface CKQueryCursor : NSCopying, NSSecureCoding {

	}

	delegate void CKFetchWebAuthTokenOperationHandler (string webAuthToken, NSError operationError);

	[iOS (9,2), Mac (10,11,2, onlyOn64 : true)]
	[TV (9,1)]
	[Watch (3,0)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchWebAuthTokenOperation {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithAPIToken:")]
		IntPtr Constructor (string token);

		[NullAllowed]
		[Export ("APIToken")]
		string ApiToken { get; set; }

		[NullAllowed]
		[Export ("fetchWebAuthTokenCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchWebAuthTokenOperationHandler Completed { get; set; }
	}

	[iOS (10,0), TV (10,0), Watch (3,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverUserIdentitiesOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithUserIdentityLookupInfos:")]
		IntPtr Constructor (CKUserIdentityLookupInfo[] userIdentityLookupInfos);

		[Export ("userIdentityLookupInfos", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo[] UserIdentityLookupInfos { get; set; }

		[NullAllowed, Export ("userIdentityDiscoveredBlock", ArgumentSemantic.Copy)]
		Action<CKUserIdentity, CKUserIdentityLookupInfo> Discovered { get; set; }

		[NullAllowed, Export ("discoverUserIdentitiesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }
	}

	[NoTV, iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverAllUserIdentitiesOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[NullAllowed, Export ("userIdentityDiscoveredBlock", ArgumentSemantic.Copy)]
		Action<CKUserIdentity> Discovered { get; set; }

		[NullAllowed, Export ("discoverAllUserIdentitiesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchShareParticipantsOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithUserIdentityLookupInfos:")]
		IntPtr Constructor (CKUserIdentityLookupInfo[] userIdentityLookupInfos);

		[NullAllowed]
		[Export ("userIdentityLookupInfos", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo[] UserIdentityLookupInfos { get; set; }

		[NullAllowed, Export ("shareParticipantFetchedBlock", ArgumentSemantic.Copy)]
		Action<CKShareParticipant> Fetched { get; set; }

		[NullAllowed, Export ("fetchShareParticipantsCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void CKAcceptPerShareCompletionHandler (CKShareMetadata shareMetadata, CKShare acceptedShare, NSError error);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKAcceptSharesOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithShareMetadatas:")]
		IntPtr Constructor (CKShareMetadata[] shareMetadatas);

		[Export ("shareMetadatas", ArgumentSemantic.Copy)]
		[NullAllowed]
		CKShareMetadata[] ShareMetadatas { get; set; }

		[NullAllowed, Export ("perShareCompletionBlock", ArgumentSemantic.Copy)]
		CKAcceptPerShareCompletionHandler PerShareCompleted { get; set; }

		[NullAllowed, Export ("acceptSharesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> AcceptSharesCompleted { get; set; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void CKFetchPerShareMetadataHandler (NSUrl shareURL, CKShareMetadata shareMetadata, NSError error);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchShareMetadataOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithShareURLs:")]
		IntPtr Constructor (NSUrl[] shareUrls);

		[NullAllowed]
		[Export ("shareURLs", ArgumentSemantic.Copy)]
		NSUrl[] ShareUrls { get; set; }

		[Export ("shouldFetchRootRecord")]
		bool ShouldFetchRootRecord { get; set; }

		[NullAllowed, Export ("rootRecordDesiredKeys", ArgumentSemantic.Copy)]
		string[] RootRecordDesiredKeys { get; set; }

		[NullAllowed, Export ("perShareMetadataBlock", ArgumentSemantic.Copy)]
		CKFetchPerShareMetadataHandler PerShareMetadata { get; set; }

		[NullAllowed, Export ("fetchShareMetadataCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> FetchShareMetadataCompleted { get; set; }
	}

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	delegate void CKFetchDatabaseChangesCompletionHandler (CKServerChangeToken serverChangeToken, bool moreComing, NSError operationError);

	[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchDatabaseChangesOperation
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithPreviousServerChangeToken:")]
		IntPtr Constructor ([NullAllowed] CKServerChangeToken previousServerChangeToken);

		[NullAllowed, Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		// @property (assign, nonatomic) NSUInteger resultsLimit;
		[Export ("resultsLimit")]
		nuint ResultsLimit { get; set; }

		[Export ("fetchAllChanges")]
		bool FetchAllChanges { get; set; }

		[NullAllowed, Export ("recordZoneWithIDChangedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordZoneID> Changed { get; set; }

		[NullAllowed, Export ("recordZoneWithIDWasDeletedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordZoneID> WasDeleted { get; set; }

		[NullAllowed, Export ("changeTokenUpdatedBlock", ArgumentSemantic.Copy)]
		Action<CKServerChangeToken> ChangeTokenUpdated { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[NullAllowed, Export ("recordZoneWithIDWasPurgedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordZoneID> WasPurged { get; set; }

		[NullAllowed, Export ("fetchDatabaseChangesCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchDatabaseChangesCompletionHandler ChangesCompleted { get; set; }
	}
}
