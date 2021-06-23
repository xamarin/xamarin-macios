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
		public static bool FindWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, out SecIdentity? identity)
		{
			identity = null;
			IntPtr outPtr = IntPtr.Zero;
			var result = CWKeychainCopyWiFiEAPIdentity (domain, ssid.GetHandle (), out outPtr);
			if (result == 0)
			{
				identity = new SecIdentity (outPtr, true);
			}

			return result == 0;
		}

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
		public static bool TryDeleteWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid)
			=> CWKeychainDeleteWiFiEAPUsernameAndPassword (domain, ssid.GetHandle ()) == 0;

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
		public static bool TryDeleteWiFiPassword (CWKeychainDomain domain, NSData ssid)
			=> CWKeychainDeleteWiFiPassword (domain, ssid.GetHandle ()) == 0;

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
		public static bool TryFindWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, out NSString? username, out NSString? password)
		{
			username = null;
			password = null;
			NSStringRef usernamePtr = IntPtr.Zero;
			NSStringRef passwordPtr = IntPtr.Zero;
			var result = CWKeychainFindWiFiEAPUsernameAndPassword (domain, ssid.GetHandle (), out usernamePtr, out passwordPtr);
			if (usernamePtr != IntPtr.Zero) {
				username = Runtime.GetNSObject<NSString> (usernamePtr, true);
			}
			if (passwordPtr != IntPtr.Zero) {
				password= Runtime.GetNSObject<NSString> (passwordPtr, true);
			}
			return result == 0;
		}

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
		public static bool TryFindWiFiPassword (CWKeychainDomain domain, NSData ssid, out NSString? password)
		{
			password = null;
			NSStringRef passwordPtr = IntPtr.Zero;
			var result = CWKeychainFindWiFiPassword (domain, ssid.GetHandle (), out passwordPtr);
			if (passwordPtr != IntPtr.Zero) {
				password= Runtime.GetNSObject<NSString> (passwordPtr, true);
			}
			return result == 0;
		}

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
		public static bool TrySetWiFiEAPIdentity (CWKeychainDomain domain, NSData ssid, SecIdentity? identity)
			=> CWKeychainSetWiFiEAPIdentity (domain, ssid.GetHandle (), identity.GetHandle ()!) == 0;

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
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, NSString? username, NSString? password)
			=> CWKeychainSetWiFiEAPUsernameAndPassword (domain, ssid.GetHandle (), username.GetHandle ()!, password.GetHandle ()!) == 0;

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10, 9)]
#endif
		public static bool TrySetWiFiEAPUsernameAndPassword (CWKeychainDomain domain, NSData ssid, string? username, string? password)
		{
			using (NSString? nsUsername = (username == null)? null : new NSString (username))
			using (NSString? nsPassword = (password == null)? null : new NSString (password)) {
				return TrySetWiFiEAPUsernameAndPassword (domain, ssid, nsUsername, nsPassword);
			}
		}

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
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, NSString password)
			=> CWKeychainSetWiFiPassword (domain, ssid.GetHandle (), password.GetHandle ()) == 0;

#if NET
		[SupportedOSPlatform ("macos10.9")]
#else
		[Mac (10,9)]
#endif
		public static bool TrySetWiFiPassword (CWKeychainDomain domain, NSData ssid, string password)
		{
			using (NSString nsPassword = new NSString (password)) {
				return TrySetWiFiPassword (domain, ssid, nsPassword);
			}
		}


#if NET
		[SupportedOSPlatform ("macos10.7")]
#else
		[Mac (10,7)]
#endif
		[DllImport (Constants.CoreWlanLibrary)]
		static extern OSStatus CWKeychainCopyEAPIdentityList (CFArrayRef list);

#if NET
		[SupportedOSPlatform ("macos10.7")]
#else
		[Mac (10,7)]
#endif
		public static bool TryGetEAPIdentityList (NSArray? list)
			=> CWKeychainCopyEAPIdentityList (list.GetHandle ()!) == 0;
	}
}
