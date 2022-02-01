// Compatibility stubs

#if !NET && IOS

using System;

namespace CoreSpotlight {

	partial class CSCustomAttributeKey {

		[Obsolete ("Use .ctor(string)")]
		public CSCustomAttributeKey () : this (String.Empty)
		{
		}
	}
}

#endif
