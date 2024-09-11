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

using OS_nw_browse_result = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

namespace Network {

	[Flags, TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWBrowseResultChange : ulong {
		Invalid = 0x00,
		Identical = 0x01,
		ResultAdded = 0x02,
		ResultRemoved = 0x04,
		TxtRecordChanged = 0x20,
		InterfaceAdded = 0x08,
		InterfaceRemoved = 0x10,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWBrowserState {
		Invalid = 0,
		Ready = 1,
		Failed = 2,
		Cancelled = 3,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWConnectionState {
		Invalid = 0,
		Waiting = 1,
		Preparing = 2,
		Ready = 3,
		Failed = 4,
		Cancelled = 5,
	}

	[TV (14, 0), iOS (14, 0), Watch (7, 0)]
	[MacCatalyst (14, 0)]
	public enum NWConnectionGroupState {
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWDataTransferReportState {
		Collecting = 1,
		Collected = 2,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWEndpointType {
		Invalid = 0,
		Address = 1,
		Host = 2,
		BonjourService = 3,
		[TV (13, 0), iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		Url = 4,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWReportResolutionSource {
		Query = 1,
		Cache = 2,
		ExpiredCache = 3,
	}

	[NoWatch, NoTV, NoiOS]
	[NoMacCatalyst]
	public enum NWEthernetChannelState {
		Invalid = 0,
		Waiting = 1,
		Preparing = 2,
		Ready = 3,
		Failed = 4,
		Cancelled = 5,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
	[Flags]
	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWFramerCreateFlags : uint {
		Default = 0x00,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWFramerStartResult {
		Unknown = 0,
		Ready = 1,
		WillMarkReady = 2,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	public enum NWIPLocalAddressPreference {
		Default = 0,
		Temporary = 1,
		Stable = 2,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWIPVersion {
		Any = 0,
		Version4 = 1,
		Version6 = 2,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWInterfaceType {
		Other = 0,
		Wifi = 1,
		Cellular = 2,
		Wired = 3,
		Loopback = 4,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWListenerState {
		Invalid = 0,
		Waiting = 1,
		Ready = 2,
		Failed = 3,
		Cancelled = 4,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWMultiPathService {
		Disabled = 0,
		Handover = 1,
		Interactive = 2,
		Aggregate = 3,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWParametersExpiredDnsBehavior {
		Default = 0,
		Allow = 1,
		Prohibit = 2,
	}

	// this maps to `nw_path_status_t` in Network/Headers/path.h (and not the enum from NetworkExtension)
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
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

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWTxtRecordFindKey {
		Invalid = 0,
		NotPresent = 1,
		NoValue = 2,
		EmptyValue = 3,
		NonEmptyValue = 4,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWWebSocketOpCode : int {
		Cont = 0x0,
		Text = 0x1,
		Binary = 0x2,
		Close = 0x8,
		Ping = 0x9,
		Pong = 0xA,
		Invalid = -1,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
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
	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWWebSocketVersion {
		Invalid = 0,
		Version13 = 1,
	}

	[TV (13, 0), iOS (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum NWWebSocketResponseStatus {
		Invalid = 0,
		Accept = 1,
		Reject = 2,
	}

	[TV (15, 0), iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
	public enum NWReportResolutionProtocol {
		Unknown = 0,
		Udp = 1,
		Tcp = 2,
		Tls = 3,
		Https = 4,
	}

	[Watch (7, 0), TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum NWResolverConfigEndpointType {
		Https,
		Tls,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum NWMultipathVersion {
		Unspecified = -1,
		Version0 = 0,
		Version1 = 1,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum NWParametersAttribution {
		Developer = 1,
		User = 2,
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	public enum NWQuicStreamType {
		Unknown = 0,
		Bidirectional = 1,
		Unidirectional = 2,
		[Watch (9, 4), TV (16, 4), Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		Datagram = 3,
	}
}
