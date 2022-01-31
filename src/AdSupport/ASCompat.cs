#if !NET

using System;
using System.Runtime.Versioning;

namespace AdSupport {

	public  partial class ASIdentifierManager {

#if MONOMAC
		[Obsolete ("Empty stub. This member was retroactively marked as unavailable for macOS.")]
		public virtual void ClearAdvertisingIdentifier ()
		{
		}
#endif
	}
}

#endif
