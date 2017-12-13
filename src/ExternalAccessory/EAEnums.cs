//
// EAEnums.cs: API definition for ExternalAccessory binding
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.ExternalAccessory {

	[NoMac]
	[TV (10,0)]
	[iOS (8,0)]
	[Native]
	[Flags]
	public enum EAWiFiUnconfiguredAccessoryProperties : nuint {
		SupportsAirPlay  = (1 << 0),
		SupportsAirPrint = (1 << 1),
		SupportsHomeKit  = (1 << 2), // iOS 8 beta 5
	}

	[NoMac]
	[TV (10,0)]
	[iOS (8,0)]
	[Native]
	public enum EAWiFiUnconfiguredAccessoryBrowserState : nint {
		WiFiUnavailable = 0,
		Stopped,
		Searching,
		Configuring,
	}

	// NSInteger -> EAWiFiUnconfiguredAccessoryBrowser.h
	[NoMac]
	[TV (10,0)]
	[iOS (8,0)]
	[Native]
	public enum EAWiFiUnconfiguredAccessoryConfigurationStatus : nint {
		Success,
		UserCancelledConfiguration,
		Failed,
	}

	// NSInteger -> EAAccessoryManager.h
	[TV (10,0)]
	[iOS (6,0)]
	[Mac (10, 13)]
	[Native]
	[ErrorDomain ("EABluetoothAccessoryPickerErrorDomain")]
	public enum EABluetoothAccessoryPickerError : nint {
		AlreadyConnected,
		NotFound,
		Cancelled,
		Failed
	}
}
