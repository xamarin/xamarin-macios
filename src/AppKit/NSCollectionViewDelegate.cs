#if !XAMCORE_4_0
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit
{
	public partial class NSCollectionViewDelegate
	{
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		[Mac (10, 11)]
		public virtual NSDragOperation ValidateDrop (NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}

	public partial class NSCollectionViewDelegateFlowLayout
	{
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		[Mac (10, 11)]
		public virtual NSDragOperation ValidateDrop (NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}

	public static partial class NSCollectionViewDelegate_Extensions
	{
		[Obsolete ("Use 'ValidateDropOperation (NSCollectionView collectionView, NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation)' instead.")]
		[Mac (10, 11)]
		public static NSDragOperation ValidateDrop (this INSCollectionViewDelegate This, NSCollectionView collectionView, NSDraggingInfo draggingInfo, out NSIndexPath proposedDropIndexPath, out NSCollectionViewDropOperation proposedDropOperation)
		{
			proposedDropIndexPath = null;
			proposedDropOperation = NSCollectionViewDropOperation.On;
			return This.ValidateDropOperation (collectionView, draggingInfo, ref proposedDropIndexPath, ref proposedDropOperation);
		}
	}
}
#endif