// corewlan.cs: bindings for CoreWLAN
//
// Author:
//   Ashok Gelal, Chris Hamons
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using Foundation;
using CoreFoundation;
using ObjCRuntime;
using Security;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreWlan {

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWChannel : NSCoding, NSSecureCoding, NSCopying {
		[Export ("channelNumber")]
		nint ChannelNumber { get; }

		[Export ("channelWidth")]
		CWChannelWidth ChannelWidth { get; }

		[Export ("channelBand")]
		CWChannelBand ChannelBand { get; }

		[Export ("isEqualToChannel:")]
		bool IsEqualToChannel (CWChannel channel);
	}

#if !NET
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
	[BaseType (typeof (NSObject))]
	interface CW8021XProfile : NSCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("userDefinedName", ArgumentSemantic.Copy)]
		string UserDefinedName { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("ssid", ArgumentSemantic.Copy)]
		string Ssid { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("username", ArgumentSemantic.Copy)]
		string Username { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("password", ArgumentSemantic.Copy)]
		string Password { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("alwaysPromptForPassword")]
		bool AlwaysPromptForPassword { get; set; }

		[Static]
		[Export ("profile")]
		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
		CW8021XProfile Profile { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
		[Export ("isEqualToProfile:")]
		bool IsEqualToProfile (CW8021XProfile profile);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
		[Static]
		[Export ("allUser8021XProfiles")]
		CW8021XProfile [] AllUser8021XProfiles { get; }
	}
#endif

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWConfiguration : NSSecureCoding, NSMutableCopying {
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("rememberedNetworks")]
		NSSet RememberedNetworks { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("preferredNetworks")]
		CWWirelessProfile [] PreferredNetworks { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("alwaysRememberNetworks")]
		bool AlwaysRememberNetworks { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("disconnectOnLogout")]
		bool DisconnectOnLogout { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("requireAdminForNetworkChange")]
		bool RequireAdminForNetworkChange { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("requireAdminForPowerChange")]
		bool RequireAdminForPowerChange { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("requireAdminForIBSSCreation")]
		bool RequireAdminForIBSSCreation { get; set; }
#endif

		[Export ("networkProfiles", ArgumentSemantic.Copy)]
		[Internal]
		NSOrderedSet _NetworkProfiles { get; }

		[Export ("requireAdministratorForAssociation", ArgumentSemantic.Assign)]
		bool RequireAdministratorForAssociation { get; }

		[Export ("requireAdministratorForPower", ArgumentSemantic.Assign)]
		bool RequireAdministratorForPower { get; }

		[Export ("requireAdministratorForIBSSMode", ArgumentSemantic.Assign)]
		bool RequireAdministratorForIbssMode { get; }

		[Export ("rememberJoinedNetworks", ArgumentSemantic.Assign)]
		bool RememberJoinedNetworks { get; }

		[Export ("initWithConfiguration:")]
		NativeHandle Constructor (CWConfiguration configuration);

		[Export ("isEqualToConfiguration:")]
		bool IsEqualToConfiguration (CWConfiguration configuration);

		[Static]
		[Export ("configuration")]
		CWConfiguration Create ();

		[Static]
		[Export ("configurationWithConfiguration:")]
		CWConfiguration Create (CWConfiguration configuration);
	}

	[NoMacCatalyst]
	[BaseType (typeof (CWConfiguration))]
	interface CWMutableConfiguration {

		[Export ("networkProfiles", ArgumentSemantic.Copy)]
		NSOrderedSet<CWNetworkProfile> NetworkProfiles { get; set; }

		[Export ("requireAdministratorForAssociation")]
		bool RequireAdministratorForAssociation { get; set; }

		[Export ("requireAdministratorForPower", ArgumentSemantic.Assign)]
		bool RequireAdministratorForPower { get; set; }

		[Deprecated (PlatformName.MacOSX, 11, 0)]
		[Export ("requireAdministratorForIBSSMode", ArgumentSemantic.Assign)]
		bool RequireAdministratorForIbssMode { get; set; }

		[Export ("rememberJoinedNetworks", ArgumentSemantic.Assign)]
		bool RememberJoinedNetworks { get; set; }
	}

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWInterface {
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsWoW")]
		bool SupportsWow { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsWEP")]
		bool SupportsWep { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsAES_CCM")]
		bool SupportsAesCcm { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsIBSS")]
		bool SupportsIbss { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsTKIP")]
		bool SupportsTkip { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsPMGT")]
		bool SupportsPmgt { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsHostAP")]
		bool SupportsHostAP { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsMonitorMode")]
		bool SupportsMonitorMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsWPA")]
		bool SupportsWpa { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsWPA2")]
		bool SupportsWpa2 { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsWME")]
		bool SupportsWme { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsShortGI40MHz")]
		bool SupportsShortGI40MHz { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsShortGI20MHz")]
		bool SupportsShortGI20MHz { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportsTSN")]
		bool SupportsTsn { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("power")]
		bool Power { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("powerSave")]
		bool PowerSave { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("name")]
		string Name { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportedChannels")]
		NSNumber [] SupportedChannels { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("supportedPHYModes")]
		NSNumber [] SupportedPhyModes { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("channel")]
		NSNumber Channel { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("phyMode")]
		NSNumber PhyMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("bssidData")]
		NSData BssidData { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("rssi")]
		NSNumber Rssi { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("noise")]
		NSNumber Noise { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("txRate")]
		NSNumber TxRate { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("securityMode")]
		NSNumber SecurityMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("interfaceState")]
		NSNumber InterfaceState { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("opMode")]
		NSNumber OpMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("txPower")]
		NSNumber TxPower { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Static]
		[Export ("supportedInterfaces")]
		string [] SupportedInterfaces { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Static]
		[Export ("interface")]
		CWInterface MainInterface { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Static]
		[Export ("interfaceWithName:")]
		CWInterface FromName ([NullAllowed] string name);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("isEqualToInterface:")]
		bool IsEqualToInterface (CWInterface intface);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("setChannel:error:")]
		bool SetChannel (nuint channel, out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("scanForNetworksWithParameters:error:")]
		CWNetwork [] ScanForNetworksWithParameters ([NullAllowed] NSDictionary parameters, out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("associateToNetwork:parameters:error:")]
		bool AssociateToNetwork ([NullAllowed] CWNetwork network, [NullAllowed] NSDictionary parameters, out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("enableIBSSWithParameters:error:")]
		bool EnableIBSSWithParameters ([NullAllowed] NSDictionary parameters, out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("commitConfiguration:error:")]
		bool CommitConfiguration (CWConfiguration config, out NSError error);
#endif // !NET

		[Export ("powerOn", ArgumentSemantic.Assign)]
		bool PowerOn { get; }

		[NullAllowed]
		[Export ("interfaceName", ArgumentSemantic.Copy)]
		string InterfaceName { get; }

		[NullAllowed]
		[Export ("supportedWLANChannels")]
		[Internal]
		NSSet _SupportedWlanChannels { get; }

		[NullAllowed]
		[Export ("wlanChannel")]
		CWChannel WlanChannel { get; }

		[Export ("activePHYMode")]
		CWPhyMode ActivePHYMode { get; }

		[NullAllowed]
		[Export ("ssid")]
		string Ssid { get; }

		[NullAllowed]
		[Export ("ssidData")]
		NSData SsidData { get; }

		[NullAllowed]
		[Export ("bssid")]
		string Bssid { get; }

		[Export ("rssiValue")]
		nint RssiValue { get; }

		[Export ("noiseMeasurement")]
		nint NoiseMeasurement { get; }

		[Export ("security")]
		CWSecurity Security { get; }

		[Export ("transmitRate")]
		double TransmitRate { get; }

		[NullAllowed]
		[Export ("countryCode")]
		string CountryCode { get; }

		[Export ("interfaceMode")]
		CWInterfaceMode InterfaceMode { get; }

		[Export ("transmitPower")]
		nint TransmitPower { get; }

		[NullAllowed]
		[Export ("hardwareAddress")]
		string HardwareAddress { get; }

		[Export ("deviceAttached", ArgumentSemantic.Assign)]
		bool DeviceAttached { get; }

		[Export ("serviceActive")]
		bool ServiceActive { get; }

		[NullAllowed]
		[Export ("cachedScanResults")]
		[Internal]
		NSSet _CachedScanResults { get; }

		[NullAllowed]
		[Export ("configuration")]
		CWConfiguration Configuration { get; }

		[NullAllowed]
		[Static]
		[Export ("interfaceNames")]
		[Internal]
		NSSet _InterfaceNames { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'CWWiFiClient.FromName' instead.")]
		[Export ("initWithInterfaceName:")]
		NativeHandle Constructor ([NullAllowed] string name);

		[Export ("setPower:error:")]
		bool SetPower (bool power, out NSError error);

		[Export ("setWLANChannel:error:")]
		bool SetWlanChannel (CWChannel channel, out NSError error);

		[Export ("setPairwiseMasterKey:error:")]
		bool SetPairwiseMasterKey ([NullAllowed] NSData key, out NSError error);

		[Export ("setWEPKey:flags:index:error:")]
		bool SetWEPKey ([NullAllowed] NSData key, CWCipherKeyFlags flags, nint index, out NSError error);

		[return: NullAllowed]
		[Export ("scanForNetworksWithSSID:error:")]
		[Internal]
		NSSet _ScanForNetworksWithSsid ([NullAllowed] NSData ssid, out NSError error);

		[return: NullAllowed]
		[Export ("scanForNetworksWithName:error:")]
		[Internal]
		NSSet _ScanForNetworksWithName ([NullAllowed] string networkName, out NSError error);

		[Export ("associateToNetwork:password:error:")]
		bool AssociateToNetwork (CWNetwork network, [NullAllowed] string password, out NSError error);

		[Export ("associateToEnterpriseNetwork:identity:username:password:error:")]
		bool AssociateToEnterpriseNetwork (CWNetwork network, [NullAllowed] SecIdentity identity, [NullAllowed] string username, [NullAllowed] string password, out NSError error);

		[Deprecated (PlatformName.MacOSX, 11, 0)]
		[Export ("startIBSSModeWithSSID:security:channel:password:error:")]
		bool StartIbssModeWithSsid (NSData ssidData, CWIbssModeSecurity security, nuint channel, [NullAllowed] string password, out NSError error);

		[Export ("disassociate")]
		void Disassociate ();

		[Export ("commitConfiguration:authorization:error:")]
		bool CommitConfiguration (CWConfiguration configuration, [NullAllowed] NSObject authorization, out NSError error);

		[Export ("scanForNetworksWithSSID:includeHidden:error:")]
		[return: NullAllowed]
		[Internal]
		NSSet _ScanForNetworksWithSsid ([NullAllowed] NSData ssid, bool includeHidden, [NullAllowed] out NSError error);

		[Export ("scanForNetworksWithName:includeHidden:error:")]
		[return: NullAllowed]
		[Internal]
		NSSet _ScanForNetworksWithName ([NullAllowed] string networkName, bool includeHidden, [NullAllowed] out NSError error);
	}

#if !NET
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
	interface CWWirelessProfile : NSCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("ssid", ArgumentSemantic.Copy)]
		string Ssid { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("securityMode", ArgumentSemantic.Retain)]
		NSNumber SecurityMode { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("passphrase", ArgumentSemantic.Copy)]
		string Passphrase { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 10)]
		[Export ("user8021XProfile", ArgumentSemantic.Retain)]
		CW8021XProfile User8021XProfile { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 7)]
		[Export ("isEqualToProfile:")]
		bool IsEqualToProfile (CWWirelessProfile profile);
	}
#endif // !NET

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWNetwork : NSSecureCoding, NSCopying {
#if !NET
		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[NullAllowed]
		[Export ("bssidData")]
		NSData BssidData { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("securityMode")]
		NSNumber SecurityMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("phyMode")]
		NSNumber PhyMode { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("channel")]
		NSNumber Channel { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("rssi")]
		NSNumber Rssi { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("noise")]
		NSNumber Noise { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("ieData")]
		NSData IeData { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("isIBSS")]
		bool IsIBSS { get; }

		[Deprecated (PlatformName.MacOSX, 10, 7), Obsoleted (PlatformName.MacOSX, 10, 9)]
		[Export ("wirelessProfile")]
		CWWirelessProfile WirelessProfile { get; }
#endif

		[NullAllowed]
		[Export ("ssid")]
		string Ssid { get; }

		[NullAllowed]
		[Export ("ssidData")]
		NSData SsidData { get; }

		[NullAllowed]
		[Export ("bssid")]
		string Bssid { get; }

		[NullAllowed]
		[Export ("wlanChannel")]
		CWChannel WlanChannel { get; }

		[Export ("rssiValue")]
		nint RssiValue { get; }

		[Export ("noiseMeasurement")]
		nint NoiseMeasurement { get; }

		[NullAllowed]
		[Export ("informationElementData")]
		NSData InformationElementData { get; }

		[NullAllowed]
		[Export ("countryCode")]
		string CountryCode { get; }

		[Export ("beaconInterval")]
		nint BeaconInterval { get; }

		[Export ("ibss")]
		bool Ibss { get; }

		[Export ("isEqualToNetwork:")]
		bool IsEqualToNetwork (CWNetwork network);

		[Export ("supportsSecurity:")]
		bool SupportsSecurity (CWSecurity security);

		[Export ("supportsPHYMode:")]
		bool SupportsPhyMode (CWPhyMode phyMode);
	}

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWNetworkProfile : NSCoding, NSSecureCoding, NSCopying, NSMutableCopying {
		[NullAllowed]
		[Export ("ssid", ArgumentSemantic.Copy)]
		string Ssid { get; }

		[Export ("ssidData", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData SsidData { get; }

		[Export ("security", ArgumentSemantic.Assign)]
		CWSecurity Security { get; }

		[Static]
		[Export ("networkProfile")]
		NSObject NetworkProfile ();

		[Export ("initWithNetworkProfile:")]
		NativeHandle Constructor (CWNetworkProfile networkProfile);

		[Static]
		[Export ("networkProfileWithNetworkProfile:")]
		NSObject NetworkProfileWithNetworkProfile (CWNetworkProfile networkProfile);

		[Export ("isEqualToNetworkProfile:")]
		bool IsEqualToNetworkProfile (CWNetworkProfile networkProfile);
	}

	[NoMacCatalyst]
	[BaseType (typeof (CWNetworkProfile))]
	interface CWMutableNetworkProfile : NSCoding, NSSecureCoding, NSCopying, NSMutableCopying {
		[NullAllowed]
		[Export ("ssidData", ArgumentSemantic.Copy)]
		NSData SsidData { get; set; }

		[Export ("security", ArgumentSemantic.Assign)]
		CWSecurity Security { get; set; }
	}

	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CWWiFiClient {
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		ICWEventDelegate Delegate { get; set; }

		[Export ("interface")]
		[NullAllowed]
		CWInterface MainInterface { get; }

		[return: NullAllowed]
		[Export ("interfaceWithName:")]
		CWInterface FromName ([NullAllowed] string name);

		[Export ("interfaces")]
		[NullAllowed]
		CWInterface [] Interfaces { get; }

		[NullAllowed]
		[Export ("interfaceNames")]
		[Static]
		string [] InterfaceNames { get; }

		[Export ("sharedWiFiClient")]
		[Static]
		CWWiFiClient SharedWiFiClient { get; }

		[Export ("startMonitoringEventWithType:error:")]
		bool StartMonitoringEvent (CWEventType type, out NSError error);

		[Export ("stopMonitoringAllEventsAndReturnError:")]
		bool StopMonitoringAllEvents (out NSError error);

		[Export ("stopMonitoringEventWithType:error:")]
		bool StopMonitoringEvent (CWEventType type, out NSError error);
	}

	interface ICWEventDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CWEventDelegate {
		[Export ("clientConnectionInterrupted")]
		void ClientConnectionInterrupted ();

		[Export ("clientConnectionInvalidated")]
		void ClientConnectionInvalidated ();

		[Export ("powerStateDidChangeForWiFiInterfaceWithName:")]
		void PowerStateDidChangeForWiFi (string interfaceName);

		[Export ("ssidDidChangeForWiFiInterfaceWithName:")]
		void SsidDidChangeForWiFi (string interfaceName);

		[Export ("bssidDidChangeForWiFiInterfaceWithName:")]
		void BssidDidChangeForWiFi (string interfaceName);

		[Export ("countryCodeDidChangeForWiFiInterfaceWithName:")]
		void CountryCodeDidChangeForWiFi (string interfaceName);

		[Export ("linkDidChangeForWiFiInterfaceWithName:")]
		void LinkDidChangeForWiFi (string interfaceName);

		[Export ("linkQualityDidChangeForWiFiInterfaceWithName:rssi:transmitRate:")]
		void LinkQualityDidChangeForWiFi (string interfaceName, int rssi, double transmitRate);

		[Export ("modeDidChangeForWiFiInterfaceWithName:")]
		void ModeDidChangeForWiFi (string interfaceName);

		[Export ("scanCacheUpdatedForWiFiInterfaceWithName:")]
		void ScanCacheUpdatedForWiFi (string interfaceName);
	}
}
