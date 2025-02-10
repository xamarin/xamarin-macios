#if IOS || __MACCATALYST__

#nullable enable

using System;

using Foundation;
using ObjCRuntime;
using UIKit;

namespace MetricKit {

	public partial class MXMetaData {

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Introduced (PlatformName.iOS, 14, 0)]
#endif
		public virtual NSDictionary DictionaryRepresentation {
			get {
				if (SystemVersion.CheckiOS (14,0))
					return _DictionaryRepresentation14;
				else
					return _DictionaryRepresentation13;
			}
		}
	}
}
#endif
