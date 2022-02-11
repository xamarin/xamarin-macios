using System;
using System.Runtime.Versioning;

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSDateComponents {
		public static readonly nint Undefined = nint.MaxValue;
	}
}
