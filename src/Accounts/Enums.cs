using System;

using ObjCRuntime;

#nullable enable

namespace Accounts {

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
		InvalidClientBundleID,      // in the header file, but not in the API diff
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
	public enum ACAccountCredentialRenewResult : long {
		Renewed, Rejected, Failed
	}
}
