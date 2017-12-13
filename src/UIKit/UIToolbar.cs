// Copyright 2011 Xamarin Inc. All rights reserved.

#if IOS

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.UIKit {

	public partial class UIToolbar : UIView {
		
		// note: we cannot autogenerate this overload and still update the (same) __mt_Items_var local
		// previously we 'lost' the managed reference to the array and this caused bug #410
		// http://bugzilla.xamarin.com/show_bug.cgi?id=410

		[Export ("setItems:animated:")]
		public virtual void SetItems (UIBarButtonItem[] items, bool animated)
		{
			if (items == null)
				throw new ArgumentNullException ("items");
			
			// must be identical the [get|set]_Items
			var nsa_items = NSArray.FromNSObjects (items);
			
			if (IsDirectBinding) {
				XamCore.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_bool (this.Handle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated);
			} else {
				XamCore.ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr_bool (this.SuperHandle, Selector.GetHandle ("setItems:animated:"), nsa_items.Handle, animated);
			}
			nsa_items.Dispose ();
		}
	}
}

#endif // IOS
