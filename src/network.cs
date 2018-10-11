using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {

	[TV (12,0), Mac (10,14 ,onlyOn64: true), iOS (12,0)]
	public enum NWErrorDomain {
		Invalid = 0,
		[Field ("kNWErrorDomainPOSIX")]
		Posix = 1,
		[Field ("kNWErrorDomainDNS")]
		Dns = 2,
		[Field ("kNWErrorDomainTLS")]
		Tls = 3,
	}

	[TV (12,0), Mac (10,14 ,onlyOn64: true), iOS (12,0)]
	[Static]
	[Internal]
	partial interface NWContentContextConstants {
		[Field ("_nw_content_context_default_message")]
		IntPtr _DefaultMessage { get; }
		
		[Field ("_nw_content_context_final_send")]
		IntPtr _FinalSend { get; }

		[Field ("_nw_content_context_default_stream")]
		IntPtr _DefaultStream { get; }
	}

	[TV (12,0), Mac (10,14 ,onlyOn64: true), iOS (12,0)]
	[Static]
	[Internal]
	partial interface NWConnectionConstants {

		[Field ("_nw_connection_send_idempotent_content")]
		IntPtr _SendIdempotentContent { get; }
	}

	[TV (12,0), Mac (10,14 ,onlyOn64: true), iOS (12,0)]
	[Static]
	[Internal]
	partial interface NWParametersConstants {
		[Field ("_nw_parameters_configure_protocol_default_configuration")]
		IntPtr _DefaultConfiguration { get; }

		[Field ("_nw_parameters_configure_protocol_disable")]
		IntPtr _ProtocolDisable { get; }

	}
}