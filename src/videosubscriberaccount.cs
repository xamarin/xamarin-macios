//
// VideoSubscriberAccount bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.VideoSubscriberAccount {

	[Native]
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[ErrorDomain ("VSErrorDomain")]
	public enum VSErrorCode : nint {
		AccessNotGranted = 0,
		UnsupportedProvider = 1,
		UserCancelled = 2,
		ServiceTemporarilyUnavailable = 3,
		ProviderRejected = 4,
		InvalidVerificationToken = 5
	}

	[Native]
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	public enum VSAccountAccessStatus : nint {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Granted = 3
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
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
	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface VSAccountManagerDelegate {

		[Abstract]
		[Export ("accountManager:presentViewController:")]
		void PresentViewController (VSAccountManager accountManager, UIViewController viewController);

		[Abstract]
		[Export ("accountManager:dismissViewController:")]
		void DismissViewController (VSAccountManager accountManager, UIViewController viewController);

		[iOS (11,0)][TV (11,0)]
		[Export ("accountManager:shouldAuthenticateAccountProviderWithIdentifier:")]
		bool ShouldAuthenticateAccountProvider (VSAccountManager accountManager, string accountProviderIdentifier);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	interface VSAccountManager {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IVSAccountManagerDelegate Delegate { get; set; }

		[Async]
		[Export ("checkAccessStatusWithOptions:completionHandler:")]
		void CheckAccessStatus (NSDictionary options, Action<VSAccountAccessStatus, NSError> completionHandler);

		[Async]
		[Export ("enqueueAccountMetadataRequest:completionHandler:")]
		VSAccountManagerResult Enqueue (VSAccountMetadataRequest accountMetadataRequest, Action<VSAccountMetadata, NSError> completionHandler);
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[Static]
	[Internal]
	interface VSCheckAccessOptionKeys {

		[Field ("VSCheckAccessOptionPrompt")]
		NSString CheckAccessOptionPrompt { get; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[StrongDictionary ("VSCheckAccessOptionKeys")]
	interface VSAccountManagerAccessOptions {

		[Export ("CheckAccessOptionPrompt")]
		bool CheckAccessOptionPrompt { get; set; }
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSAccountManagerResult {

		[Export ("cancel")]
		void Cancel ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
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

	[Introduced (PlatformName.iOS, 10, 0)]
	[Introduced (PlatformName.TvOS, 10, 0)]
	[Unavailable (PlatformName.WatchOS)]
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
	}

	[iOS (10,2)]
	[TV (10,1)]
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
	enum VSAccountProviderAuthenticationScheme {
		[Field ("VSAccountProviderAuthenticationSchemeSAML")]
		Saml,
	}

	[TV (11,0)][iOS (11,0)]
	[Native]
	public enum VSSubscriptionAccessLevel : nint {
		Unknown,
		FreeWithAccount,
		Paid,
	}

	[TV (11,0)][iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface VSSubscription {
		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[Export ("accessLevel", ArgumentSemantic.Assign)]
		VSSubscriptionAccessLevel AccessLevel { get; set; }

		[Export ("tierIdentifiers", ArgumentSemantic.Copy)]
		string[] TierIdentifiers { get; set; }

		[TV (11,3), iOS (11,3)]
		[NullAllowed, Export ("billingIdentifier")]
		string BillingIdentifier { get; set; }
	}

	[TV (11,0)][iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSSubscriptionRegistrationCenter {
		[Static]
		[Export ("defaultSubscriptionRegistrationCenter")]
		VSSubscriptionRegistrationCenter Default { get; }

		[Export ("setCurrentSubscription:")]
		void SetCurrentSubscription ([NullAllowed] VSSubscription currentSubscription);
	}
}

