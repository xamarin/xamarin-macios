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
#endif
}

#endif
