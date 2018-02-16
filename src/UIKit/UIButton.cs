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
using ObjCRuntime;

namespace UIKit {
	public partial class UIButton {
		
		public UIButton (UIButtonType type) : base (ObjCRuntime.Messaging.IntPtr_objc_msgSend_int (class_ptr, Selector.GetHandle ("buttonWithType:"), (int)type))
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
