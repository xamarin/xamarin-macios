// Copyright 2014-2015 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

using System;
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

	// Just to satisfy the core dll contract, the right type will be used on the generated file
	interface NWInterface { }
	interface NWParameters { }

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

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	enum NEFilterReportFrequency : long {
		None,
		Low,
		Medium,
		High,
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst]
	[Native]
	public enum NEFilterDataAttribute : long {
		HasIpHeader = 1,
	}

	[Watch (8, 0), NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	enum NEHotspotNetworkSecurityType : long {
		Open = 0,
		Wep = 1,
		Personal = 2,
		Enterprise = 3,
		Unknown = 4,
	}

	[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (17, 0)]
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


	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract] // documented as such and ...
	[DisableDefaultCtor] // can't be created (with `init`) without crashing introspection tests
	interface NEAppProxyFlow {
		[Export ("openWithLocalEndpoint:completionHandler:")]
		[Async]
		void OpenWithLocalEndpoint ([NullAllowed] NWHostEndpoint localEndpoint, Action<NSError> completionHandler);

		[Export ("closeReadWithError:")]
		void CloseRead ([NullAllowed] NSError error);

		[Export ("closeWriteWithError:")]
		void CloseWrite ([NullAllowed] NSError error);

		[Internal]
		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("setMetadata:")]
		void SetMetadata (OS_nw_parameters nwparameters);

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Wrap ("SetMetadata (parameters.GetHandle ())")]
		void SetMetadata (NWParameters parameters);

		[Export ("metaData")]
		NEFlowMetaData MetaData { get; }

		[Internal]
		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("networkInterface", ArgumentSemantic.Copy)]
		OS_nw_interface WeakNetworkInterface { get; set; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		NWInterface NetworkInterface {
			[Wrap ("Runtime.GetINativeObject<NWInterface> (WeakNetworkInterface, false)!")]
			get;
			[Wrap ("WeakNetworkInterface = value.GetHandle ()")]
			set;
		}

		[Mac (11, 0)]
		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("remoteHostname")]
		[NullAllowed]
		string RemoteHostname { get; }

		[Mac (11, 1), iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Export ("isBound")]
		bool IsBound { get; }

#if !NET
		[Field ("NEAppProxyErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

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
		bool HandleNewUdpFlow (NEAppProxyUdpFlow flow, NWEndpoint remoteEndpoint);
	}

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

		[Export ("remoteEndpoint")]
		NWEndpoint RemoteEndpoint { get; }
	}

	delegate void NEDatagramRead (NSData [] datagrams, NWEndpoint [] remoteEndpoints, NSError error);

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEAppProxyFlow), Name = "NEAppProxyUDPFlow")]
	[DisableDefaultCtor]
	interface NEAppProxyUdpFlow {
		[Export ("readDatagramsWithCompletionHandler:")]
		[Async (ResultTypeName = "NEDatagramReadResult")]
		void ReadDatagrams (NEDatagramRead completionHandler);

		[Export ("writeDatagrams:sentByEndpoints:completionHandler:")]
		[Async]
		void WriteDatagrams (NSData [] datagrams, NWEndpoint [] remoteEndpoints, Action<NSError> completionHandler);

		[NullAllowed, Export ("localEndpoint")]
		NWEndpoint LocalEndpoint { get; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NEAppRule : NSSecureCoding, NSCopying {
		[MacCatalyst (13, 1)]
		[NoWatch]
#if NET
		[NoMac]
#endif
		[Export ("initWithSigningIdentifier:")]
		NativeHandle Constructor (string signingIdentifier);

		[NoiOS, NoWatch, NoMacCatalyst]
		[Export ("initWithSigningIdentifier:designatedRequirement:")]
		NativeHandle Constructor (string signingIdentifier, string designatedRequirement);

		[NoiOS, NoWatch, MacCatalyst (15, 0)]
		[Export ("matchDesignatedRequirement")]
		string MatchDesignatedRequirement { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("matchPath")]
		string MatchPath { get; set; }

		[Export ("matchSigningIdentifier")]
		string MatchSigningIdentifier { get; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }

		[NoWatch, NoiOS, MacCatalyst (15, 0)]
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

		[NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dnsProtocol")]
		NEDnsProtocol DnsProtocol { get; }

		[Mac (11, 0), iOS (14, 0), NoTV, NoWatch]
		[MacCatalyst (14, 0)]
		[Notification]
		[Field ("NEDNSSettingsConfigurationDidChangeNotification")]
		NSString ConfigurationDidChangeNotification { get; }
	}

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

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Export ("updateFlow:usingVerdict:forDirection:")]
		void UpdateFlow (NEFilterSocketFlow flow, NEFilterDataVerdict verdict, NETrafficDirection direction);
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterVerdict))]
	interface NEFilterDataVerdict : NSSecureCoding, NSCopying {
		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
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

		[Mac (13, 0), NoiOS, NoMacCatalyst, NoWatch, NoTV]
		[NullAllowed]
		[Export ("sourceProcessAuditToken")]
		NSData SourceProcessAuditToken { get; }

		[iOS (13, 1)]
		[MacCatalyst (13, 1)]
		[Export ("identifier")]
		NSUuid Identifier { get; }
	}

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
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterVerdict))]
	interface NEFilterNewFlowVerdict : NSSecureCoding, NSCopying {
		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
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

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NEFilterVerdict : NSSecureCoding, NSCopying {
		[MacCatalyst (13, 1)]
		[Export ("shouldReport")]
		bool ShouldReport { get; set; }
	}

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

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[NullAllowed, Export ("filterFlowIdentifier")]
		NSUuid FilterFlowIdentifier { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void NEHotspotHelperHandler (NEHotspotHelperCommand cmd);

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

	[NoTV]
	[Static]
	[NoMac]
	[MacCatalyst (13, 1)]
	interface NEHotspotHelperOptionInternal {
		[Field ("kNEHotspotHelperOptionDisplayName")]
		NSString DisplayName { get; }
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSMutableUrlRequest))]
	interface NSMutableURLRequest_NEHotspotHelper {
		[Export ("bindToHotspotHelperCommand:")]
		void BindTo (NEHotspotHelperCommand command);
	}

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

		[Export ("createTCPConnection:")]
		NWTcpConnection CreateTcpConnection (NWEndpoint endpoint);

		[Export ("createUDPSession:")]
		NWUdpSession CreateUdpSession (NWEndpoint endpoint);
	}

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
		[Watch (7, 0), NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("fetchCurrentWithCompletionHandler:")]
		void FetchCurrent (Action<NEHotspotNetwork> completionHandler);

		[Watch (8, 0), NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
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

		[NoWatch, NoTV, NoiOS, Mac (13, 0)]
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

		[Export ("createTCPConnectionToEndpoint:enableTLS:TLSParameters:delegate:")]
		NWTcpConnection CreateTcpConnectionToEndpoint (NWEndpoint remoteEndpoint, bool enableTLS, [NullAllowed] NWTlsParameters TLSParameters, [NullAllowed] NSObject connectionDelegate);

		[Export ("createUDPSessionToEndpoint:fromEndpoint:")]
		NWUdpSession CreateUdpSessionToEndpoint (NWEndpoint remoteEndpoint, [NullAllowed] NWHostEndpoint localEndpoint);

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

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Static]
		[Export ("forPerAppVPN")]
		NETunnelProviderManager CreatePerAppVpn ();

		[NoTV]
		[return: NullAllowed]
		[Export ("copyAppRules")]
		NEAppRule [] CopyAppRules ();

		// CopyAppRules was incorrectly bound to AppRules and it is only available on macOS
#if NET || MONOMAC || __MACCATALYST__
		[NoWatch, NoTV, NoiOS, MacCatalyst (15,0)]
		[Export ("appRules", ArgumentSemantic.Copy)]
		NEAppRule[] AppRules { get; set; }
#else
		[NoTV]
		[Obsolete ("Use 'CopyAppRules' instead, this property will be removed in the future.")]
		NEAppRule [] AppRules { [Wrap ("CopyAppRules ()!", IsVirtual = true)] get; }
#endif

		[NoTV]
		[Export ("routingMethod")]
		NETunnelProviderRoutingMethod RoutingMethod { get; }

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("safariDomains", ArgumentSemantic.Copy)]
		string [] SafariDomains { get; set; }

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("mailDomains", ArgumentSemantic.Copy)]
		string [] MailDomains { get; set; }

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("calendarDomains", ArgumentSemantic.Copy)]
		string [] CalendarDomains { get; set; }

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("contactsDomains", ArgumentSemantic.Copy)]
		string [] ContactsDomains { get; set; }

#if !NET
		[Field ("NETunnelProviderErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[NoWatch, NoTV, NoiOS, Mac (11, 0), MacCatalyst (15, 0)]
		[Export ("excludedDomains", ArgumentSemantic.Copy)]
		string [] ExcludedDomains { get; set; }

		[NoWatch, NoTV, NoiOS, Mac (11, 0), MacCatalyst (15, 0)]
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

	[MacCatalyst (13, 1)]
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
		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
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

		[NoWatch, NoTV, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("excludeCellularServices")]
		bool ExcludeCellularServices { get; set; }

		[NoWatch, NoTV, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("excludeAPNs")]
		bool ExcludeApns { get; set; }

		[Mac (11, 0)]
		[iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Export ("enforceRoutes")]
		bool EnforceRoutes { get; set; }
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

		[NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("mtu")]
		nuint Mtu { get; set; }
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
	interface NWEndpoint : NSSecureCoding, NSCopying {
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NWEndpoint))]
	[DisableDefaultCtor]
	interface NWHostEndpoint {
		[Static]
		[Export ("endpointWithHostname:port:")]
		NWHostEndpoint Create (string hostname, string port);

		[Export ("hostname")]
		string Hostname { get; }

		[Export ("port")]
		string Port { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NWEndpoint))]
	[DisableDefaultCtor]
	interface NWBonjourServiceEndpoint {

		[Static]
		[Export ("endpointWithName:type:domain:")]
		NWBonjourServiceEndpoint Create (string name, string type, string domain);

		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		string Type { get; }

		[Export ("domain")]
		string Domain { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NWPath {
		[Export ("status")]
		NWPathStatus Status { get; }

		[Export ("expensive")]
		bool Expensive { [Bind ("isExpensive")] get; }

		[Export ("isEqualToPath:")]
		bool IsEqualToPath (NWPath path);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("constrained")]
		bool Constrained { [Bind ("isConstrained")] get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWTCPConnection")]
	interface NWTcpConnection {
		[Export ("initWithUpgradeForConnection:")]
		NativeHandle Constructor (NWTcpConnection connection);

		[Export ("state")]
		NWTcpConnectionState State { get; }

		[Export ("viable")]
		bool Viable { [Bind ("isViable")] get; }

		[Export ("hasBetterPath")]
		bool HasBetterPath { get; }

		[Export ("endpoint")]
		NWEndpoint Endpoint { get; }

		[NullAllowed, Export ("connectedPath")]
		NWPath ConnectedPath { get; }

		[NullAllowed, Export ("localAddress")]
		NWEndpoint LocalAddress { get; }

		[NullAllowed, Export ("remoteAddress")]
		NWEndpoint RemoteAddress { get; }

		[NullAllowed, Export ("txtRecord")]
		NSData TxtRecord { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("readLength:completionHandler:")]
		[Async]
		void ReadLength (nuint length, Action<NSData, NSError> completion);

		[Export ("readMinimumLength:maximumLength:completionHandler:")]
		[Async]
		void ReadMinimumLength (nuint minimum, nuint maximum, Action<NSData, NSError> completion);

		[Export ("write:completionHandler:")]
		[Async]
		void Write (NSData data, Action<NSError> completion);

		[Export ("writeClose")]
		void WriteClose ();
	}

	interface INWTcpConnectionAuthenticationDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject), Name = "NWTCPConnectionAuthenticationDelegate")]
	interface NWTcpConnectionAuthenticationDelegate {
		[Export ("shouldProvideIdentityForConnection:")]
		bool ShouldProvideIdentity (NWTcpConnection connection);

		[Export ("provideIdentityForConnection:completionHandler:")]
		void ProvideIdentity (NWTcpConnection connection, Action<SecIdentity, NSArray> completion);

		[Export ("shouldEvaluateTrustForConnection:")]
		bool ShouldEvaluateTrust (NWTcpConnection connection);

		[Export ("evaluateTrustForConnection:peerCertificateChain:completionHandler:")]
		[Async]
		void EvaluateTrust (NWTcpConnection connection, NSArray peerCertificateChain, Action<SecTrust> completion);
		// note: it's not clear (from headers) but based on other API it's likely to accept a mix of SecIdentity
		// and SecCertificate - both *NOT* NSObject -> because of that NSArray is used above
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWTLSParameters")]
	interface NWTlsParameters {
		[NullAllowed, Export ("TLSSessionID", ArgumentSemantic.Copy)]
		NSData TlsSessionID { get; set; }

		[NullAllowed, Export ("SSLCipherSuites", ArgumentSemantic.Copy)]
		NSSet<NSNumber> SslCipherSuites { get; set; }

		[Export ("minimumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MinimumSslProtocolVersion { get; set; }

		[Export ("maximumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MaximumSslProtocolVersion { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NWUDPSession")]
	interface NWUdpSession {
		[Export ("initWithUpgradeForSession:")]
		NativeHandle Constructor (NWUdpSession session);

		[Export ("state")]
		NWUdpSessionState State { get; }

		[Export ("endpoint")]
		NWEndpoint Endpoint { get; }

		[NullAllowed, Export ("resolvedEndpoint")]
		NWEndpoint ResolvedEndpoint { get; }

		[Export ("viable")]
		bool Viable { [Bind ("isViable")] get; }

		[Export ("hasBetterPath")]
		bool HasBetterPath { get; }

		[NullAllowed, Export ("currentPath")]
		NWPath CurrentPath { get; }

		[Export ("tryNextResolvedEndpoint")]
		void TryNextResolvedEndpoint ();

		[Export ("maximumDatagramLength")]
		nuint MaximumDatagramLength { get; }

		[Export ("setReadHandler:maxDatagrams:")]
		void SetReadHandler (Action<NSArray, NSError> handler, nuint maxDatagrams);

		[Export ("writeMultipleDatagrams:completionHandler:")]
		[Async]
		void WriteMultipleDatagrams (NSData [] datagramArray, Action<NSError> completionHandler);

		[Export ("writeDatagram:completionHandler:")]
		[Async]
		void WriteDatagram (NSData datagram, Action<NSError> completionHandler);

		[Export ("cancel")]
		void Cancel ();
	}

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

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NEFilterFlow))]
	interface NEFilterSocketFlow {
		[NullAllowed]
		[Export ("remoteEndpoint")]
		NWEndpoint RemoteEndpoint { get; }

		[NullAllowed]
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
		[NoWatch, NoTV, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("remoteHostname")]
		string RemoteHostname { get; }
	}

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

		[Export ("createTCPConnectionThroughTunnelToEndpoint:enableTLS:TLSParameters:delegate:")]
		NWTcpConnection CreateTcpConnection (NWEndpoint remoteEndpoint, bool enableTls, [NullAllowed] NWTlsParameters tlsParameters, [NullAllowed] INWTcpConnectionAuthenticationDelegate @delegate);

		[Export ("createUDPSessionThroughTunnelToEndpoint:fromEndpoint:")]
		NWUdpSession CreateUdpSession (NWEndpoint remoteEndpoint, [NullAllowed] NWHostEndpoint localEndpoint);
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
		bool HandleNewUdpFlow (NEAppProxyUdpFlow flow, NWEndpoint remoteEndpoint);
	}

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
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NENetworkRule : NSSecureCoding, NSCopying {

		[Export ("initWithDestinationNetwork:prefix:protocol:")]
		NativeHandle Constructor (NWHostEndpoint networkEndpoint, nuint destinationPrefix, NENetworkRuleProtocol protocol);

		[Export ("initWithDestinationHost:protocol:")]
		NativeHandle Constructor (NWHostEndpoint hostEndpoint, NENetworkRuleProtocol protocol);

		[Export ("initWithRemoteNetwork:remotePrefix:localNetwork:localPrefix:protocol:direction:")]
		NativeHandle Constructor ([NullAllowed] NWHostEndpoint remoteNetwork, nuint remotePrefix, [NullAllowed] NWHostEndpoint localNetwork, nuint localPrefix, NENetworkRuleProtocol protocol, NETrafficDirection direction);

		[NullAllowed, Export ("matchRemoteEndpoint")]
		NWHostEndpoint MatchRemoteEndpoint { get; }

		[Export ("matchRemotePrefix")]
		nuint MatchRemotePrefix { get; }

		[NullAllowed, Export ("matchLocalNetwork")]
		NWHostEndpoint MatchLocalNetwork { get; }

		[Export ("matchLocalPrefix")]
		nuint MatchLocalPrefix { get; }

		[Export ("matchProtocol")]
		NENetworkRuleProtocol MatchProtocol { get; }

		[Export ("matchDirection")]
		NETrafficDirection MatchDirection { get; }
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

	[NoTV]
	[NoWatch, NoTV, NoMac, iOS (14, 0)]
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

		[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("matchPrivateLTENetworks", ArgumentSemantic.Copy)]
		NEPrivateLteNetwork [] MatchPrivateLteNetworks { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
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

		[NoWatch, NoTV, NoMacCatalyst, NoMac, iOS (16, 4)]
		[Export ("reportPushToTalkMessageWithUserInfo:")]
		void ReportPushToTalkMessage (NSDictionary userInfo);

		[Export ("handleTimerEvent")]
		void HandleTimerEvent ();

		[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("start")]
		void Start ();
	}

	[NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NEDnsSettings), Name = "NEDNSOverHTTPSSettings")]
	interface NEDnsOverHttpsSettings {
		[NullAllowed]
		[Export ("serverURL", ArgumentSemantic.Copy)]
		NSUrl ServerUrl { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("identityReference", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData IdentityReference { get; set; }
	}

	[NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NEDnsSettings), Name = "NEDNSOverTLSSettings")]
	interface NEDnsOverTlsSettings {
		[NullAllowed]
		[Export ("serverName")]
		string ServerName { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("identityReference", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData IdentityReference { get; set; }
	}

	[NoWatch, NoTV, Mac (11, 0), iOS (14, 0)]
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

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
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

	[Mac (11, 0), NoMacCatalyst]
	[NoiOS]
	[NoTV]
	[NoWatch]
	[BaseType (typeof (NEAppProxyProvider))]
	[DisableDefaultCtor] // `init` returns `nil`
	interface NETransparentProxyProvider {
	}

	[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
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

	[NoWatch, NoTV, NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NEPacketTunnelProvider))]
	[DisableDefaultCtor]
	interface NEEthernetTunnelProvider {
	}

	[NoWatch, NoTV, NoiOS, Mac (13, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NEPacketTunnelNetworkSettings))]
	interface NEEthernetTunnelNetworkSettings {
		[Export ("initWithTunnelRemoteAddress:ethernetAddress:mtu:")]
		NativeHandle Constructor (string address, string ethernetAddress, nint mtu);

		[Export ("ethernetAddress")]
		string EthernetAddress { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}
}
