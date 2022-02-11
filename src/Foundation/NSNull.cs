using System;
using System.Runtime.Versioning;

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class NSNull {

		static NSNull _null;

		// helper to avoid all, but one, native calls when called repeatedly
		static public NSNull Null {
			get {
				if (_null == null)
					_null = _Null;
				return _null;
			}
		}
	}
}
