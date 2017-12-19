// Copyright 2012 Xamarin Inc. All rights reserved.

#if !WATCH

using System;
using XamCore.ObjCRuntime;

namespace XamCore.UIKit {

	public partial class UIPopoverController {

		[iOS (5,0)]
		// cute helper to avoid using `Class` in the public API
		public virtual Type PopoverBackgroundViewType {
			get {
				IntPtr p = PopoverBackgroundViewClass;
				if (p == IntPtr.Zero)
					return null;
				return Class.Lookup (p);
			}
			set {
				PopoverBackgroundViewClass =  (value == null) ? IntPtr.Zero : 
					Class.GetHandle (value);
			}
		}
	}
}

#endif // !WATCH
