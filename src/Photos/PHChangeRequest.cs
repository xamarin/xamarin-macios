using System;
using System.Runtime.Versioning;

namespace Photos {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class PHChangeRequest {
#if !XAMCORE_3_0
		// This constructor is required for the default constructor in PHAssetChangeRequest to compile.
		internal PHChangeRequest ()
		{
		}
#endif
	}
}
