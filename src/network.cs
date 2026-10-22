using System.Runtime.CompilerServices;
using CoreFoundation;

namespace Network {

	[MacCatalyst (13, 1)]
	[NativeName ("nw_error_domain_t")]
	public enum NWErrorDomain {
		/// <summary>An invalid error domain.</summary>
		Invalid = 0,
		/// <summary>A POSIX error domain.</summary>
		[Field ("kNWErrorDomainPOSIX")]
		Posix = 1,
		/// <summary>A DNS error domain.</summary>
		[Field ("kNWErrorDomainDNS")]
		Dns = 2,
		/// <summary>A TLS error domain.</summary>
		[Field ("kNWErrorDomainTLS")]
		Tls = 3,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("kNWErrorDomainWiFiAware")]
		WiFiAware,
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	partial interface NWConnectionConstants {

		[Field ("_nw_connection_send_idempotent_content")]
		IntPtr _SendIdempotentContent { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	partial interface NWParametersConstants {
		[Field ("_nw_parameters_configure_protocol_default_configuration")]
		IntPtr _DefaultConfiguration { get; }

		[Field ("_nw_parameters_configure_protocol_disable")]
		IntPtr _ProtocolDisable { get; }

	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static]
	[Internal]
	partial interface NWPrivacyContextConstants {

		[Field ("_nw_privacy_context_default_context")]
		IntPtr _DefaultContext { get; }
	}

	[iOS (14, 2)]
	[TV (14, 2)]
	[MacCatalyst (14, 2)]
	// untyped `nw_path_unsatisfied_reason_t` enum
	[NativeName ("nw_path_unsatisfied_reason_t")]
	enum NWPathUnsatisfiedReason {
		NotAvailable = 0,
		CellularDenied = 1,
		WifiDenied = 2,
		LocalNetworkDenied = 3,
	}
}
