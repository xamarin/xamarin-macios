#if !XAMCORE_4_0

using System;
using Metal;

namespace Vision {
	public partial class VNRequest {

		[Obsolete ("Empty stub (not a public API).")]
		public virtual IMTLDevice PreferredMetalContext { get; set; }
	}
}

#endif
