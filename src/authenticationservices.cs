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
#if WATCH
using UIControl = Foundation.NSObject;
using UIViewController = Foundation.NSObject;
using UIWindow = Foundation.NSObject;
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

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[Native]
	[ErrorDomain ("ASCredentialIdentityStoreErrorDomain")]
	public enum ASCredentialIdentityStoreErrorCode : long {
		InternalError = 0,
		StoreDisabled = 1,
		StoreBusy = 2,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[Native]
	[ErrorDomain ("ASExtensionErrorDomain")]
	public enum ASExtensionErrorCode : long {
		Failed = 0,
		UserCanceled = 1,
		UserInteractionRequired = 100,
		CredentialIdentityNotFound = 101,
	}

	[Partial]
	interface ASExtensionErrorCodeExtensions {

#if NET || TVOS || WATCH
		// Type `ASExtensionErrorCode` is already decorated, so it becomes a duplicate (after code gen)
		// on those platforms and intro tests complains (on other platforms, e.g. iOS, Catalyst)
		// OTOH if we don't add them here then we'll get the extra, not really usable, extension type
		// on tvOS and watchOS (which is incorrect)
		[NoTV][NoWatch]
#endif
		[NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("ASExtensionLocalizedFailureReasonErrorKey")]
		NSString LocalizedFailureReasonErrorKey { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[Native]
	public enum ASCredentialServiceIdentifierType : long {
		Domain,
		Url,
	}

	[TV (16, 0)]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("ASWebAuthenticationSessionErrorDomain")]
	public enum ASWebAuthenticationSessionErrorCode : long {
		CanceledLogin = 1,
		PresentationContextNotProvided = 2,
		PresentationContextInvalid = 3,
	}

	[Flags, NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum ASAuthorizationControllerRequestOptions : ulong {
		ImmediatelyAvailableCredentials = 1uL << 0,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Native]
	public enum ASAuthorizationProviderExtensionAuthenticationMethod : long {
		Password = 1,
		UserSecureEnclaveKey = 2,
		[Mac (14, 0)]
		SmartCard = 3,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
	[Native]
	public enum ASAuthorizationProviderExtensionRegistrationResult : long {
		Success = 0,
		Failed = 1,
		UserInterfaceRequired = 2,
		FailedNoRetry = 3,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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
	}

	[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (16, 4), Mac (13, 3)]
	[Native]
	public enum ASAuthorizationWebBrowserPublicKeyCredentialManagerAuthorizationState : long {
		Authorized,
		Denied,
		NotDetermined,
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (14, 0)]
	[Native]
	public enum ASCredentialRequestType : long {
		Password = 0,
		PasskeyAssertion,
	}

	delegate void ASCredentialIdentityStoreCompletionHandler (bool success, NSError error);

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
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
		[Deprecated (PlatformName.MacOSX, 17, 0, message: "Use 'ReplaceCredentialIdentityEntries (ASPasswordCredentialIdentity [])' instead.")]
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

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStoreState {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("supportsIncrementalUpdates")]
		bool SupportsIncrementalUpdates { get; }
	}

	delegate void ASCredentialProviderExtensionRequestCompletionHandler (bool expired);

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
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
		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("completeRegistrationRequestWithSelectedPasskeyCredential:completionHandler:")]
		void CompleteRegistrationRequest (ASPasskeyRegistrationCredential credential, [NullAllowed] Action<bool> completionHandler);

		[Async]
		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("completeAssertionRequestWithSelectedPasskeyCredential:completionHandler:")]
		void CompleteAssertionRequest (ASPasskeyAssertionCredential credential, [NullAllowed] Action<bool> completionHandler);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
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

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
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

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[Mac (11, 0)]
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

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareCredentialListForServiceIdentifiers:requestParameters:")]
		void PrepareCredentialList (ASCredentialServiceIdentifier [] serviceIdentifiers, ASPasskeyCredentialRequestParameters requestParameters);

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("provideCredentialWithoutUserInteractionForRequest:")]
		void ProvideCredentialWithoutUserInteraction (IASCredentialRequest credentialRequest);

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareInterfaceToProvideCredentialForRequest:")]
		void PrepareInterfaceToProvideCredential (IASCredentialRequest credentialRequest);

		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prepareInterfaceForPasskeyRegistration:")]
		void PrepareInterfaceForPasskeyRegistration (IASCredentialRequest registrationRequest);

	}

	[Watch (6, 0), TV (13, 0)]
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
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSession {
		[Export ("initWithURL:callbackURLScheme:completionHandler:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] string callbackUrlScheme, ASWebAuthenticationSessionCompletionHandler completionHandler);

		[Export ("start")]
		bool Start ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cancel")]
		void Cancel ();

		[iOS (13, 0), NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASWebAuthenticationPresentationContextProviding PresentationContextProvider { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("prefersEphemeralWebBrowserSession")]
		bool PrefersEphemeralWebBrowserSession { get; set; }

		[iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("canStart")]
		bool CanStart { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	enum ASAuthorizationScope {
		[Field ("ASAuthorizationScopeFullName")]
		FullName,
		[Field ("ASAuthorizationScopeEmail")]
		Email,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASUserDetectionStatus : long {
		Unsupported,
		Unknown,
		LikelyReal,
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement : long {
		Required,
		Preferred,
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialLargeBlobAssertionOperation : long {
		Read,
		Write,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASPublicKeyCredentialClientDataCrossOriginValue : long {
		NotSet,
		CrossOrigin,
		SameOriginWithAncestors,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum ASAuthorizationPublicKeyCredentialAttachment : long {
		Platform,
		CrossPlatform,
	}

	[NoWatch, NoTV, Mac (14, 0), NoiOS, NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionFederationType : long {
		None = 0,
		WSTrust = 1,
		DynamicWSTrust = 2,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdProviderCredentialState : long {
		Revoked,
		Authorized,
		NotFound,
		Transferred,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest), Name = "ASAuthorizationAppleIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationAppleIDRequest init]: unrecognized selector sent to instance 0x600002ff8b40 
	interface ASAuthorizationAppleIdRequest {

		[NullAllowed, Export ("user")]
		string User { get; set; }
	}

	interface IASAuthorizationControllerDelegate { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("authorizationController:didCompleteWithCustomMethod:")]
		void DidComplete (ASAuthorizationController controller, NSString method);
	}

	interface IASAuthorizationControllerPresentationContextProviding { }

	[TV (13, 0), NoWatch, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAuthorizationController:")]
		UIWindow GetPresentationAnchor (ASAuthorizationController controller);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASAuthorizationControllerPresentationContextProviding PresentationContextProvider { get; set; }

		[Export ("performRequests")]
		void PerformRequests ();

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("customAuthorizationMethods", ArgumentSemantic.Copy)]
		NSString [] CustomAuthorizationMethods { get; set; }

		[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (16, 0)]
		[Export ("performAutoFillAssistedRequests")]
		void PerformAutoFillAssistedRequests ();

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("performRequestsWithOptions:")]
		void PerformRequests (ASAuthorizationControllerRequestOptions options);

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("cancel")]
		void Cancel ();
	}

	interface IASAuthorizationCredential { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationCredential : NSCopying, NSSecureCoding { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPasswordProvider : ASAuthorizationProvider {

		[Export ("createRequest")]
		ASAuthorizationPasswordRequest CreateRequest ();
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationRequest))]
	// Name: NSInvalidArgumentException Reason: -[ASAuthorizationPasswordRequest init]: unrecognized selector sent to instance 0x6000005f2dc0
	[DisableDefaultCtor]
	interface ASAuthorizationPasswordRequest { }

	interface IASAuthorizationProvider { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ASAuthorizationProvider { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[Protocol]
	interface ASAuthorizationProviderExtensionAuthorizationRequestHandler {

		[Abstract]
		[Export ("beginAuthorizationWithRequest:")]
		void BeginAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);

		[Export ("cancelAuthorizationWithRequest:")]
		void CancelAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	enum ASAuthorizationProviderAuthorizationOperation {
		// no value yet - but we must handle `nil` as a default value
		[DefaultEnumValue]
		[Field (null)]
		None,

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationConfigurationRemoved")]
		ConfigurationRemoved,

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("ASAuthorizationProviderAuthorizationOperationDirectRequest")]
		DirectRequest,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
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

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Export ("callerManaged")]
		bool CallerManaged { [Bind ("isCallerManaged")] get; }

		[iOS (14, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Export ("callerTeamIdentifier")]
		string CallerTeamIdentifier { get; }

		[iOS (14, 0)]
		[Mac (11, 0)]
		[MacCatalyst (14, 0)]
		[Export ("localizedCallerDisplayName")]
		string LocalizedCallerDisplayName { get; }

		[Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("userInterfaceEnabled")]
		bool UserInterfaceEnabled { [Bind ("isUserInterfaceEnabled")] get; }

		[NullAllowed]
		[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
		[Export ("loginManager", ArgumentSemantic.Strong)]
		ASAuthorizationProviderExtensionLoginManager LoginManager { get; }

		[NoWatch, NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
		[Export ("callerAuditToken")]
		NSData CallerAuditToken { get; }

	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[NoWatch, NoTV, iOS (13, 0)]
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

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("privateKeys")]
		SecKey [] PrivateKeys { get; }
	}

	[NoWatch, NoTV, iOS (13, 0)]
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

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationSingleSignOnRequest init]: unrecognized selector sent to instance 0x60000095aa60
	interface ASAuthorizationSingleSignOnRequest {

		[Export ("authorizationOptions", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] AuthorizationOptions { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("userInterfaceEnabled")]
		bool UserInterfaceEnabled { [Bind ("isUserInterfaceEnabled")] get; set; }
	}

	[TV (13, 0), NoWatch, iOS (13, 0)]
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

	[TV (13, 0), NoWatch, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum ASAuthorizationAppleIdButtonStyle : long {
		White = 0,
		WhiteOutline = 1,
		Black = 2,
	}

	[TV (13, 0), NoWatch, iOS (13, 0)]
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

	[NoWatch, NoTV, iOS (13, 0)]
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
	[NoWatch]
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
	[NoWatch]
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
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSessionRequest : NSSecureCoding, NSCopying {

		[Export ("UUID")]
		NSUuid Uuid { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[NullAllowed, Export ("callbackURLScheme")]
		string CallbackUrlScheme { get; }

		[Export ("shouldUseEphemeralSession")]
		bool ShouldUseEphemeralSession { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IASWebAuthenticationSessionRequestDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("cancelWithError:")]
		void Cancel (NSError error);

		[Export ("completeWithCallbackURL:")]
		void Complete (NSUrl callbackUrl);
	}

	[NoTV]
	[NoiOS]
	[NoWatch]
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
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAccountAuthenticationModificationRequest {
	}

	interface IASAccountAuthenticationModificationControllerDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoWatch, NoTV, NoMac]
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
	[NoWatch, NoTV, NoMac]
	[Protocol]
	interface ASAccountAuthenticationModificationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAccountAuthenticationModificationController:")]
		UIWindow GetPresentationAnchor (ASAccountAuthenticationModificationController controller);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[iOS (14, 0)]
	[NoWatch, NoTV, NoMac]
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
	[NoWatch, NoTV, NoMac]
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
	[NoWatch, NoTV, NoMac]
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
	[NoWatch, NoTV, NoMac]
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
	[NoWatch, NoTV, NoMac]
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

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), NoWatch, NoTV]
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

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), NoWatch, TV (16, 0)]
	[Static]
	interface ASAuthorizationPublicKeyCredentialUserVerificationPreference {
		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferencePreferred")]
		NSString Preferred { get; }

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceRequired")]
		NSString Required { get; }

		[Field ("ASAuthorizationPublicKeyCredentialUserVerificationPreferenceDiscouraged")]
		NSString Discouraged { get; }
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), NoWatch, NoTV]
	enum ASAuthorizationPublicKeyCredentialResidentKeyPreference {
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferenceDiscouraged")]
		Discouraged,
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferencePreferred")]
		Preferred,
		[Field ("ASAuthorizationPublicKeyCredentialResidentKeyPreferenceRequired")]
		Required,
	}

	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), NoWatch, NoTV]
	enum ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport {
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportUSB")]
		Usb,
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportNFC")]
		Nfc,
		[Field ("ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransportBluetooth")]
		Bluetooth,
	}

	[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch, NoTV]
	[Native]
	enum ASCoseAlgorithmIdentifier : long {
		ES256 = -7,
	}

	// Introduced in Xcode13 Beta3 but not used anywhere
	// [Mac (12,0), iOS (15,0), MacCatalyst (15,0), NoWatch, NoTV]
	// [Native]
	// enum AscoseEllipticCurveIdentifier : long {
	// 	P256 = 1,
	// }
	//

	[Flags, NoWatch, NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionSupportedGrantTypes : long {
		None = 0x0,
		Password = 1L << 0,
		JwtBearer = 1L << 1,
		Saml11 = 1L << 2,
		Saml20 = 1L << 3,
	}

	[NoWatch, NoTV, NoiOS, Mac (14, 0), NoMacCatalyst]
	[Native]
	public enum ASAuthorizationProviderExtensionPlatformSSOProtocolVersion : long {
		Version1_0 = 0,
		Version2_0 = 1,
	}


	interface IASAuthorizationPublicKeyCredentialAssertion { }

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
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
	[Obsoleted (PlatformName.WatchOS, 9, 4, message: Constants.ApiRemovedGeneral)]
	[NoWatch, Mac (13, 3), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
#else
	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 3)]
#endif
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialAssertionRequest : ASAuthorizationPublicKeyCredentialAssertionRequest {
		/* issues when overriding this property */
		[Sealed]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] PlatformAllowedCredentials { get; set; }

		[NullAllowed]
		[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobAssertionInput LargeBlob { get; set; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialAssertionRequest : ASAuthorizationPublicKeyCredentialAssertionRequest {
		/* issues when overriding this property */
		[Sealed]
		[Export ("allowedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationSecurityKeyPublicKeyCredentialDescriptor [] SecurityAllowedCredentials { get; set; }
	}

	interface IASAuthorizationPublicKeyCredentialAssertionRequest { }

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
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

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialDescriptor : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; set; }
	}

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASAuthorizationPublicKeyCredentialRegistration : ASPublicKeyCredential {
		[Abstract]
		[NullAllowed, Export ("rawAttestationObject", ArgumentSemantic.Copy)]
		NSData RawAttestationObject { get; }
	}

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
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

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[Protocol]
	interface ASPublicKeyCredential : ASAuthorizationCredential {
		[Abstract]
		[Export ("rawClientDataJSON", ArgumentSemantic.Copy)]
		NSData RawClientDataJson { get; }

		[Abstract]
		[Export ("credentialID", ArgumentSemantic.Copy)]
		NSData CredentialId { get; }
	}

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialDescriptor : ASAuthorizationPublicKeyCredentialDescriptor {
		[Export ("initWithCredentialID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData credentialId);
	}

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), TV (16, 0)]
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
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialParameters : NSSecureCoding, NSCopying {
		[Export ("initWithAlgorithm:")]
		NativeHandle Constructor (ASCoseAlgorithmIdentifier algorithm);

		[Export ("algorithm")]
		ASCoseAlgorithmIdentifier Algorithm { get; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[TV (15, 0), NoWatch, NoiOS, NoMac, NoMacCatalyst]
	[Static]
	interface ASAuthorizationCustomMethod {
		[Field ("ASAuthorizationCustomMethodVideoSubscriberAccount")]
		NSString SubscriberAccount { get; }

		[Field ("ASAuthorizationCustomMethodRestorePurchase")]
		NSString RestorePurchase { get; }

		[Field ("ASAuthorizationCustomMethodOther")]
		NSString Other { get; }
	}

	[NoWatch, TV (16, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPlatformPublicKeyCredentialAssertion : ASAuthorizationPublicKeyCredentialAssertion {

		[iOS (17, 0), NoWatch, NoTV, Mac (14, 0), NoMacCatalyst]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }
	}

	[NoWatch, TV (16, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	[DisableDefaultCtor]
	interface ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest : ASAuthorizationPublicKeyCredentialRegistrationRequest {

		[NullAllowed]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; set; }
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialAssertion : ASAuthorizationPublicKeyCredentialAssertion {
	}

	[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationSecurityKeyPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {
	}

	[NoWatch, TV (16, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPlatformPublicKeyCredentialRegistration : ASAuthorizationPublicKeyCredentialRegistration {

		[iOS (17, 0), NoWatch, NoTV, Mac (14, 0), NoMacCatalyst]
		[Export ("attachment")]
		ASAuthorizationPublicKeyCredentialAttachment Attachment { get; }

		[NullAllowed]
		[NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("largeBlob", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput LargeBlob { get; }
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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

	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 0)]
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
	}

	interface IASAuthorizationWebBrowserExternallyAuthenticatableRequest { }

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (13, 3)]
	[Protocol]
	interface ASAuthorizationWebBrowserExternallyAuthenticatableRequest {

		[Abstract]
		[NullAllowed, Export ("authenticatedContext", ArgumentSemantic.Assign)]
		LAContext AuthenticatedContext { get; set; }
	}

	[NoWatch, NoTV, NoiOS, MacCatalyst (16, 4), Mac (13, 3)]
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

	[NoWatch, NoTV, NoiOS, MacCatalyst (16, 4), Mac (13, 3)]
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

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	interface IASCredentialRequest { }

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface ASCredentialRequest : NSSecureCoding, NSCopying {
		[Abstract]
		[Export ("type")]
		ASCredentialRequestType Type { get; }

		[Abstract]
		[Export ("credentialIdentity")]
		IASCredentialIdentity CredentialIdentity { get; }
	}

	[NoWatch, NoTV, MacCatalyst (17, 0), Mac (14, 0), iOS (17, 0)]
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

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface ASPasskeyRegistrationCredential : ASAuthorizationCredential {
		[Export ("initWithRelyingParty:clientDataHash:credentialID:attestationObject:")]
		NativeHandle Constructor (string relyingParty, NSData clientDataHash, NSData credentialId, NSData attestationObject);

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
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasskeyCredentialRequest : ASCredentialRequest {
		[Export ("initWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:")]
		NativeHandle Constructor (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, string userVerificationPreference, NSNumber [] supportedAlgorithms);

		[Export ("clientDataHash")]
		NSData ClientDataHash { get; }

		[Export ("userVerificationPreference")]
		string UserVerificationPreference { get; set; }

		[Export ("supportedAlgorithms")]
		NSNumber [] SupportedAlgorithms { get; }

		[Static]
		[Export ("requestWithCredentialIdentity:clientDataHash:userVerificationPreference:supportedAlgorithms:")]
		ASPasskeyCredentialRequest Create (ASPasskeyCredentialIdentity credentialIdentity, NSData clientDataHash, string userVerificationPreference, NSNumber [] supportedAlgorithms);

	}

	interface IASCredentialIdentity { }

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ASPasskeyAssertionCredential : ASAuthorizationCredential {
		[Export ("initWithUserHandle:relyingParty:signature:clientDataHash:authenticatorData:credentialID:")]
		NativeHandle Constructor (NSData userHandle, string relyingParty, NSData signature, NSData clientDataHash, NSData authenticatorData, NSData credentialId);

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
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationInput {
		[Export ("initWithSupportRequirement:")]
		NativeHandle Constructor (ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement requirement);

		[Export ("supportRequirement", ArgumentSemantic.Assign)]
		ASAuthorizationPublicKeyCredentialLargeBlobSupportRequirement SupportRequirement { get; set; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationPublicKeyCredentialLargeBlobAssertionOutput {
		[NullAllowed, Export ("readData")]
		NSData ReadData { get; }

		[Export ("didWrite")]
		bool DidWrite { get; }
	}

	[NoWatch, NoTV, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPublicKeyCredentialLargeBlobRegistrationOutput {
		[Export ("isSupported")]
		bool IsSupported { get; }
	}

	[NoWatch, NoTV, NoiOS, MacCatalyst (17, 0), Mac (14, 0)]
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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (14, 0)]
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

	[NoWatch, NoTV, NoiOS, MacCatalyst (17, 0), Mac (14, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialProvider {
		[Abstract]
		[Export ("createCredentialRegistrationRequestWithClientData:name:userID:")]
		ASAuthorizationPlatformPublicKeyCredentialRegistrationRequest CreateCredentialRegistrationRequest (ASPublicKeyCredentialClientData clientData, string name, NSData userId);

		[Abstract]
		[Export ("createCredentialAssertionRequestWithClientData:")]
		ASAuthorizationPlatformPublicKeyCredentialAssertionRequest CreateCredentialAssertionRequest (ASPublicKeyCredentialClientData clientData);
	}

	[NoWatch, NoTV, NoiOS, MacCatalyst (17, 0), Mac (14, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialRegistrationRequest {
		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }

		[Abstract]
		[NullAllowed, Export ("excludedCredentials", ArgumentSemantic.Copy)]
		ASAuthorizationPlatformPublicKeyCredentialDescriptor [] ExcludedCredentials { get; set; }

		[Abstract]
		[Export ("shouldShowHybridTransport")]
		bool ShouldShowHybridTransport { get; set; }
	}

	[NoWatch, NoTV, NoiOS, MacCatalyst (17, 0), Mac (14, 0)]
	[Protocol]
	interface ASAuthorizationWebBrowserPlatformPublicKeyCredentialAssertionRequest {
		[Abstract]
		[Export ("clientData")]
		ASPublicKeyCredentialClientData ClientData { get; }

		[Abstract]
		[Export ("shouldShowHybridTransport")]
		bool ShouldShowHybridTransport { get; set; }
	}

}
