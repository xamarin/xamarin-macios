using ObjCRuntime;

namespace NetworkExtension {

	[MacCatalyst (13, 1)]
	[ErrorDomain ("NEVPNErrorDomain")]
	[Native]
	public enum NEVpnError : long {
		ConfigurationInvalid = 1,
		ConfigurationDisabled = 2,
		ConnectionFailed = 3,
		ConfigurationStale = 4,
		ConfigurationReadWriteFailed = 5,
		ConfigurationUnknown = 6
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEVpnStatus : long {
		Invalid = 0,
		Disconnected = 1,
		Connecting = 2,
		Connected = 3,
		Reasserting = 4,
		Disconnecting = 5
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEVpnIkeAuthenticationMethod : long {
		None = 0,
		Certificate = 1,
		SharedSecret = 2
	}

	[MacCatalyst (13, 1)]
	[Native ("NEVPNIKEv2EncryptionAlgorithm")]
	public enum NEVpnIke2EncryptionAlgorithm : long {
		[NoTV]
		DES = 1,
		[NoTV]
		TripleDES = 2,
		AES128 = 3,
		AES256 = 4,
		[MacCatalyst (13, 1)]
		AES128GCM = 5,
		[MacCatalyst (13, 1)]
		AES256GCM = 6,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		ChaCha20Poly1305 = 7,
	}

	[MacCatalyst (13, 1)]
	[Native ("NEVPNIKEv2IntegrityAlgorithm")]
	public enum NEVpnIke2IntegrityAlgorithm : long {
		[NoTV]
		SHA96 = 1,
		[NoTV]
		SHA160 = 2,
		SHA256 = 3,
		SHA384 = 4,
		SHA512 = 5
	}

	[MacCatalyst (13, 1)]
	[Native ("NEVPNIKEv2DeadPeerDetectionRate")]
	public enum NEVpnIke2DeadPeerDetectionRate : long {
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3
	}

	[MacCatalyst (13, 1)]
	[Native ("NEVPNIKEv2DiffieHellmanGroup")]
	public enum NEVpnIke2DiffieHellman : long {
		Invalid = 0,
		Group0 = Invalid,
		[NoTV]
		Group1 = 1,
		[NoTV]
		Group2 = 2,
		[NoTV]
		Group5 = 5,
		Group14 = 14,
		Group15 = 15,
		Group16 = 16,
		Group17 = 17,
		Group18 = 18,
		Group19 = 19,
		Group20 = 20,
		Group21 = 21,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Group31 = 31,
		[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		Group32 = 32,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEOnDemandRuleAction : long {
		Connect = 1,
		Disconnect = 2,
		EvaluateConnection = 3,
		Ignore = 4
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEOnDemandRuleInterfaceType : long {
		Any = 0,
		Ethernet = 1,
		WiFi = 2,
		[NoTV]
		Cellular = 3
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEEvaluateConnectionRuleAction : long {
		ConnectIfNeeded = 1,
		NeverConnect = 2
	}

	[MacCatalyst (13, 1)]
	[Native ("NEVPNIKEv2CertificateType")] // NSInteger
	public enum NEVpnIke2CertificateType : long {
		RSA = 1,
		ECDSA256 = 2,
		ECDSA384 = 3,
		ECDSA521 = 4,
		[NoTV]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Ed25519 = 5,
		[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		Rsapss = 6,
	}

	// in Xcode7 SDK but marked as 8.0
	[NoTV]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("NEFilterErrorDomain")]
	[Native]
	public enum NEFilterManagerError : long {
		None = 0,
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4,
		ConfigurationPermissionDenied = 5,
		ConfigurationInternalError = 6,
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("NETunnelProviderErrorDomain")]
	[Native]
	public enum NETunnelProviderError : long {
		None = 0,
		Invalid = 1,
		Canceled = 2,
		Failed = 3
	}

	[MacCatalyst (13, 1)]
	[ErrorDomain ("NEAppProxyErrorDomain")]
	[Native]
	public enum NEAppProxyFlowError : long {
		None = 0,
		NotConnected = 1,
		PeerReset = 2,
		HostUnreachable = 3,
		InvalidArgument = 4,
		Aborted = 5,
		Refused = 6,
		TimedOut = 7,
		Internal = 8,
		// iOS 9.3
		DatagramTooLarge = 9,
		ReadAlreadyPending = 10,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NEProviderStopReason : long {
		None = 0,
		UserInitiated = 1,
		ProviderFailed = 2,
		NoNetworkAvailable = 3,
		UnrecoverableNetworkChange = 4,
		ProviderDisabled = 5,
		AuthenticationCanceled = 6,
		ConfigurationFailed = 7,
		IdleTimeout = 8,
		ConfigurationDisabled = 9,
		ConfigurationRemoved = 10,
		Superseded = 11,
		UserLogout = 12,
		UserSwitch = 13,
		ConnectionFailed = 14,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Sleep = 15,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		AppUpdate,
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NWPathStatus : long {
		Invalid = 0,
		Satisfied = 1,
		Unsatisfied = 2,
		Satisfiable = 3
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NWTcpConnectionState : long {
		Invalid = 0,
		Connecting = 1,
		Waiting = 2,
		Connected = 3,
		Disconnected = 4,
		Cancelled = 5
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum NWUdpSessionState : long {
		Invalid = 0,
		Waiting = 1,
		Preparing = 2,
		Ready = 3,
		Failed = 4,
		Cancelled = 5
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NETunnelProviderRoutingMethod : long {
		DestinationIP = 1,
		SourceApplication = 2,
		[NoiOS]
		[NoMacCatalyst]
		NetworkRule = 3,
	}

#if !MONOMAC
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NEHotspotHelperCommandType : long {
		None = 0,
		FilterScanList = 1,
		Evaluate = 2,
		Authenticate = 3,
		PresentUI = 4,
		Maintain = 5,
		Logoff = 6
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NEHotspotHelperConfidence : long {
		None = 0,
		Low = 1,
		High = 2
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NEHotspotHelperResult : long {
		Success = 0,
		Failure = 1,
		UIRequired = 2,
		CommandNotRecognized = 3,
		AuthenticationRequired = 4,
		UnsupportedNetwork = 5,
		TemporaryFailure = 6
	}
#endif

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	[ErrorDomain ("NEAppPushErrorDomain")]
	public enum NEAppPushManagerError : long {
		ConfigurationInvalid = 1,
		ConfigurationNotLoaded = 2,
		InternalError = 3,
		InactiveSession = 4,
	}

	[NoWatch, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native ("NEDNSProtocol")]
	public enum NEDnsProtocol : long {
		Cleartext = 1,
		Tls = 2,
		Https = 3,
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native ("NEDNSSettingsManagerError")]
	[ErrorDomain ("NEDNSSettingsErrorDomain")]
	public enum NEDnsSettingsManagerError : long {
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4,
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NERelayManagerError : long {
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4,
	}

}
