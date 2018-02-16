// Copyright 2014-2015 Xamarin Inc. All rights reserved.
#if XAMCORE_2_0 || !MONOMAC
using System;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Security;

namespace NetworkExtension {

	[ErrorDomain ("NEDNSProxyErrorDomain")]
	[iOS (11,0), NoMac]
	[Native]
	enum NEDnsProxyManagerError : long {
		Invalid = 1,
		Disabled = 2,
		Stale = 3,
		CannotBeRemoved = 4,
	}

	[iOS (11,0), NoMac]
	[Native]
	enum NEFilterAction : long {
		Invalid = 0,
		Allow = 1,
		Drop = 2,
		Remediate = 3,
		FilterData = 4,
	}

	[iOS (11,0), Mac (10,13, onlyOn64: true)]
	[Native]
	enum NEVpnIkev2TlsVersion : long {
		Default = 0,
		Tls1_0 = 1,
		Tls1_1 = 2,
		Tls1_2 = 3,
	}

	[iOS (11,0), NoMac]
	[Native]
	enum NEHotspotConfigurationEapType : long {
		Tls = 13,
		Ttls = 21,
		Peap = 25,
		Fast = 43,
	}

	[iOS (11,0), NoMac]
	[Native]
	enum NEHotspotConfigurationTtlsInnerAuthenticationType : long {
		Pap = 0,
		Chap = 1,
		MSChap = 2,
		MSChapv2 = 3,
		Eap = 4,
	}

	[iOS (11,0), NoMac]
	[Native]
	enum NEHotspotConfigurationEapTlsVersion : long {
		Tls1_0 = 0,
		Tls1_1 = 1,
		Tls1_2 = 2,
	}

	[iOS (11,0), NoMac]
	[Native]
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
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
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
	
		[Export ("metaData")]
		NEFlowMetaData MetaData { get; }

#if !XAMCORE_4_0
		[Field ("NEAppProxyErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NETunnelProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEAppProxyProvider
	{
		[Export ("startProxyWithOptions:completionHandler:")]
		[Async]
		void StartProxy ([NullAllowed] NSDictionary<NSString,NSObject> options, Action<NSError> completionHandler);
	
		[Export ("stopProxyWithReason:completionHandler:")]
		[Async]
		void StopProxy (NEProviderStopReason reason, Action completionHandler);
	
		[Export ("cancelProxyWithError:")]
		void CancelProxy ([NullAllowed] NSError error);
	
		[Export ("handleNewFlow:")]
		bool HandleNewFlow (NEAppProxyFlow flow);
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NETunnelProviderManager))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEAppProxyProviderManager
	{
		[Static]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadAllFromPreferences (Action<NSArray, NSError> completionHandler);
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NEAppProxyFlow), Name="NEAppProxyTCPFlow")]
	[DisableDefaultCtor]
	interface NEAppProxyTcpFlow
	{
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
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NEAppProxyFlow), Name="NEAppProxyUDPFlow")]
	[DisableDefaultCtor]
	interface NEAppProxyUdpFlow
	{
		[Export ("readDatagramsWithCompletionHandler:")]
		[Async (ResultTypeName="NEDatagramReadResult")]
		void ReadDatagrams (NEDatagramRead completionHandler);
	
		[Export ("writeDatagrams:sentByEndpoints:completionHandler:")]
		[Async]
		void WriteDatagrams (NSData[] datagrams, NWEndpoint[] remoteEndpoints, Action<NSError> completionHandler);
	
		[NullAllowed, Export ("localEndpoint")]
		NWEndpoint LocalEndpoint { get; }
	}
		
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEAppRule : NSSecureCoding, NSCopying
	{
		[Export ("initWithSigningIdentifier:")]
		IntPtr Constructor (string signingIdentifier);

#if MONOMAC
		[Export ("initWithSigningIdentifier:designatedRequirement:")]
		IntPtr Constructor (string signingIdentifier, string designatedRequirement);

		[Export ("matchDesignatedRequirement")]
		string MatchDesignatedRequirement { get; }
#endif

		[iOS (9,3)]
		[NullAllowed, Export ("matchPath")]
		string MatchPath { get; set; }
	
		[Export ("matchSigningIdentifier")]
		string MatchSigningIdentifier { get; }

		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string [] MatchDomains { get; set; }
	}
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject), Name="NEDNSSettings")]
	[DisableDefaultCtor]
	interface NEDnsSettings : NSSecureCoding, NSCopying
	{
		[Export ("initWithServers:")]
		IntPtr Constructor (string[] servers);
	
		[Export ("servers")]
		string[] Servers { get; }
	
		[NullAllowed, Export ("searchDomains", ArgumentSemantic.Copy)]
		string[] SearchDomains { get; set; }
	
		[NullAllowed, Export ("domainName")]
		string DomainName { get; set; }
	
		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string[] MatchDomains { get; set; }
	
		[Export ("matchDomainsNoSearch")]
		bool MatchDomainsNoSearch { get; set; }
	}

#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof(NEFilterProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEFilterControlProvider
	{
		[NullAllowed, Export ("remediationMap", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSDictionary<NSString,NSObject>> RemediationMap { get; set; }
	
		[NullAllowed, Export ("URLAppendStringMap", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSString> UrlAppendStringMap { get; set; }
	
		[Export ("handleRemediationForFlow:completionHandler:")]
		[Async]
		void HandleRemediationForFlow (NEFilterFlow flow, Action<NEFilterControlVerdict> completionHandler);
	
		[Export ("handleNewFlow:completionHandler:")]
		[Async]
		void HandleNewFlow (NEFilterFlow flow, Action<NEFilterControlVerdict> completionHandler);
	
		[Export ("notifyRulesChanged")]
		void NotifyRulesChanged ();

		[iOS (11,0)]
		[Export ("handleReport:")]
		void HandleReport (NEFilterReport report);
	}

	[iOS (9,0)]
	[BaseType (typeof(NEFilterNewFlowVerdict))]
	interface NEFilterControlVerdict : NSSecureCoding, NSCopying
	{
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

	[iOS (9,0)]
	[BaseType (typeof(NEFilterProvider))]
	[DisableDefaultCtor] // no valid handle when `init` is called
	interface NEFilterDataProvider
	{
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
	
		[Export ("handleRemediationForFlow:")]
		NEFilterRemediationVerdict HandleRemediationForFlow (NEFilterFlow flow);
	
		[Export ("handleRulesChanged")]
		void HandleRulesChanged ();
	}

	[iOS (9,0)]
	[BaseType (typeof(NEFilterVerdict))]
	interface NEFilterDataVerdict : NSSecureCoding, NSCopying
	{
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
	}
		
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface NEFilterFlow : NSSecureCoding, NSCopying
	{
		[NullAllowed, Export ("URL")]
		NSUrl Url { get; }

		[iOS (11,0)]
		[NullAllowed, Export ("sourceAppUniqueIdentifier")]
		NSData SourceAppUniqueIdentifier { get; }

		[iOS (11,0)]
		[NullAllowed, Export ("sourceAppIdentifier")]
		string SourceAppIdentifier { get; }

		[iOS (11,0)]
		[NullAllowed, Export ("sourceAppVersion")]
		string SourceAppVersion { get; }
	}
#endif

	// according to Xcode7 SDK this was available (in parts) in iOS8
	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
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

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Strong)]
		NEFilterProviderConfiguration ProviderConfiguration { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Field ("NEFilterConfigurationDidChangeNotification")]
		[Notification]
		NSString ConfigurationDidChangeNotification { get; }

#if !XAMCORE_4_0
		[Field ("NEFilterErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof(NEFilterVerdict))]
	interface NEFilterNewFlowVerdict : NSSecureCoding, NSCopying
	{
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
	}
#endif
		
#if XAMCORE_4_0
	[NoMac] // Not available on mac
#elif MONOMAC
	[Obsolete ("'NEFilterProvider' is not available on macOS and will be removed in a future version.")]
	[Mac (10,11, onlyOn64 : true)]
#endif
	[iOS (9,0)]
	[BaseType (typeof(NEProvider))]
	[Abstract] // documented as such
	interface NEFilterProvider
	{
		[Export ("startFilterWithCompletionHandler:")]
		[Async]
		void StartFilter (Action<NSError> completionHandler);
	
		[Export ("stopFilterWithReason:completionHandler:")]
		[Async]
		void StopFilter (NEProviderStopReason reason, Action completionHandler);
	
		[Export ("filterConfiguration")]
		NEFilterProviderConfiguration FilterConfiguration { get; }

		[Field ("NEFilterProviderRemediationMapRemediationButtonTexts")]
		NSString RemediationMapRemediationButtonTexts { get; }

		[Field ("NEFilterProviderRemediationMapRemediationURLs")]
		NSString RemediationMapRemediationUrls { get; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface NEFilterProviderConfiguration : NSSecureCoding, NSCopying
	{
		[Export ("filterBrowsers")]
		bool FilterBrowsers { get; set; }
	
		[Export ("filterSockets")]
		bool FilterSockets { get; set; }
	
		[NullAllowed, Export ("vendorConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSObject> VendorConfiguration { get; set; }
	
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
	}

#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof(NEFilterVerdict))]
	interface NEFilterRemediationVerdict : NSSecureCoding, NSCopying
	{
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

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface NEFilterVerdict : NSSecureCoding, NSCopying
	{
		[iOS (11,0)]
		[Export ("shouldReport")]
		bool ShouldReport { get; set; }
	}
#endif
		
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface NEFlowMetaData : NSCopying, NSSecureCoding
	{
		[Export ("sourceAppUniqueIdentifier")]
		NSData SourceAppUniqueIdentifier { get; }
	
		[Export ("sourceAppSigningIdentifier")]
		string SourceAppSigningIdentifier { get; }
	}

#if !MONOMAC
	[iOS (9,0)]
	delegate void NEHotspotHelperHandler (NEHotspotHelperCommand cmd);

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelper {
		[Static][Internal]
		[Export ("registerWithOptions:queue:handler:")]
		bool Register ([NullAllowed] NSDictionary options, DispatchQueue queue, NEHotspotHelperHandler handler);

		[Static]
		[Wrap ("Register (options == null ? null : options.Dictionary, queue, handler)")]
		bool Register ([NullAllowed] NEHotspotHelperOptions options, DispatchQueue queue, NEHotspotHelperHandler handler);

		[Static]
		[Export ("logoff:")]
		bool Logoff (NEHotspotNetwork network);

		[Static, NullAllowed]
		[Export ("supportedNetworkInterfaces")]
		NEHotspotNetwork[] SupportedNetworkInterfaces { get; }
	}

	[Static]
	[iOS (9,0)]
	interface NEHotspotHelperOptionInternal {
		[Field ("kNEHotspotHelperOptionDisplayName")]
		NSString DisplayName { get; }
	}

	[iOS (9,0)]
	[Category]
	[BaseType (typeof (NSMutableUrlRequest))]
	interface NSMutableURLRequest_NEHotspotHelper {
		[Export ("bindToHotspotHelperCommand:")]
		void BindTo (NEHotspotHelperCommand command);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelperCommand {
		[Export ("commandType")]
		NEHotspotHelperCommandType CommandType { get; }

		[NullAllowed, Export ("network")]
		NEHotspotNetwork Network { get; }

		[NullAllowed, Export ("networkList")]
		NEHotspotNetwork[] NetworkList { get; }

		[Export ("createResponse:")]
		NEHotspotHelperResponse CreateResponse (NEHotspotHelperResult result);

		[Export ("createTCPConnection:")]
		NWTcpConnection CreateTcpConnection (NWEndpoint endpoint);

		[Export ("createUDPSession:")]
		NWUdpSession CreateUdpSession (NWEndpoint endpoint);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface NEHotspotHelperResponse {
		[Export ("setNetwork:")]
		void SetNetwork (NEHotspotNetwork network);

		[Export ("setNetworkList:")]
		void SetNetworkList (NEHotspotNetwork[] networkList);

		[Export ("deliver")]
		void Deliver ();
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
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
	}
#endif

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEIPv4Route : NSSecureCoding, NSCopying
	{
		[Export ("initWithDestinationAddress:subnetMask:")]
		IntPtr Constructor (string address, string subnetMask);
	
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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEIPv6Route : NSSecureCoding, NSCopying
	{
		[Export ("initWithDestinationAddress:networkPrefixLength:")]
		IntPtr Constructor (string address, NSNumber networkPrefixLength);
	
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
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEIPv4Settings : NSSecureCoding, NSCopying
	{
		[Export ("initWithAddresses:subnetMasks:")]
		IntPtr Constructor (string[] addresses, string[] subnetMasks);
	
		[Export ("addresses")]
		string[] Addresses { get; }
	
		[Export ("subnetMasks")]
		string[] SubnetMasks { get; }
	
		[NullAllowed, Export ("includedRoutes", ArgumentSemantic.Copy)]
		NEIPv4Route[] IncludedRoutes { get; set; }
	
		[NullAllowed, Export ("excludedRoutes", ArgumentSemantic.Copy)]
		NEIPv4Route[] ExcludedRoutes { get; set; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEIPv6Settings : NSSecureCoding, NSCopying
	{
		[Export ("initWithAddresses:networkPrefixLengths:")]
		IntPtr Constructor (string[] addresses, NSNumber[] networkPrefixLengths);
	
		[Export ("addresses")]
		string[] Addresses { get; }
	
		[Export ("networkPrefixLengths")]
		NSNumber[] NetworkPrefixLengths { get; }
	
		[NullAllowed, Export ("includedRoutes", ArgumentSemantic.Copy)]
		NEIPv6Route[] IncludedRoutes { get; set; }
	
		[NullAllowed, Export ("excludedRoutes", ArgumentSemantic.Copy)]
		NEIPv6Route[] ExcludedRoutes { get; set; }
	}
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // init returns nil
	interface NEProvider
	{
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

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[Export ("displayMessage:completionHandler:")]
		[Async]
		void DisplayMessage (string message, Action<bool> completionHandler);
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface NEProxySettings : NSSecureCoding, NSCopying
	{
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
		string[] ExceptionList { get; set; }
	
		[NullAllowed, Export ("matchDomains", ArgumentSemantic.Copy)]
		string[] MatchDomains { get; set; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NEProxyServer : NSSecureCoding, NSCopying
	{
		[Export ("initWithAddress:port:")]
		IntPtr Constructor (string address, nint port);
	
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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NETunnelNetworkSettings : NSSecureCoding, NSCopying
	{
		[Export ("initWithTunnelRemoteAddress:")]
		IntPtr Constructor (string address);
	
		[Export ("tunnelRemoteAddress")]
		string TunnelRemoteAddress { get; }
	
		[NullAllowed, Export ("DNSSettings", ArgumentSemantic.Copy)]
		NEDnsSettings DnsSettings { get; set; }
	
		[NullAllowed, Export ("proxySettings", ArgumentSemantic.Copy)]
		NEProxySettings ProxySettings { get; set; }
	}
		
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NEProvider))]
	[DisableDefaultCtor] // init returns nil
	interface NETunnelProvider
	{
		[Export ("handleAppMessage:completionHandler:")]
		[Async]
		void HandleAppMessage (NSData messageData, [NullAllowed] Action<NSData> completionHandler);
	
		[Export ("setTunnelNetworkSettings:completionHandler:")]
		[Async]
		void SetTunnelNetworkSettings ([NullAllowed] NETunnelNetworkSettings tunnelNetworkSettings, [NullAllowed] Action<NSError> completionHandler);
	
		[Export ("protocolConfiguration")]
		NEVpnProtocol ProtocolConfiguration { get; }
	
		[NullAllowed, Export ("appRules")]
		NEAppRule[] AppRules { get; }
	
		[Export ("routingMethod")]
		NETunnelProviderRoutingMethod RoutingMethod { get; }

		[Export ("reasserting")]
		bool Reasserting { get; set; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NEVpnManager))]
	interface NETunnelProviderManager
	{
		[Static]
		[Export ("loadAllFromPreferencesWithCompletionHandler:")]
		[Async]
		void LoadAllFromPreferences (Action<NSArray, NSError> completionHandler);
	
		[NullAllowed, Export ("copyAppRules")]
		NEAppRule[] AppRules { get; }
	
		[Export ("routingMethod")]
		NETunnelProviderRoutingMethod RoutingMethod { get; }

#if !XAMCORE_4_0
		[Field ("NETunnelProviderErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

	
	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject), Name="NEVPNManager")]
	[DisableDefaultCtor] // Assertion failed: (0), function -[NEVPNManager init], file /SourceCache/NetworkExtension_Sim/NetworkExtension-168.1.8/Framework/NEVPNManager.m, line 41.
	interface NEVpnManager {

		[NullAllowed]
		[Export ("onDemandRules", ArgumentSemantic.Copy)]
		NEOnDemandRule [] OnDemandRules { get; set; }

		[Export ("onDemandEnabled")]
		bool OnDemandEnabled { [Bind ("isOnDemandEnabled")] get; set; }

		[Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[NullAllowed]
		[Export ("protocol", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'ProtocolConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'ProtocolConfiguration' instead.")]
		NEVpnProtocol Protocol { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
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

		[Mac (10,11, onlyOn64: true), NoiOS]
		[Internal]
		[Export ("setAuthorization:")]
		void _SetAuthorization (IntPtr auth);

#if !XAMCORE_4_0
		[Field ("NEVPNErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Notification]
		[Field ("NEVPNConfigurationChangeNotification")]
		NSString ConfigurationChangeNotification { get; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject), Name="NEVPNConnection")]
	interface NEVpnConnection {

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[NullAllowed, Export ("connectedDate")]
		NSDate ConnectedDate { get; }

		[Export ("status")]
		NEVpnStatus Status { get; }

		[Export ("startVPNTunnelAndReturnError:")]
		bool StartVpnTunnel (out NSError error);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Internal]
		[Export ("startVPNTunnelWithOptions:andReturnError:")]
		bool StartVpnTunnel ([NullAllowed] NSDictionary options, out NSError error);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Wrap ("StartVpnTunnel (options == null ? null : options.Dictionary, out error);")]
		bool StartVpnTunnel ([NullAllowed] NEVpnConnectionStartOptions options, out NSError error);

		[Export ("stopVPNTunnel")]
		void StopVpnTunnel ();

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[Export ("manager")]
		NEVpnManager Manager { get; }

		[Notification]
		[Field ("NEVPNStatusDidChangeNotification")]
		NSString StatusDidChangeNotification { get; }
	}

	[Static][Internal]
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	interface NEVpnConnectionStartOptionInternal {
		[Field ("NEVPNConnectionStartOptionPassword")]
		NSString Password { get; }

		[Field ("NEVPNConnectionStartOptionUsername")]
		NSString Username { get; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[Abstract]
	[BaseType (typeof (NSObject), Name="NEVPNProtocol")]
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

		[iOS (9,0)]
		[NullAllowed, Export ("identityReference", ArgumentSemantic.Copy)]
		NSData IdentityReference { get; set; }

		[Mac (10,11, onlyOn64 : true)]
		[NullAllowed] // by default this property is null
		[Export ("identityData", ArgumentSemantic.Copy)]
		NSData IdentityData { get; set; }

		[Mac (10,11, onlyOn64 : true)]
		[NullAllowed] // by default this property is null
		[Export ("identityDataPassword")]
		string IdentityDataPassword { get; set; }

		[Export ("disconnectOnSleep")]
		bool DisconnectOnSleep { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("proxySettings", ArgumentSemantic.Copy)]
		NEProxySettings ProxySettings { get; set; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEVpnProtocol), Name="NEVPNProtocolIPSec")]
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

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject), Name="NEVPNIKEv2SecurityAssociationParameters")]
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

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEVpnProtocolIpSec), Name="NEVPNProtocolIKEv2")]
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

		[iOS (8,3)][Mac (10,11, onlyOn64 : true)]
		[Export ("certificateType")]
		NEVpnIke2CertificateType CertificateType { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("useConfigurationAttributeInternalIPSubnet")]
		bool UseConfigurationAttributeInternalIPSubnet { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("disableMOBIKE")]
		bool DisableMobike { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("disableRedirect")]
		bool DisableRedirect { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("enablePFS")]
		bool EnablePfs { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("enableRevocationCheck")]
		bool EnableRevocationCheck { get; set; }

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[Export ("strictRevocationCheck")]
		bool StrictRevocationCheck { get; set; }

		[iOS (11,0), Mac (10,13, onlyOn64: true)]
		[Export ("minimumTLSVersion", ArgumentSemantic.Assign)]
		NEVpnIkev2TlsVersion MinimumTlsVersion { get; set; }

		[iOS (11,0), Mac (10,13, onlyOn64: true)]
		[Export ("maximumTLSVersion", ArgumentSemantic.Assign)]
		NEVpnIkev2TlsVersion MaximumTlsVersion { get; set; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
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

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleConnect {
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleDisconnect {
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleIgnore {
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NEOnDemandRule))]
	interface NEOnDemandRuleEvaluateConnection {

		[NullAllowed]
		[Export ("connectionRules", ArgumentSemantic.Copy)]
		NEEvaluateConnectionRule [] ConnectionRules { get; set; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface NEEvaluateConnectionRule : NSSecureCoding, NSCopying {

		[Export ("initWithMatchDomains:andAction:")]
		IntPtr Constructor (string [] domains, NEEvaluateConnectionRuleAction action);

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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[Abstract]
	interface NWEndpoint : NSSecureCoding, NSCopying {
	}
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NWEndpoint))]
	interface NWHostEndpoint
	{
		[Static]
		[Export ("endpointWithHostname:port:")]
		NWHostEndpoint Create (string hostname, string port);
	
		[Export ("hostname")]
		string Hostname { get; }
	
		[Export ("port")]
		string Port { get; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface NWPath
	{
		[Export ("status")]
		NWPathStatus Status { get; }
	
		[Export ("expensive")]
		bool Expensive { [Bind ("isExpensive")] get; }
	
		[Export ("isEqualToPath:")]
		bool IsEqualToPath (NWPath path);
	}
	
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject), Name="NWTCPConnection")]
	interface NWTcpConnection
	{
		[Export ("initWithUpgradeForConnection:")]
		IntPtr Constructor (NWTcpConnection connection);
	
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

	interface INWTcpConnectionAuthenticationDelegate {}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject), Name="NWTLSParameters")]
	interface NWTlsParameters
	{
		[NullAllowed, Export ("TLSSessionID", ArgumentSemantic.Copy)]
		NSData TlsSessionID { get; set; }
	
		[NullAllowed, Export ("SSLCipherSuites", ArgumentSemantic.Copy)]
		NSSet<NSNumber> SslCipherSuites { get; set; }
	
		[Export ("minimumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MinimumSslProtocolVersion { get; set; }
	
		[Export ("maximumSSLProtocolVersion", ArgumentSemantic.Assign)]
		nuint MaximumSslProtocolVersion { get; set; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject), Name="NWUDPSession")]
	interface NWUdpSession
	{
		[Export ("initWithUpgradeForSession:")]
		IntPtr Constructor (NWUdpSession session);
	
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
		void WriteMultipleDatagrams (NSData[] datagramArray, Action<NSError> completionHandler);
	
		[Export ("writeDatagram:completionHandler:")]
		[Async]
		void WriteDatagram (NSData datagram, Action<NSError> completionHandler);
	
		[Export ("cancel")]
		void Cancel ();
	}

#if !MONOMAC
	[iOS (9,0)]
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
		
	[iOS (9,0)]
	[BaseType (typeof (NEFilterFlow))]
	interface NEFilterSocketFlow {
		[NullAllowed]
		[Export ("remoteEndpoint")]
		NWEndpoint RemoteEndpoint { get; }

		[NullAllowed]
		[Export ("localEndpoint")]
		NWEndpoint LocalEndpoint { get; }

		[Export ("socketFamily")]
		int SocketFamily { get; set; }

		[Export ("socketType")]
		int SocketType { get; set; }

		[Export ("socketProtocol")]
		int SocketProtocol { get; set; }
	}

	[iOS (11,0), NoMac]
	[BaseType (typeof (NSObject))]
	interface NEFilterReport : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("flow")]
		NEFilterFlow Flow { get; }

		[Export ("action")]
		NEFilterAction Action { get; }
	}
#endif
				
	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NETunnelNetworkSettings))]
	[DisableDefaultCtor]
	interface NEPacketTunnelNetworkSettings {
		[Export ("initWithTunnelRemoteAddress:")]
		IntPtr Constructor (string address);

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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface NEPacketTunnelFlow {
		[Export ("readPacketsWithCompletionHandler:")]
		[Async (ResultType = typeof (NEPacketTunnelFlowReadResult))]
		void ReadPackets (Action<NSData[], NSNumber[]> completionHandler);

		[Export ("writePackets:withProtocols:")]
		bool WritePackets (NSData[] packets, NSNumber[] protocols);

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[Async]
		[Export ("readPacketObjectsWithCompletionHandler:")]
		void ReadPacketObjects (Action<NEPacket[]> completionHandler);

		[iOS (10,0)][Mac (10,12, onlyOn64 : true)]
		[Export ("writePacketObjects:")]
		bool WritePacketObjects (NEPacket[] packets);
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NETunnelProvider))]
	interface NEPacketTunnelProvider {
		[Export ("startTunnelWithOptions:completionHandler:")]
		[Async]
		void StartTunnel ([NullAllowed] NSDictionary<NSString,NSObject> options, Action<NSError> completionHandler);

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

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof (NEVpnProtocol))]
	interface NETunnelProviderProtocol {
		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSObject> ProviderConfiguration { get; set; }

		[NullAllowed, Export ("providerBundleIdentifier")]
		string ProviderBundleIdentifier { get; set; }
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(NEVpnConnection))]
	interface NETunnelProviderSession {
		[Export ("startTunnelWithOptions:andReturnError:")]
		bool StartTunnel ([NullAllowed] NSDictionary<NSString,NSObject> options, [NullAllowed] out NSError error);

		[Export ("stopTunnel")]
		void StopTunnel ();

		[Export ("sendProviderMessage:returnError:responseHandler:")]
		bool SendProviderMessage (NSData messageData, [NullAllowed] out NSError error, [NullAllowed] Action<NSData> responseHandler);
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12, onlyOn64 : true)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	interface NEPacket : NSCopying, NSSecureCoding {
		[Export ("initWithData:protocolFamily:")]
		IntPtr Constructor (NSData data, /* sa_family_t */ byte protocolFamily);

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[Export ("protocolFamily")]
		byte ProtocolFamily { get; }

		[NullAllowed, Export ("metadata")]
		NEFlowMetaData Metadata { get; }
	}

	[iOS (11,0), NoMac]
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

	[iOS (11,0), NoMac]
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
	}

	[iOS (11,0), Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NEVpnProtocol), Name = "NEDNSProxyProviderProtocol")]
	interface NEDnsProxyProviderProtocol {

		[NullAllowed, Export ("providerConfiguration", ArgumentSemantic.Copy)]
		NSDictionary ProviderConfiguration { get; set; }

		[NullAllowed, Export ("providerBundleIdentifier")]
		string ProviderBundleIdentifier { get; set; }
	}

	[iOS (11,0), NoMac]
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
		IntPtr Constructor (string domainName, bool roamingEnabled);
	}

	[iOS (11,0), NoMac]
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

	[iOS (11,0), NoMac]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NEHotspotConfiguration : NSCopying, NSSecureCoding {

		[Export ("SSID")]
		string Ssid { get; }

		[Export ("joinOnce")]
		bool JoinOnce { get; set; }

		[Export ("lifeTimeInDays", ArgumentSemantic.Copy)]
		NSNumber LifeTimeInDays { get; set; }

		[Export ("initWithSSID:")]
		IntPtr Constructor (string ssid);

		[Export ("initWithSSID:passphrase:isWEP:")]
		IntPtr Constructor (string ssid, string passphrase, bool isWep);

		[Export ("initWithSSID:eapSettings:")]
		IntPtr Constructor (string ssid, NEHotspotEapSettings eapSettings);

		[Export ("initWithHS20Settings:eapSettings:")]
		IntPtr Constructor (NEHotspotHS20Settings hs20Settings, NEHotspotEapSettings eapSettings);
	}

	[iOS (11,0), NoMac]
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


}
#endif
