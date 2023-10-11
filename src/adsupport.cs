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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASIdentifierManager {

		[Export ("sharedManager")]
		[Static]
		ASIdentifierManager SharedManager { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Export ("advertisingTrackingEnabled")]
		bool IsAdvertisingTrackingEnabled { [Bind ("isAdvertisingTrackingEnabled")] get; }

		[Export ("advertisingIdentifier")]
		NSUuid AdvertisingIdentifier { get; }

		[NoTV]
		[NoiOS]
		[NoMac] // unclear when that was changed (xcode 12 GM allowed it)
		[NoMacCatalyst]
		[Export ("clearAdvertisingIdentifier")]
		void ClearAdvertisingIdentifier ();
	}
}
