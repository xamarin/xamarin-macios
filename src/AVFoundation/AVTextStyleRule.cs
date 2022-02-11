using System;
using System.Runtime.Versioning;

namespace AVFoundation {

#if NET
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class AVTextStyleRule {
#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public AVTextStyleRule ()
		{
		}
#endif
	}
}
