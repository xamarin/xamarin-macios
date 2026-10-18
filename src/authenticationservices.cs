//
// AuthenticationServices bindings
//
// Copyright 2018-2019 Microsoft Corporation
//

using Security;
using System.ComponentModel;
#if MONOMAC
using AppKit;
using UIControl = AppKit.NSControl;
using UIViewController = AppKit.NSViewController;
using UIWindow = AppKit.NSWindow;
#else
using UIKit;
#endif
#if !TVOS
using LocalAuthentication;
#endif
#if TVOS
using LAContext = Foundation.NSObject;
#endif

namespace AuthenticationServices {

	/// <summary>Enumerates error codes associated with authentication service store requests.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	[ErrorDomain ("ASCredentialIdentityStoreErrorDomain")]
	public enum ASCredentialIdentityStoreErrorCode : long {
		/// <summary>To be added.</summary>
		InternalError = 0,
		/// <summary>To be added.</summary>
		StoreDisabled = 1,
		/// <summary>To be added.</summary>
		StoreBusy = 2,
	}

	/// <summary>Error codes associated with Authentication Services extensions.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	[ErrorDomain ("ASExtensionErrorDomain")]
	public enum ASExtensionErrorCode : long {
		/// <summary>A general failure.</summary>
		Failed = 0,
		/// <summary>The user initiated the cancellation of the authentication request.</summary>
		UserCanceled = 1,
		/// <summary>Additional user interaction is required.</summary>
		UserInteractionRequired = 100,
		/// <summary>The identity could not be found.</summary>
		CredentialIdentityNotFound = 101,
		MatchedExcludedCredential = 102,
	}

	[Partial]
	interface ASExtensionErrorCodeExtensions {
		// Type `ASExtensionErrorCode` is already decorated, so it becomes a duplicate (after code gen)
		// on those platforms and intro tests complains (on other platforms, e.g. iOS, Catalyst)
		// OTOH if we don't add them here then we'll get the extra, not really usable, extension type
		// on tvOS and watchOS (which is incorrect)
		[NoTV]
		[NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("ASExtensionLocalizedFailureReasonErrorKey")]
		NSString LocalizedFailureReasonErrorKey { get; }
	}

	/// <summary>Enumerates the types of service identified.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	public enum ASCredentialServiceIdentifierType : long {
		/// <summary>The identifier specifies a domain.</summary>
		Domain,
		/// <summary>The identifier specifies a URL.</summary>
		Url,
		/// <summary>The identifier specifies an app.</summary>
		[MacCatalyst (26, 2), iOS (26, 2), Mac (26, 2)]
		App,
	}

	/// <summary>Enumerates errors associated with a <see cref="AuthenticationServices.ASWebAuthenticationSession" />.</summary>
	/// <remarks>To be added.</remarks>
	[TV (16, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("ASWebAuthenticationSessionErrorDomain")]
	public enum ASWebAuthenticationSessionErrorCode : long {
		/// <summary>The login was cancelled.</summary>
		CanceledLogin = 1,
		PresentationContextNotProvided = 2,
		PresentationContextInvalid = 3,
	}

	[Flags, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum ASAuthorizationControllerRequestOptions : ulong {
		ImmediatelyAvailableCredentials = 1uL << 0,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionAuthenticationMethod : long {
		Password = 1,
		UserSecureEnclaveKey = 2,
		SmartCard = 3,
		[Mac (27, 0)]
		OpenId = 5,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionKeyType : long {
		DeviceSigning = 1,
		DeviceEncryption = 2,
		SecureEnclaveKey = 3,
		SharedDeviceSigning = 4,
		SharedDeviceEncryption = 5,
		CurrentDeviceSigning = 10,
		CurrentDeviceEncryption = 11,
		UserSmartCard = 20,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionRegistrationResult : long {
		Success = 0,
		Failed = 1,
		UserInterfaceRequired = 2,
		FailedNoRetry = 3,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Flags]
	[Native]
	public enum ASAuthorizationProviderExtensionRequestOptions : ulong {
		None = 0x0,
		UserInteractionEnabled = 1uL << 0,
		RegistrationRepair = 1uL << 1,
		RegistrationSharedDeviceKeys = 1uL << 2,
		RegistrationDeviceKeyMigration = 1uL << 3,
		[Mac (15, 0)]
		StrongerKeyAvailable = 1uL << 4,
		[Mac (14, 4)]
		UserKeyInvalid = 1uL << 5,
		[Mac (26, 0)]
		SetupAssistant = 1 << 6,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (16, 4)]
	[Native]
	public enum ASAuthorizationWebBrowserPublicKeyCredentialManagerAuthorizationState : long {
		Authorized,
		Denied,
		NotDetermined,
	}

	[NoTV, iOS (17, 0), MacCatalyst (14, 0)]
	[Native]
	public enum ASCredentialRequestType : long {
		Password = 0,
		PasskeyAssertion,
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		PasskeyRegistration,
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		OneTimeCode,
	}

	[NoTV, Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
	[Flags]
	[Native]
	public enum ASCredentialIdentityTypes : ulong {
		All = 0,
		Password = 1,
		Passkey = 1uL << 1,
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		OneTimeCode = 1uL << 2,
	}

	[NoTV, Mac (14, 4), NoiOS, NoMacCatalyst]
	[Flags]
	[Native]
	public enum ASAuthorizationProviderExtensionUserSecureEnclaveKeyBiometricPolicy : ulong {
		None = 0x0,
		TouchIdOrWatchCurrentSet = 1uL << 0,
		TouchIdOrWatchAny = 1uL << 1,
		ReuseDuringUnlock = 1uL << 2,
		PasswordFallback = 1uL << 3,
	}

	/// <summary>Delegate used in callbacks by <see cref="AuthenticationServices.ASCredentialIdentityStore" />.</summary>
	delegate void ASCredentialIdentityStoreCompletionHandler (bool success, [NullAllowed] NSError error);
	delegate void ASCredentialIdentityStoreGetCredentialIdentitiesHandler (IASCredentialIdentity [] credentialIdentities);

	/// <summary>A class whose shared instance (see <see cref="AuthenticationServices.ASCredentialIdentityStore.SharedStore" />) holds credentials across providers.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStore {
		/// <summary>The singleton shared credential identity store.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("sharedStore")]
		ASCredentialIdentityStore SharedStore { get; }

		/// <param name="completion">To be added.</param>
		///         <summary>Retrieves the state of store, which is passed as an argument to the <paramref name="completion" /> handler.</summary>
		///         <remarks>To be added.</remarks>
		[Async (XmlDocs = """
			<summary>Asynchronously gets the state of the identity store.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("getCredentialIdentityStoreStateWithCompletion:")]
		void GetCredentialIdentityStoreState (Action<ASCredentialIdentityStoreState> completion);

		[Async]
		[Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("getCredentialIdentitiesForService:credentialIdentityTypes:completionHandler:")]
		void GetCredentialIdentities ([NullAllowed] ASCredentialServiceIdentifier serviceIdentifier, [NullAllowed] ASCredentialIdentityTypes credentialIdentityTypes, ASCredentialIdentityStoreGetCredentialIdentitiesHandler completion);

		/// <param name="credentialIdentities">To be added.</param>
		///         <param name="completion">To be added.</param>
		///         <summary>Saves (or replaces, if the store does not support incremental updates) the <paramref name="credentialIdentities" /> to the store.</summary>
		///         <remarks>To be added.</remarks>
		[Async (XmlDocs = """
			<param name="credentialIdentities">To be added.</param>
			<summary>Asynchronously saves (or replaces, if the store does not support incremental updates) the <paramref name="credentialIdentities" /> to the store.</summary>
			<returns>The first value will be <see langword="true" /> on success. The second value will be non-null on error.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Export ("saveCredentialIdentities:completion:")]
		void SaveCredentialIdentities (ASPasswordCredentialIdentity [] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		/// <param name="credentialIdentities">To be added.</param>
		///         <param name="completion">To be added.</param>
		///         <summary>Removes the specified <paramref name="credentialIdentities" /> from the store. The <paramref name="completion" /> handler is called after the process completes.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Async (XmlDocs = """
			<param name="credentialIdentities">To be added.</param>
			<summary>Asynchronously removes the specified <paramref name="credentialIdentities" /> from the store. The returned tuple will have a first value of <see langword="true" /> if all identities were removed without error.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("removeCredentialIdentities:completion:")]
		void RemoveCredentialIdentities (ASPasswordCredentialIdentity [] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		/// <param name="completion">To be added.</param>
		///         <summary>Removes all credential identities from the store. The <paramref name="completion" /> handler is called after the process completes.</summary>
		///         <remarks>To be added.</remarks>
		[Async (XmlDocs = """
			<summary>Asynchronously removes all credential identities from the store. The returned tuple will have a first value of <see langword="true" /> if all identities were removed without error.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("removeAllCredentialIdentitiesWithCompletion:")]
		void RemoveAllCredentialIdentities ([NullAllowed] Action<bool, NSError> completion);

		/// <param name="newCredentialIdentities">To be added.</param>
		///         <param name="completion">To be added.</param>
		///         <summary>Replaces the existing identities with the specified <paramref name="newCredentialIdentities" /> from the store. The <paramref name="completion" /> handler is called after the process completes.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Async (XmlDocs = """
			<param name="newCredentialIdentities">To be added.</param>
			<summary>Asynchronously replaces the existing identities with the specified <paramref name="newCredentialIdentities" /> from the store. The returned tuple will have a first value of <see langword="true" /> if all identities were removed without error.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("replaceCredentialIdentitiesWithIdentities:completion:")]
		void ReplaceCredentialIdentities (ASPasswordCredentialIdentity [] newCredentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Async]
		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("saveCredentialIdentityEntries:completion:")]
		void SaveCredentialIdentityEntries (IASCredentialIdentity [] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("replaceCredentialIdentityEntries:completion:")]
		void ReplaceCredentialIdentityEntries (IASCredentialIdentity [] newCredentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeCredentialIdentityEntries:completion:")]
		void RemoveCredentialIdentityEntries (IASCredentialIdentity [] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);
	}

	/// <summary>Data related to the availability and capability of the credential identity store.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStoreState {
		/// <summary>Gets whether the credential identity store is enabled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		/// <summary>Gets whether the credential identity store can be incrementally updated.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("supportsIncrementalUpdates")]
		bool SupportsIncrementalUpdates { get; }
	}

	/// <summary>Delegate object for completion handlers in methods within <see cref="ASCredentialProviderExtensionContext" />.</summary>
	delegate void ASCredentialProviderExtensionRequestCompletionHandler (bool expired);

	/// <summary>An <see cref="NSExtensionContext" /> subclass that provides context for a credential provider.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSExtensionContext))]
	[DisableDefaultCtor]
	interface ASCredentialProviderExtensionContext {
		/// <param name="credential">To be added.</param>
		///         <param name="completionHandler">To be added.</param>
		///         <summary>Completes the request by providing <paramref name="credential" />.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("completeRequestWithSelectedCredential:completionHandler:")]
		void CompleteRequest (ASPasswordCredential credential, [NullAllowed] ASCredentialProviderExtensionRequestCompletionHandler completionHandler);

		/// <summary>Called to complete the request and dismiss the associated view controller.</summary>
		[Export ("completeExtensionConfigurationRequest")]
		void CompleteExtensionConfigurationRequest ();

		/// <param name="error">The error must be of type <see cref="AuthenticationServices.ASExtensionErrorCode" />.</param>
		///         <summary>Cancels the request.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);

		[Async]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("completeRegistrationRequestWithSelectedPasskeyCredential:completionHandler:")]
		void CompleteRegistrationRequest (ASPasskeyRegistrationCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("completeAssertionRequestWithSelectedPasskeyCredential:completionHandler:")]
		void CompleteAssertionRequest (ASPasskeyAssertionCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("completeOneTimeCodeRequestWithSelectedCredential:completionHandler:")]
		void CompleteOneTimeCodeRequest (ASOneTimeCodeCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, NoMac, iOS (18, 0), NoMacCatalyst]
		[Export ("completeRequestWithTextToInsert:completionHandler:")]
		void CompleteRequest (string textToInsert, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("completeSavePasswordRequestWithCompletionHandler:")]
		void CompleteSavePasswordRequest ([NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("completeGeneratePasswordRequestWithResults:completionHandler:")]
		void CompleteGeneratePasswordRequest (ASGeneratedPassword [] results, [NullAllowed] Action<bool> completionHandler);
	}

	/// <summary>Holds the identification for a credential service.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialServiceIdentifier : NSCopying, NSSecureCoding {
		/// <param name="identifier">To be added.</param>
		/// <param name="type">To be added.</param>
		/// <summary>Constructs a new <see cref="AuthenticationServices.ASCredentialServiceIdentifier" /> with the specified <paramref name="identifier" /> and of the specified <paramref name="type" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithIdentifier:type:")]
		NativeHandle Constructor (string identifier, ASCredentialServiceIdentifierType type);

		/// <summary>Gets the service identifier.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("identifier")]
		string Identifier { get; }

		/// <summary>Gets the <see cref="AuthenticationServices.ASCredentialServiceIdentifierType" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("type")]
		ASCredentialServiceIdentifierType Type { get; }

		/// <summary>
		/// Constructs a new <see cref="AuthenticationServices.ASCredentialServiceIdentifier" /> with the specified <paramref name="identifier" />, of the specified <paramref name="type" />, and with the specified <paramref name="displayName" />.
		/// </summary>
		/// <param name="identifier">To be added.</param>
		/// <param name="type">To be added.</param>
		/// <param name="displayName">To be added.</param>
		/// <remarks>To be added.</remarks>
		[NoTV, Mac (26, 2), iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("initWithIdentifier:type:displayName:")]
		NativeHandle Constructor (string identifier, ASCredentialServiceIdentifierType type, string displayName);

		/// <summary>Gets the display name.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[NoTV, Mac (26, 2), iOS (26, 2), MacCatalyst (26, 2)]
		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	}

	/// <summary>Associates a <see cref="AuthenticationServices.ASPasswordCredentialIdentity.User" /> string with a record in the developer's credential database.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredentialIdentity : NSCopying, NSSecureCoding, ASCredentialIdentity {
		/// <param name="serviceIdentifier">To be added.</param>
		/// <param name="user">To be added.</param>
		/// <param name="recordIdentifier">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithServiceIdentifier:user:recordIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		/// <param name="serviceIdentifier">To be added.</param>
		///         <param name="user">To be added.</param>
		///         <param name="recordIdentifier">To be added.</param>
		///         <summary>Static factory method to create a new <see cref="AuthenticationServices.ASPasswordCredentialIdentity" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("identityWithServiceIdentifier:user:recordIdentifier:")]
		ASPasswordCredentialIdentity Create (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		/// <summary>Gets the <see cref="AuthenticationServices.ASCredentialServiceIdentifier" /> that provides a hint as to when the credential should be displayed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("serviceIdentifier", ArgumentSemantic.Strong)]
		new ASCredentialServiceIdentifier ServiceIdentifier { get; }

		/// <summary>A user-meaningful name to help identify the credential.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("user")]
		new string User { get; }

		/// <summary>Gets the string associating this identity to a record in the developer's database. (May be <see langword="null" />.)</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("recordIdentifier")]
		new string RecordIdentifier { get; }

		/// <summary>Gets or sets the priority for the credential identity.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("rank")]
		new nint Rank { get; set; }
	}

	/// <summary>System-provided standard <see cref="UIViewController" /> for presenting a credential provider extension.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (UIViewController))]
	interface ASCredentialProviderViewController {
		/// <summary>The <see cref="AuthenticationServices.ASCredentialProviderExtensionContext" /> of the provider.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("extensionContext", ArgumentSemantic.Strong)]
		ASCredentialProviderExtensionContext ExtensionContext { get; }

		/// <param name="serviceIdentifiers">Zero or more service identifiers. More-specific identifiers are at lower index values.</param>
		///         <summary>Developers should override this method to prepare a list of credentials for the <paramref name="serviceIdentifiers" />.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("prepareCredentialListForServiceIdentifiers:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers);

		/// <param name="credentialIdentity">To be added.</param>
		///         <summary>Developers should override this method to attempt to provide the credential without user interaction.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Export ("provideCredentialWithoutUserInteractionForIdentity:")]
		void ProvideCredentialWithoutUserInteraction (ASPasswordCredentialIdentity credentialIdentity);

		/// <param name="credentialIdentity">To be added.</param>
		///         <summary>Developers should override this method which is called shortly before the user is shown the interface for the credential.</summary>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Export ("prepareInterfaceToProvideCredentialForIdentity:")]
		void PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity credentialIdentity);

		/// <summary>Developers should override this method to prepare for the user-experience of enabling the developer's extension.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("prepareInterfaceForExtensionConfiguration")]
		void PrepareInterfaceForExtensionConfiguration ();

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareCredentialListForServiceIdentifiers:requestParameters:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers, ASPasskeyCredentialRequestParameters requestParameters);

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("provideCredentialWithoutUserInteractionForRequest:")]
		void ProvideCredentialWithoutUserInteraction (IASCredentialRequest credentialRequest);

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareInterfaceToProvideCredentialForRequest:")]
		void PrepareInterfaceToProvideCredential (IASCredentialRequest credentialRequest);

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareInterfaceForPasskeyRegistration:")]
		void PrepareInterfaceForPasskeyRegistration (IASCredentialRequest registrationRequest);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prepareOneTimeCodeCredentialListForServiceIdentifiers:")]
		void PrepareOneTimeCodeCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers);

		[NoTV, NoMac, iOS (18, 0), NoMacCatalyst]
		[Export ("prepareInterfaceForUserChoosingTextToInsert")]
		void PrepareInterfaceForUserChoosingTextToInsert ();

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("performPasskeyRegistrationWithoutUserInteractionIfPossible:")]
		void PerformPasskeyRegistrationWithoutUserInteractionIfPossible (ASPasskeyCredentialRequest registrationRequest);

		[iOS (26, 0), Mac (26, 0), MacCatalyst (26, 0), NoTV]
		[Export ("reportPublicKeyCredentialUpdateForRelyingParty:userHandle:newName:")]
		void ReportPublicKeyCredentialUpdate (string relyingParty, NSData userHandle, string newName);

		[iOS (26, 0), Mac (26, 0), MacCatalyst (26, 0), NoTV]
		[Export ("reportUnknownPublicKeyCredentialForRelyingParty:credentialID:")]
		void ReportUnknownPublicKeyCredential (string relyingParty, NSData credentialId);

		[iOS (26, 0), Mac (26, 0), MacCatalyst (26, 0), NoTV]
		[Export ("reportAllAcceptedPublicKeyCredentialsForRelyingParty:userHandle:acceptedCredentialIDs:")]
		void ReportAllAcceptedPublicKeyCredentials (string relyingParty, NSData userHandle, NSData [] acceptedCredentialIds);

		[iOS (26, 0), Mac (26, 0), MacCatalyst (26, 0), NoTV]
		[Export ("reportUnusedPasswordCredentialForDomain:userName:")]
		void ReportUnusedPasswordCredential (string domain, string userName);

		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("performSavePasswordRequestWithoutUserInteractionIfPossible:")]
		void PerformSavePasswordRequestWithoutUserInteractionIfPossible (ASSavePasswordRequest savePasswordRequest);

		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("prepareInterfaceForSavePasswordRequest:")]
		void PrepareInterfaceForSavePasswordRequest (ASSavePasswordRequest savePasswordRequest);

		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("performGeneratePasswordsRequestWithoutUserInteraction:")]
		void PerformGeneratePasswordsRequestWithoutUserInteraction (ASGeneratePasswordsRequest generatePasswordsRequest);

		[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
		[Export ("prepareInterfaceForGeneratePasswordsRequest:")]
		void PrepareInterfaceForGeneratePasswordsRequest (ASGeneratePasswordsRequest generatePasswordsRequest);
	}

	/// <summary>Associates a username and a password.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredential : NSCopying, NSSecureCoding, ASAuthorizationCredential {
		/// <param name="user">To be added.</param>
		/// <param name="password">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithUser:password:")]
		NativeHandle Constructor (string user, string password);

		/// <param name="user">To be added.</param>
		///         <param name="password">To be added.</param>
		///         <summary>Static factory methods to create a new <see cref="AuthenticationServices.ASPasswordCredential" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("credentialWithUser:password:")]
		ASPasswordCredential Create (string user, string password);

		/// <summary>The identifier associated with the password.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("user")]
		string User { get; }

		/// <summary>The password associated with the user.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("password")]
		string Password { get; }
	}

	/// <param name="callbackUrl">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>Delegate method used in <see cref="AuthenticationServices.ASWebAuthenticationSession" /> interactions.</summary>
	/// <remarks>To be added.</remarks>
	delegate void ASWebAuthenticationSessionCompletionHandler ([NullAllowed] NSUrl callbackUrl, [NullAllowed] NSError error);

	/// <summary>Manages a one-time Safari login experience for the developer's app.</summary>
	/// <remarks>To be added.</remarks>
	[TV (16, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSession {

		/// <param name="url">To be added.</param>
		/// <param name="callbackUrlScheme">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.TvOS, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Export ("initWithURL:callbackURLScheme:completionHandler:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] string callbackUrlScheme, ASWebAuthenticationSessionCompletionHandler completionHandler);

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("initWithURL:callback:completionHandler:")]
		NativeHandle Constructor (NSUrl url, ASWebAuthenticationSessionCallback callback, ASWebAuthenticationSessionCompletionHandler completionHandler);

		/// <summary>Begins the Safari-based logon, returning <see langword="true" /> if the session started successfully.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("start")]
		bool Start ();

		/// <summary>Developers can call this method to cancel the authentication session and dismiss the associated view controller.</summary>
		/// <remarks>
		///   <para>After the initial call to this method, subsequent calls have no effect.</para>
		/// </remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cancel")]
		void Cancel ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASWebAuthenticationPresentationContextProviding PresentationContextProvider { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("prefersEphemeralWebBrowserSession")]
		bool PrefersEphemeralWebBrowserSession { get; set; }

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[NullAllowed, Export ("additionalHeaderFields", ArgumentSemantic.Assign)]
		NSDictionary AdditionalHeaderFields { get; set; }

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("canStart")]
		bool CanStart { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorization {

		// Unfortunately Apple returns an internal wrapper type that can quack, bark and moo
		// depending on context of the request and all objects that implement IASAuthorizationProvider
		// are not related between them.
		[Internal]
		[Export ("provider", ArgumentSemantic.Strong)]
		IntPtr _Provider { get; }

		// Unfortunately Apple returns an internal wrapper type that can quack, bark and moo
		// depending on context of the request and all objects that implement IASAuthorizationCredential
		// are not related between them.
		[Internal]
		[Export ("credential", ArgumentSemantic.Strong)]
		IntPtr _Credential { get; }
	}

	[MacCatalyst (13, 1)]
	enum ASAuthorizationScope {
		[Field ("ASAuthorizationScopeFullName")]
		FullName,
		[Field ("ASAuthorizationScopeEmail")]
		Email,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum ASUserDetectionStatus : long {
		Unsupported,
		Unknown,
		LikelyReal,
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement : long {
		Required,
		Preferred,
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobAssertionOperation : long {
		Read,
		Write,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASPublicKeyCredentialClientDataCrossOriginValue : long {
		NotSet,
		CrossOrigin,
		SameOriginWithAncestors,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialAttachment : long {
		Platform,
		CrossPlatform,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionFederationType : long {
		None = 0,
		WSTrust = 1,
		DynamicWSTrust = 2,
		[Mac (27, 0)]
		OpenId = 3,
		[Mac (27, 0)]
		DynamicOpenId = 4,
	}

	[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum ASUserAgeRange : long {
		Unknown,
		Child,
		NotChild,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationAppleIDCredential")]
	[DisableDefaultCtor]
	interface ASAuthorizationAppleIdCredential : ASAuthorizationCredential {

		[Export ("user")]
		string User { get; }

		[NullAllowed, Export ("state")]
		string State { get; }

		[Export ("authorizedScopes", ArgumentSemantic.Copy)]
		[BindAs (typeof (ASAuthorizationScope []))]
		NSString [] AuthorizedScopes { get; }

		[NullAllowed, Export ("authorizationCode", ArgumentSemantic.Copy)]
		NSData AuthorizationCode { get; }

		[NullAllowed, Export ("identityToken", ArgumentSemantic.Copy)]
		NSData IdentityToken { get; }

		[NullAllowed, Export ("email")]
		string Email { get; }

		[NullAllowed, Export ("fullName", ArgumentSemantic.Copy)]
		NSPersonNameComponents FullName { get; }

		[Export ("realUserStatus")]
		ASUserDetectionStatus RealUserStatus { get; }

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("userAgeRange")]
		ASUserAgeRange UserAgeRange { get; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdProviderCredentialState : long {
		Revoked,
		Authorized,
		NotFound,
		Transferred,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationAppleIDProvider")]
	interface ASAuthorizationAppleIdProvider : ASAuthorizationProvider {

		[Export ("createRequest")]
		ASAuthorizationAppleIdRequest CreateRequest ();

		[Export ("getCredentialStateForUserID:completion:")]
		[Async]
		void GetCredentialState (string userID, Action<ASAuthorizationAppleIdProviderCredentialState, NSError> completion);

		[Notification]
		[Field ("ASAuthorizationAppleIDProviderCredentialRevokedNotification")]
		NSString CredentialRevokedNotification { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest), Name = "ASAuthorizationAppleIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationAppleIDRequest init]: unrecognized selector sent to instance 0x600002ff8b40 
	interface ASAuthorizationAppleIdRequest {

		[NullAllowed, Export ("user")]
		string User { get; set; }
	}

	interface IASAuthorizationControllerDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationControllerDelegate {

		[Export ("authorizationController:didCompleteWithAuthorization:")]
		void DidComplete (ASAuthorizationController controller, ASAuthorization authorization);

		[Export ("authorizationController:didCompleteWithError:")]
		void DidComplete (ASAuthorizationController controller, NSError error);

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("authorizationController:didCompleteWithCustomMethod:")]
		void DidComplete (ASAuthorizationController controller, NSString method);
	}

	interface IASAuthorizationControllerPresentationContextProviding { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAuthorizationController:")]
		UIWindow GetPresentationAnchor (ASAuthorizationController controller);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationController {

		[Export ("initWithAuthorizationRequests:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASAuthorizationRequest [] authorizationRequests);

		[Export ("authorizationRequests", ArgumentSemantic.Strong)]
		ASAuthorizationRequest [] AuthorizationRequests { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IASAuthorizationControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASAuthorizationControllerPresentationContextProviding PresentationContextProvider { get; set; }

		[Export ("performRequests")]
		void PerformRequests ();

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("customAuthorizationMethods", ArgumentSemantic.Copy)]
		NSString [] CustomAuthorizationMethods { get; set; }

		[NoTV, NoMacCatalyst, NoMac, iOS (16, 0)]
		[Export ("performAutoFillAssistedRequests")]
		void PerformAutoFillAssistedRequests ();

		[NoTV, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("performRequestsWithOptions:")]
		void PerformRequests (ASAuthorizationControllerRequestOptions options);

		[TV (18, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("cancel")]
		void Cancel ();
	}

	interface IASAuthorizationCredential { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationCredential : NSCopying, NSSecureCoding { }

	[MacCatalyst (13, 1)]
	[ErrorDomain ("ASAuthorizationErrorDomain")]
	[Native]
	public enum ASAuthorizationError : long {
		Unknown = 1000,
		Canceled = 1001,
		InvalidResponse = 1002,
		NotHandled = 1003,
		Failed = 1004,
		NotInteractive = 1005,
		MatchedExcludedCredential = 1006,
		CredentialImport = 1007,
		CredentialExport = 1008,
		PreferSignInWithApple = 1009,
		DeviceNotConfiguredForPasskeyCreation = 1010,
	}

	[MacCatalyst (13, 1)]
	enum ASAuthorizationOperation {
		[Field ("ASAuthorizationOperationImplicit")]
		Implicit,

		[Field ("ASAuthorizationOperationLogin")]
		Login,

		[Field ("ASAuthorizationOperationRefresh")]
		Refresh,

		[Field ("ASAuthorizationOperationLogout")]
		Logout,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationRequest), Name = "ASAuthorizationOpenIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationOpenIDRequest init]: unrecognized selector sent to instance 0x600002ff0660 
	interface ASAuthorizationOpenIdRequest {

		[NullAllowed, Export ("requestedScopes", ArgumentSemantic.Copy)]
		[BindAs (typeof (ASAuthorizationScope []))]
		NSString [] RequestedScopes { get; set; }

		[NullAllowed, Export ("state")]
		string State { get; set; }

		[NullAllowed, Export ("nonce")]
		string Nonce { get; set; }

		[Export ("requestedOperation")]
		[BindAs (typeof (ASAuthorizationOperation))]
		NSString RequestedOperation { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPasswordProvider : ASAuthorizationProvider {

		[Export ("createRequest")]
		ASAuthorizationPasswordRequest CreateRequest ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationRequest))]
	// Name: NSInvalidArgumentException Reason: -[ASAuthorizationPasswordRequest init]: unrecognized selector sent to instance 0x6000005f2dc0
	[DisableDefaultCtor]
	interface ASAuthorizationPasswordRequest { }

	interface IASAuthorizationProvider { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationProvider { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Protocol]
	interface ASAuthorizationProviderExtensionAuthorizationRequestHandler {

		[Abstract]
		[Export ("beginAuthorizationWithRequest:")]
		void BeginAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);

		[Export ("cancelAuthorizationWithRequest:")]
		void CancelAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	enum ASAuthorizationProviderAuthorizationOperation {
		// no value yet - but we must handle `nil` as a default value
		[DefaultEnumValue]
		[Field (null)]
		None,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationConfigurationRemoved")]
		ConfigurationRemoved,

		[NoTV, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationDirectRequest")]
		DirectRequest,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationProviderExtensionAuthorizationRequest {

		[Export ("doNotHandle")]
		void DoNotHandle ();

		[Export ("cancel")]
		void Cancel ();

		[Export ("complete")]
		void Complete ();

		[Export ("completeWithHTTPAuthorizationHeaders:")]
		void Complete (NSDictionary<NSString, NSString> httpAuthorizationHeaders);

		[Export ("completeWithHTTPResponse:httpBody:")]
		void Complete (NSHttpUrlResponse httpResponse, [NullAllowed] NSData httpBody);

		[Export ("completeWithError:")]
		void Complete (NSError error);

		[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("completeWithAuthorizationResult:")]
		void Complete (ASAuthorizationProviderExtensionAuthorizationResult authorizationResult);

		[Async]
		[Export ("presentAuthorizationViewControllerWithCompletion:")]
		void PresentAuthorizationViewController (Action<bool, NSError> completion);

		[Export ("url")]
		NSUrl Url { get; }

		[Wrap ("ASAuthorizationProviderAuthorizationOperationExtensions.GetValue (WeakRequestedOperation)")]
		ASAuthorizationProviderAuthorizationOperation RequestedOperation { get; }

		[Export ("requestedOperation")]
		NSString WeakRequestedOperation { get; }

		[Export ("httpHeaders")]
		NSDictionary<NSString, NSString> HttpHeaders { get; }

		[Export ("httpBody")]
		NSData HttpBody { get; }

		[Export ("realm")]
		string Realm { get; }

		[Export ("extensionData")]
		NSDictionary ExtensionData { get; }

		[Export ("callerBundleIdentifier")]
		string CallerBundleIdentifier { get; }

		[Export ("authorizationOptions")]
		NSDictionary AuthorizationOptions { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("callerManaged")]
		bool CallerManaged { [Bind ("isCallerManaged")] get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("callerTeamIdentifier")]
		string CallerTeamIdentifier { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("localizedCallerDisplayName")]
		string LocalizedCallerDisplayName { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("userInterfaceEnabled")]
		bool UserInterfaceEnabled { [Bind ("isUserInterfaceEnabled")] get; }

		[NullAllowed]
		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("loginManager", ArgumentSemantic.Strong)]
		ASAuthorizationProviderExtensionLoginManager LoginManager { get; }

		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("callerAuditToken")]
		NSData CallerAuditToken { get; }

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationRequest : NSCopying, NSSecureCoding {

		// Unfortunately Apple returns an internal wrapper type that can quack, bark and moo
		// depending on context of the request and all objects that implement IASAuthorizationProvider
		// are not related between them.
		[Internal]
		[Export ("provider", ArgumentSemantic.Strong)]
		IntPtr _Provider { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSingleSignOnCredential : ASAuthorizationCredential {

		[NullAllowed, Export ("state")]
		string State { get; }

		[NullAllowed, Export ("accessToken", ArgumentSemantic.Copy)]
		NSData AccessToken { get; }

		[NullAllowed, Export ("identityToken", ArgumentSemantic.Copy)]
		NSData IdentityToken { get; }

		[Export ("authorizedScopes", ArgumentSemantic.Copy)]
		[BindAs (typeof (ASAuthorizationScope []))]
		NSString [] AuthorizedScopes { get; }

		[NullAllowed, Export ("authenticatedResponse", ArgumentSemantic.Copy)]
		NSHttpUrlResponse AuthenticatedResponse { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("privateKeys")]
		SecKey [] PrivateKeys { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSingleSignOnProvider : ASAuthorizationProvider {

		[Static]
		[Export ("authorizationProviderWithIdentityProviderURL:")]
		ASAuthorizationSingleSignOnProvider CreateProvider (NSUrl identityProviderUrl);

		[Export ("createRequest")]
		ASAuthorizationSingleSignOnRequest CreateRequest ();

		[Export ("url")]
		NSUrl Url { get; }

		[Export ("canPerformAuthorization")]
		bool CanPerformAuthorization { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationSingleSignOnRequest init]: unrecognized selector sent to instance 0x60000095aa60
	interface ASAuthorizationSingleSignOnRequest {

		[Export ("authorizationOptions", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] AuthorizationOptions { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("userInterfaceEnabled")]
		bool UserInterfaceEnabled { [Bind ("isUserInterfaceEnabled")] get; set; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdButtonType : long {
		SignIn,
		Continue,
		[TV (13, 2), iOS (13, 2)]
		[MacCatalyst (13, 1)]
		SignUp,
		Default = SignIn,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdButtonStyle : long {
		White = 0,
		WhiteOutline = 1,
		Black = 2,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl), Name = "ASAuthorizationAppleIDButton")]
	[DisableDefaultCtor]
#if MONOMAC
	interface ASAuthorizationAppleIdButton : NSAccessibilityButton {
#else
	interface ASAuthorizationAppleIdButton {
#endif

		[Static]
		[Export ("buttonWithType:style:")]
		ASAuthorizationAppleIdButton Create (ASAuthorizationAppleIdButtonType type, ASAuthorizationAppleIdButtonStyle style);

		[Export ("initWithAuthorizationButtonType:authorizationButtonStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASAuthorizationAppleIdButtonType type, ASAuthorizationAppleIdButtonStyle style);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	}

	interface IASWebAuthenticationPresentationContextProviding { }

	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASWebAuthenticationPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForWebAuthenticationSession:")]
		UIWindow GetPresentationAnchor (ASWebAuthenticationSession session);
	}

	interface IASWebAuthenticationSessionRequestDelegate { }

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[NoTV]
	[NoiOS]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface ASWebAuthenticationSessionRequestDelegate {

		[Export ("authenticationSessionRequest:didCompleteWithCallbackURL:")]
		void DidComplete (ASWebAuthenticationSessionRequest authenticationSessionRequest, NSUrl callbackUrl);

		[Export ("authenticationSessionRequest:didCancelWithError:")]
		void DidCancel (ASWebAuthenticationSessionRequest authenticationSessionRequest, NSError error);
	}

	interface IASWebAuthenticationSessionWebBrowserSessionHandling { }

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Protocol]
	interface ASWebAuthenticationSessionWebBrowserSessionHandling {

		[Abstract]
		[Export ("beginHandlingWebAuthenticationSessionRequest:")]
		void BeginHandlingWebAuthenticationSessionRequest (ASWebAuthenticationSessionRequest request);

		[Abstract]
		[Export ("cancelWebAuthenticationSessionRequest:")]
		void CancelWebAuthenticationSessionRequest (ASWebAuthenticationSessionRequest request);
	}

	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[NoTV]
	[NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSessionRequest : NSSecureCoding, NSCopying {

		[Export ("UUID")]
		NSUuid Uuid { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Deprecated (PlatformName.MacOSX, 14, 4, message: "Use 'Callback' to match all callback types.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 4, message: "Use 'Callback' to match all callback types.")]
		[NullAllowed, Export ("callbackURLScheme")]
		string CallbackUrlScheme { get; }

		[Export ("shouldUseEphemeralSession")]
		bool ShouldUseEphemeralSession { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IASWebAuthenticationSessionRequestDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[MacCatalyst (17, 4), Mac (14, 4)]
		[NullAllowed, Export ("additionalHeaderFields")]
		NSDictionary AdditionalHeaderFields { get; }

		[MacCatalyst (17, 4), Mac (14, 4)]
		[NullAllowed, Export ("callback")]
		ASWebAuthenticationSessionCallback Callback { get; }

		[Export ("cancelWithError:")]
		void Cancel (NSError error);

		[Export ("completeWithCallbackURL:")]
		void Complete (NSUrl callbackUrl);
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // implied by `sharedManager`
	interface ASWebAuthenticationSessionWebBrowserSessionManager {

		[Static]
		[Export ("sharedManager")]
		ASWebAuthenticationSessionWebBrowserSessionManager SharedManager { get; }

		[Export ("sessionHandler", ArgumentSemantic.Assign)]
		IASWebAuthenticationSessionWebBrowserSessionHandling SessionHandler { get; set; }

		[Export ("wasLaunchedByAuthenticationServices")]
		bool WasLaunchedByAuthenticationServices { get; }

		[NoMacCatalyst]
		[Static]
		[Export ("registerDefaultsForASWASInSetupAssistantIfNeeded")]
		void RegisterDefaultsForAsWasInSetupAssistantIfNeeded ();

#if !XAMCORE_5_0
		[Obsolete ("Use 'RegisterDefaultsForAsWasInSetupAssistantIfNeeded' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Wrap ("RegisterDefaultsForAsWasInSetupAssistantIfNeeded ()")]
		[NoMacCatalyst]
		[Static]
		void RegisterDefaultsForAswasInSetupAssistantIfNeeded ();
#endif
	}


	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAccountAuthenticationModificationRequest {
	}

	interface IASAccountAuthenticationModificationControllerDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface ASAccountAuthenticationModificationControllerDelegate {

		[Export ("accountAuthenticationModificationController:didSuccessfullyCompleteRequest:withUserInfo:")]
		void DidSuccessfullyCompleteRequest (ASAccountAuthenticationModificationController controller, ASAccountAuthenticationModificationRequest request, [NullAllowed] NSDictionary userInfo);

		[Export ("accountAuthenticationModificationController:didFailRequest:withError:")]
		void DidFailRequest (ASAccountAuthenticationModificationController controller, ASAccountAuthenticationModificationRequest request, NSError error);
	}

	interface IASAccountAuthenticationModificationControllerPresentationContextProviding { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[Protocol]
	interface ASAccountAuthenticationModificationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAccountAuthenticationModificationController:")]
		UIWindow GetPresentationAnchor (ASAccountAuthenticationModificationController controller);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	interface ASAccountAuthenticationModificationController {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IASAccountAuthenticationModificationControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASAccountAuthenticationModificationControllerPresentationContextProviding PresentationContextProvider { get; set; }

		[Export ("performRequest:")]
		void PerformRequest (ASAccountAuthenticationModificationRequest request);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (NSExtensionContext))]
	[DisableDefaultCtor]
	interface ASAccountAuthenticationModificationExtensionContext {

		[Async]
		[Export ("getSignInWithAppleUpgradeAuthorizationWithState:nonce:completionHandler:")]
		void GetSignInWithAppleUpgradeAuthorization ([NullAllowed] string state, [NullAllowed] string nonce, Action<ASAuthorizationAppleIdCredential, NSError> completionHandler);

		[Export ("completeUpgradeToSignInWithAppleWithUserInfo:")]
		void CompleteUpgradeToSignInWithApple ([NullAllowed] NSDictionary userInfo);

		[Export ("completeChangePasswordRequestWithUpdatedCredential:userInfo:")]
		void CompleteChangePasswordRequest (ASPasswordCredential updatedCredential, [NullAllowed] NSDictionary userInfo);

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (ASAccountAuthenticationModificationRequest))]
	[DisableDefaultCtor]
	interface ASAccountAuthenticationModificationReplacePasswordWithSignInWithAppleRequest {

		[Export ("initWithUser:serviceIdentifier:userInfo:")]
		NativeHandle Constructor (string user, ASCredentialServiceIdentifier serviceIdentifier, [NullAllowed] NSDictionary userInfo);

		[Export ("user")]
		string User { get; }

		[Export ("serviceIdentifier")]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[NullAllowed, Export ("userInfo")]
		NSDictionary UserInfo { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (ASAccountAuthenticationModificationRequest))]
	[DisableDefaultCtor]
	interface ASAccountAuthenticationModificationUpgradePasswordToStrongPasswordRequest {

		[Export ("initWithUser:serviceIdentifier:userInfo:")]
		NativeHandle Constructor (string user, ASCredentialServiceIdentifier serviceIdentifier, [NullAllowed] NSDictionary userInfo);

		[Export ("user")]
		string User { get; }

		[Export ("serviceIdentifier")]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[NullAllowed, Export ("userInfo")]
		NSDictionary UserInfo { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoTV, NoMac]
	[BaseType (typeof (UIViewController))]
	interface ASAccountAuthenticationModificationViewController {

		[Export ("extensionContext", ArgumentSemantic.Strong)]
		ASAccountAuthenticationModificationExtensionContext ExtensionContext { get; }

		[Export ("convertAccountToSignInWithAppleWithoutUserInteractionForServiceIdentifier:existingCredential:userInfo:")]
		void ConvertAccountToSignInWithAppleWithoutUserInteraction (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential existingCredential, [NullAllowed] NSDictionary userInfo);

		[Export ("prepareInterfaceToConvertAccountToSignInWithAppleForServiceIdentifier:existingCredential:userInfo:")]
		void PrepareInterfaceToConvertAccountToSignInWithApple (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential existingCredential, [NullAllowed] NSDictionary userInfo);

		[Export ("changePasswordWithoutUserInteractionForServiceIdentifier:existingCredential:newPassword:userInfo:")]
		void ChangePasswordWithoutUserInteraction (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential existingCredential, string newPassword, [NullAllowed] NSDictionary userInfo);

		[Export ("prepareInterfaceToChangePasswordForServiceIdentifier:existingCredential:newPassword:userInfo:")]
		void PrepareInterfaceToChangePassword (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential existingCredential, string newPassword, [NullAllowed] NSDictionary userInfo);

		[Export ("cancelRequest")]
		void CancelRequest ();
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoTV]
	[Static]
	interface ASAuthorizationPublicKeyCredentialAttestationKind {
		[Field ("ASAuthorizationPublicKeyCredentialAttestationKindNone")]
		NSString None { get; }

		[Field ("ASAuthorizationPublicKeyCredentialAttestationKindDirect")]
		NSString Direct { get; }

		[Field ("ASAuthorizationPublicKeyCredentialAttestationKindIndirect")]
		NSString Indirect { get; }

		[Field ("ASAuthorizationPublicKeyCredentialAttestationKindEnterprise")]
		NSString Enterprise { get; }
	}

#if !XAMCORE_5_0
	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Static]
	interface ASAuthorizationPublicKeyCredentialUserVerificationPreference {
		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferencePreferred")]
		NSString Preferred { get; }

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceRequired")]
		NSString Required { get; }

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceDiscouraged")]
		NSString Discouraged { get; }
	}
#endif

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
#if XAMCORE_5_0
	enum ASAuthorizationPublicKeyCredentialUserVerificationPreference {
#else
	enum ASAuthorizationPublicKeyCredentialUserVerificationPreferenceEnum {
#endif
		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferencePreferred")]
		Preferred,

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceRequired")]
		Required,

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceDiscouraged")]
		Discouraged,
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoTV]
	enum ASAuthorizationPublicKeyCredentialResidentKeyPreference {
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferenceDiscouraged")]
		Discouraged,
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferencePreferred")]
		Preferred,
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferenceRequired")]
		Required,
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoTV]
	enum ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport {
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportUSB")]
		Usb,
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportNFC")]
		Nfc,
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportBluetooth")]
		Bluetooth,
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoTV]
	[Native]
	enum ASCoseAlgorithmIdentifier : long {
		ES256 = -7,
	}

	// Introduced in Xcode13 Beta3 but not used anywhere
	// [iOS (15,0), MacCatalyst (15,0), NoTV]
	// [Native]
	// enum AscoseEllipticCurveIdentifier : long {
	// 	P256 = 1,
	// }
	//

	[Flags, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionSupportedGrantTypes : long {
		None = 0x0,
		Password = 1L << 0,
		JwtBearer = 1L << 1,
		Saml11 = 1L << 2,
		Saml20 = 1L << 3,
		[Mac (27, 0)]
		TokenExchange = 1L << 4,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionPlatformSSOProtocolVersion : long {
		Version1_0 = 0,
		Version2_0 = 1,
	}


	interface IASAuthorizationPublicKeyCredentialAssertion { }

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialAssertion : ASPublicKeyCredential {
		[Abstract]
		[Export ("rawAuthenticatorData", ArgumentSemantic.Copy)]
		NSData RawAuthenticatorData { get; }

		[Abstract]
		[Export ("userID", ArgumentSemantic.Copy)]
		NSData UserId { get; }

		[Abstract]
		[Export ("signature", ArgumentSemantic.Copy)]
		NSData Signature { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialAssertionRequest : ASAuthorizationPublicKeyCredentialAssertionRequest {
		/* issues when overriding this property */
		[Sealed]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] PlatformAllowedCredentials { get; set; }

		[NullAllowed]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionInput LargeBlob { get; set; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionInput Prf { get; set; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialAssertionRequest : ASAuthorizationPublicKeyCredentialAssertionRequest {
		/* issues when overriding this property */
		[Sealed]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationSecurityKeyPublicKeyCredentialDescriptor [] SecurityAllowedCredentials { get; set; }

		[Mac (14, 5), iOS (17, 5), MacCatalyst (17, 5)]
		[NullAllowed, Export ("appID")]
		string AppId { get; set; }

		/// <summary>Gets or sets the PRF (Pseudo-Random Function) extension input for the security key credential assertion request.</summary>
		[Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionInput Prf { get; set; }
	}

	interface IASAuthorizationPublicKeyCredentialAssertionRequest { }

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialAssertionRequest : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("challenge", ArgumentSemantic.Copy)]
		NSData Challenge { get; set; }

		[Abstract]
		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; set; }

		[Abstract]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		IASAuthorizationPublicKeyCredentialDescriptor [] AllowedCredentials { get; set; }

		[Abstract]
		[Export ("userVerificationPreference")]
		NSString UserVerificationPreference { get; set; }
	}

	interface IASAuthorizationPublicKeyCredentialDescriptor { }

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialDescriptor : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; set; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialRegistration : ASPublicKeyCredential {
		[Abstract]
		[NullAllowed, Export ("rawAttestationObject", ArgumentSemantic.Copy)]
		NSData RawAttestationObject { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialRegistrationRequest : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }

		[Abstract]
		[Export ("userID", ArgumentSemantic.Copy)]
		NSData UserId { get; set; }

		[Abstract]
		[Export ("name")]
		string Name { get; set; }

		[Abstract]
		[NullAllowed, Export ("displayName")]
		string DisplayName { get; set; }

		[Abstract]
		[Export ("challenge", ArgumentSemantic.Copy)]
		NSData Challenge { get; set; }

		[Abstract]
		[Export ("userVerificationPreference")]
		NSString UserVerificationPreference { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Abstract]
		[Export ("attestationPreference")]
		NSString AttestationPreference { get; set; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASPublicKeyCredential : ASAuthorizationCredential {
		[Abstract]
		[Export ("rawClientDataJSON", ArgumentSemantic.Copy)]
		NSData RawClientDataJson { get; }

		[Abstract]
		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; }
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialDescriptor : ASAuthorizationPublicKeyCredentialDescriptor {
		[Export ("initWithCredentialID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData credentialId);
	}

	[iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialProvider : ASAuthorizationProvider {
		[Export ("initWithRelyingPartyIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string relyingPartyIdentifier);

		[Export ("createCredentialRegistrationRequestWithChallenge:name:userID:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (NSData challenge, string name, NSData userId);

		[Export ("createCredentialAssertionRequestWithChallenge:")]
		ASAuthorizationPlatformPublicKeyCredentialAssertionRequest CreateCredentialAssertionRequest (NSData challenge);

		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("createCredentialRegistrationRequestWithChallenge:name:userID:requestStyle:")]
		void CreateCredentialRegistrationRequest (NSData challenge, string name, NSData userId, ASAuthorizationPlatformPublicKeyCredentialRegistrationRequestStyle requestStyle);
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationProviderExtensionAuthorizationResult {
		[Export ("initWithHTTPAuthorizationHeaders:")]
		NativeHandle Constructor (NSDictionary<NSString, NSString> httpAuthorizationHeaders);

		[Export ("initWithHTTPResponse:httpBody:")]
		NativeHandle Constructor (NSHttpUrlResponse httpResponse, [NullAllowed] NSData httpBody);

		[NullAllowed, Export ("httpAuthorizationHeaders", ArgumentSemantic.Assign)]
		NSDictionary<NSString, NSString> HttpAuthorizationHeaders { get; set; }

		[NullAllowed, Export ("httpResponse", ArgumentSemantic.Copy)]
		NSHttpUrlResponse HttpResponse { get; set; }

		[NullAllowed, Export ("httpBody", ArgumentSemantic.Assign)]
		NSData HttpBody { get; set; }

		[Export ("privateKeys", ArgumentSemantic.Assign)]
		SecKey [] PrivateKeys { get; set; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialParameters : NSSecureCoding, NSCopying {
		[Export ("initWithAlgorithm:")]
		NativeHandle Constructor (ASCoseAlgorithmIdentifier algorithm);

		[Export ("algorithm")]
		ASCoseAlgorithmIdentifier Algorithm { get; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialDescriptor : ASAuthorizationPublicKeyCredentialDescriptor {
		[Export ("initWithCredentialID:transports:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData credentialId, [BindAs (typeof (ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport []))] NSString [] allowedTransports);

		[Export ("transports", ArgumentSemantic.Assign)]
		[BindAs (typeof (ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport []))]
		NSString [] Transports { get; set; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialProvider : ASAuthorizationProvider {
		[Export ("initWithRelyingPartyIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string relyingPartyIdentifier);

		[Export ("createCredentialRegistrationRequestWithChallenge:displayName:name:userID:")]
		ASAuthorizationSecurityKeyPublicKeyCredentialRegistrationRequest Create (NSData challenge, string displayName, string name, NSData userId);

		[Export ("createCredentialAssertionRequestWithChallenge:")]
		ASAuthorizationSecurityKeyPublicKeyCredentialAssertionRequest Create (NSData challenge);

		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialRegistrationRequest : ASAuthorizationPublicKeyCredentialRegistrationRequest {
		[Export ("credentialParameters", ArgumentSemantic.Copy)]
		ASAuthorizationPublicKeyCredentialParameters [] CredentialParameters { get; set; }

		[Export ("excludedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationSecurityKeyPublicKeyCredentialDescriptor [] ExcludedCredentials { get; set; }

		[Export ("residentKeyPreference")]
		[BindAs (typeof (ASAuthorizationPublicKeyCredentialResidentKeyPreference))]
		NSString ResidentKeyPreference { get; set; }

		/// <summary>Gets or sets the PRF (Pseudo-Random Function) extension input for the security key credential registration request.</summary>
		[Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfRegistrationInput Prf { get; set; }
	}

	[TV (15, 0), NoiOS, NoMac, NoMacCatalyst]
	[Static]
	interface ASAuthorizationCustomMethod {
		[Field ("ASAuthorizationCustomMethodVideoSubscriberAccount")]
		NSString SubscriberAccount { get; }

		[Field ("ASAuthorizationCustomMethodRestorePurchase")]
		NSString RestorePurchase { get; }

		[Field ("ASAuthorizationCustomMethodOther")]
		NSString Other { get; }
	}

	[TV (16, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPlatformPublicKeyCredentialAssertion : ASAuthorizationPublicKeyCredentialAssertion {

		[iOS (17, 0), NoTV, MacCatalyst (17, 0)]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[Obsolete ("Use 'LargeBlob2' instead, this property has an incorrect property type..")]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
#if XAMCORE_5_0
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput LargeBlob { get; }
#else
		[Sealed]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput LargeBlob2 { get; }
#endif

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionOutput Prf { get; }
	}

	[TV (16, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest : ASAuthorizationPublicKeyCredentialRegistrationRequest {

		[NullAllowed]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; set; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfRegistrationInput Prf { get; set; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("requestStyle")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequestStyle RequestStyle { get; set; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialAssertion : ASAuthorizationPublicKeyCredentialAssertion {

		[Mac (14, 5), iOS (17, 5), MacCatalyst (17, 5)]
		[Export ("appID")]
		bool AppId { get; }

		/// <summary>Gets the PRF (Pseudo-Random Function) extension output from the security key credential assertion.</summary>
		[Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionOutput Prf { get; }
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {

		[Mac (14, 5), iOS (17, 5), MacCatalyst (17, 5)]
		[Export ("transports", ArgumentSemantic.Assign)]
		[BindAs (typeof (ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport []))]
		NSString [] Transports { get; }

		/// <summary>Gets the PRF (Pseudo-Random Function) extension output from the security key credential registration.</summary>
		[Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfRegistrationOutput Prf { get; }
	}

	[TV (16, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPlatformPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {

		[iOS (17, 0), NoTV, MacCatalyst (17, 0)]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[NullAllowed]
		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfRegistrationOutput Prf { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationProviderExtensionKerberosMapping {
		[NullAllowed, Export ("ticketKeyPath")]
		string TicketKeyPath { get; set; }

		[NullAllowed, Export ("messageBufferKeyName")]
		string MessageBufferKeyName { get; set; }

		[NullAllowed, Export ("realmKeyName")]
		string RealmKeyName { get; set; }

		[NullAllowed, Export ("serviceNameKeyName")]
		string ServiceNameKeyName { get; set; }

		[NullAllowed, Export ("clientNameKeyName")]
		string ClientNameKeyName { get; set; }

		[NullAllowed, Export ("encryptionKeyTypeKeyName")]
		string EncryptionKeyTypeKeyName { get; set; }

		[NullAllowed, Export ("sessionKeyKeyName")]
		string SessionKeyKeyName { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationProviderExtensionLoginConfiguration {
		[Export ("initWithClientID:issuer:tokenEndpointURL:jwksEndpointURL:audience:")]
		NativeHandle Constructor (string clientId, string issuer, NSUrl tokenEndpointUrl, NSUrl jwksEndpointUrl, [NullAllowed] string audience);

		[Async]
		[Static]
		[Export ("configurationWithOpenIDConfigurationURL:clientID:issuer:completion:")]
		void Create (NSUrl openIdConfigurationUrl, string clientId, [NullAllowed] string issuer, Action<ASAuthorizationProviderExtensionLoginConfiguration, NSError> handler);

		[NullAllowed, Export ("invalidCredentialPredicate")]
		string InvalidCredentialPredicate { get; set; }

		[NullAllowed, Export ("accountDisplayName")]
		string AccountDisplayName { get; set; }

		[Export ("clientID")]
		string ClientId { get; }

		[Export ("issuer")]
		string Issuer { get; }

		[Export ("audience")]
		string Audience { get; set; }

		[Export ("tokenEndpointURL", ArgumentSemantic.Copy)]
		NSUrl TokenEndpointUrl { get; set; }

		[Export ("jwksEndpointURL", ArgumentSemantic.Copy)]
		NSUrl JwksEndpointUrl { get; set; }

		[Export ("nonceEndpointURL", ArgumentSemantic.Copy)]
		NSUrl NonceEndpointUrl { get; set; }

		[Export ("nonceResponseKeypath")]
		string NonceResponseKeypath { get; set; }

		[Export ("serverNonceClaimName")]
		string ServerNonceClaimName { get; set; }

		[Export ("customNonceRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomNonceRequestValues { get; set; }

		/// <summary>Gets or sets the expiration time for a server nonce.</summary>
		[Mac (27, 2)]
		[Export ("serverNonceExpirationTime", ArgumentSemantic.Copy)]
		NSNumber ServerNonceExpirationTime { get; set; }

		[Export ("setCustomAssertionRequestHeaderClaims:returningError:")]
		bool SetCustomAssertionRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomAssertionRequestBodyClaims:returningError:")]
		bool SetCustomAssertionRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("additionalScopes")]
		string AdditionalScopes { get; set; }

		[Export ("includePreviousRefreshTokenInLoginRequest")]
		bool IncludePreviousRefreshTokenInLoginRequest { get; set; }

		[Export ("previousRefreshTokenClaimName")]
		string PreviousRefreshTokenClaimName { get; set; }

		[Export ("customLoginRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomLoginRequestValues { get; set; }

		[Export ("setCustomLoginRequestHeaderClaims:returningError:")]
		bool SetCustomLoginRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomLoginRequestBodyClaims:returningError:")]
		bool SetCustomLoginRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("kerberosTicketMappings", ArgumentSemantic.Copy)]
		ASAuthorizationProviderExtensionKerberosMapping [] KerberosTicketMappings { get; set; }

		[NullAllowed]
		[Export ("deviceContext", ArgumentSemantic.Copy)]
		NSData DeviceContext { get; set; }

		[Mac (14, 4)]
		[Export ("userSecureEnclaveKeyBiometricPolicy", ArgumentSemantic.Assign)]
		ASAuthorizationProviderExtensionUserSecureEnclaveKeyBiometricPolicy UserSecureEnclaveKeyBiometricPolicy { get; set; }

		[Export ("jwksTrustedRootCertificates", ArgumentSemantic.Copy)]
		NSObject [] JwksTrustedRootCertificates { get; set; }

		[NullAllowed]
		[Export ("refreshEndpointURL", ArgumentSemantic.Copy)]
		NSUrl RefreshEndpointUrl { get; set; }

		[NullAllowed, Export ("uniqueIdentifierClaimName")]
		string UniqueIdentifierClaimName { get; set; }

		[NullAllowed, Export ("customRequestJWTParameterName")]
		string CustomRequestJwtParameterName { get; set; }

		[NullAllowed, Export ("groupRequestClaimName")]
		string GroupRequestClaimName { get; set; }

		[NullAllowed, Export ("groupResponseClaimName")]
		string GroupResponseClaimName { get; set; }

		[Internal]
		[Export ("loginRequestEncryptionPublicKey", ArgumentSemantic.Assign)]
		IntPtr /* SecKeyRef */ _LoginRequestEncryptionPublicKey { get; set; }

		SecKey LoginRequestEncryptionPublicKey {
			[Wrap ("new SecKey (this._LoginRequestEncryptionPublicKey, owns: false)")]
			get;
			[Wrap ("_LoginRequestEncryptionPublicKey = Runtime.RetainAndAutoreleaseNativeObject (value)")]
			set;
		}

		[NullAllowed]
		[Export ("keyEndpointURL", ArgumentSemantic.Copy)]
		NSUrl KeyEndpointUrl { get; set; }

		[Export ("customKeyExchangeRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomKeyExchangeRequestValues { get; set; }

		[NullAllowed]
		[Export ("loginRequestEncryptionAPVPrefix", ArgumentSemantic.Copy)]
		NSData LoginRequestEncryptionApvPrefix { get; set; }

		[Export ("customRefreshRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomRefreshRequestValues { get; set; }

		[Export ("setCustomRefreshRequestHeaderClaims:returningError:")]
		bool SetCustomRefreshRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomRefreshRequestBodyClaims:returningError:")]
		bool SetCustomRefreshRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("customKeyRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomKeyRequestValues { get; set; }

		[Export ("setCustomKeyRequestHeaderClaims:returningError:")]
		bool SetCustomKeyRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomKeyRequestBodyClaims:returningError:")]
		bool SetCustomKeyRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomKeyExchangeRequestHeaderClaims:returningError:")]
		bool SetCustomKeyExchangeRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomKeyExchangeRequestBodyClaims:returningError:")]
		bool SetCustomKeyExchangeRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[NullAllowed, Export ("additionalAuthorizationScopes")]
		string AdditionalAuthorizationScopes { get; set; }

		/// <summary>Gets or sets whether requests include the Platform SSO authorization scopes.</summary>
		[Mac (27, 0)]
		[Export ("includePlatformSSOAuthorizationScopes")]
		bool IncludePlatformSsoAuthorizationScopes { get; set; }

		[NullAllowed]
		[Export ("federationRequestURN")]
		string FederationRequestUrn { get; set; }

		[NullAllowed]
		[Export ("federationMEXURL", ArgumentSemantic.Copy)]
		NSUrl FederationMexUrl { get; set; }

		[NullAllowed]
		[Export ("federationUserPreauthenticationURL", ArgumentSemantic.Copy)]
		NSUrl FederationUserPreauthenticationUrl { get; set; }

		[Export ("federationType", ArgumentSemantic.Assign)]
		ASAuthorizationProviderExtensionFederationType FederationType { get; set; }

		[Mac (27, 0)]
		[Export ("fallbackFederationType", ArgumentSemantic.Assign)]
		ASAuthorizationProviderExtensionFederationType FallbackFederationType { get; set; }

		[Mac (27, 0)]
		[Export ("authorizationURL", ArgumentSemantic.Copy)]
		NSUrl AuthorizationUrl { get; set; }

		[Mac (27, 0)]
		[Export ("authorizationURLKeypath")]
		string AuthorizationUrlKeypath { get; set; }

		[NullAllowed]
		[Export ("federationPredicate")]
		string FederationPredicate { get; set; }

		[Export ("customFederationUserPreauthenticationRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomFederationUserPreauthenticationRequestValues { get; set; }

		[NullAllowed]
		[Export ("federationMEXURLKeypath")]
		string FederationMexUrlKeypath { get; set; }

		[Mac (15, 0)]
		[Export ("loginRequestEncryptionAlgorithm", ArgumentSemantic.Copy)]
		ASAuthorizationProviderExtensionEncryptionAlgorithm LoginRequestEncryptionAlgorithm { get; set; }

		[Mac (15, 0)]
		[Export ("loginRequestHPKEPreSharedKey", ArgumentSemantic.Copy), NullAllowed]
		NSData LoginRequestHpkePreSharedKey { get; set; }

		[Mac (15, 0)]
		[Export ("loginRequestHPKEPreSharedKeyID", ArgumentSemantic.Copy), NullAllowed]
		NSData LoginRequestHpkePreSharedKeyID { get; set; }

		[Mac (15, 0)]
		[Export ("hpkePreSharedKey", ArgumentSemantic.Copy), NullAllowed]
		NSData HpkePreSharedKey { get; set; }

		[Mac (15, 0)]
		[Export ("hpkePreSharedKeyID", ArgumentSemantic.Copy), NullAllowed]
		NSData HpkePreSharedKeyId { get; set; }

		[Mac (15, 0)]
		[Export ("hpkeAuthPublicKey"), NullAllowed]
		SecKey HpkeAuthPublicKey { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationProviderExtensionLoginManager {
		[Export ("deviceRegistered")]
		bool DeviceRegistered { [Bind ("isDeviceRegistered")] get; }

		[Export ("userRegistered")]
		bool UserRegistered { [Bind ("isUserRegistered")] get; }

		[NullAllowed, Export ("registrationToken")]
		string RegistrationToken { get; }

		[NullAllowed, Export ("loginUserName")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'UserLoginConfiguration.LoginUserName' instead.")]
		string LoginUserName { get; set; }

		[NullAllowed, Export ("ssoTokens", ArgumentSemantic.Copy)]
		NSDictionary SsoTokens { get; set; }

		[NullAllowed, Export ("loginConfiguration", ArgumentSemantic.Copy)]
		ASAuthorizationProviderExtensionLoginConfiguration LoginConfiguration { get; }

		[Export ("saveLoginConfiguration:error:")]
		bool Save (ASAuthorizationProviderExtensionLoginConfiguration loginConfiguration, [NullAllowed] out NSError error);

		[Protected]
		[Export ("saveCertificate:keyType:")]
		void _Save (IntPtr certificate, ASAuthorizationProviderExtensionKeyType keyType);

		[Protected]
		[Export ("copyKeyForKeyType:")]
		[return: NullAllowed]
		IntPtr _CopyKey (ASAuthorizationProviderExtensionKeyType keyType);

		[Wrap ("new SecKey (_CopyKey (keyType), true);")]
		SecKey CopyKey (ASAuthorizationProviderExtensionKeyType keyType);

		[Export ("copyIdentityForKeyType:")]
		[return: NullAllowed, Release]
		SecIdentity CopyIdentity (ASAuthorizationProviderExtensionKeyType keyType);

		[Async]
		[Export ("userNeedsReauthenticationWithCompletion:")]
		void UserNeedsReauthentication (Action<NSError> completion);

		[Export ("deviceRegistrationsNeedsRepair")]
		void DeviceRegistrationsNeedsRepair ();

		[Export ("userRegistrationsNeedsRepair")]
		void UserRegistrationsNeedsRepair ();

		[Export ("resetKeys")]
		void ResetKeys ();

		[Async]
		[Export ("presentRegistrationViewControllerWithCompletion:")]
		void PresentRegistrationViewController (Action<NSError> completion);

		[Export ("resetUserSecureEnclaveKey")]
		void ResetUserSecureEnclaveKey ();

		[Export ("resetDeviceKeys")]
		void ResetDeviceKeys ();

		[NullAllowed]
		[Export ("userLoginConfiguration", ArgumentSemantic.Copy)]
		ASAuthorizationProviderExtensionUserLoginConfiguration UserLoginConfiguration { get; }

		[Export ("decryptionKeysNeedRepair")]
		void DecryptionKeysNeedRepair ();

		[Export ("saveUserLoginConfiguration:error:")]
		bool SaveUserLoginConfiguration (ASAuthorizationProviderExtensionUserLoginConfiguration userLoginConfiguration, [NullAllowed] out NSError error);

		[Export ("extensionData")]
		NSDictionary ExtensionData { get; }

		[Mac (15, 0)]
		[return: NullAllowed]
		[Export ("beginKeyRotationForKeyType:")]
		SecKey BeginKeyRotation (ASAuthorizationProviderExtensionKeyType keyType); // FIXME: CF_RETURNS_RETAINED;

		[Mac (15, 0)]
		[Export ("completeKeyRotationForKeyType:")]
		void CompleteKeyRotation (ASAuthorizationProviderExtensionKeyType keyType);

		[Mac (15, 4)]
		[Export ("attestKey:clientDataHash:completion:")]
		[Async]
		void AttestKey (ASAuthorizationProviderExtensionKeyType keyType, NSData clientDataHash, ASAuthorizationProviderExtensionLoginManagerAttestCallback completion);

		[Mac (15, 4)]
		[Export ("attestPendingKey:clientDataHash:completion:")]
		[Async]
		void AttestPendingKey (ASAuthorizationProviderExtensionKeyType keyType, NSData clientDataHash, ASAuthorizationProviderExtensionLoginManagerAttestCallback completion);

		[Mac (26, 0)]
		[Export ("authenticationMethod")]
		ASAuthorizationProviderExtensionAuthenticationMethod AuthenticationMethod { get; }
	}

	delegate void ASAuthorizationProviderExtensionLoginManagerAttestCallback ([NullAllowed] NSArray attestationCertificates, [NullAllowed] NSError error);

	[NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationProviderExtensionRegistrationHandler {

		[Abstract]
		[Export ("beginDeviceRegistrationUsingLoginManager:options:completion:")]
		void BeginDeviceRegistration (ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRequestOptions options, Action<ASAuthorizationProviderExtensionRegistrationResult> handler);

		[Abstract]
		[Export ("beginUserRegistrationUsingLoginManager:userName:authenticationMethod:options:completion:")]
		void BeginUserRegistration (ASAuthorizationProviderExtensionLoginManager loginManager, [NullAllowed] string userName, ASAuthorizationProviderExtensionAuthenticationMethod authenticationMethod, ASAuthorizationProviderExtensionRequestOptions options, Action<ASAuthorizationProviderExtensionRegistrationResult> handler);

		[Export ("registrationDidComplete")]
		void RegistrationDidComplete ();

		[Export ("supportedGrantTypes")]
		ASAuthorizationProviderExtensionSupportedGrantTypes SupportedGrantTypes { get; }

		[Export ("registrationDidCancel")]
		void RegistrationDidCancel ();

		[Export ("protocolVersion")]
		ASAuthorizationProviderExtensionPlatformSSOProtocolVersion ProtocolVersion { get; }

		[Mac (15, 0)]
		[Export ("supportedDeviceSigningAlgorithms")]
		NSNumber [] WeakSupportedDeviceSigningAlgorithms { get; }

		[Mac (15, 0)]
		ASAuthorizationProviderExtensionSigningAlgorithm [] SupportedDeviceSigningAlgorithms {
			[Wrap ("ASAuthorizationProviderExtensionSigningAlgorithmExtensions.ToEnumArray (WeakSupportedDeviceSigningAlgorithms)!")]
			get;
		}

		[Mac (15, 0)]
		[Export ("supportedDeviceEncryptionAlgorithms")]
		NSNumber [] WeakSupportedDeviceEncryptionAlgorithms {
			get;
		}

		[Mac (15, 0)]
		ASAuthorizationProviderExtensionEncryptionAlgorithm [] SupportedDeviceEncryptionAlgorithms {
			[Wrap ("ASAuthorizationProviderExtensionEncryptionAlgorithmExtensions.ToEnumArray (WeakSupportedDeviceEncryptionAlgorithms)!")]
			get;
		}

		[Mac (15, 0)]
		[Export ("supportedUserSecureEnclaveKeySigningAlgorithms")]
		NSNumber [] WeakSupportedUserSecureEnclaveKeySigningAlgorithms { get; }

		[Mac (15, 0)]
		ASAuthorizationProviderExtensionSigningAlgorithm [] SupportedUserSecureEnclaveKeySigningAlgorithms {
			[Wrap ("ASAuthorizationProviderExtensionSigningAlgorithmExtensions.ToEnumArray (WeakSupportedUserSecureEnclaveKeySigningAlgorithms)!")]
			get;
		}

		[Mac (15, 0)]
		[Export ("keyWillRotateForKeyType:newKey:loginManager:completion:")]
		void KeyWillRotateForKeyType (ASAuthorizationProviderExtensionKeyType keyType, SecKey newKey, ASAuthorizationProviderExtensionLoginManager loginManager, Action<bool> completion);

		[Mac (26, 0)]
		[Export ("displayNamesForGroups:loginManager:completion:")]
		void GetDisplayNames (string [] groups, ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRegistrationHandlerGetDisplayNamesCallback completion);

		[Mac (26, 0)]
		[Export ("profilePictureForUserUsingLoginManager:completion:")]
		void GetProfilePictureForUser (ASAuthorizationProviderExtensionLoginManager loginManager, ASAuthorizationProviderExtensionRegistrationHandlerGetProfilePictureForUserCallback completion);
	}

	delegate void ASAuthorizationProviderExtensionRegistrationHandlerGetDisplayNamesCallback (NSDictionary<NSString, NSString> fullNames);
	delegate void ASAuthorizationProviderExtensionRegistrationHandlerGetProfilePictureForUserCallback (NSData jpegData);

	interface IASAuthorizationWebBrowserExternallyAuthenticatableRequest { }

	[NoTV, NoiOS, NoMacCatalyst]
	[Protocol]
	interface ASAuthorizationWebBrowserExternallyAuthenticatableRequest {

		[Abstract]
		[NullAllowed, Export ("authenticatedContext", ArgumentSemantic.Assign)]
		LAContext AuthenticatedContext { get; set; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (16, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredential {

		[Export ("name")]
		string Name { get; }

		[Export ("relyingParty")]
		string RelyingParty { get; }

		[Export ("credentialID")]
		NSData CredentialId { get; }

		[Export ("userHandle")]
		NSData UserHandle { get; }

		[MacCatalyst (17, 0)]
		[Export ("providerName")]
		string ProviderName { get; }

		[MacCatalyst (17, 0)]
		[NullAllowed, Export ("customTitle")]
		string CustomTitle { get; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (16, 4)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface ASAuthorizationWebBrowserPublicKeyCredentialManager {

		[Mac (26, 1), iOS (26, 1), MacCatalyst (26, 1)]
		[Static]
		[Export ("isDeviceConfiguredForPasskeys")]
		bool IsDeviceConfiguredForPasskeys { get; }

		[Async]
		[Export ("requestAuthorizationForPublicKeyCredentials:")]
		void RequestAuthorization (Action<ASAuthorizationWebBrowserPublicKeyCredentialManagerAuthorizationState> completionHandler);

		[Async]
		[Export ("platformCredentialsForRelyingParty:completionHandler:")]
		void GetPlatformCredentials (string relyingParty, Action<ASAuthorizationWebBrowserPlatformPublicKeyCredential []> completionHandler);

		[Export ("authorizationStateForPlatformCredentials")]
		ASAuthorizationWebBrowserPublicKeyCredentialManagerAuthorizationState AuthorizationStateForPlatformCredentials { get; }
	}

	delegate void ASSettingsHelperRequestToTurnOnCredentialProviderExtensionCallback (bool appWasEnabledForAutofill);

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASSettingsHelper {
		[Async]
		[Static]
		[Export ("openCredentialProviderAppSettingsWithCompletionHandler:")]
		void OpenCredentialProviderAppSettings ([NullAllowed] Action<NSError> completionHandler);

		[Async]
		[Static]
		[Export ("openVerificationCodeAppSettingsWithCompletionHandler:")]
		void OpenVerificationCodeAppSettings ([NullAllowed] Action<NSError> completionHandler);

		[iOS (18, 0), Mac (15, 0), MacCatalyst (18, 0), NoTV]
		[Async]
		[Static]
		[Export ("requestToTurnOnCredentialProviderExtensionWithCompletionHandler:")]
		void RequestToTurnOnCredentialProviderExtension (ASSettingsHelperRequestToTurnOnCredentialProviderExtensionCallback completionHandler);
	}

	interface IASCredentialRequest { }

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface ASCredentialRequest : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("type")]
		ASCredentialRequestType Type { get; }

		[Abstract]
		[Export ("credentialIdentity")]
		IASCredentialIdentity CredentialIdentity { get; }
	}

	[NoTV, MacCatalyst (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredentialRequest : ASCredentialRequest {
		[Export ("initWithCredentialIdentity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASPasswordCredentialIdentity credentialIdentity);

		[Static]
		[Export ("requestWithCredentialIdentity:")]
		ASPasswordCredentialRequest Request (ASPasswordCredentialIdentity credentialIdentity);
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface ASPasskeyRegistrationCredential : ASAuthorizationCredential {
		[Export ("initWithRelyingParty:clientDataHash:credentialID:attestationObject:")]
		NativeHandle Constructor (string relyingParty, NSData clientDataHash, NSData credentialId, NSData attestationObject);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithRelyingParty:clientDataHash:credentialID:attestationObject:extensionOutput:")]
		NativeHandle Constructor (string relyingParty, NSData clientDataHash, NSData credentialId, NSData attestationObject, [NullAllowed] ASPasskeyRegistrationCredentialExtensionOutput extensionOutput);

		[Static]
		[Export ("credentialWithRelyingParty:clientDataHash:credentialID:attestationObject:")]
		ASPasskeyRegistrationCredential CreateCredential (string relyingParty, NSData clientDataHash, NSData credentialId, NSData attestationObject);

		[Export ("relyingParty")]
		string RelyingParty { get; }

		[Export ("clientDataHash")]
		NSData ClientDataHash { get; }

		[Export ("credentialID")]
		NSData CredentialId { get; }

		[Export ("attestationObject")]
		NSData AttestationObject { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("extensionOutput", ArgumentSemantic.Copy), NullAllowed]
		ASPasskeyRegistrationCredentialExtensionOutput ExtensionOutput { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyCredentialRequest : ASCredentialRequest {
		[Export ("initWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:")]
		NativeHandle Constructor (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, string userVerificationPreference, NSNumber [] supportedAlgorithms);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:assertionExtensionInput:")]
		NativeHandle Constructor (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, [BindAs (typeof (ASAuthorizationPublicKeyCredentialUserVerificationPreferenceEnum))] NSString userVerificationPreference, NSNumber [] supportedAlgorithms, [NullAllowed] ASPasskeyAssertionCredentialExtensionInput assertionExtensionInput);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:registrationExtensionInput:")]
		NativeHandle Constructor (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, [BindAs (typeof (ASAuthorizationPublicKeyCredentialUserVerificationPreferenceEnum))] NSString userVerificationPreference, NSNumber [] supportedAlgorithms, [NullAllowed] ASPasskeyRegistrationCredentialExtensionInput registrationExtensionInput);

		[Export ("clientDataHash")]
		NSData ClientDataHash { get; }

		[Export ("userVerificationPreference")]
		string UserVerificationPreference { get; set; }

		[Export ("supportedAlgorithms")]
		NSNumber [] SupportedAlgorithms { get; }

		[Static]
		[Export ("requestWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:")]
		ASPasskeyCredentialRequest Create (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, string userVerificationPreference, NSNumber [] supportedAlgorithms);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("excludedCredentials"), NullAllowed]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] ExcludedCredentials { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("assertionExtensionInput"), NullAllowed]
		ASPasskeyAssertionCredentialExtensionInput AssertionExtensionInput { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("registrationExtensionInput"), NullAllowed]
		ASPasskeyRegistrationCredentialExtensionInput RegistrationExtensionInput { get; }
	}

	interface IASCredentialIdentity { }

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface ASCredentialIdentity {

		[Abstract]
		[Export ("serviceIdentifier", ArgumentSemantic.Strong)]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[Abstract]
		[Export ("user")]
		string User { get; }

		[Abstract]
		[NullAllowed, Export ("recordIdentifier")]
		string RecordIdentifier { get; }

		[Abstract]
		[Export ("rank")]
		nint Rank { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyCredentialIdentity : NSCopying, NSSecureCoding, ASCredentialIdentity {
		[Export ("initWithRelyingPartyIdentifier:userName:credentialID:userHandle:recordIdentifier:")]
		NativeHandle Constructor (string relyingPartyIdentifier, string userName, NSData credentialId, NSData userHandle, [NullAllowed] string recordIdentifier);

		[Static]
		[Export ("identityWithRelyingPartyIdentifier:userName:credentialID:userHandle:recordIdentifier:")]
		ASPasskeyCredentialIdentity CreateIdentity (string relyingPartyIdentifier, string userName, NSData credentialId, NSData userHandle, [NullAllowed] string recordIdentifier);

		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }

		[Export ("userName")]
		string UserName { get; }

		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; }

		[Export ("userHandle", ArgumentSemantic.Copy)]
		NSData UserHandle { get; }

		[NullAllowed, Export ("recordIdentifier")]
		new string RecordIdentifier { get; }

		[Export ("rank")]
		new nint Rank { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ASPasskeyAssertionCredential : ASAuthorizationCredential {
		[Export ("initWithUserHandle:relyingParty:signature:clientDataHash:authenticatorData:credentialID:")]
		NativeHandle Constructor (NSData userHandle, string relyingParty, NSData signature, NSData clientDataHash, NSData authenticatorData, NSData credentialId);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("initWithUserHandle:relyingParty:signature:clientDataHash:authenticatorData:credentialID:extensionOutput:")]
		NativeHandle Constructor (NSData userHandle, string relyingParty, NSData signature, NSData clientDataHash, NSData authenticatorData, NSData credentialId, [NullAllowed] ASPasskeyAssertionCredentialExtensionOutput extensionOutput);

		[Static]
		[Export ("credentialWithUserHandle:relyingParty:signature:clientDataHash:authenticatorData:credentialID:")]
		ASPasskeyAssertionCredential CreateCredential (NSData userHandle, string relyingParty, NSData signature, NSData clientDataHash, NSData authenticatorData, NSData credentialId);

		[Export ("userHandle", ArgumentSemantic.Copy)]
		NSData UserHandle { get; }

		[Export ("relyingParty")]
		string RelyingParty { get; }

		[Export ("signature", ArgumentSemantic.Copy)]
		NSData Signature { get; }

		[Export ("clientDataHash", ArgumentSemantic.Copy)]
		NSData ClientDataHash { get; }

		[Export ("authenticatorData", ArgumentSemantic.Copy)]
		NSData AuthenticatorData { get; }

		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("extensionOutput", ArgumentSemantic.Copy), NullAllowed]
		ASPasskeyAssertionCredentialExtensionOutput ExtensionOutput { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyCredentialRequestParameters : NSSecureCoding, NSCopying {
		[Export ("relyingPartyIdentifier")]
		string RelyingPartyIdentifier { get; }

		[Export ("clientDataHash", ArgumentSemantic.Copy)]
		NSData ClientDataHash { get; }

		[Export ("userVerificationPreference")]
		string UserVerificationPreference { get; }

		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		NSData [] AllowedCredentials { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("extensionInput"), NullAllowed]
		ASPasskeyAssertionCredentialExtensionInput ExtensionInput { get; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput {
		[Export ("initWithSupportRequirement:")]
		NativeHandle Constructor (ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement requirement);

		[Export ("supportRequirement", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement SupportRequirement { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobAssertionInput {
		[Export ("initWithOperation:")]
		NativeHandle Constructor (ASAuthorizationPublicKeyCredentialLargeBlobAssertionOperation operation);

		[Export ("operation")]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionOperation Operation { get; }

		[NullAllowed, Export ("dataToWrite", ArgumentSemantic.Assign)]
		NSData DataToWrite { get; set; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput {
		[NullAllowed, Export ("readData")]
		NSData ReadData { get; }

		[Export ("didWrite")]
		bool DidWrite { get; }
	}

	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationOutput : NSCopying, NSSecureCoding {
		[Export ("isSupported")]
		bool IsSupported { get; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPublicKeyCredentialClientData {
		[Export ("initWithChallenge:origin:")]
		NativeHandle Constructor (NSData challenge, string origin);

		[Export ("challenge", ArgumentSemantic.Assign)]
		NSData Challenge { get; set; }

		[Export ("origin")]
		string Origin { get; set; }

		[NullAllowed, Export ("topOrigin")]
		string TopOrigin { get; set; }

		[Export ("crossOrigin", ArgumentSemantic.Assign)]
		ASPublicKeyCredentialClientDataCrossOriginValue CrossOrigin { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationProviderExtensionUserLoginConfiguration {
		[Export ("loginUserName")]
		string LoginUserName { get; set; }

		[Export ("initWithLoginUserName:")]
		NativeHandle Constructor (string loginUserName);

		[Export ("setCustomAssertionRequestHeaderClaims:returningError:")]
		bool SetCustomAssertionRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomAssertionRequestBodyClaims:returningError:")]
		bool SetCustomAssertionRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomLoginRequestHeaderClaims:returningError:")]
		bool SetCustomLoginRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Export ("setCustomLoginRequestBodyClaims:returningError:")]
		bool SetCustomLoginRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialProvider {
		[Abstract]
		[Export ("createCredentialRegistrationRequestWithClientData:name:userID:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string name, NSData userId);

		[Abstract]
		[Export ("createCredentialAssertionRequestWithClientData:")]
		ASAuthorizationPlatformPublicKeyCredentialAssertionRequest CreateCredentialAssertionRequest (ASPublicKeyCredentialClientData clientData);

		[Abstract]
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("createCredentialRegistrationRequestWithClientData:name:userID:requestStyle:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string name, NSData userId, ASAuthorizationPlatformPublicKeyCredentialRegistrationRequestStyle requestStyle);
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialRegistrationRequest {
		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }

		[Abstract]
		[NullAllowed, Export ("excludedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] ExcludedCredentials { get; set; }

#if XAMCORE_5_0
		[NoMacCatalyst]
#else
		[MacCatalyst (17, 0)] // not true, it's not available on Mac Catalyst, but we've released it this way so we need to keep it.
#endif
		[NoiOS]
		[Abstract]
		[Export ("shouldShowHybridTransport")]
		bool ShouldShowHybridTransport { get; set; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 4), Mac (14, 4)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialAssertionRequest {
		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }

		[Abstract]
		[Export ("shouldShowHybridTransport")]
		bool ShouldShowHybridTransport { get; set; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 4), Mac (14, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface ASAuthorizationWebBrowserSecurityKeyPublicKeyCredentialAssertionRequest {

		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 4), Mac (14, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface ASAuthorizationWebBrowserSecurityKeyPublicKeyCredentialProvider {

		[Abstract]
		[Export ("createCredentialRegistrationRequestWithClientData:displayName:name:userID:")]
		ASAuthorizationSecurityKeyPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string displayName, string name, NSData userId);

		[Abstract]
		[Export ("createCredentialAssertionRequestWithClientData:")]
		ASAuthorizationSecurityKeyPublicKeyCredentialAssertionRequest CreateCredentialAssertionRequest (ASPublicKeyCredentialClientData clientData);
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 4), Mac (14, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface ASAuthorizationWebBrowserSecurityKeyPublicKeyCredentialRegistrationRequest {

		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }
	}

	[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSessionCallback {

		[Static]
		[Export ("callbackWithCustomScheme:")]
		ASWebAuthenticationSessionCallback Create (string customScheme);

		[Static]
		[Export ("callbackWithHTTPSHost:path:")]
		ASWebAuthenticationSessionCallback Create (string httpsHost, string path);

		[Export ("matchesURL:")]
		bool MatchesUrl (NSUrl url);
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum ASAuthorizationPlatformPublicKeyCredentialRegistrationRequestStyle : long {
		Standard,
		Conditional,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationPublicKeyCredentialPRFAssertionInputValues")]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialPrfAssertionInputValues {
		[Export ("initWithSaltInput1:saltInput2:")]
		NativeHandle Constructor (NSData saltInput1, [NullAllowed] NSData saltInput2);

		[Export ("saltInput1")]
		NSData SaltInput1 { get; }

		[Export ("saltInput2"), NullAllowed]
		NSData SaltInput2 { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationPublicKeyCredentialPRFAssertionInput")]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialPrfAssertionInput {
		[Export ("initWithInputValues:perCredentialInputValues:")]
		NativeHandle Constructor ([NullAllowed] ASAuthorizationPublicKeyCredentialPrfAssertionInputValues inputValues, [NullAllowed] NSDictionary<NSData, ASAuthorizationPublicKeyCredentialPrfAssertionInputValues> perCredentialInputValues);

		[Export ("inputValues"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionInputValues InputValues { get; }

		[Export ("perCredentialInputValues"), NullAllowed]
		NSDictionary<NSData, ASAuthorizationPublicKeyCredentialPrfAssertionInputValues> PerCredentialInputValues { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationPublicKeyCredentialPRFAssertionOutput")]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialPrfAssertionOutput {
		[Export ("first")]
		NSData First { get; }

		[Export ("second"), NullAllowed]
		NSData Second { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationPublicKeyCredentialPRFRegistrationInput")]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialPrfRegistrationInput {
		[Static]
		[Export ("checkForSupport")]
		ASAuthorizationPublicKeyCredentialPrfRegistrationInput GetCheckForSupport ();

		[Export ("initWithInputValues:")]
		NativeHandle Constructor ([NullAllowed] ASAuthorizationPublicKeyCredentialPrfAssertionInputValues inputValues);

		[Export ("shouldCheckForSupport")]
		bool ShouldCheckForSupport { get; }

		[Export ("inputValues"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfAssertionInputValues InputValues { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationPublicKeyCredentialPRFRegistrationOutput")]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialPrfRegistrationOutput {
		[Export ("isSupported")]
		bool IsSupported { get; }

		[Export ("first"), NullAllowed]
		NSData First { get; }

		[Export ("second"), NullAllowed]
		NSData Second { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASOneTimeCodeCredential : ASAuthorizationCredential {
		[Static]
		[Export ("credentialWithCode:")]
		ASOneTimeCodeCredential Create (string code);

		[DesignatedInitializer]
		[Export ("initWithCode:")]
		NativeHandle Constructor (string code);

		[Export ("code", ArgumentSemantic.Copy)]
		string Code { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASOneTimeCodeCredentialIdentity : NSCopying, NSSecureCoding, ASCredentialIdentity {
		[Export ("initWithServiceIdentifier:label:recordIdentifier:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, string label, [NullAllowed] string recordIdentifier);

		[Export ("label", ArgumentSemantic.Copy)]
		string Label { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASOneTimeCodeCredentialRequest : ASCredentialRequest {
		[Export ("initWithCredentialIdentity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASOneTimeCodeCredentialIdentity credentialIdentity);
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyAssertionCredentialExtensionInput : NSCopying, NSSecureCoding {
		[Export ("largeBlob"), NullAllowed]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionInput LargeBlob { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyAssertionCredentialExtensionOutput : NSCopying, NSSecureCoding {
		[Export ("initWithLargeBlobOutput:")]
		NativeHandle Constructor ([NullAllowed] ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput largeBlob);

		[Export ("largeBlobAssertionOutput", ArgumentSemantic.Copy), NullAllowed]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput LargeBlobAssertionOutput { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyRegistrationCredentialExtensionInput : NSCopying, NSSecureCoding {
		[Export ("largeBlob"), NullAllowed]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyRegistrationCredentialExtensionOutput : NSCopying, NSSecureCoding {
		[Export ("initWithLargeBlobOutput:")]
		NativeHandle Constructor ([NullAllowed] ASAuthorizationPublicKeyCredentialLargeBlobRegistrationOutput largeBlob);

		[Export ("largeBlobRegistrationOutput", ArgumentSemantic.Copy), NullAllowed]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationOutput LargeBlobRegistrationOutput { get; }
	}

	[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
	[BackingFieldType (typeof (NSNumber))]
	public enum ASAuthorizationProviderExtensionEncryptionAlgorithm {
		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmECDHE_A256GCM")]
		EcdheA256Gcm,

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P256_SHA256_AES_GCM_256")]
		HpkeP256Sha256AesGcm256,

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P384_SHA384_AES_GCM_256")]
		HpkeP384Sha384AesGcm256,

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_Curve25519_SHA256_ChachaPoly")]
		HpkeCurve25519Sha256ChachaPoly,
	}

	[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
	[BackingFieldType (typeof (NSNumber))]
	public enum ASAuthorizationProviderExtensionSigningAlgorithm {
		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmES256")]
		ES256,

		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmES384")]
		ES384,

		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmEd25519")]
		Ed25519,
	}

	[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
	[Static]
	interface ASAuthorizationProviderExtensionEncryptionAlgorithm222 {
		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmECDHE_A256GCM")]
		NSNumber EcdheA256Gcm { get; }

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P256_SHA256_AES_GCM_256")]
		NSNumber HpkeP256Sha256AesGcm256 { get; }

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P384_SHA384_AES_GCM_256")]
		NSNumber HpkeP384Sha384AesGcm256 { get; }

		[Field ("ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_Curve25519_SHA256_ChachaPoly")]
		NSNumber HpkeCurve25519Sha256ChachaPoly { get; }
	}

	[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
	[Static]
	interface ASAuthorizationProviderExtensionSigningAlgorithm222 {
		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmES256")]
		NSNumber ES256 { get; }

		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmES384")]
		NSNumber ES384 { get; }

		[Field ("ASAuthorizationProviderExtensionSigningAlgorithmEd25519")]
		NSNumber Ed25519 { get; }
	}

	[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
	[Native]
	public enum ASSavePasswordRequestEvent : long {
		UserInitiated,
		FormDidDisappear,
		GeneratedPasswordFilled
	}


	[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
	enum ASGeneratedPasswordKind {
		[Field ("ASGeneratedPasswordKindAlphanumeric")]
		Alphanumeric,
		[Field ("ASGeneratedPasswordKindPassphrase")]
		Passphrase,
		[Field ("ASGeneratedPasswordKindStrong")]
		Strong,
	}

	[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASGeneratedPassword : NSCopying, NSSecureCoding {

		[BindAs (typeof (ASGeneratedPasswordKind))]
		[Export ("kind")]
		NSString Kind { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("initWithKind:value:")]
		NativeHandle Constructor ([BindAs (typeof (ASGeneratedPasswordKind))] NSString kind, string value);
	}

	[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASGeneratePasswordsRequest : NSCopying, NSSecureCoding {

		[Export ("serviceIdentifier")]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[NullAllowed, Export ("passwordFieldPasswordRules")]
		string PasswordFieldPasswordRules { get; }

		[NullAllowed, Export ("confirmPasswordFieldPasswordRules")]
		string ConfirmPasswordFieldPasswordRules { get; }

		[NullAllowed, Export ("passwordRulesFromQuirks")]
		string PasswordRulesFromQuirks { get; }

		[Export ("initWithServiceIdentifier:passwordFieldPasswordRules:confirmPasswordFieldPasswordRules:passwordRulesFromQuirks:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, [NullAllowed] string passwordFieldPasswordRules, [NullAllowed] string confirmPasswordFieldPasswordRules, [NullAllowed] string passwordRulesFromQuirks);
	}

	[NoTV, NoMac, iOS (26, 2), MacCatalyst (26, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASSavePasswordRequest : NSCopying, NSSecureCoding {

		[Export ("serviceIdentifier")]
		ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[Export ("credential")]
		ASPasswordCredential Credential { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("sessionID")]
		string SessionId { get; }

		[Export ("event")]
		ASSavePasswordRequestEvent Event { get; }

		[BindAs (typeof (ASGeneratedPasswordKind))]
		[NullAllowed, Export ("passwordKind")]
		NSString PasswordKind { get; }

		[Export ("initWithServiceIdentifier:credential:sessionID:event:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential credential, string sessionId, ASSavePasswordRequestEvent @event);

		[Export ("initWithServiceIdentifier:credential:title:sessionID:event:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential credential, [NullAllowed] string title, string sessionId, ASSavePasswordRequestEvent @event);

		[Export ("initWithServiceIdentifier:credential:sessionID:event:passwordKind:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential credential, string sessionId, ASSavePasswordRequestEvent @event, [NullAllowed][BindAs (typeof (ASGeneratedPasswordKind))] NSString passwordKind);

		[Export ("initWithServiceIdentifier:credential:title:sessionID:event:passwordKind:")]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, ASPasswordCredential credential, [NullAllowed] string title, string sessionId, ASSavePasswordRequestEvent @event, [NullAllowed][BindAs (typeof (ASGeneratedPasswordKind))] NSString passwordKind);
	}
}
