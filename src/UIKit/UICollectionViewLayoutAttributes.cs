// 
// UICollectionViewLayoutAttributes.cs
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2013 Xamarin Inc
//

#if !WATCH

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Foundation; 
using ObjCRuntime;
using CoreGraphics;

namespace UIKit {
	public partial class UICollectionViewLayoutAttributes : NSObject {
		[CompilerGenerated]
		public static T CreateForCell<T> (NSIndexPath indexPath) where T : UICollectionViewLayoutAttributes
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (indexPath == null)
				throw new ArgumentNullException ("indexPath");
			return (T) Runtime.GetNSObject (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (T)), Selector.GetHandle ("layoutAttributesForCellWithIndexPath:"), indexPath.Handle));
		}

		[CompilerGenerated]
		public static T CreateForDecorationView<T> (NSString kind, NSIndexPath indexPath) where T: UICollectionViewLayoutAttributes
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (kind == null)
				throw new ArgumentNullException ("kind");
			if (indexPath == null)
				throw new ArgumentNullException ("indexPath");
			return (T) Runtime.GetNSObject (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (T)), Selector.GetHandle ("layoutAttributesForDecorationViewOfKind:withIndexPath:"), kind.Handle, indexPath.Handle));
		}
		
		[CompilerGenerated]
		public static T CreateForSupplementaryView<T> (NSString kind, NSIndexPath indexPath) where T: UICollectionViewLayoutAttributes
		{
			global::UIKit.UIApplication.EnsureUIThread ();
			if (kind == null)
				throw new ArgumentNullException ("kind");
			if (indexPath == null)
				throw new ArgumentNullException ("indexPath");
			return (T) Runtime.GetNSObject (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (T)), Selector.GetHandle ("layoutAttributesForSupplementaryViewOfKind:withIndexPath:"), kind.Handle, indexPath.Handle));
		}
		
		static NSString GetKindForSection (UICollectionElementKindSection section)
		{
			switch (section) {
			case UICollectionElementKindSection.Header:
				return UICollectionElementKindSectionKey.Header;
			case UICollectionElementKindSection.Footer:
				return UICollectionElementKindSectionKey.Footer;
			default:
				throw new ArgumentOutOfRangeException ("section");
			}
		}

		public static UICollectionViewLayoutAttributes CreateForSupplementaryView (UICollectionElementKindSection section, NSIndexPath indexPath)
		{
			return CreateForSupplementaryView (GetKindForSection (section), indexPath);			
		}

		public static T CreateForSupplementaryView<T> (UICollectionElementKindSection section, NSIndexPath indexPath) where T: UICollectionViewLayoutAttributes
		{
			return CreateForSupplementaryView<T> (GetKindForSection (section), indexPath);			
		}
	}
}

#endif // !WATCH
