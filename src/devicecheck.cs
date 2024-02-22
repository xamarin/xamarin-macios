//
// DeviceCheck C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation.
//

using System;
using ObjCRuntime;
using Foundation;

namespace DeviceCheck {

	[Watch (9, 0)]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("DCErrorDomain")]
	[Native]
	public enum DCError : long {
		UnknownSystemFailure,
		FeatureUnsupported,
		InvalidInput,
		InvalidKey,
		ServerUnavailable,
	}

	[Watch (9, 0)]
	[MacCatalyst (13, 1)]
	delegate void DCDeviceGenerateTokenCompletionHandler ([NullAllowed] NSData token, [NullAllowed] NSError error);

	[Watch (9, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // From the documentation it seems the only way to create a usable instance is to use the static CurrentDevice property.
	[BaseType (typeof (NSObject))]
	interface DCDevice {

		[Static]
		[Export ("currentDevice")]
		DCDevice CurrentDevice { get; }

		[Export ("supported")]
		bool Supported { [Bind ("isSupported")] get; }

		[Async]
		[Export ("generateTokenWithCompletionHandler:")]
		void GenerateToken (DCDeviceGenerateTokenCompletionHandler completion);
	}

	[Watch (9, 0)]
	[iOS (14, 0)]
	[TV (15, 0)]
	[MacCatalyst (14, 5)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface DCAppAttestService {

		[Static]
		[Export ("sharedService")]
		DCAppAttestService SharedService { get; }

		[Export ("supported")]
		bool Supported { [Bind ("isSupported")] get; }

		[Async]
		[Export ("generateKeyWithCompletionHandler:")]
		void GenerateKey (Action<string, NSError> completionHandler);

		[Async]
		[Export ("attestKey:clientDataHash:completionHandler:")]
		void AttestKey (string keyId, NSData clientDataHash, Action<NSData, NSError> completionHandler);

		[Async]
		[Export ("generateAssertion:clientDataHash:completionHandler:")]
		void GenerateAssertion (string keyId, NSData clientDataHash, Action<NSData, NSError> completionHandler);
	}
}
