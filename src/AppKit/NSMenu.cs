using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreAnimation;

namespace AppKit {
	public partial class NSMenu {
		object __mt_items_var;
		
		NSMenuItem InsertItem (string title, string charCode, nint index)
		{
			return InsertItem (title, null, charCode, index);
		}
		
	}
}
		