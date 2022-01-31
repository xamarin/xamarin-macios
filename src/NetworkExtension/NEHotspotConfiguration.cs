// Copyright 2019 Microsoft Corporation

#if !MONOMAC

using System.Runtime.Versioning;
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

		[iOS (13,0)]
		public NEHotspotConfiguration (string ssid, bool ssidIsPrefix)
		{
			var h = ssidIsPrefix ? initWithSsidPrefix (ssid) : initWithSsid (ssid);
			InitializeHandle (h);
		}

		[iOS (13,0)]
		public NEHotspotConfiguration (string ssid, string passphrase, bool isWep, bool ssidIsPrefix)
		{
			var h = ssidIsPrefix ? initWithSsidPrefix (ssid, passphrase, isWep) : initWithSsid (ssid, passphrase, isWep);
			InitializeHandle (h);
		}
	}
}

#endif
