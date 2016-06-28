using XamCore.ObjCRuntime;

namespace XamCore.NetworkExtension {

	[iOS (8,0)][Mac (10,10)]
	[ErrorDomain ("NEVPNErrorDomain")]
	[Native]
	public enum NEVpnError : nint {
		ConfigurationInvalid = 1,
		ConfigurationDisabled = 2,
		ConnectionFailed = 3,
		ConfigurationStale = 4,
		ConfigurationReadWriteFailed = 5,
		ConfigurationUnknown = 6
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnStatus : nint {
		Invalid = 0,
		Disconnected = 1,
		Connecting = 2,
		Connected = 3,
		Reasserting = 4,
		Disconnecting = 5
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnIkeAuthenticationMethod : nint {
		None = 0,
		Certificate = 1,
		SharedSecret = 2
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnIke2EncryptionAlgorithm : nint {
		DES = 1,
		TripleDES = 2,
		AES128 = 3,
		AES256 = 4,
		[iOS (8,3)][Mac (10,11)]
		AES128GCM = 5,
		[iOS (8,3)][Mac (10,11)]
		AES256GCM = 6,
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnIke2IntegrityAlgorithm : nint {
		SHA96 = 1,
		SHA160 = 2,
		SHA256 = 3,
		SHA384 = 4,
		SHA512 = 5
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnIke2DeadPeerDetectionRate : nint {
		None = 0,
		Low = 1,
		Medium = 2,
		High = 3
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEVpnIke2DiffieHellman : nint {
		Group0 = 0,
		Group1 = 1,
		Group2 = 2,
		Group5 = 5,
		Group14 = 14,
		Group15 = 15,
		Group16 = 16,
		Group17 = 17,
		Group18 = 18,
		Group19 = 19,
		Group20 = 20,
		Group21 = 21,
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEOnDemandRuleAction : nint {
		Connect = 1,
		Disconnect = 2,
		EvaluateConnection = 3,
		Ignore = 4
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEOnDemandRuleInterfaceType : nint {
		[iOS (9,0)][Mac (10,11)]
		Any = 0,
		Ethernet = 1,
		WiFi = 2,
		Cellular = 3
	}

	[iOS (8,0)][Mac (10,10)]
	[Native]
	public enum NEEvaluateConnectionRuleAction : nint {
		ConnectIfNeeded = 1,
		NeverConnect = 2
	}

	[iOS (8,3)][Mac (10,11)]
	[Native] // NSInteger
	public enum NEVpnIke2CertificateType : nint {
		RSA = 1,
		ECDSA256 = 2,
		ECDSA384 = 3,
		ECDSA521 = 4,
	}

	// in Xcode7 SDK but marked as 8.0
	[iOS (8,0)]
	[ErrorDomain ("NEFilterErrorDomain")]
	[Native]
	public enum NEFilterManagerError : nint {
		None = 0,
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4
	}

	[iOS (9,0)]
	[ErrorDomain ("NETunnelProviderErrorDomain")]
	[Native]
	public enum NETunnelProviderError : nint {
		None = 0,
		Invalid = 1,
		Canceled = 2,
		Failed = 3
	}

	[iOS (9,0)]
	[ErrorDomain ("NEAppProxyErrorDomain")]
	[Native]
	public enum NEAppProxyFlowError : nint {
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

	[iOS (9,0)]
	[Native]
	public enum NEProviderStopReason : nint
	{
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
		ConnectionFailed = 14
	}

	[iOS (9,0)]
	[Native]
	public enum NWPathStatus : nint
	{
		Invalid = 0,
		Satisfied = 1,
		Unsatisfied = 2,
		Satisfiable = 3
	}
	
	[iOS (9,0)]
	[Native]
	public enum NWTcpConnectionState : nint
	{
		Invalid = 0,
		Connecting = 1,
		Waiting = 2,
		Connected = 3,
		Disconnected = 4,
		Cancelled = 5
	}

	[iOS (9,0)]
	[Native]
	public enum NWUdpSessionState : nint
	{
		Invalid = 0,
		Waiting = 1,
		Preparing = 2,
		Ready = 3,
		Failed = 4,
		Cancelled = 5
	}
	
	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum NETunnelProviderRoutingMethod : nint {
		DestinationIP = 1,
		SourceApplication = 2,
	}

#if !MONOMAC
	[iOS (9,0)]
	[Native]
	public enum NEHotspotHelperCommandType : nint {
		None = 0,
		FilterScanList = 1,
		Evaluate = 2,
		Authenticate = 3,
		PresentUI = 4,
		Maintain = 5,
		Logoff = 6
	}

	[iOS (9,0)]
	[Native]
	public enum NEHotspotHelperConfidence : nint {
		None = 0,
		Low = 1,
		High = 2
	}

	[iOS (9,0)]
	[Native]
	public enum NEHotspotHelperResult : nint {
		Success = 0,
		Failure = 1,
		UIRequired = 2,
		CommandNotRecognized = 3,
		AuthenticationRequired = 4,
		UnsupportedNetwork = 5,
		TemporaryFailure = 6
	}
#endif
}
