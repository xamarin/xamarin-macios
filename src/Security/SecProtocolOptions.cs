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
	
	public class SecProtocolOptions : NativeObject {
#if !COREBUILD
		public SecProtocolOptions (IntPtr handle, bool owns) : base (handle, owns) {}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_local_identity (sec_protocol_options_t handle, sec_identity_t identity);

		public void SetLocalIdentity (SecIdentity2 identity)
		{
			if (identity == null)
				throw new ArgumentNullException (nameof (identity));
			sec_protocol_options_set_local_identity (GetHandle (), identity.GetHandle ());
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite (sec_protocol_options_t handle, SslCipherSuite cipherSuite);

		public void AddTlsCipherSuite (SslCipherSuite cipherSuite) => sec_protocol_options_add_tls_ciphersuite (GetHandle (), cipherSuite);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_ciphersuite_group (sec_protocol_options_t handle, SslCiphersuiteGroup cipherSuiteGroup);
		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void AddTlsCiphersuiteGroup (SslCiphersuiteGroup cipherSuiteGroup) => sec_protocol_options_add_tls_ciphersuite_group (GetHandle (), cipherSuiteGroup);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_min_version (sec_protocol_options_t handle, SslProtocol protocol);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		void SetTlsMinVersion (SslProtocol protocol) => sec_protocol_options_set_tls_min_version (GetHandle (), protocol);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_max_version (sec_protocol_options_t handle, SslProtocol protocol);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		void SetTlsMaxVersion (SslProtocol protocol) => sec_protocol_options_set_tls_max_version (GetHandle (), protocol);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_tls_application_protocol (sec_protocol_options_t handle, string applicationProtocol);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		void AddTlsApplicationProtocol (string applicationProtocol)
		{
			if (applicationProtocol == null)
				throw new ArgumentNullException (nameof (applicationProtocol));
			sec_protocol_options_add_tls_application_protocol (GetHandle (), applicationProtocol);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_server_name (sec_protocol_options_t handle, string serverName);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsServerName (string serverName)
		{
			if (serverName == null)
				throw new ArgumentNullException (nameof (serverName));
			sec_protocol_options_set_tls_server_name (GetHandle (), serverName);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_diffie_hellman_parameters (IntPtr handle, IntPtr dispatchDataParameter);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsDiffieHellmanParameters (DispatchData parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			sec_protocol_options_set_tls_diffie_hellman_parameters (GetHandle (), parameters.Handle);
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_pre_shared_key (IntPtr handle, IntPtr dispatchDataParameter);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void AddPreSharedKey (DispatchData parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException (nameof (parameters));
			sec_protocol_options_set_tls_diffie_hellman_parameters (GetHandle (), parameters.Handle);
		}
		

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_tickets_enabled (IntPtr handle, byte ticketsEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsTicketsEnabled (bool ticketsEnabled) => sec_protocol_options_set_tls_tickets_enabled (GetHandle (), (byte)(ticketsEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_is_fallback_attempt (IntPtr handle, byte isFallbackAttempt);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsIsFallbackAttempt (bool isFallbackAttempt) => sec_protocol_options_set_tls_is_fallback_attempt (GetHandle (), (byte)(isFallbackAttempt ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_resumption_enabled (IntPtr handle, byte resumptionEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsResumptionEnabled (bool resumptionEnabled) => sec_protocol_options_set_tls_resumption_enabled (GetHandle (), (byte)(resumptionEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_false_start_enabled (IntPtr handle, byte falseStartEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsFalseStartEnabled (bool falseStartEnabled) => sec_protocol_options_set_tls_false_start_enabled (GetHandle (), (byte)(falseStartEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_ocsp_enabled (IntPtr handle, byte ocspEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsOcspEnabled (bool ocspEnabled) => sec_protocol_options_set_tls_ocsp_enabled (GetHandle (), (byte)(ocspEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_sct_enabled (IntPtr handle, byte sctEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsSignCertificateTimestampEnabled (bool sctEnabled) => sec_protocol_options_set_tls_sct_enabled (GetHandle (), (byte)(sctEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_tls_renegotiation_enabled (IntPtr handle, byte renegotiationEnabled);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetTlsRenegotiationEnabled (bool renegotiationEnabled) => sec_protocol_options_set_tls_renegotiation_enabled (GetHandle (), (byte)(renegotiationEnabled ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_peer_authentication_required (IntPtr handle, byte peerAuthenticationRequired);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetPeerAuthenticationRequired (bool peerAuthenticationRequired) => sec_protocol_options_set_peer_authentication_required (GetHandle (), (byte)(peerAuthenticationRequired ? 1 : 0));

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_key_update_block(sec_protocol_options_t options, IntPtr key_update_block, dispatch_queue_t key_update_queue);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		public void SetKeyUpdateCallback (SecProtocolKeyUpdate keyUpdate, DispatchQueue keyUpdateQueue)
		{
			if (keyUpdate == null)
				throw new ArgumentNullException(nameof(keyUpdate));
			if (keyUpdateQueue == null)
				throw new ArgumentNullException(nameof(keyUpdateQueue));
			unsafe {
                                BlockLiteral *block_ptr_handler;
                                BlockLiteral block_handler;
                                block_handler = new BlockLiteral ();
                                block_ptr_handler = &block_handler;
                                block_handler.SetupBlockUnsafe (Trampolines.SDSecProtocolKeyUpdate.Handler, keyUpdate);

                                sec_protocol_options_set_key_update_block (handle, (IntPtr)((void*) block_ptr_handler), keyUpdateQueue.Handle);
                                block_ptr_handler->CleanupBlock ();
			}
		}

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_challenge_block(sec_protocol_options_t options, IntPtr challenge_block, dispatch_queue_t challenge_queue);
		
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_set_verify_block(sec_protocol_options_t options, IntPtr verify_block, dispatch_queue_t verify_block_queue);
#endif
	}
}
