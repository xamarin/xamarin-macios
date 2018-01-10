using Foundation;

namespace NetworkExtension {

	// needed for generated Async support
	//public delegate void NEPacketTunnelFlowReadHandler (NSData [] packets, NSNumber [] protocols);

	// avoid generator default `Arg1` and `Arg2` since Action<> was used
	public class NEPacketTunnelFlowReadResult {

#if !COREBUILD
		public NEPacketTunnelFlowReadResult (NSData [] packets, NSNumber [] protocols)
		{
			Packets = packets;
			Protocols = protocols;
		}

		public NSData [] Packets { get; set; }

		public NSNumber [] Protocols { get; set; }
#endif
	}
}
