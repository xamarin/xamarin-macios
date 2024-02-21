// Copyright 2014 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using System;

namespace CoreWlan {

	[NoMacCatalyst]
	[Native]
	[ErrorDomain ("CWErrorDomain")] // enum named `CWErr` in headers
	public enum CWStatus : long {
		Ok = 0,
		EAPOL = 1,
		InvalidParameter = -3900,
		NoMemory = -3901,
		Unknown = -3902,
		NotSupported = -3903,
		InvalidFormat = -3904,
		Timeout = -3905,
		UnspecifiedFailure = -3906,
		UnsupportedCapabilities = -3907,
		ReassociationDenied = -3908,
		AssociationDenied = -3909,
		AuthenticationAlgorithmUnsupported = -3910,
		InvalidAuthenticationSequenceNumber = -3911,
		ChallengeFailure = -3912,
		APFull = -3913,
		UnsupportedRateSet = -3914,
		ShortSlotUnsupported = -3915,
		DSSSOFDMUnsupported = -3916,
		InvalidInformationElement = -3917,
		InvalidGroupCipher = -3918,
		InvalidPairwiseCipher = -3919,
		InvalidAKMP = -3920,
		UnsupportedRSNVersion = -3921,
		InvalidRSNCapabilities = -3922,
		CipherSuiteRejected = -3923,
		InvalidPMK = -3924,
		SupplicantTimeout = -3925,
		HTFeaturesNotSupported = -3926,
		PCOTransitionTimeNotSupported = -3927,
		ReferenceNotBound = -3928,
		IPCFailure = -3929,
		OperationNotPermitted = -3930,
		Status = -3931,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWPhyMode : ulong {
		None = 0,
		A = 1,
		B = 2,
		G = 3,
		N = 4,
		AC = 5,
		AX = 6,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWInterfaceMode : ulong {
		None = 0,
		Station = 1,
		Ibss = 2,
		HostAP = 3,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWSecurity : ulong {
		None = 0,
		WEP = 1,
		WPAPersonal = 2,
		WPAPersonalMixed = 3,
		WPA2Personal = 4,
		Personal = 5,
		DynamicWEP = 6,
		WPAEnterprise = 7,
		WPAEnterpriseMixed = 8,
		WPA2Enterprise = 9,
		Enterprise = 10,
		Wpa3Personal = 11,
		Wpa3Enterprise = 12,
		Wpa3Transition = 13,
		[Mac (13, 0)]
		Owe = 14,
		[Mac (13, 0)]
		OweTransition = 15,
		Unknown = long.MaxValue,
	}

	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
	[Native]
	public enum CWIbssModeSecurity : ulong {
		None = 0,
		WEP40 = 1,
		WEP104 = 2,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWChannelWidth : ulong {
		Unknown = 0,
		TwentyMHz = 1,
		FourtyMHz = 2,
		EightyMHz = 3,
		OneHundredSixtyMHz = 4,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWChannelBand : ulong {
		Unknown = 0,
		TwoGHz = 1,
		FiveGHz = 2,
		SixGHz = 3,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWCipherKeyFlags : ulong {
		None = 0,
		Unicast = 1 << 1,
		Multicast = 1 << 2,
		Tx = 1 << 3,
		Rx = 1 << 4,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWKeychainDomain : ulong {
		None = 0,
		User = 1,
		System = 2,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWEventType : long {
		None = 0,
		PowerDidChange = 1,
		SsidDidChange = 2,
		BssidDidChange = 3,
		CountryCodeDidChange = 4,
		LinkDidChange = 5,
		LinkQualityDidChange = 6,
		ModeDidChange = 7,
		ScanCacheUpdated = 8,

		[Deprecated (PlatformName.MacOSX, 11, 0)]
		VirtualInterfaceStateChanged = 9,

		[Deprecated (PlatformName.MacOSX, 11, 0)]
		RangingReportEvent = 10,
		Unknown = long.MaxValue
	}
}
