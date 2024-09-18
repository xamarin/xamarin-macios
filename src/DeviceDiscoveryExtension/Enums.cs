//
// Enums.cs
//
// Authors:
//   Israel Soto (issoto@microsoft.com)
//   Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2022, 2024 Microsoft Corporation.
//

using System;

using ObjCRuntime;
using Foundation;

namespace DeviceDiscoveryExtension {

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	public enum DDDeviceProtocol : long {
		Invalid = 0,
		Dial = 1,
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	public enum DDDeviceCategory : long {
		HiFiSpeaker = 0,
		HiFiSpeakerMultiple = 1,
		TvWithMediaBox = 2,
		Tv = 3,
		LaptopComputer = 4,
		DesktopComputer = 5,
		[iOS (18, 0)]
		AccessorySetup = 6,
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	public enum DDDeviceState : long {
		Invalid = 0,
		Activating = 10,
		Activated = 20,
		Authorized = 25,
		Invalidating = 30,
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	public enum DDDeviceMediaPlaybackState : long {
		NoContent = 0,
		Paused = 1,
		Playing = 2,
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[ErrorDomain ("DDErrorDomain")]
	[Native]
	public enum DDErrorCode : long {
		Success = 0,
		Unknown = 350000,
		BadParameter = 350001,
		Unsupported = 350002,
		Timeout = 350003,
		Internal = 350004,
		MissingEntitlement = 350005,
		Permission = 350006,
		// Next, // don't bind 'Next', it's not static and we'd have to break compat if it were ever to change
	}

	[Mac (15, 0), iOS (16, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	public enum DDEventType : long {
		Unknown = 0,
		DeviceFound = 40,
		DeviceLost = 41,
		DeviceChanged = 42,
	}

	[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0), NoWatch, NoTV]
	[Native]
	[Flags]
	public enum DDDeviceSupports : ulong {
		BluetoothPairingLE = 1 << 1,
		BluetoothTransportBridging = 1 << 2,
	}
}
