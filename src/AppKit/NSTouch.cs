using System;

namespace AppKit {
	public partial class NSTouch {
#if !XAMCORE_4_0
		[Obsolete ("This type is not meant to be user-created")]
		public NSTouch ()
		{
		}
#endif
	}
}
