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

		public string NegotiatedProtocol => Marshal.PtrToStringAnsi (sec_protocol_metadata_get_negotiated_protocol (GetHandle ()));

		[DllImport (Constants.SecurityLibrary)]
		extern static IntPtr sec_protocol_metadata_copy_peer_public_key (IntPtr handle);

#if !COREBUILD
		public DispatchData PeerPublicKey => new DispatchData (sec_protocol_metadata_copy_peer_public_key (GetHandle ()), owns: true);
#endif
		[DllImport (Constants.SecurityLibrary)]
		extern static SslProtocol sec_protocol_metadata_get_negotiated_protocol_version (IntPtr handle);

		public SslProtocol NegotiatedProtocolVersion => sec_protocol_metadata_get_negotiated_protocol_version (GetHandle ());

		[DllImport (Constants.SecurityLibrary)]
		extern static SslCipherSuite sec_protocol_metadata_get_negotiated_ciphersuite (IntPtr handle);

		public SslCipherSuite NegotiatedCipherSuite => sec_protocol_metadata_get_negotiated_ciphersuite (GetHandle ());

		[DllImport (Constants.SecurityLibrary)]
		extern static byte sec_protocol_metadata_get_early_data_accepted (IntPtr handle);

		public bool EarlyDataAccepted => sec_protocol_metadata_get_early_data_accepted (GetHandle ()) != 0;

		//
		// MISSING: all the block APIs
		//
		
	}
}
