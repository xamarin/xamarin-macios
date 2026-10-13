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
		/// <summary>A generic error occurred, and no specific error code is available.</summary>
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

	/// <summary>Specifies the security mode of a wireless network.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWSecurity : ulong {
		/// <summary>The network has no security.</summary>
		None = 0,
		/// <summary>Wired Equivalent Privacy (WEP).</summary>
		WEP = 1,
		/// <summary>Wi-Fi Protected Access (WPA) Personal.</summary>
		WPAPersonal = 2,
		/// <summary>Mixed WPA and WPA2 Personal.</summary>
		WPAPersonalMixed = 3,
		/// <summary>Wi-Fi Protected Access 2 (WPA2) Personal.</summary>
		WPA2Personal = 4,
		/// <summary>WPA or WPA2 Personal.</summary>
		Personal = 5,
		/// <summary>Dynamic WEP.</summary>
		DynamicWEP = 6,
		/// <summary>WPA Enterprise.</summary>
		WPAEnterprise = 7,
		/// <summary>Mixed WPA and WPA2 Enterprise.</summary>
		WPAEnterpriseMixed = 8,
		/// <summary>WPA2 Enterprise.</summary>
		WPA2Enterprise = 9,
		/// <summary>WPA or WPA2 Enterprise.</summary>
		Enterprise = 10,
		/// <summary>WPA3 Personal.</summary>
		Wpa3Personal = 11,
		/// <summary>WPA3 Enterprise.</summary>
		Wpa3Enterprise = 12,
		/// <summary>WPA3 Personal transition mode, which supports WPA2 Personal clients.</summary>
		Wpa3Transition = 13,
		/// <summary>Opportunistic Wireless Encryption (OWE).</summary>
		Owe = 14,
		/// <summary>OWE transition mode, which supports clients that do not support OWE.</summary>
		OweTransition = 15,
		/// <summary>The security mode is unknown.</summary>
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

	/// <summary>Specifies the channel width of a wireless network.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWChannelWidth : ulong {
		/// <summary>The channel width is unknown.</summary>
		Unknown = 0,
		/// <summary>A 20 MHz channel width.</summary>
		TwentyMHz = 1,
		/// <summary>A 40 MHz channel width.</summary>
		FortyMHz = 2,
#if !XAMCORE_5_0
		/// <summary>A 40 MHz channel width. Use <see cref="CWChannelWidth.FortyMHz"/> instead.</summary>
		[Obsolete ("Use 'FortyMHz' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		FourtyMHz = FortyMHz,
#endif
		/// <summary>An 80 MHz channel width.</summary>
		EightyMHz = 3,
		/// <summary>A 160 MHz channel width.</summary>
		OneHundredSixtyMHz = 4,
	}

	/// <summary>Specifies the frequency band of a wireless channel.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWChannelBand : ulong {
		/// <summary>The channel band is unknown.</summary>
		Unknown = 0,
		/// <summary>The 2.4 GHz band.</summary>
		TwoGHz = 1,
		/// <summary>The 5 GHz band.</summary>
		FiveGHz = 2,
		/// <summary>The 6 GHz band.</summary>
		SixGHz = 3,
	}

	/// <summary>Specifies the flags associated with a cipher key.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWCipherKeyFlags : ulong {
		/// <summary>No cipher key flags are set.</summary>
		None = 0,
		/// <summary>The key is used for unicast traffic.</summary>
		Unicast = 1 << 1,
		/// <summary>The key is used for multicast traffic.</summary>
		Multicast = 1 << 2,
		/// <summary>The key is used to transmit data.</summary>
		Tx = 1 << 3,
		/// <summary>The key is used to receive data.</summary>
		Rx = 1 << 4,
	}

	/// <summary>Specifies the keychain domain for a wireless network credential.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWKeychainDomain : ulong {
		/// <summary>No keychain domain is specified.</summary>
		None = 0,
		/// <summary>The user's keychain.</summary>
		User = 1,
		/// <summary>The system keychain.</summary>
		System = 2,
	}

	/// <summary>Identifies events that a wireless interface can report.</summary>
	[NoMacCatalyst]
	[Native]
	public enum CWEventType : long {
		/// <summary>No event is specified.</summary>
		None = 0,
		/// <summary>The interface's power state changed.</summary>
		PowerDidChange = 1,
		/// <summary>The network name (SSID) changed.</summary>
		SsidDidChange = 2,
		/// <summary>The access point's basic service set identifier (BSSID) changed.</summary>
		BssidDidChange = 3,
		/// <summary>The country code changed.</summary>
		CountryCodeDidChange = 4,
		/// <summary>The wireless link state changed.</summary>
		LinkDidChange = 5,
		/// <summary>The wireless link quality changed.</summary>
		LinkQualityDidChange = 6,
		/// <summary>The interface mode changed.</summary>
		ModeDidChange = 7,
		/// <summary>The scan cache was updated.</summary>
		ScanCacheUpdated = 8,

		/// <summary>The state of a virtual interface changed.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0)]
		VirtualInterfaceStateChanged = 9,

		/// <summary>A ranging report was received.</summary>
		[Deprecated (PlatformName.MacOSX, 11, 0)]
		RangingReportEvent = 10,
		/// <summary>An unknown event occurred.</summary>
		Unknown = long.MaxValue,
	}
}
