#if !MONOMAC

using System;
using System.Runtime.Versioning;

namespace Photos {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class PHAssetChangeRequest {

#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public PHAssetChangeRequest ()
		{
		}
#endif
	}
}

#endif
