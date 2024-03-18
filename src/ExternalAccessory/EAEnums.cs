//
// EAEnums.cs: API definition for ExternalAccessory binding
//
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//
using System;
using Foundation;
using ObjCRuntime;

namespace ExternalAccessory {

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum EAWiFiUnconfiguredAccessoryProperties : ulong {
		SupportsAirPlay = (1 << 0),
		SupportsAirPrint = (1 << 1),
		SupportsHomeKit = (1 << 2), // iOS 8 beta 5
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum EAWiFiUnconfiguredAccessoryBrowserState : long {
		WiFiUnavailable = 0,
		Stopped,
		Searching,
		Configuring,
	}

	// NSInteger -> EAWiFiUnconfiguredAccessoryBrowser.h
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum EAWiFiUnconfiguredAccessoryConfigurationStatus : long {
		Success,
		UserCancelledConfiguration,
		Failed,
	}

	// NSInteger -> EAAccessoryManager.h
	[MacCatalyst (13, 1)]
	[Native ("EABluetoothAccessoryPickerErrorCode")]
	[ErrorDomain ("EABluetoothAccessoryPickerErrorDomain")]
	public enum EABluetoothAccessoryPickerError : long {
		AlreadyConnected,
		NotFound,
		Cancelled,
		Failed
	}
}
