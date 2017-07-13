using System;
using System.ComponentModel;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit{
#if !XAMCORE_4_0
	public partial class NSCollectionViewTransitionLayout {

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		[Obsolete ("Use the constructor that allows you to set currentLayout and newLayout.")]
		public NSCollectionViewTransitionLayout () : base (NSObjectFlag.Empty)
		{
			if (IsDirectBinding) {
				InitializeHandle (Messaging.IntPtr_objc_msgSend (this.Handle, Selector.Init), "init");
			} else {
				InitializeHandle (Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, Selector.Init), "init");
			}
		}
	}
#endif
}
