#if !WATCH

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;

namespace UIKit {

	public partial class UITableViewCell {
		public UITableViewCell (UITableViewCellStyle style, string reuseIdentifier) : this (style, reuseIdentifier is null ? (NSString) null : new NSString (reuseIdentifier))
		{
		}
	} /* class UITableViewCell */
}

#endif // !WATCH
