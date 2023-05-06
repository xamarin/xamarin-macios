//
// UIAlertView.cs: Eventsion to UIAlertView
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2015 Xamarin Inc.
//

#if IOS

using System;
using ObjCRuntime;
using Foundation;

namespace UIKit {
	public partial class UIAlertView {
		public UIAlertView (string title, string message, IUIAlertViewDelegate del, string cancelButtonTitle, params string [] otherButtons)
			: this (title, message, del, cancelButtonTitle, otherButtons is null || otherButtons.Length == 0 ? IntPtr.Zero : new NSString (otherButtons [0]).Handle, IntPtr.Zero, IntPtr.Zero)
		{
			if (otherButtons is null)
				return;

			// first button, if present, was already added
			for (int i = 1; i < otherButtons.Length; i++)
				AddButton (otherButtons [i]);
		}

#if !NET
		[Obsolete ("Use overload with a IUIAlertViewDelegate parameter")]
		public UIAlertView (string title, string message, UIAlertViewDelegate del, string cancelButtonTitle, params string [] otherButtons)
			: this (title, message, (IUIAlertViewDelegate) del, cancelButtonTitle, otherButtons)
		{
		}
#endif
	}
}

#endif // IOS
