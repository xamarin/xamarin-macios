using System;

using XamCore.CoreGraphics;

namespace XamCore.AppKit {

	public partial class NSComboBoxCell {
#if !XAMCORE_2_0
		[Obsolete]
		public NSComboBoxCell (CGRect frameRect)
		{
		}
#endif
	}
}
