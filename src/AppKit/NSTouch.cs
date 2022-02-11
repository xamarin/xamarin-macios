using System;
using System.Runtime.Versioning;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSTouch {
#if !NET
		[Obsolete ("This type is not meant to be user-created")]
		public NSTouch ()
		{
		}
#endif
	}
}
