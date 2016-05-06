#if !WATCH
using System;
using XamCore.UIKit;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.UIKit {
	public partial class UIAdaptivePresentationControllerDelegate
	{
		// this is a workaround to allow exposing the old API ()
		[Export ("adaptivePresentationStyleForPresentationController:")]
		public virtual UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController)
		{
			throw new You_Should_Not_Call_base_In_This_Method ();
		}
	}
	
	public static partial class UIAdaptivePresentationControllerDelegate_Extensions
	{
		public static UIModalPresentationStyle GetAdaptivePresentationStyle (this IUIAdaptivePresentationControllerDelegate This, UIPresentationController forPresentationController)
		{
			UIKit.UIApplication.EnsureUIThread ();
			if (forPresentationController == null)
				throw new ArgumentNullException ("forPresentationController");
			UIModalPresentationStyle ret;
			if (IntPtr.Size == 8) {
				ret = (UIModalPresentationStyle) ObjCRuntime.Messaging.Int64_objc_msgSend_IntPtr (This.Handle, Selector.GetHandle ("adaptivePresentationStyleForPresentationController:"), forPresentationController.Handle);
			} else {
				ret = (UIModalPresentationStyle) ObjCRuntime.Messaging.int_objc_msgSend_IntPtr (This.Handle, Selector.GetHandle ("adaptivePresentationStyleForPresentationController:"), forPresentationController.Handle);
			}
			return ret;
		}
	}

}

#endif
