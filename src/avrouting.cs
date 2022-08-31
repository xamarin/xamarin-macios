//
// AVRouting bindings
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

using System;
using System.ComponentModel;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AVRouting {
	[NoWatch, NoTV, NoMac, iOS (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomDeviceRoute
	{
		// @property (readonly, nonatomic) API_AVAILABLE(ios(16.0)) nw_endpoint_t networkEndpoint __attribute__((availability(ios, introduced=16.0))) __attribute__((availability(macos, unavailable))) __attribute__((availability(tvos, unavailable))) __attribute__((availability(watchos, unavailable)));
		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("networkEndpoint")]
		OS_nw_endpoint NetworkEndpoint { get; }

		// @property (readonly, nonatomic) API_AVAILABLE(ios(16.0)) NSUUID * bluetoothIdentifier __attribute__((availability(ios, introduced=16.0))) __attribute__((availability(macos, unavailable))) __attribute__((availability(tvos, unavailable))) __attribute__((availability(watchos, unavailable)));
		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("bluetoothIdentifier")]
		NSUuid BluetoothIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomRoutingActionItem
	{
		// @property (copy, nonatomic) API_AVAILABLE(ios(16.0)) UTType * type __attribute__((availability(ios, introduced=16.0))) __attribute__((availability(macos, unavailable))) __attribute__((availability(tvos, unavailable))) __attribute__((availability(watchos, unavailable)));
		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("type", ArgumentSemantic.Copy)]
		UTType Type { get; set; }

		// @property (copy, nonatomic) API_AVAILABLE(ios(16.0)) NSString * overrideTitle __attribute__((availability(ios, introduced=16.0))) __attribute__((availability(macos, unavailable))) __attribute__((availability(tvos, unavailable))) __attribute__((availability(watchos, unavailable)));
		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("overrideTitle")]
		string OverrideTitle { get; set; }
	}
}
