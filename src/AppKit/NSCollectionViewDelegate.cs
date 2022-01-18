#if !XAMCORE_4_0 && !__MACCATALYST__
using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace AppKit
{
	public partial class NSCollectionViewDelegate
	{
#if NET
		[SupportedOSPlatform ("macos10.11")]
#else
		[Mac (10, 11)]
#endif
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		public virtual NSDragOperation ValidateDrop (NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}

	public partial class NSCollectionViewDelegateFlowLayout
	{
#if NET
		[SupportedOSPlatform ("macos10.11")]
#else
		[Mac (10, 11)]
#endif
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		public virtual NSDragOperation ValidateDrop (NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}

	public static partial class NSCollectionViewDelegate_Extensions
	{
#if NET
		[SupportedOSPlatform ("macos10.11")]
#else
		[Mac (10, 11)]
#endif
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		public static NSDragOperation ValidateDrop (this INSCollectionViewDelegate This, NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return This.ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}
}
#endif
