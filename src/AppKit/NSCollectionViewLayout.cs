#if !__MACCATALYST__
using System;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AppKit {

	public partial class NSCollectionViewLayout {
		public void RegisterClassForDecorationView (Type itemClass, NSString elementKind)
		{
			_RegisterClassForDecorationView (itemClass is null ? IntPtr.Zero : Class.GetHandle (itemClass), elementKind);
		}
	}
}
#endif // !__MACCATALYST__
