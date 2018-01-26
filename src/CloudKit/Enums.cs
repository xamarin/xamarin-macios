using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;

namespace XamCore.CloudKit
{
	// NSInteger -> CKContainer.h
	[Watch (3,0)]
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
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	[Flags]
	public enum CKApplicationPermissions : nuint {
		UserDiscoverability = 1 << 0
	}

	// NSInteger -> CKContainer.h
	[Watch (3,0)]
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
	[Watch (3,0)]
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
		[iOS (10,0), TV (10,0), Mac (10,12)] TooManyParticipants = 29,
		[iOS (10,0), TV (10,0), Mac (10,12)] AlreadyShared = 30,
		[iOS (10,0), TV (10,0), Mac (10,12)] ReferenceViolation = 31,
		[iOS (10,0), TV (10,0), Mac (10,12)] ManagedAccountRestricted = 32,
		[iOS (10,0), TV (10,0), Mac (10,12)] ParticipantMayNeedVerification = 33,
		[iOS (11,0), TV (11,0), Mac (10,13), Watch (4,0)] ResponseLost = 34,
		[iOS (11,3), TV (11,3), Mac (10,13), Watch (4,3)] AssetNotAvailable = 35,
	}

	// NSInteger -> CKModifyRecordsOperation.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKRecordSavePolicy : nint {
		SaveIfServerRecordUnchanged = 0,
		SaveChangedKeys = 1,
		SaveAllKeys = 2
	}

	// NSInteger -> CKNotification.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKNotificationType : nint {
		Query = 1,
		RecordZone = 2,
		ReadNotification = 3,
		[iOS (10,0), TV (10,0), Mac (10,12), Watch (3,0)] Database = 4,
	}

	// NSInteger -> CKNotification.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKQueryNotificationReason : nint {
		RecordCreated = 1,
		RecordUpdated,
		RecordDeleted
	}

	// NSUInteger -> CKRecordZone.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Flags]
	[Native]
	public enum CKRecordZoneCapabilities : nuint {
		FetchChanges = 1 << 0,
		Atomic = 1 << 1,
		[iOS (10,0), Watch (3,0), TV (10,0), Mac (10,12)] Sharing = 1 << 2,
	}

	// NSUInteger -> CKReference.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKReferenceAction : nuint {
		None = 0,
		DeleteSelf = 1
	}

	// NSInteger -> CKSubscription.h
	[NoWatch]
	[iOS (8,0)]
	[Availability (Platform.Mac_10_10)]
	[Native]
	public enum CKSubscriptionType : nint {
		Query = 1,
		RecordZone = 2,
		[iOS (10,0), TV (10,0), Mac (10,12)] Database = 3,
	}

	// NSInteger -> CKSubscription.h

	[NoWatch]
	[Availability (Introduced = Platform.iOS_8_0 | Platform.Mac_10_10 , Deprecated = Platform.iOS_10_0 | Platform.Mac_10_12, Message = "Use 'CKQuerySubscriptionOptions' instead.")]
	[Flags]
	[Native]
	public enum CKSubscriptionOptions : nuint {
		FiresOnRecordCreation = 1 << 0,
		FiresOnRecordUpdate = 1 << 1,
		FiresOnRecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}
	
	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKDatabaseScope : nint
	{
		Public = 1,
		Private,
		Shared,
	}
	
	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKShareParticipantAcceptanceStatus : nint
	{
		Unknown,
		Pending,
		Accepted,
		Removed,
	}

	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKShareParticipantPermission : nint
	{
		Unknown,
		None,
		ReadOnly,
		ReadWrite,
	}

	[Watch (3,0)]
	[iOS (10,10), Mac (10,12)]
	[Native]
	public enum CKShareParticipantType : nint
	{
		Unknown = 0,
		Owner = 1,
		PrivateUser = 3,
		PublicUser = 4,
	}

	[NoWatch]
	[iOS (10,0), Mac(10,12)]
	[Native]
	public enum CKQuerySubscriptionOptions : nuint
	{
		RecordCreation = 1 << 0,
		RecordUpdate = 1 << 1,
		RecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum CKOperationGroupTransferSize : nint
	{
		Unknown,
		Kilobytes,
		Megabytes,
		TensOfMegabytes,
		HundredsOfMegabytes,
		Gigabytes,
		TensOfGigabytes,
		HundredsOfGigabytes,
	}
}
