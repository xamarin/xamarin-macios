#if !WATCH

using System;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;
using XamCore.CoreGraphics;

namespace XamCore.UIKit {

	public partial class UITableViewCell {
		public UITableViewCell (UITableViewCellStyle style, string reuseIdentifier) : this (style, reuseIdentifier == null ? (NSString) null : new NSString (reuseIdentifier))
		{
		}
	} /* class UITableViewCell */
}

#endif // !WATCH
