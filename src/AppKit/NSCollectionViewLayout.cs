#if !__MACCATALYST__
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos10.11")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSCollectionViewLayout {
		public void RegisterClassForDecorationView (Type itemClass, NSString elementKind)
		{
			_RegisterClassForDecorationView (itemClass == null ? IntPtr.Zero : Class.GetHandle (itemClass), elementKind);
		}

	}
}
#endif // !__MACCATALYST__
