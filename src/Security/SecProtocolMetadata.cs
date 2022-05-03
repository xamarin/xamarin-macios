//
// SecProtocolMetadata.cs: Bindings the Security sec_protocol_metadata_t
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
using Security;
using sec_protocol_metadata_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;

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
	public class SecProtocolMetadata : NativeObject {
#if !NET
		internal SecProtocolMetadata (NativeHandle handle) : base (handle, false) {}
#endif

		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public[tv
		[Preserve (Conditional = true)]
		internal SecProtocolMetadata (NativeHandle handle, bool owns) : base (handle, owns) {}

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_get_negotiated_protocol (IntPtr handle);

		public string? NegotiatedProtocol => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_negotiated_protocol (GetCheckedHandle ()));

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_peer_public_key (IntPtr handle);

		public DispatchData? PeerPublicKey => CreateDispatchData (sec_protocol_metadata_copy_peer_public_key (GetCheckedHandle ()));

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SslProtocol sec_protocol_metadata_get_negotiated_protocol_version (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos10.14")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15")]
		[UnsupportedOSPlatform ("tvos13.0")]
		[UnsupportedOSPlatform ("ios13.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.0 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.0 use 'NegotiatedTlsProtocolVersion' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
#endif
		public SslProtocol NegotiatedProtocolVersion => sec_protocol_metadata_get_negotiated_protocol_version (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
		[Watch (6,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_metadata_get_negotiated_tls_protocol_version (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
		[Watch (6,0)]
#endif
		public TlsProtocolVersion NegotiatedTlsProtocolVersion => sec_protocol_metadata_get_negotiated_tls_protocol_version (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
		[Watch (6,0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsCipherSuite sec_protocol_metadata_get_negotiated_tls_ciphersuite (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13,0)]
		[Mac (10,15)]
		[iOS (13,0)]
		[Watch (6,0)]
#endif
		public TlsCipherSuite NegotiatedTlsCipherSuite => sec_protocol_metadata_get_negotiated_tls_ciphersuite (GetCheckedHandle ());

#if !NET
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[DllImport (Constants.SecurityLibrary)]
		extern static SslCipherSuite sec_protocol_metadata_get_negotiated_ciphersuite (IntPtr handle);
#endif

#if !NET
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.WatchOS, 6,0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		public SslCipherSuite NegotiatedCipherSuite => sec_protocol_metadata_get_negotiated_ciphersuite (GetCheckedHandle ());
#endif

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_get_early_data_accepted (IntPtr handle);

		public bool EarlyDataAccepted => sec_protocol_metadata_get_early_data_accepted (GetCheckedHandle ()) != 0;

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool sec_protocol_metadata_challenge_parameters_are_equal (IntPtr metadataA, IntPtr metadataB);

		public static bool ChallengeParametersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			if (metadataA == null)
				return metadataB == null;
			else if (metadataB == null)
				return false; // This was tested in a native app. We do copy the behaviour.
			return sec_protocol_metadata_challenge_parameters_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool sec_protocol_metadata_peers_are_equal (IntPtr metadataA, IntPtr metadataB);

		public static bool PeersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			if (metadataA == null)
				return metadataB == null;
			else if (metadataB == null)
				return false; // This was tested in a native app. We do copy the behaviour.
			return sec_protocol_metadata_peers_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

		delegate void sec_protocol_metadata_access_distinguished_names_handler_t (IntPtr block, IntPtr dispatchData);
 		static sec_protocol_metadata_access_distinguished_names_handler_t static_DistinguishedNamesForPeer = TrampolineDistinguishedNamesForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_distinguished_names_handler_t))]
 		static void TrampolineDistinguishedNamesForPeer (IntPtr block, IntPtr data)
 		{
 			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
 			if (del != null) {
 				var dispatchData = new DispatchData (data, owns: false);
 				del (dispatchData);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
 		static extern bool sec_protocol_metadata_access_distinguished_names (IntPtr handle, ref BlockLiteral callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetDistinguishedNamesForPeerHandler (Action<DispatchData> callback)
 		{
			if (callback == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			var block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_DistinguishedNamesForPeer, callback);

			try {
				if (!sec_protocol_metadata_access_distinguished_names (GetCheckedHandle (), ref block_handler)) {
					throw new InvalidOperationException ("Distinguished names are not accessible.");
				}
			} finally {
				block_handler.CleanupBlock ();
			}
 		}

		delegate void sec_protocol_metadata_access_ocsp_response_handler_t (IntPtr block, IntPtr dispatchData);
 		static sec_protocol_metadata_access_ocsp_response_handler_t static_OcspReposeForPeer = TrampolineOcspReposeForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_ocsp_response_handler_t))]
 		static void TrampolineOcspReposeForPeer (IntPtr block, IntPtr data)
 		{
 			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
 			if (del != null) {
 				var dispatchData = new DispatchData (data, owns: false);
 				del (dispatchData);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
 		static extern bool sec_protocol_metadata_access_ocsp_response (IntPtr handle, ref BlockLiteral callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetOcspResponseForPeerHandler (Action<DispatchData> callback)
 		{
			if (callback == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			var block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_OcspReposeForPeer, callback);

			try {
				if (!sec_protocol_metadata_access_ocsp_response (GetCheckedHandle (), ref block_handler)) {
					throw new InvalidOperationException ("The OSCP response is not accessible.");
				}
			} finally {
				block_handler.CleanupBlock ();
			}
 		}

		delegate void sec_protocol_metadata_access_peer_certificate_chain_handler_t (IntPtr block, IntPtr certificate);
 		static sec_protocol_metadata_access_peer_certificate_chain_handler_t static_CertificateChainForPeer = TrampolineCertificateChainForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_peer_certificate_chain_handler_t))]
 		static void TrampolineCertificateChainForPeer (IntPtr block, IntPtr certificate)
 		{
 			var del = BlockLiteral.GetTarget<Action<SecCertificate>> (block);
 			if (del != null) {
 				var secCertificate = new SecCertificate (certificate, owns: false);
 				del (secCertificate);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
 		static extern bool sec_protocol_metadata_access_peer_certificate_chain (IntPtr handle, ref BlockLiteral callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetCertificateChainForPeerHandler (Action<SecCertificate> callback)
 		{
			if (callback == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			var block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_CertificateChainForPeer, callback);

			try {
				if (!sec_protocol_metadata_access_peer_certificate_chain (GetCheckedHandle (), ref block_handler)) {
					throw new InvalidOperationException ("The peer certificates are not accessible.");
				}
			} finally {
				block_handler.CleanupBlock ();
			}
 		}

		delegate void sec_protocol_metadata_access_supported_signature_algorithms_handler_t (IntPtr block, ushort signatureAlgorithm);
 		static sec_protocol_metadata_access_supported_signature_algorithms_handler_t static_SignatureAlgorithmsForPeer = TrampolineSignatureAlgorithmsForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_supported_signature_algorithms_handler_t))]
 		static void TrampolineSignatureAlgorithmsForPeer (IntPtr block, ushort signatureAlgorithm)
 		{
 			var del = BlockLiteral.GetTarget<Action<ushort>> (block);
 			if (del != null) {
 				del (signatureAlgorithm);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
 		static extern bool sec_protocol_metadata_access_supported_signature_algorithms (IntPtr handle, ref BlockLiteral callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetSignatureAlgorithmsForPeerHandler (Action<ushort> callback)
 		{
			if (callback == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			var block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (static_SignatureAlgorithmsForPeer, callback);

			try {
				if (!sec_protocol_metadata_access_supported_signature_algorithms (GetCheckedHandle (), ref block_handler)) {
					throw new InvalidOperationException ("The supported signature list is not accessible.");
				}
			} finally {
				block_handler.CleanupBlock ();
			}
 		}

		[DllImport (Constants.SecurityLibrary)]
		static extern /* OS_dispatch_data */ IntPtr sec_protocol_metadata_create_secret (/* OS_sec_protocol_metadata */ IntPtr metadata, /* size_t */ nuint label_len, /* const char*/ [MarshalAs(UnmanagedType.LPStr)] string label, /* size_t */ nuint exporter_length);

		public DispatchData? CreateSecret (string label, nuint exporterLength)
		{
			if (label == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (label));
			return CreateDispatchData (sec_protocol_metadata_create_secret (GetCheckedHandle (), (nuint) label.Length, label, exporterLength));
		}

		[DllImport (Constants.SecurityLibrary)]
		static unsafe extern /* OS_dispatch_data */ IntPtr sec_protocol_metadata_create_secret_with_context (/* OS_sec_protocol_metadata */ IntPtr metadata, /* size_t */ nuint label_len, /* const char*/ [MarshalAs(UnmanagedType.LPStr)] string label, /* size_t */  nuint context_len, byte* context, /* size_t */ nuint exporter_length);

		public unsafe DispatchData? CreateSecret (string label, byte[] context, nuint exporterLength)
		{
			if (label == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (label));
			if (context == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			fixed (byte* p = context)
				return CreateDispatchData (sec_protocol_metadata_create_secret_with_context (GetCheckedHandle (), (nuint) label.Length, label, (nuint) context.Length, p, exporterLength));
		}

		// API returning `OS_dispatch_data` can also return `null` and
		// a managed instance with (with an empty handle) is not the same
		internal static DispatchData? CreateDispatchData (IntPtr handle)
		{
			return handle == IntPtr.Zero ? null : new DispatchData (handle, owns: true);
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
		static extern /* const char* */ IntPtr sec_protocol_metadata_get_server_name (IntPtr /* sec_protocol_metadata_t */ handle);

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
		public string? ServerName => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_server_name (GetCheckedHandle ()));

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
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool sec_protocol_metadata_access_pre_shared_keys (IntPtr /* sec_protocol_metadata_t */ handle, ref BlockLiteral block);

		public delegate void SecAccessPreSharedKeysHandler (DispatchData psk, DispatchData pskIdentity);

		internal delegate void AccessPreSharedKeysHandler (IntPtr block, IntPtr dd_psk, IntPtr dd_psk_identity);
		static readonly AccessPreSharedKeysHandler presharedkeys = TrampolineAccessPreSharedKeys;

		[MonoPInvokeCallback (typeof (AccessPreSharedKeysHandler))]
		static void TrampolineAccessPreSharedKeys (IntPtr block, IntPtr psk, IntPtr psk_identity)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData?,DispatchData?>> (block);
			if (del != null)
				del (CreateDispatchData (psk), CreateDispatchData (psk_identity));
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
		// no [Async] as it can be called multiple times
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool AccessPreSharedKeys (SecAccessPreSharedKeysHandler handler)
		{
			if (handler == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			BlockLiteral block_handler = new BlockLiteral ();
			try {
				block_handler.SetupBlockUnsafe (presharedkeys, handler);
				return sec_protocol_metadata_access_pre_shared_keys (GetCheckedHandle (), ref block_handler);
			}
			finally {
				block_handler.CleanupBlock ();
			}
		}
#endif
	}
}
