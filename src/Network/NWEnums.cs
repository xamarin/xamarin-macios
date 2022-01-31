//
// NWEnums.cs: Network.framework enumerations
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//
#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using OS_nw_browse_result=System.IntPtr;
using OS_nw_endpoint=System.IntPtr;
using OS_nw_txt_record=System.IntPtr;

namespace Network {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	[Flags]
	public enum NWBrowseResultChange : ulong {
		Invalid = 0x00,
		Identical = 0x01,
		ResultAdded = 0x02,
		ResultRemoved = 0x04,
		TxtRecordChanged = 0x20,
		InterfaceAdded = 0x08, 
		InterfaceRemoved = 0x10,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWBrowserState {
		Invalid = 0,
		Ready = 1,
		Failed = 2,
		Cancelled = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWConnectionState {
		Invalid   = 0,
		Waiting   = 1,
		Preparing = 2,
		Ready     = 3,
		Failed    = 4,
		Cancelled = 5,
	}

#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("maccatalyst14.0")]
#else
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
	[Watch (7,0)]
	[MacCatalyst (14,0)]
#endif
	public enum NWConnectionGroupState {
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWDataTransferReportState {
		Collecting = 1,
		Collected = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWEndpointType {
		Invalid = 0,
		Address = 1,
		Host = 2,
		BonjourService = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWReportResolutionSource {
		Query = 1,
		Cache = 2,
		ExpiredCache = 3,
	}

#if NET
	[SupportedOSPlatform ("macos10.15")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("ios")]
#else
	[NoWatch]
	[NoTV]
	[NoiOS]
	[Mac (10,15)]
#endif
	public enum NWEthernetChannelState {
		Invalid = 0,
		Waiting = 1,
		Preparing = 2,
		Ready = 3,
		Failed = 4,
		Cancelled = 5,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	[Flags]
	public enum NWFramerCreateFlags : uint {
		Default = 0x00,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWFramerStartResult {
		Unknown = 0,
		Ready = 1,
		WillMarkReady = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[Watch (6,0)]
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
#endif
	public enum NWIPLocalAddressPreference {
		Default = 0,
		Temporary = 1,
		Stable = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[Watch (6,0)]
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
#endif
	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWInterfaceType {
		Other = 0,
		Wifi = 1,
		Cellular = 2,
		Wired = 3,
		Loopback = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWListenerState {
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWMultiPathService {
		Disabled = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3, 
	}

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWParametersExpiredDnsBehavior {
		Default = 0,
		Allow = 1,
		Prohibit = 2,
	}

	// this maps to `nw_path_status_t` in Network/Headers/path.h (and not the enum from NetworkExtension)
#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (6,0)]
#endif
	public enum NWPathStatus {
		Invalid = 0,
		Satisfied = 1,
		Unsatisfied = 2,
		Satisfiable = 3,
	}

	public enum NWServiceClass {
		BestEffort = 0,
		Background = 1,
		InteractiveVideo = 2,
		InteractiveVoice = 3,
		ResponsiveData = 4,
		Signaling = 5,
	}

	public enum NWIPEcnFlag {
		NonEct = 0,
		Ect = 2,
		Ect1 = 1,
		Ce = 3,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWTxtRecordFindKey {
		Invalid = 0,
		NotPresent = 1,
		NoValue = 2,
		EmptyValue = 3,
		NonEmptyValue = 4,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWWebSocketOpCode : int {
		Cont = 0x0,
		Text = 0x1,
		Binary = 0x2,
		Close = 0x8,
		Ping = 0x9,
		Pong = 0xA,
		Invalid = -1,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWWebSocketCloseCode : int {
		NormalClosure = 1000,
		GoingAway = 1001,
		ProtocolError = 1002,
		UnsupportedData = 1003,
		NoStatusReceived = 1005,
		AbnormalClosure = 1006,
		InvalidFramePayloadData = 1007,
		PolicyViolation = 1008,
		MessageTooBig = 1009,
		MandatoryExtension = 1010,
		InternalServerError = 1011,
		TlsHandshake = 1015,
	}

	// this maps to `nw_ws_version_t` in Network.framework/Headers/ws_options.h (and not the enum from NetworkExtension)
#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWWebSocketVersion {
		Invalid = 0,
		Version13 = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
#else
	[TV (13,0)]
	[Mac (10,15)]
	[iOS (13,0)]
	[Watch (6,0)]
#endif
	public enum NWWebSocketResponseStatus {
		Invalid = 0,
		Accept = 1,
		Reject = 2,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[TV (15,0)]
	[Mac (12,0)]
	[iOS (15,0)]
	[Watch (8,0)]
	[MacCatalyst (15,0)]
#endif
	public enum NWReportResolutionProtocol {
		Unknown = 0,
		Udp = 1,
		Tcp = 2,
		Tls = 3,
		Https = 4,
	}
	
#if NET
	[SupportedOSPlatform ("tvos14.0")]
	[SupportedOSPlatform ("macos11.0")]
	[SupportedOSPlatform ("ios14.0")]
#else
	[Watch (7,0)]
	[TV (14,0)]
	[Mac (11,0)]
	[iOS (14,0)]
#endif
	public enum NWResolverConfigEndpointType {
		Https,
		Tls,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
#else
	[Watch (8, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[iOS (15, 0)]
#endif
	public enum NWMultipathVersion {
		Unspecified = -1,
		Version0 = 0,
		Version1 = 1,
	}

#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
#else
	[Watch (8, 0)]
	[TV (15, 0)]
	[Mac (12, 0)]
	[iOS (15, 0)]
#endif
	public enum NWInterfaceRadioType {
		Unknown = 0,
		WifiB = 1,
		WifiA = 2,
		WifiG = 3,
		WifiN = 4,
		WifiAC = 5,
		WifiAX = 6,
		
		CellLte = 0x80,
		CellEndcSub6 = 0x81,
		CellEndcMmw = 0x82,
		CellNrSaSub6 = 0x83,
		CellNrSaMmw = 0x84,
		CellWcdma = 0x85,
		CellGsm = 0x86,
		CellCdma = 0x87,
		CellEvdo = 0x88,
	}
	
#if NET
	[SupportedOSPlatform ("tvos15.0")]
	[SupportedOSPlatform ("macos12.0")]
	[SupportedOSPlatform ("ios15.0")]
#else
	[Watch (8,0)]
	[TV (15,0)]
	[Mac (12,0)]
	[iOS (15,0)]
#endif
	public enum NWParametersAttribution {
		Developer = 1,
		User = 2,
	}
}
