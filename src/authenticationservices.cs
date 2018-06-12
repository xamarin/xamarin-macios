//
// AuthenticationServices bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AuthenticationServices {

	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASCredentialIdentityStoreErrorDomain")]
	public enum ASCredentialIdentityStoreErrorCode : long {
		InternalError = 0,
		StoreDisabled = 1,
		StoreBusy = 2,
	}

	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASExtensionErrorDomain")]
	public enum ASExtensionErrorCode : long {
		Failed = 0,
		UserCanceled = 1,
		UserInteractionRequired = 100,
		CredentialIdentityNotFound = 101,
	}

	[iOS (12,0)]
	[Native]
	public enum ASCredentialServiceIdentifierType : long {
		Domain,
		Url,
	}

	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASWebAuthenticationSessionErrorDomain")]
	public enum ASWebAuthenticationSessionErrorCode : long {
		CanceledLogin = 1,
	}

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStore {
		[Static]
		[Export ("sharedStore")]
		ASCredentialIdentityStore SharedStore { get; }

		[Async]
		[Export ("getCredentialIdentityStoreStateWithCompletion:")]
		void GetCredentialIdentityStoreState (Action<ASCredentialIdentityStoreState> completion);

		[Async]
		[Export ("saveCredentialIdentities:completion:")]
		void SaveCredentialIdentities (ASPasswordCredentialIdentity[] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[Export ("removeCredentialIdentities:completion:")]
		void RemoveCredentialIdentities (ASPasswordCredentialIdentity[] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[Export ("removeAllCredentialIdentitiesWithCompletion:")]
		void RemoveAllCredentialIdentities ([NullAllowed] Action<bool, NSError> completion);

		[Async]
		[Export ("replaceCredentialIdentitiesWithIdentities:completion:")]
		void ReplaceCredentialIdentities (ASPasswordCredentialIdentity[] newCredentialIdentities, [NullAllowed] Action<bool, NSError> completion);
	}

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStoreState {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("supportsIncrementalUpdates")]
		bool SupportsIncrementalUpdates { get; }
	}

	[iOS (12,0)]
	[BaseType (typeof (NSExtensionContext))]
	[DisableDefaultCtor]
	interface ASCredentialProviderExtensionContext {
		[Export ("completeRequestWithSelectedCredential:completionHandler:")]
		void CompleteRequest (ASPasswordCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Export ("completeExtensionConfigurationRequest")]
		void CompleteExtensionConfigurationRequest ();

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);
	}

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialServiceIdentifier : NSCopying, NSSecureCoding {
		[Export ("initWithIdentifier:type:")]
		IntPtr Constructor (string identifier, ASCredentialServiceIdentifierType type);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("type")]
		ASCredentialServiceIdentifierType Type { get; }
	}

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredentialIdentity : NSCopying, NSSecureCoding {
		[Export ("initWithServiceIdentifier:user:recordIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		[Static]
		[Export ("identityWithServiceIdentifier:user:recordIdentifier:")]
		ASPasswordCredentialIdentity Create (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		[Export ("serviceIdentifier", ArgumentSemantic.Strong)]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[Export ("user")]
		string User { get; }

		[NullAllowed, Export ("recordIdentifier")]
		string RecordIdentifier { get; }

		[Export ("rank")]
		nint Rank { get; set; }
	}

	[iOS (12,0)]
	[BaseType (typeof (UIViewController))]
	interface ASCredentialProviderViewController {
		[Export ("extensionContext", ArgumentSemantic.Strong)]
		ASCredentialProviderExtensionContext ExtensionContext { get; }

		[Export ("prepareCredentialListForServiceIdentifiers:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier[] serviceIdentifiers);

		[Export ("provideCredentialWithoutUserInteractionForIdentity:")]
		void ProvideCredentialWithoutUserInteraction (ASPasswordCredentialIdentity credentialIdentity);

		[Export ("prepareInterfaceToProvideCredentialForIdentity:")]
		void PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity credentialIdentity);

		[Export ("prepareInterfaceForExtensionConfiguration")]
		void PrepareInterfaceForExtensionConfiguration ();
	}

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredential : NSCopying, NSSecureCoding {
		[Export ("initWithUser:password:")]
		IntPtr Constructor (string user, string password);

		[Static]
		[Export ("credentialWithUser:password:")]
		ASPasswordCredential Create (string user, string password);

		[Export ("user")]
		string User { get; }

		[Export ("password")]
		string Password { get; }
	}

	delegate void ASWebAuthenticationSessionCompletionHandler ([NullAllowed] NSUrl callbackUrl, [NullAllowed] NSError error);

	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSession {
		[Export ("initWithURL:callbackURLScheme:completionHandler:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] string callbackUrlScheme, ASWebAuthenticationSessionCompletionHandler completionHandler);

		[Export ("start")]
		bool Start ();

		[Export ("cancel")]
		void Cancel ();
	}
}
