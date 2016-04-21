using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit {

	public partial class NSCollectionViewLayout {
		public void RegisterClassForDecorationView (Type itemClass, NSString elementKind)
		{
			_RegisterClassForDecorationView (itemClass == null ? IntPtr.Zero : Class.GetHandle (itemClass), elementKind);
		}
	}
}
