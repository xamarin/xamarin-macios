//
// AVRouting bindings
//
// Authors:
//	TJ Lambert  <TJ.Lambert@microsoft.com>
//
// Copyright 2022 Microsoft Corp. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;
using UniformTypeIdentifiers;
using AVKit;

#if !NET
using NativeHandle = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
#else
using OS_nw_endpoint = ObjCRuntime.NativeHandle;
#endif

namespace AVRouting {

	[Mac (13,0), iOS (16,0), MacCatalyst (16,0), NoTV, NoWatch]
	[Native]
	public enum AVCustomRoutingEventReason : long
	{
		Activate = 0,
		Deactivate,
		Reactivate,
	}

	[NoWatch, NoTV, NoMac, iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomDeviceRoute
	{
		[Internal]
		[Export ("networkEndpoint")]
		OS_nw_endpoint _NetworkEndpoint { get; }

		[NullAllowed, Export ("bluetoothIdentifier")]
		NSUuid BluetoothIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomRoutingActionItem
	{
		[Export ("type", ArgumentSemantic.Copy)]
		UTType Type { get; set; }

		[NullAllowed, Export ("overrideTitle")]
		string OverrideTitle { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomRoutingController
	{
		[Wrap ("WeakDelegate")]
		IAVCustomRoutingControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("authorizedRoutes")]
		AVCustomDeviceRoute[] AuthorizedRoutes { get; }

		[Export ("customActionItems", ArgumentSemantic.Strong)]
		AVCustomRoutingActionItem[] CustomActionItems { get; set; }

		[Export ("invalidateAuthorizationForRoute:")]
		void InvalidateAuthorization (AVCustomDeviceRoute route);

		[Export ("setActive:forRoute:")]
		void SetActive (bool active, AVCustomDeviceRoute route);

		[Export ("isRouteActive:")]
		bool IsRouteActive (AVCustomDeviceRoute route);

		[NoWatch, NoTV, NoMac, iOS (16,0), MacCatalyst (16,0)]
		[Notification, Field ("AVCustomRoutingControllerAuthorizedRoutesDidChangeNotification")]
		NSString AuthorizedRoutesDidChangeNotification { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (16,0), MacCatalyst (16,0)]
	[BaseType (typeof (NSObject))]
	interface AVCustomRoutingEvent
	{
		[Export ("reason")]
		AVCustomRoutingEventReason Reason { get; }

		[Export ("route")]
		AVCustomDeviceRoute Route { get; }
	}
}
