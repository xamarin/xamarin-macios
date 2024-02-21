// Copyright 2011 Xamarin Inc. All rights reserved.

#if IOS

using System;
using Foundation;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {

	public partial class UIToolbar : UIView {
		
		// note: we cannot autogenerate this overload and still update the (same) __mt_Items_var local
		// previously we 'lost' the managed reference to the array and this caused bug #410
		// http://bugzilla.xamarin.com/show_bug.cgi?id=410

		[Export ("setItems:animated:")]
		public virtual void SetItems (UIBarButtonItem[] items, bool animated)
		{
			if (items is null)
				throw new ArgumentNullException ("items");
			
			// must be identical the [get|set]_Items
			var nsa_items = NSArray.FromNSObjects (items);
			
#if NET
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_NativeHandle_bool (this.Handle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated ? (byte) 1 : (byte) 0);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_NativeHandle_bool (this.SuperHandle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated ? (byte) 1 : (byte) 0);
			}
#else
			if (IsDirectBinding) {
				ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_bool (this.Handle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated ? (byte) 1 : (byte) 0);
			} else {
				ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_bool (this.SuperHandle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated ? (byte) 1 : (byte) 0);
			}
#endif
			nsa_items.Dispose ();
		}
	}
}

#endif // IOS
