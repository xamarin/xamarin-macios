//
// NWEnums.cs: Network.framework enumerations
//
// Authors:
//   Manuel de la Pena (mandel@microsoft.com)
//
// Copyright 2019 Microsoft Inc
//
#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;

using OS_nw_browse_result = System.IntPtr;
using OS_nw_endpoint = System.IntPtr;
using OS_nw_txt_record = System.IntPtr;

namespace Network {

	/// <summary>Describes changes between two network browse results.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum NWBrowseResultChange : ulong {
		/// <summary>The change is invalid.</summary>
		Invalid = 0x00,
		/// <summary>The browse results are identical.</summary>
		Identical = 0x01,
		/// <summary>A browse result was added.</summary>
		ResultAdded = 0x02,
		/// <summary>A browse result was removed.</summary>
		ResultRemoved = 0x04,
		/// <summary>The TXT record changed.</summary>
		TxtRecordChanged = 0x20,
		/// <summary>The service became discoverable on another interface.</summary>
		InterfaceAdded = 0x08,
		/// <summary>The service is no longer discoverable on an interface.</summary>
		InterfaceRemoved = 0x10,
	}

	/// <summary>Describes the state of a network browser.</summary>
	[MacCatalyst (13, 1)]
	public enum NWBrowserState {
		/// <summary>The browser state is invalid.</summary>
		Invalid = 0,
		/// <summary>The browser can receive endpoint updates.</summary>
		Ready = 1,
		/// <summary>The browser has failed and cannot be restarted.</summary>
		Failed = 2,
		/// <summary>The browser was cancelled and cannot be restarted.</summary>
		Cancelled = 3,
	}

	/// <summary>Describes the state of a network connection.</summary>
	[MacCatalyst (13, 1)]
	public enum NWConnectionState {
		/// <summary>The connection state is invalid.</summary>
		Invalid = 0,
		/// <summary>The connection is waiting for a usable network.</summary>
		Waiting = 1,
		/// <summary>The connection is being established.</summary>
		Preparing = 2,
		/// <summary>The connection can send and receive data.</summary>
		Ready = 3,
		/// <summary>The connection has irrecoverably failed or closed.</summary>
		Failed = 4,
		/// <summary>The connection was cancelled.</summary>
		Cancelled = 5,
	}

	/// <summary>Describes the state of a network connection group.</summary>
	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum NWConnectionGroupState {
		/// <summary>The connection group state is invalid.</summary>
		Invalid = 0,
		/// <summary>The group is waiting for a usable network.</summary>
		Waiting = 1,
		/// <summary>The group can receive and process incoming messages.</summary>
		Ready = 2,
		/// <summary>The group has irrecoverably failed.</summary>
		Failed = 3,
		/// <summary>The group was cancelled.</summary>
		Cancelled = 4,
	}

	/// <summary>Describes whether a network data transfer report is still collecting data.</summary>
	[MacCatalyst (13, 1)]
	public enum NWDataTransferReportState {
		/// <summary>The report is collecting transfer data.</summary>
		Collecting = 1,
		/// <summary>The report has finished collecting transfer data.</summary>
		Collected = 2,
	}

	/// <summary>Identifies the kind of network endpoint.</summary>
	[MacCatalyst (13, 1)]
	public enum NWEndpointType {
		/// <summary>The endpoint type is invalid.</summary>
		Invalid = 0,
		/// <summary>An IP address and port.</summary>
		Address = 1,
		/// <summary>A hostname and port.</summary>
		Host = 2,
		/// <summary>A Bonjour service name, type, and domain.</summary>
		BonjourService = 3,
		/// <summary>A URL endpoint.</summary>
		[MacCatalyst (13, 1)]
		Url = 4,
	}

	/// <summary>Identifies where a network name resolution result came from.</summary>
	[MacCatalyst (13, 1)]
	public enum NWReportResolutionSource {
		/// <summary>The result came from a network query.</summary>
		Query = 1,
		/// <summary>The result came from a cached response.</summary>
		Cache = 2,
		/// <summary>The result came from an expired cached response.</summary>
		ExpiredCache = 3,
	}

	/// <summary>Describes the state of an Ethernet channel.</summary>
	[NoTV, NoiOS]
	[NoMacCatalyst]
	[NativeName ("nw_ethernet_channel_state_t")]
	public enum NWEthernetChannelState {
		/// <summary>The channel state is invalid.</summary>
		Invalid = 0,
		/// <summary>The channel is waiting for a usable network.</summary>
		Waiting = 1,
		/// <summary>The channel is being established.</summary>
		Preparing = 2,
		/// <summary>The channel can send and receive data.</summary>
		Ready = 3,
		/// <summary>The channel has irrecoverably failed or closed.</summary>
		Failed = 4,
		/// <summary>The channel was cancelled.</summary>
		Cancelled = 5,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
	/// <summary>Specifies options for creating a custom framer protocol.</summary>
	[Flags]
	[MacCatalyst (13, 1)]
	public enum NWFramerCreateFlags : uint {
		/// <summary>Use the default framer protocol options.</summary>
		Default = 0x00,
	}

	// from System/Library/Frameworks/Network.framework/Headers/framer_options.h:
	/// <summary>Indicates when a custom framer protocol becomes ready.</summary>
	[MacCatalyst (13, 1)]
	public enum NWFramerStartResult {
		/// <summary>The result is unknown.</summary>
		Unknown = 0,
		/// <summary>The protocol is marked ready when the start handler returns.</summary>
		Ready = 1,
		/// <summary>The start handler will mark the protocol ready later.</summary>
		WillMarkReady = 2,
	}

	/// <summary>Specifies a preference for choosing a local IP address for an outbound connection.</summary>
	[MacCatalyst (13, 1)]
	public enum NWIPLocalAddressPreference {
		/// <summary>Use the system's default address selection.</summary>
		Default = 0,
		/// <summary>Prefer a temporary address for privacy.</summary>
		Temporary = 1,
		/// <summary>Prefer a stable address.</summary>
		Stable = 2,
	}

	/// <summary>Specifies which version of the Internet Protocol a connection can use.</summary>
	[MacCatalyst (13, 1)]
	public enum NWIPVersion {
		/// <summary>Allow either IP version.</summary>
		Any = 0,
		/// <summary>Use IPv4.</summary>
		Version4 = 1,
		/// <summary>Use IPv6.</summary>
		Version6 = 2,
	}

	/// <summary>Identifies the underlying media of a network interface.</summary>
	[MacCatalyst (13, 1)]
	public enum NWInterfaceType {
		/// <summary>A virtual interface or an interface of an unknown type.</summary>
		Other = 0,
		/// <summary>A Wi-Fi interface.</summary>
		Wifi = 1,
		/// <summary>A cellular interface.</summary>
		Cellular = 2,
		/// <summary>A wired Ethernet interface.</summary>
		Wired = 3,
		/// <summary>A loopback interface.</summary>
		Loopback = 4,
	}

	/// <summary>Describes the state of a network listener.</summary>
	[MacCatalyst (13, 1)]
	public enum NWListenerState {
		/// <summary>The listener state is invalid.</summary>
		Invalid = 0,
		/// <summary>The listener is waiting for a usable network.</summary>
		Waiting = 1,
		/// <summary>The listener can accept incoming connections.</summary>
		Ready = 2,
		/// <summary>The listener has irrecoverably failed or closed.</summary>
		Failed = 3,
		/// <summary>The listener was cancelled.</summary>
		Cancelled = 4,
	}

	/// <summary>Specifies how a connection can use multiple network interfaces.</summary>
	[MacCatalyst (13, 1)]
	public enum NWMultiPathService {
		/// <summary>Do not attempt multipath transport.</summary>
		Disabled = 0,
		/// <summary>Use an expensive interface only when the primary interface is unavailable.</summary>
		Handover = 1,
		/// <summary>Use an expensive interface more aggressively to reduce latency.</summary>
		Interactive = 2,
		/// <summary>Use all available interfaces to improve throughput and latency.</summary>
		Aggregate = 3,
	}

	/// <summary>Specifies whether a connection can use expired DNS answers during establishment.</summary>
	[MacCatalyst (13, 1)]
	public enum NWParametersExpiredDnsBehavior {
		/// <summary>Let the system decide whether to use expired DNS answers.</summary>
		Default = 0,
		/// <summary>Allow the use of expired DNS answers.</summary>
		Allow = 1,
		/// <summary>Prohibit the use of expired DNS answers.</summary>
		Prohibit = 2,
		/// <summary>Allow expired DNS answers and cache answers persistently for the process. Use only for hostnames whose resolutions do not change across networks.</summary>
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		Persistent = 3,
	}

	// this maps to `nw_path_status_t` in Network/Headers/path.h (and not the enum from NetworkExtension)
	/// <summary>Describes whether a network path has a usable route.</summary>
	[MacCatalyst (13, 1)]
	public enum NWPathStatus {
		/// <summary>The path is invalid.</summary>
		Invalid = 0,
		/// <summary>The path has a usable route for sending and receiving data.</summary>
		Satisfied = 1,
		/// <summary>The path has no usable route.</summary>
		Unsatisfied = 2,
		/// <summary>The path has no usable route, but a connection attempt will trigger network attachment.</summary>
		Satisfiable = 3,
	}

	/// <summary>Specifies the network queuing priority for a connection's traffic.</summary>
	public enum NWServiceClass {
		/// <summary>Use the default traffic priority.</summary>
		BestEffort = 0,
		/// <summary>Prioritize bulk traffic below foreground traffic.</summary>
		Background = 1,
		/// <summary>Prioritize interactive video traffic.</summary>
		InteractiveVideo = 2,
		/// <summary>Prioritize interactive voice traffic.</summary>
		InteractiveVoice = 3,
		/// <summary>Prioritize interactive user data.</summary>
		ResponsiveData = 4,
		/// <summary>Prioritize short, delay-sensitive signaling traffic.</summary>
		Signaling = 5,
	}

	/// <summary>Identifies the explicit congestion notification (ECN) marking in an IP header.</summary>
	public enum NWIPEcnFlag {
		/// <summary>The transport is not ECN-capable.</summary>
		NonEct = 0,
		/// <summary>The transport is ECN-capable with the ECT(0) marking.</summary>
		Ect = 2,
		/// <summary>The transport is ECN-capable with the ECT(1) marking.</summary>
		Ect1 = 1,
		/// <summary>Congestion was experienced.</summary>
		Ce = 3,
	}

	/// <summary>Describes the result of looking up a key in a DNS TXT record.</summary>
	[MacCatalyst (13, 1)]
	public enum NWTxtRecordFindKey {
		/// <summary>The key is empty, contains non-ASCII characters, or exceeds 255 bytes.</summary>
		Invalid = 0,
		/// <summary>The key is not present in the TXT record.</summary>
		NotPresent = 1,
		/// <summary>The key is present without an assigned value.</summary>
		NoValue = 2,
		/// <summary>The key is present with an empty value.</summary>
		EmptyValue = 3,
		/// <summary>The key is present with a non-empty value.</summary>
		NonEmptyValue = 4,
	}

	/// <summary>Identifies the type of a WebSocket frame.</summary>
	[MacCatalyst (13, 1)]
	public enum NWWebSocketOpCode : int {
		/// <summary>A continuation frame. The WebSocket protocol handles these frames internally.</summary>
		Cont = 0x0,
		/// <summary>A text frame.</summary>
		Text = 0x1,
		/// <summary>A binary frame.</summary>
		Binary = 0x2,
		/// <summary>A close frame.</summary>
		Close = 0x8,
		/// <summary>A ping frame.</summary>
		Ping = 0x9,
		/// <summary>A pong frame.</summary>
		Pong = 0xA,
		/// <summary>An invalid frame.</summary>
		Invalid = -1,
	}

	/// <summary>Indicates why a WebSocket connection was closed.</summary>
	[MacCatalyst (13, 1)]
	public enum NWWebSocketCloseCode : int {
		/// <summary>The connection completed its intended purpose.</summary>
		NormalClosure = 1000,
		/// <summary>An endpoint is going away, such as a server shutting down.</summary>
		GoingAway = 1001,
		/// <summary>An endpoint closed the connection because of a protocol error.</summary>
		ProtocolError = 1002,
		/// <summary>An endpoint received a type of data it cannot accept.</summary>
		UnsupportedData = 1003,
		/// <summary>No status code was present in the close frame. This reserved code must not be sent in a close frame.</summary>
		NoStatusReceived = 1005,
		/// <summary>The connection closed abnormally, without a close frame. This reserved code must not be sent in a close frame.</summary>
		AbnormalClosure = 1006,
		/// <summary>The payload was inconsistent with the message type, such as non-UTF-8 text.</summary>
		InvalidFramePayloadData = 1007,
		/// <summary>An endpoint closed the connection because a message violated its policy.</summary>
		PolicyViolation = 1008,
		/// <summary>An endpoint received a message too large to process.</summary>
		MessageTooBig = 1009,
		/// <summary>The client expected an extension that the server did not negotiate.</summary>
		MandatoryExtension = 1010,
		/// <summary>The server encountered an unexpected condition while processing the request.</summary>
		InternalServerError = 1011,
		/// <summary>The TLS handshake failed. This reserved code must not be sent in a close frame.</summary>
		TlsHandshake = 1015,
	}

	// this maps to `nw_ws_version_t` in Network.framework/Headers/ws_options.h (and not the enum from NetworkExtension)
	/// <summary>Identifies the WebSocket protocol version used by Network framework options.</summary>
	[MacCatalyst (13, 1)]
	public enum NWWebSocketVersion {
		/// <summary>An invalid WebSocket protocol version.</summary>
		Invalid = 0,
		/// <summary>WebSocket protocol version 13, defined by RFC 6455.</summary>
		Version13 = 1,
	}

	/// <summary>Indicates whether a WebSocket server accepts a connection request.</summary>
	[MacCatalyst (13, 1)]
	public enum NWWebSocketResponseStatus {
		/// <summary>An invalid response status.</summary>
		Invalid = 0,
		/// <summary>Accept the request and begin framing WebSocket data.</summary>
		Accept = 1,
		/// <summary>Reject the request and close the connection.</summary>
		Reject = 2,
	}

	/// <summary>Identifies the protocol used to resolve an endpoint in a resolution report.</summary>
	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	public enum NWReportResolutionProtocol {
		/// <summary>The resolution protocol is unknown or not applicable.</summary>
		Unknown = 0,
		/// <summary>DNS over UDP.</summary>
		Udp = 1,
		/// <summary>DNS over TCP.</summary>
		Tcp = 2,
		/// <summary>DNS over TLS.</summary>
		Tls = 3,
		/// <summary>DNS over HTTPS.</summary>
		Https = 4,
	}

	/// <summary>Selects the encrypted DNS protocol for a resolver configuration.</summary>
	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum NWResolverConfigEndpointType {
		/// <summary>Use a URL endpoint for DNS over HTTPS.</summary>
		Https,
		/// <summary>Use a server endpoint for DNS over TLS.</summary>
		Tls,
	}

	/// <summary>Identifies the Multipath TCP (MPTCP) protocol version.</summary>
	[TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum NWMultipathVersion {
		/// <summary>No specific MPTCP version is selected.</summary>
		Unspecified = -1,
		/// <summary>MPTCP version 0.</summary>
		Version0 = 0,
		/// <summary>MPTCP version 1.</summary>
		Version1 = 1,
	}

	/// <summary>Identifies the radio technology used by a network interface.</summary>
	[TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum NWInterfaceRadioType {
		/// <summary>The radio technology is unknown.</summary>
		Unknown = 0,
		/// <summary>Wi-Fi using IEEE 802.11b.</summary>
		WifiB = 1,
		/// <summary>Wi-Fi using IEEE 802.11a.</summary>
		WifiA = 2,
		/// <summary>Wi-Fi using IEEE 802.11g.</summary>
		WifiG = 3,
		/// <summary>Wi-Fi using IEEE 802.11n.</summary>
		WifiN = 4,
		/// <summary>Wi-Fi using IEEE 802.11ac.</summary>
		WifiAC = 5,
		/// <summary>Wi-Fi using IEEE 802.11ax.</summary>
		WifiAX = 6,

		/// <summary>Cellular using 4G LTE.</summary>
		CellLte = 0x80,
		/// <summary>Cellular using LTE and 5G New Radio in the sub-6 GHz band.</summary>
		CellEndcSub6 = 0x81,
		/// <summary>Cellular using LTE and 5G New Radio in the millimeter-wave band.</summary>
		CellEndcMmw = 0x82,
		/// <summary>Cellular using standalone 5G New Radio in the sub-6 GHz band.</summary>
		CellNrSaSub6 = 0x83,
		/// <summary>Cellular using standalone 5G New Radio in the millimeter-wave band.</summary>
		CellNrSaMmw = 0x84,
		/// <summary>Cellular using 3G WCDMA.</summary>
		CellWcdma = 0x85,
		/// <summary>Cellular using 2G GSM.</summary>
		CellGsm = 0x86,
		/// <summary>Cellular using CDMA 1x data.</summary>
		CellCdma = 0x87,
		/// <summary>Cellular using EV-DO high-data-rate service.</summary>
		CellEvdo = 0x88,
	}

	/// <summary>Identifies who selected the network content accessed using these parameters.</summary>
	[TV (15, 0), iOS (15, 0)]
	[MacCatalyst (15, 0)]
	public enum NWParametersAttribution {
		/// <summary>The developer selected the content.</summary>
		Developer = 1,
		/// <summary>The user selected the content.</summary>
		User = 2,
	}

	/// <summary>Identifies the direction or datagram nature of a QUIC stream.</summary>
	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	public enum NWQuicStreamType {
		/// <summary>The stream direction cannot be determined.</summary>
		Unknown = 0,
		/// <summary>Data can flow in both directions.</summary>
		Bidirectional = 1,
		/// <summary>Data flows in one direction.</summary>
		Unidirectional = 2,
		/// <summary>A QUIC datagram stream.</summary>
		[TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		Datagram = 3,
	}

	[NativeName ("nw_link_quality_t")]
#if XAMCORE_5_0
	public enum NWLinkQuality : byte {
#else
	public enum NWLinkQuality : uint {
#endif
		Unknown = 0,
		Minimal = 10,
		Moderate = 20,
		Good = 30,
	}
}
