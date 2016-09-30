//
// Support code for NSView
//

using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;

namespace XamCore.AppKit {
	public partial class NSView {
		object __mt_tracking_var;

#if XAMCORE_4_0
	public virtual nint AddToolTip (CGRect rect, INSToolTipOwner owner)
	{
		return _AddToolTip (rect, (NSObject)owner, IntPtr.Zero);
	}

#endif
	}
}
