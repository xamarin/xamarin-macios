#if XAMCORE_2_0 || !MONOMAC

using System;
using XamCore.Foundation;

namespace XamCore.NetworkExtension {

#if !XAMCORE_4_0
	public partial class NEPacketTunnelNetworkSettings {

		[Obsolete ("This constructor does not create a valid instance of the type")]
		public NEPacketTunnelNetworkSettings () : base (NSObjectFlag.Empty)
		{
		}
	}

	public partial class NEPacketTunnelProvider {

		[Obsolete ("Use the overload accepting a INWTcpConnectionAuthenticationDelegate argument")]
		public virtual NWTcpConnection CreateTcpConnection (NWEndpoint remoteEndpoint, bool enableTls, NWTlsParameters tlsParameters, NWTcpConnectionAuthenticationDelegate @delegate)
		{
			return CreateTcpConnection (remoteEndpoint, enableTls, tlsParameters, (INWTcpConnectionAuthenticationDelegate) @delegate);
		}
	}
#endif
}

#endif
