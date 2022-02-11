using System;
using System.Runtime.Versioning;

namespace GameplayKit {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class GKGameModel {

		public const int MaxScore = (1 << 24);
		public const int MinScore = (-(1 << 24));
	}
}
