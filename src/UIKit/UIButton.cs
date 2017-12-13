// 
// UIButton.cs: Extension method for buttons
//
// Authors:
//   Miguel de Icaza
//     
// Copyright 2012, 2015 Xamarin Inc
//

#if !WATCH

using System;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.UIKit {
	public partial class UIButton {
		
		public UIButton (UIButtonType type) : base (XamCore.ObjCRuntime.Messaging.IntPtr_objc_msgSend_int (class_ptr, Selector.GetHandle ("buttonWithType:"), (int)type))
		{
			VerifyIsUIButton ();
		}

		// do NOT change this signature without updating the linker's RemoveCode step
		// this is being removed from non-debug (release) builds
		// https://trello.com/c/Nf2B8mIM/484-remove-debug-code-in-the-linker
		private void VerifyIsUIButton ()
		{
			if (GetType () == typeof(UIButton))
				return;

			Console.WriteLine ("The UIButton subclass {0} called the (UIButtonType) constructor, but this is not allowed. Please use the default UIButton constructor from subclasses.\n{1}", GetType (), Environment.StackTrace);
		}
	}
}

#endif // !WATCH
