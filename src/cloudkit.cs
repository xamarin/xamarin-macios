using System;
using ObjCRuntime;
using Foundation;
using CoreLocation;
#if !TVOS
using Contacts;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CloudKit {

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKAsset initWithFileURL:] or -[CKAsset initWithData:]
	[BaseType (typeof (NSObject))]
	interface CKAsset : NSCoding, NSSecureCoding, CKRecordValue {

		[Export ("initWithFileURL:")]
		NativeHandle Constructor (NSUrl fileUrl);

		[Export ("fileURL", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSUrl FileUrl { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CKUserIdentityLookupInfo : NSSecureCoding, NSCopying {
		[Internal, Export ("initWithEmailAddress:")]
		IntPtr _FromEmail (string emailAddress);

		[Internal, Export ("initWithPhoneNumber:")]
		IntPtr _FromPhoneNumber (string phoneNumber);

		[Export ("initWithUserRecordID:")]
		CKUserIdentityLookupInfo Constructor (CKRecordID userRecordID);

		[Static]
		[Export ("lookupInfosWithEmails:")]
		CKUserIdentityLookupInfo [] GetLookupInfosWithEmails (string [] emails);

		[Static]
		[Export ("lookupInfosWithPhoneNumbers:")]
		CKUserIdentityLookupInfo [] GetLookupInfosWithPhoneNumbers (string [] phoneNumbers);

		[Static]
		[Export ("lookupInfosWithRecordIDs:")]
		CKUserIdentityLookupInfo [] GetLookupInfos (CKRecordID [] recordIDs);

		[NullAllowed, Export ("emailAddress")]
		string EmailAddress { get; }

		[NullAllowed, Export ("phoneNumber")]
		string PhoneNumber { get; }

		[NullAllowed, Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordID { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CKUserIdentity : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("lookupInfo", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo LookupInfo { get; }

		[NullAllowed, Export ("nameComponents", ArgumentSemantic.Copy)]
		NSPersonNameComponents NameComponents { get; }

		[NullAllowed, Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordID { get; }

		[Export ("hasiCloudAccount")]
		bool HasICloudAccount { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[Export ("contactIdentifiers", ArgumentSemantic.Copy)]
		string [] ContactIdentifiers { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CKShareMetadata : NSCopying, NSSecureCoding {
		[Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("share", ArgumentSemantic.Strong)]
		CKShare Share { get; }

		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'HierarchicalRootRecordId' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'HierarchicalRootRecordId' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'HierarchicalRootRecordId' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'HierarchicalRootRecordId' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'HierarchicalRootRecordId' instead.")]
		[Export ("rootRecordID", ArgumentSemantic.Copy)]
		CKRecordID RootRecordID { get; }

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("participantRole", ArgumentSemantic.Assign)]
		CKShareParticipantRole ParticipantRole { get; }

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ParticipantRole' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ParticipantRole' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ParticipantRole' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ParticipantRole' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ParticipantRole' instead.")]
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

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("hierarchicalRootRecordID", ArgumentSemantic.Copy)]
		CKRecordID HierarchicalRootRecordId { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKRecord))]
	[DisableDefaultCtor]
	interface CKShare {
		[Export ("initWithRootRecord:")]
		NativeHandle Constructor (CKRecord rootRecord);

		[Export ("initWithRootRecord:shareID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CKRecord rootRecord, CKRecordID shareID);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithRecordZoneID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CKRecordZoneID recordZoneId);

		[Export ("publicPermission", ArgumentSemantic.Assign)]
		CKShareParticipantPermission PublicPermission { get; set; }

		[NullAllowed]
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("participants", ArgumentSemantic.Strong)]
		CKShareParticipant [] Participants { get; }

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
	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	partial interface CKShareKeys {

		[Field ("CKShareTitleKey")]
		NSString Title { get; }

		[Field ("CKShareThumbnailImageDataKey")]
		NSString ThumbnailImageData { get; }

		[Field ("CKShareTypeKey")]
		NSString Type { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CKShareParticipant : NSSecureCoding, NSCopying {
		[Export ("userIdentity", ArgumentSemantic.Strong)]
		CKUserIdentity UserIdentity { get; }

		[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("role", ArgumentSemantic.Assign)]
		CKShareParticipantRole Role { get; set; }

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'Role' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Role' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Role' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Role' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Role' instead.")]
		[Export ("type", ArgumentSemantic.Assign)]
		CKShareParticipantType Type { get; set; }

		[Export ("acceptanceStatus", ArgumentSemantic.Assign)]
		CKShareParticipantAcceptanceStatus AcceptanceStatus { get; }

		[Export ("permission", ArgumentSemantic.Assign)]
		CKShareParticipantPermission Permission { get; set; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use +[CKContainer privateCloudDatabase] or +[CKContainer publicCloudDatabase] instead of creating your own
	[BaseType (typeof (NSObject))]
	interface CKContainer {

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CurrentUserDefaultName' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'CurrentUserDefaultName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CurrentUserDefaultName' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CurrentUserDefaultName' instead.")]
		[Field ("CKOwnerDefaultName")]
		NSString OwnerDefaultName { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
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

		[iOS (10, 0)]
		[Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("sharedCloudDatabase")]
		CKDatabase SharedCloudDatabase { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("databaseWithDatabaseScope:")]
		CKDatabase GetDatabase (CKDatabaseScope databaseScope);

		[Export ("accountStatusWithCompletionHandler:")]
		[Async]
		void GetAccountStatus (Action<CKAccountStatus, NSError> completionHandler);

		[Export ("statusForApplicationPermission:completionHandler:")]
		[Async]
		void StatusForApplicationPermission (CKApplicationPermissions applicationPermission, Action<CKApplicationPermissionStatus, NSError> completionHandler);

		[Export ("requestApplicationPermission:completionHandler:")]
		[Async]
		void RequestApplicationPermission (CKApplicationPermissions applicationPermission, Action<CKApplicationPermissionStatus, NSError> completionHandler);

		[Export ("fetchUserRecordIDWithCompletionHandler:")]
		[Async]
		void FetchUserRecordId (Action<CKRecordID, NSError> completionHandler);

		[iOS (10, 0)]
		[Mac (10, 12)]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("discoverAllIdentitiesWithCompletionHandler:")]
		[Async]
		void DiscoverAllIdentities (Action<CKUserIdentity [], NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("discoverUserIdentityWithEmailAddress:completionHandler:")]
		[Async]
		void DiscoverUserIdentityWithEmailAddress (string email, Action<CKUserIdentity, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("discoverUserIdentityWithPhoneNumber:completionHandler:")]
		[Async]
		void DiscoverUserIdentityWithPhoneNumber (string phoneNumber, Action<CKUserIdentity, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("discoverUserIdentityWithUserRecordID:completionHandler:")]
		[Async]
		void DiscoverUserIdentity (CKRecordID userRecordID, Action<CKUserIdentity, NSError> completionHandler);

		[iOS (9, 0)]
		[Mac (10, 11)]
		[MacCatalyst (13, 1)]
		[Field ("CKAccountChangedNotification")]
		[Notification]
		NSString AccountChangedNotification { get; }

		[iOS (9, 3)]
		[Mac (10, 11, 4)]
		[NoTV] // does not answer on devices
		[MacCatalyst (13, 1)]
		[Export ("fetchAllLongLivedOperationIDsWithCompletionHandler:")]
		[Async]
		void FetchAllLongLivedOperationIDs (Action<NSDictionary<NSString, NSOperation>, NSError> completionHandler);

		[iOS (9, 3)]
		[Mac (10, 11, 4)]
		[NoTV] // does not answer on devices
		[MacCatalyst (13, 1)]
		[Export ("fetchLongLivedOperationWithID:completionHandler:")]
		[Async]
		void FetchLongLivedOperation (string [] operationID, Action<NSDictionary<NSString, NSOperation>, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("fetchShareParticipantWithEmailAddress:completionHandler:")]
		[Async]
		void FetchShareParticipantWithEmailAddress (string emailAddress, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("fetchShareParticipantWithPhoneNumber:completionHandler:")]
		[Async]
		void FetchShareParticipantWithPhoneNumber (string phoneNumber, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("fetchShareParticipantWithUserRecordID:completionHandler:")]
		[Async]
		void FetchShareParticipant (CKRecordID userRecordID, Action<CKShareParticipant, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("fetchShareMetadataWithURL:completionHandler:")]
		[Async]
		void FetchShareMetadata (NSUrl url, Action<CKShareMetadata, NSError> completionHandler);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("acceptShareMetadata:completionHandler:")]
		[Async]
		void AcceptShareMetadata (CKShareMetadata metadata, Action<CKShare, NSError> completionHandler);
	}

	delegate void CKDatabaseDeleteSubscriptionHandler (string subscriptionId, NSError error);

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // *** Assertion failure in -[CKDatabase init]
	[BaseType (typeof (NSObject))]
	interface CKDatabase {
		[Export ("addOperation:")]
		void AddOperation (CKDatabaseOperation operation);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
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
		void PerformQuery (CKQuery query, [NullAllowed] CKRecordZoneID zoneId, Action<CKRecord [], NSError> completionHandler);

		[Export ("fetchAllRecordZonesWithCompletionHandler:")]
		[Async]
		void FetchAllRecordZones (Action<CKRecordZone [], NSError> completionHandler);

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
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		void FetchSubscription (string subscriptionId, Action<CKSubscription, NSError> completionHandler);

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("fetchAllSubscriptionsWithCompletionHandler:")]
		[Async]
		void FetchAllSubscriptions (Action<CKSubscription [], NSError> completionHandler);

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("saveSubscription:completionHandler:")]
		[Async]
		void SaveSubscription (CKSubscription subscription, Action<CKSubscription, NSError> completionHandler);

		[Export ("deleteSubscriptionWithID:completionHandler:")]
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		void DeleteSubscription (string subscriptionID, CKDatabaseDeleteSubscriptionHandler completionHandler);
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor]
#if NET || WATCH
	[Abstract] // as per docs
#endif
	interface CKDatabaseOperation {

		[Export ("database", ArgumentSemantic.Retain)]
		[NullAllowed]
		CKDatabase Database { get; set; }
	}

#if !NET
	// This type is no longer in the headers.
	[NoWatch]
	[NoTV]
	[Obsoleted (PlatformName.iOS, 14, 0, message: "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[Obsoleted (PlatformName.MacOSX, 10, 16, message: "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKDiscoverAllUserIdentitiesOperation' instead.")]
	[iOS (8, 0), Mac (10, 10)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverAllContactsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

	}

	// This type is no longer in the headers.
	[Obsoleted (PlatformName.iOS, 14, 0, message: "Use 'CKUserIdentity' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKUserIdentity' instead.")]
	[Obsoleted (PlatformName.MacOSX, 10, 16, message: "Use 'CKUserIdentity' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKUserIdentity' instead.")]
	[iOS (8, 0), Mac (10, 10)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface CKDiscoveredUserInfo : NSCoding, NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed]
		[Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordId { get; }
	}
#endif // !NET

	// CKError.h Fields
	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
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

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CKErrorUserDidResetEncryptedDataKey")]
		NSString UserDidResetEncryptedDataKey { get; }
	}

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[BaseType (typeof (CKOperation))]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[DisableDefaultCtor] // designated
	interface CKFetchNotificationChangesOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithPreviousServerChangeToken:")]
		NativeHandle Constructor ([NullAllowed] CKServerChangeToken previousServerChangeToken);

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

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: CKException Reason: You can't call init on CKServerChangeToken
	[BaseType (typeof (NSObject))]
	interface CKServerChangeToken : NSCopying, NSSecureCoding {

	}

	[NoWatch]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchRecordChangesHandler (CKServerChangeToken serverChangeToken, NSData clientChangeTokenData, NSError operationError);

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[iOS (8, 0), Mac (10, 10)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKFetchRecordZoneChangesOperation' instead.")]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchRecordChangesOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithRecordZoneID:previousServerChangeToken:")]
		NativeHandle Constructor (CKRecordZoneID recordZoneID, [NullAllowed] CKServerChangeToken previousServerChangeToken);

		[NullAllowed] // by default this property is null
		[Export ("recordZoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID RecordZoneId { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit", ArgumentSemantic.UnsafeUnretained)]
		nuint ResultsLimit { get; set; }

		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		[NullAllowed]
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

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchRecordZoneChangesWithIDWasDeletedHandler (CKRecordID recordID, NSString recordType);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchRecordZoneChangesTokensUpdatedHandler (CKRecordZoneID recordZoneID, CKServerChangeToken serverChangeToken, NSData clientChangeTokenData);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchRecordZoneChangesFetchCompletedHandler (CKRecordZoneID recordZoneID, CKServerChangeToken serverChangeToken, NSData clientChangeTokenData, bool moreComing, NSError recordZoneError);

	[iOS (15, 0), Watch (8, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	delegate void CKFetchRecordZoneChangesRecordWasChangedHandler (CKRecordID recordId, CKRecord record, NSError error);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchRecordZoneChangesOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use the overload with the 'NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration>' parameter instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use the overload with the 'NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload with the 'NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration>' parameter instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use the overload with the 'NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration>' parameter instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload with the 'NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration>' parameter instead.")]
		[Export ("initWithRecordZoneIDs:optionsByRecordZoneID:")]
		NativeHandle Constructor (CKRecordZoneID [] recordZoneIDs, [NullAllowed] NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesOptions> optionsByRecordZoneID);

		[iOS (12, 0), Watch (5, 0), TV (12, 0), Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Export ("initWithRecordZoneIDs:configurationsByRecordZoneID:")]
		NativeHandle Constructor (CKRecordZoneID [] recordZoneIDs, [NullAllowed] NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration> configurationsByRecordZoneID);

		[NullAllowed]
		[Export ("recordZoneIDs", ArgumentSemantic.Copy)]
		CKRecordZoneID [] RecordZoneIDs { get; set; }

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ConfigurationsByRecordZoneID' instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ConfigurationsByRecordZoneID' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ConfigurationsByRecordZoneID' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ConfigurationsByRecordZoneID' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ConfigurationsByRecordZoneID' instead.")]
		[NullAllowed, Export ("optionsByRecordZoneID", ArgumentSemantic.Copy)]
		NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesOptions> OptionsByRecordZoneID { get; set; }

		[iOS (12, 0), Watch (5, 0), TV (12, 0), Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("configurationsByRecordZoneID", ArgumentSemantic.Copy)]
		NSDictionary<CKRecordZoneID, CKFetchRecordZoneChangesConfiguration> ConfigurationsByRecordZoneID { get; set; }

		[Export ("fetchAllChanges")]
		bool FetchAllChanges { get; set; }

		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'RecordWasChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'RecordWasChangedHandler' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'RecordWasChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'RecordWasChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'RecordWasChangedHandler' instead.")]
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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("recordWasChangedBlock", ArgumentSemantic.Copy)]
		CKFetchRecordZoneChangesRecordWasChangedHandler RecordWasChangedHandler { get; set; }
	}

	[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'CKFetchRecordZoneChangesConfiguration' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'CKFetchRecordZoneChangesConfiguration' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'CKFetchRecordZoneChangesConfiguration' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'CKFetchRecordZoneChangesConfiguration' instead.")]
	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKFetchRecordZoneChangesConfiguration' instead.")]
	[BaseType (typeof (NSObject))]
	interface CKFetchRecordZoneChangesOptions : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit")]
		nuint ResultsLimit { get; set; }

		[NullAllowed, Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }
	}

	[Watch (5, 0), TV (12, 0), Mac (10, 14), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CKFetchRecordZoneChangesConfiguration : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("previousServerChangeToken", ArgumentSemantic.Copy)]
		CKServerChangeToken PreviousServerChangeToken { get; set; }

		[Export ("resultsLimit")]
		nuint ResultsLimit { get; set; }

		[NullAllowed, Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }
	}

	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchRecordsCompletedHandler (NSDictionary recordsByRecordId, NSError error);

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchRecordsOperation {

		[Export ("initWithRecordIDs:")]
		NativeHandle Constructor (CKRecordID [] recordIds);

		[NullAllowed] // by default this property is null
		[Export ("recordIDs", ArgumentSemantic.Copy)]
		CKRecordID [] RecordIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("perRecordProgressBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID, double> PerRecordProgress {
			get;
			set;
		}

		[NullAllowed] // by default this property is null
		[Export ("perRecordCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKRecord, CKRecordID, NSError> PerRecordCompletion {
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

	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKRecordZoneCompleteHandler (NSDictionary recordZonesByZoneId, NSError operationError);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKRecordZonePerRecordZoneCompletionHandler (CKRecordZoneID recordZoneId, CKRecordZone recordZone, NSError error);

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	interface CKFetchRecordZonesOperation {

		[Export ("initWithRecordZoneIDs:")]
		NativeHandle Constructor (CKRecordZoneID [] zoneIds);

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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perRecordZoneCompletionBlock", ArgumentSemantic.Copy)]
		CKRecordZonePerRecordZoneCompletionHandler PerRecordZoneCompletionHandler { get; set; }
	}

	[NoWatch]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchSubscriptionsCompleteHandler (NSDictionary subscriptionsBySubscriptionId, NSError operationError);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKFetchSubscriptionsPerSubscriptionCompletionHandler (NSString subscriptionId, CKSubscription subscription, NSError error);

	[iOS (8, 0), Mac (10, 10)]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchSubscriptionsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithSubscriptionIDs:")]
		NativeHandle Constructor (string [] subscriptionIds);

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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perSubscriptionCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchSubscriptionsPerSubscriptionCompletionHandler PerSubscriptionCompletionHandler { get; set; }
	}

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
#if NET || WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
	[DisableDefaultCtor]
#endif
	[BaseType (typeof (NSSortDescriptor))]
	interface CKLocationSortDescriptor : NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithKey:relativeLocation:")]
		NativeHandle Constructor (string key, CLLocation relativeLocation);

		[Export ("relativeLocation", ArgumentSemantic.Copy)]
		CLLocation RelativeLocation { get; }
	}

	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKMarkNotificationsReadHandler (CKNotificationID [] notificationIDsMarkedRead, NSError operationError);

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKMarkNotificationsReadOperation initWithNotificationIDsToMarkRead:]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	interface CKMarkNotificationsReadOperation {

		[DesignatedInitializer]
		[Export ("initWithNotificationIDsToMarkRead:")]
		NativeHandle Constructor (CKNotificationID [] notificationIds);

		[NullAllowed]
		[Export ("notificationIDs", ArgumentSemantic.Copy)]
		CKNotificationID [] NotificationIds { get; set; }

		[NullAllowed]
		[Export ("markNotificationsReadCompletionBlock", ArgumentSemantic.Copy)]
		CKMarkNotificationsReadHandler Completed {
			get;
			set;
		}
	}

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	[BaseType (typeof (CKOperation))]
	[Deprecated (PlatformName.iOS, 11, 0)]
	[Deprecated (PlatformName.MacOSX, 10, 13)]
	[Deprecated (PlatformName.WatchOS, 4, 0)]
	[Deprecated (PlatformName.TvOS, 11, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	interface CKModifyBadgeOperation {

		[Export ("initWithBadgeValue:")]
		NativeHandle Constructor (nuint badgeValue);

		[Export ("badgeValue", ArgumentSemantic.UnsafeUnretained)]
		nuint BadgeValue { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifyBadgeCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed {
			get;
			set;
		}
	}

	[iOS (8, 0), Mac (10, 10), Watch (3, 0)]
	[MacCatalyst (13, 1)]
	delegate void CKModifyRecordsOperationHandler (CKRecord [] savedRecords, CKRecordID [] deletedRecordIds, NSError operationError);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifyRecordsOperationPerRecordSaveHandler (CKRecordID recordId, CKRecord record, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifyRecordsOperationPerRecordDeleteHandler (CKRecordID recordId, NSError error);

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordsOperation {

		[Export ("initWithRecordsToSave:recordIDsToDelete:")]
		NativeHandle Constructor ([NullAllowed] CKRecord [] recordsToSave, [NullAllowed] CKRecordID [] recordsToDelete);

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
		Action<CKRecord, double> PerRecordProgress {
			get;
			set;
		}

		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'PerRecordResultHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'PerRecordResultHandler' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'PerRecordResultHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'PerRecordResultHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PerRecordResultHandler' instead.")]
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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perRecordSaveBlock", ArgumentSemantic.Copy)]
		CKModifyRecordsOperationPerRecordSaveHandler PerRecordSaveHandler { get; set; }

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perRecordDeleteBlock", ArgumentSemantic.Copy)]
		CKModifyRecordsOperationPerRecordDeleteHandler PerRecordDeleteHandler { get; set; }

	}

	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKModifyRecordZonesHandler (CKRecordZone [] savedRecordZones, CKRecordZoneID [] deletedRecordZoneIds, NSError operationError);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifyRecordZonesPerRecordZoneSaveHandler (CKRecordZoneID zoneId, CKRecordZone zone, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifyRecordZonesPerRecordZoneDeleteHandler (CKRecordZoneID zoneId, NSError error);

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordZonesOperation {

		[Export ("initWithRecordZonesToSave:recordZoneIDsToDelete:")]
		NativeHandle Constructor ([NullAllowed] CKRecordZone [] recordZonesToSave, [NullAllowed] CKRecordZoneID [] recordZoneIdsToDelete);

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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perRecordZoneSaveBlock", ArgumentSemantic.Copy)]
		CKModifyRecordZonesPerRecordZoneSaveHandler PerRecordZoneSaveHandler { get; set; }

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perRecordZoneDeleteBlock", ArgumentSemantic.Copy)]
		CKModifyRecordZonesPerRecordZoneDeleteHandler PerRecordZoneDeleteHandler { get; set; }
	}

	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	delegate void CKModifySubscriptionsHandler (CKSubscription [] savedSubscriptions, string [] deletedSubscriptionIds, NSError operationError);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifySubscriptionsPerSubscriptionSaveHandler (NSString subscriptionId, CKSubscription subscription, NSError error);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKModifySubscriptionsPerSubscriptionDeleteHandler (NSString subscriptionId, NSError error);

	[iOS (8, 0), Mac (10, 10)]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKModifySubscriptionsOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithSubscriptionsToSave:subscriptionIDsToDelete:")]
		NativeHandle Constructor ([NullAllowed] CKSubscription [] subscriptionsToSave, [NullAllowed] string [] subscriptionIdsToDelete);

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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perSubscriptionSaveBlock", ArgumentSemantic.Copy)]
		CKModifySubscriptionsPerSubscriptionSaveHandler PerSubscriptionSaveHandler { get; set; }

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("perSubscriptionDeleteBlock", ArgumentSemantic.Copy)]
		CKModifySubscriptionsPerSubscriptionDeleteHandler PerSubscriptionDeleteHandler { get; set; }
	}

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // doc: <quote>You do not create notification IDs directly.</quote>
	[BaseType (typeof (NSObject))]
	interface CKNotificationID : NSCopying, NSSecureCoding, NSCoding {

	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKNotification is not meant for direct instantiation
	[BaseType (typeof (NSObject))]
#if NET || WATCH
	[Abstract] // as per doc
#endif
	interface CKNotification : NSSecureCoding {

		[Export ("notificationType", ArgumentSemantic.Assign)]
		CKNotificationType NotificationType { get; }

		[NullAllowed]
		[Export ("notificationID", ArgumentSemantic.Copy)]
		CKNotificationID NotificationId { get; }

		[NullAllowed, Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("isPruned", ArgumentSemantic.UnsafeUnretained)]
		bool IsPruned { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("alertBody")]
		string AlertBody { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("alertLocalizationKey")]
		string AlertLocalizationKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("alertLocalizationArgs", ArgumentSemantic.Copy)]
		string [] AlertLocalizationArgs { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("alertActionLocalizationKey")]
		string AlertActionLocalizationKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("alertLaunchImage")]
		string AlertLaunchImage { get; }

		[TV (10, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("soundName")]
		string SoundName { get; }

		[Static]
		[Export ("notificationFromRemoteNotificationDictionary:")]
		[return: NullAllowed]
		CKNotification FromRemoteNotificationDictionary (NSDictionary notificationDictionary);

		[iOS (9, 0)]
		[Mac (10, 11)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subscriptionID")]
		string SubscriptionID { get; }

		[NoTV]
		[iOS (9, 0)]
		[Mac (10, 11)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("category")]
		string Category { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("title")]
		string Title { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("titleLocalizationKey")]
		string TitleLocalizationKey { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("titleLocalizationArgs", ArgumentSemantic.Copy)]
		string [] TitleLocalizationArgs { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitleLocalizationKey")]
		string SubtitleLocalizationKey { get; }

		[Watch (4, 0), NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitleLocalizationArgs", ArgumentSemantic.Copy)]
		string [] SubtitleLocalizationArgs { get; }

		[Watch (7, 0), TV (14, 0), Mac (10, 16), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("subscriptionOwnerUserRecordID", ArgumentSemantic.Copy)]
		CKRecordID SubscriptionOwnerUserRecordId { get; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKQueryNotification is not meant for direct instantiation
	[BaseType (typeof (CKNotification))]
	interface CKQueryNotification : NSCoding, NSSecureCoding {

		[Export ("queryNotificationReason", ArgumentSemantic.Assign)]
		CKQueryNotificationReason QueryNotificationReason { get; }

		[NullAllowed, Export ("recordFields", ArgumentSemantic.Copy)]
#if XAMCORE_5_0 // delayed until next time due to #13704.
		NSDictionary<NSString, NSObject> RecordFields { get; }
#else
		NSDictionary RecordFields { get; }
#endif

		[NullAllowed, Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // objc_exception_throw on CKNotification init
	[BaseType (typeof (CKNotification))]
	interface CKRecordZoneNotification : NSCoding, NSSecureCoding {

		[NullAllowed]
		[Export ("recordZoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID RecordZoneId { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // objc_exception_throw on CKNotification init
	[BaseType (typeof (CKNotification))]
	interface CKDatabaseNotification {
		[Export ("databaseScope", ArgumentSemantic.Assign)]
		CKDatabaseScope DatabaseScope { get; }
	}

	[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
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

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSOperation))]
	[DisableDefaultCtor]
#if NET || WATCH
	[Abstract] // as per docs
#endif
	interface CKOperation {

		[Protected] // since it should (and will) be `abstract`
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		// Apple removed, without deprecation, this property in iOS 9.3 SDK
		// [Mac (10,11), iOS (9,0)]
		// [Export ("activityStart")]
		// ulong ActivityStart ();

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKOperationConfiguration' instead.")]
		[NullAllowed, Export ("container", ArgumentSemantic.Retain)]
		CKContainer Container { get; set; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKOperationConfiguration' instead.")]
		[Export ("allowsCellularAccess", ArgumentSemantic.UnsafeUnretained)]
		bool AllowsCellularAccess { get; set; }

		[iOS (9, 3)]
		[Mac (10, 11, 4)]
		[TV (9, 2)]
		[MacCatalyst (13, 1)]
		[Export ("operationID")]
		string OperationID { get; }

		[iOS (9, 3)]
		[Mac (10, 11, 4)]
		[TV (9, 2)]
		[Export ("longLived")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKOperationConfiguration' instead.")]
		bool LongLived { [Bind ("isLongLived")] get; set; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[Export ("timeoutIntervalForRequest")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKOperationConfiguration' instead.")]
		double TimeoutIntervalForRequest { get; set; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[Export ("timeoutIntervalForResource")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CKOperationConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKOperationConfiguration' instead.")]
		double TimeoutIntervalForResource { get; set; }

		[iOS (9, 3)]
		[Mac (10, 11, 4)]
		[TV (9, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("longLivedOperationWasPersistedBlock", ArgumentSemantic.Strong)]
		Action LongLivedOperationWasPersistedCallback { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("configuration", ArgumentSemantic.Copy)]
		CKOperationConfiguration Configuration { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("group", ArgumentSemantic.Strong)]
		CKOperationGroup Group { get; set; }
	}

	[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface CKOperationGroup : NSSecureCoding {

		[Export ("operationGroupID")]
		string OperationGroupId { get; }

		[NullAllowed] // null_resettable
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

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKQuery initWithRecordType:predicate:sortDescriptors:]
	[BaseType (typeof (NSObject))]
	interface CKQuery : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("initWithRecordType:predicate:")]
		NativeHandle Constructor (string recordType, NSPredicate predicate);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[NullAllowed, Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void CKQueryOperationRecordMatchedHandler (CKRecordID recordId, CKRecord record, NSError error);

	[iOS (8, 0), Watch (3, 0), TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
#if WATCH
	[DisableDefaultCtor] // does not work on watchOS, working stub provided to ease source compatibility
#else
	[DesignatedDefaultCtor]
#endif
	interface CKQueryOperation {

		[Field ("CKQueryOperationMaximumResults")]
		nint MaximumResults { get; }

		[Export ("initWithQuery:")]
		NativeHandle Constructor (CKQuery query);

		[Export ("initWithCursor:")]
		NativeHandle Constructor (CKQueryCursor cursor);

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

		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		[NullAllowed]
		string [] DesiredKeys { get; set; }

		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'RecordMatchedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'RecordMatchedHandler' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'RecordMatchedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'RecordMatchedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'RecordMatchedHandler' instead.")]
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

		[NullAllowed]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("recordMatchedBlock", ArgumentSemantic.Copy)]
		CKQueryOperationRecordMatchedHandler RecordMatchedHandler { get; set; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CKRecordValue {

	}

	interface ICKRecordValue { }

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // Crashes [CKRecord init] objc_exception_throw
	[BaseType (typeof (NSObject))]
	interface CKRecord : NSSecureCoding, NSCopying {

		[Field ("CKRecordTypeUserRecord")]
		NSString TypeUserRecord { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Field ("CKRecordParentKey")]
		NSString ParentKey { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Field ("CKRecordShareKey")]
		NSString ShareKey { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Field ("CKRecordTypeShare")]
		NSString TypeShare { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CKRecordNameZoneWideShare")]
		NSString NameZoneWideShare { get; }

		[Export ("initWithRecordType:")]
		NativeHandle Constructor (string recordType);

		[Export ("initWithRecordType:recordID:")]
		NativeHandle Constructor (string recordType, CKRecordID recordId);

		[Export ("initWithRecordType:zoneID:")]
		NativeHandle Constructor (string recordType, CKRecordZoneID zoneId);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID Id { get; }

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

		[return: NullAllowed]
		[Export ("objectForKey:")]
		[Internal]
		NSObject _ObjectForKey (string key);

		[Export ("setObject:forKey:")]
		[Internal]
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

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("share", ArgumentSemantic.Copy)]
		CKReference Share { get; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("parent", ArgumentSemantic.Copy)]
		CKReference Parent { get; set; }

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("setParentReferenceFromRecord:")]
		void SetParent ([NullAllowed] CKRecord parentRecord);

		[iOS (10, 0), TV (10, 0), Mac (10, 12)]
		[MacCatalyst (13, 1)]
		[Export ("setParentReferenceFromRecordID:")]
		void SetParent ([NullAllowed] CKRecordID parentRecordID);
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException You must call -[CKRecordID initWithRecordName:] or -[CKRecordID initWithRecordName:zoneID:]
	interface CKRecordID : NSSecureCoding, NSCopying {

		[Export ("initWithRecordName:")]
		NativeHandle Constructor (string recordName);

		[DesignatedInitializer]
		[Export ("initWithRecordName:zoneID:")]
		NativeHandle Constructor (string recordName, CKRecordZoneID zoneId);

		[Export ("recordName", ArgumentSemantic.Retain)]
		string RecordName { get; }

		[Export ("zoneID", ArgumentSemantic.Retain)]
		CKRecordZoneID ZoneId { get; }
	}

	[iOS (8, 0), Mac (10, 10)]
	[Watch (3, 0)]
	[MacCatalyst (13, 1)]
#if NET || WATCH // does not work on watchOS, existiong init* does not allow null to be used to fake it
	[DisableDefaultCtor]
#endif
	[BaseType (typeof (NSObject))]
	interface CKRecordZone : NSSecureCoding, NSCopying {

		[Field ("CKRecordZoneDefaultName")]
		NSString DefaultName { get; }

		[Export ("initWithZoneName:")]
		NativeHandle Constructor (string zoneName);

		[Export ("initWithZoneID:")]
		NativeHandle Constructor (CKRecordZoneID zoneId);

		[Export ("zoneID", ArgumentSemantic.Retain)]
		CKRecordZoneID ZoneId { get; }

		[Export ("capabilities", ArgumentSemantic.UnsafeUnretained)]
		CKRecordZoneCapabilities Capabilities { get; }

		[Static]
		[Export ("defaultRecordZone")]
		CKRecordZone DefaultRecordZone ();

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("share", ArgumentSemantic.Copy)]
		CKReference Share { get; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException You must call -[CKRecordZoneID initWithZoneName:ownerName:]
	interface CKRecordZoneID : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("initWithZoneName:ownerName:")]
		NativeHandle Constructor (string zoneName, string ownerName);

		[Export ("zoneName", ArgumentSemantic.Retain)]
		string ZoneName { get; }

		[Export ("ownerName", ArgumentSemantic.Retain)]
		string OwnerName { get; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKReference initWithRecordID:] or -[CKReference initWithRecord:] or -[CKReference initWithAsset:]
	[BaseType (typeof (NSObject))]
	interface CKReference : NSSecureCoding, NSCopying, CKRecordValue {

		[DesignatedInitializer]
		[Export ("initWithRecordID:action:")]
		NativeHandle Constructor (CKRecordID recordId, CKReferenceAction action);

		[Export ("initWithRecord:action:")]
		NativeHandle Constructor (CKRecord record, CKReferenceAction action);

		[Export ("referenceAction", ArgumentSemantic.Assign)]
		CKReferenceAction ReferenceAction { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }
	}

	[Watch (6, 0)]
	[iOS (10, 0)]
	[TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKSubscription))]
	interface CKQuerySubscription : NSSecureCoding, NSCopying {
		[Export ("initWithRecordType:predicate:options:")]
		NativeHandle Constructor (string recordType, NSPredicate predicate, CKQuerySubscriptionOptions querySubscriptionOptions);

		[Export ("initWithRecordType:predicate:subscriptionID:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string recordType, NSPredicate predicate, string subscriptionID, CKQuerySubscriptionOptions querySubscriptionOptions);

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[NullAllowed, Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneID { get; set; }

		[Export ("querySubscriptionOptions", ArgumentSemantic.Assign)]
		CKQuerySubscriptionOptions SubscriptionOptions { get; }
	}

	[Watch (6, 0)]
	[iOS (10, 0)]
	[TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKSubscription))]
	interface CKRecordZoneSubscription : NSSecureCoding, NSCopying {
		[Export ("initWithZoneID:")]
		NativeHandle Constructor (CKRecordZoneID zoneID);

		[Export ("initWithZoneID:subscriptionID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CKRecordZoneID zoneID, string subscriptionID);

		[Export ("zoneID", ArgumentSemantic.Copy)]
		// we need the setter since it was bound in the base type
		CKRecordZoneID ZoneID { get; [NotImplemented] set; }

		[NullAllowed, Export ("recordType")]
		string RecordType { get; set; }
	}

	[Watch (6, 0)]
	[iOS (10, 0)]
	[TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKSubscription))]
	interface CKDatabaseSubscription : NSSecureCoding, NSCopying {
		[Export ("initWithSubscriptionID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string subscriptionID);

		[NullAllowed, Export ("recordType")]
		string RecordType { get; set; }
	}

	[Watch (6, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // objc_exception_throw on [CKSubscription init]
	[BaseType (typeof (NSObject))]
	interface CKSubscription : NSSecureCoding, NSCopying {

#if !NET
		// This constructor does not exist in the headers (anymore?)
		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKQuerySubscription'.")]
		[Export ("initWithRecordType:predicate:options:")]
		NativeHandle Constructor (string recordType, NSPredicate predicate, CKSubscriptionOptions subscriptionOptions);

		// This constructor does not exist in the headers (anymore?)
		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKQuerySubscription'.")]
		[Export ("initWithRecordType:predicate:subscriptionID:options:")]
		NativeHandle Constructor (string recordType, NSPredicate predicate, string subscriptionId, CKSubscriptionOptions subscriptionOptions);
#endif

		[Export ("subscriptionID")]
		string SubscriptionId { get; }

		[Export ("subscriptionType", ArgumentSemantic.UnsafeUnretained)]
		CKSubscriptionType SubscriptionType { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKQuerySubscription'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKQuerySubscription'.")]
		[NullAllowed]
		[Export ("recordType")]
		string RecordType { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'CKQuerySubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKQuerySubscription'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKQuerySubscription'.")]
		[NullAllowed]
		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[TV (10, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("notificationInfo", ArgumentSemantic.Copy)]
		CKNotificationInfo NotificationInfo { get; set; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKRecordZoneSubscription'.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'CKRecordZoneSubscription'.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKRecordZoneSubscription'.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKRecordZoneSubscription'.")]
		[NullAllowed]
		[Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneID { get; set; }
	}

	[Watch (6, 0)]
	[iOS (8, 0)]
	[TV (10, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CKNotificationInfo : NSSecureCoding, NSCopying, NSCoding {

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("alertBody")]
		string AlertBody { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("alertLocalizationKey")]
		string AlertLocalizationKey { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("alertLocalizationArgs", ArgumentSemantic.Copy)]
		string [] AlertLocalizationArgs { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("alertActionLocalizationKey")]
		string AlertActionLocalizationKey { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("alertLaunchImage")]
		string AlertLaunchImage { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("soundName")]
		string SoundName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("desiredKeys", ArgumentSemantic.Copy)]
		string [] DesiredKeys { get; set; }

		[Export ("shouldBadge", ArgumentSemantic.UnsafeUnretained)]
		bool ShouldBadge { get; set; }

		[Export ("shouldSendContentAvailable")]
		bool ShouldSendContentAvailable { get; set; }

		[NoTV]
		[iOS (9, 0)]
		[Mac (10, 11)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("category")]
		string Category { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("titleLocalizationKey")]
		string TitleLocalizationKey { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("titleLocalizationArgs", ArgumentSemantic.Copy)]
		string [] TitleLocalizationArgs { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitleLocalizationKey")]
		string SubtitleLocalizationKey { get; set; }

		[NoTV, Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subtitleLocalizationArgs", ArgumentSemantic.Copy)]
		string [] SubtitleLocalizationArgs { get; set; }

		[Mac (10, 13), iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Export ("shouldSendMutableContent")]
		bool ShouldSendMutableContent { get; set; }

		[Mac (10, 13), iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("collapseIDKey")]
		string CollapseIdKey { get; set; }
	}

	[Watch (3, 0)]
	[iOS (8, 0), Mac (10, 10)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // Name: CKException Reason: You can't call init on CKQueryCursor
	[BaseType (typeof (NSObject))]
	interface CKQueryCursor : NSCopying, NSSecureCoding {

	}

	delegate void CKFetchWebAuthTokenOperationHandler (string webAuthToken, NSError operationError);

	[iOS (9, 2), Mac (10, 11, 2)]
	[TV (9, 1)]
	[Watch (3, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchWebAuthTokenOperation {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithAPIToken:")]
		NativeHandle Constructor (string token);

		[NullAllowed]
		[Export ("APIToken")]
		string ApiToken { get; set; }

		[NullAllowed]
		[Export ("fetchWebAuthTokenCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchWebAuthTokenOperationHandler Completed { get; set; }
	}

	[iOS (10, 0), TV (10, 0), Watch (3, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverUserIdentitiesOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithUserIdentityLookupInfos:")]
		NativeHandle Constructor (CKUserIdentityLookupInfo [] userIdentityLookupInfos);

		[Export ("userIdentityLookupInfos", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo [] UserIdentityLookupInfos { get; set; }

		[NullAllowed, Export ("userIdentityDiscoveredBlock", ArgumentSemantic.Copy)]
		Action<CKUserIdentity, CKUserIdentityLookupInfo> Discovered { get; set; }

		[NullAllowed, Export ("discoverUserIdentitiesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }
	}

	[NoTV, iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKDiscoverAllUserIdentitiesOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed, Export ("userIdentityDiscoveredBlock", ArgumentSemantic.Copy)]
		Action<CKUserIdentity> Discovered { get; set; }

		[NullAllowed, Export ("discoverAllUserIdentitiesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }
	}

	[iOS (15, 0), Watch (8, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	delegate void CKFetchShareParticipantsOperationPerShareParticipantCompletionHandler (CKUserIdentityLookupInfo identityLookupInfo, CKShareParticipant participant, NSError error);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchShareParticipantsOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithUserIdentityLookupInfos:")]
		NativeHandle Constructor (CKUserIdentityLookupInfo [] userIdentityLookupInfos);

		[NullAllowed]
		[Export ("userIdentityLookupInfos", ArgumentSemantic.Copy)]
		CKUserIdentityLookupInfo [] UserIdentityLookupInfos { get; set; }

		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'PerShareParticipantCompletionHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'PerShareParticipantCompletionHandler' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'PerShareParticipantCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'PerShareParticipantCompletionHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PerShareParticipantCompletionHandler' instead.")]
		[NullAllowed, Export ("shareParticipantFetchedBlock", ArgumentSemantic.Copy)]
		Action<CKShareParticipant> Fetched { get; set; }

		[NullAllowed, Export ("fetchShareParticipantsCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed { get; set; }

		[NullAllowed]
		[iOS (15, 0), Watch (8, 0), TV (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("perShareParticipantCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchShareParticipantsOperationPerShareParticipantCompletionHandler PerShareParticipantCompletionBlock { get; set; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKAcceptPerShareCompletionHandler (CKShareMetadata shareMetadata, CKShare acceptedShare, NSError error);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKAcceptSharesOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithShareMetadatas:")]
		NativeHandle Constructor (CKShareMetadata [] shareMetadatas);

		[Export ("shareMetadatas", ArgumentSemantic.Copy)]
		[NullAllowed]
		CKShareMetadata [] ShareMetadatas { get; set; }

		[NullAllowed, Export ("perShareCompletionBlock", ArgumentSemantic.Copy)]
		CKAcceptPerShareCompletionHandler PerShareCompleted { get; set; }

		[NullAllowed, Export ("acceptSharesCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> AcceptSharesCompleted { get; set; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchPerShareMetadataHandler (NSUrl shareURL, CKShareMetadata shareMetadata, NSError error);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchShareMetadataOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithShareURLs:")]
		NativeHandle Constructor (NSUrl [] shareUrls);

		[NullAllowed]
		[Export ("shareURLs", ArgumentSemantic.Copy)]
		NSUrl [] ShareUrls { get; set; }

		[Export ("shouldFetchRootRecord")]
		bool ShouldFetchRootRecord { get; set; }

		[NullAllowed, Export ("rootRecordDesiredKeys", ArgumentSemantic.Copy)]
		string [] RootRecordDesiredKeys { get; set; }

		[NullAllowed, Export ("perShareMetadataBlock", ArgumentSemantic.Copy)]
		CKFetchPerShareMetadataHandler PerShareMetadata { get; set; }

		[NullAllowed, Export ("fetchShareMetadataCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> FetchShareMetadataCompleted { get; set; }
	}

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	delegate void CKFetchDatabaseChangesCompletionHandler (CKServerChangeToken serverChangeToken, bool moreComing, NSError operationError);

	[iOS (10, 0), Watch (3, 0), TV (10, 0), Mac (10, 12)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CKDatabaseOperation))]
	[DisableDefaultCtor] // designated
	interface CKFetchDatabaseChangesOperation {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithPreviousServerChangeToken:")]
		NativeHandle Constructor ([NullAllowed] CKServerChangeToken previousServerChangeToken);

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

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("recordZoneWithIDWasPurgedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordZoneID> WasPurged { get; set; }

		[NullAllowed, Export ("fetchDatabaseChangesCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchDatabaseChangesCompletionHandler ChangesCompleted { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("recordZoneWithIDWasDeletedDueToUserEncryptedDataResetBlock", ArgumentSemantic.Copy)]
		Action<CKRecordZoneID> RecordZoneWithIdWasDeletedDueToUserEncryptedDataReset { get; set; }
	}

	[NoTV, NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CKAllowedSharingOptions : NSSecureCoding, NSCopying {
		[Export ("initWithAllowedParticipantPermissionOptions:allowedParticipantAccessOptions:")]
		NativeHandle Constructor (CKSharingParticipantPermissionOption allowedParticipantPermissionOptions, CKSharingParticipantAccessOption allowedParticipantAccessOptions);

		[Export ("allowedParticipantPermissionOptions", ArgumentSemantic.Assign)]
		CKSharingParticipantPermissionOption AllowedParticipantPermissionOptions { get; set; }

		[Export ("allowedParticipantAccessOptions", ArgumentSemantic.Assign)]
		CKSharingParticipantAccessOption AllowedParticipantAccessOptions { get; set; }

		[Static]
		[Export ("standardOptions", ArgumentSemantic.Strong)]
		CKAllowedSharingOptions StandardOptions { get; }
	}

	[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CKSystemSharingUIObserver {
		[Export ("initWithContainer:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CKContainer container);

		[NullAllowed, Export ("systemSharingUIDidSaveShareBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID, CKShare, NSError> SystemSharingUIDidSaveShareHandler { get; set; }

		[NullAllowed, Export ("systemSharingUIDidStopSharingBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID, NSError> SystemSharingUIDidStopSharingHandler { get; set; }
	}
}
