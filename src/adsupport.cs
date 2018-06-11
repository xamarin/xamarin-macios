//
// AdSupport bindings.
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using ObjCRuntime;
using Foundation;
using System;

namespace AdSupport {

	[iOS (6,0)][Mac (10,14)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASIdentifierManager {

		[Export ("sharedManager")]
		[Static]
		ASIdentifierManager SharedManager { get; }

		[Export ("advertisingTrackingEnabled")]
		bool IsAdvertisingTrackingEnabled { [Bind ("isAdvertisingTrackingEnabled")] get; }

		[Export ("advertisingIdentifier")]
		NSUuid AdvertisingIdentifier { get; }

		[NoTV][NoiOS]
		[Export ("clearAdvertisingIdentifier")]
		void ClearAdvertisingIdentifier ();
	}
}