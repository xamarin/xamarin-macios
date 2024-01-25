#if !NET
using System;
using System.ComponentModel;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AppKit {
	partial class NSMutableParagraphStyle {

		[Obsolete ("Use the 'TextBlocks' property instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public virtual void SetTextBlocks (NSTextBlock [] array)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			var nsa_array = NSArray.FromNSObjects (array);
			if (IsDirectBinding) {
				global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetTextBlocks_XHandle, nsa_array.Handle);
			} else {
				global::ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetTextBlocks_XHandle, nsa_array.Handle);
			}
			nsa_array.Dispose ();
		}

		[Obsolete ("Use the 'TextLists' property instead.", false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
		public virtual void SetTextLists (NSTextList [] array)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			var nsa_array = NSArray.FromNSObjects (array);
			if (IsDirectBinding) {
				global::ObjCRuntime.Messaging.void_objc_msgSend_IntPtr (this.Handle, selSetTextLists_XHandle, nsa_array.Handle);
			} else {
				global::ObjCRuntime.Messaging.void_objc_msgSendSuper_IntPtr (this.SuperHandle, selSetTextLists_XHandle, nsa_array.Handle);
			}
			nsa_array.Dispose ();
		}
	}

	public static class NSFileTypeForHFSTypeCode {
		public static readonly string ComputerIcon = "root";
		public static readonly string DesktopIcon = "desk";
		public static readonly string FinderIcon = "FNDR";
	}
}
#endif // !NET
