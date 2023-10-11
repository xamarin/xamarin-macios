// Copyright 2019 Microsoft Corporation

#nullable enable

#if !MONOMAC && !TVOS

using Foundation;

namespace NetworkExtension {

	public partial class NEHotspotConfiguration {

		public NEHotspotConfiguration (string ssid)
		{
			InitializeHandle (initWithSsid (ssid));
		}

		public NEHotspotConfiguration (string ssid, string passphrase, bool isWep)
		{
			InitializeHandle (initWithSsid (ssid, passphrase, isWep));
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13, 0)]
#endif
		public NEHotspotConfiguration (string ssid, bool ssidIsPrefix)
		{
			var h = ssidIsPrefix ? initWithSsidPrefix (ssid) : initWithSsid (ssid);
			InitializeHandle (h);
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[iOS (13, 0)]
#endif
		public NEHotspotConfiguration (string ssid, string passphrase, bool isWep, bool ssidIsPrefix)
		{
			var h = ssidIsPrefix ? initWithSsidPrefix (ssid, passphrase, isWep) : initWithSsid (ssid, passphrase, isWep);
			InitializeHandle (h);
		}
	}
}

#endif
