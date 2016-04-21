//
// SslContext
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014-2015 Xamarin Inc.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Security {
	[Mac (10,8)] // SSLCreateContext is 10.8, only constructor
	public class SslContext : INativeObject, IDisposable {

		SslConnection connection;
		SslStatus result;

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern static /* SSLContextRef */ IntPtr SSLCreateContext (/* CFAllocatorRef */ IntPtr alloc, SslProtocolSide protocolSide, SslConnectionType connectionType);

		public SslContext (SslProtocolSide protocolSide, SslConnectionType connectionType)
		{
			Handle = SSLCreateContext (IntPtr.Zero, protocolSide, connectionType);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLClose (/* SSLContextRef */ IntPtr context);

		~SslContext ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		bool disposed;

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;

			disposed = true;

			if (Handle != IntPtr.Zero)
				result = SSLClose (Handle);

			// don't remove the read/write delegates before we closed the connection, i.e.
			// the SSLClose will send an Alert for a "close notify"
			if (connection != null) {
				connection.Dispose ();
				connection = null;
			}

			// SSLCreateContext -> CFRelease (not SSLDisposeContext)
			if (Handle != IntPtr.Zero) {
				CFObject.CFRelease (Handle);
				Handle = IntPtr.Zero;
			}
		}

		public IntPtr Handle { get; private set; }

		public SslStatus GetLastStatus ()
		{
			return result;
		}

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetProtocolVersionMax (/* SSLContextRef */ IntPtr context, out SslProtocol maxVersion);

		[Mac (10,8)]
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

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetProtocolVersionMin (/* SSLContextRef */ IntPtr context, out SslProtocol minVersion);

		[Mac (10,8)]
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
		extern static /* OSStatus */ SslStatus SSLSetIOFuncs (/* SSLContextRef */ IntPtr context, /* SSLReadFunc */ SslReadFunc readFunc, /* SSLWriteFunc */ SslWriteFunc writeFunc);

		public SslConnection Connection {
			get {
				if (connection == null)
					return null;
				IntPtr value;
				result = SSLGetConnection (Handle, out value);
				if (value != connection.ConnectionId)
					throw new InvalidOperationException ();
				return connection;
			}
			set {
				// the read/write delegates needs to be set before set set the connection id
				if (value == null)
					result = SSLSetIOFuncs (Handle, null, null);
				else
					result = SSLSetIOFuncs (Handle, value.ReadFunc, value.WriteFunc);

				if (result == SslStatus.Success) {
					result = SSLSetConnection (Handle, value == null ? IntPtr.Zero : value.ConnectionId);
					connection = value;
				}
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLGetSessionOption (/* SSLContextRef */ IntPtr context, SslSessionOption option, out bool value);

		public SslStatus GetSessionOption (SslSessionOption option, out bool value)
		{
			result = SSLGetSessionOption (Handle, option, out value);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static /* OSStatus */ SslStatus SSLSetSessionOption (/* SSLContextRef */ IntPtr context, SslSessionOption option, bool value);

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

		public unsafe byte[] PeerId {
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
				nint length = (value == null) ? 0 : value.Length;
				fixed (byte *p = value) {
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

		internal unsafe SslStatus Read (byte[] data, int offset, int size, out nint processed)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			fixed (byte *d = &data [offset])
				result = SSLRead (Handle, d, size, out processed);
			return result;
		}

		public unsafe SslStatus Read (byte[] data, out nint processed)
		{
			int size = data == null ? 0 : data.Length;
			fixed (byte *d = data)
				result = SSLRead (Handle, d, size, out processed);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLWrite (/* SSLContextRef */ IntPtr context, /* const void* */ byte* data, /* size_t */ nint dataLength, /* size_t* */ out nint processed);

		internal unsafe SslStatus Write (byte[] data, int offset, int size, out nint processed)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			fixed (byte *d = &data [offset])
				result = SSLWrite (Handle, d, size, out processed);
			return result;
		}

		public unsafe SslStatus Write (byte[] data, out nint processed)
		{
			int size = data == null ? 0 : data.Length;
			fixed (byte *d = data)
				result = SSLWrite (Handle, d, size, out processed);
			return result;
		}


		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetNumberSupportedCiphers (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint numCiphers);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetSupportedCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite *ciphers, /* size_t* */ ref nint numCiphers);

		public unsafe IList<SslCipherSuite> GetSupportedCiphers ()
		{
			nint n;
			result = SSLGetNumberSupportedCiphers (Handle, out n);
			if ((result != SslStatus.Success) || (n <= 0))
				return null;

			var ciphers = new SslCipherSuite [n];
			fixed (SslCipherSuite *p = ciphers) {
				result = SSLGetSupportedCiphers (Handle, p, ref n);
				if (result != SslStatus.Success)
					return null;
			}
			return new List<SslCipherSuite> (ciphers);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetNumberEnabledCiphers (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint numCiphers);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetEnabledCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite *ciphers, /* size_t* */ ref nint numCiphers);

		public unsafe IList<SslCipherSuite> GetEnabledCiphers ()
		{
			nint n;
			result = SSLGetNumberEnabledCiphers (Handle, out n);
			if ((result != SslStatus.Success) || (n <= 0))
				return null;

			var ciphers = new SslCipherSuite [n];
			fixed (SslCipherSuite *p = ciphers) {
				result = SSLGetEnabledCiphers (Handle, p, ref n);
				if (result != SslStatus.Success)
					return null;
			}
			return new List<SslCipherSuite> (ciphers);
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetEnabledCiphers (/* SSLContextRef */ IntPtr context, SslCipherSuite *ciphers, /* size_t */ nint numCiphers);

		public unsafe SslStatus SetEnabledCiphers (IEnumerable<SslCipherSuite> ciphers)
		{
			if (ciphers == null)
				throw new ArgumentNullException ("ciphers");

			var array = ciphers.ToArray ();
			fixed (SslCipherSuite *p = array)
			result = SSLSetEnabledCiphers (Handle, p, ciphers.Count ());
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

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetDatagramWriteSize (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint bufSize);

		public nint DatagramWriteSize {
			get {
				nint value;
				result = SSLGetDatagramWriteSize (Handle, out value);
				return value;
			}
		}

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetMaxDatagramRecordSize (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint maxSize);

		[Mac (10,8)]
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

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetDatagramHelloCookie (/* SSLContextRef */ IntPtr context, /* const void* */ byte *cookie, nint cookieLength);

		public unsafe SslStatus SetDatagramHelloCookie (byte[] cookie)
		{
			nint len = cookie == null ? 0 : cookie.Length;
			fixed (byte *p = cookie)
				result = SSLSetDatagramHelloCookie (Handle, p, len);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetPeerDomainNameLength (/* SSLContextRef */ IntPtr context, /* size_t* */ out nint peerNameLen);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLGetPeerDomainName (/* SSLContextRef */ IntPtr context, /* char* */ byte[] peerName, /* size_t */ ref nint peerNameLen);

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetPeerDomainName (/* SSLContextRef */ IntPtr context, /* char* */ byte[] peerName, /* size_t */ nint peerNameLen);

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
				if (value == null) {
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
		public IList<T> GetDistinguishedNames<T> ()
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
		extern unsafe static /* OSStatus */ SslStatus SSLSetCertificate (/* SSLContextRef */ IntPtr context, /* CFArrayRef */ IntPtr certRefs);

		NSArray Bundle (SecIdentity identity, IEnumerable<SecCertificate> certificates)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");
			int i = 0;
			int n = certificates == null ? 0 : certificates.Count ();
			var ptrs = new IntPtr [n + 1];
			ptrs [0] = identity.Handle;
			foreach (var certificate in certificates)
				ptrs [++i] = certificate.Handle;
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

		[DllImport (Constants.SecurityLibrary)]
		[Availability (Deprecated = Platform.iOS_9_0 | Platform.Mac_10_11)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetEncryptionCertificate (/* SSLContextRef */ IntPtr context, /* CFArrayRef */ IntPtr certRefs);

		[Availability (Deprecated = Platform.iOS_9_0 | Platform.Mac_10_11, Message = "Export ciphers are not available anymore")]
		public SslStatus SetEncryptionCertificate (SecIdentity identify, IEnumerable<SecCertificate> certificates)
		{
			using (var array = Bundle (identify, certificates)) {
				result = SSLSetEncryptionCertificate (Handle, array.Handle);
				return result;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLCopyPeerTrust (/* SSLContextRef */ IntPtr context, /* SecTrustRef */ out IntPtr trust);

		public SecTrust PeerTrust {
			get {
				IntPtr value;
				result = SSLCopyPeerTrust (Handle, out value);
				return (value == IntPtr.Zero) ? null : new SecTrust (value);
			}
		}

		[Mac (10,8)]
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* CFType */ IntPtr SSLContextGetTypeID ();

		[Mac (10,8)]
		public static IntPtr GetTypeId ()
		{
			return SSLContextGetTypeID ();
		}

		[iOS(9,0)][Mac (10,11)]
		//[Availability (Deprecated = Platform.iOS_9_2 | Platform.Mac_10_11)]
		[DllImport (Constants.SecurityLibrary)]
		extern unsafe static /* OSStatus */ SslStatus SSLSetSessionStrengthPolicy (/* SSLContextRef */ IntPtr context, SslSessionStrengthPolicy policyStrength);

		[iOS(9,0)][Mac (10,11)]
		// TODO: Headers say /* Deprecated, does nothing */ but we are not completly sure about it since there is no deprecation macro
		// Plus they added new members to SslSessionStrengthPolicy enum opened radar://23379052 https://trello.com/c/NbdTLVD3
		//[Availability (Deprecated = Platform.iOS_9_2 | Platform.Mac_10_11, Message = "SetSessionStrengthPolicy is not available anymore")]
		public SslStatus SetSessionStrengthPolicy (SslSessionStrengthPolicy policyStrength)
		{
			return SSLSetSessionStrengthPolicy (Handle, policyStrength);
		}
	}
}
