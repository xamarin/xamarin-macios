// Copyright 2014 Xamarin Inc. All rights reserved.

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using System;

namespace CoreWlan {
	[Native]
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

	[Native]
	public enum CWPhyMode : ulong {
		None = 0,
		A = 1,
		B = 2,
		G = 3,
		N = 4,
		AC = 5,
	}

	[Native]
	public enum CWInterfaceMode : ulong {
		None = 0,
		Station = 1,
		Ibss = 2,
		HostAP = 3,
	}

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
		Unknown = int.MaxValue,
	}

	[Native]
	public enum CWIbssModeSecurity : ulong {
		None = 0,
		WEP40 = 1,
		WEP104 = 2,
	}

	[Native]
	public enum CWChannelWidth : ulong {
		Unknown = 0,
		TwentyMHz = 1,
		FourtyMHz = 2,
		EightyMHz = 3,
		OneHundredSixtyMHz = 4,
	}

	[Native]
	public enum CWChannelBand : ulong {
		Unknown = 0,
		TwoGHz = 1,
		FiveGHz = 2,
	}

	[Native]
	public enum CWCipherKeyFlags : ulong {
		None = 0,
		Unicast = 1 << 1,
		Multicast = 1 << 2,
		Tx = 1 << 3,
		Rx = 1 << 4,
	}

	[Native]
	public enum CWKeychainDomain : ulong {
		None = 0,
		User = 1,
		System = 2,
	}

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
		VirtualInterfaceStateChanged = 9,
		RangingReportEvent = 10,
#if XAMCORE_2_0
		Unknown = long.MaxValue
#else
		Unknown = int.MaxValue
#endif
	}
}
