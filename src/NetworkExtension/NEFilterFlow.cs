using System.Runtime.Versioning;

#if !MONOMAC

namespace NetworkExtension {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NEFilterFlow {

		// NEFilterFlowBytesMax define
		public const ulong MaxBytes = ulong.MaxValue;
	}
}

#endif
