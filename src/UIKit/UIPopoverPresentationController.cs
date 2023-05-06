// Copyright 2014 Xamarin Inc. All rights reserved.

#if IOS

using System;
using CoreGraphics;
using ObjCRuntime;

namespace UIKit {

	public partial class UIPopoverPresentationController {

		// cute helper to avoid using `Class` in the public API
		public virtual Type PopoverBackgroundViewType {
			get {
				IntPtr p = PopoverBackgroundViewClass;
				if (p == IntPtr.Zero)
					return null;
				return Class.Lookup (p);
			}
			set {
				PopoverBackgroundViewClass =  (value is null) ? IntPtr.Zero : 
					Class.GetHandle (value);
			}
		}
	}

#if !XAMCORE_3_0
	public partial class UIPopoverPresentationControllerDelegate {
		[Obsolete ("Use the overload with 'ref' parameters for 'targetRect' and 'inView'.")]
		public virtual void WillRepositionPopover (UIPopoverPresentationController popoverPresentationController, CGRect targetRect, UIView inView)
		{
		}
	}

	public static partial class UIPopoverPresentationControllerDelegate_Extensions {
		[Obsolete ("Use the overload with 'ref' parameters for 'targetRect' and 'inView'.")]
		public static void WillRepositionPopover (IUIPopoverPresentationControllerDelegate This, UIPopoverPresentationController popoverPresentationController, CGRect targetRect, UIView inView)
		{
		}
	}
#endif
}

#endif // IOS
