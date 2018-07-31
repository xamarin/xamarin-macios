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

		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* CFDictionaryRef __nullable */  CNCopyCurrentNetworkInfo (
			/* CFStringRef __nonnull */ IntPtr interfaceName);

#if !XAMCORE_2_0
		[Obsolete ("Replaced by TryCopyCurrentNetworkInfo")]
		static public NSDictionary CopyCurrentNetworkInfo (string interfaceName)
		{
			NSDictionary dict;
			TryCopyCurrentNetworkInfo (interfaceName, out dict);
			return dict ?? new NSDictionary ();
		}
#endif
		
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
		[Mac (10,8)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* CFArrayRef __nullable */ CNCopySupportedInterfaces ();
		
#if !XAMCORE_2_0
		[Obsolete ("Replaced by 'TryGetSupportedInterfaces'.")]
		[Mac (10,8)]
		static public string[] GetSupportedInterfaces ()
		{
			string[] result;
			TryGetSupportedInterfaces (out result);
			return result;
		}
#endif
		
		[Mac (10,8)]
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

		[Mac (10,8)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNMarkPortalOffline (IntPtr /* CFStringRef __nonnull */ interfaceName);

		[Mac (10,8)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNMarkPortalOnline (IntPtr /* CFStringRef __nonnull */ interfaceName);

		[Mac (10,8)]
		static public bool MarkPortalOnline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOnline (nss.Handle);
			}
		}

		[Mac (10,8)]
		static public bool MarkPortalOffline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOffline (nss.Handle);
			}
		}

		[Mac (10,8)]
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static bool CNSetSupportedSSIDs (IntPtr /* CFArrayRef __nonnull */ ssidArray);

		[Mac (10,8)]
		static public bool SetSupportedSSIDs (string [] ssids)
		{
			using (var arr = NSArray.FromStrings (ssids)) {
				return CNSetSupportedSSIDs (arr.Handle);
			}
		}
#endif // __TVOS__
	}
}
