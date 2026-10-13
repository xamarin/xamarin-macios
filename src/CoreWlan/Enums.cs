// Copyright 2014 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

using CoreFoundation;
using System.ComponentModel;

namespace CoreWlan {

	/// <summary>Indicates the result of a Core WLAN operation.</summary>
	[NoMacCatalyst]
	[Native]
	[ErrorDomain ("CWErrorDomain")] // enum named `CWErr` in headers
	public enum CWStatus : long {
		/// <summary>The operation completed successfully.</summary>
		Ok = 0,
		/// <summary>An Extensible Authentication Protocol over LAN (EAPOL) operation failed.</summary>
		EAPOL = 1,
		/// <summary>A parameter is invalid.</summary>
		InvalidParameter = -3900,
		/// <summary>There is not enough memory to complete the operation.</summary>
		NoMemory = -3901,
		/// <summary>An unknown error occurred.</summary>
		Unknown = -3902,
		/// <summary>The operation is not supported.</summary>
		NotSupported = -3903,
		/// <summary>The data format is invalid.</summary>
		InvalidFormat = -3904,
		/// <summary>The operation timed out.</summary>
		Timeout = -3905,
		/// <summary>An unspecified failure occurred.</summary>
		UnspecifiedFailure = -3906,
		/// <summary>The requested capabilities are not supported.</summary>
		UnsupportedCapabilities = -3907,
		/// <summary>The access point denied reassociation.</summary>
		ReassociationDenied = -3908,
		/// <summary>The access point denied association.</summary>
		AssociationDenied = -3909,
		/// <summary>The authentication algorithm is not supported.</summary>
		AuthenticationAlgorithmUnsupported = -3910,
		/// <summary>The authentication sequence number is invalid.</summary>
		InvalidAuthenticationSequenceNumber = -3911,
		/// <summary>The authentication challenge failed.</summary>
		ChallengeFailure = -3912,
		/// <summary>The access point has reached its client capacity.</summary>
		APFull = -3913,
		/// <summary>The rate set is not supported.</summary>
		UnsupportedRateSet = -3914,
		/// <summary>Short slot time is not supported.</summary>
		ShortSlotUnsupported = -3915,
		/// <summary>DSSS-OFDM is not supported.</summary>
		DSSSOFDMUnsupported = -3916,
		/// <summary>An information element is invalid.</summary>
		InvalidInformationElement = -3917,
		/// <summary>The group cipher is invalid.</summary>
		InvalidGroupCipher = -3918,
		/// <summary>The pairwise cipher is invalid.</summary>
		InvalidPairwiseCipher = -3919,
		/// <summary>The authentication and key management protocol is invalid.</summary>
		InvalidAKMP = -3920,
		/// <summary>The Robust Security Network (RSN) version is not supported.</summary>
		UnsupportedRSNVersion = -3921,
		/// <summary>The RSN capabilities are invalid.</summary>
		InvalidRSNCapabilities = -3922,
		/// <summary>The cipher suite was rejected.</summary>
		CipherSuiteRejected = -3923,
		/// <summary>The Pairwise Master Key (PMK) is invalid.</summary>
		InvalidPMK = -3924,
		/// <summary>The supplicant operation timed out.</summary>
		SupplicantTimeout = -3925,
		/// <summary>High Throughput (HT) features are not supported.</summary>
		HTFeaturesNotSupported = -3926,
		/// <summary>The PCO transition time is not supported.</summary>
		PCOTransitionTimeNotSupported = -3927,
		/// <summary>The reference is not bound.</summary>
		ReferenceNotBound = -3928,
		/// <summary>An interprocess communication (IPC) operation failed.</summary>
		IPCFailure = -3929,
		/// <summary>The operation is not permitted.</summary>
		OperationNotPermitted = -3930,
		/// <summary>An unspecified status was returned.</summary>
		Status = -3931,
	}

	/// <summary>Specifies the IEEE 802.11 physical-layer mode of a wireless interface.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWPhyMode : ulong {
		/// <summary>The physical-layer mode is unknown or unavailable.</summary>
		None = 0,
		/// <summary>IEEE 802.11a.</summary>
		A = 1,
		/// <summary>IEEE 802.11b.</summary>
		B = 2,
		/// <summary>IEEE 802.11g.</summary>
		G = 3,
		/// <summary>IEEE 802.11n (Wi-Fi 4).</summary>
		N = 4,
		/// <summary>IEEE 802.11ac (Wi-Fi 5).</summary>
		AC = 5,
		/// <summary>IEEE 802.11ax (Wi-Fi 6).</summary>
		AX = 6,
		/// <summary>IEEE 802.11be (Wi-Fi 7).</summary>
		[Mac (26, 4)]
		BE = 7,
	}

	/// <summary>Specifies the operating mode of a wireless interface.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWInterfaceMode : ulong {
		/// <summary>The interface mode is unknown or unavailable.</summary>
		None = 0,
		/// <summary>The interface operates as a station connected to a wireless network.</summary>
		Station = 1,
		/// <summary>The interface operates as an independent basic service set (IBSS) peer.</summary>
		Ibss = 2,
		/// <summary>The interface operates as a host access point.</summary>
		HostAP = 3,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWSecurity : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		WEP = 1,
		/// <summary>To be added.</summary>
		WPAPersonal = 2,
		/// <summary>To be added.</summary>
		WPAPersonalMixed = 3,
		/// <summary>To be added.</summary>
		WPA2Personal = 4,
		/// <summary>To be added.</summary>
		Personal = 5,
		/// <summary>To be added.</summary>
		DynamicWEP = 6,
		/// <summary>To be added.</summary>
		WPAEnterprise = 7,
		/// <summary>To be added.</summary>
		WPAEnterpriseMixed = 8,
		/// <summary>To be added.</summary>
		WPA2Enterprise = 9,
		/// <summary>To be added.</summary>
		Enterprise = 10,
		Wpa3Personal = 11,
		Wpa3Enterprise = 12,
		Wpa3Transition = 13,
		Owe = 14,
		OweTransition = 15,
		/// <summary>To be added.</summary>
		Unknown = long.MaxValue,
	}

	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 11, 0)]
	[Native]
	public enum CWIbssModeSecurity : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		WEP40 = 1,
		/// <summary>To be added.</summary>
		WEP104 = 2,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWChannelWidth : ulong {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		TwentyMHz = 1,
		/// <summary>To be added.</summary>
		FortyMHz = 2,
#if !XAMCORE_5_0
		[Obsolete ("Use 'FortyMHz' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		FourtyMHz = FortyMHz,
#endif
		/// <summary>To be added.</summary>
		EightyMHz = 3,
		/// <summary>To be added.</summary>
		OneHundredSixtyMHz = 4,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWChannelBand : ulong {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		TwoGHz = 1,
		/// <summary>To be added.</summary>
		FiveGHz = 2,
		SixGHz = 3,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWCipherKeyFlags : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Unicast = 1 << 1,
		/// <summary>To be added.</summary>
		Multicast = 1 << 2,
		/// <summary>To be added.</summary>
		Tx = 1 << 3,
		/// <summary>To be added.</summary>
		Rx = 1 << 4,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWKeychainDomain : ulong {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		User = 1,
		/// <summary>To be added.</summary>
		System = 2,
	}

	[NoMacCatalyst]
	[Native]
	public enum CWEventType : long {
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		PowerDidChange = 1,
		/// <summary>To be added.</summary>
		SsidDidChange = 2,
		/// <summary>To be added.</summary>
		BssidDidChange = 3,
		/// <summary>To be added.</summary>
		CountryCodeDidChange = 4,
		/// <summary>To be added.</summary>
		LinkDidChange = 5,
		/// <summary>To be added.</summary>
		LinkQualityDidChange = 6,
		/// <summary>To be added.</summary>
		ModeDidChange = 7,
		/// <summary>To be added.</summary>
		ScanCacheUpdated = 8,

		/// <summary>To be added.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0)]
		VirtualInterfaceStateChanged = 9,

		/// <summary>To be added.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0)]
		RangingReportEvent = 10,
		/// <summary>To be added.</summary>
		Unknown = long.MaxValue,
	}
}
