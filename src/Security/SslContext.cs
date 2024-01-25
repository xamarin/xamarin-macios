//
// SslContext
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014-2016 Xamarin Inc.
//

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
	[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
	[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
	[ObsoletedOSPlatform ("maccatalyst13.0", "Use 'Network.framework' instead.")]
#else
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Network.framework' instead.")]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Network.framework' instead.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Network.framework' instead.")]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Network.framework' instead.")]
#endif
	public class SslContext : NativeObject {

		SslConnection? connection;
		SslStatus result;

		[DllImport (Constants.SecurityLibrary)]
		extern static /* SSLContextRef */ IntPtr SSLCreateContext (/* CFAllocatorRef */ IntPtr alloc, SslProtocolSide protocolSide, SslConnectionType connectionType);

		public SslContext (SslProtocolSide protocolSide, SslConnectionType connectionType)
			: base (SSLCreateContext (IntPtr.Zero, protocolSide, connectionType), true)
		{
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLClose (/* SSLContextRef */ IntPtr context);

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero)
				result = SSLClose (Handle);

			// don't remove the read/write delegates before we closed the connection, i.e.
			// the SSLClose will send an Alert for a "close notify"
			if (connection is not null) {
				connection.Dispose ();
				connection = null;
			}

			base.Dispose (disposing);
		}

		public SslStatus GetLastStatus ()
		{
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetProtocolVersionMax (/* SSLContextRef */ IntPtr context, out SslProtocol maxVersion);

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetProtocolVersionMax (/* SSLContextRef */ IntPtr context, SslProtocol maxVersion);

		public SslProtocol MaxProtocol {
			get {
				SslProtocol value;
				result = SSLGetProtocolVersionMax (Handle, out value);
				return value;
			}
			set {
				result = SSLSetProtocolVersionMax (Handle, value);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetProtocolVersionMin (/* SSLContextRef */ IntPtr context, out SslProtocol minVersion);

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetProtocolVersionMin (/* SSLContextRef */ IntPtr context, SslProtocol minVersion);

		public SslProtocol MinProtocol {
			get {
				SslProtocol value;
				result = SSLGetProtocolVersionMin (Handle, out value);
				return value;
			}
			set {
				result = SSLSetProtocolVersionMin (Handle, value);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetNegotiatedProtocolVersion (/* SSLContextRef */ IntPtr context, out SslProtocol protocol);

		public SslProtocol NegotiatedProtocol {
			get {
				SslProtocol value;
				result = SSLGetNegotiatedProtocolVersion (Handle, out value);
				return value;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetConnection (/* SSLContextRef */ IntPtr context, /* SSLConnectionRef* */ out IntPtr connection);

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetConnection (/* SSLContextRef */ IntPtr context, /* SSLConnectionRef */ IntPtr connection);

		[DllImport (Constants.SecurityLibrary)]
#if NET
		unsafe extern static /* OSStatus */ SslStatus SSLSetIOFuncs (
			/* SSLContextRef */ IntPtr context,
			/* SSLReadFunc */ delegate* unmanaged<IntPtr, IntPtr, nint*, SslStatus> readFunc,
			/* SSLWriteFunc */ delegate* unmanaged<IntPtr, IntPtr, nint*, SslStatus> writeFunc);
#else
		extern static /* OSStatus */ SslStatus SSLSetIOFuncs (/* SSLContextRef */ IntPtr context, /* SSLReadFunc */ SslReadFunc? readFunc, /* SSLWriteFunc */ SslWriteFunc? writeFunc);
#endif

		public SslConnection? Connection {
			get {
				if (connection is null)
					return null;
				IntPtr value;
				result = SSLGetConnection (Handle, out value);
				if (value != connection.ConnectionId)
					throw new InvalidOperationException ();
				return connection;
			}
			set {
				// the read/write delegates needs to be set before setting the connection id
				unsafe {
					if (value is null)
						result = SSLSetIOFuncs (Handle, null, null);
					else
						result = SSLSetIOFuncs (Handle, value.ReadFunc, value.WriteFunc);
				}

				if (result == SslStatus.Success) {
					result = SSLSetConnection (Handle, value is null ? IntPtr.Zero : value.ConnectionId);
					connection = value;
				}
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetSessionOption (/* SSLContextRef */ IntPtr context, SslSessionOption option, [MarshalAs (UnmanagedType.I1)] out bool value);

		public SslStatus GetSessionOption (SslSessionOption option, out bool value)
		{
			result = SSLGetSessionOption (Handle, option, out value);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetSessionOption (/* SSLContextRef */ IntPtr context, SslSessionOption option, [MarshalAs (UnmanagedType.I1)] bool value);

		public SslStatus SetSessionOption (SslSessionOption option, bool value)
		{
			result = SSLSetSessionOption (Handle, option, value);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetClientSideAuthenticate (/* SSLContextRef */ IntPtr context, SslAuthenticate auth);

		public SslStatus SetClientSideAuthenticate (SslAuthenticate auth)
		{
			result = SSLSetClientSideAuthenticate (Handle, auth);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLHandshake (/* SSLContextRef */ IntPtr context);

		public SslStatus Handshake ()
		{
			result = SSLHandshake (Handle);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetSessionState (/* SSLContextRef */ IntPtr context, ref SslSessionState state);

		public SslSessionState SessionState {
			get {
				var value = SslSessionState.Invalid;
				result = SSLGetSessionState (Handle, ref value);
				return value;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetPeerID (/* SSLContextRef */ IntPtr context, /* const void** */ out IntPtr peerID, /* size_t* */ out nint peerIDLen);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetPeerID (/* SSLContextRef */ IntPtr context, /* const void* */ byte* peerID, /* size_t */ nint peerIDLen);

		public unsafe byte []? PeerId {
			get {
				nint length;
				IntPtr id;
				result = SSLGetPeerID (Handle, out id, out length);
				if ((result != SslStatus.Success) || (length == 0))
					return null;
				var data = new byte [length];
				Marshal.Copy (id, data, 0, (int) length);
				return data;
			}
			set {
				nint length = (value is null) ? 0 : value.Length;
				fixed (byte* p = value) {
					result = SSLSetPeerID (Handle, p, length);
				}
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetBufferedReadSize (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint bufSize);

		public nint BufferedReadSize {
			get {
				nint value;
				result = SSLGetBufferedReadSize (Handle, out value);
				return value;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLRead (/* SSLContextRef */ IntPtr context, /* const void* */ byte* data, /* size_t */ nint dataLength, /* size_t* */ out nint processed);

		internal unsafe SslStatus Read (byte [] data, int offset, int size, out nint processed)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			fixed (byte* d = &data [offset])
				result = SSLRead (Handle, d, size, out processed);
			return result;
		}

		public unsafe SslStatus Read (byte [] data, out nint processed)
		{
			int size = data is null ? 0 : data.Length;
			fixed (byte* d = data)
				result = SSLRead (Handle, d, size, out processed);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLWrite (/* SSLContextRef */ IntPtr context, /* const void* */ byte* data, /* size_t */ nint dataLength, /* size_t* */ out nint processed);

		internal unsafe SslStatus Write (byte [] data, int offset, int size, out nint processed)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			fixed (byte* d = &data [offset])
				result = SSLWrite (Handle, d, size, out processed);
			return result;
		}

		public unsafe SslStatus Write (byte [] data, out nint processed)
		{
			int size = data is null ? 0 : data.Length;
			fixed (byte* d = data)
				result = SSLWrite (Handle, d, size, out processed);
			return result;
		}


#if !NET
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetNumberSupportedCiphers (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint numCiphers);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetSupportedCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite* ciphers, /* size_t* */ ref nint numCiphers);

		public unsafe IList<SslCipherSuite>? GetSupportedCiphers ()
		{
			nint n;
			result = SSLGetNumberSupportedCiphers (Handle, out n);
			if ((result != SslStatus.Success) || (n <= 0))
				return null;

			var ciphers = new SslCipherSuite [n];
			fixed (SslCipherSuite* p = ciphers) {
				result = SSLGetSupportedCiphers (Handle, p, ref n);
				if (result != SslStatus.Success)
					return null;
			}
			return new List<SslCipherSuite> (ciphers);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetNumberEnabledCiphers (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint numCiphers);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetEnabledCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite* ciphers, /* size_t* */ ref nint numCiphers);

		public unsafe IList<SslCipherSuite>? GetEnabledCiphers ()
		{
			nint n;
			result = SSLGetNumberEnabledCiphers (Handle, out n);
			if ((result != SslStatus.Success) || (n <= 0))
				return null;

			var ciphers = new SslCipherSuite [n];
			fixed (SslCipherSuite* p = ciphers) {
				result = SSLGetEnabledCiphers (Handle, p, ref n);
				if (result != SslStatus.Success)
					return null;
			}
			return new List<SslCipherSuite> (ciphers);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetEnabledCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite* ciphers, /* size_t */ nint numCiphers);

		public unsafe SslStatus SetEnabledCiphers (IEnumerable<SslCipherSuite> ciphers)
		{
			if (ciphers is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (ciphers));

			var array = ciphers.ToArray ();
			fixed (SslCipherSuite* p = array)
				result = SSLSetEnabledCiphers (Handle, p, array.Length);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetNegotiatedCipher (/* SSLContextRef */ IntPtr context, /* SslCipherSuite* */ out SslCipherSuite cipherSuite);

		public SslCipherSuite NegotiatedCipher {
			get {
				SslCipherSuite value;
				result = SSLGetNegotiatedCipher (Handle, out value);
				return value;
			}
		}
#endif

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetDatagramWriteSize (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint bufSize);

		public nint DatagramWriteSize {
			get {
				nint value;
				result = SSLGetDatagramWriteSize (Handle, out value);
				return value;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetMaxDatagramRecordSize (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint maxSize);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetMaxDatagramRecordSize (/* SSLContextRef */ IntPtr context, /* size_t */ nint maxSize);

		public nint MaxDatagramRecordSize {
			get {
				nint value;
				result = SSLGetMaxDatagramRecordSize (Handle, out value);
				return value;
			}
			set {
				result = SSLSetMaxDatagramRecordSize (Handle, value);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetDatagramHelloCookie (/* SSLContextRef */ IntPtr context, /* const void* */ byte* cookie, nint cookieLength);

		public unsafe SslStatus SetDatagramHelloCookie (byte [] cookie)
		{
			nint len = cookie is null ? 0 : cookie.Length;
			fixed (byte* p = cookie)
				result = SSLSetDatagramHelloCookie (Handle, p, len);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetPeerDomainNameLength (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint peerNameLen);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetPeerDomainName (/* SSLContextRef */ IntPtr context, /* char* */ byte []? peerName, /* size_t */ ref nint peerNameLen);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetPeerDomainName (/* SSLContextRef */ IntPtr context, /* char* */ byte []? peerName, /* size_t */ nint peerNameLen);

		public string PeerDomainName {
			get {
				nint length;
				result = SSLGetPeerDomainNameLength (Handle, out length);
				if (result != SslStatus.Success || length == 0)
					return String.Empty;
				var bytes = new byte [length];
				result = SSLGetPeerDomainName (Handle, bytes, ref length);
				return result == SslStatus.Success ? Encoding.UTF8.GetString (bytes) : String.Empty;
			}
			set {
				if (value is null) {
					result = SSLSetPeerDomainName (Handle, null, 0);
				} else {
					var bytes = Encoding.UTF8.GetBytes (value);
					result = SSLSetPeerDomainName (Handle, bytes, bytes.Length);
				}
			}
		}

		// SSLAddDistinguishedName
		// Documented as unsupported and unimplemented (both iOS and OSX)
#if false
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLCopyDistinguishedNames (/* SSLContextRef */ IntPtr context, /* CFArrayRef* */ out IntPtr names);

		// TODO: need to setup a server that requires client-side certificates
		public IList<T>? GetDistinguishedNames<T> ()
		{
			IntPtr p;
			result = SSLCopyDistinguishedNames (Handle, out p);
			if (p == IntPtr.Zero)
				return null; // empty

			if (result != SslStatus.Success) {
				CFObject.CFRelease (p);
				return null; // error
			}
			var array = new CFArray (p, false);
			var list = new List<T> ();
			for (int i = 0; i < array.Count; i++) {
				// CFData -> X500DistinguishedName -> string
				list.Add (Convert <T> (p));
			}
			CFObject.CFRelease (p);
			return list;
		}

		T Convert<T> (IntPtr p)
		{
			object value = null;
			var tt = typeof(T);
			if (tt == typeof (NSData))
				value = new NSData (p);
			else if (tt == typeof (string))
				value = p.ToString ();
			// X500DistinguishedName
			// NSString ?
			return (T) value;
		}
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetCertificate (/* SSLContextRef */ IntPtr context, /* _Nullable CFArrayRef */ IntPtr certRefs);

		NSArray Bundle (SecIdentity? identity, IEnumerable<SecCertificate>? certificates)
		{
			int i = identity is null ? 0 : 1;
			int n = certificates is null ? 0 : certificates.Count ();
			var ptrs = new NativeHandle [n + i];
			if (i == 1)
				ptrs [0] = identity!.Handle;
			if (certificates is not null) {
				foreach (var certificate in certificates)
					ptrs [i++] = certificate.Handle;
			}
			return NSArray.FromIntPtrs (ptrs);
		}

		public SslStatus SetCertificate (SecIdentity identify, IEnumerable<SecCertificate> certificates)
		{
			using (var array = Bundle (identify, certificates)) {
				result = SSLSetCertificate (Handle, array.Handle);
				return result;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetClientCertificateState (/* SSLContextRef */ IntPtr context, out SslClientCertificateState clientState);

		public SslClientCertificateState ClientCertificateState {
			get {
				SslClientCertificateState value;
				result = SSLGetClientCertificateState (Handle, out value);
				return value;
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios9.0", "The use of different RSA certificates for signing and encryption is no longer allowed.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "The use of different RSA certificates for signing and encryption is no longer allowed.")]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetEncryptionCertificate (/* SSLContextRef */ IntPtr context, /* CFArrayRef */ IntPtr certRefs);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.11", "Export ciphers are not available anymore.")]
		[ObsoletedOSPlatform ("tvos13.0", "Export ciphers are not available anymore.")]
		[ObsoletedOSPlatform ("ios9.0", "Export ciphers are not available anymore.")]
#else
		[Deprecated (PlatformName.iOS, 9, 0, message: "Export ciphers are not available anymore.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Export ciphers are not available anymore.")]
#endif
		public SslStatus SetEncryptionCertificate (SecIdentity identify, IEnumerable<SecCertificate> certificates)
		{
			using (var array = Bundle (identify, certificates)) {
				result = SSLSetEncryptionCertificate (Handle, array.Handle);
				return result;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLCopyPeerTrust (/* SSLContextRef */ IntPtr context, /* SecTrustRef */ out IntPtr trust);

		public SecTrust? PeerTrust {
			get {
				IntPtr value;
				result = SSLCopyPeerTrust (Handle, out value);
				return (value == IntPtr.Zero) ? null : new SecTrust (value, true);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* CFType */ IntPtr SSLContextGetTypeID ();

		public static IntPtr GetTypeId ()
		{
			return SSLContextGetTypeID ();
		}

#if !WATCH
		// TODO: Headers say /* Deprecated, does nothing */ but we are not completly sure about it since there is no deprecation macro
		// Plus they added new members to SslSessionStrengthPolicy enum opened radar://23379052 https://trello.com/c/NbdTLVD3
		// Xcode 8 beta 1: the P/Invoke was removed completely.

#if !XAMCORE_5_0
#if NET
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("macos")]
#else
		[Unavailable (PlatformName.iOS, message: "'SetSessionStrengthPolicy' is not available anymore.")]
		[Unavailable (PlatformName.MacOSX, message: "'SetSessionStrengthPolicy' is not available anymore.")]
#endif
		[Obsolete ("'SetSessionStrengthPolicy' is not available anymore.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public SslStatus SetSessionStrengthPolicy (SslSessionStrengthPolicy policyStrength)
		{
			Runtime.NSLog ("SetSessionStrengthPolicy is not available anymore.");
			return SslStatus.Success;
		}
#endif
#endif // !XAMCORE_5_0

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern int SSLSetSessionConfig (IntPtr /* SSLContextRef* */ context, IntPtr /* CFStringRef* */ config);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public int SetSessionConfig (NSString config)
		{
			if (config is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (config));

			return SSLSetSessionConfig (Handle, config.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int SetSessionConfig (SslSessionConfig config)
		{
			return SetSessionConfig (config.GetConstant ()!);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern int SSLReHandshake (IntPtr /* SSLContextRef* */ context);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int ReHandshake ()
		{
			return SSLReHandshake (Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ SslStatus SSLCopyRequestedPeerName (IntPtr /* SSLContextRef* */ context, byte [] /* char* */ peerName, ref nuint /* size_t */ peerNameLen);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ SslStatus SSLCopyRequestedPeerNameLength (IntPtr /* SSLContextRef* */ context, ref nuint /* size_t */ peerNameLen);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public string GetRequestedPeerName ()
		{
			var result = String.Empty;
			nuint length = 0;
			if (SSLCopyRequestedPeerNameLength (Handle, ref length) == SslStatus.Success) {
				var bytes = new byte [length];
				if (SSLCopyRequestedPeerName (Handle, bytes, ref length) == SslStatus.Success)
					result = Encoding.UTF8.GetString (bytes);
			}
			return result;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ int SSLSetSessionTicketsEnabled (IntPtr /* SSLContextRef */ context, [MarshalAs (UnmanagedType.I1)] bool /* Boolean */ enabled);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int SetSessionTickets (bool enabled)
		{
			return SSLSetSessionTicketsEnabled (Handle, enabled);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ int SSLSetError (IntPtr /* SSLContextRef */ context, SecStatusCode /* OSStatus */ status);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int SetError (SecStatusCode status)
		{
			return SSLSetError (Handle, status);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ int SSLSetOCSPResponse (IntPtr /* SSLContextRef */ context, IntPtr /* CFDataRef __nonnull */ response);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int SetOcspResponse (NSData response)
		{
			if (response is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (response));
			return SSLSetOCSPResponse (Handle, response.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ int SSLSetALPNProtocols (IntPtr /* SSLContextRef */ context, IntPtr /* CFArrayRef */ protocols);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public int SetAlpnProtocols (string [] protocols)
		{
			using (var array = NSArray.FromStrings (protocols))
				return SSLSetALPNProtocols (Handle, array.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* OSStatus */ int SSLCopyALPNProtocols (IntPtr /* SSLContextRef */ context, ref IntPtr /* CFArrayRef* */ protocols);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif

		public string? [] GetAlpnProtocols (out int error)
		{
			IntPtr protocols = IntPtr.Zero; // must be null, CFArray allocated by SSLCopyALPNProtocols
			error = SSLCopyALPNProtocols (Handle, ref protocols);
			if (protocols == IntPtr.Zero)
				return Array.Empty<string> ();
			return CFArray.StringArrayFromHandle (protocols, true)!;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'Network.framework' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'Network.framework' instead.")]
#endif
		public string? [] GetAlpnProtocols ()
		{
			int error;
			return GetAlpnProtocols (out error);
		}
	}
}
