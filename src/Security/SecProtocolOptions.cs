//
// SecProtocolOptions.cs: Bindings the Security sec_protocol_options_t
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

using sec_protocol_options_t=System.IntPtr;
using dispatch_queue_t=System.IntPtr;
using sec_identity_t=System.IntPtr;

namespace Security {

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	public class SecProtocolOptions : NativeObject {
#if !COREBUILD
		// This type is only ever surfaced in response to callbacks in TLS/Network and documented as read-only
		// if this ever changes, make this public
		internal SecProtocolOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_local_identity (sec_protocol_options_t handle, sec_identity_t identity);

		public void SetLocalIdentity (SecIdentity2 identity)
		{
			if (identity == null)
				throw new ArgumentNullException (nameof (identity));
			sec_protocol_options_set_local_identity (GetCheckedHandle (), identity.GetCheckedHandle ());
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite (sec_protocol_options_t handle, SslCipherSuite cipherSuite);

		public void AddTlsCipherSuite (SslCipherSuite cipherSuite) => sec_protocol_options_add_tls_ciphersuite (GetCheckedHandle (), cipherSuite);

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite_group (sec_protocol_options_t handle, SslCipherSuiteGroup cipherSuiteGroup);

		public void AddTlsCipherSuiteGroup (SslCipherSuiteGroup cipherSuiteGroup) => sec_protocol_options_add_tls_ciphersuite_group (GetCheckedHandle (), cipherSuiteGroup);

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_min_version (sec_protocol_options_t handle, SslProtocol protocol);

		public void SetTlsMinVersion (SslProtocol protocol) => sec_protocol_options_set_tls_min_version (GetCheckedHandle (), protocol);

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_max_version (sec_protocol_options_t handle, SslProtocol protocol);

		public void SetTlsMaxVersion (SslProtocol protocol) => sec_protocol_options_set_tls_max_version (GetCheckedHandle (), protocol);

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_application_protocol (sec_protocol_options_t handle, string applicationProtocol);

		public void AddTlsApplicationProtocol (string applicationProtocol)
		{
			if (applicationProtocol == null)
				throw new ArgumentNullException (nameof (applicationProtocol));
			sec_protocol_options_add_tls_application_protocol (GetCheckedHandle (), applicationProtocol);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_server_name (sec_protocol_options_t handle, string serverName);

		public void SetTlsServerName (string serverName)
		{
			if (serverName == null)
				throw new ArgumentNullException (nameof (serverName));
			sec_protocol_options_set_tls_server_name (GetCheckedHandle (), serverName);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_diffie_hellman_parameters (IntPtr handle, IntPtr dispatchDataParameter);

		public void SetTlsDiffieHellmanParameters (DispatchData parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			sec_protocol_options_set_tls_diffie_hellman_parameters (GetCheckedHandle (), parameters.Handle);
		}

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_pre_shared_key (IntPtr handle, IntPtr dispatchDataParameter);

		public void AddPreSharedKey (DispatchData parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
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
			if (keyUpdate == null)
				throw new ArgumentNullException (nameof (keyUpdate));
			if (keyUpdateQueue == null)
				throw new ArgumentNullException (nameof (keyUpdateQueue));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (Trampolines.SDSecProtocolKeyUpdate.Handler, keyUpdate);

			sec_protocol_options_set_key_update_block (Handle, ref block_handler, keyUpdateQueue.Handle);
			block_handler.CleanupBlock ();
		}

#if false
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_challenge_block(sec_protocol_options_t options, IntPtr challenge_block, dispatch_queue_t challenge_queue);

		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_verify_block(sec_protocol_options_t options, IntPtr verify_block, dispatch_queue_t verify_block_queue);
#endif

#endif
	}
}
