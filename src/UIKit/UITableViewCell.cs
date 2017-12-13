#if !WATCH

using System;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

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
