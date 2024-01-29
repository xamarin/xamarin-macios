using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;

#nullable enable

namespace CloudKit {
	// NSInteger -> CKContainer.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKAccountStatus : long {
		CouldNotDetermine = 0,
		Available = 1,
		Restricted = 2,
		NoAccount = 3,
		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		TemporarilyUnavailable = 4,
	}

	// NSUInteger -> CKContainer.h
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum CKApplicationPermissions : ulong {
		UserDiscoverability = 1 << 0
	}

	// NSInteger -> CKContainer.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKApplicationPermissionStatus : long {
		InitialState = 0,
		CouldNotComplete = 1,
		Denied = 2,
		Granted = 3
	}

	// NSInteger -> CKError.h
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("CKErrorDomain")]
	public enum CKErrorCode : long {
		None,
		InternalError = 1,
		PartialFailure = 2,
		NetworkUnavailable = 3,
		NetworkFailure = 4,
		BadContainer = 5,
		ServiceUnavailable = 6,
		RequestRateLimited = 7,
		MissingEntitlement = 8,
		NotAuthenticated = 9,
		PermissionFailure = 10,
		UnknownItem = 11,
		InvalidArguments = 12,
		ResultsTruncated = 13,
		ServerRecordChanged = 14,
		ServerRejectedRequest = 15,
		AssetFileNotFound = 16,
		AssetFileModified = 17,
		IncompatibleVersion = 18,
		ConstraintViolation = 19,
		OperationCancelled = 20,
		ChangeTokenExpired = 21,
		BatchRequestFailed = 22,
		ZoneBusy = 23,
		BadDatabase = 24,
		QuotaExceeded = 25,
		ZoneNotFound = 26,
		LimitExceeded = 27,
		UserDeletedZone = 28,
		TooManyParticipants = 29,
		AlreadyShared = 30,
		ReferenceViolation = 31,
		ManagedAccountRestricted = 32,
		ParticipantMayNeedVerification = 33,
		ResponseLost = 34,
		AssetNotAvailable = 35,
		TemporarilyUnavailable = 36,
	}

	// NSInteger -> CKModifyRecordsOperation.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKRecordSavePolicy : long {
		SaveIfServerRecordUnchanged = 0,
		SaveChangedKeys = 1,
		SaveAllKeys = 2
	}

	// NSInteger -> CKNotification.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKNotificationType : long {
		Query = 1,
		RecordZone = 2,
		ReadNotification = 3,
		[MacCatalyst (13, 1)]
		Database = 4,
	}

	// NSInteger -> CKNotification.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKQueryNotificationReason : long {
		RecordCreated = 1,
		RecordUpdated,
		RecordDeleted
	}

	// NSUInteger -> CKRecordZone.h
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	public enum CKRecordZoneCapabilities : ulong {
		FetchChanges = 1 << 0,
		Atomic = 1 << 1,
		[MacCatalyst (13, 1)]
		Sharing = 1 << 2,
		[Mac (12, 0), iOS (15, 0), TV (15, 0)]
		[MacCatalyst (15, 0)]
		ZoneWideSharing = 1 << 3,

	}

	// NSUInteger -> CKReference.h
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKReferenceAction : ulong {
		None = 0,
		DeleteSelf = 1
	}

	// NSInteger -> CKSubscription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKSubscriptionType : long {
		Query = 1,
		RecordZone = 2,
		[MacCatalyst (13, 1)]
		Database = 3,
	}

	// NSInteger -> CKSubscription.h

#if !NET
	[NoWatch]
	[Obsoleted (PlatformName.iOS, 14, 0, message: "Use 'CKQuerySubscriptionOptions' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'CKQuerySubscriptionOptions' instead.")]
	[Obsoleted (PlatformName.MacOSX, 10, 16, message: "Use 'CKQuerySubscriptionOptions' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'CKQuerySubscriptionOptions' instead.")]
	[Flags]
	[Native]
	public enum CKSubscriptionOptions : ulong {
		FiresOnRecordCreation = 1 << 0,
		FiresOnRecordUpdate = 1 << 1,
		FiresOnRecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}
#endif

	[MacCatalyst (13, 1)]
	[Native]
	public enum CKDatabaseScope : long {
		Public = 1,
		Private,
		Shared,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CKShareParticipantAcceptanceStatus : long {
		Unknown,
		Pending,
		Accepted,
		Removed,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CKShareParticipantPermission : long {
		Unknown,
		None,
		ReadOnly,
		ReadWrite,
	}

	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'CKShareParticipantRole' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'CKShareParticipantRole' instead.")]
	[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'CKShareParticipantRole' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'CKShareParticipantRole' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CKShareParticipantRole' instead.")]
	[Native]
	public enum CKShareParticipantType : long {
		Unknown = 0,
		Owner = 1,
		PrivateUser = 3,
		PublicUser = 4,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKQuerySubscriptionOptions : ulong {
		RecordCreation = 1 << 0,
		RecordUpdate = 1 << 1,
		RecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum CKOperationGroupTransferSize : long {
		Unknown,
		Kilobytes,
		Megabytes,
		TensOfMegabytes,
		HundredsOfMegabytes,
		Gigabytes,
		TensOfGigabytes,
		HundredsOfGigabytes,
	}

	[Watch (5, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum CKShareParticipantRole : long {
		Unknown = 0,
		Owner = 1,
		PrivateUser = 3,
		PublicUser = 4,
	}

	[NoTV, NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native, Flags]
	public enum CKSharingParticipantAccessOption : ulong {
		AnyoneWithLink = 1uL << 0,
		SpecifiedRecipientsOnly = 1uL << 1,
		Any = AnyoneWithLink | SpecifiedRecipientsOnly,
	}

	[NoTV, NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native, Flags]
	public enum CKSharingParticipantPermissionOption : ulong {
		ReadOnly = 1uL << 0,
		ReadWrite = 1uL << 1,
		Any = ReadOnly | ReadWrite,
	}


	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEngineAccountChangeType : long {
		SignIn,
		SignOut,
		SwitchAccounts,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEngineSyncReason : long {
		Scheduled,
		Manual,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEngineEventType : long {
		StateUpdate,
		AccountChange,
		FetchedDatabaseChanges,
		FetchedRecordZoneChanges,
		SentDatabaseChanges,
		SentRecordZoneChanges,
		WillFetchChanges,
		WillFetchRecordZoneChanges,
		DidFetchRecordZoneChanges,
		DidFetchChanges,
		WillSendChanges,
		DidSendChanges,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEnginePendingRecordZoneChangeType : long {
		SaveRecord,
		DeleteRecord,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEngineZoneDeletionReason : long {
		Deleted,
		Purged,
		EncryptedDataReset,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum CKSyncEnginePendingDatabaseChangeType : long {
		SaveZone,
		DeleteZone,
	}

}
