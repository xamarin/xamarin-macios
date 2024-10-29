using System;
using ObjCRuntime;

#nullable enable

namespace Accounts {

	// untyped enum -> ACError.h
	/// <summary>An enumeration whose values indicate various errors relating to accessing accounts.</summary>
	///     
	///     <!-- TODO: Confirm that this value is used somewhere -->
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
		MissingTransportMessageId,
		CredentialItemNotFound,
		CredentialItemNotExpired,
	}

	// NSInteger -> ACAccountStore.h
	/// <summary>An enumeration whose values indicate the result of a credential renewal request (see <see cref="M:Accounts.ACAccountStore.RenewCredentials(Accounts.ACAccount,System.Action{Accounts.ACAccountCredentialRenewResult,Foundation.NSError})" />).</summary>
	[Native]
	public enum ACAccountCredentialRenewResult : long {
		Renewed, Rejected, Failed
	}
}
