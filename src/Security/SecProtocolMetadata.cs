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
using sec_protocol_metadata_t = System.IntPtr;
using dispatch_queue_t = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Security {
#if NET
	[SupportedOSPlatform ("tvos12.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios12.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (12, 0)]
	[iOS (12, 0)]
	[Watch (5, 0)]
#endif
	public class SecProtocolMetadata : NativeObject {
#if !NET
		internal SecProtocolMetadata (NativeHandle handle) : base (handle, false) { }
#endif

		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public[tv
		[Preserve (Conditional = true)]
		internal SecProtocolMetadata (NativeHandle handle, bool owns) : base (handle, owns) { }

#if !COREBUILD
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_get_negotiated_protocol (IntPtr handle);

		public string? NegotiatedProtocol => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_negotiated_protocol (GetCheckedHandle ()));

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_peer_public_key (IntPtr handle);

		public DispatchData? PeerPublicKey => CreateDispatchData (sec_protocol_metadata_copy_peer_public_key (GetCheckedHandle ()));

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SslProtocol sec_protocol_metadata_get_negotiated_protocol_version (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("macos10.15", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
#else
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NegotiatedTlsProtocolVersion' instead.")]
#endif
		public SslProtocol NegotiatedProtocolVersion => sec_protocol_metadata_get_negotiated_protocol_version (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_metadata_get_negotiated_tls_protocol_version (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		public TlsProtocolVersion NegotiatedTlsProtocolVersion => sec_protocol_metadata_get_negotiated_tls_protocol_version (GetCheckedHandle ());

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsCipherSuite sec_protocol_metadata_get_negotiated_tls_ciphersuite (IntPtr handle);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[TV (13, 0)]
		[iOS (13, 0)]
		[Watch (6, 0)]
#endif
		public TlsCipherSuite NegotiatedTlsCipherSuite => sec_protocol_metadata_get_negotiated_tls_ciphersuite (GetCheckedHandle ());

#if !NET
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[DllImport (Constants.SecurityLibrary)]
		extern static SslCipherSuite sec_protocol_metadata_get_negotiated_ciphersuite (IntPtr handle);
#endif

#if !NET
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NegotiatedTlsCipherSuite' instead.")]
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
			if (metadataA is null)
				return metadataB is null;
			else if (metadataB is null)
				return false; // This was tested in a native app. We do copy the behaviour.
			return sec_protocol_metadata_challenge_parameters_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool sec_protocol_metadata_peers_are_equal (IntPtr metadataA, IntPtr metadataB);

		public static bool PeersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			if (metadataA is null)
				return metadataB is null;
			else if (metadataB is null)
				return false; // This was tested in a native app. We do copy the behaviour.
			return sec_protocol_metadata_peers_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

#if !NET
		delegate void sec_protocol_metadata_access_distinguished_names_handler_t (IntPtr block, IntPtr dispatchData);
		static sec_protocol_metadata_access_distinguished_names_handler_t static_DistinguishedNamesForPeer = TrampolineDistinguishedNamesForPeer;

		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_distinguished_names_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineDistinguishedNamesForPeer (IntPtr block, IntPtr data)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
			if (del is not null) {
				var dispatchData = new DispatchData (data, owns: false);
				del (dispatchData);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool sec_protocol_metadata_access_distinguished_names (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetDistinguishedNamesForPeerHandler (Action<DispatchData> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineDistinguishedNamesForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineDistinguishedNamesForPeer));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_DistinguishedNamesForPeer, callback);
#endif
				if (!sec_protocol_metadata_access_distinguished_names (GetCheckedHandle (), &block))
					throw new InvalidOperationException ("Distinguished names are not accessible.");
			}
		}

#if !NET
		delegate void sec_protocol_metadata_access_ocsp_response_handler_t (IntPtr block, IntPtr dispatchData);
		static sec_protocol_metadata_access_ocsp_response_handler_t static_OcspReposeForPeer = TrampolineOcspReposeForPeer;

		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_ocsp_response_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineOcspReposeForPeer (IntPtr block, IntPtr data)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
			if (del is not null) {
				var dispatchData = new DispatchData (data, owns: false);
				del (dispatchData);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool sec_protocol_metadata_access_ocsp_response (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetOcspResponseForPeerHandler (Action<DispatchData> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineOcspReposeForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineOcspReposeForPeer));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_OcspReposeForPeer, callback);
#endif
				if (!sec_protocol_metadata_access_ocsp_response (GetCheckedHandle (), &block))
					throw new InvalidOperationException ("The OSCP response is not accessible.");
			}
		}

#if !NET
		delegate void sec_protocol_metadata_access_peer_certificate_chain_handler_t (IntPtr block, IntPtr certificate);
		static sec_protocol_metadata_access_peer_certificate_chain_handler_t static_CertificateChainForPeer = TrampolineCertificateChainForPeer;

		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_peer_certificate_chain_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineCertificateChainForPeer (IntPtr block, IntPtr certificate)
		{
			var del = BlockLiteral.GetTarget<Action<SecCertificate>> (block);
			if (del is not null) {
				var secCertificate = new SecCertificate (certificate, owns: false);
				del (secCertificate);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern bool sec_protocol_metadata_access_peer_certificate_chain (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetCertificateChainForPeerHandler (Action<SecCertificate> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineCertificateChainForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineCertificateChainForPeer));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_CertificateChainForPeer, callback);
#endif
				if (!sec_protocol_metadata_access_peer_certificate_chain (GetCheckedHandle (), &block))
					throw new InvalidOperationException ("The peer certificates are not accessible.");
			}
		}

#if !NET
		delegate void sec_protocol_metadata_access_supported_signature_algorithms_handler_t (IntPtr block, ushort signatureAlgorithm);
		static sec_protocol_metadata_access_supported_signature_algorithms_handler_t static_SignatureAlgorithmsForPeer = TrampolineSignatureAlgorithmsForPeer;

		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_supported_signature_algorithms_handler_t))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineSignatureAlgorithmsForPeer (IntPtr block, ushort signatureAlgorithm)
		{
			var del = BlockLiteral.GetTarget<Action<ushort>> (block);
			if (del is not null) {
				del (signatureAlgorithm);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe static extern byte sec_protocol_metadata_access_supported_signature_algorithms (IntPtr handle, BlockLiteral* callback);

		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetSignatureAlgorithmsForPeerHandler (Action<ushort> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, ushort, void> trampoline = &TrampolineSignatureAlgorithmsForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineSignatureAlgorithmsForPeer));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (static_SignatureAlgorithmsForPeer, callback);
#endif
				if (sec_protocol_metadata_access_supported_signature_algorithms (GetCheckedHandle (), &block) != 0)
					throw new InvalidOperationException ("The supported signature list is not accessible.");
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern /* OS_dispatch_data */ IntPtr sec_protocol_metadata_create_secret (/* OS_sec_protocol_metadata */ IntPtr metadata, /* size_t */ nuint label_len, /* const char*/ IntPtr label, /* size_t */ nuint exporter_length);

		public DispatchData? CreateSecret (string label, nuint exporterLength)
		{
			if (label is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (label));
			using var labelPtr = new TransientString (label, TransientString.Encoding.Ansi);
			return CreateDispatchData (sec_protocol_metadata_create_secret (GetCheckedHandle (), (nuint) label.Length, labelPtr, exporterLength));
		}

		[DllImport (Constants.SecurityLibrary)]
		static unsafe extern /* OS_dispatch_data */ IntPtr sec_protocol_metadata_create_secret_with_context (/* OS_sec_protocol_metadata */ IntPtr metadata, /* size_t */ nuint label_len, /* const char*/ IntPtr label, /* size_t */  nuint context_len, byte* context, /* size_t */ nuint exporter_length);

		public unsafe DispatchData? CreateSecret (string label, byte [] context, nuint exporterLength)
		{
			if (label is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (label));
			if (context is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (context));
			using var labelPtr = new TransientString (label, TransientString.Encoding.Ansi);
			fixed (byte* p = context)
				return CreateDispatchData (sec_protocol_metadata_create_secret_with_context (GetCheckedHandle (), (nuint) label.Length, labelPtr, (nuint) context.Length, p, exporterLength));
		}

		// API returning `OS_dispatch_data` can also return `null` and
		// a managed instance with (with an empty handle) is not the same
		internal static DispatchData? CreateDispatchData (IntPtr handle)
		{
			return handle == IntPtr.Zero ? null : new DispatchData (handle, owns: true);
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		static extern /* const char* */ IntPtr sec_protocol_metadata_get_server_name (IntPtr /* sec_protocol_metadata_t */ handle);

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		public string? ServerName => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_server_name (GetCheckedHandle ()));

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		unsafe static extern bool sec_protocol_metadata_access_pre_shared_keys (IntPtr /* sec_protocol_metadata_t */ handle, BlockLiteral* block);

		public delegate void SecAccessPreSharedKeysHandler (DispatchData psk, DispatchData pskIdentity);

#if !NET
		internal delegate void AccessPreSharedKeysHandler (IntPtr block, IntPtr dd_psk, IntPtr dd_psk_identity);
		static readonly AccessPreSharedKeysHandler presharedkeys = TrampolineAccessPreSharedKeys;

		[MonoPInvokeCallback (typeof (AccessPreSharedKeysHandler))]
#else
		[UnmanagedCallersOnly]
#endif
		static void TrampolineAccessPreSharedKeys (IntPtr block, IntPtr psk, IntPtr psk_identity)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData?, DispatchData?>> (block);
			if (del is not null)
				del (CreateDispatchData (psk), CreateDispatchData (psk_identity));
		}

#if NET
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (6, 0)]
		[TV (13, 0)]
		[iOS (13, 0)]
#endif
		// no [Async] as it can be called multiple times
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool AccessPreSharedKeys (SecAccessPreSharedKeysHandler handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineAccessPreSharedKeys;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecProtocolMetadata), nameof (TrampolineAccessPreSharedKeys));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (presharedkeys, handler);
#endif
				return sec_protocol_metadata_access_pre_shared_keys (GetCheckedHandle (), &block);
			}
		}
#endif
	}
}
