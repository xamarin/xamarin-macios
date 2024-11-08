//
// Copyright 2011, 2013 Xamarin, Inc.
//

using System;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Accounts {

	/// <include file="../docs/api/Accounts/ACAccount.xml" path="/Documentation/Docs[@DocId='T:Accounts.ACAccount']/*" />
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[BaseType (typeof (NSObject))]
	interface ACAccount : NSSecureCoding {
		[Export ("identifier", ArgumentSemantic.Weak)]
		string Identifier { get; }

		[NullAllowed] // by default this property is null
		[Export ("accountType", ArgumentSemantic.Retain)]
		ACAccountType AccountType { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accountDescription", ArgumentSemantic.Copy)]
		string AccountDescription { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("username", ArgumentSemantic.Copy)]
		string Username { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("credential", ArgumentSemantic.Retain)]
		ACAccountCredential Credential { get; set; }

		[DesignatedInitializer]
		[Export ("initWithAccountType:")]
		NativeHandle Constructor (ACAccountType type);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("userFullName")]
		string UserFullName { get; }
	}

	/// <summary>Encapsulates information needed to authenticate a user.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Accounts/Reference/ACAccountCredentialClassRef/index.html">Apple documentation for <c>ACAccountCredential</c></related>
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[BaseType (typeof (NSObject))]
	interface ACAccountCredential : NSSecureCoding {
		[Export ("initWithOAuthToken:tokenSecret:")]
		NativeHandle Constructor (string oauthToken, string tokenSecret);

		[Export ("initWithOAuth2Token:refreshToken:expiryDate:")]
		NativeHandle Constructor (string oauth2Token, string refreshToken, NSDate expiryDate);

		[NullAllowed] // by default this property is null
		[Export ("oauthToken", ArgumentSemantic.Copy)]
		string OAuthToken { get; set; }
	}

	/// <summary>A delegate that specifies the completion handler in calls to the <see cref="M:Accounts.ACAccountStore.SaveAccount(Accounts.ACAccount,Accounts.ACAccountStoreSaveCompletionHandler)" /> method.</summary>
	delegate void ACAccountStoreSaveCompletionHandler (bool success, NSError error);
	/// <param name="success">
	///       <see langword="true" /> if the account was removed. Otherwise, <see langword="false" />.</param>
	///     <param name="error">The error that was encountered, or <see langword="null" /> if no error was encountered.</param>
	///     <summary>A handler to be run when an attempt is made to remove an account from the store.</summary>
	delegate void ACAccountStoreRemoveCompletionHandler (bool success, NSError error);
	/// <summary>A delegate that specifies the handler executed at the completion of calls to <see cref="M:Accounts.ACAccountStore.RequestAccessAsync(Accounts.ACAccountType,Foundation.NSDictionary)" />s.</summary>
	delegate void ACRequestCompletionHandler (bool granted, NSError error);

	/// <include file="../docs/api/Accounts/ACAccountStore.xml" path="/Documentation/Docs[@DocId='T:Accounts.ACAccountStore']/*" />
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[BaseType (typeof (NSObject))]
	interface ACAccountStore {
		[Export ("accounts", ArgumentSemantic.Weak)]
		ACAccount [] Accounts { get; }

		[Export ("accountWithIdentifier:")]
		ACAccount FindAccount (string identifier);

		[Export ("accountTypeWithAccountTypeIdentifier:")]
		ACAccountType FindAccountType (string typeIdentifier);

		[Export ("accountsWithAccountType:")]
		ACAccount [] FindAccounts (ACAccountType accountType);

		[Export ("saveAccount:withCompletionHandler:")]
		[Async]
		void SaveAccount (ACAccount account, ACAccountStoreSaveCompletionHandler completionHandler);

#if NET
		[NoMac] // marked as unavailable in xcode10 beta 2
#endif
		[Export ("requestAccessToAccountsWithType:withCompletionHandler:")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'RequestAccess (ACAccountType, AccountStoreOptions, ACRequestCompletionHandler)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequestAccess (ACAccountType, AccountStoreOptions, ACRequestCompletionHandler)' instead.")]
		[Async]
		void RequestAccess (ACAccountType accountType, ACRequestCompletionHandler completionHandler);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacOSX, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Field ("ACAccountStoreDidChangeNotification")]
		[Notification]
		NSString ChangeNotification { get; }

		[Export ("renewCredentialsForAccount:completion:")]
		[Async]
		void RenewCredentials (ACAccount account, Action<ACAccountCredentialRenewResult, NSError> completionHandler);

		[Protected]
		[Export ("requestAccessToAccountsWithType:options:completion:")]
		[Async]
		void RequestAccess (ACAccountType accountType, [NullAllowed] NSDictionary options, ACRequestCompletionHandler completion);

		[Wrap ("RequestAccess (accountType, options.GetDictionary (), completion)")]
		[Async]
		void RequestAccess (ACAccountType accountType, [NullAllowed] AccountStoreOptions options, ACRequestCompletionHandler completion);

		[Export ("removeAccount:withCompletionHandler:")]
		[Async]
		void RemoveAccount (ACAccount account, ACAccountStoreRemoveCompletionHandler completionHandler);
	}

	/// <summary>A class that contains information about <see cref="T:Accounts.ACAccount" />s of a particular type.</summary>
	///     <remarks>
	///       <para>Application developers do not instantiate <see cref="T:Accounts.ACAccountType" /> directly. Rather, they can retrieve an appropriate object with the <see cref="M:Accounts.ACAccountStore.FindAccountType(System.String)" /> method.
	/// 	</para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// 		ACAccountStore objStore = new ACAccountStore();
	/// 		objStore.RequestAccess(objStore.FindAccountType(ACAccountType.Twitter), new AccountStoreOptions(), (granted, error) => { });
	/// 	]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Accounts/Reference/ACAccountTypeClassRef/index.html">Apple documentation for <c>ACAccountType</c></related>
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the non-Apple SDK relating to your account type instead.")]
	[BaseType (typeof (NSObject))]
	interface ACAccountType : NSSecureCoding {
		[Export ("accountTypeDescription")]
		string Description { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("accessGranted")]
		bool AccessGranted { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Twitter SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Twitter SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Twitter SDK instead.")]
		[Field ("ACAccountTypeIdentifierTwitter")]
		NSString Twitter { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Sina Weibo SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Sina Weibo SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Sina Weibo SDK instead.")]
		[Field ("ACAccountTypeIdentifierSinaWeibo")]
		NSString SinaWeibo { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Facebook SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Facebook SDK instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Facebook SDK instead.")]
		[Field ("ACAccountTypeIdentifierFacebook")]
		NSString Facebook { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use Tencent Weibo SDK instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Tencent Weibo SDK instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Tencent Weibo SDK instead.")]
		[Field ("ACAccountTypeIdentifierTencentWeibo")]
		NSString TencentWeibo { get; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use LinkedIn SDK instead.")]
		[NoMacCatalyst]
		[Field ("ACAccountTypeIdentifierLinkedIn")]
		NSString LinkedIn { get; }
	}

	/// <summary>A class that encapsulates keys necessary for Facebook requests. Used with <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)&amp;scope=Xamarin" title="M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)">M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)</a></format>.</summary>
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use Facebook SDK instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Facebook SDK instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Facebook SDK instead.")]
	[Static]
	interface ACFacebookKey {
		[Field ("ACFacebookAppIdKey")]
		NSString AppId { get; }

		[Field ("ACFacebookPermissionsKey")]
		NSString Permissions { get; }

		// FIXME: does not exists in OSX 10.8 - which breaks our custom, higher level API for permissions
		[Field ("ACFacebookAudienceKey")]
		NSString Audience { get; }
	}

	/// <summary>An enumeration whose values specify the visibility of a post to Facebook.</summary>
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use Facebook SDK instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Facebook SDK instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Facebook SDK instead.")]
	[Static]
	interface ACFacebookAudienceValue {
		[Field ("ACFacebookAudienceEveryone")]
		NSString Everyone { get; }

		[Field ("ACFacebookAudienceFriends")]
		NSString Friends { get; }

		[Field ("ACFacebookAudienceOnlyMe")]
		NSString OnlyMe { get; }
	}

	/// <summary>Key to use when accessing Tencent Weibo accounts. Used with <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)&amp;scope=Xamarin" title="M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)">M:Accounts.ACAccountStore.RequestAccess (Accounts.ACAccountType,Accounts.AccountStoreOptions,Accounts.ACRequestCompletionHandler)</a></format>.</summary>
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use Tencent Weibo SDK instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use Tencent Weibo SDK instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Tencent Weibo SDK instead.")]
	[Static]
	interface ACTencentWeiboKey {
		[Field ("ACTencentWeiboAppIdKey")]
		NSString AppId { get; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use LinkedIn SDK instead.")]
	[NoMacCatalyst]
	[Static]
	interface ACLinkedInKey {
		[Field ("ACLinkedInAppIdKey")]
		NSString AppId { get; }

		[Field ("ACLinkedInPermissionsKey")]
		NSString Permissions { get; }
	}
}
