using System;
using System.ComponentModel;

using Foundation;
using ObjCRuntime;

namespace AppKit {
	partial class NSMutableParagraphStyle {
#if !XAMCORE_4_0
		[Obsolete ("Use the 'TextBlocks' property instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public virtual void SetTextBlocks (NSTextBlock[] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			var nsa_array = NSArray.FromNSObjects (array);
			if (IsDirectBinding) {
				global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetTextBlocks_Handle, nsa_array.Handle);
			} else {
				global::ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetTextBlocks_Handle, nsa_array.Handle);
			}
			nsa_array.Dispose ();
		}

		[Obsolete ("Use the 'TextLists' property instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public virtual void SetTextLists (NSTextList[] array)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			var nsa_array = NSArray.FromNSObjects (array);
			if (IsDirectBinding) {
				global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetTextLists_Handle, nsa_array.Handle);
			} else {
				global::ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetTextLists_Handle, nsa_array.Handle);
			}
			nsa_array.Dispose ();
		}
#endif
	}
}
