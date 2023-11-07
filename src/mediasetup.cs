using System;
using Foundation;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MediaSetup {

	[NoTV]
	[NoWatch]
	[NoMac]
	[iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MSServiceAccount {

		[Export ("initWithServiceName:accountName:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string serviceName, string accountName);

		[Export ("serviceName")]
		string ServiceName { get; }

		[Export ("accountName")]
		string AccountName { get; }

		[NullAllowed, Export ("clientID")]
		string ClientId { get; set; }

		[NullAllowed, Export ("clientSecret")]
		string ClientSecret { get; set; }

		[NullAllowed, Export ("configurationURL", ArgumentSemantic.Copy)]
		NSUrl ConfigurationUrl { get; set; }

		[NullAllowed, Export ("authorizationTokenURL", ArgumentSemantic.Copy)]
		NSUrl AuthorizationTokenUrl { get; set; }

		[NullAllowed, Export ("authorizationScope")]
		string AuthorizationScope { get; set; }
	}

	interface IMSAuthenticationPresentationContext { }

	[NoTV]
	[NoWatch]
	[NoMac]
	[iOS (14, 0)]
	[Protocol]
	interface MSAuthenticationPresentationContext {

		[Abstract]
		[NullAllowed, Export ("presentationAnchor")]
		UIWindow PresentationAnchor { get; }
	}

	[NoTV]
	[NoWatch]
	[NoMac]
	[iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MSSetupSession {

		[Export ("initWithServiceAccount:")]
		NativeHandle Constructor (MSServiceAccount serviceAccount);

		[Export ("startWithError:")]
		bool Start ([NullAllowed] out NSError error);

		[NullAllowed, Export ("presentationContext", ArgumentSemantic.Weak)]
		IMSAuthenticationPresentationContext PresentationContext { get; set; }

		[Export ("account", ArgumentSemantic.Strong)]
		MSServiceAccount Account { get; }
	}
}
