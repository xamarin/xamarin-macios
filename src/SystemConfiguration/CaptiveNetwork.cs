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

#nullable enable

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
#if !NET
		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString? NetworkInfoKeyBSSID => null;

		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString? NetworkInfoKeySSID => null;

		[Obsolete ("Always return 'null'.")]
		[Unavailable (PlatformName.TvOS)]
		public static Foundation.NSString? NetworkInfoKeySSIDData => null;

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
#endif // !NET
#else
		
#if !MONOMAC

#if NET
		[UnsupportedOSPlatform ("ios14.0")]
#if IOS
		[Obsolete ("Starting with ios14.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14,0)]
#endif
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* CFDictionaryRef __nullable */  CNCopyCurrentNetworkInfo (
			/* CFStringRef __nonnull */ IntPtr interfaceName);

#if NET
		[UnsupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("maccatalyst14.0")]
#if IOS
		[Obsolete ("Starting with ios14.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14,0)]
#endif
		static public StatusCode TryCopyCurrentNetworkInfo (string interfaceName, out NSDictionary? currentNetworkInfo)
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

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("ios14.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios14.0 use 'NEHotspotNetwork.FetchCurrent' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 14,0, message: "Use 'NEHotspotNetwork.FetchCurrent' instead.")]
#endif
		static public StatusCode TryGetSupportedInterfaces (out string?[]? supportedInterfaces)
		{
			IntPtr array = CNCopySupportedInterfaces ();
			if (array == IntPtr.Zero) {
				supportedInterfaces = null;
				return StatusCodeError.SCError ();
			}
			
			supportedInterfaces = CFArray.StringArrayFromHandle (array);
			CFObject.CFRelease (array);
			return StatusCode.OK;
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool CNMarkPortalOffline (IntPtr /* CFStringRef __nonnull */ interfaceName);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool CNMarkPortalOnline (IntPtr /* CFStringRef __nonnull */ interfaceName);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		static public bool MarkPortalOnline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOnline (nss.Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		static public bool MarkPortalOffline (string iface)
		{
			using (var nss = new NSString (iface)) {
				return CNMarkPortalOffline (nss.Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		[DllImport (Constants.SystemConfigurationLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static bool CNSetSupportedSSIDs (IntPtr /* CFArrayRef __nonnull */ ssidArray);

#if NET
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("ios9.0")]
		[UnsupportedOSPlatform ("maccatalyst")]
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 9,0)]
#endif
		static public bool SetSupportedSSIDs (string [] ssids)
		{
			using (var arr = NSArray.FromStrings (ssids)) {
				return CNSetSupportedSSIDs (arr.Handle);
			}
		}
#endif // __TVOS__
	}
}
