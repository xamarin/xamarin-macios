#if !__MACCATALYST__
using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

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
#if NET
		[SupportedOSPlatform ("macos10.11")]
		[Obsolete ("Use 'GetLayoutAttributes' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#else
		[Mac (10, 11)]
		[Obsolete ("Use 'GetLayoutAttributes' instead.")]
#endif
		public virtual NSCollectionViewLayoutAttributes GetLayoutAttributest (string kind, NSIndexPath indexPath)
		{
			return GetLayoutAttributes (kind, indexPath);
		}
#endif
	}
}
#endif // !__MACCATALYST__
