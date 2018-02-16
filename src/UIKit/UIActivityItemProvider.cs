#if !XAMCORE_3_0

using System;

namespace UIKit {

	public partial class UIActivityItemProvider {
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public UIActivityItemProvider ()
		{
		}
	}
}

#endif
