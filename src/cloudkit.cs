using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreLocation;
#if !TVOS
using XamCore.Contacts;
#endif

namespace XamCore.CloudKit {

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKAsset initWithFileURL:] or -[CKAsset initWithData:]
	[BaseType (typeof (NSObject))]
	interface CKAsset : NSCoding, NSSecureCoding {

		[Export ("initWithFileURL:")]
		IntPtr Constructor (NSUrl fileUrl);

		[Export ("fileURL", ArgumentSemantic.Copy)]
		NSUrl FileUrl { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use +[CKContainer privateCloudDatabase] or +[CKContainer publicCloudDatabase] instead of creating your own
	[BaseType (typeof (NSObject))]
	interface CKContainer {

		[Field ("CKOwnerDefaultName")]
		NSString OwnerDefaultName { get; }

		[Static]
		[Export ("defaultContainer")]
		CKContainer DefaultContainer { get; }

		[Static]
		[Export ("containerWithIdentifier:")]
		CKContainer FromIdentifier (string containerIdentifier);

		[Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("addOperation:")]
		void AddOperation (CKOperation operation);

		[Export ("privateCloudDatabase")]
		CKDatabase PrivateCloudDatabase { get; }

		[Export ("publicCloudDatabase")]
		CKDatabase PublicCloudDatabase { get; }

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

		[NoWatch]
		[NoTV]
		[Export ("discoverAllContactUserInfosWithCompletionHandler:")]
		[Async]
		void DiscoverAllContactUserInfos (Action<CKDiscoveredUserInfo[], NSError> completionHandler);

		[NoWatch]
		[Export ("discoverUserInfoWithEmailAddress:completionHandler:")]
		[Async]
		void DiscoverUserInfo (string email, Action<CKDiscoveredUserInfo, NSError> completionHandler);

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
		
	}

	delegate void CKDatabaseDeleteSubscriptionHandler (string subscriptionId, NSError error);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // *** Assertion failure in -[CKDatabase init]
	[BaseType (typeof (NSObject))]
	interface CKDatabase {
		[Export ("addOperation:")]
		void AddOperation (CKDatabaseOperation operation);

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
		[Async]
		void FetchSubscription (string subscriptionId, Action<CKSubscription, NSError> completionHandler);

		[Export ("fetchAllSubscriptionsWithCompletionHandler:")]
		[Async]
		void FetchAllSubscriptions (Action<CKSubscription[], NSError> completionHandler);

		[Export ("saveSubscription:completionHandler:")]
		[Async]
		void SaveSubscription (CKSubscription subscription, Action<CKSubscription, NSError> completionHandler);

		[Export ("deleteSubscriptionWithID:completionHandler:")]
		[Async]
		void DeleteSubscription (string subscriptionID, CKDatabaseDeleteSubscriptionHandler completionHandler);
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor]
	interface CKDatabaseOperation {

		[Export ("database", ArgumentSemantic.Retain)] [NullAllowed]
		CKDatabase Database { get; set; }
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	interface CKDiscoverAllContactsOperation {
		[NullAllowed] // by default this property is null
		[Export ("discoverAllContactsCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKDiscoveredUserInfo[], NSError> DiscoverAllContactsHandler { get; set; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface CKDiscoveredUserInfo : NSCoding, NSCopying, NSSecureCoding {

		[Export ("userRecordID", ArgumentSemantic.Copy)]
		CKRecordID UserRecordId { get; }

		[Availability (Introduced = Platform.Mac_10_10 | Platform.iOS_8_0, Deprecated = Platform.Mac_10_11 | Platform.iOS_9_0, Message = "Use DisplayContact.GivenName")]
		[Export ("firstName", ArgumentSemantic.Copy)]
		string FirstName { get; }

		[Availability (Introduced = Platform.Mac_10_10 | Platform.iOS_8_0, Deprecated = Platform.Mac_10_11 | Platform.iOS_9_0, Message = "Use DisplayContact.FamilyName")]
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

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKDiscoverUserInfosCompletionHandler (NSDictionary emailsToUserInfos, NSDictionary userRecordIdsToUserInfos, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKOperation))]
	interface CKDiscoverUserInfosOperation {

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
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	// CKError.h Fields
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

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKOperation))]
	interface CKFetchNotificationChangesOperation {
		[Export ("initWithPreviousServerChangeToken:")]
		IntPtr Constructor (CKServerChangeToken previousServerChangeToken);

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
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("fetchNotificationChangesCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKServerChangeToken, NSError> Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: CKException Reason: You can't call init on CKServerChangeToken
	[BaseType (typeof (NSObject))]
	interface CKServerChangeToken : NSCopying, NSSecureCoding {
	
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchRecordChangesHandler (CKServerChangeToken serverChangeToken, NSData clientChangeTokenData, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchRecordChangesOperation {

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
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("recordWithIDWasDeletedBlock", ArgumentSemantic.Copy)]
		Action<CKRecordID> RecordDeleted {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[Export ("moreComing")]
		bool MoreComing { get; }

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordChangesCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchRecordChangesHandler AllChangesReported {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchRecordsCompletedHandler (NSDictionary recordsByRecordId, NSError error);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchRecordsOperation {

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
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("perRecordCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKRecord,CKRecordID,NSError> PerRecordCompletion {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordsCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchRecordsCompletedHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[Static]
		[Export ("fetchCurrentUserRecordOperation")]
		CKFetchRecordsOperation FetchCurrentUserRecordOperation ();
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKRecordZoneCompleteHandler (NSDictionary recordZonesByZoneId, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchRecordZonesOperation {
		[Export ("initWithRecordZoneIDs:")]
		IntPtr Constructor (CKRecordZoneID [] zoneIds);

		[NullAllowed] // by default this property is null
		[Export ("recordZoneIDs", ArgumentSemantic.Copy)]
		CKRecordZoneID [] RecordZoneIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fetchRecordZonesCompletionBlock", ArgumentSemantic.Copy)]
		CKRecordZoneCompleteHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[Static]
		[Export ("fetchAllRecordZonesOperation")]
		CKFetchRecordZonesOperation FetchAllRecordZonesOperation ();
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKFetchSubscriptionsCompleteHandler (NSDictionary subscriptionsBySubscriptionId, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchSubscriptionsOperation {

		[Export ("initWithSubscriptionIDs:")]
		IntPtr Constructor (string [] subscriptionIds);

		[NullAllowed] // by default this property is null
		[Export ("subscriptionIDs", ArgumentSemantic.Copy)]
		string [] SubscriptionIds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fetchSubscriptionCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchSubscriptionsCompleteHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[Static]
		[Export ("fetchAllSubscriptionsOperation")]
		CKFetchSubscriptionsOperation FetchAllSubscriptionsOperation ();
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
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

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (CKOperation))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKMarkNotificationsReadOperation initWithNotificationIDsToMarkRead:]
	interface CKMarkNotificationsReadOperation {

		[DesignatedInitializer]
		[Export ("initWithNotificationIDsToMarkRead:")]
		IntPtr Constructor (CKNotificationID [] notificationIds);

		[Export ("notificationIDs", ArgumentSemantic.Copy)]
		CKNotificationID [] NotificationIds { get; set; }

		[Export ("markNotificationsReadCompletionBlock", ArgumentSemantic.Copy)]
		CKMarkNotificationsReadHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKOperation))]
	interface CKModifyBadgeOperation {

		[Export ("initWithBadgeValue:")]
		IntPtr Constructor (nuint badgeValue);

		[Export ("badgeValue", ArgumentSemantic.UnsafeUnretained)]
		nuint BadgeValue { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifyBadgeCompletionBlock", ArgumentSemantic.Copy)]
		Action<NSError> Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKModifyRecordsOperationHandler (CKRecord [] savedRecords, CKRecordID [] deletedRecordIds, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordsOperation {

		[Export ("initWithRecordsToSave:recordIDsToDelete:")]
		IntPtr Constructor (CKRecord [] recordsToSave, [NullAllowed] CKRecordID [] recordsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("recordsToSave", ArgumentSemantic.Copy)]
		CKRecord [] RecordsToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordIDsToDelete", ArgumentSemantic.Copy)]
		CKRecordID [] RecordIdsToDelete { get; set; }

		[Export ("savePolicy", ArgumentSemantic.UnsafeUnretained)]
		CKRecordSavePolicy SavePolicy { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("clientChangeTokenData", ArgumentSemantic.Copy)]
		NSData ClientChangeTokenData { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("perRecordProgressBlock", ArgumentSemantic.Copy)]
		Action<CKRecord,double> PerRecordProgress {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("perRecordCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKRecord, NSError> PerRecordCompletion {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("modifyRecordsCompletionBlock", ArgumentSemantic.Copy)]
		CKModifyRecordsOperationHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[Export ("atomic", ArgumentSemantic.UnsafeUnretained)]
		bool Atomic { get; set; }
		
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKModifyRecordZonesHandler (CKRecordZone [] savedRecordZones, CKRecordZoneID [] deletedRecordZoneIds, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifyRecordZonesOperation {

		[Export ("initWithRecordZonesToSave:recordZoneIDsToDelete:")]
		IntPtr Constructor (CKRecordZone [] recordZonesToSave, CKRecordZoneID [] recordZoneIdsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("recordZonesToSave", ArgumentSemantic.Copy)]
		CKRecordZone [] RecordZonesToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("recordZoneIDsToDelete", ArgumentSemantic.Copy)]
		CKRecordZoneID [] RecordZoneIdsToDelete { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifyRecordZonesCompletionBlock", ArgumentSemantic.Copy)]
		CKModifyRecordZonesHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	delegate void CKModifySubscriptionsHandler (CKSubscription [] savedSubscriptions, string [] deletedSubscriptionIds, NSError operationError);

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKModifySubscriptionsOperation {

		[DesignatedInitializer]
		[Export ("initWithSubscriptionsToSave:subscriptionIDsToDelete:")]
		IntPtr Constructor (CKSubscription [] subscriptionsToSave, string [] subscriptionIdsToDelete);

		[NullAllowed] // by default this property is null
		[Export ("subscriptionsToSave", ArgumentSemantic.Copy)]
		CKSubscription [] SubscriptionsToSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("subscriptionIDsToDelete", ArgumentSemantic.Copy)]
		string [] SubscriptionIdsToDelete { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("modifySubscriptionsCompletionBlock", ArgumentSemantic.Copy)]
		CKModifySubscriptionsHandler Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface CKNotificationID : NSCopying, NSSecureCoding, NSCoding {

	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKNotification is not meant for direct instantiation
	[BaseType (typeof (NSObject))]
	interface CKNotification : NSSecureCoding {

		[Export ("notificationType", ArgumentSemantic.UnsafeUnretained)]
		CKNotificationType NotificationType { get; }

		[Export ("notificationID", ArgumentSemantic.Copy)]
		CKNotificationID NotificationId { get; }

		[Export ("containerIdentifier")]
		string ContainerIdentifier { get; }

		[Export ("isPruned", ArgumentSemantic.UnsafeUnretained)]
		bool IsPruned { get; }

		[NoTV]
		[Export ("alertBody")]
		string AlertBody { get; }

		[NoTV]
		[Export ("alertLocalizationKey")]
		string AlertLocalizationKey { get; }

		[NoTV]
		[Export ("alertLocalizationArgs", ArgumentSemantic.Copy)]
		string [] AlertLocalizationArgs { get; }

		[NoTV]
		[Export ("alertActionLocalizationKey")]
		string AlertActionLocalizationKey { get; }

		[NoTV]
		[Export ("alertLaunchImage")]
		string AlertLaunchImage { get; }

		[NoTV]
		[Export ("badge", ArgumentSemantic.Copy)]
		NSNumber Badge { get; }

		[NoTV]
		[Export ("soundName")]
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
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: CKQueryNotification is not meant for direct instantiation
	[BaseType (typeof (CKNotification))]
	interface CKQueryNotification : NSCoding, NSSecureCoding {

		[Export ("queryNotificationReason", ArgumentSemantic.UnsafeUnretained)]
		CKQueryNotificationReason QueryNotificationReason { get; }

		[Export ("recordFields", ArgumentSemantic.Copy)]
		NSDictionary RecordFields { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }

		[Export ("isPublicDatabase", ArgumentSemantic.UnsafeUnretained)]
		bool IsPublicDatabase { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // objc_exception_throw on CKNotification init
	[BaseType (typeof (CKNotification))]
	interface CKRecordZoneNotification : NSCoding, NSSecureCoding {

		[Export ("recordZoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID RecordZoneId { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSOperation))]
	[DisableDefaultCtor] // Assertion failure in -[CKOperation init], /SourceCache/CloudKit/CloudKit-175.3/Framework/Operations/CKOperation.m:65
	interface CKOperation {

		// Apple removed, without deprecation, this property in iOS 9.3 SDK
		// [Mac (10,11), iOS (9,0)]
		// [Export ("activityStart")]
		// ulong ActivityStart ();

		[Export ("container", ArgumentSemantic.Retain)]
		CKContainer Container { get; set; }

		[Deprecated (PlatformName.iOS, 9,0, message: "Use QualityOfService property")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use QualityOfService property")]
		[Export ("usesBackgroundSession", ArgumentSemantic.UnsafeUnretained)]
		bool UsesBackgroundSession { get; set; }

		[Export ("allowsCellularAccess", ArgumentSemantic.UnsafeUnretained)]
		bool AllowsCellularAccess { get; set; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[Export ("operationID")]
		string OperationID { get; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[Export ("longLived")]
		bool LongLived { [Bind ("isLongLived")] get; set; }

		[iOS (9,3)][Mac (10,11,4)]
		[TV (9,2)]
		[Export ("longLivedOperationWasPersistedBlock", ArgumentSemantic.Strong)]
		Action LongLivedOperationWasPersistedCallback { get; set; }		
	}

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

		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; set; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKQueryOperation {

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
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}

		[NullAllowed] // by default this property is null
		[Export ("queryCompletionBlock", ArgumentSemantic.Copy)]
		Action<CKQueryCursor, NSError> Completed {
			[NotImplemented ("Only setting the handler is currently supported.")]
			get;
			set; 
		}
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CKRecordValue {

	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Crashes [CKRecord init] objc_exception_throw
	[BaseType (typeof (NSObject))]
	interface CKRecord : NSSecureCoding, NSCopying {

		[Field ("CKRecordTypeUserRecord")]
		NSString TypeUserRecord { get; }

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
		[Obsolete ("Use Id instead")]
		CKRecordID RecordId { get; }
#endif

		[Export ("recordChangeTag")]
		string RecordChangeTag { get; }

		[Export ("creatorUserRecordID", ArgumentSemantic.Copy)]
		CKRecordID CreatorUserRecordId { get; }

		[Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; }

		[Export ("lastModifiedUserRecordID", ArgumentSemantic.Copy)]
		CKRecordID LastModifiedUserRecordId { get; }

		[Export ("modificationDate", ArgumentSemantic.Copy)]
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
	}

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
	[DisableDefaultCtor]
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

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CKReference initWithRecordID:] or -[CKReference initWithRecord:] or -[CKReference initWithAsset:]
	[BaseType (typeof (NSObject))]
	interface CKReference : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("initWithRecordID:action:")]
		IntPtr Constructor (CKRecordID recordId, CKReferenceAction action);

		[Export ("initWithRecord:action:")]
		IntPtr Constructor (CKRecord record, CKReferenceAction action);

		[Export ("referenceAction", ArgumentSemantic.UnsafeUnretained)]
		CKReferenceAction ReferenceAction { get; }

		[Export ("recordID", ArgumentSemantic.Copy)]
		CKRecordID RecordId { get; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // objc_exception_throw on [CKSubscription init]
	[BaseType (typeof (NSObject))]
	interface CKSubscription : NSSecureCoding, NSCopying {

		[Export ("initWithRecordType:predicate:options:")]
		IntPtr Constructor (string recordType, NSPredicate predicate, CKSubscriptionOptions subscriptionOptions);

		[DesignatedInitializer]
		[Export ("initWithRecordType:predicate:subscriptionID:options:")]
		IntPtr Constructor (string recordType, NSPredicate predicate, string subscriptionId, CKSubscriptionOptions subscriptionOptions);

		[Export ("initWithZoneID:options:")]
		IntPtr Constructor (CKRecordZoneID zoneId, CKSubscriptionOptions subscriptionOptions);

		[DesignatedInitializer]
		[Export ("initWithZoneID:subscriptionID:options:")]
		IntPtr Constructor (CKRecordZoneID zoneId, string subscriptionId, CKSubscriptionOptions subscriptionOptions);

		[Export ("subscriptionID")]
		string SubscriptionId { get; }

		[Export ("subscriptionType", ArgumentSemantic.UnsafeUnretained)]
		CKSubscriptionType SubscriptionType { get; }

		[Export ("recordType")]
		string RecordType { get; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		NSPredicate Predicate { get; }

		[Export ("subscriptionOptions", ArgumentSemantic.UnsafeUnretained)]
		CKSubscriptionOptions SubscriptionOptions { get; }

		[NoWatch]
		[Export ("notificationInfo", ArgumentSemantic.Copy)]
		CKNotificationInfo NotificationInfo { get; set; }

		[Export ("zoneID", ArgumentSemantic.Copy)]
		CKRecordZoneID ZoneID { get; set; }
	}

	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[NoWatch]
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

		[NoTV]
		[Export ("shouldBadge", ArgumentSemantic.UnsafeUnretained)]
		bool ShouldBadge { get; set; }

		[NoTV]
		[Export ("shouldSendContentAvailable")]
		bool ShouldSendContentAvailable { get; set; }

		[NoTV]
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("category")]
		string Category { get; set; }
	}
	
	[iOS (8,0), Mac (10,10, onlyOn64 : true)]
	[DisableDefaultCtor] // Name: CKException Reason: You can't call init on CKQueryCursor
	[BaseType (typeof (NSObject))]
	interface CKQueryCursor : NSCopying, NSSecureCoding {

	}

#if !MONOMAC // radar 23904638 - The header does not match symbols in nm, and the header isn't even in CloudKit.h :facepalm:
	delegate void CKFetchWebAuthTokenOperationHandler (string webAuthToken, NSError operationError);

	[iOS (9,2), Mac (10,11,2, onlyOn64 : true)]
	[TV (9,1)]
	[Watch (3,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (CKDatabaseOperation))]
	interface CKFetchWebAuthTokenOperation {

		[DesignatedInitializer]
		[Export ("initWithAPIToken:")]
		IntPtr Constructor (string token);

		[NullAllowed]
		[Export ("APIToken")]
		string ApiToken { get; set; }

		[NullAllowed]
		[Export ("fetchWebAuthTokenCompletionBlock", ArgumentSemantic.Copy)]
		CKFetchWebAuthTokenOperationHandler Completed { get; set; }
	}
#endif
}
