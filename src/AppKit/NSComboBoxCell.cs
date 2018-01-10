using System;

using CoreGraphics;

namespace AppKit {

	public partial class NSComboBoxCell {
#if !XAMCORE_2_0
		[Obsolete]
		public NSComboBoxCell (CGRect frameRect)
		{
		}
#endif
	}
}
