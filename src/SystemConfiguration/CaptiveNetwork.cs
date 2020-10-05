//
// CaptiveNetwork.cs: CaptiveNetwork binding
//
// Authors:
//	Miguel de Icaza (miguel@xamarin.com)
//	Sebastien Pouliot  <sebastien@xamarin.com>
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace SystemConfiguration {
	
	// http://developer.apple.com/library/ios/#documentation/SystemConfiguration/Reference/CaptiveNetworkRef/Reference/reference.html
	// CaptiveNetwork.h
	public static partial class CaptiveNetwork {

#if __TVOS__
		// in Xcode 10 the CaptiveNetwork API are marked as prohibited on tvOS
#if !XAMCORE_4_0
		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString NetworkInfoKeyBSSID => null;

		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString NetworkInfoKeySSID => null;

		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString NetworkInfoKeySSIDData => null;

		[Obsolete ("Throw a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static bool MarkPortalOffline (string iface) => throw new NotSupportedException ();

		[Obsolete ("Throw a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static bool MarkPortalOnline (string iface)  => throw new NotSupportedException ();

		[Obsolete ("Throw a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static bool SetSupportedSSIDs (string[] ssids) => throw new NotSupportedException ();

		[Obsolete ("Throw a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static StatusCode TryCopyCurrentNetworkInfo (string interfaceName, out Foundation.NSDictionary currentNetworkInfo)  => throw new NotSupportedException ();

		[Obsolete ("Throw a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static StatusCode TryGetSupportedInterfaces (out string[] supportedInterfaces)  => throw new NotSupportedException ();
#endif
#else
		
#if !MONOMAC

		[Deprecated (PlatformName.iOS, 14,0)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* CFDictionaryRef __nullable */  CNCopyCurrentNetworkInfo (
			/* CFStringRef __nonnull */ IntPtr interfaceName);

		[Deprecated (PlatformName.iOS, 14,0)]
		static public StatusCode TryCopyCurrentNetworkInfo (string interfaceName, out NSDictionary currentNetworkInfo)
		{
			using (var nss = new NSString (interfaceName)) {
				var ni = CNCopyCurrentNetworkInfo (nss.Handle);
				if (ni == IntPtr.Zero) {
					currentNetworkInfo = null;
					return StatusCodeError.SCError ();
				}
					
				currentNetworkInfo = new NSDictionary (ni);

				// Must release since the IntPtr constructor calls Retain
				currentNetworkInfo.DangerousRelease ();
				return StatusCode.OK;
			}
		}

#endif
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* CFArrayRef __nullable */ CNCopySupportedInterfaces ();

		[Deprecated (PlatformName.iOS, 14,0, message: "Use 'NEHotspotNetwork.FetchCurrent' instead.")]
		static public StatusCode TryGetSupportedInterfaces (out string[] supportedInterfaces)
		{
			IntPtr array = CNCopySupportedInterfaces ();
			if (array == IntPtr.Zero) {
				supportedInterfaces = null;
				return StatusCodeError.SCError ();
			}
			
			supportedInterfaces = NSArray.StringArrayFromHandle (array);
			CFObject.CFRelease (array);
			return StatusCode.OK;
		}

		[Deprecated (PlatformName.iOS, 9,0)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNMarkPortalOffline (IntPtr /* CFStringRef __nonnull */ interfaceName);

		[Deprecated (PlatformName.iOS, 9,0)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNMarkPortalOnline (IntPtr /* CFStringRef __nonnull */ interfaceName);

		[Deprecated (PlatformName.iOS, 9,0)]
		static public bool MarkPortalOnline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOnline (nss.Handle);
			}
		}

		[Deprecated (PlatformName.iOS, 9,0)]
		static public bool MarkPortalOffline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOffline (nss.Handle);
			}
		}

		[Deprecated (PlatformName.iOS, 9,0)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNSetSupportedSSIDs (IntPtr /* CFArrayRef __nonnull */ ssidArray);

		[Deprecated (PlatformName.iOS, 9,0)]
		static public bool SetSupportedSSIDs (string [] ssids)
		{
			using (var arr = NSArray.FromStrings (ssids)) {
				return CNSetSupportedSSIDs (arr.Handle);
			}
		}
#endif // __TVOS__
	}
}
