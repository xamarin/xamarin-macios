// Copyright 2014 Xamarin Inc. All rights reserved.

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using System;

namespace XamCore.CoreWlan {
	public unsafe partial class CWInterface {
		public CWChannel [] SupportedWlanChannels  {
			get {
				NSSet channels = _SupportedWlanChannels;
				if (channels != null)
					return channels.ToArray<CWChannel> ();
				return null;
			}
		}

		public CWNetwork [] CachedScanResults {
			get {
				NSSet results = _CachedScanResults;
				if (results != null)
					return results.ToArray<CWNetwork> ();
				return null;
			}
		}

		public static string [] InterfaceNames {
			get {
				NSSet interfaceNames = _InterfaceNames;
				if (interfaceNames != null)
					return Array.ConvertAll (interfaceNames.ToArray<NSString> (), item => (string)item);
				return null;
			}
		}

		public CWNetwork [] ScanForNetworksWithSsid (NSData ssid, out NSError error) 
		{
			NSSet networks = _ScanForNetworksWithSsid (ssid, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}

		public CWNetwork [] ScanForNetworksWithName (string networkName, out NSError error) 
		{
			NSSet networks = _ScanForNetworksWithName (networkName, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}
	}
}
