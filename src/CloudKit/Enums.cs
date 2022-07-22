using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;

namespace CloudKit
{
	// NSInteger -> CKContainer.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKAccountStatus : long {
		CouldNotDetermine = 0,
		Available = 1,
		Restricted = 2,
		NoAccount = 3,
		[Mac (12,0), iOS (15,0), TV (15,0), MacCatalyst (15,0)]
		TemporarilyUnavailable = 4,
	}

	// NSUInteger -> CKContainer.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	[Flags]
	public enum CKApplicationPermissions : ulong {
		UserDiscoverability = 1 << 0
	}

	// NSInteger -> CKContainer.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKApplicationPermissionStatus : long {
		InitialState = 0,
		CouldNotComplete = 1,
		Denied = 2,
		Granted = 3
	}

	// NSInteger -> CKError.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
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
		LimitExceeded  = 27,
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
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKRecordSavePolicy : long {
		SaveIfServerRecordUnchanged = 0,
		SaveChangedKeys = 1,
		SaveAllKeys = 2
	}

	// NSInteger -> CKNotification.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKNotificationType : long {
		Query = 1,
		RecordZone = 2,
		ReadNotification = 3,
		[iOS (10,0), TV (10,0), Mac (10,12)] Database = 4,
	}

	// NSInteger -> CKNotification.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKQueryNotificationReason : long {
		RecordCreated = 1,
		RecordUpdated,
		RecordDeleted
	}

	// NSUInteger -> CKRecordZone.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Flags]
	[Native]
	public enum CKRecordZoneCapabilities : ulong {
		FetchChanges = 1 << 0,
		Atomic = 1 << 1,
		[iOS (10,0), TV (10,0), Mac (10,12)]
		Sharing = 1 << 2,
		[Mac (12,0), iOS (15,0), TV (15,0)]
		ZoneWideSharing = 1 << 3,

	}

	// NSUInteger -> CKReference.h
	[Watch (3,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKReferenceAction : ulong {
		None = 0,
		DeleteSelf = 1
	}

	// NSInteger -> CKSubscription.h
	[Watch (6,0)]
	[iOS (8,0)]
	[Mac (10, 10)]
	[Native]
	public enum CKSubscriptionType : long {
		Query = 1,
		RecordZone = 2,
		[iOS (10,0), TV (10,0), Mac (10,12)] Database = 3,
	}

	// NSInteger -> CKSubscription.h

#if !NET
	[NoWatch]
	[iOS (8, 0)]
	[Obsoleted (PlatformName.iOS, 14, 0, message : "Use 'CKQuerySubscriptionOptions' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message : "Use 'CKQuerySubscriptionOptions' instead.")]
	[Mac (10, 10)]
	[Obsoleted (PlatformName.MacOSX, 10, 16, message : "Use 'CKQuerySubscriptionOptions' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 12, message : "Use 'CKQuerySubscriptionOptions' instead.")]
	[Flags]
	[Native]
	public enum CKSubscriptionOptions : ulong {
		FiresOnRecordCreation = 1 << 0,
		FiresOnRecordUpdate = 1 << 1,
		FiresOnRecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}
#endif
	
	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKDatabaseScope : long
	{
		Public = 1,
		Private,
		Shared,
	}
	
	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKShareParticipantAcceptanceStatus : long
	{
		Unknown,
		Pending,
		Accepted,
		Removed,
	}

	[Watch (3,0)]
	[iOS (10,0), Mac (10,12)]
	[Native]
	public enum CKShareParticipantPermission : long
	{
		Unknown,
		None,
		ReadOnly,
		ReadWrite,
	}

	[Watch (3,0)]
	[iOS (10,10), Mac (10,12)]
	[Native]
	public enum CKShareParticipantType : long
	{
		Unknown = 0,
		Owner = 1,
		PrivateUser = 3,
		PublicUser = 4,
	}

	[Watch (6,0)]
	[iOS (10,0), Mac(10,12)]
	[Native]
	public enum CKQuerySubscriptionOptions : ulong
	{
		RecordCreation = 1 << 0,
		RecordUpdate = 1 << 1,
		RecordDeletion = 1 << 2,
		FiresOnce = 1 << 3,
	}

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	[Native]
	public enum CKOperationGroupTransferSize : long
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

	[Watch (5,0), TV (12,0), Mac (10,14), iOS (12,0)]
	[Native]
	public enum CKShareParticipantRole : long {
		Unknown = 0,
		Owner = 1,
		PrivateUser = 3,
		PublicUser = 4,
	}

	[TV (16,0), NoWatch, Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[Native, Flags]
	public enum CKSharingParticipantAccessOption : ulong
	{
		AnyoneWithLink = 1uL << 0,
		SpecifiedRecipientsOnly = 1uL << 1,
		Any = AnyoneWithLink | SpecifiedRecipientsOnly,
	}

	[TV (16,0), NoWatch, Mac (13,0), iOS (16,0), MacCatalyst (16,0)]
	[Native, Flags]
	public enum CKSharingParticipantPermissionOption : ulong
	{
		ReadOnly = 1uL << 0,
		ReadWrite = 1uL << 1,
		Any = ReadOnly | ReadWrite,
	}
}
