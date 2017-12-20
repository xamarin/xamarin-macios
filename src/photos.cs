using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.CoreLocation;
using XamCore.AVFoundation;
using XamCore.CoreGraphics;
using XamCore.CoreImage;
using XamCore.CoreMedia;
using XamCore.ImageIO;
using System;
#if !MONOMAC
using XamCore.UIKit;
using NSImage = XamCore.Foundation.NSObject; // help [NoiOS] and [NoTV]
#else
using XamCore.AppKit;
#endif

namespace XamCore.Photos
{
	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHAdjustmentData : NSCoding, NSSecureCoding {

		[Export ("initWithFormatIdentifier:formatVersion:data:")]
		IntPtr Constructor (string formatIdentifier, string formatVersion, NSData data);

		[Export ("formatIdentifier", ArgumentSemantic.Copy)]
		string FormatIdentifier { get; }

		[Export ("formatVersion", ArgumentSemantic.Copy)]
		string FormatVersion { get; }

		[Export ("data", ArgumentSemantic.Strong)]
		NSData Data { get; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
#if MONOMAC
	[DisableDefaultCtor] // Crashes mac introspection test
#endif
	[BaseType (typeof (PHObject))]
	interface PHAsset {

		[Export ("mediaType")]
		PHAssetMediaType MediaType { get; }

		[Export ("mediaSubtypes")]
		PHAssetMediaSubtype MediaSubtypes { get; }

		[Export ("pixelWidth")]
		nuint PixelWidth { get; }

		[Export ("pixelHeight")]
		nuint PixelHeight { get; }

		[Export ("creationDate", ArgumentSemantic.Strong)]
		NSDate CreationDate { get; }

		[Export ("modificationDate", ArgumentSemantic.Strong)]
		NSDate ModificationDate { get; }

		[Export ("location", ArgumentSemantic.Strong)]
		CLLocation Location { get; }

		[Export ("duration", ArgumentSemantic.Assign)]
		double Duration { get; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; }

		[NoTV][NoiOS]
		[Export ("syncFailureHidden")]
		bool SyncFailureHidden { [Bind ("isSyncFailureHidden")] get; }

		[Export ("favorite")]
		bool Favorite { [Bind ("isFavorite")] get; }

		[Export ("burstIdentifier", ArgumentSemantic.Strong)]
		string BurstIdentifier { get; }

		[Export ("burstSelectionTypes")]
		PHAssetBurstSelectionType BurstSelectionTypes { get; }

		[Export ("representsBurst")]
		bool RepresentsBurst { get; }

		[Export ("canPerformEditOperation:")]
		bool CanPerformEditOperation (PHAssetEditOperation editOperation);

		[Static]
		[Export ("fetchAssetsInAssetCollection:options:")]
		PHFetchResult FetchAssets (PHAssetCollection assetCollection, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetsWithMediaType:options:")]
		PHFetchResult FetchAssets (PHAssetMediaType mediaType, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetsWithLocalIdentifiers:options:")]
		PHFetchResult FetchAssetsUsingLocalIdentifiers (string[] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchKeyAssetsInAssetCollection:options:")]
		PHFetchResult FetchKeyAssets (PHAssetCollection assetCollection, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetsWithBurstIdentifier:options:")]
		PHFetchResult FetchAssets (string burstIdentifier, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetsWithOptions:")]
		PHFetchResult FetchAssets ([NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.TvOS, 11,0)]
		[Deprecated (PlatformName.iOS, 11,0)]
		[NoMac]
		[Static]
		[Export ("fetchAssetsWithALAssetURLs:options:")]
		PHFetchResult FetchAssets (NSUrl[] assetUrls, [NullAllowed] PHFetchOptions options);

		[iOS (9,0)]
		[Export ("sourceType", ArgumentSemantic.Assign)]
		PHAssetSourceType SourceType { get; }

		[TV (11,0), iOS (11,0), NoMac]
		[Export ("playbackStyle", ArgumentSemantic.Assign)]
		PHAssetPlaybackStyle PlaybackStyle { get; }

		[NoTV][NoiOS]
		[Field ("PHLocalIdentifierNotFound")]
		NSString LocalIdentifierNotFound { get; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[PHAssetChangeRequest init]: unrecognized selector sent to instance 0x8165d150
	[BaseType (typeof (NSObject))]
	interface PHAssetChangeRequest {

#if !MONOMAC
		[Static]
		[Export ("creationRequestForAssetFromImage:")]
		PHAssetChangeRequest FromImage (UIImage image);
#endif

		[Static]
		[Export ("creationRequestForAssetFromImageAtFileURL:")]
		PHAssetChangeRequest FromImage (NSUrl fileUrl);

		[Static]
		[Export ("creationRequestForAssetFromVideoAtFileURL:")]
		PHAssetChangeRequest FromVideo (NSUrl fileUrl);

		[Export ("placeholderForCreatedAsset", ArgumentSemantic.Strong)]
		PHObjectPlaceholder PlaceholderForCreatedAsset { get; }

		[Static]
		[Export ("deleteAssets:")]
		void DeleteAssets (PHAsset[] assets);

		[Static]
		[Export ("changeRequestForAsset:")]
		PHAssetChangeRequest ChangeRequest (PHAsset asset);

		[Export ("creationDate", ArgumentSemantic.Strong)]
		NSDate CreationDate { get; set; }

		[Export ("location", ArgumentSemantic.Strong)]
		CLLocation Location { get; set; }

		[Export ("favorite", ArgumentSemantic.Assign)]
		bool Favorite { [Bind ("isFavorite")] get; set; }

		[Export ("hidden", ArgumentSemantic.Assign)]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NullAllowed] // by default this property is null
		[Export ("contentEditingOutput", ArgumentSemantic.Strong)]
		PHContentEditingOutput ContentEditingOutput { get; set; }

		[Export ("revertAssetContentToOriginal")]
		void RevertAssetContentToOriginal ();

	}

	[iOS (9,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof(PHAssetChangeRequest))]
	[DisableDefaultCtor]
	interface PHAssetCreationRequest
	{
		[Static]
		[Export ("creationRequestForAsset")]
		PHAssetCreationRequest CreationRequestForAsset ();

		// +(BOOL)supportsAssetResourceTypes:(NSArray<NSNumber * __nonnull> * __nonnull)types;
		[Static]
		[Internal, Export ("supportsAssetResourceTypes:")]
		bool _SupportsAssetResourceTypes (NSNumber[] types);

		[Export ("addResourceWithType:fileURL:options:")]
		void AddResource (PHAssetResourceType type, NSUrl fileURL, [NullAllowed] PHAssetResourceCreationOptions options);

		[Export ("addResourceWithType:data:options:")]
		void AddResource (PHAssetResourceType type, NSData data, [NullAllowed] PHAssetResourceCreationOptions options);
	}

	[NoMac]
	delegate void PHProgressHandler (double progress, ref bool stop);

	[iOS (9,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // crashes: -[PHAssetResource init]: unrecognized selector sent to instance 0x7f9e15884e90
	interface PHAssetResource
	{
		[Export ("type", ArgumentSemantic.Assign)]
		PHAssetResourceType ResourceType { get; }

		[Export ("assetLocalIdentifier")]
		string AssetLocalIdentifier { get; }

		[Export ("uniformTypeIdentifier")]
		string UniformTypeIdentifier { get; }

		[Export ("originalFilename")]
		string OriginalFilename { get; }

		[Static]
		[Export ("assetResourcesForAsset:")]
		PHAssetResource[] GetAssetResources (PHAsset forAsset);

		[iOS (9,1)]
		[Static]
		[Export ("assetResourcesForLivePhoto:")]
		PHAssetResource[] GetAssetResources (PHLivePhoto livePhoto);
	}

	[iOS (9,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof(NSObject))]
	interface PHAssetResourceCreationOptions : NSCopying
	{
		[NullAllowed, Export ("originalFilename")]
		string OriginalFilename { get; set; }

		[NullAllowed, Export ("uniformTypeIdentifier")]
		string UniformTypeIdentifier { get; set; }

		[Export ("shouldMoveFile")]
		bool ShouldMoveFile { get; set; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (NSObject))]
	interface PHContentEditingInputRequestOptions {

		[Export ("canHandleAdjustmentData", ArgumentSemantic.Copy)]
		Func<PHAdjustmentData, bool> CanHandleAdjustmentData { get; set; }

		[Export ("networkAccessAllowed", ArgumentSemantic.Assign)]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		PHProgressHandler ProgressHandler { get; set; }

		[Field ("PHContentEditingInputResultIsInCloudKey")]
		NSString ResultIsInCloudKey { get; }

		[Field ("PHContentEditingInputCancelledKey")]
		NSString CancelledKey { get; }

		[Field ("PHContentEditingInputErrorKey")]
		NSString InputErrorKey { get; }
	}

	delegate void PHContentEditingHandler (PHContentEditingInput contentEditingInput, NSDictionary requestStatusInfo);

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[Category]
	[BaseType (typeof (PHAsset))]
	interface PHAssetContentEditingInputExtensions {

		[Export ("requestContentEditingInputWithOptions:completionHandler:")]
		nuint RequestContentEditingInput ([NullAllowed] PHContentEditingInputRequestOptions options, PHContentEditingHandler completionHandler);

		[Export ("cancelContentEditingInputRequest:")]
		void CancelContentEditingInputRequest (nuint requestID);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // fails when calling ToString (see below) and there are (static) API to create them
	// NSInternalInconsistencyException Reason: This method can only be called from inside of -[PHPhotoLibrary performChanges:] or -[PHPhotoLibrary performChangeAndWait:]
	interface PHAssetCollectionChangeRequest {

		[Static]
		[Export ("creationRequestForAssetCollectionWithTitle:")]
		PHAssetCollectionChangeRequest CreateAssetCollection (string title);

		[Export ("placeholderForCreatedAssetCollection", ArgumentSemantic.Strong)]
		PHObjectPlaceholder PlaceholderForCreatedAssetCollection { get; }

		[Static]
		[Export ("deleteAssetCollections:")]
		void DeleteAssetCollections (PHAssetCollection[] assetCollections);

		[Static]
		[Export ("changeRequestForAssetCollection:")]
		PHAssetCollectionChangeRequest ChangeRequest (PHAssetCollection assetCollection);

		[Static]
		[Export ("changeRequestForAssetCollection:assets:")]
		PHAssetCollectionChangeRequest ChangeRequest (PHAssetCollection assetCollection, PHFetchResult assets);

		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set; }

		[Export ("addAssets:")]
		void AddAssets (PHObject [] assets);

		[Export ("insertAssets:atIndexes:")]
		void InsertAssets (PHObject [] assets, NSIndexSet indexes);

		[Export ("removeAssets:")]
		void RemoveAssets (PHObject[] assets);

		[Export ("removeAssetsAtIndexes:")]
		void RemoveAssets (NSIndexSet indexes);

		[Export ("replaceAssetsAtIndexes:withAssets:")]
		void ReplaceAssets (NSIndexSet indexes, PHObject[] assets);

		[Export ("moveAssetsAtIndexes:toIndex:")]
		void MoveAssets (NSIndexSet fromIndexes, nuint toIndex);
	}

	[iOS (9,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface PHAssetResourceManager
	{
		[Static]
		[Export ("defaultManager")]
		PHAssetResourceManager DefaultManager { get; }

		[Export ("requestDataForAssetResource:options:dataReceivedHandler:completionHandler:")]
		int RequestData (PHAssetResource forResource, [NullAllowed] PHAssetResourceRequestOptions options, Action<NSData> handler, Action<NSError> completionHandler);

		[Export ("writeDataForAssetResource:toFile:options:completionHandler:")]
		[Async]
		void WriteData (PHAssetResource forResource, NSUrl fileURL, [NullAllowed] PHAssetResourceRequestOptions options, Action<NSError> completionHandler);

		[Export ("cancelDataRequest:")]
		void CancelDataRequest (int requestID);
	}

	[iOS (9,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof(NSObject))]
	interface PHAssetResourceRequestOptions : NSCopying
	{
		[Export ("networkAccessAllowed")]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		Action<double> ProgressHandler { get; set; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHChange {

		[Export ("changeDetailsForObject:")]
		PHObjectChangeDetails GetObjectChangeDetails ([NullAllowed] PHObject obj);

		[Export ("changeDetailsForFetchResult:")]
		PHFetchResultChangeDetails GetFetchResultChangeDetails (PHFetchResult obj);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHObjectChangeDetails {

		[Export ("objectBeforeChanges", ArgumentSemantic.Strong)]
		NSObject ObjectBeforeChanges { get; }

		[Export ("objectAfterChanges", ArgumentSemantic.Strong)]
		NSObject ObjectAfterChanges { get; }

		[Export ("assetContentChanged")]
		bool AssetContentChanged { get; }

		[Export ("objectWasDeleted")]
		bool ObjectWasDeleted { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	delegate void PHChangeDetailEnumerator (nuint fromIndex, nuint toIndex);

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHFetchResultChangeDetails {

		[Export ("fetchResultBeforeChanges", ArgumentSemantic.Strong)]
		PHFetchResult FetchResultBeforeChanges { get; }

		[Export ("fetchResultAfterChanges", ArgumentSemantic.Strong)]
		PHFetchResult FetchResultAfterChanges { get; }

		[Export ("hasIncrementalChanges", ArgumentSemantic.Assign)]
		bool HasIncrementalChanges { get; }

		[Export ("removedIndexes", ArgumentSemantic.Strong)]
		NSIndexSet RemovedIndexes { get; }

		[Export ("removedObjects", ArgumentSemantic.Strong)]
		PHObject[] RemovedObjects { get; }

		[Export ("insertedIndexes", ArgumentSemantic.Strong)]
		NSIndexSet InsertedIndexes { get; }

		[Export ("insertedObjects", ArgumentSemantic.Strong)]
		PHObject[] InsertedObjects { get; }

		[Export ("changedIndexes", ArgumentSemantic.Strong)]
		NSIndexSet ChangedIndexes { get; }

		[Export ("changedObjects", ArgumentSemantic.Strong)]
		PHObject[] ChangedObjects { get; }

		[Export ("enumerateMovesWithBlock:")]
		void EnumerateMoves (PHChangeDetailEnumerator handler);

		[Export ("hasMoves", ArgumentSemantic.Assign)]
		bool HasMoves { get; }

		[Static]
		[Export ("changeDetailsFromFetchResult:toFetchResult:changedObjects:")]
		PHFetchResultChangeDetails ChangeDetails (PHFetchResult fromResult, PHFetchResult toResult, PHObject[] changedObjects);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (PHObject))]
	[DisableDefaultCtor] // not user createable (calling description fails, see below) must be fetched by API
	// NSInternalInconsistencyException Reason: PHCollection has no identifier
#if TVOS || XAMCORE_4_0
	[Abstract] // Acording to docs: The abstract superclass for Photos asset collections and collection lists.
#endif
	interface PHCollection {

		[Export ("canContainAssets", ArgumentSemantic.Assign)]
		bool CanContainAssets { get; }

		[Export ("canContainCollections", ArgumentSemantic.Assign)]
		bool CanContainCollections { get; }

		[Export ("localizedTitle", ArgumentSemantic.Strong)]
		string LocalizedTitle { get; }

		[Export ("canPerformEditOperation:")]
		bool CanPerformEditOperation (PHCollectionEditOperation anOperation);

		[Static]
		[Export ("fetchCollectionsInCollectionList:options:")]
		PHFetchResult FetchCollections (PHCollectionList collectionList, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchTopLevelUserCollectionsWithOptions:")]
		PHFetchResult FetchTopLevelUserCollections ([NullAllowed] PHFetchOptions options);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (PHCollection))]
	interface PHAssetCollection {

		[Export ("assetCollectionType")]
		PHAssetCollectionType AssetCollectionType { get; }

		[Export ("assetCollectionSubtype")]
		PHAssetCollectionSubtype AssetCollectionSubtype { get; }

		[Export ("estimatedAssetCount")]
		nuint EstimatedAssetCount { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Export ("approximateLocation", ArgumentSemantic.Strong)]
		CLLocation ApproximateLocation { get; }

		[Export ("localizedLocationNames", ArgumentSemantic.Strong)]
		string[] LocalizedLocationNames { get; }

		[Static]
		[Export ("fetchAssetCollectionsWithLocalIdentifiers:options:")]
		PHFetchResult FetchAssetCollections (string[] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetCollectionsWithType:subtype:options:")]
		PHFetchResult FetchAssetCollections (PHAssetCollectionType type, PHAssetCollectionSubtype subtype, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetCollectionsContainingAsset:withType:options:")]
		PHFetchResult FetchAssetCollections (PHAsset asset, PHAssetCollectionType type, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetCollectionsWithALAssetGroupURLs:options:")]
		PHFetchResult FetchAssetCollections (NSUrl[] assetGroupUrls, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchMomentsInMomentList:options:")]
		PHFetchResult FetchMoments (PHCollectionList momentList, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchMomentsWithOptions:")]
		PHFetchResult FetchMoments ([NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("transientAssetCollectionWithAssets:title:")]
		PHAssetCollection GetTransientAssetCollection (PHAsset[] assets, string title);

		[Static]
		[Export ("transientAssetCollectionWithAssetFetchResult:title:")]
		PHAssetCollection GetTransientAssetCollection (PHFetchResult fetchResult, string title);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (PHCollection))]
	interface PHCollectionList {

		[Export ("collectionListType")]
		PHCollectionListType CollectionListType { get; }

		[Export ("collectionListSubtype")]
		PHCollectionListSubtype CollectionListSubtype { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; }

		[Export ("localizedLocationNames", ArgumentSemantic.Strong)]
		string[] LocalizedLocationNames { get; }

		[Static]
		[Export ("fetchCollectionListsContainingCollection:options:")]
		PHFetchResult FetchCollectionLists (PHCollection collection, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchCollectionListsWithLocalIdentifiers:options:")]
		PHFetchResult FetchCollectionLists (string[] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchCollectionListsWithType:subtype:options:")]
		PHFetchResult FetchCollectionLists (PHCollectionListType type, PHCollectionListSubtype subType, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchMomentListsWithSubtype:containingMoment:options:")]
		PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, PHAssetCollection moment, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchMomentListsWithSubtype:options:")]
		PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("transientCollectionListWithCollections:title:")]
		PHCollectionList CreateTransientCollectionList (PHAssetCollection[] collections, string title);

		[Static]
		[Export ("transientCollectionListWithCollectionsFetchResult:title:")]
		PHCollectionList CreateTransientCollectionList (PHFetchResult fetchResult, string title);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // sometimes crash when calling 'description'
	// This method can only be called from inside of -[PHPhotoLibrary performChanges:] or -[PHPhotoLibrary performChangeAndWait:]
	// as it ties to get 'title' which was never set (e.g. using FromCreationRequest)
	interface PHCollectionListChangeRequest {

		[Static]
		[Export ("creationRequestForCollectionListWithTitle:")]
		PHCollectionListChangeRequest CreateAssetCollection (string title);

		[Export ("placeholderForCreatedCollectionList", ArgumentSemantic.Strong)]
		PHObjectPlaceholder PlaceholderForCreatedCollectionList { get; }

		[Static]
		[Export ("deleteCollectionLists:")]
		void DeleteCollectionLists (PHCollectionList[] collectionLists);

		[Static]
		[Export ("changeRequestForCollectionList:")]
		PHCollectionListChangeRequest ChangeRequest (PHCollectionList collectionList);

		[Static]
		[Export ("changeRequestForCollectionList:childCollections:")]
		PHCollectionListChangeRequest ChangeRequest (PHCollectionList collectionList, PHFetchResult childCollections);

		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set; }

		[Export ("addChildCollections:")]
		void AddChildCollections (PHCollection[] collections);

		[Export ("insertChildCollections:atIndexes:")]
		void InsertChildCollections (PHCollection[] collections, NSIndexSet indexes);

		[Export ("removeChildCollections:")]
		void RemoveChildCollections (PHCollection[] collections);

		[Export ("removeChildCollectionsAtIndexes:")]
		void RemoveChildCollections (NSIndexSet indexes);

		[Export ("replaceChildCollectionsAtIndexes:withChildCollections:")]
		void ReplaceChildCollection (NSIndexSet indexes, PHCollection[] collections);

		[Export ("moveChildCollectionsAtIndexes:toIndex:")]
		void MoveChildCollections (NSIndexSet indexes, nuint toIndex);
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHContentEditingInput {

		[Export ("mediaType")]
		PHAssetMediaType MediaType { get; }

		[Export ("mediaSubtypes")]
		PHAssetMediaSubtype MediaSubtypes { get; }

		[Export ("creationDate", ArgumentSemantic.Copy)]
		NSDate CreationDate { get; }

		[Export ("location", ArgumentSemantic.Copy)]
		CLLocation Location { get; }

		[Export ("uniformTypeIdentifier")]
		string UniformTypeIdentifier { get; }

		[NullAllowed]
		[Export ("adjustmentData", ArgumentSemantic.Strong)]
		PHAdjustmentData AdjustmentData { get; }

		[Export ("displaySizeImage", ArgumentSemantic.Strong)]
#if MONOMAC
		NSImage DisplaySizeImage { get; }
#else
		UIImage DisplaySizeImage { get; }
#endif

		[Export ("fullSizeImageURL", ArgumentSemantic.Copy)]
		NSUrl FullSizeImageUrl { get; }

		[Export ("fullSizeImageOrientation")]
		XamCore.CoreImage.CIImageOrientation FullSizeImageOrientation { get; }

		[Export ("avAsset", ArgumentSemantic.Strong)]
		[Availability (Deprecated = Platform.iOS_9_0, Message="Use 'AudiovisualAsset' property instead.")]
		AVAsset AvAsset { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("audiovisualAsset", ArgumentSemantic.Strong)]
		AVAsset AudiovisualAsset { get; }

		[iOS (10,0)]
		[NullAllowed, Export ("livePhoto", ArgumentSemantic.Strong)]
		PHLivePhoto LivePhoto { get; }

		[TV (11,0), iOS (11,0)]
		[Mac (10,13, onlyOn64 : true)]
		[Export ("playbackStyle", ArgumentSemantic.Assign)]
		PHAssetPlaybackStyle PlaybackStyle { get; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHContentEditingOutput : NSCoding, NSSecureCoding {

		[Export ("initWithContentEditingInput:")]
		IntPtr Constructor (PHContentEditingInput contentEditingInput);

		[NoMac]
		[Export ("initWithPlaceholderForCreatedAsset:")]
		IntPtr Constructor (PHObjectPlaceholder placeholderForCreatedAsset);

		[NullAllowed] // by default this property is null
		[Export ("adjustmentData", ArgumentSemantic.Strong)]
		PHAdjustmentData AdjustmentData { get; set; }

		[Export ("renderedContentURL", ArgumentSemantic.Copy)]
		NSUrl RenderedContentUrl { get; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHFetchOptions : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("predicate", ArgumentSemantic.Strong)]
		NSPredicate Predicate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sortDescriptors", ArgumentSemantic.Strong)]
		NSSortDescriptor[] SortDescriptors { get; set; }

		[Export ("includeHiddenAssets")]
		bool IncludeHiddenAssets { get; set; }

		[Export ("includeAllBurstAssets", ArgumentSemantic.Assign)]
		bool IncludeAllBurstAssets { get; set; }

		[Export ("wantsIncrementalChangeDetails", ArgumentSemantic.Assign)]
		bool WantsIncrementalChangeDetails { get; set; }

		[iOS (9,0)]
		[Export ("includeAssetSourceTypes", ArgumentSemantic.Assign)]
		PHAssetSourceType IncludeAssetSourceTypes { get; set; }

		[iOS (9,0)]
		[Export ("fetchLimit", ArgumentSemantic.Assign)]
		nuint FetchLimit { get; set; }
	}

	[Mac (10,13, onlyOn64 : true)]
	delegate void PHFetchResultEnumerator (NSObject element, nuint elementIndex, out bool stop);

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash when calling 'description' and seems to be only returned from iOS (not user created)
	interface PHFetchResult : NSCopying {

		[Export ("count")]
		nint Count { get; }

		[Export ("objectAtIndex:")]
		NSObject ObjectAt (nint index);

		[Internal, Export ("objectAtIndexedSubscript:")]
		NSObject _ObjectAtIndexedSubscript (nint index);

		[Export ("containsObject:")]
		bool Contains (NSObject id);

		[Export ("indexOfObject:")]
		nint IndexOf (NSObject id);

		[Export ("indexOfObject:inRange:")]
		nint IndexOf (NSObject id, NSRange range);

		[Export ("firstObject")]
		NSObject firstObject { get; }

		[Export ("lastObject")]
		NSObject LastObject { get; }

		[Internal, Export ("objectsAtIndexes:")]
		IntPtr _ObjectsAt (NSIndexSet indexes);

		[Export ("enumerateObjectsUsingBlock:")]
		void Enumerate (PHFetchResultEnumerator handler);

		[Export ("enumerateObjectsWithOptions:usingBlock:")]
		void Enumerate (NSEnumerationOptions opts, PHFetchResultEnumerator handler);

		[Export ("enumerateObjectsAtIndexes:options:usingBlock:")]
		void Enumerate (NSIndexSet idx, NSEnumerationOptions opts, PHFetchResultEnumerator handler);

		[Export ("countOfAssetsWithMediaType:")]
		nuint CountOfAssetsWithMediaType (PHAssetMediaType mediaType);
	}

	[NoMac]
	delegate void PHAssetImageProgressHandler (double progress, NSError error, out bool stop, NSDictionary info);

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHImageRequestOptions : NSCopying {

		[Export ("version", ArgumentSemantic.Assign)]
		PHImageRequestOptionsVersion Version { get; set; }

		[Export ("deliveryMode", ArgumentSemantic.Assign)]
		PHImageRequestOptionsDeliveryMode DeliveryMode { get; set; }

		[Export ("resizeMode", ArgumentSemantic.Assign)]
		PHImageRequestOptionsResizeMode ResizeMode { get; set; }

		[Export ("normalizedCropRect", ArgumentSemantic.Assign)]
		CGRect NormalizedCropRect { get; set; }

		[Export ("networkAccessAllowed", ArgumentSemantic.Assign)]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[Export ("synchronous", ArgumentSemantic.Assign)]
		bool Synchronous { [Bind ("isSynchronous")] get; set; }

		[Export ("progressHandler", ArgumentSemantic.Copy)] [NullAllowed]
		PHAssetImageProgressHandler ProgressHandler { get; set; }
	}

	[NoMac]
	delegate void PHAssetVideoProgressHandler (double progress, NSError error, out bool stop, NSDictionary info);

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (NSObject))]
	interface PHVideoRequestOptions {

		[Export ("networkAccessAllowed", ArgumentSemantic.Assign)]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[Export ("version", ArgumentSemantic.Assign)]
		PHVideoRequestOptionsVersion Version { get; set; }

		[Export ("deliveryMode", ArgumentSemantic.Assign)]
		PHVideoRequestOptionsDeliveryMode DeliveryMode { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("progressHandler", ArgumentSemantic.Copy)]
		PHAssetVideoProgressHandler ProgressHandler { get; set; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[Static]
	interface PHImageKeys {

		[Field ("PHImageResultIsInCloudKey")]
		NSString ResultIsInCloud { get; }

		[Field ("PHImageResultIsDegradedKey")]
		NSString ResultIsDegraded { get; }

		[Field ("PHImageCancelledKey")]
		NSString Cancelled { get; }

		[Field ("PHImageErrorKey")]
		NSString Error { get; }

		[Field ("PHImageResultRequestIDKey")]
		NSString ResultRequestID { get; }
	}

#if MONOMAC
	delegate void PHImageResultHandler (NSImage result, NSDictionary info);
#else
	delegate void PHImageResultHandler (UIImage result, NSDictionary info);
#endif

	delegate void PHImageManagerRequestPlayerHandler (AVPlayerItem playerItem, NSDictionary info);
	delegate void PHImageManagerRequestExportHandler (AVAssetExportSession exportSession, NSDictionary info);
#if XAMCORE_4_0
	delegate void PHImageManagerRequestAVAssetHandler (AVAsset asset, AVAudioMix audioMix, NSDictionary info);
#else
	delegate void PHImageManagerRequestAvAssetHandler (AVAsset asset, AVAudioMix audioMix, NSDictionary info);
#endif
	delegate void PHImageManagerRequestLivePhoto (PHLivePhoto livePhoto, NSDictionary info);

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	interface PHImageManager {

		[Static]
		[Export ("defaultManager")]
		PHImageManager DefaultManager { get; }

		[Export ("requestImageForAsset:targetSize:contentMode:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestImageForAsset (PHAsset asset, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHImageRequestOptions options, PHImageResultHandler resultHandler);

		[Export ("cancelImageRequest:")]
		void CancelImageRequest (int /* PHImageRequestID = int32_t */ requestID);

		[Export ("requestImageDataForAsset:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestImageData (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageDataHandler handler);

		[NoMac]
		[Export ("requestPlayerItemForVideo:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestPlayerItem (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestPlayerHandler resultHandler);

		[NoMac]
		[Export ("requestExportSessionForVideo:options:exportPreset:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestExportSession (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, string exportPreset, PHImageManagerRequestExportHandler resultHandler);

		[NoMac]
		[Export ("requestAVAssetForVideo:options:resultHandler:")]
#if XAMCORE_4_0
		int /* PHImageRequestID = int32_t */ RequestAVAsset (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestAVAssetHandler resultHandler);
#else
		int /* PHImageRequestID = int32_t */ RequestAvAsset (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestAvAssetHandler resultHandler);
#endif

		[Field ("PHImageManagerMaximumSize")]
		CGSize MaximumSize { get; }

		[iOS (9,1)]
		[NoMac]
		[Export ("requestLivePhotoForAsset:targetSize:contentMode:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestLivePhoto (PHAsset asset, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHLivePhotoRequestOptions options, PHImageManagerRequestLivePhoto resultHandler);
	}

#if MONOMAC
	delegate void PHImageDataHandler (NSData data, NSString dataUti, CGImagePropertyOrientation orientation, NSDictionary info);
#else
	delegate void PHImageDataHandler (NSData data, NSString dataUti, UIImageOrientation orientation, NSDictionary info);
#endif

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (PHImageManager))]
	interface PHCachingImageManager {

		[Export ("allowsCachingHighQualityImages", ArgumentSemantic.Assign)]
		bool AllowsCachingHighQualityImages { get; set; }

		[Export ("startCachingImagesForAssets:targetSize:contentMode:options:")]
		void StartCaching (PHAsset [] assets, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHImageRequestOptions options);

		[Export ("stopCachingImagesForAssets:targetSize:contentMode:options:")]
		void StopCaching (PHAsset [] assets, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHImageRequestOptions options);

		[Export ("stopCachingImagesForAllAssets")]
		void StopCaching ();
	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // doc -> "abstract base class"
	// throws "NSInternalInconsistencyException Reason: PHObject has no identifier"
#if TVOS || XAMCORE_4_0
	[Abstract] // Acording to docs: The abstract base class for Photos model objects (assets and collections).
#endif
	interface PHObject : NSCopying {

		[Export ("localIdentifier", ArgumentSemantic.Copy)]
		string LocalIdentifier { get; }
	}

	[iOS (8,0)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (PHObject))]
	interface PHObjectPlaceholder {

	}

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface PHPhotoLibraryChangeObserver {

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("photoLibraryDidChange:")]
		void PhotoLibraryDidChange (PHChange changeInstance);
	}

	delegate void PHPhotoLibraryCancellableChangeHandler (out bool cancel);

	[iOS (8,0)]
	[TV (10,0)]
	[Mac (10,13, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: -[PHPhotoLibrary init] unsupported
	interface PHPhotoLibrary {

		[Static]
		[Export ("sharedPhotoLibrary")]
		PHPhotoLibrary SharedPhotoLibrary { get; }

		[Static, Export ("authorizationStatus")]
		PHAuthorizationStatus AuthorizationStatus { get; }

		[Static, Export ("requestAuthorization:")]
		[Async]
		void RequestAuthorization (Action<PHAuthorizationStatus> handler);

		// no [Async] since we're binding performChangesAndWait:error: too
		[Export ("performChanges:completionHandler:")]
		void PerformChanges (Action changeHandler, Action<bool, NSError> completionHandler);

		[Export ("performChangesAndWait:error:")]
		bool PerformChangesAndWait (Action changeHandler, out NSError error);

		[Export ("registerChangeObserver:")]
		void RegisterChangeObserver ([Protocolize] PHPhotoLibraryChangeObserver observer);

		[Export ("unregisterChangeObserver:")]
		void UnregisterChangeObserver ([Protocolize] PHPhotoLibraryChangeObserver observer);
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoTV][NoiOS]
	[Category]
	[BaseType (typeof (PHPhotoLibrary))]
	interface PHPhotoLibrary_CloudIdentifiers {

		[Export ("localIdentifiersForCloudIdentifiers:")]
		string[] GetLocalIdentifiers (PHCloudIdentifier[] cloudIdentifiers);

		[Export ("cloudIdentifiersForLocalIdentifiers:")]
		PHCloudIdentifier[] GetCloudIdentifiers (string[] localIdentifiers);
	}

	[iOS (9,1)]
	[TV (10,0)]
#if MONOMAC
	[DisableDefaultCtor] // NS_UNAVAILABLE
#endif
	[Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof(NSObject))]
	interface PHLivePhoto
#if !MONOMAC
	: NSSecureCoding, NSCopying
#endif
	{
		[Export ("size")]
		CGSize Size { get; }

#if !MONOMAC
		[Static]
		[Export ("requestLivePhotoWithResourceFileURLs:placeholderImage:targetSize:contentMode:resultHandler:")]
		int RequestLivePhoto (NSUrl[] fileUrls, [NullAllowed] UIImage image, CGSize targetSize, PHImageContentMode contentMode, Action<PHLivePhoto, NSDictionary> resultHandler);

		[Static]
		[Export ("cancelLivePhotoRequestWithRequestID:")]
		void CancelLivePhotoRequest (int requestID);
#endif
	}

	[iOS (9,1)]
	[TV (10,0)]
	[NoMac]
	[BaseType (typeof (NSObject))]
	interface PHLivePhotoRequestOptions : NSCopying	{
		[Export ("deliveryMode", ArgumentSemantic.Assign)]
		PHImageRequestOptionsDeliveryMode DeliveryMode { get; set; }

		[Export ("networkAccessAllowed")]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		PHAssetImageProgressHandler ProgressHandler { get; set; }

		[iOS (10,0)]
		[Export ("version", ArgumentSemantic.Assign)]
		PHImageRequestOptionsVersion Version { get; set; }
	}

	[iOS (9,1)]
	[TV (10,0)]
	[NoMac]
	[Static]
	interface PHLivePhotoInfo {
		[Field ("PHLivePhotoInfoErrorKey")]
		NSString ErrorKey { get; }

		[Field ("PHLivePhotoInfoIsDegradedKey")]
		NSString IsDegradedKey { get; }

		[Field ("PHLivePhotoInfoCancelledKey")]
		NSString CancelledKey { get; }
	}

#if XAMCORE_4_0
	delegate CIImage PHLivePhotoFrameProcessingBlock (IPHLivePhotoFrame frame, ref NSError error);
#else
	delegate CIImage PHLivePhotoFrameProcessingBlock2 (IPHLivePhotoFrame frame, ref NSError error);
#endif

	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NS_UNAVAILABLE
	interface PHLivePhotoEditingContext {
		[Export ("initWithLivePhotoEditingInput:")]
		[DesignatedInitializer]
		IntPtr Constructor (PHContentEditingInput livePhotoInput);

		[Export ("fullSizeImage")]
		CIImage FullSizeImage { get; }

		[Export ("duration")]
		CMTime Duration { get; }

		[Export ("photoTime")]
		CMTime PhotoTime { get; }

		[NullAllowed, Export ("frameProcessor", ArgumentSemantic.Copy)]
#if XAMCORE_4_0
		PHLivePhotoFrameProcessingBlock FrameProcessor { get; set; }
#else
		PHLivePhotoFrameProcessingBlock2 FrameProcessor2 { get; set; }
#endif

		[Export ("audioVolume")]
		float AudioVolume { get; set; }

		[Export ("orientation")]
		CGImagePropertyOrientation Orientation { get; }

		[Internal]
		[Export ("prepareLivePhotoForPlaybackWithTargetSize:options:completionHandler:")]
		void _PrepareLivePhotoForPlayback (CGSize targetSize, [NullAllowed] NSDictionary options, Action<PHLivePhoto, NSError> handler);

		[Async]
		[Wrap ("_PrepareLivePhotoForPlayback (targetSize, null, handler)")]
		void PrepareLivePhotoForPlayback (CGSize targetSize, Action<PHLivePhoto, NSError> handler);

		[Async]
		[Wrap ("_PrepareLivePhotoForPlayback (targetSize, (NSDictionary)options, handler)", IsVirtual = true)]
		void PrepareLivePhotoForPlayback (CGSize targetSize, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<PHLivePhoto, NSError> handler);

#if XAMCORE_2_0
		// the API existed earlier but the key needed to create the strong dictionary did not work
		[iOS (11,0)][TV (11,0)][Mac (10,12, onlyOn64 : true)]
		[Async]
		[Wrap ("_PrepareLivePhotoForPlayback (targetSize, options?.Dictionary, handler)")]
		void PrepareLivePhotoForPlayback (CGSize targetSize, [NullAllowed] PHLivePhotoEditingOption options, Action<PHLivePhoto, NSError> handler);
#endif

		[Internal]
		[Export ("saveLivePhotoToOutput:options:completionHandler:")]
		void _SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] NSDictionary options, Action<bool, NSError> handler);

		[Async]
		[Wrap ("_SaveLivePhoto (output, null, handler)")]
		void SaveLivePhoto (PHContentEditingOutput output, Action<bool, NSError> handler);

		[Async]
		[Wrap ("_SaveLivePhoto (output, options, handler)", IsVirtual = true)]
		void SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<bool, NSError> handler);

#if XAMCORE_2_0
		// the API existed earlier but the key needed to create the strong dictionary did not work
		[iOS (11,0)][TV (11,0)][Mac (10,12, onlyOn64 : true)]
		[Async]
		[Wrap ("_SaveLivePhoto (output, options?.Dictionary, handler)")]
		void SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] PHLivePhotoEditingOption options, Action<bool, NSError> handler);
#endif

		[Export ("cancel")]
		void Cancel ();
	}

	interface IPHLivePhotoFrame {}

	[iOS (10,0)]
	[TV (10,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Protocol]
	interface PHLivePhotoFrame {
		[Abstract]
		[Export ("image")]
		CIImage Image { get; }

		[Abstract]
		[Export ("time")]
		CMTime Time { get; }

		[Abstract]
		[Export ("type")]
		PHLivePhotoFrameType Type { get; }

		[Abstract]
		[Export ("renderScale")]
		nfloat RenderScale { get; }
	}

#if XAMCORE_2_0 // fails to build with mac/classic
	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,12, onlyOn64 : true)]
	[Static][Internal]
	interface PHLivePhotoEditingOptionKeys {
		[Field ("PHLivePhotoShouldRenderAtPlaybackTime")]
		NSString ShouldRenderAtPlaybackTimeKey { get; }
	}

	[iOS (11,0)]
	[TV (11,0)]
	[Mac (10,12, onlyOn64 : true)]
	[StrongDictionary ("PHLivePhotoEditingOptionKeys")]
	interface PHLivePhotoEditingOption {
		bool ShouldRenderAtPlaybackTime { get; }
	}
#endif

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (PHAssetCollection))]
	interface PHProject {

		[Export ("projectExtensionData")]
		NSData ProjectExtensionData { get; }
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	interface PHProjectChangeRequest {

		[Export ("initWithProject:")]
		IntPtr Constructor (PHProject project);

		[Export ("title")]
		string Title { get; set; }

		[Export ("projectExtensionData", ArgumentSemantic.Copy)]
		NSData ProjectExtensionData { get; set; }

		[Export ("setKeyAsset:")]
		void SetKeyAsset ([NullAllowed] PHAsset keyAsset);
	}

	[Mac (10,13, onlyOn64 : true)]
	[NoiOS][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHCloudIdentifier : NSSecureCoding {

		[Static]
		[Export ("notFoundIdentifier")]
		PHCloudIdentifier NotFoundIdentifier { get; }

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("initWithStringValue:")]
		IntPtr Constructor (string stringValue);
	}
}
