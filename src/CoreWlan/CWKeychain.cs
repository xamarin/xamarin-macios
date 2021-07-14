using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
using Security;

#if NET
using System.Runtime.Versioning;
#endif


#nullable enable

using OSStatus = System.Int32;
using SecIdentityRef = System.IntPtr;
using CFArrayRef = System.IntPtr;
using NSDataRef = System.IntPtr;
using NSStringRef = System.IntPtr;

namespace CoreWlan {
#if NET
	[SupportedOSPlatform ("maccatalyst15.0")]
#else
	[MacCatalyst (15,0)]
#endif
	public static partial class CWKeychain {

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainCopyWiFiEAPIdentity (CWKeychainDomain domain, NSDataRef ssid, out SecIdentityRef identity);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, out SecIdentity? identity, out OSStatus status)
		{
			identity = null;
			IntPtr outPtr = IntPtr.Zero;
			status = CWKeychainCopyWiFiEAPIdentity (domain, ssid.GetHandle (), out outPtr);
			if (status == 0) {
				identity = new SecIdentity (outPtr, true);
			}

			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, out SecIdentity? identity)
			=> TryFindWiFiEAPIdentity (domain, ssid, out identity, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainDeleteWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSDataRef ssid);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryDeleteWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, out OSStatus status)
		{
			status = CWKeychainDeleteWiFiEAPUsernameAndPassword (domain, ssid.GetHandle ());
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryDeleteWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid)
			=> TryDeleteWiFiEAPUsernameAndPassword (domain, ssid, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainDeleteWiFiPassword (CWKeychainDomain domain, NSDataRef ssid);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryDeleteWiFiPassword (CWKeychainDomain domain, NSData ssid, out OSStatus status)
		{
			status = CWKeychainDeleteWiFiPassword (domain, ssid.GetHandle ());
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryDeleteWiFiPassword (CWKeychainDomain domain, NSData ssid)
			=> TryDeleteWiFiPassword (domain, ssid, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainFindWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSDataRef ssid, out NSStringRef username, out NSStringRef password);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, out NSString? username, out NSString? password, out OSStatus status)
		{
			username = null;
			password = null;
			NSStringRef usernamePtr = IntPtr.Zero;
			NSStringRef passwordPtr = IntPtr.Zero;
			status = CWKeychainFindWiFiEAPUsernameAndPassword (domain, ssid.GetHandle (), out usernamePtr, out passwordPtr);
			if (usernamePtr != IntPtr.Zero) {
				username = Runtime.GetNSObject<NSString> (usernamePtr, false);
			}
			if (passwordPtr != IntPtr.Zero) {
				password = Runtime.GetNSObject<NSString> (passwordPtr, false);
			}
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, out NSString? username, out NSString? password)
			=> TryFindWiFiEAPUsernameAndPassword (domain, ssid, out username, out password, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainFindWiFiPassword (CWKeychainDomain domain, NSDataRef ssid, out NSStringRef password);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiPassword (CWKeychainDomain domain, NSData ssid, out NSString? password, out OSStatus status)
		{
			password = null;
			NSStringRef passwordPtr = IntPtr.Zero;
			status = CWKeychainFindWiFiPassword (domain, ssid.GetHandle (), out passwordPtr);
			if (passwordPtr != IntPtr.Zero) {
				password = Runtime.GetNSObject<NSString> (passwordPtr, false);
			}
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TryFindWiFiPassword (CWKeychainDomain domain, NSData ssid, out NSString? password)
			=> TryFindWiFiPassword (domain, ssid, out password, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainSetWiFiEAPIdentity (CWKeychainDomain domain, NSDataRef ssid, SecIdentityRef identity);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, SecIdentity? identity, out OSStatus status)
		{
			status = CWKeychainSetWiFiEAPIdentity (domain, ssid.GetHandle (), identity.GetHandle ()!);
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, SecIdentity? identity)
			=> TrySetWiFiEAPIdentity (domain, ssid, identity, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainSetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSDataRef ssid, NSStringRef username, NSStringRef password);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, NSString? username, NSString? password, out OSStatus status)
		{
			status = CWKeychainSetWiFiEAPUsernameAndPassword (domain, ssid.GetHandle (), username.GetHandle ()!, password.GetHandle ()!);
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, NSString? username, NSString? password)
			=>  TrySetWiFiEAPUsernameAndPassword (domain, ssid, username, password, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10, 9)]
#endif
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, string? username, string? password, out OSStatus status)
		{
			status = CWKeychainSetWiFiEAPUsernameAndPassword (domain, ssid.GetHandle (), CFString.CreateNative (username), CFString.CreateNative (password));
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10, 9)]
#endif
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, string? username, string? password)
			=> TrySetWiFiEAPUsernameAndPassword (domain, ssid, username, password, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainSetWiFiPassword (CWKeychainDomain domain, NSDataRef ssid, NSStringRef password);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, NSString password, out OSStatus status)
		{
			status = CWKeychainSetWiFiPassword (domain, ssid.GetHandle (), password.GetHandle ());
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, NSString password)
			=> TrySetWiFiPassword (domain, ssid, password, out var _);

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, string password, out OSStatus status)
		{
			status = CWKeychainSetWiFiPassword (domain, ssid.GetHandle (), CFString.CreateNative (password));
			return status == 0;
		}

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, string password)
			=> TrySetWiFiPassword (domain, ssid, password, out var _);


		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainCopyEAPIdentityList (CFArrayRef list);

		public static bool TryGetEAPIdentityList (NSArray? list, out OSStatus status)
		{
			status = CWKeychainCopyEAPIdentityList (list.GetHandle ()!);
			return status == 0;
		}

		public static bool TryGetEAPIdentityList (NSArray? list)
			=> TryGetEAPIdentityList (list, out var _);
	}
}
