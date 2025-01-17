// Copyright 2014-2015 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

using System;

#if NET
#if IOS && !__MACCATALYST__
using AccessorySetupKit;
#else
using ASAccessory = Foundation.NSObject;
#endif
#endif
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Security;
using Network;
using OS_nw_parameters = System.IntPtr;
using OS_nw_interface = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if MONOMAC || TVOS
using NEHotspotHelperOptions = Foundation.NSObject;
#endif

#if MONOMAC
using NEHotspotHelperResult = Foundation.NSObject;
using NEHotspotHelperCommandType = Foundation.NSObject;
using NEHotspotHelperConfidence = Foundation.NSObject;
#endif

namespace NetworkExtension {
	/// <summary>Enumerates errors relating to a DNS proxy.</summary>
	[NoTV]
	[ErrorDomain ("NEDNSProxyErrorDomain")]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEDnsProxyManagerError : long {
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4,
	}

	/// <summary>Enumerates the actions to be taken when a <see cref="T:NetworkExtension.NEFilterVerdict" /> is that a report should be made.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEFilterAction : long {
		Invalid = 0,
		Allow = 1,
		Drop = 2,
		Remediate = 3,
		FilterData = 4,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NEVpnIkev2TlsVersion : long {
		Default = 0,
		Tls1_0 = 1,
		Tls1_1 = 2,
		Tls1_2 = 3,
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEHotspotConfigurationEapType : long {
		Tls = 13,
		Ttls = 21,
		Peap = 25,
		Fast = 43,
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEHotspotConfigurationTtlsInnerAuthenticationType : long {
		Pap = 0,
		Chap = 1,
		MSChap = 2,
		MSChapv2 = 3,
		Eap = 4,
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEHotspotConfigurationEapTlsVersion : long {
		Tls1_0 = 0,
		Tls1_1 = 1,
		Tls1_2 = 2,
	}

	/// <summary>Enumerates errors relating to network hostpot configuration.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	[ErrorDomain ("NEHotspotConfigurationErrorDomain")]
	public enum NEHotspotConfigurationError : long {
		Invalid = 0,
		InvalidSsid = 1,
		InvalidWpaPassphrase = 2,
		InvalidWepPassphrase = 3,
		InvalidEapSettings = 4,
		InvalidHS20Settings = 5,
		InvalidHS20DomainName = 6,
		UserDenied = 7,
		Internal = 8,
		Pending = 9,
		SystemConfiguration = 10,
		Unknown = 11,
		JoinOnceNotSupported = 12,
		AlreadyAssociated = 13,
		ApplicationIsNotInForeground = 14,
		InvalidSsidPrefix = 15,
		UserUnauthorized = 16,
		SystemDenied = 17,
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Native]
	enum NEFilterManagerGrade : long {
		Firewall = 1,
		Inspector = 2,
	}

	[NoTV]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NETrafficDirection : long {
		Any = 0,
		Inbound = 1,
		Outbound = 2,
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Native]
	enum NENetworkRuleProtocol : long {
		Any = 0,
		Tcp = 1,
		Udp = 2,
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Native]
	enum NEFilterPacketProviderVerdict : long {
		Allow = 0,
		Drop = 1,
		Delay = 2,
	}

	[NoTV]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NEFilterReportEvent : long {
		NewFlow = 1,
		DataDecision = 2,
		FlowClosed = 3,
		[NoiOS]
		[NoMacCatalyst]
		Statistics = 4,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	enum NEFilterReportFrequency : long {
		None,
		Low,
		Medium,
		High,
	}

	[NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum NEFilterDataAttribute : long {
		HasIpHeader = 1,
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	enum NEHotspotNetworkSecurityType : long {
		Open = 0,
		Wep = 1,
		Personal = 2,
		Enterprise = 3,
		Unknown = 4,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (17, 0)]
	[Native]
	[ErrorDomain ("NEVPNConnectionErrorDomain")]
	public enum NEVpnConnectionError : long {
		Overslept = 1,
		NoNetworkAvailable = 2,
		UnrecoverableNetworkChange = 3,
		ConfigurationFailed = 4,
		ServerAddressResolutionFailed = 5,
		ServerNotResponding = 6,
		ServerDead = 7,
		AuthenticationFailed = 8,
		ClientCertificateInvalid = 9,
		ClientCertificateNotYetValid = 10,
		ClientCertificateExpired = 11,
		PluginFailed = 12,
		ConfigurationNotFound = 13,
		PluginDisabled = 14,
		NegotiationFailed = 15,
		ServerDisconnected = 16,
		ServerCertificateInvalid = 17,
		ServerCertificateNotYetValid = 18,
		ServerCertificateExpired = 19,
	}

	delegate void NEAppProxyFlowOpenCallback ([NullAllowed] NSError completionHandler);

	/// <summary>Provides IO over a network socket.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppProxyFlowClassRef/index.html">Apple documentation for <c>NEAppProxyFlow</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract] // documented as such and ...
	[DisableDefaultCtor] // can't be created (with `init`) without crashing introspection tests
	interface NEAppProxyFlow {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'OpenWithLocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'OpenWithLocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'OpenWithLocalFlowEndpoint' instead.")]
		[Export ("openWithLocalEndpoint:completionHandler:")]
		[Async]
		void OpenWithLocalEndpoint ([NullAllowed] NWHostEndpoint localEndpoint, Action<NSError> completionHandler);

		[Export ("closeReadWithError:")]
		void CloseRead ([NullAllowed] NSError error);

		[Export ("closeWriteWithError:")]
		void CloseWrite ([NullAllowed] NSError error);

		[Internal]
		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("setMetadata:")]
		void SetMetadata (OS_nw_parameters nwparameters);

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Wrap ("SetMetadata (parameters.GetHandle ())")]
		void SetMetadata (NWParameters parameters);

		[Export ("metaData")]
		NEFlowMetaData MetaData { get; }

		[Internal]
		[NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("networkInterface", ArgumentSemantic.Copy)]
		OS_nw_interface WeakNetworkInterface { get; set; }

		[NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		NWInterface NetworkInterface {
			[Wrap ("Runtime.GetINativeObject<NWInterface> (WeakNetworkInterface, false)!")]
			get;
			[Wrap ("WeakNetworkInterface = value.GetHandle ()")]
			set;
		}

		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("remoteHostname")]
		[NullAllowed]
		string RemoteHostname { get; }

		[iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Export ("isBound")]
		bool IsBound { get; }

#if !NET
		[Field ("NEAppProxyErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Async]
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("openWithLocalFlowEndpoint:completionHandler:")]
		void OpenWithLocalFlowEndpoint ([NullAllowed] Network.NWEndpoint localEndpoint, NEAppProxyFlowOpenCallback completionHandler);
	}

	/// <summary>Provides sockets by creating <see cref="T:NetworkExtension.NEAppProxyFlow" /> objects.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppProxyProviderClassRef/index.html">Apple documentation for <c>NEAppProxyProvider</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NETunnelProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEAppProxyProvider {
		[Export ("startProxyWithOptions:completionHandler:")]
		[Async]
		void StartProxy ([NullAllowed] NSDictionary<NSString, NSObject> options, Action<NSError> completionHandler);

		[Export ("stopProxyWithReason:completionHandler:")]
		[Async]
		void StopProxy (NEProviderStopReason reason, Action completionHandler);

		[Export ("cancelProxyWithError:")]
		void CancelProxy ([NullAllowed] NSError error);

		[Export ("handleNewFlow:")]
		bool HandleNewFlow (NEAppProxyFlow flow);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("handleNewUDPFlow:initialRemoteEndpoint:")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Call 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Call 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Call 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		bool HandleNewUdpFlow (NEAppProxyUdpFlow flow, NWEndpoint remoteEndpoint);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("handleNewUDPFlow:initialRemoteFlowEndpoint:")]
		bool HandleNewUdpFlowWithInitialFlowEndPoint (NEAppProxyUdpFlow flow, Network.NWEndpoint remoteEndpoint);
	}

	/// <summary>Configures and controls network tunnels received from an App Proxy Provider extension.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppProxyProviderManagerClassRef/index.html">Apple documentation for <c>NEAppProxyProviderManager</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NETunnelProviderManager))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEAppProxyProviderManager {
		[Static]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadAllFromPreferences (Action<NSArray, NSError> completionHandler);
	}

	/// <summary>Provides IO over a TCP socket.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppProxyTCPFlowClassRef/index.html">Apple documentation for <c>NEAppProxyTCPFlow</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEAppProxyFlow), Name = "NEAppProxyTCPFlow")]
	[DisableDefaultCtor]
	interface NEAppProxyTcpFlow {
		[Export ("readDataWithCompletionHandler:")]
		[Async]
		void ReadData (Action<NSData, NSError> completionHandler);

		[Export ("writeData:withCompletionHandler:")]
		[Async]
		void WriteData (NSData data, Action<NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Call 'RemoveFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Call 'RemoveFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Call 'RemoveFlowEndpoint' instead.")]
		[Export ("remoteEndpoint")]
		NWEndpoint RemoteEndpoint { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("remoteFlowEndpoint")]
		Network.NWEndpoint RemoteFlowEndpoint { get; }
	}

	delegate void NEDatagramRead (NSData [] datagrams, NWEndpoint [] remoteEndpoints, NSError error);
	delegate void NEDatagramAndFlowEndpointsRead ([NullAllowed] NSData [] datagrams, [NullAllowed] Network.NWEndpoint [] remoteEndpoints, [NullAllowed] NSError error);
	delegate void NEDatagramWriteResult ([NullAllowed] NSError error);
	/// <summary>Provides IO over a UDP socket.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppProxyUDPFlowClassRef/index.html">Apple documentation for <c>NEAppProxyUDPFlow</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEAppProxyFlow), Name = "NEAppProxyUDPFlow")]
	[DisableDefaultCtor]
	interface NEAppProxyUdpFlow {
		[Export ("readDatagramsWithCompletionHandler:")]
		[Async (ResultTypeName = "NEDatagramReadResult")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'ReadDatagramsAndFlowEndpoints' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'ReadDatagramsAndFlowEndpoints' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'ReadDatagramsAndFlowEndpoints' instead.")]
		void ReadDatagrams (NEDatagramRead completionHandler);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'WriteDatagramsAndFlowEndpoints' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'WriteDatagramsAndFlowEndpoints' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'WriteDatagramsAndFlowEndpoints' instead.")]
		[Export ("writeDatagrams:sentByEndpoints:completionHandler:")]
		[Async]
		void WriteDatagrams (NSData [] datagrams, NWEndpoint [] remoteEndpoints, Action<NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[NullAllowed, Export ("localEndpoint")]
		NWEndpoint LocalEndpoint { get; }

#if NET
		[Async (ResultTypeName = "NEDatagramAndFlowEndpointsReadResult")]
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("readDatagramsAndFlowEndpointsWithCompletionHandler:")]
		void ReadDatagramsAndFlowEndpoints (NEDatagramAndFlowEndpointsRead completionHandler);
#endif

		[Async]
		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("writeDatagrams:sentByFlowEndpoints:completionHandler:")]
		void WriteDatagramsAndFlowEndpoints (NSData [] datagrams, Network.NWEndpoint [] flowEndpoints, NEDatagramWriteResult completionHandler);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("localFlowEndpoint"), NullAllowed]
		Network.NWEndpoint LocalFlowEndpoint { get; }
	}

	/// <summary>Defines a rule that select network connections based on application signing identifier, application path, and/or hostname domain.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEAppRuleClassRef/index.html">Apple documentation for <c>NEAppRule</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEAppRule : NSSecureCoding, NSCopying {
		[MacCatalyst (13, 1)]
#if NET
		[NoMac]
#endif
		[Export ("initWithSigningIdentifier:")]
		NativeHandle Constructor (string signingIdentifier);

		[NoiOS, NoMacCatalyst]
		[Export ("initWithSigningIdentifier:designatedRequirement:")]
		NativeHandle Constructor (string signingIdentifier, string designatedRequirement);

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("matchDesignatedRequirement")]
		string MatchDesignatedRequirement { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("matchPath")]
		string MatchPath { get; set; }

		[Export ("matchSigningIdentifier")]
		string MatchSigningIdentifier { get; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }

		[NoiOS, MacCatalyst (15, 0)]
		[NullAllowed, Export ("matchTools", ArgumentSemantic.Copy)]
		NEAppRule [] MatchTools { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NEDNSSettings")]
	[DisableDefaultCtor]
	interface NEDnsSettings : NSSecureCoding, NSCopying {
		[Export ("initWithServers:")]
		NativeHandle Constructor (string [] servers);

		[Export ("servers")]
		string [] Servers { get; }

		[NullAllowed, Export ("searchDomains", ArgumentSemantic.Copy)]
		string [] SearchDomains { get; set; }

		[NullAllowed, Export ("domainName")]
		string DomainName { get; set; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }

		[Export ("matchDomainsNoSearch")]
		bool MatchDomainsNoSearch { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dnsProtocol")]
		NEDnsProtocol DnsProtocol { get; }

		[iOS (14, 0), NoTV]
		[MacCatalyst (14, 0)]
		[Notification]
		[Field ("NEDNSSettingsConfigurationDidChangeNotification")]
		NSString ConfigurationDidChangeNotification { get; }
	}

	/// <summary>Provides filter flow control information to a <see cref="T:NetworkExtension.NEFilterDataProvider" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterControlProviderClassRef/index.html">Apple documentation for <c>NEFilterControlProvider</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEFilterControlProvider {
		[NullAllowed, Export ("remediationMap", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSDictionary<NSString, NSObject>> RemediationMap { get; set; }

		[NullAllowed, Export ("URLAppendStringMap", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> UrlAppendStringMap { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("handleReport:")]
		void HandleReport (NEFilterReport report);

		[Export ("handleRemediationForFlow:completionHandler:")]
		[Async]
		void HandleRemediationForFlow (NEFilterFlow flow, Action<NEFilterControlVerdict> completionHandler);

		[Export ("handleNewFlow:completionHandler:")]
		[Async]
		void HandleNewFlow (NEFilterFlow flow, Action<NEFilterControlVerdict> completionHandler);

		[Export ("notifyRulesChanged")]
		void NotifyRulesChanged ();
	}

	/// <summary>Represents a Filter Control Provider decision about network data flow.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterControlVerdictClassRef/index.html">Apple documentation for <c>NEFilterControlVerdict</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterNewFlowVerdict))]
	interface NEFilterControlVerdict : NSSecureCoding, NSCopying {
		[Static]
		[Export ("allowVerdictWithUpdateRules:")]
		NEFilterControlVerdict AllowVerdictWithUpdateRules (bool updateRules);

		[Static]
		[Export ("dropVerdictWithUpdateRules:")]
		NEFilterControlVerdict DropVerdictWithUpdateRules (bool updateRules);

		[Static]
		[Export ("updateRules")]
		NEFilterControlVerdict UpdateRules ();
	}

	/// <summary>Filters network data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterDataProviderClassRef/index.html">Apple documentation for <c>NEFilterDataProvider</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEFilterDataProvider {
		[Export ("handleNewFlow:")]
		NEFilterNewFlowVerdict HandleNewFlow (NEFilterFlow flow);

		[Export ("handleInboundDataFromFlow:readBytesStartOffset:readBytes:")]
		NEFilterDataVerdict HandleInboundDataFromFlow (NEFilterFlow flow, nuint offset, NSData readBytes);

		[Export ("handleOutboundDataFromFlow:readBytesStartOffset:readBytes:")]
		NEFilterDataVerdict HandleOutboundDataFromFlow (NEFilterFlow flow, nuint offset, NSData readBytes);

		[Export ("handleInboundDataCompleteForFlow:")]
		NEFilterDataVerdict HandleInboundDataCompleteForFlow (NEFilterFlow flow);

		[Export ("handleOutboundDataCompleteForFlow:")]
		NEFilterDataVerdict HandleOutboundDataCompleteForFlow (NEFilterFlow flow);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("handleRemediationForFlow:")]
		NEFilterRemediationVerdict HandleRemediationForFlow (NEFilterFlow flow);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("handleRulesChanged")]
		void HandleRulesChanged ();

		[NoiOS, NoMacCatalyst]
		[Export ("applySettings:completionHandler:")]
		[Async]
		void ApplySettings ([NullAllowed] NEFilterSettings settings, Action<NSError> completionHandler);

		[NoiOS, NoMacCatalyst]
		[Export ("resumeFlow:withVerdict:")]
		void ResumeFlow (NEFilterFlow flow, NEFilterVerdict verdict);

		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("updateFlow:usingVerdict:forDirection:")]
		void UpdateFlow (NEFilterSocketFlow flow, NEFilterDataVerdict verdict, NETrafficDirection direction);
	}

	/// <summary>Represents a Filter Data Provider decision about network data flow.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterDataVerdictClassRef/index.html">Apple documentation for <c>NEFilterDataVerdict</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterVerdict))]
	interface NEFilterDataVerdict : NSSecureCoding, NSCopying {
		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("statisticsReportFrequency", ArgumentSemantic.Assign)]
		NEFilterReportFrequency StatisticsReportFrequency { get; set; }

		[Static]
		[Export ("allowVerdict")]
		NEFilterDataVerdict AllowVerdict ();

		[Static]
		[Export ("dropVerdict")]
		NEFilterDataVerdict DropVerdict ();

		[Static]
		[Export ("remediateVerdictWithRemediationURLMapKey:remediationButtonTextMapKey:")]
		NEFilterDataVerdict RemediateVerdict ([NullAllowed] string remediationUrlMapKey, [NullAllowed] string remediationButtonTextMapKey);

		[Static]
		[Export ("dataVerdictWithPassBytes:peekBytes:")]
		NEFilterDataVerdict DataVerdict (nuint passBytes, nuint peekBytes);

		[Static]
		[Export ("needRulesVerdict")]
		NEFilterDataVerdict NeedRulesVerdict ();

		[NoiOS, MacCatalyst (15, 0)]
		[Static]
		[Export ("pauseVerdict")]
		NEFilterDataVerdict PauseVerdict ();
	}

	/// <summary>Contains information about a network data flow.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterFlowClassRef/index.html">Apple documentation for <c>NEFilterFlow</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFilterFlow : NSSecureCoding, NSCopying {
		[NullAllowed, Export ("URL")]
		NSUrl Url { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sourceAppUniqueIdentifier")]
		NSData SourceAppUniqueIdentifier { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sourceAppIdentifier")]
		string SourceAppIdentifier { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sourceAppVersion")]
		string SourceAppVersion { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("direction")]
		NETrafficDirection Direction { get; }

		[NoiOS, MacCatalyst (15, 0)]
		[NullAllowed, Export ("sourceAppAuditToken")]
		NSData SourceAppAuditToken { get; }

		[Mac (13, 0), NoiOS, NoMacCatalyst, NoTV]
		[NullAllowed]
		[Export ("sourceProcessAuditToken")]
		NSData SourceProcessAuditToken { get; }

		[iOS (13, 1)]
		[MacCatalyst (13, 1)]
		[Export ("identifier")]
		NSUuid Identifier { get; }
	}

	/// <summary>Manages content filter configurations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterManagerClassRef/index.html">Apple documentation for <c>NEFilterManager</c></related>
	[NoTV]
	// according to Xcode7 SDK this was available (in parts) in iOS8
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEFilterManager {
		[Static]
		[Export ("sharedManager")]
		NEFilterManager SharedManager { get; }

		[Export ("loadFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadFromPreferences (Action<NSError> completionHandler);

		[Export ("removeFromPreferencesWithCompletionHandler:")]
		[Async]
		void RemoveFromPreferences (Action<NSError> completionHandler);

		[Export ("saveToPreferencesWithCompletionHandler:")]
		[Async]
		void SaveToPreferences (Action<NSError> completionHandler);

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Strong)]
		NEFilterProviderConfiguration ProviderConfiguration { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Field ("NEFilterConfigurationDidChangeNotification")]
		[Notification]
		NSString ConfigurationDidChangeNotification { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("grade", ArgumentSemantic.Assign)]
		NEFilterManagerGrade Grade { get; set; }

#if !NET
		[Field ("NEFilterErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[NoTV, Mac (15, 0), NoiOS, MacCatalyst (18, 0)]
		[Export ("disableEncryptedDNSSettings", ArgumentSemantic.Assign)]
		bool DisableEncryptedDnsSettings { get; set; }
	}

	/// <summary>Represents a Filter Provider decision about network data flow the first time that the data is seen.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterNewFlowVerdictClassRef/index.html">Apple documentation for <c>NEFilterNewFlowVerdict</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterVerdict))]
	interface NEFilterNewFlowVerdict : NSSecureCoding, NSCopying {
		[NoTV, NoiOS, NoMacCatalyst]
		[Export ("statisticsReportFrequency", ArgumentSemantic.Assign)]
		NEFilterReportFrequency StatisticsReportFrequency { get; set; }

		[Static]
		[Export ("needRulesVerdict")]
		NEFilterNewFlowVerdict NeedRulesVerdict ();

		[Static]
		[Export ("allowVerdict")]
		NEFilterNewFlowVerdict AllowVerdict ();

		[Static]
		[Export ("dropVerdict")]
		NEFilterNewFlowVerdict DropVerdict ();

		[Static]
		[Export ("remediateVerdictWithRemediationURLMapKey:remediationButtonTextMapKey:")]
		NEFilterNewFlowVerdict RemediateVerdict (string remediationUrlMapKey, string remediationButtonTextMapKey);

		[Static]
		[Export ("URLAppendStringVerdictWithMapKey:")]
		NEFilterNewFlowVerdict UrlAppendStringVerdict (string urlAppendMapKey);

		[Static]
		[Export ("filterDataVerdictWithFilterInbound:peekInboundBytes:filterOutbound:peekOutboundBytes:")]
		NEFilterNewFlowVerdict FilterDataVerdict (bool filterInbound, nuint peekInboundBytes, bool filterOutbound, nuint peekOutboundBytes);

		[NoiOS, MacCatalyst (15, 0)]
		[Static]
		[Export ("pauseVerdict")]
		NEFilterDataVerdict PauseVerdict ();
	}

	/// <summary>Base class for data and control filter providers.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterProviderClassRef/index.html">Apple documentation for <c>NEFilterProvider</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEProvider))]
	[Abstract] // documented as such
	interface NEFilterProvider {
		[Export ("startFilterWithCompletionHandler:")]
		[Async]
		void StartFilter (Action<NSError> completionHandler);

		[Export ("stopFilterWithReason:completionHandler:")]
		[Async]
		void StopFilter (NEProviderStopReason reason, Action completionHandler);

		[iOS (13, 0)] // new in this (base) type
		[MacCatalyst (13, 1)]
		[Export ("handleReport:")]
		void HandleReport (NEFilterReport report);

		[Export ("filterConfiguration")]
		NEFilterProviderConfiguration FilterConfiguration { get; }

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#endif
		[Field ("NEFilterProviderRemediationMapRemediationButtonTexts")]
		NSString RemediationMapRemediationButtonTexts { get; }

#if NET
		[NoMac]
		[MacCatalyst (13, 1)]
#endif
		[Field ("NEFilterProviderRemediationMapRemediationURLs")]
		NSString RemediationMapRemediationUrls { get; }
	}

	/// <summary>Contains Filter Provider configuration data.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterProviderConfigurationClassRef/index.html">Apple documentation for <c>NEFilterProviderConfiguration</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFilterProviderConfiguration : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Not supported on the platform.")]
		[Export ("filterBrowsers")]
		bool FilterBrowsers { get; set; }

		[Export ("filterSockets")]
		bool FilterSockets { get; set; }

		[NullAllowed, Export ("vendorConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> VendorConfiguration { get; set; }

		[NullAllowed, Export ("serverAddress")]
		string ServerAddress { get; set; }

		[NullAllowed, Export ("username")]
		string Username { get; set; }

		[NullAllowed, Export ("organization")]
		string Organization { get; set; }

		[NullAllowed, Export ("passwordReference", ArgumentSemantic.Copy)]
		NSData PasswordReference { get; set; }

		[NullAllowed, Export ("identityReference", ArgumentSemantic.Copy)]
		NSData IdentityReference { get; set; }

		[NoiOS]
		[MacCatalyst (15, 0)]
		[Export ("filterPackets")]
		bool FilterPackets { get; set; }

		[NoiOS]
		[MacCatalyst (15, 0)]
		[NullAllowed, Export ("filterDataProviderBundleIdentifier")]
		string FilterDataProviderBundleIdentifier { get; set; }

		[NoiOS]
		[MacCatalyst (15, 0)]
		[NullAllowed, Export ("filterPacketProviderBundleIdentifier")]
		string FilterPacketProviderBundleIdentifier { get; set; }
	}

	/// <summary>Represents a Filter Data Provider decision about network data flow after the user requests access.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterRemediationVerdictClassRef/index.html">Apple documentation for <c>NEFilterRemediationVerdict</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterVerdict))]
	interface NEFilterRemediationVerdict : NSSecureCoding, NSCopying {
		[Static]
		[Export ("allowVerdict")]
		NEFilterRemediationVerdict AllowVerdict ();

		[Static]
		[Export ("dropVerdict")]
		NEFilterRemediationVerdict DropVerdict ();

		[Static]
		[Export ("needRulesVerdict")]
		NEFilterRemediationVerdict NeedRulesVerdict ();
	}

	/// <summary>Base class for content filter verdicts.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterVerdictClassRef/index.html">Apple documentation for <c>NEFilterVerdict</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFilterVerdict : NSSecureCoding, NSCopying {
		[MacCatalyst (13, 1)]
		[Export ("shouldReport")]
		bool ShouldReport { get; set; }
	}

	/// <summary>Contains additional data about a network data flow.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFlowMetaDataClassRef/index.html">Apple documentation for <c>NEFlowMetaData</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFlowMetaData : NSCopying, NSSecureCoding {
		[Export ("sourceAppUniqueIdentifier")]
		NSData SourceAppUniqueIdentifier { get; }

		[Export ("sourceAppSigningIdentifier")]
		string SourceAppSigningIdentifier { get; }

		[NoiOS]
		[MacCatalyst (15, 0)]
		[NullAllowed, Export ("sourceAppAuditToken")]
		NSData SourceAppAuditToken { get; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[NullAllowed, Export ("filterFlowIdentifier")]
		NSUuid FilterFlowIdentifier { get; }
	}

	/// <summary>A command handler block for a Hotspot Helper.</summary>
	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void NEHotspotHelperHandler (NEHotspotHelperCommand cmd);

	/// <summary>Enables registration for Wi-Fi hotspots.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEHotspotHelperClassRef/index.html">Apple documentation for <c>NEHotspotHelper</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelper {
		[Static]
		[Internal]
		[Export ("registerWithOptions:queue:handler:")]
		bool Register ([NullAllowed] NSDictionary options, DispatchQueue queue, NEHotspotHelperHandler handler);

		[Static]
		[Wrap ("Register (options.GetDictionary (), queue, handler)")]
		bool Register ([NullAllowed] NEHotspotHelperOptions options, DispatchQueue queue, NEHotspotHelperHandler handler);

		[Static]
		[Export ("logoff:")]
		bool Logoff (NEHotspotNetwork network);

		[Static, NullAllowed]
		[Export ("supportedNetworkInterfaces")]
		NEHotspotNetwork [] SupportedNetworkInterfaces { get; }
	}

	/// <summary>Holds the name of the network handled by the applications.</summary>
	[NoTV]
	[Static]
	[NoMac]
	[MacCatalyst (13, 1)]
	interface NEHotspotHelperOptionInternal {
		[Field ("kNEHotspotHelperOptionDisplayName")]
		NSString DisplayName { get; }
	}

	/// <summary>A reusable request for registering Wi-Fi hotspots.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSMutableUrlRequest))]
	interface NSMutableURLRequest_NEHotspotHelper {
		[Export ("bindToHotspotHelperCommand:")]
		void BindTo (NEHotspotHelperCommand command);
	}

	/// <summary>Represents commands to a Hotspot Helper.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEHotspotHelperCommandClassRef/index.html">Apple documentation for <c>NEHotspotHelperCommand</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelperCommand {
		[Export ("commandType")]
		NEHotspotHelperCommandType CommandType { get; }

		[NullAllowed, Export ("network")]
		NEHotspotNetwork Network { get; }

		[NullAllowed, Export ("networkList")]
		NEHotspotNetwork [] NetworkList { get; }

		[Export ("createResponse:")]
		NEHotspotHelperResponse CreateResponse (NEHotspotHelperResult result);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'Interface' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'Interface' property instead.")]
		[Export ("createTCPConnection:")]
		NWTcpConnection CreateTcpConnection (NWEndpoint endpoint);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'Interface' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'Interface' property instead.")]
		[Export ("createUDPSession:")]
		NWUdpSession CreateUdpSession (NWEndpoint endpoint);

		[NoTV, NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("interface")]
		NWInterface Interface { get; }
	}

	/// <summary>Contains a Hotspot Helper command response.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEHotspotHelperResponseClassRef/index.html">Apple documentation for <c>NEHotspotHelperResponse</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelperResponse {
		[Export ("setNetwork:")]
		void SetNetwork (NEHotspotNetwork network);

		[Export ("setNetworkList:")]
		void SetNetworkList (NEHotspotNetwork [] networkList);

		[Export ("deliver")]
		void Deliver ();
	}

	/// <summary>Contains information about a Wi-Fi network hotspot.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEHotspotNetworkClassRef/index.html">Apple documentation for <c>NEHotspotNetwork</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotNetwork {
		[Export ("SSID")]
		string Ssid { get; }

		[Export ("BSSID")]
		string Bssid { get; }

		[Export ("signalStrength")]
		double SignalStrength { get; }

		[Export ("secure")]
		bool Secure { [Bind ("isSecure")] get; }

		[Export ("autoJoined")]
		bool AutoJoined { [Bind ("didAutoJoin")] get; }

		[Export ("justJoined")]
		bool JustJoined { [Bind ("didJustJoin")] get; }

		[Export ("chosenHelper")]
		bool ChosenHelper { [Bind ("isChosenHelper")] get; }

		[Export ("setConfidence:")]
		void SetConfidence (NEHotspotHelperConfidence confidence);

		[Export ("setPassword:")]
		void SetPassword (string password);

		[Async]
		[NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("fetchCurrentWithCompletionHandler:")]
		void FetchCurrent (Action<NEHotspotNetwork> completionHandler);

		[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("securityType")]
		NEHotspotNetworkSecurityType SecurityType { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEIPv4Route : NSSecureCoding, NSCopying {
		[Export ("initWithDestinationAddress:subnetMask:")]
		NativeHandle Constructor (string address, string subnetMask);

		[Export ("destinationAddress")]
		string DestinationAddress { get; }

		[Export ("destinationSubnetMask")]
		string DestinationSubnetMask { get; }

		[NullAllowed, Export ("gatewayAddress")]
		string GatewayAddress { get; set; }

		[Static]
		[Export ("defaultRoute")]
		NEIPv4Route DefaultRoute { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEIPv6Route : NSSecureCoding, NSCopying {
		[Export ("initWithDestinationAddress:networkPrefixLength:")]
		NativeHandle Constructor (string address, NSNumber networkPrefixLength);

		[Export ("destinationAddress")]
		string DestinationAddress { get; }

		[Export ("destinationNetworkPrefixLength")]
		NSNumber DestinationNetworkPrefixLength { get; }

		[NullAllowed, Export ("gatewayAddress")]
		string GatewayAddress { get; set; }

		[Static]
		[Export ("defaultRoute")]
		NEIPv6Route DefaultRoute { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEIPv4Settings : NSSecureCoding, NSCopying {
		[Export ("initWithAddresses:subnetMasks:")]
		NativeHandle Constructor (string [] addresses, string [] subnetMasks);

		[Export ("addresses")]
		string [] Addresses { get; }

		[Export ("subnetMasks")]
		string [] SubnetMasks { get; }

		[NullAllowed, Export ("includedRoutes", ArgumentSemantic.Copy)]
		NEIPv4Route [] IncludedRoutes { get; set; }

		[NullAllowed, Export ("excludedRoutes", ArgumentSemantic.Copy)]
		NEIPv4Route [] ExcludedRoutes { get; set; }

		[NoTV, NoiOS, Mac (13, 0)]
		[NoMacCatalyst]
		[Export ("router")]
		[NullAllowed]
		string Router { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEIPv6Settings : NSSecureCoding, NSCopying {
		[Export ("initWithAddresses:networkPrefixLengths:")]
		NativeHandle Constructor (string [] addresses, NSNumber [] networkPrefixLengths);

		[Export ("addresses")]
		string [] Addresses { get; }

		[Export ("networkPrefixLengths")]
		NSNumber [] NetworkPrefixLengths { get; }

		[NullAllowed, Export ("includedRoutes", ArgumentSemantic.Copy)]
		NEIPv6Route [] IncludedRoutes { get; set; }

		[NullAllowed, Export ("excludedRoutes", ArgumentSemantic.Copy)]
		NEIPv6Route [] ExcludedRoutes { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init returns nil
	interface NEProvider {
		[Export ("sleepWithCompletionHandler:")]
		[Async]
		void Sleep (Action completionHandler);

		[Export ("wake")]
		void Wake ();

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Export ("createTCPConnectionToEndpoint:enableTLS:TLSParameters:delegate:")]
		NWTcpConnection CreateTcpConnectionToEndpoint (NWEndpoint remoteEndpoint, bool enableTLS, [NullAllowed] NWTlsParameters TLSParameters, [NullAllowed] NSObject connectionDelegate);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
		[Export ("createUDPSessionToEndpoint:fromEndpoint:")]
		NWUdpSession CreateUdpSessionToEndpoint (NWEndpoint remoteEndpoint, [NullAllowed] NWHostEndpoint localEndpoint);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPathMonitor' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPathMonitor' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPathMonitor' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPathMonitor' instead.")]
		[NullAllowed, Export ("defaultPath")]
		NWPath DefaultPath { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 12, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("displayMessage:completionHandler:")]
		[Async]
		void DisplayMessage (string message, Action<bool> completionHandler);

		[NoiOS]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("startSystemExtensionMode")]
		void StartSystemExtensionMode ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEProxySettings : NSSecureCoding, NSCopying {
		[Export ("autoProxyConfigurationEnabled")]
		bool AutoProxyConfigurationEnabled { get; set; }

		[NullAllowed, Export ("proxyAutoConfigurationURL", ArgumentSemantic.Copy)]
		NSUrl ProxyAutoConfigurationUrl { get; set; }

		[NullAllowed, Export ("proxyAutoConfigurationJavaScript")]
		string ProxyAutoConfigurationJavaScript { get; set; }

		[Export ("HTTPEnabled")]
		bool HttpEnabled { get; set; }

		[NullAllowed, Export ("HTTPServer", ArgumentSemantic.Copy)]
		NEProxyServer HttpServer { get; set; }

		[Export ("HTTPSEnabled")]
		bool HttpsEnabled { get; set; }

		[NullAllowed, Export ("HTTPSServer", ArgumentSemantic.Copy)]
		NEProxyServer HttpsServer { get; set; }

		[Export ("excludeSimpleHostnames")]
		bool ExcludeSimpleHostnames { get; set; }

		[NullAllowed, Export ("exceptionList", ArgumentSemantic.Copy)]
		string [] ExceptionList { get; set; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEProxyServer : NSSecureCoding, NSCopying {
		[Export ("initWithAddress:port:")]
		NativeHandle Constructor (string address, nint port);

		[Export ("address")]
		string Address { get; }

		[Export ("port")]
		nint Port { get; }

		[Export ("authenticationRequired")]
		bool AuthenticationRequired { get; set; }

		[NullAllowed, Export ("username")]
		string Username { get; set; }

		[NullAllowed, Export ("password")]
		string Password { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NETunnelNetworkSettings : NSSecureCoding, NSCopying {
		[Export ("initWithTunnelRemoteAddress:")]
		NativeHandle Constructor (string address);

		[Export ("tunnelRemoteAddress")]
		string TunnelRemoteAddress { get; }

		[NullAllowed, Export ("DNSSettings", ArgumentSemantic.Copy)]
		NEDnsSettings DnsSettings { get; set; }

		[NullAllowed, Export ("proxySettings", ArgumentSemantic.Copy)]
		NEProxySettings ProxySettings { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEProvider))]
	[DisableDefaultCtor] // init returns nil
	interface NETunnelProvider {
		[Export ("handleAppMessage:completionHandler:")]
		[Async]
		void HandleAppMessage (NSData messageData, [NullAllowed] Action<NSData> completionHandler);

		[Export ("setTunnelNetworkSettings:completionHandler:")]
		[Async]
		void SetTunnelNetworkSettings ([NullAllowed] NETunnelNetworkSettings tunnelNetworkSettings, [NullAllowed] Action<NSError> completionHandler);

		[Export ("protocolConfiguration")]
		NEVpnProtocol ProtocolConfiguration { get; }

		[NoTV]
		[NullAllowed, Export ("appRules")]
		NEAppRule [] AppRules { get; }

		[NoTV]
		[Export ("routingMethod")]
		NETunnelProviderRoutingMethod RoutingMethod { get; }

		[Export ("reasserting")]
		bool Reasserting { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnManager))]
	interface NETunnelProviderManager {
		[Static]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadAllFromPreferences (Action<NSArray, NSError> completionHandler);

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Static]
		[Export ("forPerAppVPN")]
		NETunnelProviderManager CreatePerAppVpn ();

		[NoTV]
		[return: NullAllowed]
		[Export ("copyAppRules")]
		NEAppRule [] CopyAppRules ();

		// CopyAppRules was incorrectly bound to AppRules and it is only available on macOS
#if NET || MONOMAC || __MACCATALYST__
		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("appRules", ArgumentSemantic.Copy)]
		NEAppRule [] AppRules { get; set; }
#else
		[NoTV]
		[Obsolete ("Use 'CopyAppRules' instead, this property will be removed in the future.")]
		NEAppRule [] AppRules { [Wrap ("CopyAppRules ()!", IsVirtual = true)] get; }
#endif

		[NoTV]
		[Export ("routingMethod")]
		NETunnelProviderRoutingMethod RoutingMethod { get; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("safariDomains", ArgumentSemantic.Copy)]
		string [] SafariDomains { get; set; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("mailDomains", ArgumentSemantic.Copy)]
		string [] MailDomains { get; set; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("calendarDomains", ArgumentSemantic.Copy)]
		string [] CalendarDomains { get; set; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("contactsDomains", ArgumentSemantic.Copy)]
		string [] ContactsDomains { get; set; }

#if !NET
		[Field ("NETunnelProviderErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("excludedDomains", ArgumentSemantic.Copy)]
		string [] ExcludedDomains { get; set; }

		[NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("associatedDomains", ArgumentSemantic.Copy)]
		string [] AssociatedDomains { get; set; }
	}


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NEVPNManager")]
	[DisableDefaultCtor] // Assertion failed: (0), function -[NEVPNManager init], file /SourceCache/NetworkExtension_Sim/NetworkExtension-168.1.8/Framework/NEVPNManager.m, line 41.
	interface NEVpnManager {

		[NullAllowed]
		[Export ("onDemandRules", ArgumentSemantic.Copy)]
		NEOnDemandRule [] OnDemandRules { get; set; }

		[Export ("onDemandEnabled")]
		bool OnDemandEnabled { [Bind ("isOnDemandEnabled")] get; set; }

		[NullAllowed]
		[Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[NoTV]
		[NullAllowed]
		[Export ("protocol", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ProtocolConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'ProtocolConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ProtocolConfiguration' instead.")]
		NEVpnProtocol Protocol { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("protocolConfiguration", ArgumentSemantic.Retain)]
		NEVpnProtocol ProtocolConfiguration { get; set; }

		[Export ("connection")]
		NEVpnConnection Connection { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Static, Export ("sharedManager")]
		NEVpnManager SharedManager { get; }

		[Export ("loadFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadFromPreferences (Action<NSError> completionHandler); // nonnull !

		[Export ("removeFromPreferencesWithCompletionHandler:")]
		[Async]
		void RemoveFromPreferences ([NullAllowed] Action<NSError> completionHandler);

		[Export ("saveToPreferencesWithCompletionHandler:")]
		[Async]
		void SaveToPreferences ([NullAllowed] Action<NSError> completionHandler);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Internal]
		[Export ("setAuthorization:")]
		void _SetAuthorization (IntPtr auth);

#if !NET
		[Field ("NEVPNErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Notification]
		[Field ("NEVPNConfigurationChangeNotification")]
		NSString ConfigurationChangeNotification { get; }
	}

	[MacCatalyst (13, 1), TV (17, 0)]
	[BaseType (typeof (NSObject), Name = "NEVPNConnection")]
	interface NEVpnConnection {

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("connectedDate")]
		NSDate ConnectedDate { get; }

		[Export ("status")]
		NEVpnStatus Status { get; }

		[Export ("startVPNTunnelAndReturnError:")]
		bool StartVpnTunnel (out NSError error);

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("startVPNTunnelWithOptions:andReturnError:")]
		bool StartVpnTunnel ([NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("StartVpnTunnel (options.GetDictionary (), out error);")]
		bool StartVpnTunnel ([NullAllowed] NEVpnConnectionStartOptions options, out NSError error);

		[Export ("stopVPNTunnel")]
		void StopVpnTunnel ();

		[MacCatalyst (13, 1)]
		[Export ("manager")]
		NEVpnManager Manager { get; }

		[Notification]
		[Field ("NEVPNStatusDidChangeNotification")]
		NSString StatusDidChangeNotification { get; }

		[Async]
		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("fetchLastDisconnectErrorWithCompletionHandler:")]
		void FetchLastDisconnectError (Action<NSError> handler);
	}

	[Static]
	[Internal]
	[MacCatalyst (13, 1)]
	interface NEVpnConnectionStartOptionInternal {
		[Field ("NEVPNConnectionStartOptionPassword")]
		NSString Password { get; }

		[Field ("NEVPNConnectionStartOptionUsername")]
		NSString Username { get; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject), Name = "NEVPNProtocol")]
	interface NEVpnProtocol : NSCopying, NSSecureCoding {

		[NullAllowed] // by default this property is null
		[Export ("serverAddress")]
		string ServerAddress { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("username")]
		string Username { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("passwordReference", ArgumentSemantic.Copy)]
		NSData PasswordReference { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("identityReference", ArgumentSemantic.Copy)]
		NSData IdentityReference { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("identityData", ArgumentSemantic.Copy)]
		NSData IdentityData { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("identityDataPassword")]
		string IdentityDataPassword { get; set; }

		[Export ("disconnectOnSleep")]
		bool DisconnectOnSleep { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("proxySettings", ArgumentSemantic.Copy)]
		NEProxySettings ProxySettings { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("includeAllNetworks")]
		bool IncludeAllNetworks { get; set; }

		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("excludeLocalNetworks")]
		bool ExcludeLocalNetworks { get; set; }

		[NoTV, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("excludeCellularServices")]
		bool ExcludeCellularServices { get; set; }

		[NoTV, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("excludeAPNs")]
		bool ExcludeApns { get; set; }

		[NoTV, Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("excludeDeviceCommunication")]
		bool ExcludeDeviceCommunication { get; set; }

		[Mac (11, 0)]
		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("enforceRoutes")]
		bool EnforceRoutes { get; set; }

		[NoTV, NoMac, iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("sliceUUID", ArgumentSemantic.Copy), NullAllowed]
		string SliceUuid { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnProtocol), Name = "NEVPNProtocolIPSec")]
	interface NEVpnProtocolIpSec {

		[Export ("authenticationMethod")]
		NEVpnIkeAuthenticationMethod AuthenticationMethod { get; set; }

		[Export ("useExtendedAuthentication")]
		bool UseExtendedAuthentication { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sharedSecretReference", ArgumentSemantic.Copy)]
		NSData SharedSecretReference { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("localIdentifier")]
		string LocalIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("remoteIdentifier")]
		string RemoteIdentifier { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NEVPNIKEv2SecurityAssociationParameters")]
	interface NEVpnIke2SecurityAssociationParameters : NSSecureCoding, NSCopying {

		[Export ("encryptionAlgorithm")]
		NEVpnIke2EncryptionAlgorithm EncryptionAlgorithm { get; set; }

		[Export ("integrityAlgorithm")]
		NEVpnIke2IntegrityAlgorithm IntegrityAlgorithm { get; set; }

		[Export ("diffieHellmanGroup")]
		NEVpnIke2DiffieHellman DiffieHellmanGroup { get; set; }

		[Export ("lifetimeMinutes")]
		int LifetimeMinutes { get; set; } /* int32_t */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnProtocolIpSec), Name = "NEVPNProtocolIKEv2")]
	interface NEVpnProtocolIke2 {

		[Export ("deadPeerDetectionRate")]
		NEVpnIke2DeadPeerDetectionRate DeadPeerDetectionRate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("serverCertificateIssuerCommonName")]
		string ServerCertificateIssuerCommonName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("serverCertificateCommonName")]
		string ServerCertificateCommonName { get; set; }

		[Export ("IKESecurityAssociationParameters")]
		NEVpnIke2SecurityAssociationParameters IKESecurityAssociationParameters { get; }

		[Export ("childSecurityAssociationParameters")]
		NEVpnIke2SecurityAssociationParameters ChildSecurityAssociationParameters { get; }

		[MacCatalyst (13, 1)]
		[Export ("certificateType")]
		NEVpnIke2CertificateType CertificateType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("useConfigurationAttributeInternalIPSubnet")]
		bool UseConfigurationAttributeInternalIPSubnet { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("disableMOBIKE")]
		bool DisableMobike { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("disableRedirect")]
		bool DisableRedirect { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enablePFS")]
		bool EnablePfs { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enableRevocationCheck")]
		bool EnableRevocationCheck { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("strictRevocationCheck")]
		bool StrictRevocationCheck { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("minimumTLSVersion", ArgumentSemantic.Assign)]
		NEVpnIkev2TlsVersion MinimumTlsVersion { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumTLSVersion", ArgumentSemantic.Assign)]
		NEVpnIkev2TlsVersion MaximumTlsVersion { get; set; }

		[NoMac]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("enableFallback")]
		bool EnableFallback { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("mtu")]
		nuint Mtu { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("ppkConfiguration", ArgumentSemantic.Copy), NullAllowed]
		NEVpnIkev2PpkConfiguration PpkConfiguration { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface NEOnDemandRule : NSSecureCoding, NSCopying {

		[Export ("action")]
		NEOnDemandRuleAction Action { get; }

		[NullAllowed]
		[Export ("DNSSearchDomainMatch")]
		string [] DnsSearchDomainMatch { get; set; }

		[NullAllowed]
		[Export ("DNSServerAddressMatch")]
		string [] DnsServerAddressMatch { get; set; }

		[Export ("interfaceTypeMatch")]
		NEOnDemandRuleInterfaceType InterfaceTypeMatch { get; set; }

		[NullAllowed]
		[Export ("SSIDMatch")]
		string [] SsidMatch { get; set; }

		[NullAllowed]
		[Export ("probeURL", ArgumentSemantic.Copy)]
		NSUrl ProbeUrl { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleConnect {
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleDisconnect {
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleIgnore {
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleEvaluateConnection {

		[NullAllowed]
		[Export ("connectionRules", ArgumentSemantic.Copy)]
		NEEvaluateConnectionRule [] ConnectionRules { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEEvaluateConnectionRule : NSSecureCoding, NSCopying {

		[Export ("initWithMatchDomains:andAction:")]
		NativeHandle Constructor (string [] domains, NEEvaluateConnectionRuleAction action);

		[Export ("action")]
		NEEvaluateConnectionRuleAction Action { get; }

		[Export ("matchDomains")]
		string [] MatchDomains { get; }

		[NullAllowed]
		[Export ("useDNSServers", ArgumentSemantic.Copy)]
		string [] UseDnsServers { get; set; }

		[NullAllowed]
		[Export ("probeURL", ArgumentSemantic.Copy)]
		NSUrl ProbeUrl { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	interface NWEndpoint : NSSecureCoding, NSCopying {
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NWEndpoint))]
	[DisableDefaultCtor]
	interface NWHostEndpoint {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.Create' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.Create' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.Create' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.Create' instead.")]
		[Static]
		[Export ("endpointWithHostname:port:")]
		NWHostEndpoint Create (string hostname, string port);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.Hostname' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.Hostname' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.Hostname' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.Hostname' instead.")]
		[Export ("hostname")]
		string Hostname { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.PortNumber' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.PortNumber' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.PortNumber' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.PortNumber' instead.")]
		[Export ("port")]
		string Port { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NWEndpoint))]
	[DisableDefaultCtor]
	interface NWBonjourServiceEndpoint {

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.CreateBonjourService' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.CreateBonjourService' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.CreateBonjourService' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.CreateBonjourService' instead.")]
		[Static]
		[Export ("endpointWithName:type:domain:")]
		NWBonjourServiceEndpoint Create (string name, string type, string domain);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceName' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.BonjourServiceName' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceName' instead.")]
		[Export ("name")]
		string Name { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceType' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceType' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.BonjourServiceType' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceType' instead.")]

		[Export ("type")]
		string Type { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceDomain' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceDomain' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.BonjourServiceDomain' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.BonjourServiceDomain' instead.")]

		[Export ("domain")]
		string Domain { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NWPath {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.Status' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.Status' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.Status' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.Status' instead.")]
		[Export ("status")]
		NWPathStatus Status { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.IsExpensive' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.IsExpensive' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.IsExpensive' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.IsExpensive' instead.")]
		[Export ("expensive")]
		bool Expensive { [Bind ("isExpensive")] get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.EqualTo' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.EqualTo' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.EqualTo' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.EqualTo' instead.")]
		[Export ("isEqualToPath:")]
		bool IsEqualToPath (NWPath path);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.IsConstrained' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.IsConstrained' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.IsConstrained' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.IsConstrained' instead.")]
		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("constrained")]
		bool Constrained { [Bind ("isConstrained")] get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWTCPConnection")]
	interface NWTcpConnection {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Export ("initWithUpgradeForConnection:")]
		NativeHandle Constructor (NWTcpConnection connection);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Export ("state")]
		NWTcpConnectionState State { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Export ("viable")]
		bool Viable { [Bind ("isViable")] get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Export ("hasBetterPath")]
		bool HasBetterPath { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Export ("endpoint")]
		NWEndpoint Endpoint { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[NullAllowed, Export ("connectedPath")]
		NWPath ConnectedPath { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.EffectiveLocalEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.EffectiveLocalEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.EffectiveLocalEndpoint' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.EffectiveLocalEndpoint' instead.")]
		[NullAllowed, Export ("localAddress")]
		NWEndpoint LocalAddress { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWPath.EffectiveRemoteEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWPath.EffectiveRemoteEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWPath.EffectiveRemoteEndpoint' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWPath.EffectiveRemoteEndpoint' instead.")]
		[NullAllowed, Export ("remoteAddress")]
		NWEndpoint RemoteAddress { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWEndpoint.TxtRecord' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWEndpoint.TxtRecord' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWEndpoint.TxtRecord' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWEndpoint.TxtRecord' instead.")]
		[NullAllowed, Export ("txtRecord")]
		NSData TxtRecord { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Export ("cancel")]
		void Cancel ();

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Export ("readLength:completionHandler:")]
		[Async]
		void ReadLength (nuint length, Action<NSData, NSError> completion);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Export ("readMinimumLength:maximumLength:completionHandler:")]
		[Async]
		void ReadMinimumLength (nuint minimum, nuint maximum, Action<NSData, NSError> completion);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Export ("write:completionHandler:")]
		[Async]
		void Write (NSData data, Action<NSError> completion);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Export ("writeClose")]
		void WriteClose ();
	}

	interface INWTcpConnectionAuthenticationDelegate { }

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject), Name = "NWTCPConnectionAuthenticationDelegate")]
	interface NWTcpConnectionAuthenticationDelegate {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Export ("shouldProvideIdentityForConnection:")]
		bool ShouldProvideIdentity (NWTcpConnection connection);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetChallengeBlock' instead.")]
		[Export ("provideIdentityForConnection:completionHandler:")]
		void ProvideIdentity (NWTcpConnection connection, Action<SecIdentity, NSArray> completion);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Export ("shouldEvaluateTrustForConnection:")]
		bool ShouldEvaluateTrust (NWTcpConnection connection);


		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetVerifyBlock' instead.")]
		[Export ("evaluateTrustForConnection:peerCertificateChain:completionHandler:")]
		[Async]
		void EvaluateTrust (NWTcpConnection connection, NSArray peerCertificateChain, Action<SecTrust> completion);
		// note: it's not clear (from headers) but based on other API it's likely to accept a mix of SecIdentity
		// and SecCertificate - both *NOT* NSObject -> because of that NSArray is used above
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWTLSParameters")]
	interface NWTlsParameters {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsResumptionEnabled' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsResumptionEnabled' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetTlsResumptionEnabled' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsResumptionEnabled' instead.")]
		[NullAllowed, Export ("TLSSessionID", ArgumentSemantic.Copy)]
		NSData TlsSessionID { get; set; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.AddTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.AddTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.AddTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.AddTlsCipherSuite' instead.")]
		[NullAllowed, Export ("SSLCipherSuites", ArgumentSemantic.Copy)]
		NSSet<NSNumber> SslCipherSuites { get; set; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMinVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMinVersion' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMinVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMinVersion' instead.")]
		[Export ("minimumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MinimumSslProtocolVersion { get; set; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMaxVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMaxVersion' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMaxVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Security.SecProtocolOptions.SetTlsMaxVersion' instead.")]
		[Export ("maximumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MaximumSslProtocolVersion { get; set; }
	}

	[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection' instead.")]
	[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection' instead.")]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWUDPSession")]
	interface NWUdpSession {
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use the 'Network.NWConnection' constructor instead.")]
		[Export ("initWithUpgradeForSession:")]
		NativeHandle Constructor (NWUdpSession session);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetStateChangedHandler' instead.")]
		[Export ("state")]
		NWUdpSessionState State { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Endpoint' instead.")]
		[Export ("endpoint")]
		NWEndpoint Endpoint { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[NullAllowed, Export ("resolvedEndpoint")]
		NWEndpoint ResolvedEndpoint { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetViabilityChangedHandler' instead.")]
		[Export ("viable")]
		bool Viable { [Bind ("isViable")] get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.SetBetterPathAvailableHandler' instead.")]
		[Export ("hasBetterPath")]
		bool HasBetterPath { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[NullAllowed, Export ("currentPath")]
		NWPath CurrentPath { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.CancelCurrentEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.CurrentPath' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.CancelCurrentEndpoint' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.CancelCurrentEndpoint' instead.")]
		[Export ("tryNextResolvedEndpoint")]
		void TryNextResolvedEndpoint ();

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.MaximumDatagramSize' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.MaximumDatagramSize' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.MaximumDatagramSize' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.MaximumDatagramSize' instead.")]
		[Export ("maximumDatagramLength")]
		nuint MaximumDatagramLength { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Receive' instead.")]
		[Export ("setReadHandler:maxDatagrams:")]
		void SetReadHandler (Action<NSArray, NSError> handler, nuint maxDatagrams);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Export ("writeMultipleDatagrams:completionHandler:")]
		[Async]
		void WriteMultipleDatagrams (NSData [] datagramArray, Action<NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Send' instead.")]
		[Export ("writeDatagram:completionHandler:")]
		[Async]
		void WriteDatagram (NSData datagram, Action<NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'Network.NWConnection.Cancel' instead.")]
		[Export ("cancel")]
		void Cancel ();
	}

	/// <summary>Contains information about a WebKit browser network data flow.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterBrowserFlowClassRef/index.html">Apple documentation for <c>NEFilterBrowserFlow</c></related>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterFlow))]
	interface NEFilterBrowserFlow {

		[NullAllowed]
		[Export ("request")]
		NSUrlRequest Request { get; }

		[Export ("response")]
		[NullAllowed]
		NSUrlResponse Response { get; }

		[Export ("parentURL")]
		[NullAllowed]
		NSUrl ParentUrl { get; }
	}

	/// <summary>Contains information about network data flow over a socket.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/NetworkExtension/Reference/NEFilterSocketFlowClassRef/index.html">Apple documentation for <c>NEFilterSocketFlow</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterFlow))]
	interface NEFilterSocketFlow {
		[NullAllowed]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'RemoteFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'RemoteFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'RemoteFlowEndpoint' instead.")]
		[Export ("remoteEndpoint")]
		NWEndpoint RemoteEndpoint { get; }

		[NullAllowed]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'LocalFlowEndpoint' instead.")]
		[Export ("localEndpoint")]
		NWEndpoint LocalEndpoint { get; }

		[Export ("socketFamily")]
		int SocketFamily {
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}

		[Export ("socketType")]
		int SocketType {
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}

		[Export ("socketProtocol")]
		int SocketProtocol {
			get;
#if !NET
			[NotImplemented]
			set;
#endif
		}

		[NullAllowed]
		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("remoteHostname")]
		string RemoteHostname { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("localFlowEndpoint"), NullAllowed]
		Network.NWEndpoint LocalFlowEndpoint { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("remoteFlowEndpoint"), NullAllowed]
		Network.NWEndpoint RemoteFlowEndpoint { get; }
	}

	/// <summary>A reporting action taken on a filtered data flow.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFilterReport : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("flow")]
		NEFilterFlow Flow { get; }

		[Export ("action")]
		NEFilterAction Action { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("event")]
		NEFilterReportEvent Event { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("bytesInboundCount")]
		nuint BytesInboundCount { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("bytesOutboundCount")]
		nuint BytesOutboundCount { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NETunnelNetworkSettings))]
	[DisableDefaultCtor]
	interface NEPacketTunnelNetworkSettings {
		[Export ("initWithTunnelRemoteAddress:")]
		NativeHandle Constructor (string address);

		[Export ("IPv4Settings", ArgumentSemantic.Copy)]
		[NullAllowed]
		NEIPv4Settings IPv4Settings { get; set; }

		[Export ("IPv6Settings", ArgumentSemantic.Copy)]
		[NullAllowed]
		NEIPv6Settings IPv6Settings { get; set; }

		[Export ("tunnelOverheadBytes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber TunnelOverheadBytes { get; set; }

		[Export ("MTU", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber Mtu { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEPacketTunnelFlow {
		[Export ("readPacketsWithCompletionHandler:")]
		[Async (ResultType = typeof (NEPacketTunnelFlowReadResult))]
		void ReadPackets (Action<NSData [], NSNumber []> completionHandler);

		[Export ("writePackets:withProtocols:")]
		bool WritePackets (NSData [] packets, NSNumber [] protocols);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("readPacketObjectsWithCompletionHandler:")]
		void ReadPacketObjects (Action<NEPacket []> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("writePacketObjects:")]
		bool WritePacketObjects (NEPacket [] packets);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NETunnelProvider))]
	interface NEPacketTunnelProvider {
		[Export ("startTunnelWithOptions:completionHandler:")]
		[Async]
		void StartTunnel ([NullAllowed] NSDictionary<NSString, NSObject> options, Action<NSError> completionHandler);

		[Export ("stopTunnelWithReason:completionHandler:")]
		[Async]
		void StopTunnel (NEProviderStopReason reason, Action completionHandler);

		[Export ("cancelTunnelWithError:")]
		void CancelTunnel ([NullAllowed] NSError error);

		[Export ("packetFlow")]
		NEPacketTunnelFlow PacketFlow { get; }

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Export ("createTCPConnectionThroughTunnelToEndpoint:enableTLS:TLSParameters:delegate:")]
		NWTcpConnection CreateTcpConnection (NWEndpoint remoteEndpoint, bool enableTls, [NullAllowed] NWTlsParameters tlsParameters, [NullAllowed] INWTcpConnectionAuthenticationDelegate @delegate);

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Deprecated (PlatformName.TvOS, 18, 0, message: "Use 'NWParameters.RequiredInterface' with the 'VirtualInterface' property instead.")]
		[Export ("createUDPSessionThroughTunnelToEndpoint:fromEndpoint:")]
		NWUdpSession CreateUdpSession (NWEndpoint remoteEndpoint, [NullAllowed] NWHostEndpoint localEndpoint);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("virtualInterface"), NullAllowed]
		NWInterface VirtualInterface { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnProtocol))]
	interface NETunnelProviderProtocol {
		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ProviderConfiguration { get; set; }

		[NullAllowed, Export ("providerBundleIdentifier")]
		string ProviderBundleIdentifier { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnConnection))]
	interface NETunnelProviderSession {
		[Export ("startTunnelWithOptions:andReturnError:")]
		bool StartTunnel ([NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError error);

		[Export ("stopTunnel")]
		void StopTunnel ();

		[Export ("sendProviderMessage:returnError:responseHandler:")]
		bool SendProviderMessage (NSData messageData, [NullAllowed] out NSError error, [NullAllowed] Action<NSData> responseHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEPacket : NSCopying, NSSecureCoding {
		[Export ("initWithData:protocolFamily:")]
		NativeHandle Constructor (NSData data, /* sa_family_t */ byte protocolFamily);

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[Export ("protocolFamily")]
		byte ProtocolFamily { get; }

		[NoTV]
		[NullAllowed, Export ("metadata")]
		NEFlowMetaData Metadata { get; }

		[NoTV]
		[NoiOS]
		[MacCatalyst (15, 0)]
		[Export ("direction")]
		NETrafficDirection Direction { get; }
	}

	/// <summary>Encapsulates a DNS proxy.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "NEDNSProxyManager")]
	interface NEDnsProxyManager {

		[Notification]
		[Field ("NEDNSProxyConfigurationDidChangeNotification")]
		NSString ProxyConfigurationDidChangeNotification { get; }

		[Static]
		[Export ("sharedManager")]
		NEDnsProxyManager SharedManager { get; }

		[Async]
		[Export ("loadFromPreferencesWithCompletionHandler:")]
		void LoadFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("removeFromPreferencesWithCompletionHandler:")]
		void RemoveFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("saveToPreferencesWithCompletionHandler:")]
		void SaveToPreferences (Action<NSError> completionHandler);

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[NullAllowed, Export ("providerProtocol", ArgumentSemantic.Strong)]
		NEDnsProxyProviderProtocol ProviderProtocol { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	/// <summary>The main class for intercepting DNS traffic originating on the device.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NEProvider), Name = "NEDNSProxyProvider")]
	interface NEDnsProxyProvider {

		[Async]
		[Export ("startProxyWithOptions:completionHandler:")]
		void StartProxy ([NullAllowed] NSDictionary options, Action<NSError> completionHandler);

		[Async]
		[Export ("stopProxyWithReason:completionHandler:")]
		void StopProxy (NEProviderStopReason reason, Action completionHandler);

		[Export ("cancelProxyWithError:")]
		void CancelProxy ([NullAllowed] NSError error);

		[Export ("handleNewFlow:")]
		bool HandleNewFlow (NEAppProxyFlow flow);

		[NullAllowed, Export ("systemDNSSettings")]
		NEDnsSettings [] SystemDnsSettings { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("handleNewUDPFlow:initialRemoteEndpoint:")]
		[Deprecated (PlatformName.iOS, 18, 0, message: "Use 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'HandleNewUdpFlowWithInitialFlowEndPoint' instead.")]
		bool HandleNewUdpFlow (NEAppProxyUdpFlow flow, NWEndpoint remoteEndpoint);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("handleNewUDPFlow:initialRemoteFlowEndpoint:")]
		bool HandleNewUdpFlowWithInitialFlowEndPoint (NEAppProxyUdpFlow flow, Network.NWEndpoint remoteEndpoint);
	}

	/// <summary>Configuration settings for <see cref="T:NetworkExtension.NEDnsProxyProvider" /> objects.</summary>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEVpnProtocol), Name = "NEDNSProxyProviderProtocol")]
	interface NEDnsProxyProviderProtocol {

		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Copy)]
		NSDictionary ProviderConfiguration { get; set; }

		[NullAllowed, Export ("providerBundleIdentifier")]
		string ProviderBundleIdentifier { get; set; }
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHS20Settings : NSCopying, NSSecureCoding {

		[Export ("domainName")]
		string DomainName { get; }

		[Export ("roamingEnabled")]
		bool RoamingEnabled { [Bind ("isRoamingEnabled")] get; set; }

		[Export ("roamingConsortiumOIs", ArgumentSemantic.Copy)]
		string [] RoamingConsortiumOIs { get; set; }

		[Export ("naiRealmNames", ArgumentSemantic.Copy)]
		string [] NaiRealmNames { get; set; }

		[Export ("MCCAndMNCs", ArgumentSemantic.Copy)]
		string [] MccAndMncs { get; set; }

		[Export ("initWithDomainName:roamingEnabled:")]
		NativeHandle Constructor (string domainName, bool roamingEnabled);
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NEHotspotEAPSettings")]
	interface NEHotspotEapSettings : NSCopying, NSSecureCoding {

		[Internal]
		[Export ("supportedEAPTypes", ArgumentSemantic.Copy)]
		IntPtr _SupportedEapTypes { get; set; }

		[Export ("username")]
		string Username { get; set; }

		[Export ("outerIdentity")]
		string OuterIdentity { get; set; }

		[Export ("ttlsInnerAuthenticationType", ArgumentSemantic.Assign)]
		NEHotspotConfigurationTtlsInnerAuthenticationType TtlsInnerAuthenticationType { get; set; }

		[Export ("password")]
		string Password { get; set; }

		[Export ("trustedServerNames", ArgumentSemantic.Copy)]
		string [] TrustedServerNames { get; set; }

		[Export ("tlsClientCertificateRequired")]
		bool TlsClientCertificateRequired { [Bind ("isTLSClientCertificateRequired")] get; set; }

		[Export ("preferredTLSVersion", ArgumentSemantic.Assign)]
		NEHotspotConfigurationEapTlsVersion PreferredTlsVersion { get; set; }

		[Export ("setIdentity:")]
		bool SetIdentity (SecIdentity identity);

		[Export ("setTrustedServerCertificates:")]
		bool SetTrustedServerCertificates (NSObject [] certificates);
	}

	/// <summary>Configuration settings for a network hotspot.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NEHotspotConfiguration : NSCopying, NSSecureCoding {

		[Export ("SSID")]
		string Ssid { get; }

		[Export ("joinOnce")]
		bool JoinOnce { get; set; }

		[Export ("lifeTimeInDays", ArgumentSemantic.Copy)]
		NSNumber LifeTimeInDays { get; set; }

		[Internal]
		[Export ("initWithSSID:")]
		IntPtr initWithSsid (string ssid);

		[Internal]
		[Export ("initWithSSID:passphrase:isWEP:")]
		IntPtr initWithSsid (string ssid, string passphrase, bool isWep);

		[Export ("initWithSSID:eapSettings:")]
		NativeHandle Constructor (string ssid, NEHotspotEapSettings eapSettings);

		[Export ("initWithHS20Settings:eapSettings:")]
		NativeHandle Constructor (NEHotspotHS20Settings hs20Settings, NEHotspotEapSettings eapSettings);

		[Internal]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithSSIDPrefix:")]
		IntPtr initWithSsidPrefix (string ssidPrefix);

		[Internal]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithSSIDPrefix:passphrase:isWEP:")]
		IntPtr initWithSsidPrefix (string ssidPrefix, string passphrase, bool isWep);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hidden")]
		bool Hidden { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("SSIDPrefix")]
		string SsidPrefix { get; }
	}

	delegate void NEHotspotConfigurationManagerJoinHotspotCallback ([NullAllowed] NSError error);

	/// <summary>Manages the application or removal of <see cref="T:NetworkExtension.NEHotspotConfiguration" />.</summary>
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotConfigurationManager {

		[Static]
		[Export ("sharedManager", ArgumentSemantic.Strong)]
		NEHotspotConfigurationManager SharedManager { get; }

		[Async]
		[Export ("applyConfiguration:completionHandler:")]
		void ApplyConfiguration (NEHotspotConfiguration configuration, [NullAllowed] Action<NSError> completionHandler);

		[Export ("removeConfigurationForSSID:")]
		void RemoveConfiguration (string ssid);

		[Export ("removeConfigurationForHS20DomainName:")]
		void RemoveConfigurationForHS20DomainName (string domainName);

		[Async]
		[Export ("getConfiguredSSIDsWithCompletionHandler:")]
		void GetConfiguredSsids (Action<string []> completionHandler);

#if NET
		// Headers say this method is available on Mac Catalyst, but the AccessorySetupKit framework (thus the ASAccessory type) is not, so we can't expose it in Mac Catalyst for now.
		[NoTV, NoMac, iOS (18, 0), NoMacCatalyst]
		[Async]
		[Export ("joinAccessoryHotspot:passphrase:completionHandler:")]
		void JoinAccessoryHotspot (ASAccessory accessory, string passphrase, [NullAllowed] NEHotspotConfigurationManagerJoinHotspotCallback completionHandler);

		// Headers say this method is available on Mac Catalyst, but the AccessorySetupKit framework (thus the ASAccessory type) is not, so we can't expose it in Mac Catalyst for now.
		[NoTV, NoMac, iOS (18, 0), NoMacCatalyst]
		[Async]
		[Export ("joinAccessoryHotspotWithoutSecurity:completionHandler:")]
		void JoinAccessoryHotspotWithoutSecurit (ASAccessory accessory, [NullAllowed] NEHotspotConfigurationManagerJoinHotspotCallback completionHandler);
#endif
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NENetworkRule : NSSecureCoding, NSCopying {

		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Call the constructor overload that takes an 'NENetworkRuleConstructorOption' instead.")]
		[Export ("initWithDestinationNetwork:prefix:protocol:")]
		NativeHandle Constructor (NWHostEndpoint networkEndpoint, nuint destinationPrefix, NENetworkRuleProtocol protocol);

		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Call the constructor overload that takes an 'NENetworkRuleConstructorOption' instead.")]
		[Export ("initWithDestinationHost:protocol:")]
		NativeHandle Constructor (NWHostEndpoint hostEndpoint, NENetworkRuleProtocol protocol);

		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Call the constructor overload that takes an 'NENetworkRuleConstructorOption' instead.")]
		[Export ("initWithRemoteNetwork:remotePrefix:localNetwork:localPrefix:protocol:direction:")]
		NativeHandle Constructor ([NullAllowed] NWHostEndpoint remoteNetwork, nuint remotePrefix, [NullAllowed] NWHostEndpoint localNetwork, nuint localPrefix, NENetworkRuleProtocol protocol, NETrafficDirection direction);

		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'MatchRemoteHostOrNetworkEndpoint' instead.")]
		[NullAllowed, Export ("matchRemoteEndpoint")]
		NWHostEndpoint MatchRemoteEndpoint { get; }

		[Export ("matchRemotePrefix")]
		nuint MatchRemotePrefix { get; }

		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'MatchLocalNetworkEndpoint' instead.")]
		[NullAllowed, Export ("matchLocalNetwork")]
		NWHostEndpoint MatchLocalNetwork { get; }

		[Export ("matchLocalPrefix")]
		nuint MatchLocalPrefix { get; }

		[Export ("matchProtocol")]
		NENetworkRuleProtocol MatchProtocol { get; }

		[Export ("matchDirection")]
		NETrafficDirection MatchDirection { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("initWithDestinationNetworkEndpoint:prefix:protocol:")]
		NativeHandle Constructor (Network.NWEndpoint networkEndpoint, nuint destinationPrefix, NENetworkRuleProtocol protocol);

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("initWithDestinationHostEndpoint:protocol:")]
		NativeHandle Constructor (Network.NWEndpoint hostEndpoint, NENetworkRuleProtocol protocol);

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("initWithRemoteNetworkEndpoint:remotePrefix:localNetworkEndpoint:localPrefix:protocol:direction:")]
		NativeHandle Constructor ([NullAllowed] Network.NWEndpoint remoteNetwork, nuint remotePrefix, [NullAllowed] Network.NWEndpoint localNetwork, nuint localPrefix, NENetworkRuleProtocol protocol, NETrafficDirection direction);

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("matchRemoteHostOrNetworkEndpoint"), NullAllowed]
		Network.NWEndpoint MatchRemoteHostOrNetworkEndpoint { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("matchLocalNetworkEndpoint"), NullAllowed]
		Network.NWEndpoint MatchLocalNetworkEndpoint { get; }
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEFilterRule : NSSecureCoding, NSCopying {

		[Export ("initWithNetworkRule:action:")]
		NativeHandle Constructor (NENetworkRule networkRule, NEFilterAction action);

		[Export ("networkRule", ArgumentSemantic.Copy)]
		NENetworkRule NetworkRule { get; }

		[Export ("action")]
		NEFilterAction Action { get; }
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEFilterSettings : NSSecureCoding, NSCopying {

		[Export ("initWithRules:defaultAction:")]
		NativeHandle Constructor (NEFilterRule [] rules, NEFilterAction defaultAction);

		[Export ("rules", ArgumentSemantic.Copy)]
		NEFilterRule [] Rules { get; }

		[Export ("defaultAction")]
		NEFilterAction DefaultAction { get; }
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface NEFilterPacketContext {
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	delegate NEFilterPacketProviderVerdict NEFilterPacketHandler (NEFilterPacketContext context, IntPtr @interface, NETrafficDirection directiom, IntPtr packetBytes, nuint packetLength);

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NEFilterProvider))]
	[DisableDefaultCtor] // returns `nil`
	interface NEFilterPacketProvider {
		[NullAllowed, Export ("packetHandler", ArgumentSemantic.Strong)]
		NEFilterPacketHandler PacketHandler { get; set; }

		[Export ("delayCurrentPacket:")]
		NEPacket DelayCurrentPacket (NEFilterPacketContext context);

		[Export ("allowPacket:")]
		void AllowPacket (NEPacket packet);
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NEVpnManager))]
	[DisableDefaultCtor]
	interface NETransparentProxyManager {

		[Static]
		[Async]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		void LoadAllFromPreferences (Action<NETransparentProxyManager [], NSError> completionHandler);
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NETunnelNetworkSettings))]
	interface NETransparentProxyNetworkSettings {

		[NullAllowed, Export ("includedNetworkRules", ArgumentSemantic.Copy)]
		NENetworkRule [] IncludedNetworkRules { get; set; }

		[NullAllowed, Export ("excludedNetworkRules", ArgumentSemantic.Copy)]
		NENetworkRule [] ExcludedNetworkRules { get; set; }
	}

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface NEAppPushManager {
		[Export ("matchSSIDs", ArgumentSemantic.Copy)]
		string [] MatchSsids { get; set; }

		[Export ("providerConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> ProviderConfiguration { get; set; }

		[NullAllowed]
		[Export ("providerBundleIdentifier")]
		string ProviderBundleIdentifier { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INEAppPushDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Static]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		void LoadAllFromPreferences (Action<NEAppPushManager [], NSError> completionHandler);

		[Async]
		[Export ("loadFromPreferencesWithCompletionHandler:")]
		void LoadFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("removeFromPreferencesWithCompletionHandler:")]
		void RemoveFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("saveToPreferencesWithCompletionHandler:")]
		void SaveToPreferences (Action<NSError> completionHandler);

		[NullAllowed]
		[Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("matchPrivateLTENetworks", ArgumentSemantic.Copy)]
		NEPrivateLteNetwork [] MatchPrivateLteNetworks { get; set; }
	}

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NEProvider))]
	[DisableDefaultCtor] // init returns nil
	interface NEAppPushProvider {
		[NullAllowed]
		[Export ("providerConfiguration")]
		NSDictionary<NSString, NSObject> ProviderConfiguration { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the synchronoys 'Start' method instead..")]
		[Deprecated (PlatformName.MacCatalyst, 12, 0, message: "Use the synchronoys 'Start' method instead..")]
		[Async]
		[Export ("startWithCompletionHandler:")]
		void Start (Action<NSError> completionHandler);

		[Async]
		[Export ("stopWithReason:completionHandler:")]
		void Stop (NEProviderStopReason reason, Action completionHandler);

		[Export ("reportIncomingCallWithUserInfo:")]
		void ReportIncomingCall (NSDictionary userInfo);

		[NoTV, NoMacCatalyst, NoMac, iOS (16, 4)]
		[Export ("reportPushToTalkMessageWithUserInfo:")]
		void ReportPushToTalkMessage (NSDictionary userInfo);

		[Export ("handleTimerEvent")]
		void HandleTimerEvent ();

		[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("start")]
		void Start ();
	}

	[iOS (14, 0), TV (17, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NEDnsSettings), Name = "NEDNSOverHTTPSSettings")]
	interface NEDnsOverHttpsSettings {
		[NullAllowed]
		[Export ("serverURL", ArgumentSemantic.Copy)]
		NSUrl ServerUrl { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("identityReference", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData IdentityReference { get; set; }
	}

	[iOS (14, 0), TV (17, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NEDnsSettings), Name = "NEDNSOverTLSSettings")]
	interface NEDnsOverTlsSettings {
		[NullAllowed]
		[Export ("serverName")]
		string ServerName { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("identityReference", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData IdentityReference { get; set; }
	}

	[NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "NEDNSSettingsManager")]
	interface NEDnsSettingsManager {
		[Static]
		[Export ("sharedManager")]
		NEDnsSettingsManager SharedManager { get; }

		[Async]
		[Export ("loadFromPreferencesWithCompletionHandler:")]
		void LoadFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("removeFromPreferencesWithCompletionHandler:")]
		void RemoveFromPreferences (Action<NSError> completionHandler);

		[Async]
		[Export ("saveToPreferencesWithCompletionHandler:")]
		void SaveToPreferences (Action<NSError> completionHandler);

		[NullAllowed]
		[Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[NullAllowed]
		[Export ("dnsSettings", ArgumentSemantic.Strong)]
		NEDnsSettings DnsSettings { get; set; }

		[NullAllowed]
		[Export ("onDemandRules", ArgumentSemantic.Copy)]
		NEOnDemandRule [] OnDemandRules { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }
	}

	interface INEAppPushDelegate { }

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface NEAppPushDelegate {
		[Abstract]
		[Export ("appPushManager:didReceiveIncomingCallWithUserInfo:")]
		void DidReceiveIncomingCall (NEAppPushManager manager, NSDictionary userInfo);
	}

	[NoMacCatalyst]
	[NoiOS]
	[NoTV]
	[BaseType (typeof (NEAppProxyProvider))]
	[DisableDefaultCtor] // `init` returns `nil`
	interface NETransparentProxyProvider {
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject), Name = "NEPrivateLTENetwork")]
	interface NEPrivateLteNetwork : NSCopying, NSSecureCoding {
		[Export ("mobileCountryCode")]
		string MobileCountryCode { get; set; }

		[Export ("mobileNetworkCode")]
		string MobileNetworkCode { get; set; }

		[NullAllowed]
		[Export ("trackingAreaCode")]
		string TrackingAreaCode { get; set; }
	}

	[NoTV, NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NEPacketTunnelProvider))]
	[DisableDefaultCtor]
	interface NEEthernetTunnelProvider {
	}

	[NoTV, NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NEPacketTunnelNetworkSettings))]
	interface NEEthernetTunnelNetworkSettings {
		[Export ("initWithTunnelRemoteAddress:ethernetAddress:mtu:")]
		NativeHandle Constructor (string address, string ethernetAddress, nint mtu);

		[Export ("ethernetAddress")]
		string EthernetAddress { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface NERelay : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("HTTP3RelayURL", ArgumentSemantic.Copy)]
		NSUrl Http3RelayUrl { get; set; }

		[NullAllowed, Export ("HTTP2RelayURL", ArgumentSemantic.Copy)]
		NSUrl Http2RelayUrl { get; set; }

		[NullAllowed, Export ("dnsOverHTTPSURL", ArgumentSemantic.Copy)]
		NSUrl DnsOverHttpsUrl { get; set; }

		[NullAllowed, Export ("syntheticDNSAnswerIPv4Prefix")]
		string SyntheticDnsAnswerIPv4Prefix { get; set; }

		[NullAllowed, Export ("syntheticDNSAnswerIPv6Prefix")]
		string SyntheticDnsAnswerIPv6Prefix { get; set; }

		[Export ("additionalHTTPHeaderFields", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> AdditionalHttpHeaderFields { get; set; }

		[NullAllowed, Export ("rawPublicKeys", ArgumentSemantic.Copy)]
		NSData [] RawPublicKeys { get; set; }

		[NullAllowed, Export ("identityData", ArgumentSemantic.Copy)]
		NSData IdentityData { get; set; }

		[NullAllowed, Export ("identityDataPassword")]
		string IdentityDataPassword { get; set; }

		[Field ("NERelayErrorDomain")]
		NSString ErrorDomain { get; }

		[Notification, Field ("NERelayConfigurationDidChangeNotification")]
		NSString ConfigurationDidChangeNotification { get; }
	}

	delegate void NERelayManagerGetLastClientErrorsCallback ([NullAllowed] NSError [] errors);

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface NERelayManager {
		[Static]
		[Export ("sharedManager")]
		NERelayManager Shared { get; }

		[Export ("loadFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadFromPreferences (Action<NSError> completionHandler);

		[Export ("removeFromPreferencesWithCompletionHandler:")]
		[Async]
		void RemoveFromPreferences (Action<NSError> completionHandler);

		[Export ("saveToPreferencesWithCompletionHandler:")]
		[Async]
		void SaveToPreferences (Action<NSError> completionHandler);

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed, Export ("relays", ArgumentSemantic.Strong)]
		NERelay [] Relays { get; set; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }

		[NullAllowed, Export ("excludedDomains", ArgumentSemantic.Copy)]
		string [] ExcludedDomains { get; set; }

		[NullAllowed, Export ("onDemandRules", ArgumentSemantic.Copy)]
		NEOnDemandRule [] OnDemandRules { get; set; }

		[Static]
		[Export ("loadAllManagersFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadAllManagersFromPreferences (Action<NSArray<NERelayManager>, NSError> completionHandler);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("getLastClientErrors:completionHandler:")]
		[Async]
		void GetLastClientErrors (double seconds, NERelayManagerGetLastClientErrorsCallback completionHandler);
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[ErrorDomain ("NERelayClientErrorDomain")]
	[Native]
	enum NERelayManagerClientError : long {
		None = 1,
		DNSFailed = 2,
		ServerUnreachable = 3,
		ServerDisconnected = 4,
		CertificateMissing = 5,
		CertificateInvalid = 6,
		CertificateExpired = 7,
		ServerCertificateInvalid = 8,
		ServerCertificateExpired = 9,
		Other = 10,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject), Name = "NEVPNIKEv2PPKConfiguration")]
	interface NEVpnIkev2PpkConfiguration : NSCopying {
		[Export ("initWithIdentifier:keychainReference:")]
		NativeHandle Constructor (string identifier, NSData keychainReference);

		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; }

		[Export ("keychainReference", ArgumentSemantic.Copy)]
		NSData KeychainReference { get; }

		[Export ("isMandatory")]
		bool IsMandatory { get; set; }
	}
}
