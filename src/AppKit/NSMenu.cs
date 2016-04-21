using System;
using System.Runtime.InteropServices;
using XamCore.CoreFoundation;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;
using XamCore.CoreAnimation;

namespace XamCore.AppKit {
	public partial class NSMenu {
		object __mt_items_var;
		
		NSMenuItem InsertItem (string title, string charCode, nint index)
		{
			return InsertItem (title, null, charCode, index);
		}
		
	}
}
		