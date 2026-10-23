//
// SecProtocolMetadata.cs: Bindings the Security sec_protocol_metadata_t
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//

#nullable enable

using System.Runtime.CompilerServices;
using CoreFoundation;
using Security;
using sec_protocol_metadata_t = System.IntPtr;
using dispatch_queue_t = System.IntPtr;

namespace Security {
	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	public class SecProtocolMetadata : NativeObject {
		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public[tv
		[Preserve (Conditional = true)]
		internal SecProtocolMetadata (NativeHandle handle, bool owns) : base (handle, owns) { }

#if !COREBUILD
		[ObsoletedOSPlatform ("ios18.5")]
		[ObsoletedOSPlatform ("tvos18.5")]
		[ObsoletedOSPlatform ("maccatalyst18.5")]
		[ObsoletedOSPlatform ("macos15.5")]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_get_negotiated_protocol (IntPtr handle);

		[SupportedOSPlatform ("ios18.5")]
		[SupportedOSPlatform ("tvos18.5")]
		[SupportedOSPlatform ("maccatalyst18.5")]
		[SupportedOSPlatform ("macos15.5")]
		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_negotiated_protocol (IntPtr handle);

		/// <summary>Get the negotiated application protocol.</summary>
		/// <value>The negotiated application protocol.</value>
		public string? NegotiatedProtocol {
			get {
				if (!SystemVersion.IsAtLeastXcode16_4)
					return Marshal.PtrToStringAnsi (sec_protocol_metadata_get_negotiated_protocol (GetCheckedHandle ()));

				var rv = sec_protocol_metadata_copy_negotiated_protocol (GetCheckedHandle ());
				var str = Marshal.PtrToStringUTF8 (rv);
				unsafe {
					NativeMemory.Free ((void*) rv);
				}
				return str;
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_peer_public_key (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public DispatchData? PeerPublicKey => CreateDispatchData (sec_protocol_metadata_copy_peer_public_key (GetCheckedHandle ()));

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("macos", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[DllImport (Constants.SecurityLibrary)]
		extern static SslProtocol sec_protocol_metadata_get_negotiated_protocol_version (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("macos", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("tvos13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		[ObsoletedOSPlatform ("ios13.0", "Use 'NegotiatedTlsProtocolVersion' instead.")]
		public SslProtocol NegotiatedProtocolVersion => sec_protocol_metadata_get_negotiated_protocol_version (GetCheckedHandle ());

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsProtocolVersion sec_protocol_metadata_get_negotiated_tls_protocol_version (IntPtr handle);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public TlsProtocolVersion NegotiatedTlsProtocolVersion => sec_protocol_metadata_get_negotiated_tls_protocol_version (GetCheckedHandle ());

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.SecurityLibrary)]
		static extern TlsCipherSuite sec_protocol_metadata_get_negotiated_tls_ciphersuite (IntPtr handle);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public TlsCipherSuite NegotiatedTlsCipherSuite => sec_protocol_metadata_get_negotiated_tls_ciphersuite (GetCheckedHandle ());

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_get_early_data_accepted (IntPtr handle);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool EarlyDataAccepted => sec_protocol_metadata_get_early_data_accepted (GetCheckedHandle ()) != 0;

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_challenge_parameters_are_equal (IntPtr metadataA, IntPtr metadataB);

		/// <param name="metadataA">To be added.</param>
		///         <param name="metadataB">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static bool ChallengeParametersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			if (metadataA is null)
				return metadataB is null;
			else if (metadataB is null)
				return false; // This was tested in a native app. We do copy the behaviour.
			bool result = sec_protocol_metadata_challenge_parameters_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ()) != 0;
			GC.KeepAlive (metadataA);
			GC.KeepAlive (metadataB);
			return result;
		}

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_peers_are_equal (IntPtr metadataA, IntPtr metadataB);

		/// <param name="metadataA">To be added.</param>
		///         <param name="metadataB">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static bool PeersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			if (metadataA is null)
				return metadataB is null;
			else if (metadataB is null)
				return false; // This was tested in a native app. We do copy the behaviour.
			bool result = sec_protocol_metadata_peers_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ()) != 0;
			GC.KeepAlive (metadataA);
			GC.KeepAlive (metadataB);
			return result;
		}

		[UnmanagedCallersOnly]
		static void TrampolineDistinguishedNamesForPeer (IntPtr block, IntPtr data)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
			if (del is not null) {
				var dispatchData = new DispatchData (data, owns: false);
				del (dispatchData);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_protocol_metadata_access_distinguished_names (IntPtr handle, BlockLiteral* callback);

		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetDistinguishedNamesForPeerHandler (Action<DispatchData> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineDistinguishedNamesForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineDistinguishedNamesForPeer));
				if (sec_protocol_metadata_access_distinguished_names (GetCheckedHandle (), &block) == 0)
					throw new InvalidOperationException ("Distinguished names are not accessible.");
			}
		}

		[UnmanagedCallersOnly]
		static void TrampolineOcspReposeForPeer (IntPtr block, IntPtr data)
		{
			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
			if (del is not null) {
				var dispatchData = new DispatchData (data, owns: false);
				del (dispatchData);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_protocol_metadata_access_ocsp_response (IntPtr handle, BlockLiteral* callback);

		/// <summary>Invokes a callback with the peer's available Online Certificate Status Protocol (OCSP) response data.</summary>
		/// <param name="callback">The callback to invoke for each OCSP response. Dispose each received <see cref="DispatchData" /> when it is no longer needed.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback" /> is <see langword="null" />.</exception>
		/// <exception cref="ObjectDisposedException">This metadata instance has been disposed.</exception>
		/// <exception cref="InvalidOperationException">The peer's OCSP response data is not accessible.</exception>
		/// <remarks>
		///   <para>The callback is invoked synchronously during this call and is not registered for future responses.</para>
		///   <para>Response availability depends on the TLS handshake. The received data can be retained after the callback returns.</para>
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetOcspResponseForPeerHandler (Action<DispatchData> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineOcspReposeForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineOcspReposeForPeer));
				if (sec_protocol_metadata_access_ocsp_response (GetCheckedHandle (), &block) == 0)
					throw new InvalidOperationException ("The OCSP response is not accessible.");
			}
		}

		[UnmanagedCallersOnly]
		static void TrampolineCertificateChainForPeer (IntPtr block, IntPtr certificate)
		{
			var del = BlockLiteral.GetTarget<Action<SecCertificate>> (block);
			if (del is not null) {
				using var secCertificate = new SecCertificate2 (certificate, owns: false);
				del (secCertificate.Certificate);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_protocol_metadata_access_peer_certificate_chain (IntPtr handle, BlockLiteral* callback);

		/// <param name="callback">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetCertificateChainForPeerHandler (Action<SecCertificate> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, void> trampoline = &TrampolineCertificateChainForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineCertificateChainForPeer));
				if (sec_protocol_metadata_access_peer_certificate_chain (GetCheckedHandle (), &block) == 0)
					throw new InvalidOperationException ("The peer certificates are not accessible.");
			}
		}

		[UnmanagedCallersOnly]
		static void TrampolineSignatureAlgorithmsForPeer (IntPtr block, ushort signatureAlgorithm)
		{
			var del = BlockLiteral.GetTarget<Action<ushort>> (block);
			if (del is not null) {
				del (signatureAlgorithm);
			}
		}

		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_protocol_metadata_access_supported_signature_algorithms (IntPtr handle, BlockLiteral* callback);

		/// <summary>Invokes a callback for each signature algorithm supported by the peer.</summary>
		/// <param name="callback">The callback to invoke with each TLS signature algorithm identifier.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback" /> is <see langword="null" />.</exception>
		/// <exception cref="ObjectDisposedException">This metadata instance has been disposed.</exception>
		/// <exception cref="InvalidOperationException">The peer's supported signature algorithm list is not accessible.</exception>
		/// <remarks>
		///   <para>The callback is invoked synchronously during this call and is not registered for future updates.</para>
		///   <para>Availability depends on the TLS handshake. Clients can use this method when responding to a TLS challenge.</para>
		/// </remarks>
		[BindingImpl (BindingImplOptions.Optimizable)]
		public void SetSignatureAlgorithmsForPeerHandler (Action<ushort> callback)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
				delegate* unmanaged<IntPtr, ushort, void> trampoline = &TrampolineSignatureAlgorithmsForPeer;
				using var block = new BlockLiteral (trampoline, callback, typeof (SecProtocolMetadata), nameof (TrampolineSignatureAlgorithmsForPeer));
				if (sec_protocol_metadata_access_supported_signature_algorithms (GetCheckedHandle (), &block) == 0)
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

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios18.5")]
		[ObsoletedOSPlatform ("tvos18.5")]
		[ObsoletedOSPlatform ("maccatalyst18.5")]
		[ObsoletedOSPlatform ("macos15.5")]
		[DllImport (Constants.SecurityLibrary)]
		static extern /* const char* */ IntPtr sec_protocol_metadata_get_server_name (IntPtr /* sec_protocol_metadata_t */ handle);

		[SupportedOSPlatform ("ios18.5")]
		[SupportedOSPlatform ("tvos18.5")]
		[SupportedOSPlatform ("maccatalyst18.5")]
		[SupportedOSPlatform ("macos15.5")]
		[DllImport (Constants.SecurityLibrary)]
		static extern /* const char* */ IntPtr sec_protocol_metadata_copy_server_name (IntPtr /* sec_protocol_metadata_t */ handle);

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public string? ServerName {
			get {
				if (!SystemVersion.IsAtLeastXcode16_4)
					return Marshal.PtrToStringAnsi (sec_protocol_metadata_get_server_name (GetCheckedHandle ()));

				var rv = sec_protocol_metadata_copy_server_name (GetCheckedHandle ());
				var str = Marshal.PtrToStringUTF8 (rv);
				unsafe {
					NativeMemory.Free ((void*) rv);
				}
				return str;
			}
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.SecurityLibrary)]
		unsafe static extern byte sec_protocol_metadata_access_pre_shared_keys (IntPtr /* sec_protocol_metadata_t */ handle, BlockLiteral* block);

		public delegate void SecAccessPreSharedKeysHandler (DispatchData psk, DispatchData pskIdentity);

		[UnmanagedCallersOnly]
		static void TrampolineAccessPreSharedKeys (IntPtr block, IntPtr psk, IntPtr psk_identity)
		{
			var del = BlockLiteral.GetTarget<SecAccessPreSharedKeysHandler> (block);
			if (del is not null)
				del (new DispatchData (psk, owns: false), new DispatchData (psk_identity, owns: false));
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		// no [Async] as it can be called multiple times
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool AccessPreSharedKeys (SecAccessPreSharedKeysHandler handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			unsafe {
				delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> trampoline = &TrampolineAccessPreSharedKeys;
				using var block = new BlockLiteral (trampoline, handler, typeof (SecProtocolMetadata), nameof (TrampolineAccessPreSharedKeys));
				return sec_protocol_metadata_access_pre_shared_keys (GetCheckedHandle (), &block) != 0;
			}
		}
#endif
	}
}
