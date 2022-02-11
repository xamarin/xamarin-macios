#if !XAMCORE_3_0

using System;

#nullable enable

namespace MapKit {

#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MKDirections {
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public MKDirections ()
		{
		}
	}
}

#endif
