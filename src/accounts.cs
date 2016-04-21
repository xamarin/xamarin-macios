//
// Copyright 2011, 2013 Xamarin, Inc.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.Accounts {
	
	[Since (5,0)]
	[Mac (10,8, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface ACAccount : NSSecureCoding {
		[Export ("identifier", ArgumentSemantic.Weak)]
		string Identifier { get;  }

		[NullAllowed] // by default this property is null
		[Export ("accountType", ArgumentSemantic.Retain)]
		ACAccountType AccountType { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("accountDescription", ArgumentSemantic.Copy)]
		string AccountDescription { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("username", ArgumentSemantic.Copy)]
		string Username { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("credential", ArgumentSemantic.Retain)]
		ACAccountCredential Credential { get; set;  }

		[DesignatedInitializer]
		[Export ("initWithAccountType:")]
		IntPtr Constructor (ACAccountType type);

#if !XAMCORE_3_0
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[Field ("ACErrorDomain")]
		NSString ErrorDomain { get; }
#endif

#if !MONOMAC
		[Since (7,0)]
		[Export ("userFullName")]
		string UserFullName { get; }
#endif
	}

	[Since (5,0)]
	[Mac (10,8, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface ACAccountCredential : NSSecureCoding {
		[Export ("initWithOAuthToken:tokenSecret:")]
		IntPtr Constructor (string oauthToken, string tokenSecret);

		[Since(6,0)]
		[Export ("initWithOAuth2Token:refreshToken:expiryDate:")]
		IntPtr Constructor (string oauth2Token, string refreshToken, NSDate expiryDate);

		[Since(6,0)]
		[NullAllowed] // by default this property is null
		[Export ("oauthToken", ArgumentSemantic.Copy)]
		string OAuthToken { get; set;  }
	}

	public delegate void ACAccountStoreSaveCompletionHandler (bool success, NSError error);
	public delegate void ACRequestCompletionHandler (bool granted, NSError error);
	
	[Since (5,0)]
	[Mac (10,8, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface ACAccountStore {
		[Export ("accounts", ArgumentSemantic.Weak)]
		ACAccount [] Accounts { get;  }

		[Export ("accountWithIdentifier:")]
		ACAccount FindAccount (string identifier);

		[Export ("accountTypeWithAccountTypeIdentifier:")]
		ACAccountType FindAccountType (string typeIdentifier);

		[Export ("accountsWithAccountType:")]
		ACAccount [] FindAccounts (ACAccountType accountType);

		[Export ("saveAccount:withCompletionHandler:")]
		[Async]
		void SaveAccount (ACAccount account, ACAccountStoreSaveCompletionHandler completionHandler);

		[Export ("requestAccessToAccountsWithType:withCompletionHandler:")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use RequestAccewss (ACAccountType, AccountStoreOptions, ACRequestCompletionHandler) instead")]
		[Async]
		void RequestAccess (ACAccountType accountType, ACRequestCompletionHandler completionHandler);

		[Field ("ACAccountStoreDidChangeNotification")]
		[Notification]
		NSString ChangeNotification { get; }
		
		[Since(6,0)]
		[Export ("renewCredentialsForAccount:completion:")]
		[Async]
		void RenewCredentials (ACAccount account, Action<ACAccountCredentialRenewResult,NSError> completionHandler);

		[Since(6,0)]
		[Protected]
		[Export ("requestAccessToAccountsWithType:options:completion:")]
		[Async]
		void RequestAccess (ACAccountType accountType, [NullAllowed] NSDictionary options, ACRequestCompletionHandler completion);

		[Since(6,0)]
		[Wrap ("RequestAccess (accountType, options == null ? null : options.Dictionary, completion)")]
		[Async]
		void RequestAccess (ACAccountType accountType, [NullAllowed] AccountStoreOptions options, ACRequestCompletionHandler completion);
	}

	[Since (5,0)]
	[Mac (10,8, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface ACAccountType : NSSecureCoding {
		[Export ("accountTypeDescription")]
		string Description { get;  }

		[Export ("identifier")]
		string Identifier { get;  }

		[Export ("accessGranted")]
		bool AccessGranted { get;  }

		[Field ("ACAccountTypeIdentifierTwitter")]
		NSString Twitter { get; }

		[Since (6,0)]
		[Field ("ACAccountTypeIdentifierSinaWeibo")]
		NSString SinaWeibo { get; }

		[Since (6,0)]
		[Field ("ACAccountTypeIdentifierFacebook")]
		NSString Facebook { get; }

		[Since (7,0)]
		[Mac (10,9)]
		[Field ("ACAccountTypeIdentifierTencentWeibo")]
		NSString TencentWeibo { get; }

#if MONOMAC
		[Mac (10,9)]
		[Field ("ACAccountTypeIdentifierLinkedIn")]
		NSString LinkedIn { get; }
#endif
	}

	[Since (6,0)]
	[Mac (10,8, onlyOn64 : true)]
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

	[Since (6,0)]
	[Mac (10,8, onlyOn64 : true)]
	[Static]
	interface ACFacebookAudienceValue
	{	
		[Field ("ACFacebookAudienceEveryone")]
		NSString Everyone { get; }

		[Field ("ACFacebookAudienceFriends")]
		NSString Friends { get; }

		[Field ("ACFacebookAudienceOnlyMe")]
		NSString OnlyMe { get; }
	}

	[Since (7,0)]
	[Mac (10,9, onlyOn64 : true)]
	[Static]
	interface ACTencentWeiboKey {
		[Field ("ACTencentWeiboAppIdKey")]
		NSString AppId { get; }
	}

#if MONOMAC
	[Mac (10,9, onlyOn64 : true)]
	[Static]
	interface ACLinkedInKey {
		[Field ("ACLinkedInAppIdKey")]
		NSString AppId { get; }

		[Field ("ACLinkedInPermissionsKey")]
		NSString Permissions { get; }
	}
#endif
}
#endif
