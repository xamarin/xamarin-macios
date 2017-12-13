//
// AdSupport bindings.
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using XamCore.ObjCRuntime;
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.Foundation;
using System;

namespace XamCore.AdSupport {

	[Since (6,0)]
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
	}
}