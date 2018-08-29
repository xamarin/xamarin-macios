//
// SecProtocolMetadata.cs: Bindings the Security sec_protocol_metadata_t
//
// Authors:
//   Miguel de Icaza (miguel@microsoft.com)
//
// Copyrigh 2018 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using Security;
using sec_protocol_metadata_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;

namespace Security {
	[TV (12,0), Mac (10,14), iOS (12,0)]
	public class SecProtocolMetadata : NativeObject {
		internal SecProtocolMetadata (IntPtr handle) : base (handle, false) {}

		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public[tv
		internal SecProtocolMetadata (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_get_negotiated_protocol (IntPtr handle);

		public string NegotiatedProtocol => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_negotiated_protocol (GetCheckedHandle ()));

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_peer_public_key (IntPtr handle);

#if !COREBUILD
		public DispatchData PeerPublicKey => new DispatchData (sec_protocol_metadata_copy_peer_public_key (GetCheckedHandle ()), owns: true);
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SslProtocol sec_protocol_metadata_get_negotiated_protocol_version (IntPtr handle);

		public SslProtocol NegotiatedProtocolVersion => sec_protocol_metadata_get_negotiated_protocol_version (GetCheckedHandle ());

		[DllImport (Constants.SecurityLibrary)]
		extern static SslCipherSuite sec_protocol_metadata_get_negotiated_ciphersuite (IntPtr handle);

		public SslCipherSuite NegotiatedCipherSuite => sec_protocol_metadata_get_negotiated_ciphersuite (GetCheckedHandle ());

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_get_early_data_accepted (IntPtr handle);

		public bool EarlyDataAccepted => sec_protocol_metadata_get_early_data_accepted (GetCheckedHandle ()) != 0;

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool sec_protocol_metadata_challenge_parameters_are_equal (IntPtr metadataA, IntPtr metadataB);

		public static bool ParametersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			return sec_protocol_metadata_challenge_parameters_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

 		[DllImport (Constants.SecurityLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool sec_protocol_metadata_peers_are_equal (IntPtr metadataA, IntPtr metadataB);

		public static bool PeersAreEqual (SecProtocolMetadata metadataA, SecProtocolMetadata metadataB)
		{
			return sec_protocol_metadata_peers_are_equal (metadataA.GetCheckedHandle (), metadataB.GetCheckedHandle ());
		}

#if !COREBUILD

		delegate void sec_protocol_metadata_access_distinguished_names_handler_t (IntPtr block, IntPtr dispatchData);
 		static sec_protocol_metadata_access_distinguished_names_handler_t static_DistinguishedNamesForPeer  = TrampolineDistinguishedNamesForPeer;

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
 		static extern unsafe void sec_protocol_metadata_access_distinguished_names (IntPtr handle, void *callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetDistinguishedNamesForPeerHandler (Action<DispatchData> callback)
 		{
 			unsafe {
 				if (callback == null) {
 					sec_protocol_metadata_access_distinguished_names (GetCheckedHandle (), null);
 					return;
 				}

 				BlockLiteral block_handler = new BlockLiteral ();
 				BlockLiteral *block_ptr_handler = &block_handler;
 				block_handler.SetupBlockUnsafe (static_DistinguishedNamesForPeer, callback);

 				try {
 					sec_protocol_metadata_access_distinguished_names (GetCheckedHandle (), (void*) block_ptr_handler);
 				} finally {
 					block_handler.CleanupBlock ();
 				}
 			}
 		}

		delegate void sec_protocol_metadata_access_ocsp_response_handler_t (IntPtr block, IntPtr dispatchData);
 		static sec_protocol_metadata_access_ocsp_response_handler_t static_OCSPReposeForPeer  = TrampolineOCSPReposeForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_ocsp_response_handler_t))]
 		static void TrampolineOCSPReposeForPeer (IntPtr block, IntPtr data)
 		{
 			var del = BlockLiteral.GetTarget<Action<DispatchData>> (block);
 			if (del != null) {
 				var dispatchData = new DispatchData (data, owns: false);
 				del (dispatchData);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
 		static extern unsafe void sec_protocol_metadata_access_ocsp_response (IntPtr handle, void *callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetOCSPResponseForPeerHandler (Action<DispatchData> callback)
 		{
 			unsafe {
 				if (callback == null) {
 					sec_protocol_metadata_access_ocsp_response (GetCheckedHandle (), null);
 					return;
 				}

 				BlockLiteral block_handler = new BlockLiteral ();
 				BlockLiteral *block_ptr_handler = &block_handler;
 				block_handler.SetupBlockUnsafe (static_OCSPReposeForPeer, callback);

 				try {
 					sec_protocol_metadata_access_ocsp_response (GetCheckedHandle (), (void*) block_ptr_handler);
 				} finally {
 					block_handler.CleanupBlock ();
 				}
 			}
 		}

		delegate void sec_protocol_metadata_access_peer_certificate_chain_handler_t (IntPtr block, IntPtr certificate);
 		static sec_protocol_metadata_access_peer_certificate_chain_handler_t static_CertificateChainForPeer  = TrampolineCertificateChainForPeer;

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
 		static extern unsafe void sec_protocol_metadata_access_peer_certificate_chain (IntPtr handle, void *callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetCertificateChainForPeerHandler (Action<SecCertificate> callback)
 		{
 			unsafe {
 				if (callback == null) {
 					sec_protocol_metadata_access_peer_certificate_chain (GetCheckedHandle (), null);
 					return;
 				}

 				BlockLiteral block_handler = new BlockLiteral ();
 				BlockLiteral *block_ptr_handler = &block_handler;
 				block_handler.SetupBlockUnsafe (static_CertificateChainForPeer, callback);

 				try {
 					sec_protocol_metadata_access_peer_certificate_chain (GetCheckedHandle (), (void*) block_ptr_handler);
 				} finally {
 					block_handler.CleanupBlock ();
 				}
 			}
 		}

		delegate void sec_protocol_metadata_access_supported_signature_algorithms_handler_t (IntPtr block, ushort signatureAlgorithm);
 		static sec_protocol_metadata_access_supported_signature_algorithms_handler_t static_SignatureAlgorithmsForPeer  = TrampolineSignatureAlgorithmsForPeer;

 		[MonoPInvokeCallback (typeof (sec_protocol_metadata_access_supported_signature_algorithms_handler_t))]
 		static void TrampolineSignatureAlgorithmsForPeer (IntPtr block, ushort signatureAlgorithm)
 		{
 			var del = BlockLiteral.GetTarget<Action<ushort>> (block);
 			if (del != null) {
 				del (signatureAlgorithm);
 			}
 		}

 		[DllImport (Constants.SecurityLibrary)]
 		static extern unsafe void sec_protocol_metadata_access_supported_signature_algorithms (IntPtr handle, void *callback);

 		[BindingImpl (BindingImplOptions.Optimizable)]
 		public void SetSignatureAlgorithmsForPeerHandler (Action<ushort> callback)
 		{
 			unsafe {
 				if (callback == null){
 					sec_protocol_metadata_access_supported_signature_algorithms (GetCheckedHandle (), null);
 					return;
 				}

 				BlockLiteral block_handler = new BlockLiteral ();
 				BlockLiteral *block_ptr_handler = &block_handler;
 				block_handler.SetupBlockUnsafe (static_SignatureAlgorithmsForPeer, callback);

 				try {
 					sec_protocol_metadata_access_supported_signature_algorithms (GetCheckedHandle (), (void*) block_ptr_handler);
 				} finally {
 					block_handler.CleanupBlock ();
 				}
 			}
 		}

#endif
	}
}
