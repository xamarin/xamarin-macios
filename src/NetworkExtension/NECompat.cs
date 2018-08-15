#if XAMCORE_2_0 || !MONOMAC

using System;
using System.Threading.Tasks;
using Foundation;

namespace NetworkExtension {

#if !XAMCORE_4_0
	public partial class NEPacketTunnelNetworkSettings {

		[Obsolete ("This constructor does not create a valid instance of the type.")]
		public NEPacketTunnelNetworkSettings () : base (NSObjectFlag.Empty)
		{
		}
	}

	public partial class NEPacketTunnelProvider {

		[Obsolete ("Use the overload accepting a 'INWTcpConnectionAuthenticationDelegate' argument.")]
		public virtual NWTcpConnection CreateTcpConnection (NWEndpoint remoteEndpoint, bool enableTls, NWTlsParameters tlsParameters, NWTcpConnectionAuthenticationDelegate @delegate)
		{
			return CreateTcpConnection (remoteEndpoint, enableTls, tlsParameters, (INWTcpConnectionAuthenticationDelegate) @delegate);
		}
	}

	public partial class NWPath {

		[Obsolete ("This type is not meant to be user created.")]
		public NWPath ()
		{
		}
	}

	public partial class NWHostEndpoint {

		[Obsolete ("Use the 'Create' method instead.")]
		public NWHostEndpoint ()
		{
		}
	}

	public partial class NWTcpConnectionAuthenticationDelegate : NSObject {

		[Obsolete ("Use 'NWTcpConnectionAuthenticationDelegate_Extensions.EvaluateTrustAsync' instead.")]
		public unsafe virtual Task<global::Security.SecTrust> EvaluateTrustAsync (NWTcpConnection connection, NSArray peerCertificateChain)
		{
			return NWTcpConnectionAuthenticationDelegate_Extensions.EvaluateTrustAsync (this, connection, peerCertificateChain);
		}
	}
#endif
}

#endif
