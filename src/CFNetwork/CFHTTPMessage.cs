//
// CFHTTPMessage.cs:
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//      Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014 Xamarin Inc. (http://www.xamarin.com)
//

using System;
using System.Net;
using System.Security.Authentication;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

// CFHTTPMessage is in CFNetwork.framework, no idea why it ended up in CoreServices when it was bound.
#if XAMCORE_4_0
namespace CFNetwork {
#else
namespace CoreServices {
#endif

	public partial class CFHTTPMessage : CFType, INativeObject, IDisposable {
		internal IntPtr handle;

		internal CFHTTPMessage (IntPtr handle)
			: this (handle, false)
		{
		}

		internal CFHTTPMessage (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}


		[DllImport (Constants.CFNetworkLibrary, EntryPoint="CFHTTPMessageGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		~CFHTTPMessage ()
		{
			Dispose (false);
		}
		
		protected void CheckHandle ()
		{
			if (handle == IntPtr.Zero)
				throw new ObjectDisposedException (GetType ().Name);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get {
				CheckHandle ();
				return handle;
			}
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero) {
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		static IntPtr GetVersion (Version version)
		{
			if ((version == null) || version.Equals (HttpVersion.Version11))
				return _HTTPVersion1_1.Handle;
			else if (version.Equals (HttpVersion.Version10))
				return _HTTPVersion1_0.Handle;
			else if (version.Major == 2 && version.Minor == 0)
				return _HTTPVersion2_0.Handle;
			else
				throw new ArgumentException ();
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFHTTPMessageRef __nonnull */ IntPtr CFHTTPMessageCreateEmpty (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* Boolean */ bool isRequest);

		public static CFHTTPMessage CreateEmpty (bool request)
		{
			var handle = CFHTTPMessageCreateEmpty (IntPtr.Zero, request);
			return new CFHTTPMessage (handle);			
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFHTTPMessageRef __nonnull */ IntPtr CFHTTPMessageCreateRequest (
			/* CFAllocatorRef __nullable */ IntPtr alloc, /* CFStringRef __nonnull*/ IntPtr requestMethod,
			/* CFUrlRef __nonnull */ IntPtr url, /* CFStringRef __nonnull */ IntPtr httpVersion);

		public static CFHTTPMessage CreateRequest (CFUrl url, NSString method, Version version)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			if (method == null)
				throw new ArgumentNullException ("method");

			var handle = CFHTTPMessageCreateRequest (
				IntPtr.Zero, method.Handle, url.Handle, GetVersion (version));
			return new CFHTTPMessage (handle);
		}

		public static CFHTTPMessage CreateRequest (Uri uri, string method)
		{
			return CreateRequest (uri, method, null);
		}

		public static CFHTTPMessage CreateRequest (Uri uri, string method, Version version)
		{
			if (uri == null)
				throw new ArgumentNullException ("uri");

			CFUrl urlRef = null;
			NSString methodRef = null;

			var escaped = Uri.EscapeUriString (uri.ToString ());

			try {
				urlRef = CFUrl.FromUrlString (escaped, null);
				if (urlRef == null)
					throw new ArgumentException ("Invalid URL.");
				methodRef = new NSString (method);

				return CreateRequest (urlRef, methodRef, version);
			} finally {
				if (urlRef != null)
					urlRef.Dispose ();
				if (methodRef != null)
					methodRef.Dispose ();
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageIsRequest (/* CFHTTPMessageRef __nonnull */ IntPtr message);

		public bool IsRequest {
			get {
				CheckHandle ();
				return CFHTTPMessageIsRequest (Handle);
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFIndex */ nint CFHTTPMessageGetResponseStatusCode (
			/* CFHTTPMessageRef __nonnull */ IntPtr response);

		public HttpStatusCode ResponseStatusCode {
			get {
				if (IsRequest)
					throw new InvalidOperationException ();
				int status = (int) CFHTTPMessageGetResponseStatusCode (Handle);
				return (HttpStatusCode)status;
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CFHTTPMessageCopyResponseStatusLine (
			/* CFHTTPMessageRef __nonnull */ IntPtr response);

		public string ResponseStatusLine {
			get {
				if (IsRequest)
					throw new InvalidOperationException ();
				var ptr = CFHTTPMessageCopyResponseStatusLine (Handle);
				if (ptr == IntPtr.Zero)
					return null;
				using (var line = new NSString (ptr))
					return line.ToString ();
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFStringRef __nonnull */ IntPtr CFHTTPMessageCopyVersion (
			/* CFHTTPMessageRef __nonnull */ IntPtr message);

		public Version Version {
			get {
				CheckHandle ();
				IntPtr ptr = CFHTTPMessageCopyVersion (handle);
				try {
					// FIXME: .NET HttpVersion does not include (yet) Version20, so Version11 is returned
					if (ptr == _HTTPVersion1_0.Handle)
						return HttpVersion.Version10;
					else
						return HttpVersion.Version11;
				} finally {
					if (ptr != IntPtr.Zero)
						CFObject.CFRelease (ptr);
				}
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageIsHeaderComplete (
			/* CFHTTPMessageRef __nonnull */ IntPtr message);

		public bool IsHeaderComplete {
			get {
				CheckHandle ();
				return CFHTTPMessageIsHeaderComplete (Handle);
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageAppendBytes (
			/* CFHTTPMessageRef __nonnull */ IntPtr message,
			/* const UInt8* __nonnull */ byte[] newBytes, /* CFIndex */ nint numBytes);

		public bool AppendBytes (byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");

			return CFHTTPMessageAppendBytes (Handle, bytes, bytes.Length);
		}

		public bool AppendBytes (byte[] bytes, nint count)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");

			return CFHTTPMessageAppendBytes (Handle, bytes, count);
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CFHTTPMessageCopyAllHeaderFields (
			/* CFHTTPMessageRef __nonnull */ IntPtr message);

		public NSDictionary GetAllHeaderFields ()
		{
			CheckHandle ();
			return Runtime.GetNSObject <NSDictionary> (CFHTTPMessageCopyAllHeaderFields (handle));
		}

		#region Authentication

		// CFStream.h
		struct CFStreamError {
			public /* CFIndex (CFStreamErrorDomain) */ nint domain;
			public /* SInt32 */ int code;
		}

		// untyped enum -> CFHTTPAuthentication.h
		enum CFStreamErrorHTTPAuthentication {
			TypeUnsupported = -1000,
			BadUserName = -1001,
			BadPassword = -1002
		}

		AuthenticationException GetException (CFStreamErrorHTTPAuthentication code)
		{
			switch (code) {
			case CFStreamErrorHTTPAuthentication.BadUserName:
				throw new InvalidCredentialException ("Bad username.");
			case CFStreamErrorHTTPAuthentication.BadPassword:
				throw new InvalidCredentialException ("Bad password.");
			case CFStreamErrorHTTPAuthentication.TypeUnsupported:
				throw new AuthenticationException ("Authentication type not supported.");
			default:
				throw new AuthenticationException ("Unknown error.");
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageApplyCredentials (/* CFHTTPMessageRef */ IntPtr request, 
			/* CFHTTPAuthenticationRef */ IntPtr auth, /* CFString */ IntPtr username, /* CFString */ IntPtr password,
			/* CFStreamError* */ out CFStreamError error);

		public void ApplyCredentials (CFHTTPAuthentication auth, NetworkCredential credential)
		{
			if (auth == null)
				throw new ArgumentNullException ("auth");
			if (credential == null)
				throw new ArgumentNullException ("credential");

			if (auth.RequiresAccountDomain) {
				ApplyCredentialDictionary (auth, credential);
				return;
			}

			var username = new CFString (credential.UserName);
			var password = new CFString (credential.Password);

			try {
				CFStreamError error;

				var ok = CFHTTPMessageApplyCredentials (
					Handle, auth.Handle, username.Handle, password.Handle,
					out error);
				if (!ok)
					throw GetException ((CFStreamErrorHTTPAuthentication) error.code);
			} finally {
				username.Dispose ();
				password.Dispose ();
			}
		}

		// convenience enum on top of kCFHTTPAuthenticationScheme* NSString fields
		public enum AuthenticationScheme {
			Default,
			Basic,
			Negotiate,
			NTLM,
			Digest,
			OAuth1
		}

		internal static IntPtr GetAuthScheme (AuthenticationScheme scheme)
		{
			switch (scheme) {
			case AuthenticationScheme.Default:
				return IntPtr.Zero;
			case AuthenticationScheme.Basic:
				return _AuthenticationSchemeBasic.Handle;
			case AuthenticationScheme.Negotiate:
				return _AuthenticationSchemeNegotiate.Handle;
			case AuthenticationScheme.NTLM:
				return _AuthenticationSchemeNTLM.Handle;
			case AuthenticationScheme.Digest:
				return _AuthenticationSchemeDigest.Handle;
			case AuthenticationScheme.OAuth1:
				if (_AuthenticationSchemeOAuth1 == null)
					throw new NotSupportedException ("requires iOS 7.0 or OSX 10.9");
				return _AuthenticationSchemeOAuth1.Handle;
			default:
				throw new ArgumentException ();
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageAddAuthentication (
			/* CFHTTPMessageRef __nonnull */ IntPtr request, 
			/* CFHTTPMessageRef __nullable */ IntPtr authenticationFailureResponse,
			/* CFStringRef __nonnull */ IntPtr username, 
			/* CFStringRef __nonnull */ IntPtr password,
			/* CFStringRef __nullable */ IntPtr authenticationScheme,
			/* Boolean */ bool forProxy);

		public bool AddAuthentication (CFHTTPMessage failureResponse, NSString username,
		                               NSString password, AuthenticationScheme scheme,
		                               bool forProxy)
		{
			if (username == null)
				throw new ArgumentNullException ("username");
			if (password == null)
				throw new ArgumentNullException ("password");

			return CFHTTPMessageAddAuthentication (
				Handle, failureResponse.GetHandle (), username.Handle,
				password.Handle, GetAuthScheme (scheme), forProxy);
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* Boolean */ bool CFHTTPMessageApplyCredentialDictionary (/* CFHTTPMessageRef */ IntPtr request, 
			/* CFHTTPAuthenticationRef */ IntPtr auth, /* CFDictionaryRef */ IntPtr dict, /* CFStreamError* */ out CFStreamError error);

		public void ApplyCredentialDictionary (CFHTTPAuthentication auth, NetworkCredential credential)
		{
			if (auth == null)
				throw new ArgumentNullException ("auth");
			if (credential == null)
				throw new ArgumentNullException ("credential");

			var keys = new NSString [3];
			var values = new CFString [3];
			keys [0] = _AuthenticationUsername;
			keys [1] = _AuthenticationPassword;
			keys [2] = _AuthenticationAccountDomain;
			values [0] = (CFString)credential.UserName;
			values [1] = (CFString)credential.Password;
			values [2] = credential.Domain != null ? (CFString)credential.Domain : null;

			var dict = CFDictionary.FromObjectsAndKeys (values, keys);

			try {
				CFStreamError error;
				var ok = CFHTTPMessageApplyCredentialDictionary (
					Handle, auth.Handle, dict.Handle, out error);
				if (ok)
					return;
				throw GetException ((CFStreamErrorHTTPAuthentication) error.code);
			} finally {
				dict.Dispose ();
				values [0].Dispose ();
				values [1].Dispose ();
				if (values [2] != null)
					values [2].Dispose ();
			}
		}

		#endregion

		[DllImport (Constants.CFNetworkLibrary)]
		extern static void CFHTTPMessageSetHeaderFieldValue (/* CFHTTPMessageRef __nonnull */IntPtr message, 
			/* CFStringRef __nonnull */ IntPtr headerField, /* CFStringRef __nullable */ IntPtr value);

		public void SetHeaderFieldValue (string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			NSString nstr = (NSString)name;
			NSString vstr = value != null ? (NSString)value : null;
			IntPtr vptr = vstr != null ? vstr.Handle : IntPtr.Zero;

			CFHTTPMessageSetHeaderFieldValue (Handle, nstr.Handle, vptr);

			nstr.Dispose ();
			if (vstr != null)
				vstr.Dispose ();
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static void CFHTTPMessageSetBody (/* CFHTTPMessageRef __nonnull */ IntPtr message,
			/* CFDataRef__nonnull  */ IntPtr data);

		public void SetBody (byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");
			
			using (var data = new CFDataBuffer (buffer))
				CFHTTPMessageSetBody (Handle, data.Handle);
		}
	}
}
