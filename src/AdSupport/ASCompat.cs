#if !XAMCORE_4_0

using System;
using System.Runtime.Versioning;

namespace AdSupport {

	public  partial class ASIdentifierManager {

#if MONOMAC
#if NET
		[UnsupportedOSPlatform ("macos")]
#endif
		[Obsolete ("Empty stub. This member was retroactively marked as unavailable for macOS.")]
		public virtual void ClearAdvertisingIdentifier ()
		{
		}
#endif
	}
}

#endif
