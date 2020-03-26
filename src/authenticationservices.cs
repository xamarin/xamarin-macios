//
// AuthenticationServices bindings
//
// Copyright 2018-2019 Microsoft Corporation
//

using System;
using Foundation;
using ObjCRuntime;
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

namespace AuthenticationServices {

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASCredentialIdentityStoreErrorDomain")]
	public enum ASCredentialIdentityStoreErrorCode : long {
		InternalError = 0,
		StoreDisabled = 1,
		StoreBusy = 2,
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASExtensionErrorDomain")]
	public enum ASExtensionErrorCode : long {
		Failed = 0,
		UserCanceled = 1,
		UserInteractionRequired = 100,
		CredentialIdentityNotFound = 101,
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
	[iOS (12,0)]
	[Native]
	public enum ASCredentialServiceIdentifierType : long {
		Domain,
		Url,
	}

	[NoTV]
	[Watch (6,2)]
	[Mac (10,15)]
	[iOS (12,0)]
	[Native]
	[ErrorDomain ("ASWebAuthenticationSessionErrorDomain")]
	public enum ASWebAuthenticationSessionErrorCode : long {
		CanceledLogin = 1,
		PresentationContextNotProvided = 2,
		PresentationContextInvalid = 3,
	}

	delegate void ASCredentialIdentityStoreCompletionHandler (bool success, NSError error);

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
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
		void SaveCredentialIdentities (ASPasswordCredentialIdentity[] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Async]
		[Export ("removeCredentialIdentities:completion:")]
		void RemoveCredentialIdentities (ASPasswordCredentialIdentity[] credentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);

		[Async]
		[Export ("removeAllCredentialIdentitiesWithCompletion:")]
		void RemoveAllCredentialIdentities ([NullAllowed] Action<bool, NSError> completion);

		[Async]
		[Export ("replaceCredentialIdentitiesWithIdentities:completion:")]
		void ReplaceCredentialIdentities (ASPasswordCredentialIdentity[] newCredentialIdentities, [NullAllowed] ASCredentialIdentityStoreCompletionHandler completion);
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASCredentialIdentityStoreState {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("supportsIncrementalUpdates")]
		bool SupportsIncrementalUpdates { get; }
	}

	delegate void ASCredentialProviderExtensionRequestCompletionHandler (bool expired);

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
	[iOS (12,0)]
	[BaseType (typeof (NSExtensionContext))]
	[DisableDefaultCtor]
	interface ASCredentialProviderExtensionContext {
		[Export ("completeRequestWithSelectedCredential:completionHandler:")]
		void CompleteRequest (ASPasswordCredential credential, [NullAllowed] ASCredentialProviderExtensionRequestCompletionHandler completionHandler);

		[Export ("completeExtensionConfigurationRequest")]
		void CompleteExtensionConfigurationRequest ();

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
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

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
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

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoMac][NoTV][NoWatch]
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

	[Watch (6, 0), TV (13, 0), Mac (10, 15)]
	[iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASPasswordCredential : NSCopying, NSSecureCoding, ASAuthorizationCredential {
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

	[NoTV]
	[Watch (6,2)]
	[Mac (10, 15)]
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

		[iOS (13,0), NoWatch]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASWebAuthenticationPresentationContextProviding PresentationContextProvider { get; set; }

		[iOS (13,0)]
		[Export ("prefersEphemeralWebBrowserSession")]
		bool PrefersEphemeralWebBrowserSession { get; set; }

		[Mac (10,15,4), iOS (13,4)]
		[Export ("canStart")]
		bool CanStart { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
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

	[Watch (6, 0), TV (13, 0), Mac (10, 15), iOS (13, 0)]
	enum ASAuthorizationScope {
		[Field ("ASAuthorizationScopeFullName")]
		FullName,
		[Field ("ASAuthorizationScopeEmail")]
		Email,
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Native]
	enum ASUserDetectionStatus : long {
		Unsupported,
		Unknown,
		LikelyReal,
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject), Name = "ASAuthorizationAppleIDCredential")]
	[DisableDefaultCtor]
	interface ASAuthorizationAppleIdCredential : ASAuthorizationCredential {

		[Export ("user")]
		string User { get; }

		[NullAllowed, Export ("state")]
		string State { get; }

		[Export ("authorizedScopes", ArgumentSemantic.Copy)]
		[BindAs (typeof (ASAuthorizationScope []))]
		NSString[] AuthorizedScopes { get; }

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

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Native]
	enum ASAuthorizationAppleIdProviderCredentialState : long {
		Revoked,
		Authorized,
		NotFound,
		Transferred,
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
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

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest), Name = "ASAuthorizationAppleIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationAppleIDRequest init]: unrecognized selector sent to instance 0x600002ff8b40 
	interface ASAuthorizationAppleIdRequest {

		[NullAllowed, Export ("user")]
		string User { get; set; }
	}

	interface IASAuthorizationControllerDelegate {}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Protocol][Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationControllerDelegate {

		[Export ("authorizationController:didCompleteWithAuthorization:")]
		void DidComplete (ASAuthorizationController controller, ASAuthorization authorization);

		[Export ("authorizationController:didCompleteWithError:")]
		void DidComplete (ASAuthorizationController controller, NSError error);
	}

	interface IASAuthorizationControllerPresentationContextProviding { }

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Protocol]
	interface ASAuthorizationControllerPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForAuthorizationController:")]
		UIWindow GetPresentationAnchor (ASAuthorizationController controller);
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASAuthorizationController {

		[Export ("initWithAuthorizationRequests:")]
		[DesignatedInitializer]
		IntPtr Constructor (ASAuthorizationRequest[] authorizationRequests);

		[Export ("authorizationRequests", ArgumentSemantic.Strong)]
		ASAuthorizationRequest[] AuthorizationRequests { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IASAuthorizationControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoWatch]
		[NullAllowed, Export ("presentationContextProvider", ArgumentSemantic.Weak)]
		IASAuthorizationControllerPresentationContextProviding PresentationContextProvider { get; set; }

		[Export ("performRequests")]
		void PerformRequests ();
	}

	interface IASAuthorizationCredential { }

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Protocol]
	interface ASAuthorizationCredential : NSCopying, NSSecureCoding { }

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[ErrorDomain ("ASAuthorizationErrorDomain")]
	[Native]
	public enum ASAuthorizationError : long {
		Unknown = 1000,
		Canceled = 1001,
		InvalidResponse = 1002,
		NotHandled = 1003,
		Failed = 1004,
	}

	[Watch (6, 0), TV (13, 0), Mac (10, 15), iOS (13, 0)]
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

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (ASAuthorizationRequest), Name = "ASAuthorizationOpenIDRequest")]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationOpenIDRequest init]: unrecognized selector sent to instance 0x600002ff0660 
	interface ASAuthorizationOpenIdRequest {

		[NullAllowed, Export ("requestedScopes", ArgumentSemantic.Copy)]
		[BindAs (typeof (ASAuthorizationScope[]))]
		NSString[] RequestedScopes { get; set; }

		[NullAllowed, Export ("state")]
		string State { get; set; }

		[NullAllowed, Export ("nonce")]
		string Nonce { get; set; }

		[Export ("requestedOperation")]
		[BindAs (typeof (ASAuthorizationOperation))]
		NSString RequestedOperation { get; set; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (NSObject))]
	interface ASAuthorizationPasswordProvider : ASAuthorizationProvider {

		[Export ("createRequest")]
		ASAuthorizationPasswordRequest CreateRequest ();
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof (ASAuthorizationRequest))]
	// Name: NSInvalidArgumentException Reason: -[ASAuthorizationPasswordRequest init]: unrecognized selector sent to instance 0x6000005f2dc0
	[DisableDefaultCtor]
	interface ASAuthorizationPasswordRequest { }

	interface IASAuthorizationProvider { }

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[Protocol]
	interface ASAuthorizationProvider { }

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[Protocol]
	interface ASAuthorizationProviderExtensionAuthorizationRequestHandler {

		[Abstract]
		[Export ("beginAuthorizationWithRequest:")]
		void BeginAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);

		[Export ("cancelAuthorizationWithRequest:")]
		void CancelAuthorization (ASAuthorizationProviderExtensionAuthorizationRequest request);
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	enum ASAuthorizationProviderAuthorizationOperation {
		// no value yet - but we must handle `nil` as a default value
		[DefaultEnumValue]
		[Field (null)]
		None,
	}

	[Unavailable (PlatformName.UIKitForMac)][Advice ("This API is not available when using UIKit on macOS.")]
	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
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
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
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

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
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
		NSString[] AuthorizedScopes { get; }

		[NullAllowed, Export ("authenticatedResponse", ArgumentSemantic.Copy)]
		NSHttpUrlResponse AuthenticatedResponse { get; }
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
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

	[Introduced (PlatformName.UIKitForMac, 13, 0)]
	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof (ASAuthorizationOpenIdRequest))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[ASAuthorizationSingleSignOnRequest init]: unrecognized selector sent to instance 0x60000095aa60
	interface ASAuthorizationSingleSignOnRequest {

		[Export ("authorizationOptions", ArgumentSemantic.Copy)]
		NSUrlQueryItem[] AuthorizationOptions { get; set; }
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	enum ASAuthorizationAppleIdButtonType : long {
		SignIn,
		Continue,
		[TV (13,2), Mac (10,15,1), iOS (13,2)]
		SignUp,
		Default = SignIn,
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[Native]
	enum ASAuthorizationAppleIdButtonStyle : long {
		White = 0,
		WhiteOutline = 1,
		Black = 2,
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
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
		IntPtr Constructor (ASAuthorizationAppleIdButtonType type, ASAuthorizationAppleIdButtonStyle style);

		[NoTV]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	}

	interface IASWebAuthenticationPresentationContextProviding { }

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[Protocol]
	interface ASWebAuthenticationPresentationContextProviding {

		[Abstract]
		[Export ("presentationAnchorForWebAuthenticationSession:")]
		UIWindow GetPresentationAnchor (ASWebAuthenticationSession session);
	}

	interface IASWebAuthenticationSessionRequestDelegate { }

	[Introduced (PlatformName.UIKitForMac, 13, 0)]
	[Mac (10,15)]
	[NoTV][NoiOS][NoWatch]
	[Protocol][Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject))]
	interface ASWebAuthenticationSessionRequestDelegate {
	
		[Export ("authenticationSessionRequest:didCompleteWithCallbackURL:")]
		void DidComplete (ASWebAuthenticationSessionRequest authenticationSessionRequest, NSUrl callbackUrl);

		[Export ("authenticationSessionRequest:didCancelWithError:")]
		void DidCancel (ASWebAuthenticationSessionRequest authenticationSessionRequest, NSError error);
	}

	interface IASWebAuthenticationSessionWebBrowserSessionHandling { }

	[Mac (10,15)]
	[NoTV][NoiOS][NoWatch]
	[Protocol]
	interface ASWebAuthenticationSessionWebBrowserSessionHandling {

		[Abstract]
		[Export ("beginHandlingWebAuthenticationSessionRequest:")]
		void BeginHandlingWebAuthenticationSessionRequest (ASWebAuthenticationSessionRequest request);

		[Abstract]
		[Export ("cancelWebAuthenticationSessionRequest:")]
		void CancelWebAuthenticationSessionRequest (ASWebAuthenticationSessionRequest request);
	}

	[Introduced (PlatformName.UIKitForMac, 13, 0)]
	[Mac (10,15)]
	[NoTV][NoiOS][NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASWebAuthenticationSessionRequest : NSSecureCoding, NSCopying	{

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

	[Mac (10,15)]
	[NoTV][NoiOS][NoWatch]
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
	}
}
