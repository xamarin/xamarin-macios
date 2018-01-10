using System;
using Foundation;
using ObjCRuntime;

namespace AppKit {
	public partial class NSCollectionView {
		public void RegisterClassForItem (Type itemClass, string identifier)
		{
			_RegisterClassForItem (itemClass == null ? IntPtr.Zero : Class.GetHandle (itemClass), identifier);
		}

		public void RegisterClassForSupplementaryView (Type viewClass, NSString kind, string identifier)
		{
			_RegisterClassForSupplementaryView (viewClass == null ? IntPtr.Zero : Class.GetHandle (viewClass), kind, identifier);
		}

#if !XAMCORE_4_0
		[Mac (10, 11)]
		[Obsolete ("Use 'GetLayoutAttributes' instead.")]
		public virtual NSCollectionViewLayoutAttributes GetLayoutAttributest (string kind, NSIndexPath indexPath)
		{
			return GetLayoutAttributes (kind, indexPath);
		}
#endif
	}
}
