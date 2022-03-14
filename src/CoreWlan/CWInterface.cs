// Copyright 2014 Xamarin Inc. All rights reserved.
#if !__MACCATALYST__

#nullable enable

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using System;

namespace CoreWlan {
	public unsafe partial class CWInterface {
		public CWChannel []? SupportedWlanChannels  {
			get {
				NSSet? channels = _SupportedWlanChannels;
				if (channels != null)
					return channels.ToArray<CWChannel> ();
				return null;
			}
		}

		public CWNetwork []? CachedScanResults {
			get {
				NSSet? results = _CachedScanResults;
				if (results != null)
					return results.ToArray<CWNetwork> ();
				return null;
			}
		}

		public static string []? InterfaceNames {
			get {
				NSSet? interfaceNames = _InterfaceNames;
				if (interfaceNames != null)
					return Array.ConvertAll (interfaceNames.ToArray<NSString> (), item => (string)item);
				return null;
			}
		}

		public CWNetwork []? ScanForNetworksWithSsid (NSData ssid, out NSError error)
		{
			NSSet? networks = _ScanForNetworksWithSsid (ssid, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}

		public CWNetwork []? ScanForNetworksWithName (string networkName, out NSError error)
		{
			NSSet? networks = _ScanForNetworksWithName (networkName, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}

#if NET
		[SupportedOSPlatform ("macos10.13")]
#else
		[Mac (10,13)]
#endif
		public CWNetwork []? ScanForNetworksWithSsid (NSData ssid, bool includeHidden, out NSError? error)
		{
			NSSet? networks = _ScanForNetworksWithSsid (ssid, includeHidden, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}

#if NET
		[SupportedOSPlatform ("macos10.13")]
#else
		[Mac (10,13)]
#endif
		public CWNetwork []? ScanForNetworksWithName (string networkName, bool includeHidden, out NSError? error)
		{
			NSSet? networks = _ScanForNetworksWithName (networkName, includeHidden, out error);
			if (networks != null)
				return networks.ToArray<CWNetwork> ();
			return null;
		}

	}
}
#endif // !__MACCATALYST__
