//
// DeviceCheck C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using ObjCRuntime;
using Foundation;

namespace DeviceCheck {

	[TV (11,0), NoWatch, NoMac, iOS (11,0)]
	[ErrorDomain ("DCErrorDomain")]
	[Native]
	public enum DCError : long {
		UnknownSystemFailure,
		FeatureUnsupported
	}

	[TV (11,0), NoWatch, NoMac, iOS (11,0)]
	delegate void DCDeviceGenerateTokenCompletionHandler ([NullAllowed] NSData token, [NullAllowed] NSError error);

	[TV (11,0), NoWatch, NoMac, iOS (11,0)]
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
}
#endif
