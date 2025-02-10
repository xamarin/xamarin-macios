//
// AuthenticationServices bindings
//
// Copyright 2018-2019 Microsoft Corporation
//

using System;
using Foundation;
using ObjCRuntime;
using Security;
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

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AuthenticationServices {

	/// <summary>Enumerates error codes associated with authentication service store requests.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	[ErrorDomain ("ASCredentialIdentityStoreErrorDomain")]
	public enum ASCredentialIdentityStoreErrorCode : long {
		InternalError = 0,
		StoreDisabled = 1,
		StoreBusy = 2,
	}

	/// <summary>Error codes associated with Authentication Services extensions.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[Native]
	[ErrorDomain ("ASExtensionErrorDomain")]
	public enum ASExtensionErrorCode : long {
		Failed = 0,
		UserCanceled = 1,
		UserInteractionRequired = 100,
		CredentialIdentityNotFound = 101,
		MatchedExcludedCredential = 102,
	}

	[Partial]
	interface ASExtensionErrorCodeExtensions {

#if NET || TVOS
		// Type `ASExtensionErrorCode` is already decorated, so it becomes a duplicate (after code gen)
		// on those platforms and intro tests complains (on other platforms, e.g. iOS, Catalyst)
		// OTOH if we don't add them here then we'll get the extra, not really usable, extension type
		// on tvOS and watchOS (which is incorrect)
		[NoTV]
#endif
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
		Domain,
		Url,
	}

	[TV (16, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("ASWebAuthenticationSessionErrorDomain")]
	public enum ASWebAuthenticationSessionErrorCode : long {
		CanceledLogin = 1,
		PresentationContextNotProvided = 2,
		PresentationContextInvalid = 3,
	}

	[Flags, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum ASAuthorizationControllerRequestOptions : ulong {
		ImmediatelyAvailableCredentials = 1uL << 0,
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Native]
	public enum ASAuthorizationProviderExtensionAuthenticationMethod : long {
		Password = 1,
		UserSecureEnclaveKey = 2,
		[Mac (14, 0)]
		SmartCard = 3,
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Native]
	public enum ASAuthorizationProviderExtensionKeyType : long {
		DeviceSigning = 1,
		DeviceEncryption = 2,
		SecureEnclaveKey = 3,
		[Mac (14, 0)]
		SharedDeviceSigning = 4,
		[Mac (14, 0)]
		SharedDeviceEncryption = 5,
		[Mac (14, 0)]
		CurrentDeviceSigning = 10,
		[Mac (14, 0)]
		CurrentDeviceEncryption = 11,
		[Mac (14, 0)]
		UserSmartCard = 20,
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Native]
	public enum ASAuthorizationProviderExtensionRegistrationResult : long {
		Success = 0,
		Failed = 1,
		UserInterfaceRequired = 2,
		FailedNoRetry = 3,
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Flags]
	[Native]
	public enum ASAuthorizationProviderExtensionRequestOptions : ulong {
		None = 0x0,
		UserInteractionEnabled = 1uL << 0,
		RegistrationRepair = 1uL << 1,
		[Mac (14, 0)]
		RegistrationSharedDeviceKeys = 1uL << 2,
		[Mac (14, 0)]
		RegistrationDeviceKeyMigration = 1uL << 3,
		[Mac (15, 0)]
		StrongerKeyAvailable = 1uL << 4,
		[Mac (14, 4)]
		UserKeyInvalid = 1uL << 5,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (16, 4), Mac (13, 3)]
	[Native]
	public enum ASAuthorizationWebBrowserPublicKeyCredentialManagerAuthorizationState : long {
		Authorized,
		Denied,
		NotDetermined,
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (14, 0)]
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

	/// <summary>Delegate used in callbacks by <see cref="T:AuthenticationServices.ASCredentialIdentityStore" />.</summary>
	delegate void ASCredentialIdentityStoreCompletionHandler (bool success, NSError error);
	delegate void ASCredentialIdentityStoreGetCredentialIdentitiesHandler (IASCredentialIdentity [] credentialIdentities);

	/// <summary>A class whose shared instance (see <see cref="P:AuthenticationServices.ASCredentialIdentityStore.SharedStore" />) holds credentials across providers.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
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
		[Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("getCredentialIdentitiesForService:credentialIdentityTypes:completionHandler:")]
		void GetCredentialIdentities ([NullAllowed] ASCredentialServiceIdentifier serviceIdentifier, [NullAllowed] ASCredentialIdentityTypes credentialIdentityTypes, ASCredentialIdentityStoreGetCredentialIdentitiesHandler completion);

		[Async]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'SaveCredentialIdentityEntries  (ASCredentialIdentity [])' instead.")]
		[Export ("saveCredentialIdentities:completion:")]
		void SaveCredentialIdentities (ASPasswordCredentialIdentity [] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'RemoveCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Async]
		[Export ("removeCredentialIdentities:completion:")]
		void RemoveCredentialIdentities (ASPasswordCredentialIdentity [] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Async]
		[Export ("removeAllCredentialIdentitiesWithCompletion:")]
		void RemoveAllCredentialIdentities ([NullAllowed] Action<bool, NSError> completion);

		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
		[Async]
		[Export ("replaceCredentialIdentitiesWithIdentities:completion:")]
		void ReplaceCredentialIdentities (ASPasswordCredentialIdentity [] newCredentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Async]
		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("saveCredentialIdentityEntries:completion:")]
		void SaveCredentialIdentityEntries (IASCredentialIdentity [] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("replaceCredentialIdentityEntries:completion:")]
		void ReplaceCredentialIdentityEntries (IASCredentialIdentity [] newCredentialIdentities, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("removeCredentialIdentityEntries:completion:")]
		void RemoveCredentialIdentityEntries (IASCredentialIdentity [] credentialIdentities, [NullAllowed] Action<bool, NSError> completion);
	}

	/// <summary>Data related to the availability and capability of the credential identity store.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStoreState {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("supportsIncrementalUpdates")]
		bool SupportsIncrementalUpdates { get; }
	}

	/// <summary>Delegate object for completion handlers in methods within <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Authentication%20Services%20ASCredential%20Provider%20Extension&amp;scope=Xamarin" title="T:AuthenticationServices.ASCredentialProviderExtension">T:AuthenticationServices.ASCredentialProviderExtension</a></format>.</summary>
	delegate void ASCredentialProviderExtensionRequestCompletionHandler (bool expired);

	/// <summary>An <see cref="NSExtensionContext" /> subclass that provides context for a credential provider.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSExtensionContext))]
	[DisableDefaultCtor]
	interface ASCredentialProviderExtensionContext {
		[Export ("completeRequestWithSelectedCredential:completionHandler:")]
		void CompleteRequest (ASPasswordCredential credential, [NullAllowed] ASCredentialProviderExtensionRequestCompletionHandler completionHandler);

		[Export ("completeExtensionConfigurationRequest")]
		void CompleteExtensionConfigurationRequest ();

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);

		[Async]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("completeRegistrationRequestWithSelectedPasskeyCredential:completionHandler:")]
		void CompleteRegistrationRequest (ASPasskeyRegistrationCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	/// <summary>Holds the identification for a credential service.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialServiceIdentifier : NSCopying, NSSecureCoding {
		[Export ("initWithIdentifier:type:")]
		NativeHandle Constructor (string identifier, ASCredentialServiceIdentifierType type);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("type")]
		ASCredentialServiceIdentifierType Type { get; }
	}

	/// <summary>Associates a <see cref="P:AuthenticationServices.ASPasswordCredentialIdentity.User" /> string with a record in the developer's credential database.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredentialIdentity : NSCopying, NSSecureCoding, ASCredentialIdentity {
		[Export ("initWithServiceIdentifier:user:recordIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		[Static]
		[Export ("identityWithServiceIdentifier:user:recordIdentifier:")]
		ASPasswordCredentialIdentity Create (ASCredentialServiceIdentifier serviceIdentifier, string user, [NullAllowed] string recordIdentifier);

		[Export ("serviceIdentifier", ArgumentSemantic.Strong)]
		new ASCredentialServiceIdentifier ServiceIdentifier { get; }

		[Export ("user")]
		new string User { get; }

		[NullAllowed, Export ("recordIdentifier")]
		new string RecordIdentifier { get; }

		[Export ("rank")]
		new nint Rank { get; set; }
	}

	/// <summary>System-provided standard <see cref="T:UIKit.UIViewController" /> for presenting a credential provider extension.</summary>
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[BaseType (typeof (UIViewController))]
	interface ASCredentialProviderViewController {
		[Export ("extensionContext", ArgumentSemantic.Strong)]
		ASCredentialProviderExtensionContext ExtensionContext { get; }

		[Export ("prepareCredentialListForServiceIdentifiers:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers);

		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'ProvideCredentialWithoutUserInteraction (ASCredentialRequest)' instead.")]
		[Export ("provideCredentialWithoutUserInteractionForIdentity:")]
		void ProvideCredentialWithoutUserInteraction (ASPasswordCredentialIdentity credentialIdentity);

		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity)' instead.")]
		[Export ("prepareInterfaceToProvideCredentialForIdentity:")]
		void PrepareInterfaceToProvideCredential (ASPasswordCredentialIdentity credentialIdentity);

		[Export ("prepareInterfaceForExtensionConfiguration")]
		void PrepareInterfaceForExtensionConfiguration ();

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareCredentialListForServiceIdentifiers:requestParameters:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers, ASPasskeyCredentialRequestParameters requestParameters);

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("provideCredentialWithoutUserInteractionForRequest:")]
		void ProvideCredentialWithoutUserInteraction (IASCredentialRequest credentialRequest);

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareInterfaceToProvideCredentialForRequest:")]
		void PrepareInterfaceToProvideCredential (IASCredentialRequest credentialRequest);

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredential : NSCopying, NSSecureCoding, ASAuthorizationCredential {
		[Export ("initWithUser:password:")]
		NativeHandle Constructor (string user, string password);

		[Static]
		[Export ("credentialWithUser:password:")]
		ASPasswordCredential Create (string user, string password);

		[Export ("user")]
		string User { get; }

		[Export ("password")]
		string Password { get; }
	}

	delegate void ASWebAuthenticationSessionCompletionHandler ([NullAllowed] NSUrl callbackUrl, [NullAllowed] NSError error);

	[TV (16, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSession {

		[Deprecated (PlatformName.iOS, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Deprecated (PlatformName.TvOS, 17, 4, message: "Use the 'ASWebAuthenticationSessionCallback' overload instead.")]
		[Export ("initWithURL:callbackURLScheme:completionHandler:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] string callbackUrlScheme, ASWebAuthenticationSessionCompletionHandler completionHandler);

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("initWithURL:callback:completionHandler:")]
		NativeHandle Constructor (NSUrl url, ASWebAuthenticationSessionCallback callback, ASWebAuthenticationSessionCompletionHandler completionHandler);

		[Export ("start")]
		bool Start ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cancel")]
		void Cancel ();

		[iOS (13, 0), NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASWebAuthenticationPresentationContextProviding PresentationContextProvider { get; set; }

		[iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	enum ASAuthorizationScope {
		[Field ("ASAuthorizationScopeFullName")]
		FullName,
		[Field ("ASAuthorizationScopeEmail")]
		Email,
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASUserDetectionStatus : long {
		Unsupported,
		Unknown,
		LikelyReal,
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement : long {
		Required,
		Preferred,
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobAssertionOperation : long {
		Read,
		Write,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASPublicKeyCredentialClientDataCrossOriginValue : long {
		NotSet,
		CrossOrigin,
		SameOriginWithAncestors,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialAttachment : long {
		Platform,
		CrossPlatform,
	}

	[NoTV, Mac (14, 0), NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionFederationType : long {
		None = 0,
		WSTrust = 1,
		DynamicWSTrust = 2,
	}

	[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
	[Native]
	public enum ASUserAgeRange : long {
		Unknown,
		Child,
		NotChild,
	}

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdProviderCredentialState : long {
		Revoked,
		Authorized,
		NotFound,
		Transferred,
	}

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest), Name = "ASAuthorizationAppleIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationAppleIDRequest init]: unrecognized selector sent to instance 0x600002ff8b40 
	interface ASAuthorizationAppleIdRequest {

		[NullAllowed, Export ("user")]
		string User { get; set; }
	}

	interface IASAuthorizationControllerDelegate { }

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAuthorizationController:")]
		UIWindow GetPresentationAnchor (ASAuthorizationController controller);
	}

	[TV (13, 0), iOS (13, 0)]
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

		[NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("performRequestsWithOptions:")]
		void PerformRequests (ASAuthorizationControllerRequestOptions options);

		[TV (18, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("cancel")]
		void Cancel ();
	}

	interface IASAuthorizationCredential { }

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationCredential : NSCopying, NSSecureCoding { }

	[TV (13, 0), iOS (13, 0)]
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
	}

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPasswordProvider : ASAuthorizationProvider {

		[Export ("createRequest")]
		ASAuthorizationPasswordRequest CreateRequest ();
	}

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationRequest))]
	// Name: NSInvalidArgumentException Reason: -[ASAuthorizationPasswordRequest init]: unrecognized selector sent to instance 0x6000005f2dc0
	[DisableDefaultCtor]
	interface ASAuthorizationPasswordRequest { }

	interface IASAuthorizationProvider { }

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationProvider { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, iOS (13, 0)]
	[Protocol]
	interface ASAuthorizationProviderExtensionAuthorizationRequestHandler {

		[Abstract]
		[Export ("beginAuthorizationWithRequest:")]
		void BeginAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);

		[Export ("cancelAuthorizationWithRequest:")]
		void CancelAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, iOS (13, 0)]
	enum ASAuthorizationProviderAuthorizationOperation {
		// no value yet - but we must handle `nil` as a default value
		[DefaultEnumValue]
		[Field (null)]
		None,

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationConfigurationRemoved")]
		ConfigurationRemoved,

		[NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationDirectRequest")]
		DirectRequest,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, iOS (13, 0)]
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

		[Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("userInterfaceEnabled")]
		bool UserInterfaceEnabled { [Bind ("isUserInterfaceEnabled")] get; }

		[NullAllowed]
		[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
		[Export ("loginManager", ArgumentSemantic.Strong)]
		ASAuthorizationProviderExtensionLoginManager LoginManager { get; }

		[NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
		[Export ("callerAuditToken")]
		NSData CallerAuditToken { get; }

	}

	[TV (13, 0), iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
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

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdButtonStyle : long {
		White = 0,
		WhiteOutline = 1,
		Black = 2,
	}

	[TV (13, 0), iOS (13, 0)]
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

	[NoTV, iOS (13, 0)]
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
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
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

		[Mac (12, 3)]
		[NoMacCatalyst]
		[Static]
		[Export ("registerDefaultsForASWASInSetupAssistantIfNeeded")]
		void RegisterDefaultsForAswasInSetupAssistantIfNeeded ();
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
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
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

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (16, 0)]
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

	[Flags, NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionSupportedGrantTypes : long {
		None = 0x0,
		Password = 1L << 0,
		JwtBearer = 1L << 1,
		Saml11 = 1L << 2,
		Saml20 = 1L << 3,
	}

	[NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
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

#if !XAMCORE_5_0 // Removed in Xcode 14.3 Beta 3
	[Obsoleted (PlatformName.iOS, 16, 4, message: Constants.ApiRemovedGeneral)]
	[Obsoleted (PlatformName.MacCatalyst, 16, 4, message: Constants.ApiRemovedGeneral)]
	[Obsoleted (PlatformName.TvOS, 16, 4, message: Constants.ApiRemovedGeneral)]
	[Mac (13, 3), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
#else
	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 3)]
#endif
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialAssertionRequest : ASAuthorizationPublicKeyCredentialAssertionRequest {
		/* issues when overriding this property */
		[Sealed]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] PlatformAllowedCredentials { get; set; }

		[NullAllowed]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

		[iOS (17, 0), NoTV, Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[Obsolete ("Use 'LargeBlob2' instead, this property has an incorrect property type..")]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {

		[Mac (14, 5), iOS (17, 5), MacCatalyst (17, 5)]
		[Export ("transports", ArgumentSemantic.Assign)]
		[BindAs (typeof (ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport []))]
		NSString [] Transports { get; }
	}

	[TV (16, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPlatformPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {

		[iOS (17, 0), NoTV, Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[NullAllowed]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prf"), NullAllowed]
		ASAuthorizationPublicKeyCredentialPrfRegistrationOutput Prf { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("deviceContext", ArgumentSemantic.Copy)]
		NSData DeviceContext { get; set; }

		[Mac (14, 4)]
		[Export ("userSecureEnclaveKeyBiometricPolicy", ArgumentSemantic.Assign)]
		ASAuthorizationProviderExtensionUserSecureEnclaveKeyBiometricPolicy UserSecureEnclaveKeyBiometricPolicy { get; set; }

		[Mac (14, 0)]
		[Export ("jwksTrustedRootCertificates", ArgumentSemantic.Copy)]
		NSObject [] JwksTrustedRootCertificates { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("refreshEndpointURL", ArgumentSemantic.Copy)]
		NSUrl RefreshEndpointUrl { get; set; }

		[Mac (14, 0)]
		[NullAllowed, Export ("uniqueIdentifierClaimName")]
		string UniqueIdentifierClaimName { get; set; }

		[Mac (14, 0)]
		[NullAllowed, Export ("customRequestJWTParameterName")]
		string CustomRequestJwtParameterName { get; set; }

		[Mac (14, 0)]
		[NullAllowed, Export ("groupRequestClaimName")]
		string GroupRequestClaimName { get; set; }

		[Mac (14, 0)]
		[NullAllowed, Export ("groupResponseClaimName")]
		string GroupResponseClaimName { get; set; }

		[Mac (14, 0)]
		[Internal]
		[Export ("loginRequestEncryptionPublicKey", ArgumentSemantic.Assign)]
		IntPtr /* SecKeyRef */ _LoginRequestEncryptionPublicKey { get; set; }

		[Mac (14, 0)]
		SecKey LoginRequestEncryptionPublicKey {
			[Wrap ("new SecKey (this._LoginRequestEncryptionPublicKey, owns: false)")]
			get;
			[Wrap ("_LoginRequestEncryptionPublicKey = value.Handle")]
			set;
		}

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("keyEndpointURL", ArgumentSemantic.Copy)]
		NSUrl KeyEndpointUrl { get; set; }

		[Mac (14, 0)]
		[Export ("customKeyExchangeRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomKeyExchangeRequestValues { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("loginRequestEncryptionAPVPrefix", ArgumentSemantic.Copy)]
		NSData LoginRequestEncryptionApvPrefix { get; set; }

		[Mac (14, 0)]
		[Export ("customRefreshRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomRefreshRequestValues { get; set; }

		[Mac (14, 0)]
		[Export ("setCustomRefreshRequestHeaderClaims:returningError:")]
		bool SetCustomRefreshRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("setCustomRefreshRequestBodyClaims:returningError:")]
		bool SetCustomRefreshRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("customKeyRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomKeyRequestValues { get; set; }

		[Mac (14, 0)]
		[Export ("setCustomKeyRequestHeaderClaims:returningError:")]
		bool SetCustomKeyRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("setCustomKeyRequestBodyClaims:returningError:")]
		bool SetCustomKeyRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("setCustomKeyExchangeRequestHeaderClaims:returningError:")]
		bool SetCustomKeyExchangeRequestHeaderClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("setCustomKeyExchangeRequestBodyClaims:returningError:")]
		bool SetCustomKeyExchangeRequestBodyClaims (NSDictionary<NSString, NSObject> claims, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[NullAllowed, Export ("additionalAuthorizationScopes")]
		string AdditionalAuthorizationScopes { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("federationRequestURN")]
		string FederationRequestUrn { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("federationMEXURL", ArgumentSemantic.Copy)]
		NSUrl FederationMexUrl { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("federationUserPreauthenticationURL", ArgumentSemantic.Copy)]
		NSUrl FederationUserPreauthenticationUrl { get; set; }

		[Mac (14, 0)]
		[Export ("federationType", ArgumentSemantic.Assign)]
		ASAuthorizationProviderExtensionFederationType FederationType { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("federationPredicate")]
		string FederationPredicate { get; set; }

		[Mac (14, 0)]
		[Export ("customFederationUserPreauthenticationRequestValues", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] CustomFederationUserPreauthenticationRequestValues { get; set; }

		[Mac (14, 0)]
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

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

		[Wrap ("_Save (certificate.GetHandle (), keyType)")]
		void Save (SecCertificate certificate, ASAuthorizationProviderExtensionKeyType keyType);

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

		[Mac (14, 0)]
		[Export ("resetUserSecureEnclaveKey")]
		void ResetUserSecureEnclaveKey ();

		[Mac (14, 0)]
		[Export ("resetDeviceKeys")]
		void ResetDeviceKeys ();

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("userLoginConfiguration", ArgumentSemantic.Copy)]
		ASAuthorizationProviderExtensionUserLoginConfiguration UserLoginConfiguration { get; }

		[Mac (14, 0)]
		[Export ("decryptionKeysNeedRepair")]
		void DecryptionKeysNeedRepair ();

		[Mac (14, 0)]
		[Export ("saveUserLoginConfiguration:error:")]
		bool SaveUserLoginConfiguration (ASAuthorizationProviderExtensionUserLoginConfiguration userLoginConfiguration, [NullAllowed] out NSError error);

		[Mac (14, 0)]
		[Export ("extensionData")]
		NSDictionary ExtensionData { get; }

		[Mac (15, 0)]
		[return: NullAllowed]
		[Export ("beginKeyRotationForKeyType:")]
		SecKey BeginKeyRotation (ASAuthorizationProviderExtensionKeyType keyType); // FIXME: CF_RETURNS_RETAINED;

		[Mac (15, 0)]
		[Export ("completeKeyRotationForKeyType:")]
		void CompleteKeyRotation (ASAuthorizationProviderExtensionKeyType keyType);
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Protocol]
#if NET
	[Model]
#else
	[Model (AutoGeneratedName = true)]
#endif
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

		[Mac (14, 0)]
		[Export ("supportedGrantTypes")]
		ASAuthorizationProviderExtensionSupportedGrantTypes SupportedGrantTypes { get; }

		[Mac (14, 0)]
		[Export ("registrationDidCancel")]
		void RegistrationDidCancel ();

		[Mac (14, 0)]
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
	}

	interface IASAuthorizationWebBrowserExternallyAuthenticatableRequest { }

	[NoTV, NoiOS, NoMacCatalyst, Mac (13, 3)]
	[Protocol]
	interface ASAuthorizationWebBrowserExternallyAuthenticatableRequest {

		[Abstract]
		[NullAllowed, Export ("authenticatedContext", ArgumentSemantic.Assign)]
		LAContext AuthenticatedContext { get; set; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (16, 4), Mac (13, 3)]
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

		[MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("providerName")]
		string ProviderName { get; }

		[MacCatalyst (17, 0), Mac (14, 0)]
		[NullAllowed, Export ("customTitle")]
		string CustomTitle { get; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (16, 4), Mac (13, 3)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface ASAuthorizationWebBrowserPublicKeyCredentialManager {

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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface ASCredentialRequest : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("type")]
		ASCredentialRequestType Type { get; }

		[Abstract]
		[Export ("credentialIdentity")]
		IASCredentialIdentity CredentialIdentity { get; }
	}

	[NoTV, MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput {
		[Export ("initWithSupportRequirement:")]
		NativeHandle Constructor (ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement requirement);

		[Export ("supportRequirement", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement SupportRequirement { get; set; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput {
		[NullAllowed, Export ("readData")]
		NSData ReadData { get; }

		[Export ("didWrite")]
		bool DidWrite { get; }
	}

	[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationOutput : NSCopying, NSSecureCoding {
		[Export ("isSupported")]
		bool IsSupported { get; }
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 0), Mac (14, 0)]
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

	[NoTV, NoiOS, NoMacCatalyst, Mac (14, 0)]
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

	[NoTV, iOS (17, 4), MacCatalyst (17, 0), Mac (14, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialProvider {
		[Abstract]
		[Export ("createCredentialRegistrationRequestWithClientData:name:userID:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string name, NSData userId);

		[Abstract]
		[Export ("createCredentialAssertionRequestWithClientData:")]
		ASAuthorizationPlatformPublicKeyCredentialAssertionRequest CreateCredentialAssertionRequest (ASPublicKeyCredentialClientData clientData);

#if NET
		[Abstract]
#endif
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("createCredentialRegistrationRequestWithClientData:name:userID:requestStyle:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string name, NSData userId, ASAuthorizationPlatformPublicKeyCredentialRegistrationRequestStyle requestStyle);
	}

	[NoTV, iOS (17, 4), MacCatalyst (17, 0), Mac (14, 0)]
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
}
