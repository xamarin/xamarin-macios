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
using System.ComponentModel;
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

	/// <summary>Encapsulates errors that may occur during attempts to verify credentials.</summary>
	[Native]
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

	/// <summary>Enumerates the types of access the app has to the user's subscription.</summary>
	[Native]
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	public enum VSAccountAccessStatus : long {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Granted = 3
	}

#if !XAMCORE_5_0
	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("Use 'VSUserAccountQueryOptions' instead.")]
	[TV (16, 0), Mac (13, 0), iOS (16, 0), NoMacCatalyst]
	[Native]
	enum VSUserAccountQueryOption : ulong {
		None = 0,
		AllDevices,
	}
#endif // !XAMCORE_5_0

	[TV (16, 0), NoMacCatalyst, iOS (16, 0), Mac (13, 0)]
	[Flags]
	[Native]
	public enum VSUserAccountQueryOptions : long {
		None = 0x0,
		AllDevices,
	}

	[TV (16, 0), NoMacCatalyst, iOS (16, 0), Mac (13, 0)]
	[Native]
	public enum VSUserAccountType : long {
		Free,
		Paid,
	}

	[TV (16, 0), NoMacCatalyst, iOS (16, 0), Mac (13, 0)]
	[Native]
	public enum VSOriginatingDeviceCategory : long {
		Mobile,
		Other,
	}


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

		[Field ("VSErrorInfoKeyAccountProviderResponse")]
		NSString AccountProviderResponseKey { get; }
	}

	[NoMacCatalyst]
	[Unavailable (PlatformName.WatchOS)]
	[StrongDictionary ("VSErrorInfoKeys")]
	interface VSErrorInfo {

		string SamlResponse { get; }

		string SamlResponseStatus { get; }

		string UnsupportedProviderIdentifier { get; }

		string AccountProviderResponse { get; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:VideoSubscriberAccount.VSAccountManagerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:VideoSubscriberAccount.VSAccountManagerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:VideoSubscriberAccount.VSAccountManagerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:VideoSubscriberAccount.VSAccountManagerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IVSAccountManagerDelegate { }

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/VideoSubscriberAccount/VSAccountManagerDelegate">Apple documentation for <c>VSAccountManagerDelegate</c></related>
	[Protocol, Model]
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

		[Export ("accountManager:shouldAuthenticateAccountProviderWithIdentifier:")]
		bool ShouldAuthenticateAccountProvider (VSAccountManager accountManager, string accountProviderIdentifier);
	}

	/// <summary>Coordinates access to the user's subscription.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/VideoSubscriberAccount/VSAccountManager">Apple documentation for <c>VSAccountManager</c></related>
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
		[TV (13, 0)]
		[iOS (13, 0)]
		[Field ("VSOpenTVProviderSettingsURLString")]
		NSString OpenTVProviderSettingsUrl { get; }
	}

	[Unavailable (PlatformName.WatchOS)]
	[Static]
	[Internal]
	[NoMacCatalyst]
	interface VSCheckAccessOptionKeys {

		[Field ("VSCheckAccessOptionPrompt")]
		NSString CheckAccessOptionPrompt { get; }
	}

	/// <summary>A <see cref="T:Foundation.DictionaryContainer" /> holding keys appropriate to <see cref="M:VideoSubscriberAccount.VSAccountManager.CheckAccessStatusAsync(VideoSubscriberAccount.VSAccountManagerAccessOptions)" /> and <see cref="M:VideoSubscriberAccount.VSAccountManager.CheckAccessStatusAsync(VideoSubscriberAccount.VSAccountManagerAccessOptions)" />.</summary>
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[StrongDictionary ("VSCheckAccessOptionKeys")]
	interface VSAccountManagerAccessOptions {

		[Export ("CheckAccessOptionPrompt")]
		bool CheckAccessOptionPrompt { get; set; }
	}

	/// <summary>Represents a cancellable request that is still "in flight".</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/VideoSubscriberAccount/VSAccountManagerResult">Apple documentation for <c>VSAccountManagerResult</c></related>
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSAccountManagerResult {

		[Export ("cancel")]
		void Cancel ();
	}

	/// <summary>Information about a subscription.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/VideoSubscriberAccount/VSAccountMetadata">Apple documentation for <c>VSAccountMetadata</c></related>
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

		[NullAllowed, Export ("accountProviderResponse", ArgumentSemantic.Strong)]
		VSAccountProviderResponse AccountProviderResponse { get; }
	}

	/// <summary>Specifies information being requested from the subscriber's account.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/VideoSubscriberAccount/VSAccountMetadataRequest">Apple documentation for <c>VSAccountMetadataRequest</c></related>
	[Unavailable (PlatformName.WatchOS)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface VSAccountMetadataRequest {

		[NullAllowed, Export ("channelIdentifier")]
		string ChannelIdentifier { get; set; }

		[Export ("supportedAccountProviderIdentifiers", ArgumentSemantic.Copy)]
		string [] SupportedAccountProviderIdentifiers { get; set; }

		[Export ("featuredAccountProviderIdentifiers", ArgumentSemantic.Copy)]
		string [] FeaturedAccountProviderIdentifiers { get; set; }

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
		[Export ("supportedAuthenticationSchemes", ArgumentSemantic.Copy)]
		NSString [] SupportedAuthenticationSchemesString { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[NullAllowed, Export ("accountProviderAuthenticationToken")]
		string AccountProviderAuthenticationToken { get; set; }

		[TV (14, 2), iOS (14, 2)]
		[NullAllowed, Export ("applicationAccountProviders", ArgumentSemantic.Copy)]
		VSAccountApplicationProvider [] ApplicationAccountProviders { get; set; }
	}

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

	[NoMacCatalyst]
	enum VSAccountProviderAuthenticationScheme {
		[Field ("VSAccountProviderAuthenticationSchemeSAML")]
		Saml,

		[iOS (13, 0)]
		[TV (13, 0)]
		[Field ("VSAccountProviderAuthenticationSchemeAPI")]
		Api,
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'VSUserAccountType' enum instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use the 'VSUserAccountType' enum instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'VSUserAccount' type instead.")]
	[NoMacCatalyst]
	[Native]
	public enum VSSubscriptionAccessLevel : long {
		Unknown,
		FreeWithAccount,
		Paid,
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'VSUserAccount' type instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use the 'VSUserAccount' type instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'VSUserAccount' type instead.")]
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
		string [] TierIdentifiers { get; set; }

		[NullAllowed, Export ("billingIdentifier")]
		string BillingIdentifier { get; set; }
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'VSUserAccountManager' type instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use the 'VSUserAccountManager' type instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'VSUserAccount' type instead.")]
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

	[TV (14, 2), iOS (14, 2)]
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

	[TV (16, 0), NoMacCatalyst, iOS (16, 0), Mac (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSUserAccountManager {
		[Static]
		[Export ("sharedUserAccountManager")]
		VSUserAccountManager SharedUserAccountManager { get; }

		[Async]
		[Export ("updateUserAccount:completion:")]
		void UpdateUserAccount (VSUserAccount account, [NullAllowed] Action<NSError> completion);

		[Async]
		[Export ("queryUserAccountsWithOptions:completion:")]
		void QueryUserAccounts (VSUserAccountQueryOptions options, Action<NSArray<VSUserAccount>, NSError> completion);
	}

	[TV (16, 0), NoMacCatalyst, iOS (16, 0), Mac (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSUserAccount {
		[NullAllowed, Export ("updateURL", ArgumentSemantic.Copy)]
		NSUrl UpdateUrl { get; set; }

		[Export ("requiresSystemTrust")]
		bool RequiresSystemTrust { get; set; }

		[NullAllowed, Export ("accountProviderIdentifier", ArgumentSemantic.Strong)]
		string AccountProviderIdentifier { get; set; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; set; }

		[Export ("accountType", ArgumentSemantic.Assign)]
		VSUserAccountType AccountType { get; set; }

		[Obsoleted (PlatformName.iOS, 16, 4, message: Constants.ApiRemovedGeneral)]
		[Obsoleted (PlatformName.TvOS, 16, 4, message: Constants.ApiRemovedGeneral)]
		[Obsoleted (PlatformName.MacOSX, 13, 3, message: Constants.ApiRemovedGeneral)]
		[Export ("deleted")]
		bool Deleted { [Bind ("isDeleted")] get; set; }

		[TV (16, 4), NoMacCatalyst, iOS (16, 4), Mac (13, 3)]
		[Export ("signedOut")]
		bool SignedOut { [Bind ("isSignedOut")] get; set; }

		[NullAllowed, Export ("subscriptionBillingCycleEndDate", ArgumentSemantic.Copy)]
		NSDate SubscriptionBillingCycleEndDate { get; set; }

		[NullAllowed, Export ("tierIdentifiers", ArgumentSemantic.Copy)]
		string [] TierIdentifiers { get; set; }

		[NullAllowed, Export ("billingIdentifier")]
		string BillingIdentifier { get; set; }

		[NullAllowed, Export ("authenticationData")]
		string AuthenticationData { get; set; }

		[Export ("fromCurrentDevice")]
		bool FromCurrentDevice { [Bind ("isFromCurrentDevice")] get; }

		[Export ("deviceCategory")]
		VSOriginatingDeviceCategory DeviceCategory { get; }

		[TV (17, 4), NoWatch, NoMacCatalyst, Mac (14, 4), iOS (17, 4)]
		[NullAllowed, Export ("appleSubscription", ArgumentSemantic.Strong)]
		VSAppleSubscription AppleSubscription { get; set; }

		[Export ("initWithAccountType:updateURL:")]
		NativeHandle Constructor (VSUserAccountType accountType, [NullAllowed] NSUrl url);
	}

	[TV (17, 4), NoWatch, NoMacCatalyst, Mac (14, 4), iOS (17, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface VSAppleSubscription {

		[Export ("customerID", ArgumentSemantic.Strong)]
		string CustomerId { get; set; }

		[Export ("productCodes", ArgumentSemantic.Strong)]
		string [] ProductCodes { get; set; }

		[Export ("initWithCustomerID:productCodes:")]
		NativeHandle Constructor (string customerId, string [] productCodes);
	}

}
