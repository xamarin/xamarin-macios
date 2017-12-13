using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.Accounts {

	// untyped enum -> ACError.h
	[ErrorDomain ("ACErrorDomain")]
	public enum ACErrorCode {
		Unknown = 1,
		AccountMissingRequiredProperty,
		AccountAuthenticationFailed,
		AccountTypeInvalid,
		AccountAlreadyExits,
		AccountNotFound,
		PermissionDenied,
		AccessInfoInvalid,
		ClientPermissionDenied,
		AccessDeniedByProtectionPolicy,
		CredentialNotFound,
		FetchCredentialFailed,
		StoreCredentialFailed,
		RemoveCredentialFailed,
		UpdatingNonexistentAccount,
		InvalidClientBundleID,		// in the header file, but not in the API diff
		DeniedByPlugin,
		CoreDataSaveFailed,
		FailedSerializingAccountInfo,
		InvalidCommand,
#if XAMCORE_3_0
		MissingTransportMessageId,
#else
		[Obsolete ("Use 'MissingTransportMessageId'.")]
		MissingMessageID,
#pragma warning disable 618 // MissingMessageID is obsolete
		MissingTransportMessageId = MissingMessageID,
#pragma warning restore 618
#endif
		CredentialItemNotFound,
		CredentialItemNotExpired,
	}

	// NSInteger -> ACAccountStore.h
	[Native]
	public enum ACAccountCredentialRenewResult : nint {
		Renewed, Rejected, Failed
	}
}
