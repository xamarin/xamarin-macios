// 
// Items.cs: Implements the KeyChain query access APIs
//
// We use strong types and a helper SecQuery class to simplify the
// creation of the dictionary used to query the Keychain
// 
// Authors:
//	Miguel de Icaza
//	Sebastien Pouliot
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2016 Xamarin Inc
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

#nullable enable

using System;
using System.Collections;
using System.Runtime.InteropServices;

using CoreFoundation;

using Foundation;

using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {

	public enum SecKind {
		InternetPassword,
		GenericPassword,
		Certificate,
		Key,
		Identity
	}

	// manually mapped to KeysAccessible
	public enum SecAccessible {
		Invalid = -1,
		WhenUnlocked,
		AfterFirstUnlock,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.14", "Use 'AfterFirstUnlock' or a better suited option instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Use 'AfterFirstUnlock' or a better suited option instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'AfterFirstUnlock' or a better suited option instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'AfterFirstUnlock' or a better suited option instead.")]
#endif
		Always,
		WhenUnlockedThisDeviceOnly,
		AfterFirstUnlockThisDeviceOnly,
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.14", "Use 'AfterFirstUnlockThisDeviceOnly' or a better suited option instead.")]
		[ObsoletedOSPlatform ("ios12.0", "Use 'AfterFirstUnlockThisDeviceOnly' or a better suited option instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'AfterFirstUnlockThisDeviceOnly' or a better suited option instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'AfterFirstUnlockThisDeviceOnly' or a better suited option instead.")]
#endif
		AlwaysThisDeviceOnly,
		WhenPasscodeSetThisDeviceOnly
	}

	public enum SecProtocol {
		Invalid = -1,
		Ftp, FtpAccount, Http, Irc, Nntp, Pop3, Smtp, Socks, Imap, Ldap, AppleTalk, Afp, Telnet, Ssh,
		Ftps, Https, HttpProxy, HttpsProxy, FtpProxy, Smb, Rtsp, RtspProxy, Daap, Eppc, Ipp,
		Nntps, Ldaps, Telnets, Imaps, Ircs, Pop3s,
	}

	public enum SecAuthenticationType {
		Invalid = -1,
		Any = 0,
		Ntlm = 1835824238,
		Msn = 1634628461,
		Dpa = 1633775716,
		Rpa = 1633775730,
		HttpBasic = 1886680168,
		HttpDigest = 1685353576,
		HtmlForm = 1836216166,
		Default = 1953261156,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class SecKeyChain : INativeObject {

		internal SecKeyChain (NativeHandle handle)
		{
			Handle = handle;
		}

		public NativeHandle Handle { get; internal set; }

		static NSNumber? SetLimit (NSMutableDictionary dict, int max)
		{
			NSNumber? n = null;
			IntPtr val;
			if (max == -1)
				val = SecMatchLimit.MatchLimitAll;
			else if (max == 1)
				val = SecMatchLimit.MatchLimitOne;
			else {
				n = NSNumber.FromInt32 (max);
				val = n.Handle;
			}

			dict.LowlevelSetObject (val, SecItem.MatchLimit);
			return n;
		}

		public static NSData? QueryAsData (SecRecord query, bool wantPersistentReference, out SecStatusCode status)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				SetLimit (copy, 1);
				if (wantPersistentReference)
					copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnPersistentRef);
				else
					copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnData);

				status = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				if (status == SecStatusCode.Success)
					return Runtime.GetNSObject<NSData> (ptr, true);
				return null;
			}
		}

		public static NSData []? QueryAsData (SecRecord query, bool wantPersistentReference, int max, out SecStatusCode status)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				var n = SetLimit (copy, max);
				if (wantPersistentReference)
					copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnPersistentRef);
				else
					copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnData);

				status = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				n = null;
				if (status == SecStatusCode.Success) {
					// From the header docs, it's guaranteed the function will return an array only if we pass max > 1.

					// By default, this function returns only the first match found.
					// To obtain more than one matching item at a time, specify the search key kSecMatchLimit with a value greater than 1.
					// The result will be an object of type CFArrayRef containing up to that number of matching items.
					if (max == 1)
						return new NSData [] { Runtime.GetNSObject<NSData> (ptr, true)! };

					return CFArray.ArrayFromHandle<NSData> (ptr, true)!;
				}
				return null;
			}
		}

		public static NSData? QueryAsData (SecRecord query)
		{
			SecStatusCode status;
			return QueryAsData (query, false, out status);
		}

		public static NSData []? QueryAsData (SecRecord query, int max)
		{
			SecStatusCode status;
			return QueryAsData (query, false, max, out status);
		}

		public static SecRecord? QueryAsRecord (SecRecord query, out SecStatusCode result)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				SetLimit (copy, 1);
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnAttributes);
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnData);
				result = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				if (result == SecStatusCode.Success)
					return new SecRecord (new NSMutableDictionary (ptr, true));
				return null;
			}
		}

		public static SecRecord []? QueryAsRecord (SecRecord query, int max, out SecStatusCode result)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnAttributes);
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnData);
				var n = SetLimit (copy, max);

				result = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				n = null;
				if (result == SecStatusCode.Success)
					return CFArray.ArrayFromHandleFunc<SecRecord> (ptr, (element) => {
						var dictionary = Runtime.GetNSObject<NSMutableDictionary> (element, false)!;
						return new SecRecord (dictionary);
					}, releaseHandle: true)!;
				return null;
			}
		}

		public static INativeObject []? QueryAsReference (SecRecord query, int max, out SecStatusCode result)
		{
			if (query is null) {
				result = SecStatusCode.Param;
				return null;
			}

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnRef);
				SetLimit (copy, max);

				result = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				if ((result == SecStatusCode.Success) && (ptr != IntPtr.Zero)) {
					var array = CFArray.ArrayFromHandleFunc<INativeObject> (ptr, p => {
						nint cfType = CFType.GetTypeID (p);
						CFObject.CFRetain (p);

						if (cfType == SecCertificate.GetTypeID ())
							return new SecCertificate (p, true);
						else if (cfType == SecKey.GetTypeID ())
							return new SecKey (p, true);
						else if (cfType == SecIdentity.GetTypeID ())
							return new SecIdentity (p, true);
						else {
							CFObject.CFRelease (p);
							throw new Exception (String.Format ("Unexpected type: 0x{0:x}", cfType));
						}
					}, releaseHandle: true)!;
					return array;
				}
				return null;
			}
		}

		public static SecStatusCode Add (SecRecord record)
		{
			if (record is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (record));
			return SecItem.SecItemAdd (record.queryDict.Handle, IntPtr.Zero);

		}

		public static SecStatusCode Remove (SecRecord record)
		{
			if (record is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (record));
			return SecItem.SecItemDelete (record.queryDict.Handle);
		}

		public static SecStatusCode Update (SecRecord query, SecRecord newAttributes)
		{
			if (query is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (query));
			if (newAttributes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (newAttributes));

			return SecItem.SecItemUpdate (query.queryDict.Handle, newAttributes.queryDict.Handle);

		}
#if MONOMAC
#if NET
		[ObsoletedOSPlatform ("macos10.10")]
#else
		[Deprecated (PlatformName.MacOSX, 10,10)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainAddGenericPassword (
			IntPtr keychain,
			int serviceNameLength,
			byte[]? serviceName,
			int accountNameLength,
			byte[]? accountName,
			int passwordLength,
			byte[] passwordData,
			IntPtr itemRef);

#if NET
		[ObsoletedOSPlatform ("macos10.10")]
#else
		[Deprecated (PlatformName.MacOSX, 10,10)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainFindGenericPassword (
			IntPtr keychainOrArray,
			int serviceNameLength,
			byte[]? serviceName,
			int accountNameLength,
			byte[]? accountName,
			out int passwordLength,
			out IntPtr passwordData,
			IntPtr itemRef);

#if NET
		[ObsoletedOSPlatform ("macos10.10")]
#else
		[Deprecated (PlatformName.MacOSX, 10,10)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainAddInternetPassword (
			IntPtr keychain,
			int serverNameLength,
			byte[]? serverName,
			int securityDomainLength,
			byte[]? securityDomain,
			int accountNameLength,
			byte[]? accountName,
			int pathLength,
			byte[]? path,
			short port,
			IntPtr protocol,
			IntPtr authenticationType,
			int passwordLength,
			byte[] passwordData,
			IntPtr itemRef);

#if NET
		[ObsoletedOSPlatform ("macos10.10")]
#else
		[Deprecated (PlatformName.MacOSX, 10,10)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainFindInternetPassword (
			IntPtr keychain,
			int serverNameLength,
			byte[]? serverName,
			int securityDomainLength,
			byte[]? securityDomain,
			int accountNameLength,
			byte[]? accountName,
			int pathLength,
			byte[]? path,
			short port,
			IntPtr protocol,
			IntPtr authenticationType,
			out int passwordLength,
			out IntPtr passwordData,
			IntPtr itemRef);

#if NET
		[ObsoletedOSPlatform ("macos10.10")]
#else
		[Deprecated (PlatformName.MacOSX, 10,10)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SecStatusCode SecKeychainItemFreeContent (IntPtr attrList, IntPtr data);

		public static SecStatusCode AddInternetPassword (
			string serverName,
			string accountName,
			byte[] password,
			SecProtocol protocolType = SecProtocol.Http,
			short port = 0,
			string? path = null,
			SecAuthenticationType authenticationType = SecAuthenticationType.Default,
			string? securityDomain = null)
		{
			byte[]? serverNameBytes = null;
			byte[]? securityDomainBytes = null;
			byte[]? accountNameBytes = null;
			byte[]? pathBytes = null;
			
			if (!String.IsNullOrEmpty (serverName))
				serverNameBytes = System.Text.Encoding.UTF8.GetBytes (serverName);
			
			if (!String.IsNullOrEmpty (securityDomain))
				securityDomainBytes = System.Text.Encoding.UTF8.GetBytes (securityDomain);
			
			if (!String.IsNullOrEmpty (accountName))
				accountNameBytes = System.Text.Encoding.UTF8.GetBytes (accountName);
			
			if (!String.IsNullOrEmpty (path))
				pathBytes = System.Text.Encoding.UTF8.GetBytes (path);
			
			return SecKeychainAddInternetPassword (
				IntPtr.Zero,
				serverNameBytes?.Length ?? 0,
				serverNameBytes,
				securityDomainBytes?.Length ?? 0,
				securityDomainBytes,
				accountNameBytes?.Length ?? 0,
				accountNameBytes,
				pathBytes?.Length ?? 0,
				pathBytes,
				port,
				SecProtocolKeys.FromSecProtocol (protocolType),
				KeysAuthenticationType.FromSecAuthenticationType (authenticationType),
				password?.Length ?? 0,
				password!,
				IntPtr.Zero);
		}
		
		
		public static SecStatusCode FindInternetPassword(
			string serverName,
			string accountName,
			out byte[]? password,
			SecProtocol protocolType = SecProtocol.Http,
			short port = 0,
			string? path = null,
			SecAuthenticationType authenticationType = SecAuthenticationType.Default,
			string? securityDomain = null)
		{
			password = null;
			
			byte[]? serverBytes = null;
			byte[]? securityDomainBytes = null;
			byte[]? accountNameBytes = null;
			byte[]? pathBytes = null;

			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				if (!String.IsNullOrEmpty (serverName))
					serverBytes = System.Text.Encoding.UTF8.GetBytes (serverName);
				
				if (!String.IsNullOrEmpty (securityDomain))
					securityDomainBytes = System.Text.Encoding.UTF8.GetBytes (securityDomain);
				
				if (!String.IsNullOrEmpty (accountName))
					accountNameBytes = System.Text.Encoding.UTF8.GetBytes (accountName);
				
				if (!String.IsNullOrEmpty(path))
					pathBytes = System.Text.Encoding.UTF8.GetBytes (path);
				
				int passwordLength = 0;
				
				SecStatusCode code = SecKeychainFindInternetPassword(
					IntPtr.Zero,
					serverBytes?.Length ?? 0,
					serverBytes,
					securityDomainBytes?.Length ?? 0,
					securityDomainBytes,
					accountNameBytes?.Length ?? 0,
					accountNameBytes,
					pathBytes?.Length ?? 0,
					pathBytes,
					port,
					SecProtocolKeys.FromSecProtocol(protocolType),
					KeysAuthenticationType.FromSecAuthenticationType(authenticationType),
					out passwordLength,
					out passwordPtr,
					IntPtr.Zero);
				
				if (code == SecStatusCode.Success && passwordLength > 0) {
					password = new byte[passwordLength];
					Marshal.Copy(passwordPtr, password, 0, passwordLength);
				}
				
				return code;
				
			} finally {
				if (passwordPtr != IntPtr.Zero)
					SecKeychainItemFreeContent(IntPtr.Zero, passwordPtr);
			}
		}

		public static SecStatusCode AddGenericPassword (string serviceName, string accountName, byte[] password)
		{
			byte[]? serviceNameBytes = null;
			byte[]? accountNameBytes = null;
			
			if (!String.IsNullOrEmpty (serviceName))
				serviceNameBytes = System.Text.Encoding.UTF8.GetBytes (serviceName);

			if (!String.IsNullOrEmpty (accountName))
				accountNameBytes = System.Text.Encoding.UTF8.GetBytes (accountName);

			return SecKeychainAddGenericPassword(
				IntPtr.Zero,
				serviceNameBytes?.Length ?? 0,
				serviceNameBytes,
				accountNameBytes?.Length ?? 0,
				accountNameBytes,
				password?.Length ?? 0,
				password!,
				IntPtr.Zero
				);
		}

		public static SecStatusCode FindGenericPassword (string serviceName, string accountName, out byte[]? password)
		{
			password = null;

			byte[]? serviceNameBytes = null;
			byte[]? accountNameBytes = null;
			
			IntPtr passwordPtr = IntPtr.Zero;
			
			try {
				
				if (!String.IsNullOrEmpty (serviceName))
					serviceNameBytes = System.Text.Encoding.UTF8.GetBytes (serviceName);
				
				if (!String.IsNullOrEmpty (accountName))
					accountNameBytes = System.Text.Encoding.UTF8.GetBytes (accountName);
				
				int passwordLength = 0;
				
				var code = SecKeychainFindGenericPassword(
					IntPtr.Zero,
					serviceNameBytes?.Length ?? 0,
					serviceNameBytes,
					accountNameBytes?.Length ?? 0,
					accountNameBytes,
					out passwordLength,
					out passwordPtr,
					IntPtr.Zero
					);
				
				if (code == SecStatusCode.Success && passwordLength > 0){
					password = new byte[passwordLength];
					Marshal.Copy(passwordPtr, password, 0, passwordLength);
				}
				
				return code;
				
			} finally {
				if (passwordPtr != IntPtr.Zero)
					SecKeychainItemFreeContent(IntPtr.Zero, passwordPtr);
			}
		}
#else
		public static object? QueryAsConcreteType (SecRecord query, out SecStatusCode result)
		{
			if (query is null) {
				result = SecStatusCode.Param;
				return null;
			}

			using (var copy = NSMutableDictionary.FromDictionary (query.queryDict)) {
				copy.LowlevelSetObject (CFBoolean.TrueHandle, SecItem.ReturnRef);
				SetLimit (copy, 1);

				result = SecItem.SecItemCopyMatching (copy.Handle, out var ptr);
				if ((result == SecStatusCode.Success) && (ptr != IntPtr.Zero)) {
					nint cfType = CFType.GetTypeID (ptr);

					if (cfType == SecCertificate.GetTypeID ())
						return new SecCertificate (ptr, true);
					else if (cfType == SecKey.GetTypeID ())
						return new SecKey (ptr, true);
					else if (cfType == SecIdentity.GetTypeID ())
						return new SecIdentity (ptr, true);
					else
						throw new Exception (String.Format ("Unexpected type: 0x{0:x}", cfType));
				}
				return null;
			}
		}
#endif

		public static void AddIdentity (SecIdentity identity)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			using (var record = new SecRecord ()) {
				record.SetValueRef (identity);

				SecStatusCode result = SecKeyChain.Add (record);

				if (result != SecStatusCode.DuplicateItem && result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
			}
		}

		public static void RemoveIdentity (SecIdentity identity)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			using (var record = new SecRecord ()) {
				record.SetValueRef (identity);

				SecStatusCode result = SecKeyChain.Remove (record);

				if (result != SecStatusCode.ItemNotFound && result != SecStatusCode.Success)
					throw new InvalidOperationException (result.ToString ());
			}
		}

		public static SecIdentity? FindIdentity (SecCertificate certificate, bool throwOnError = false)
		{
			if (certificate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (certificate));
			var identity = FindIdentity (cert => SecCertificate.Equals (certificate, cert));
			if (!throwOnError || identity is not null)
				return identity;

			throw new InvalidOperationException (string.Format ("Could not find SecIdentity for certificate '{0}' in keychain.", certificate.SubjectSummary));
		}

		static SecIdentity? FindIdentity (Predicate<SecCertificate> filter)
		{
			/*
			 * Unfortunately, SecItemCopyMatching() does not allow any search
			 * filters when looking up an identity.
			 * 
			 * The following lookup will return all identities from the keychain -
			 * we then need need to find the right one.
			 */
			using (var record = new SecRecord (SecKind.Identity)) {
				SecStatusCode status;
				var result = SecKeyChain.QueryAsReference (record, -1, out status);
				if (status != SecStatusCode.Success || result is null)
					return null;

				for (int i = 0; i < result.Length; i++) {
					var identity = (SecIdentity?) result [i];
					if (filter (identity?.Certificate!))
						return identity;
				}
			}

			return null;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class SecRecord : IDisposable {
		// Fix <= iOS 6 Behaviour - Desk #83099
		// NSCFDictionary: mutating method sent to immutable object
		// iOS 6 returns an inmutable NSDictionary handle and when we try to set its values it goes kaboom
		// By explicitly calling `MutableCopy` we ensure we always have a mutable reference we expect that.
		NSMutableDictionary? _queryDict;
		internal NSMutableDictionary queryDict {
			get {
				return _queryDict!;
			}
			set {
				_queryDict = (NSMutableDictionary) value.MutableCopy ();
			}
		}

		internal SecRecord (NSMutableDictionary dict)
		{
			queryDict = dict;
		}

		// it's possible to query something without a class
		public SecRecord ()
		{
			queryDict = new NSMutableDictionary ();
		}

		public SecRecord (SecKind secKind)
		{
			var kind = SecClass.FromSecKind (secKind);
#if MONOMAC
			queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#elif WATCH
			queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#else
			// Apple changed/fixed this in iOS7 (not the only change, see comments above)
			// test suite has a test case that needs to work on both pre-7.0 and post-7.0
			if ((kind == SecClass.Identity) && !SystemVersion.CheckiOS (7, 0))
				queryDict = new NSMutableDictionary ();
			else
				queryDict = NSMutableDictionary.LowlevelFromObjectAndKey (kind, SecClass.SecClassKey);
#endif
		}

		public SecRecord (SecCertificate certificate) : this (SecKind.Certificate)
		{
			SetCertificate (certificate);
		}

		public SecRecord (SecIdentity identity) : this (SecKind.Identity)
		{
			SetIdentity (identity);
		}

		public SecRecord (SecKey key) : this (SecKind.Key)
		{
			SetKey (key);
		}

		public SecCertificate? GetCertificate ()
		{
			CheckClass (SecClass.Certificate);
			return GetValueRef<SecCertificate> ();
		}

		public SecIdentity? GetIdentity ()
		{
			CheckClass (SecClass.Identity);
			return GetValueRef<SecIdentity> ();
		}

		public SecKey? GetKey ()
		{
			CheckClass (SecClass.Key);
			return GetValueRef<SecKey> ();
		}

		void CheckClass (IntPtr secClass)
		{
			var kind = queryDict.LowlevelObjectForKey (SecClass.SecClassKey);
			if (kind != secClass)
				throw new InvalidOperationException ("SecRecord of incompatible SecClass");
		}

		public SecRecord Clone ()
		{
			return new SecRecord (NSMutableDictionary.FromDictionary (queryDict));
		}

		// some API are unusable without this (e.g. SecKey.GenerateKeyPair) without duplicating much of SecRecord logic
		public NSDictionary ToDictionary ()
		{
			return queryDict;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
				queryDict?.Dispose ();
		}

		~SecRecord ()
		{
			Dispose (false);
		}

		IntPtr Fetch (IntPtr key)
		{
			return queryDict.LowlevelObjectForKey (key);
		}

		NSObject? FetchObject (IntPtr key)
		{
			return Runtime.GetNSObject (Fetch (key));
		}

		string? FetchString (IntPtr key)
		{
			return (NSString?) FetchObject (key);
		}

		int FetchInt (IntPtr key)
		{
			var obj = (NSNumber?) FetchObject (key);
			return obj is null ? -1 : obj.Int32Value;
		}

		bool FetchBool (IntPtr key, bool defaultValue)
		{
			var obj = (NSNumber?) FetchObject (key);
			return obj is null ? defaultValue : obj.Int32Value != 0;
		}

		T? Fetch<T> (IntPtr key) where T : NSObject
		{
			return (T?) FetchObject (key);
		}


		void SetValue (NSObject val, IntPtr key)
		{
			queryDict.LowlevelSetObject (val, key);
		}

		void SetValue (IntPtr val, IntPtr key)
		{
			queryDict.LowlevelSetObject (val, key);
		}

		void SetValue (string value, IntPtr key)
		{
			// FIXME: it's not clear that we should not allow null (i.e. that null should remove entries)
			// but this is compatible with the exiting behaviour of older XI/XM
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			var ptr = CFString.CreateNative (value);
			try {
				queryDict.LowlevelSetObject (ptr, key);
			} finally {
				CFString.ReleaseNative (ptr);
			}
		}

		//
		// Attributes
		//
		public SecAccessible Accessible {
			get {
				return KeysAccessible.ToSecAccessible (Fetch (SecAttributeKey.Accessible));
			}

			set {
				SetValue (KeysAccessible.FromSecAccessible (value), SecAttributeKey.Accessible);
			}
		}

		public bool Synchronizable {
			get {
				return FetchBool (SecAttributeKey.Synchronizable, false);
			}
			set {
				SetValue (new NSNumber (value ? 1 : 0), SecAttributeKey.Synchronizable);
			}
		}

		public bool SynchronizableAny {
			get {
				return FetchBool (SecAttributeKey.SynchronizableAny, false);
			}
			set {
				SetValue (new NSNumber (value ? 1 : 0), SecAttributeKey.SynchronizableAny);
			}
		}

#if !MONOMAC
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#endif
		public string? SyncViewHint {
			get {
				return FetchString (SecAttributeKey.SyncViewHint);
			}
			set {
				SetValue (value!, SecAttributeKey.SyncViewHint);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecTokenID TokenID {
			get {
				return SecTokenIDExtensions.GetValue (Fetch<NSString> (SecKeyGenerationAttributeKeys.TokenIDKey.GetHandle ())!);
			}
			set {
				// choose wisely to avoid NSString -> string -> NSString conversion
				SetValue ((NSObject) value.GetConstant ()!, SecKeyGenerationAttributeKeys.TokenIDKey.GetHandle ());
			}
		}
#endif

		public NSDate? CreationDate {
			get {
				return (NSDate?) FetchObject (SecAttributeKey.CreationDate);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecAttributeKey.CreationDate);
			}
		}

		public NSDate? ModificationDate {
			get {
				return (NSDate?) FetchObject (SecAttributeKey.ModificationDate);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecAttributeKey.ModificationDate);
			}
		}

		public string? Description {
			get {
				return FetchString (SecAttributeKey.Description);
			}

			set {
				SetValue (value!, SecAttributeKey.Description);
			}
		}

		public string? Comment {
			get {
				return FetchString (SecAttributeKey.Comment);
			}

			set {
				SetValue (value!, SecAttributeKey.Comment);
			}
		}

		public int Creator {
			get {
				return FetchInt (SecAttributeKey.Creator);
			}

			set {
				SetValue (new NSNumber (value), SecAttributeKey.Creator);
			}
		}

		public int CreatorType {
			get {
				return FetchInt (SecAttributeKey.Type);
			}

			set {
				SetValue (new NSNumber (value), SecAttributeKey.Type);
			}
		}

		public string? Label {
			get {
				return FetchString (SecAttributeKeys.LabelKey.Handle);
			}

			set {
				SetValue (value!, SecAttributeKeys.LabelKey.Handle);
			}
		}

		public bool Invisible {
			get {
				return Fetch (SecAttributeKey.IsInvisible) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKey.IsInvisible);
			}
		}

		public bool IsNegative {
			get {
				return Fetch (SecAttributeKey.IsNegative) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKey.IsNegative);
			}
		}

		public string? Account {
			get {
				return FetchString (SecAttributeKey.Account);
			}

			set {
				SetValue (value!, SecAttributeKey.Account);
			}
		}

		public string? Service {
			get {
				return FetchString (SecAttributeKey.Service);
			}

			set {
				SetValue (value!, SecAttributeKey.Service);
			}
		}

#if !MONOMAC
		public string? UseOperationPrompt {
			get {
				return FetchString (SecItem.UseOperationPrompt);
			}
			set {
				SetValue (value!, SecItem.UseOperationPrompt);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios9.0", "Use 'AuthenticationUI' property instead.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AuthenticationUI' property instead.")]
#endif
		public bool UseNoAuthenticationUI {
			get {
				return Fetch (SecItem.UseNoAuthenticationUI) == CFBoolean.TrueHandle;
			}
			set {
				SetValue (CFBoolean.ToHandle (value), SecItem.UseNoAuthenticationUI);
			}
		}
#endif
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecAuthenticationUI AuthenticationUI {
			get {
				var s = Fetch<NSString> (SecItem.UseAuthenticationUI);
				return s is null ? SecAuthenticationUI.NotSet : SecAuthenticationUIExtensions.GetValue (s);
			}
			set {
				SetValue ((NSObject) value.GetConstant ()!, SecItem.UseAuthenticationUI);
			}
		}

#if !WATCH && !TVOS
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public LocalAuthentication.LAContext? AuthenticationContext {
			get {
				return Fetch<LocalAuthentication.LAContext> (SecItem.UseAuthenticationContext);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value.Handle, SecItem.UseAuthenticationContext);
			}
		}
#endif

		// Must store the _secAccessControl here, since we have no way of inspecting its values if
		// it is ever returned from a dictionary, so return what we cached.
		SecAccessControl? _secAccessControl;
		public SecAccessControl? AccessControl {
			get {
				return _secAccessControl;
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				_secAccessControl = value;
				SetValue (value.Handle, SecAttributeKeys.AccessControlKey.Handle);
			}
		}

		public NSData? Generic {
			get {
				return Fetch<NSData> (SecAttributeKey.Generic);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecAttributeKey.Generic);
			}
		}

		public string? SecurityDomain {
			get {
				return FetchString (SecAttributeKey.SecurityDomain);
			}

			set {
				SetValue (value!, SecAttributeKey.SecurityDomain);
			}
		}

		public string? Server {
			get {
				return FetchString (SecAttributeKey.Server);
			}

			set {
				SetValue (value!, SecAttributeKey.Server);
			}
		}

		public SecProtocol Protocol {
			get {
				return SecProtocolKeys.ToSecProtocol (Fetch (SecAttributeKey.Protocol));
			}

			set {
				SetValue (SecProtocolKeys.FromSecProtocol (value), SecAttributeKey.Protocol);
			}
		}

		public SecAuthenticationType AuthenticationType {
			get {
				var at = Fetch (SecAttributeKey.AuthenticationType);
				if (at == IntPtr.Zero)
					return SecAuthenticationType.Default;
				return KeysAuthenticationType.ToSecAuthenticationType (at);
			}

			set {
				SetValue (KeysAuthenticationType.FromSecAuthenticationType (value),
								 SecAttributeKey.AuthenticationType);
			}
		}

		public int Port {
			get {
				return FetchInt (SecAttributeKey.Port);
			}

			set {
				SetValue (new NSNumber (value), SecAttributeKey.Port);
			}
		}

		public string? Path {
			get {
				return FetchString (SecAttributeKey.Path);
			}

			set {
				SetValue (value!, SecAttributeKey.Path);
			}
		}

		// read only
		public string? Subject {
			get {
				return FetchString (SecAttributeKey.Subject);
			}
		}

		// read only
		public NSData? Issuer {
			get {
				return Fetch<NSData> (SecAttributeKey.Issuer);
			}
		}

		// read only
		public NSData? SerialNumber {
			get {
				return Fetch<NSData> (SecAttributeKey.SerialNumber);
			}
		}

		// read only
		public NSData? SubjectKeyID {
			get {
				return Fetch<NSData> (SecAttributeKey.SubjectKeyID);
			}
		}

		// read only
		public NSData? PublicKeyHash {
			get {
				return Fetch<NSData> (SecAttributeKey.PublicKeyHash);
			}
		}

		// read only
		public NSNumber? CertificateType {
			get {
				return Fetch<NSNumber> (SecAttributeKey.CertificateType);
			}
		}

		// read only
		public NSNumber? CertificateEncoding {
			get {
				return Fetch<NSNumber> (SecAttributeKey.CertificateEncoding);
			}
		}

		public SecKeyClass KeyClass {
			get {
				var k = Fetch (SecAttributeKey.KeyClass);
				if (k == IntPtr.Zero)
					return SecKeyClass.Invalid;
				using (var s = new NSString (k))
					return SecKeyClassExtensions.GetValue (s);
			}
			set {
				var k = value.GetConstant ();
				if (k is null)
					throw new ArgumentException (nameof (value));
				SetValue ((NSObject) k, SecAttributeKey.KeyClass);
			}
		}

		public string? ApplicationLabel {
			get {
				return FetchString (SecAttributeKey.ApplicationLabel);
			}

			set {
				SetValue (value!, SecAttributeKey.ApplicationLabel);
			}
		}

		public bool IsPermanent {
			get {
				return Fetch (SecAttributeKeys.IsPermanentKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.IsPermanentKey.Handle);
			}
		}

		public bool IsSensitive {
			get {
				return Fetch (SecAttributeKey.IsSensitive) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKey.IsSensitive);
			}
		}

		public bool IsExtractable {
			get {
				return Fetch (SecAttributeKey.IsExtractable) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKey.IsExtractable);
			}
		}

		public NSData? ApplicationTag {
			get {
				return Fetch<NSData> (SecAttributeKeys.ApplicationTagKey.Handle);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecAttributeKeys.ApplicationTagKey.Handle);
			}
		}

		public SecKeyType KeyType {
			get {
				var k = Fetch (SecKeyGenerationAttributeKeys.KeyTypeKey.Handle);
				if (k == IntPtr.Zero)
					return SecKeyType.Invalid;
				using (var s = new NSString (k))
					return SecKeyTypeExtensions.GetValue (s);
			}

			set {
				var k = value.GetConstant ();
				if (k is null)
					throw new ArgumentException (nameof (value));
				SetValue ((NSObject) k, SecKeyGenerationAttributeKeys.KeyTypeKey.Handle);
			}
		}

		public int KeySizeInBits {
			get {
				return FetchInt (SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
			}

			set {
				SetValue (new NSNumber (value), SecKeyGenerationAttributeKeys.KeySizeInBitsKey.Handle);
			}
		}

		public int EffectiveKeySize {
			get {
				return FetchInt (SecAttributeKeys.EffectiveKeySizeKey.Handle);
			}

			set {
				SetValue (new NSNumber (value), SecAttributeKeys.EffectiveKeySizeKey.Handle);
			}
		}

		public bool CanEncrypt {
			get {
				return Fetch (SecAttributeKeys.CanEncryptKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanEncryptKey.Handle);
			}
		}

		public bool CanDecrypt {
			get {
				return Fetch (SecAttributeKeys.CanDecryptKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanDecryptKey.Handle);
			}
		}

		public bool CanDerive {
			get {
				return Fetch (SecAttributeKeys.CanDeriveKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanDeriveKey.Handle);
			}
		}

		public bool CanSign {
			get {
				return Fetch (SecAttributeKeys.CanSignKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanSignKey.Handle);
			}
		}

		public bool CanVerify {
			get {
				return Fetch (SecAttributeKeys.CanVerifyKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanVerifyKey.Handle);
			}
		}

		public bool CanWrap {
			get {
				return Fetch (SecKeyGenerationAttributeKeys.CanWrapKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecKeyGenerationAttributeKeys.CanWrapKey.Handle);
			}
		}

		public bool CanUnwrap {
			get {
				return Fetch (SecAttributeKeys.CanUnwrapKey.Handle) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKeys.CanUnwrapKey.Handle);
			}
		}

		public string? AccessGroup {
			get {
				return FetchString (SecAttributeKey.AccessGroup);
			}

			set {
				SetValue (value!, SecAttributeKey.AccessGroup);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public bool PersistentReference {
			get {
				return Fetch (SecAttributeKey.PersistentReference) == CFBoolean.TrueHandle;
			}
			set {
				SetValue (CFBoolean.ToHandle (value), SecAttributeKey.PersistentReference);
			}
		}

#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		public bool UseDataProtectionKeychain {
			get {
				return Fetch (SecItem.UseDataProtectionKeychain) == CFBoolean.TrueHandle;
			}
			set {
				SetValue (CFBoolean.ToHandle (value), SecItem.UseDataProtectionKeychain);
			}
		}

		//
		// Matches
		//

		public SecPolicy? MatchPolicy {
			get {
				var pol = Fetch (SecItem.MatchPolicy);
				return (pol == IntPtr.Zero) ? null : new SecPolicy (pol, false);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value.Handle, SecItem.MatchPolicy);
			}
		}

		public SecKeyChain? []? MatchItemList {
			get {
				return NSArray.ArrayFromHandle<SecKeyChain> (Fetch (SecItem.MatchItemList));
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				using (var array = NSArray.FromNativeObjects (value))
					SetValue (array, SecItem.MatchItemList);
			}
		}

		public NSData? []? MatchIssuers {
			get {
				return NSArray.ArrayFromHandle<NSData> (Fetch (SecItem.MatchIssuers));
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				SetValue (NSArray.FromNSObjects (value), SecItem.MatchIssuers);
			}
		}

		public string? MatchEmailAddressIfPresent {
			get {
				return FetchString (SecItem.MatchEmailAddressIfPresent);
			}

			set {
				SetValue (value!, SecItem.MatchEmailAddressIfPresent);
			}
		}

		public string? MatchSubjectContains {
			get {
				return FetchString (SecItem.MatchSubjectContains);
			}

			set {
				SetValue (value!, SecItem.MatchSubjectContains);
			}
		}

		public bool MatchCaseInsensitive {
			get {
				return Fetch (SecItem.MatchCaseInsensitive) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecItem.MatchCaseInsensitive);
			}
		}

		public bool MatchTrustedOnly {
			get {
				return Fetch (SecItem.MatchTrustedOnly) == CFBoolean.TrueHandle;
			}

			set {
				SetValue (CFBoolean.ToHandle (value), SecItem.MatchTrustedOnly);
			}
		}

		public NSDate? MatchValidOnDate {
			get {
				return Runtime.GetNSObject<NSDate> (Fetch (SecItem.MatchValidOnDate));
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecItem.MatchValidOnDate);
			}
		}

		public NSData? ValueData {
			get {
				return Fetch<NSData> (SecItem.ValueData);
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				SetValue (value, SecItem.ValueData);
			}
		}

		public T? GetValueRef<T> () where T : class, INativeObject
		{
			return Runtime.GetINativeObject<T> (queryDict.LowlevelObjectForKey (SecItem.ValueRef), false);
		}

		// This can be used to store SecKey, SecCertificate, SecIdentity and SecKeyChainItem (not bound yet, and not availble on iOS)
		public void SetValueRef (INativeObject value)
		{
			SetValue (value.GetHandle (), SecItem.ValueRef);
		}

		public void SetCertificate (SecCertificate cert) => SetValueRef (cert);
		public void SetIdentity (SecIdentity identity) => SetValueRef (identity);
		public void SetKey (SecKey key) => SetValueRef (key);

	}

	internal partial class SecItem {

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemCopyMatching (/* CFDictionaryRef */ IntPtr query, /* CFTypeRef* */ out IntPtr result);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemAdd (/* CFDictionaryRef */ IntPtr attributes, /* CFTypeRef* */ IntPtr result);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemDelete (/* CFDictionaryRef */ IntPtr query);

		[DllImport (Constants.SecurityLibrary)]
		internal extern static SecStatusCode SecItemUpdate (/* CFDictionaryRef */ IntPtr query, /* CFDictionaryRef */ IntPtr attributesToUpdate);
	}

	internal static partial class SecClass {

		public static IntPtr FromSecKind (SecKind secKind)
		{
			switch (secKind) {
			case SecKind.InternetPassword:
				return InternetPassword;
			case SecKind.GenericPassword:
				return GenericPassword;
			case SecKind.Certificate:
				return Certificate;
			case SecKind.Key:
				return Key;
			case SecKind.Identity:
				return Identity;
			default:
				throw new ArgumentException (nameof (secKind));
			}
		}
	}

	internal static partial class KeysAccessible {
		public static IntPtr FromSecAccessible (SecAccessible accessible)
		{
			switch (accessible) {
			case SecAccessible.WhenUnlocked:
				return WhenUnlocked;
			case SecAccessible.AfterFirstUnlock:
				return AfterFirstUnlock;
			case SecAccessible.Always:
				return Always;
			case SecAccessible.WhenUnlockedThisDeviceOnly:
				return WhenUnlockedThisDeviceOnly;
			case SecAccessible.AfterFirstUnlockThisDeviceOnly:
				return AfterFirstUnlockThisDeviceOnly;
			case SecAccessible.AlwaysThisDeviceOnly:
				return AlwaysThisDeviceOnly;
			case SecAccessible.WhenPasscodeSetThisDeviceOnly:
				return WhenPasscodeSetThisDeviceOnly;
			default:
				throw new ArgumentException (nameof (accessible));
			}
		}

		// note: we're comparing pointers - but it's an (even if opaque) CFType
		// and it turns out to be using CFString - so we need to use CFTypeEqual
		public static SecAccessible ToSecAccessible (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecAccessible.Invalid;
			if (CFType.Equal (handle, WhenUnlocked))
				return SecAccessible.WhenUnlocked;
			if (CFType.Equal (handle, AfterFirstUnlock))
				return SecAccessible.AfterFirstUnlock;
			if (CFType.Equal (handle, Always))
				return SecAccessible.Always;
			if (CFType.Equal (handle, WhenUnlockedThisDeviceOnly))
				return SecAccessible.WhenUnlockedThisDeviceOnly;
			if (CFType.Equal (handle, AfterFirstUnlockThisDeviceOnly))
				return SecAccessible.AfterFirstUnlockThisDeviceOnly;
			if (CFType.Equal (handle, AlwaysThisDeviceOnly))
				return SecAccessible.AlwaysThisDeviceOnly;
			if (CFType.Equal (handle, WhenUnlockedThisDeviceOnly))
				return SecAccessible.WhenUnlockedThisDeviceOnly;
			return SecAccessible.Invalid;
		}
	}

	internal static partial class SecProtocolKeys {
		public static IntPtr FromSecProtocol (SecProtocol protocol)
		{
			switch (protocol) {
			case SecProtocol.Ftp: return FTP;
			case SecProtocol.FtpAccount: return FTPAccount;
			case SecProtocol.Http: return HTTP;
			case SecProtocol.Irc: return IRC;
			case SecProtocol.Nntp: return NNTP;
			case SecProtocol.Pop3: return POP3;
			case SecProtocol.Smtp: return SMTP;
			case SecProtocol.Socks: return SOCKS;
			case SecProtocol.Imap: return IMAP;
			case SecProtocol.Ldap: return LDAP;
			case SecProtocol.AppleTalk: return AppleTalk;
			case SecProtocol.Afp: return AFP;
			case SecProtocol.Telnet: return Telnet;
			case SecProtocol.Ssh: return SSH;
			case SecProtocol.Ftps: return FTPS;
			case SecProtocol.Https: return HTTPS;
			case SecProtocol.HttpProxy: return HTTPProxy;
			case SecProtocol.HttpsProxy: return HTTPSProxy;
			case SecProtocol.FtpProxy: return FTPProxy;
			case SecProtocol.Smb: return SMB;
			case SecProtocol.Rtsp: return RTSP;
			case SecProtocol.RtspProxy: return RTSPProxy;
			case SecProtocol.Daap: return DAAP;
			case SecProtocol.Eppc: return EPPC;
			case SecProtocol.Ipp: return IPP;
			case SecProtocol.Nntps: return NNTPS;
			case SecProtocol.Ldaps: return LDAPS;
			case SecProtocol.Telnets: return TelnetS;
			case SecProtocol.Imaps: return IMAPS;
			case SecProtocol.Ircs: return IRCS;
			case SecProtocol.Pop3s: return POP3S;
			}
			throw new ArgumentException (nameof (protocol));
		}

		public static SecProtocol ToSecProtocol (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecProtocol.Invalid;
			if (CFType.Equal (handle, FTP))
				return SecProtocol.Ftp;
			if (CFType.Equal (handle, FTPAccount))
				return SecProtocol.FtpAccount;
			if (CFType.Equal (handle, HTTP))
				return SecProtocol.Http;
			if (CFType.Equal (handle, IRC))
				return SecProtocol.Irc;
			if (CFType.Equal (handle, NNTP))
				return SecProtocol.Nntp;
			if (CFType.Equal (handle, POP3))
				return SecProtocol.Pop3;
			if (CFType.Equal (handle, SMTP))
				return SecProtocol.Smtp;
			if (CFType.Equal (handle, SOCKS))
				return SecProtocol.Socks;
			if (CFType.Equal (handle, IMAP))
				return SecProtocol.Imap;
			if (CFType.Equal (handle, LDAP))
				return SecProtocol.Ldap;
			if (CFType.Equal (handle, AppleTalk))
				return SecProtocol.AppleTalk;
			if (CFType.Equal (handle, AFP))
				return SecProtocol.Afp;
			if (CFType.Equal (handle, Telnet))
				return SecProtocol.Telnet;
			if (CFType.Equal (handle, SSH))
				return SecProtocol.Ssh;
			if (CFType.Equal (handle, FTPS))
				return SecProtocol.Ftps;
			if (CFType.Equal (handle, HTTPS))
				return SecProtocol.Https;
			if (CFType.Equal (handle, HTTPProxy))
				return SecProtocol.HttpProxy;
			if (CFType.Equal (handle, HTTPSProxy))
				return SecProtocol.HttpsProxy;
			if (CFType.Equal (handle, FTPProxy))
				return SecProtocol.FtpProxy;
			if (CFType.Equal (handle, SMB))
				return SecProtocol.Smb;
			if (CFType.Equal (handle, RTSP))
				return SecProtocol.Rtsp;
			if (CFType.Equal (handle, RTSPProxy))
				return SecProtocol.RtspProxy;
			if (CFType.Equal (handle, DAAP))
				return SecProtocol.Daap;
			if (CFType.Equal (handle, EPPC))
				return SecProtocol.Eppc;
			if (CFType.Equal (handle, IPP))
				return SecProtocol.Ipp;
			if (CFType.Equal (handle, NNTPS))
				return SecProtocol.Nntps;
			if (CFType.Equal (handle, LDAPS))
				return SecProtocol.Ldaps;
			if (CFType.Equal (handle, TelnetS))
				return SecProtocol.Telnets;
			if (CFType.Equal (handle, IMAPS))
				return SecProtocol.Imaps;
			if (CFType.Equal (handle, IRCS))
				return SecProtocol.Ircs;
			if (CFType.Equal (handle, POP3S))
				return SecProtocol.Pop3s;
			return SecProtocol.Invalid;
		}
	}

	internal static partial class KeysAuthenticationType {
		public static SecAuthenticationType ToSecAuthenticationType (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return SecAuthenticationType.Invalid;
			if (CFType.Equal (handle, NTLM))
				return SecAuthenticationType.Ntlm;
			if (CFType.Equal (handle, MSN))
				return SecAuthenticationType.Msn;
			if (CFType.Equal (handle, DPA))
				return SecAuthenticationType.Dpa;
			if (CFType.Equal (handle, RPA))
				return SecAuthenticationType.Rpa;
			if (CFType.Equal (handle, HTTPBasic))
				return SecAuthenticationType.HttpBasic;
			if (CFType.Equal (handle, HTTPDigest))
				return SecAuthenticationType.HttpDigest;
			if (CFType.Equal (handle, HTMLForm))
				return SecAuthenticationType.HtmlForm;
			if (CFType.Equal (handle, Default))
				return SecAuthenticationType.Default;
			return SecAuthenticationType.Invalid;
		}

		public static IntPtr FromSecAuthenticationType (SecAuthenticationType type)
		{
			switch (type) {
			case SecAuthenticationType.Ntlm:
				return NTLM;
			case SecAuthenticationType.Msn:
				return MSN;
			case SecAuthenticationType.Dpa:
				return DPA;
			case SecAuthenticationType.Rpa:
				return RPA;
			case SecAuthenticationType.HttpBasic:
				return HTTPBasic;
			case SecAuthenticationType.HttpDigest:
				return HTTPDigest;
			case SecAuthenticationType.HtmlForm:
				return HTMLForm;
			case SecAuthenticationType.Default:
				return Default;
			default:
				throw new ArgumentException (nameof (type));
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class SecurityException : Exception {
		static string ToMessage (SecStatusCode code)
		{

			switch (code) {
			case SecStatusCode.Success:
			case SecStatusCode.Unimplemented:
			case SecStatusCode.Param:
			case SecStatusCode.Allocate:
			case SecStatusCode.NotAvailable:
			case SecStatusCode.DuplicateItem:
			case SecStatusCode.ItemNotFound:
			case SecStatusCode.InteractionNotAllowed:
			case SecStatusCode.Decode:
				return code.ToString ();
			}
			return String.Format ("Unknown error: 0x{0:x}", code);
		}

		public SecurityException (SecStatusCode code) : base (ToMessage (code))
		{
		}
	}

	public partial class SecKeyParameters : DictionaryContainer {
		// For caching, as we can't reverse it easily.
		SecAccessControl? _secAccessControl;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecAccessControl AccessControl {
			get {
				return _secAccessControl!;
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				_secAccessControl = value;
				SetNativeValue (SecAttributeKeys.AccessControlKey, value);
			}
		}
	}

	public partial class SecKeyGenerationParameters : DictionaryContainer {
		public SecKeyType KeyType {
			get {
				var type = GetNSStringValue (SecKeyGenerationAttributeKeys.KeyTypeKey);
				if (type is null)
					return SecKeyType.Invalid;
				return SecKeyTypeExtensions.GetValue (type);
			}

			set {
				var k = value.GetConstant ();
				if (k is null)
					throw new ArgumentException (nameof (value));
				SetStringValue (SecKeyGenerationAttributeKeys.KeyTypeKey, k);
			}
		}

		// For caching, as we can't reverse it easily.
		SecAccessControl? _secAccessControl;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecAccessControl AccessControl {
			get {
				return _secAccessControl!;
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
				_secAccessControl = value;
				SetNativeValue (SecAttributeKeys.AccessControlKey, value);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public SecTokenID TokenID {
			get {
				return SecTokenIDExtensions.GetValue (GetNSStringValue (SecKeyGenerationAttributeKeys.TokenIDKey)!);
			}

			set {
				SetStringValue (SecKeyGenerationAttributeKeys.TokenIDKey, value.GetConstant ());
			}
		}
	}
}
