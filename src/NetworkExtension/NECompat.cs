#nullable enable

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace NetworkExtension {

#if !NET
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

	[Obsolete (Constants.ApiRemovedGeneral)]
	[Register ("NEFailureHandlerProvider", SkipRegistration = true)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class NEFailureHandlerProvider : NEProvider {
		public override NativeHandle ClassHandle => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);

		protected NEFailureHandlerProvider (NSObjectFlag t) : base (t) => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);
		protected internal NEFailureHandlerProvider (NativeHandle handle) : base (handle) => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);

		public unsafe virtual void HandleFailure (NSError error, Action completionHandler) => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);
		public unsafe virtual Task HandleFailureAsync (NSError error) => throw new PlatformNotSupportedException (Constants.ApiRemovedGeneral);
	}
}
