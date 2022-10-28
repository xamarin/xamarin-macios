//
// SecProtocolOptions.cs: Bindings the Security sec_protocol_options_t
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

using sec_protocol_options_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;
using sec_identity_t=System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {

#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12,0)]
	[Mac (10,14)]
	[iOS (12,0)]
	[Watch (5,0)]
#endif
	public class SecProtocolOptions : NativeObject {
#if !COREBUILD
		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public
		[Preserve (Conditional = true)]
		internal SecProtocolOptions (NativeHandle handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_local_identity (sec_protocol_options_t handle, sec_identity_t identity);

		public void SetLocalIdentity (SecIdentity2 identity)
		{
			if (identity is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			sec_protocol_options_set_local_identity (GetCheckedHandle (), identity.GetCheckedHandle ());
		}

#if !NET
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite (sec_protocol_options_t handle, SslCipherSuite cipherSuite);
#endif

#if !NET
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'AddTlsCipherSuite (TlsCipherSuite)' instead.")]
		[Unavailable (PlatformName.MacCatalyst)]
		public void AddTlsCipherSuite (SslCipherSuite cipherSuite) => sec_protocol_options_add_tls_ciphersuite (GetCheckedHandle (), cipherSuite);
#endif

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_append_tls_ciphersuite (sec_protocol_options_t options, TlsCipherSuite ciphersuite);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public void AddTlsCipherSuite (TlsCipherSuite cipherSuite) => sec_protocol_options_append_tls_ciphersuite (GetCheckedHandle (), cipherSuite);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite_group (sec_protocol_options_t handle, SslCipherSuiteGroup cipherSuiteGroup);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'AddTlsCipherSuiteGroup (TlsCipherSuiteGroup)' instead.")]
#endif
		public void AddTlsCipherSuiteGroup (SslCipherSuiteGroup cipherSuiteGroup) => sec_protocol_options_add_tls_ciphersuite_group (GetCheckedHandle (), cipherSuiteGroup);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_append_tls_ciphersuite_group (sec_protocol_options_t options, TlsCipherSuiteGroup group);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public void AddTlsCipherSuiteGroup (TlsCipherSuiteGroup cipherSuiteGroup) => sec_protocol_options_append_tls_ciphersuite_group (GetCheckedHandle (), cipherSuiteGroup);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_min_version (sec_protocol_options_t handle, SslProtocol protocol);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'SetTlsMinVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'SetTlsMinVersion (TlsProtocolVersion)' instead.")]
		[Unavailable (PlatformName.MacCatalyst)]
#endif
		public void SetTlsMinVersion (SslProtocol protocol) => sec_protocol_options_set_tls_min_version (GetCheckedHandle (), protocol);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_min_tls_protocol_version (sec_protocol_options_t handle, TlsProtocolVersion version);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public void SetTlsMinVersion (TlsProtocolVersion protocol) => sec_protocol_options_set_min_tls_protocol_version (GetCheckedHandle (), protocol);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_max_version (sec_protocol_options_t handle, SslProtocol protocol);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'SetTlsMaxVersion (TlsProtocolVersion)' instead.")]
		[Unavailable (PlatformName.MacCatalyst)]
#endif
		public void SetTlsMaxVersion (SslProtocol protocol) => sec_protocol_options_set_tls_max_version (GetCheckedHandle (), protocol);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_max_tls_protocol_version (sec_protocol_options_t handle, TlsProtocolVersion version);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public void SetTlsMaxVersion (TlsProtocolVersion protocol) => sec_protocol_options_set_max_tls_protocol_version (GetCheckedHandle (), protocol);


#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_options_get_default_min_dtls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		static public TlsProtocolVersion DefaultMinDtlsProtocolVersion => sec_protocol_options_get_default_min_dtls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_options_get_default_max_dtls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		static public TlsProtocolVersion DefaultMaxDtlsProtocolVersion => sec_protocol_options_get_default_max_dtls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_options_get_default_min_tls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		static public TlsProtocolVersion DefaultMinTlsProtocolVersion => sec_protocol_options_get_default_min_tls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_options_get_default_max_tls_protocol_version ();

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		static public TlsProtocolVersion DefaultMaxTlsProtocolVersion => sec_protocol_options_get_default_max_tls_protocol_version ();

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_application_protocol (sec_protocol_options_t handle, string applicationProtocol);

		public void AddTlsApplicationProtocol (string applicationProtocol)
		{
			if (applicationProtocol is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (applicationProtocol));
			sec_protocol_options_add_tls_application_protocol (GetCheckedHandle (), applicationProtocol);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_server_name (sec_protocol_options_t handle, string serverName);

		public void SetTlsServerName (string serverName)
		{
			if (serverName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (serverName));
			sec_protocol_options_set_tls_server_name (GetCheckedHandle (), serverName);
		}

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use non-DHE cipher suites instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_diffie_hellman_parameters (IntPtr handle, IntPtr dispatchDataParameter);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use non-DHE cipher suites instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use non-DHE cipher suites instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use non-DHE cipher suites instead.")]
#endif
		public void SetTlsDiffieHellmanParameters (DispatchData parameters)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			sec_protocol_options_set_tls_diffie_hellman_parameters (GetCheckedHandle (), parameters.Handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_pre_shared_key (IntPtr handle, IntPtr dispatchDataParameter);

		public void AddPreSharedKey (DispatchData parameters)
		{
			if (parameters is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameters));
			sec_protocol_options_set_tls_diffie_hellman_parameters (GetCheckedHandle (), parameters.Handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_tickets_enabled (IntPtr handle, byte ticketsEnabled);

		public void SetTlsTicketsEnabled (bool ticketsEnabled) => sec_protocol_options_set_tls_tickets_enabled (GetCheckedHandle (), (byte)(ticketsEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_is_fallback_attempt (IntPtr handle, byte isFallbackAttempt);

		public void SetTlsIsFallbackAttempt (bool isFallbackAttempt) => sec_protocol_options_set_tls_is_fallback_attempt (GetCheckedHandle (), (byte)(isFallbackAttempt ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_resumption_enabled (IntPtr handle, byte resumptionEnabled);

		public void SetTlsResumptionEnabled (bool resumptionEnabled) => sec_protocol_options_set_tls_resumption_enabled (GetCheckedHandle (), (byte)(resumptionEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_false_start_enabled (IntPtr handle, byte falseStartEnabled);

		public void SetTlsFalseStartEnabled (bool falseStartEnabled) => sec_protocol_options_set_tls_false_start_enabled (GetCheckedHandle (), (byte)(falseStartEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_ocsp_enabled (IntPtr handle, byte ocspEnabled);

		public void SetTlsOcspEnabled (bool ocspEnabled) => sec_protocol_options_set_tls_ocsp_enabled (GetCheckedHandle (), (byte)(ocspEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_sct_enabled (IntPtr handle, byte sctEnabled);

		public void SetTlsSignCertificateTimestampEnabled (bool sctEnabled) => sec_protocol_options_set_tls_sct_enabled (GetCheckedHandle (), (byte)(sctEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_renegotiation_enabled (IntPtr handle, byte renegotiationEnabled);

		public void SetTlsRenegotiationEnabled (bool renegotiationEnabled) => sec_protocol_options_set_tls_renegotiation_enabled (GetCheckedHandle (), (byte)(renegotiationEnabled ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_peer_authentication_required (IntPtr handle, byte peerAuthenticationRequired);

		public void SetPeerAuthenticationRequired (bool peerAuthenticationRequired) => sec_protocol_options_set_peer_authentication_required (GetCheckedHandle (), (byte)(peerAuthenticationRequired ? 1 : 0));

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_key_update_block (sec_protocol_options_t options, ref BlockLiteral key_update_block, dispatch_queue_t key_update_queue);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetKeyUpdateCallback (SecProtocolKeyUpdate keyUpdate, DispatchQueue keyUpdateQueue)
		{
			if (keyUpdate is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (keyUpdate));
			if (keyUpdateQueue is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (keyUpdateQueue));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (Trampolines.SDSecProtocolKeyUpdate.Handler, keyUpdate);

			sec_protocol_options_set_key_update_block (Handle, ref block_handler, keyUpdateQueue.Handle);
			block_handler.CleanupBlock ();
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
 		static extern bool sec_protocol_options_are_equal (sec_protocol_options_t optionsA, sec_protocol_options_t optionsB);

		// Equatable would be nice but would fail on earlier OS versions
#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public bool IsEqual (SecProtocolOptions other)
		{
			if (other is null)
				return false;
			return sec_protocol_options_are_equal (GetCheckedHandle (), other.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		static public bool IsEqual (SecProtocolOptions optionsA, SecProtocolOptions optionsB)
		{
			if (optionsA is null)
				return (optionsB is null);
			else if (optionsB is null)
				return false;
			return sec_protocol_options_are_equal (optionsA.Handle, optionsB.Handle);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_pre_shared_key_identity_hint (sec_protocol_options_t options, IntPtr /* dispatch_data */ psk_identity_hint);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6,0)]
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
#endif
		public void SetTlsPreSharedKeyIdentityHint (DispatchData pskIdentityHint)
		{
			if (pskIdentityHint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (pskIdentityHint));
			sec_protocol_options_set_tls_pre_shared_key_identity_hint (GetCheckedHandle (), pskIdentityHint.Handle);
		}
#endif
	}
}
