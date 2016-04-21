using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;

namespace XamCore.CloudKit
{
	// NSInteger -> CKContainer.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKAccountStatus : nint {
		CouldNotDetermine = 0,
		Available = 1,
		Restricted = 2,
		NoAccount = 3
	}

	// NSUInteger -> CKContainer.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	[Flags]
	public enum CKApplicationPermissions : nuint {
		UserDiscoverability = 1 << 0
	}

	// NSInteger -> CKContainer.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKApplicationPermissionStatus : nint {
		InitialState = 0,
		CouldNotComplete = 1,
		Denied = 2,
		Granted = 3
	}

	// NSInteger -> CKError.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	[ErrorDomain ("CKErrorDomain")]
	public enum CKErrorCode : nint {
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
		LimitExceeded  = 27,
		UserDeletedZone = 28,
	}

	// NSInteger -> CKModifyRecordsOperation.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKRecordSavePolicy : nint {
		SaveIfServerRecordUnchanged = 0,
		SaveChangedKeys = 1,
		SaveAllKeys = 2
	}

	// NSInteger -> CKNotification.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKNotificationType : nint {
		Query = 1,
		RecordZone = 2,
		ReadNotification = 3
	}

	// NSInteger -> CKNotification.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKQueryNotificationReason : nint {
		RecordCreated = 1,
		RecordUpdated,
		RecordDeleted
	}

	// NSUInteger -> CKRecordZone.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Flags]
	[Native]
	public enum CKRecordZoneCapabilities : nuint {
		FetchChanges = 1 << 0,
		Atomic = 1 << 1
	}

	// NSUInteger -> CKReference.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKReferenceAction : nuint {
		None = 0,
		DeleteSelf = 1
	}

	// NSInteger -> CKSubscription.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKSubscriptionType : nint {
		Query = 1,
		RecordZone = 2
	}

	// NSInteger -> CKSubscription.h
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Flags]
	[Native]
	public enum CKSubscriptionOptions : nuint {
		FiresOnRecordCreation = 1 << 0,
		FiresOnRecordUpdate = 1 << 1,
		FiresOnRecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}

}

