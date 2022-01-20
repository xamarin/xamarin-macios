//
// VideoSubscriberAccount bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
// Copyright 2018-2019 Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using UIViewController = AppKit.NSViewController;
#else
using UIKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace VideoSubscriberAccount {

	[Native]
	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[ErrorDomain ("VSErrorDomain")]
	public enum VSErrorCode : long {
		AccessNotGranted = 0,
		UnsupportedProvider = 1,
		UserCancelled = 2,
		ServiceTemporarilyUnavailable = 3,
		ProviderRejected = 4,
		InvalidVerificationToken = 5,
		Rejected = 6,
		Unsupported = 7,
	}

	[Native]
	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	public enum VSAccountAccessStatus : long {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Granted = 3
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[Static]
	[Internal]
	interface VSErrorInfoKeys {

		[Field ("VSErrorInfoKeySAMLResponse")]
		NSString SamlResponseKey { get; }

		[Field ("VSErrorInfoKeySAMLResponseStatus")]
		NSString SamlResponseStatusKey { get; }

		[Field ("VSErrorInfoKeyUnsupportedProviderIdentifier")]
		NSString UnsupportedProviderIdentifierKey { get; }

		[TV (10,1)][iOS (10,2)]
		[Field ("VSErrorInfoKeyAccountProviderResponse")]
		NSString AccountProviderResponseKey { get; }
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[NoMacCatalyst]
	[Unavailable (PlatformName.WatchOS)]
	[StrongDictionary ("VSErrorInfoKeys")]
	interface VSErrorInfo {

		string SamlResponse { get; }

		string SamlResponseStatus { get; }

		string UnsupportedProviderIdentifier { get; }

		[TV (10,1)][iOS (10,2)]
		string AccountProviderResponse { get; }
	}

	interface IVSAccountManagerDelegate { }

	[Protocol, Model]
	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountManagerDelegate {

		[Abstract]
#if NET
		[NoMac]
#elif MONOMAC
		[Obsoleted (PlatformName.MacOSX, 12,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Export ("accountManager:presentViewController:")]
		void PresentViewController (VSAccountManager accountManager, UIViewController viewController);

		[Abstract]
#if NET
		[NoMac]
#elif MONOMAC
		[Obsoleted (PlatformName.MacOSX, 12,0, message: "Unavailable on macOS, will be removed in the future.")]
#endif
		[Export ("accountManager:dismissViewController:")]
		void DismissViewController (VSAccountManager accountManager, UIViewController viewController);

		[iOS (11,0)][TV (11,0)]
		[Export ("accountManager:shouldAuthenticateAccountProviderWithIdentifier:")]
		bool ShouldAuthenticateAccountProvider (VSAccountManager accountManager, string accountProviderIdentifier);
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountManager {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IVSAccountManagerDelegate Delegate { get; set; }

		[NoMac]
		[Async]
		[Export ("checkAccessStatusWithOptions:completionHandler:")]
		void CheckAccessStatus (NSDictionary options, Action<VSAccountAccessStatus, NSError> completionHandler);

		[NoMac]
		[Async]
		[Export ("enqueueAccountMetadataRequest:completionHandler:")]
		VSAccountManagerResult Enqueue (VSAccountMetadataRequest accountMetadataRequest, Action<VSAccountMetadata, NSError> completionHandler);

		[NoMac]
		[TV (13,0)][iOS (13,0)]
		[Field ("VSOpenTVProviderSettingsURLString")]
		NSString OpenTVProviderSettingsUrl { get; }
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[Static]
	[Internal]
	[NoMacCatalyst]
	interface VSCheckAccessOptionKeys {

		[Field ("VSCheckAccessOptionPrompt")]
		NSString CheckAccessOptionPrompt { get; }
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[StrongDictionary ("VSCheckAccessOptionKeys")]
	interface VSAccountManagerAccessOptions {

		[Export ("CheckAccessOptionPrompt")]
		bool CheckAccessOptionPrompt { get; set; }
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSAccountManagerResult {

		[Export ("cancel")]
		void Cancel ();
	}

	[iOS (10, 0)]
	[TV (10, 0)]
	[Mac (10,14)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountMetadata {

		[NullAllowed, Export ("accountProviderIdentifier")]
		string AccountProviderIdentifier { get; }

		[NullAllowed, Export ("authenticationExpirationDate", ArgumentSemantic.Copy)]
		NSDate AuthenticationExpirationDate { get; }

		[NullAllowed, Export ("verificationData", ArgumentSemantic.Copy)]
		NSData VerificationData { get; }

		[NullAllowed, Export ("SAMLAttributeQueryResponse")]
		string SamlAttributeQueryResponse { get; }

		[TV (10,1)][iOS (10,2)]
		[NullAllowed, Export ("accountProviderResponse", ArgumentSemantic.Strong)]
		VSAccountProviderResponse AccountProviderResponse { get; }
	}

	[iOS (10, 0)]
	[Mac (10,14)]
	[TV (10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountMetadataRequest {

		[NullAllowed, Export ("channelIdentifier")]
		string ChannelIdentifier { get; set; }

		[Export ("supportedAccountProviderIdentifiers", ArgumentSemantic.Copy)]
		string [] SupportedAccountProviderIdentifiers { get; set; }

		[TV (11,0)][iOS (11,0)]
		[Export ("featuredAccountProviderIdentifiers", ArgumentSemantic.Copy)]
		string[] FeaturedAccountProviderIdentifiers { get; set; }

		[NullAllowed, Export ("verificationToken")]
		string VerificationToken { get; set; }

		[Export ("includeAccountProviderIdentifier")]
		bool IncludeAccountProviderIdentifier { get; set; }

		[Export ("includeAuthenticationExpirationDate")]
		bool IncludeAuthenticationExpirationDate { get; set; }

		[NullAllowed, Export ("localizedVideoTitle")]
		string LocalizedVideoTitle { get; set; }

		[Export ("interruptionAllowed")]
		bool InterruptionAllowed { [Bind ("isInterruptionAllowed")] get; set; }

		[Export ("forceAuthentication")]
		bool ForceAuthentication { get; set; }

		[Export ("attributeNames", ArgumentSemantic.Copy)]
		string [] AttributeNames { get; set; }

		[Protected]
		[TV (10,1)][iOS (10,2)]
		[Export ("supportedAuthenticationSchemes", ArgumentSemantic.Copy)]
		NSString[] SupportedAuthenticationSchemesString { get; set; }

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[NullAllowed, Export ("accountProviderAuthenticationToken")]
		string AccountProviderAuthenticationToken { get; set; }

		[TV (14,2), iOS (14,2), Mac (11,0)]
		[NullAllowed, Export ("applicationAccountProviders", ArgumentSemantic.Copy)]
		VSAccountApplicationProvider [] ApplicationAccountProviders { get; set; }
	}

	[iOS (10,2)]
	[TV (10,1)]
	[Mac (10,14)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountProviderResponse {

		[Protected]
		[Export ("authenticationScheme")]
		NSString AuthenticationSchemeString { get; }

		[Wrap ("VSAccountProviderAuthenticationSchemeExtensions.GetValue (AuthenticationSchemeString)")]
		VSAccountProviderAuthenticationScheme AuthenticationScheme { get; }

		[NullAllowed, Export ("status")]
		string Status { get; }

		[NullAllowed, Export ("body")]
		string Body { get; }
	}

	[iOS (10,2)]
	[TV (10,1)]
	[Mac (10,14)]
	[NoMacCatalyst]
	enum VSAccountProviderAuthenticationScheme {
		[Field ("VSAccountProviderAuthenticationSchemeSAML")]
		Saml,

		[iOS (13,0)][TV (13,0)][Mac (10,15)]
		[Field ("VSAccountProviderAuthenticationSchemeAPI")]
		Api,
	}

	[TV (11,0)][iOS (11,0)]
	[Mac (10,14)]
	[NoMacCatalyst]
	[Native]
	public enum VSSubscriptionAccessLevel : long {
		Unknown,
		FreeWithAccount,
		Paid,
	}

	[TV (11,0)][iOS (11,0)]
	[Mac (10,14)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSSubscription {
		[NullAllowed] // null_resettable
		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[Export ("accessLevel", ArgumentSemantic.Assign)]
		VSSubscriptionAccessLevel AccessLevel { get; set; }

		[NullAllowed] // null_resettable
		[Export ("tierIdentifiers", ArgumentSemantic.Copy)]
		string[] TierIdentifiers { get; set; }

		[TV (11,3), iOS (11,3)]
		[NullAllowed, Export ("billingIdentifier")]
		string BillingIdentifier { get; set; }
	}

	[TV (11,0)][iOS (11,0)]
	[Mac (10,14)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSSubscriptionRegistrationCenter {
		[Static]
		[Export ("defaultSubscriptionRegistrationCenter")]
		VSSubscriptionRegistrationCenter Default { get; }

		[Export ("setCurrentSubscription:")]
		void SetCurrentSubscription ([NullAllowed] VSSubscription currentSubscription);
	}

	[TV (14,2), iOS (14,2), Mac (11,0)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSAccountApplicationProvider {

		[Export ("initWithLocalizedDisplayName:identifier:")]
		NativeHandle Constructor (string localizedDisplayName, string identifier);

		[Export ("localizedDisplayName")]
		string LocalizedDisplayName { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}
}
