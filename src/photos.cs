using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreLocation;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using ImageIO;
using System;
using UniformTypeIdentifiers;
#if !MONOMAC
using UIKit;
using NSImage = Foundation.NSObject; // help [NoiOS] and [NoTV]
#else
using AppKit;
using UIImage = AppKit.NSImage;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Photos {
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHAdjustmentData : NSCoding, NSSecureCoding {

		[Export ("initWithFormatIdentifier:formatVersion:data:")]
		NativeHandle Constructor (string formatIdentifier, string formatVersion, NSData data);

		[Export ("formatIdentifier", ArgumentSemantic.Copy)]
		string FormatIdentifier { get; }

		[Export ("formatVersion", ArgumentSemantic.Copy)]
		string FormatVersion { get; }

		[Export ("data", ArgumentSemantic.Strong)]
		NSData Data { get; }
	}

	[MacCatalyst (13, 1)]
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
		[NullAllowed]
		NSDate CreationDate { get; }

		[Export ("modificationDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate ModificationDate { get; }

		[Export ("location", ArgumentSemantic.Strong)]
		[NullAllowed]
		CLLocation Location { get; }

		[Export ("duration", ArgumentSemantic.Assign)]
		double Duration { get; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "No longer supported.")]
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Export ("syncFailureHidden")]
		bool SyncFailureHidden { [Bind ("isSyncFailureHidden")] get; }

		[Export ("favorite")]
		bool Favorite { [Bind ("isFavorite")] get; }

		[MacCatalyst (13, 1)]
		[Export ("burstIdentifier", ArgumentSemantic.Strong)]
		[NullAllowed]
		string BurstIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("burstSelectionTypes")]
		PHAssetBurstSelectionType BurstSelectionTypes { get; }

		[MacCatalyst (13, 1)]
		[Export ("representsBurst")]
		bool RepresentsBurst { get; }

		[MacCatalyst (13, 1)]
		[Export ("canPerformEditOperation:")]
		bool CanPerformEditOperation (PHAssetEditOperation editOperation);

		[Static]
		[Export ("fetchAssetsInAssetCollection:options:")]
		PHFetchResult FetchAssets (PHAssetCollection assetCollection, [NullAllowed] PHFetchOptions options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("fetchAssetsWithMediaType:options:")]
		PHFetchResult FetchAssets (PHAssetMediaType mediaType, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetsWithLocalIdentifiers:options:")]
		PHFetchResult FetchAssetsUsingLocalIdentifiers (string [] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchKeyAssetsInAssetCollection:options:")]
		[return: NullAllowed]
		PHFetchResult FetchKeyAssets (PHAssetCollection assetCollection, [NullAllowed] PHFetchOptions options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("fetchAssetsWithBurstIdentifier:options:")]
		PHFetchResult FetchAssets (string burstIdentifier, [NullAllowed] PHFetchOptions options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("fetchAssetsWithOptions:")]
		PHFetchResult FetchAssets ([NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[NoMac]
		[Static]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("fetchAssetsWithALAssetURLs:options:")]
		PHFetchResult FetchAssets (NSUrl [] assetUrls, [NullAllowed] PHFetchOptions options);

		[MacCatalyst (13, 1)]
		[Export ("sourceType", ArgumentSemantic.Assign)]
		PHAssetSourceType SourceType { get; }

		[MacCatalyst (13, 1)]
		[Export ("playbackStyle", ArgumentSemantic.Assign)]
		PHAssetPlaybackStyle PlaybackStyle { get; }

		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PHPhotosError.IdentifierNotFound' instead.")]
		[NoTV]
		[NoiOS]
		[Field ("PHLocalIdentifierNotFound")]
		NSString LocalIdentifierNotFound { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("adjustmentFormatIdentifier")]
		string AdjustmentFormatIdentifier { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("hasAdjustments")]
		bool HasAdjustments { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[PHAssetChangeRequest init]: unrecognized selector sent to instance 0x8165d150
	[BaseType (typeof (PHChangeRequest))]
	interface PHAssetChangeRequest {

		[Static]
		[Export ("creationRequestForAssetFromImage:")]
		PHAssetChangeRequest FromImage (UIImage image);

		[Static]
		[Export ("creationRequestForAssetFromImageAtFileURL:")]
		[return: NullAllowed]
		PHAssetChangeRequest FromImage (NSUrl fileUrl);

		[Static]
		[Export ("creationRequestForAssetFromVideoAtFileURL:")]
		[return: NullAllowed]
		PHAssetChangeRequest FromVideo (NSUrl fileUrl);

		[Export ("placeholderForCreatedAsset", ArgumentSemantic.Strong)]
		[NullAllowed]
		PHObjectPlaceholder PlaceholderForCreatedAsset { get; }

		[Static]
		[Export ("deleteAssets:")]
		void DeleteAssets (PHAsset [] assets);

		[Static]
		[Export ("changeRequestForAsset:")]
		PHAssetChangeRequest ChangeRequest (PHAsset asset);

		[Export ("creationDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate CreationDate { get; set; }

		[Export ("location", ArgumentSemantic.Strong)]
		[NullAllowed]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHAssetChangeRequest))]
	[DisableDefaultCtor]
	interface PHAssetCreationRequest {
		[Static]
		[Export ("creationRequestForAsset")]
		PHAssetCreationRequest CreationRequestForAsset ();

		[Static]
		[Internal, Export ("supportsAssetResourceTypes:")]
		bool _SupportsAssetResourceTypes (NSNumber [] types);

		[Export ("addResourceWithType:fileURL:options:")]
		void AddResource (PHAssetResourceType type, NSUrl fileURL, [NullAllowed] PHAssetResourceCreationOptions options);

		[Export ("addResourceWithType:data:options:")]
		void AddResource (PHAssetResourceType type, NSData data, [NullAllowed] PHAssetResourceCreationOptions options);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	delegate void PHProgressHandler (double progress, ref bool stop);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crashes: -[PHAssetResource init]: unrecognized selector sent to instance 0x7f9e15884e90
	interface PHAssetResource {

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
		PHAssetResource [] GetAssetResources (PHAsset forAsset);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("assetResourcesForLivePhoto:")]
		PHAssetResource [] GetAssetResources (PHLivePhoto livePhoto);

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("pixelWidth")]
		nint PixelWidth { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("pixelHeight")]
		nint PixelHeight { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHAssetResourceCreationOptions : NSCopying {
		[NullAllowed, Export ("originalFilename")]
		string OriginalFilename { get; set; }

		[NullAllowed, Export ("uniformTypeIdentifier")]
		string UniformTypeIdentifier { get; set; }

		[Export ("shouldMoveFile")]
		bool ShouldMoveFile { get; set; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (PHAsset))]
	interface PHAssetContentEditingInputExtensions {

		[Export ("requestContentEditingInputWithOptions:completionHandler:")]
		nuint RequestContentEditingInput ([NullAllowed] PHContentEditingInputRequestOptions options, PHContentEditingHandler completionHandler);

		[Export ("cancelContentEditingInputRequest:")]
		void CancelContentEditingInputRequest (nuint requestID);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHChangeRequest))]
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
		void DeleteAssetCollections (PHAssetCollection [] assetCollections);

		[Static]
		[Export ("changeRequestForAssetCollection:")]
		[return: NullAllowed]
		PHAssetCollectionChangeRequest ChangeRequest (PHAssetCollection assetCollection);

		[Static]
		[Export ("changeRequestForAssetCollection:assets:")]
		[return: NullAllowed]
		PHAssetCollectionChangeRequest ChangeRequest (PHAssetCollection assetCollection, PHFetchResult assets);

		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set; }

		[Export ("addAssets:")]
		void AddAssets (PHObject [] assets);

		[Export ("insertAssets:atIndexes:")]
		void InsertAssets (PHObject [] assets, NSIndexSet indexes);

		[Export ("removeAssets:")]
		void RemoveAssets (PHObject [] assets);

		[Export ("removeAssetsAtIndexes:")]
		void RemoveAssets (NSIndexSet indexes);

		[Export ("replaceAssetsAtIndexes:withAssets:")]
		void ReplaceAssets (NSIndexSet indexes, PHObject [] assets);

		[Export ("moveAssetsAtIndexes:toIndex:")]
		void MoveAssets (NSIndexSet fromIndexes, nuint toIndex);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHAssetResourceManager {
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHAssetResourceRequestOptions : NSCopying {
		[Export ("networkAccessAllowed")]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		Action<double> ProgressHandler { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHChange {

		[Export ("changeDetailsForObject:")]
		[return: NullAllowed]
		PHObjectChangeDetails GetObjectChangeDetails (PHObject obj);

		[Export ("changeDetailsForFetchResult:")]
		[return: NullAllowed]
		PHFetchResultChangeDetails GetFetchResultChangeDetails (PHFetchResult obj);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHObjectChangeDetails {

		[Export ("objectBeforeChanges", ArgumentSemantic.Strong)]
		NSObject ObjectBeforeChanges { get; }

		[Export ("objectAfterChanges", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject ObjectAfterChanges { get; }

		[Export ("assetContentChanged")]
		bool AssetContentChanged { get; }

		[Export ("objectWasDeleted")]
		bool ObjectWasDeleted { get; }
	}

	// supports iOS (8,0) and iOS (13,0) only supports 64 bits, not 32 bits
	[TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	// include the availability attributes to any new member (and don't trust the type-level ones)
	interface PHChangeRequest { }

	[MacCatalyst (13, 1)]
	delegate void PHChangeDetailEnumerator (nuint fromIndex, nuint toIndex);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHFetchResultChangeDetails {

		[Export ("fetchResultBeforeChanges", ArgumentSemantic.Strong)]
		PHFetchResult FetchResultBeforeChanges { get; }

		[Export ("fetchResultAfterChanges", ArgumentSemantic.Strong)]
		PHFetchResult FetchResultAfterChanges { get; }

		[Export ("hasIncrementalChanges", ArgumentSemantic.Assign)]
		bool HasIncrementalChanges { get; }

		[Export ("removedIndexes", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSIndexSet RemovedIndexes { get; }

		[Export ("removedObjects", ArgumentSemantic.Strong)]
		PHObject [] RemovedObjects { get; }

		[Export ("insertedIndexes", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSIndexSet InsertedIndexes { get; }

		[Export ("insertedObjects", ArgumentSemantic.Strong)]
		PHObject [] InsertedObjects { get; }

		[Export ("changedIndexes", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSIndexSet ChangedIndexes { get; }

		[Export ("changedObjects", ArgumentSemantic.Strong)]
		PHObject [] ChangedObjects { get; }

		[Export ("enumerateMovesWithBlock:")]
		void EnumerateMoves (PHChangeDetailEnumerator handler);

		[Export ("hasMoves", ArgumentSemantic.Assign)]
		bool HasMoves { get; }

		[Static]
		[Export ("changeDetailsFromFetchResult:toFetchResult:changedObjects:")]
		PHFetchResultChangeDetails ChangeDetails (PHFetchResult fromResult, PHFetchResult toResult, PHObject [] changedObjects);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHObject))]
	[DisableDefaultCtor] // not user createable (calling description fails, see below) must be fetched by API
						 // NSInternalInconsistencyException Reason: PHCollection has no identifier
#if TVOS || NET
	[Abstract] // Acording to docs: The abstract superclass for Photos asset collections and collection lists.
#endif
	interface PHCollection {

		[Export ("canContainAssets", ArgumentSemantic.Assign)]
		bool CanContainAssets { get; }

		[Export ("canContainCollections", ArgumentSemantic.Assign)]
		bool CanContainCollections { get; }

		[MacCatalyst (13, 1)]
		[Export ("localizedTitle", ArgumentSemantic.Strong)]
		[NullAllowed]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHCollection))]
	interface PHAssetCollection {

		[Export ("assetCollectionType")]
		PHAssetCollectionType AssetCollectionType { get; }

		[Export ("assetCollectionSubtype")]
		PHAssetCollectionSubtype AssetCollectionSubtype { get; }

		[Export ("estimatedAssetCount")]
		nuint EstimatedAssetCount { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate EndDate { get; }

		[Export ("approximateLocation", ArgumentSemantic.Strong)]
		[NullAllowed]
		CLLocation ApproximateLocation { get; }

		[Export ("localizedLocationNames", ArgumentSemantic.Strong)]
		string [] LocalizedLocationNames { get; }

		[Static]
		[Export ("fetchAssetCollectionsWithLocalIdentifiers:options:")]
		PHFetchResult FetchAssetCollections (string [] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetCollectionsWithType:subtype:options:")]
		PHFetchResult FetchAssetCollections (PHAssetCollectionType type, PHAssetCollectionSubtype subtype, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchAssetCollectionsContainingAsset:withType:options:")]
		PHFetchResult FetchAssetCollections (PHAsset asset, PHAssetCollectionType type, [NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.iOS, 16, 0, message: "Will be removed in a future release.")]
		[Deprecated (PlatformName.TvOS, 16, 0, message: "Will be removed in a future release.")]
		[Deprecated (PlatformName.MacOSX, 13, 0, message: "Will be removed in a future release.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Will be removed in a future release.")]
		[Static]
		[Export ("fetchAssetCollectionsWithALAssetGroupURLs:options:")]
		PHFetchResult FetchAssetCollections (NSUrl [] assetGroupUrls, [NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("fetchMomentsInMomentList:options:")]
		PHFetchResult FetchMoments (PHCollectionList momentList, [NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("fetchMomentsWithOptions:")]
		PHFetchResult FetchMoments ([NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("transientAssetCollectionWithAssets:title:")]
		PHAssetCollection GetTransientAssetCollection (PHAsset [] assets, [NullAllowed] string title);

		[Static]
		[Export ("transientAssetCollectionWithAssetFetchResult:title:")]
		PHAssetCollection GetTransientAssetCollection (PHFetchResult fetchResult, [NullAllowed] string title);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHCollection))]
	interface PHCollectionList {

		[Export ("collectionListType")]
		PHCollectionListType CollectionListType { get; }

		[Export ("collectionListSubtype")]
		PHCollectionListSubtype CollectionListSubtype { get; }

		[Export ("startDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSDate EndDate { get; }

		[Export ("localizedLocationNames", ArgumentSemantic.Strong)]
		string [] LocalizedLocationNames { get; }

		[Static]
		[Export ("fetchCollectionListsContainingCollection:options:")]
		PHFetchResult FetchCollectionLists (PHCollection collection, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchCollectionListsWithLocalIdentifiers:options:")]
		PHFetchResult FetchCollectionLists (string [] identifiers, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("fetchCollectionListsWithType:subtype:options:")]
		PHFetchResult FetchCollectionLists (PHCollectionListType type, PHCollectionListSubtype subType, [NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("fetchMomentListsWithSubtype:containingMoment:options:")]
		PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, PHAssetCollection moment, [NullAllowed] PHFetchOptions options);

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static]
		[Export ("fetchMomentListsWithSubtype:options:")]
		PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, [NullAllowed] PHFetchOptions options);

		[Static]
		[Export ("transientCollectionListWithCollections:title:")]
		PHCollectionList CreateTransientCollectionList (PHAssetCollection [] collections, [NullAllowed] string title);

		[Static]
		[Export ("transientCollectionListWithCollectionsFetchResult:title:")]
		PHCollectionList CreateTransientCollectionList (PHFetchResult fetchResult, [NullAllowed] string title);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHChangeRequest))]
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
		void DeleteCollectionLists (PHCollectionList [] collectionLists);

		[Static]
		[Export ("changeRequestForCollectionList:")]
		[return: NullAllowed]
		PHCollectionListChangeRequest ChangeRequest (PHCollectionList collectionList);

		[Static]
		[Export ("changeRequestForCollectionList:childCollections:")]
		[return: NullAllowed]
		PHCollectionListChangeRequest ChangeRequest (PHCollectionList collectionList, PHFetchResult childCollections);

		[TV (14, 2), Mac (11, 0), iOS (14, 2)]
		[MacCatalyst (14, 2)]
		[Static]
		[Export ("changeRequestForTopLevelCollectionListUserCollections:")]
		[return: NullAllowed]
		PHCollectionListChangeRequest ChangeRequestForTopLevelCollectionList (PHFetchResult childCollections);

		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set; }

		[Export ("addChildCollections:")]
		void AddChildCollections (PHCollection [] collections);

		[Export ("insertChildCollections:atIndexes:")]
		void InsertChildCollections (PHCollection [] collections, NSIndexSet indexes);

		[Export ("removeChildCollections:")]
		void RemoveChildCollections (PHCollection [] collections);

		[Export ("removeChildCollectionsAtIndexes:")]
		void RemoveChildCollections (NSIndexSet indexes);

		[Export ("replaceChildCollectionsAtIndexes:withChildCollections:")]
		void ReplaceChildCollection (NSIndexSet indexes, PHCollection [] collections);

		[Export ("moveChildCollectionsAtIndexes:toIndex:")]
		void MoveChildCollections (NSIndexSet indexes, nuint toIndex);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHContentEditingInput {

		[Export ("mediaType")]
		PHAssetMediaType MediaType { get; }

		[Export ("mediaSubtypes")]
		PHAssetMediaSubtype MediaSubtypes { get; }

		[Export ("creationDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate CreationDate { get; }

		[Export ("location", ArgumentSemantic.Copy)]
		[NullAllowed]
		CLLocation Location { get; }

		[Export ("uniformTypeIdentifier")]
		[NullAllowed]
		string UniformTypeIdentifier { get; }

		[NullAllowed]
		[Export ("adjustmentData", ArgumentSemantic.Strong)]
		PHAdjustmentData AdjustmentData { get; }

		[Export ("displaySizeImage", ArgumentSemantic.Strong)]
		[NullAllowed]
#if MONOMAC
		NSImage DisplaySizeImage { get; }
#else
		UIImage DisplaySizeImage { get; }
#endif

		[Export ("fullSizeImageURL", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSUrl FullSizeImageUrl { get; }

		[Export ("fullSizeImageOrientation")]
		CoreImage.CIImageOrientation FullSizeImageOrientation { get; }

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AudiovisualAsset' property instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'AudiovisualAsset' property instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AudiovisualAsset' property instead.")]
		[NullAllowed, Export ("avAsset", ArgumentSemantic.Strong)]
		AVAsset AvAsset { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("audiovisualAsset", ArgumentSemantic.Strong)]
		AVAsset AudiovisualAsset { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("livePhoto", ArgumentSemantic.Strong)]
		PHLivePhoto LivePhoto { get; }

		[MacCatalyst (13, 1)]
		[Export ("playbackStyle", ArgumentSemantic.Assign)]
		PHAssetPlaybackStyle PlaybackStyle { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHContentEditingOutput : NSCoding, NSSecureCoding {

		[Export ("initWithContentEditingInput:")]
		NativeHandle Constructor (PHContentEditingInput contentEditingInput);

		[Export ("initWithPlaceholderForCreatedAsset:")]
		NativeHandle Constructor (PHObjectPlaceholder placeholderForCreatedAsset);

		[NullAllowed] // by default this property is null
		[Export ("adjustmentData", ArgumentSemantic.Strong)]
		PHAdjustmentData AdjustmentData { get; set; }

		[Export ("renderedContentURL", ArgumentSemantic.Copy)]
		NSUrl RenderedContentUrl { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("defaultRenderedContentType", ArgumentSemantic.Copy)]
		UTType DefaultRenderedContentType { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportedRenderedContentTypes", ArgumentSemantic.Copy)]
		UTType [] SupportedRenderedContentTypes { get; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("renderedContentURLForType:error:")]
		[return: NullAllowed]
		NSUrl GetRenderedContentUrl (UTType type, [NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHFetchOptions : NSCopying {

		[NullAllowed] // by default this property is null
		[Export ("predicate", ArgumentSemantic.Strong)]
		NSPredicate Predicate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("sortDescriptors", ArgumentSemantic.Strong)]
		NSSortDescriptor [] SortDescriptors { get; set; }

		[Export ("includeHiddenAssets")]
		bool IncludeHiddenAssets { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("includeAllBurstAssets", ArgumentSemantic.Assign)]
		bool IncludeAllBurstAssets { get; set; }

		[Export ("wantsIncrementalChangeDetails", ArgumentSemantic.Assign)]
		bool WantsIncrementalChangeDetails { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("includeAssetSourceTypes", ArgumentSemantic.Assign)]
		PHAssetSourceType IncludeAssetSourceTypes { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fetchLimit", ArgumentSemantic.Assign)]
		nuint FetchLimit { get; set; }
	}

	[MacCatalyst (13, 1)]
	delegate void PHFetchResultEnumerator (NSObject element, nuint elementIndex, out bool stop);

	[MacCatalyst (13, 1)]
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

#if !XAMCORE_5_0
		[Obsolete ("Use the 'FirstObject' property instead.")]
		[Wrap ("FirstObject", IsVirtual = true)]
		[NullAllowed]
		NSObject firstObject { get; }
#endif

		[Export ("firstObject")]
		[NullAllowed]
		NSObject FirstObject { get; }

		[Export ("lastObject")]
		[NullAllowed]
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

	delegate void PHAssetImageProgressHandler (double progress, NSError error, out bool stop, NSDictionary info);

	[MacCatalyst (13, 1)]
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

		[Export ("progressHandler", ArgumentSemantic.Copy)]
		[NullAllowed]
		PHAssetImageProgressHandler ProgressHandler { get; set; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("allowSecondaryDegradedImage")]
		bool AllowSecondaryDegradedImage { get; set; }
	}

	delegate void PHAssetVideoProgressHandler (double progress, NSError error, out bool stop, NSDictionary info);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHVideoRequestOptions : NSCopying {

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

	[MacCatalyst (13, 1)]
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
#if NET
	delegate void PHImageManagerRequestAVAssetHandler (AVAsset asset, AVAudioMix audioMix, NSDictionary info);
#else
	delegate void PHImageManagerRequestAvAssetHandler (AVAsset asset, AVAudioMix audioMix, NSDictionary info);
#endif
	delegate void PHImageManagerRequestLivePhoto (PHLivePhoto livePhoto, NSDictionary info);
	delegate void PHImageManagerRequestImageDataHandler ([NullAllowed] NSData imageData, [NullAllowed] string dataUti, CGImagePropertyOrientation orientation, [NullAllowed] NSDictionary info);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHImageManager {

		[Static]
		[Export ("defaultManager")]
		PHImageManager DefaultManager { get; }

		[Export ("requestImageForAsset:targetSize:contentMode:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestImageForAsset (PHAsset asset, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHImageRequestOptions options, PHImageResultHandler resultHandler);

		[Export ("cancelImageRequest:")]
		void CancelImageRequest (int /* PHImageRequestID = int32_t */ requestID);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.")]
		[Export ("requestImageDataForAsset:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestImageData (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageDataHandler handler);

		[MacCatalyst (13, 1)]
		[Export ("requestPlayerItemForVideo:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestPlayerItem (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestPlayerHandler resultHandler);

		[MacCatalyst (13, 1)]
		[Export ("requestExportSessionForVideo:options:exportPreset:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestExportSession (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, string exportPreset, PHImageManagerRequestExportHandler resultHandler);

		[MacCatalyst (13, 1)]
		[Export ("requestAVAssetForVideo:options:resultHandler:")]
#if NET
		int /* PHImageRequestID = int32_t */ RequestAVAsset (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestAVAssetHandler resultHandler);
#else
		int /* PHImageRequestID = int32_t */ RequestAvAsset (PHAsset asset, [NullAllowed] PHVideoRequestOptions options, PHImageManagerRequestAvAssetHandler resultHandler);
#endif

		[Field ("PHImageManagerMaximumSize")]
		CGSize MaximumSize { get; }

		[MacCatalyst (13, 1)]
		[Export ("requestLivePhotoForAsset:targetSize:contentMode:options:resultHandler:")]
		int /* PHImageRequestID = int32_t */ RequestLivePhoto (PHAsset asset, CGSize targetSize, PHImageContentMode contentMode, [NullAllowed] PHLivePhotoRequestOptions options, PHImageManagerRequestLivePhoto resultHandler);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestImageDataAndOrientationForAsset:options:resultHandler:")]
		int RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler);
	}

#if MONOMAC
	delegate void PHImageDataHandler (NSData data, NSString dataUti, CGImagePropertyOrientation orientation, NSDictionary info);
#else
	delegate void PHImageDataHandler (NSData data, NSString dataUti, UIImageOrientation orientation, NSDictionary info);
#endif

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // doc -> "abstract base class"
						 // throws "NSInternalInconsistencyException Reason: PHObject has no identifier"
#if TVOS || NET
	[Abstract] // Acording to docs: The abstract base class for Photos model objects (assets and collections).
#endif
	interface PHObject : NSCopying {

		[Export ("localIdentifier", ArgumentSemantic.Copy)]
		string LocalIdentifier { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PHObject))]
	interface PHObjectPlaceholder {

	}

	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface PHPhotoLibraryChangeObserver {

		[Abstract]
		[Export ("photoLibraryDidChange:")]
		void PhotoLibraryDidChange (PHChange changeInstance);
	}

	interface IPHPhotoLibraryAvailabilityObserver { }

	[TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface PHPhotoLibraryAvailabilityObserver {

		[Abstract]
		[Export ("photoLibraryDidBecomeUnavailable:")]
		void PhotoLibraryDidBecomeUnavailable (PHPhotoLibrary photoLibrary);
	}

	delegate void PHPhotoLibraryCancellableChangeHandler (out bool cancel);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: -[PHPhotoLibrary init] unsupported
	interface PHPhotoLibrary {

		[Static]
		[Export ("sharedPhotoLibrary")]
		PHPhotoLibrary SharedPhotoLibrary { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GetAuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'GetAuthorizationStatus' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'GetAuthorizationStatus' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GetAuthorizationStatus' instead.")]
		[Static, Export ("authorizationStatus")]
		PHAuthorizationStatus AuthorizationStatus { get; }

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("authorizationStatusForAccessLevel:")]
		PHAuthorizationStatus GetAuthorizationStatus (PHAccessLevel accessLevel);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'RequestAuthorization(PHAccessLevel, Action<PHAuthorizationStatus>)' overload instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'RequestAuthorization(PHAccessLevel, Action<PHAuthorizationStatus>)' overload instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'RequestAuthorization(PHAccessLevel, Action<PHAuthorizationStatus>)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'RequestAuthorization(PHAccessLevel, Action<PHAuthorizationStatus>)' overload instead.")]
		[Static, Export ("requestAuthorization:")]
		[Async]
		void RequestAuthorization (Action<PHAuthorizationStatus> handler);

		[TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("requestAuthorizationForAccessLevel:handler:")]
		[Async]
		void RequestAuthorization (PHAccessLevel accessLevel, Action<PHAuthorizationStatus> handler);

		// no [Async] since we're binding performChangesAndWait:error: too
		[Export ("performChanges:completionHandler:")]
		void PerformChanges (Action changeHandler, [NullAllowed] Action<bool, NSError> completionHandler);

		[Export ("performChangesAndWait:error:")]
		bool PerformChangesAndWait (Action changeHandler, out NSError error);

		[Export ("registerChangeObserver:")]
		void RegisterChangeObserver ([Protocolize] PHPhotoLibraryChangeObserver observer);

		[Export ("unregisterChangeObserver:")]
		void UnregisterChangeObserver ([Protocolize] PHPhotoLibraryChangeObserver observer);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("unavailabilityReason", ArgumentSemantic.Strong)]
		NSError UnavailabilityReason { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("registerAvailabilityObserver:")]
		void Register (IPHPhotoLibraryAvailabilityObserver observer);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("unregisterAvailabilityObserver:")]
		void Unregister (IPHPhotoLibraryAvailabilityObserver observer);

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("PHLocalIdentifiersErrorKey")]
		NSString LocalIdentifiersErrorKey { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("fetchPersistentChangesSinceToken:error:")]
		[return: NullAllowed]
		PHPersistentChangeFetchResult FetchPersistentChanges (PHPersistentChangeToken since, [NullAllowed] out NSError error);

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("currentChangeToken")]
		PHPersistentChangeToken CurrentChangeToken { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Category]
	[BaseType (typeof (PHPhotoLibrary))]
	interface PHPhotoLibrary_CloudIdentifiers {

		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("localIdentifierMappingsForCloudIdentifiers:")]
		NSDictionary<PHCloudIdentifier, PHLocalIdentifierMapping> GetLocalIdentifierMappings (PHCloudIdentifier [] cloudIdentifiers);

		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("cloudIdentifierMappingsForLocalIdentifiers:")]
		NSDictionary<NSString, PHCloudIdentifierMapping> GetCloudIdentifierMappings (string [] localIdentifiers);

		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'localIdentifierMappingsForCloudIdentifiers:' instead.")]
		[Export ("localIdentifiersForCloudIdentifiers:")]
		string [] GetLocalIdentifiers (PHCloudIdentifier [] cloudIdentifiers);

		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'cloudIdentifierMappingsForCloudIdentifiers:' instead.")]
		[Export ("cloudIdentifiersForLocalIdentifiers:")]
		PHCloudIdentifier [] GetCloudIdentifiers (string [] localIdentifiers);

		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PHPhotosError.IdentifierNotFound' instead.")]
		[Field ("PHLocalIdentifierNotFound")]
		NSString LocalIdentifierNotFound { get; }
	}

#if MONOMAC
	[DisableDefaultCtor] // NS_UNAVAILABLE
#endif
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHLivePhoto : NSSecureCoding, NSCopying
#if IOS
	, NSItemProviderReading
#endif
	{
		[Export ("size")]
		CGSize Size { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("requestLivePhotoWithResourceFileURLs:placeholderImage:targetSize:contentMode:resultHandler:")]
		int RequestLivePhoto (NSUrl [] fileUrls, [NullAllowed] UIImage image, CGSize targetSize, PHImageContentMode contentMode, Action<PHLivePhoto, NSDictionary> resultHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cancelLivePhotoRequestWithRequestID:")]
		void CancelLivePhotoRequest (int requestID);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PHLivePhotoRequestOptions : NSCopying {
		[Export ("deliveryMode", ArgumentSemantic.Assign)]
		PHImageRequestOptionsDeliveryMode DeliveryMode { get; set; }

		[Export ("networkAccessAllowed")]
		bool NetworkAccessAllowed { [Bind ("isNetworkAccessAllowed")] get; set; }

		[NullAllowed, Export ("progressHandler", ArgumentSemantic.Copy)]
		PHAssetImageProgressHandler ProgressHandler { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("version", ArgumentSemantic.Assign)]
		PHImageRequestOptionsVersion Version { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface PHLivePhotoInfo {
		[Field ("PHLivePhotoInfoErrorKey")]
		NSString ErrorKey { get; }

		[Field ("PHLivePhotoInfoIsDegradedKey")]
		NSString IsDegradedKey { get; }

		[Field ("PHLivePhotoInfoCancelledKey")]
		NSString CancelledKey { get; }
	}

#if NET
	delegate CIImage PHLivePhotoFrameProcessingBlock (IPHLivePhotoFrame frame, ref NSError error);
#else
	delegate CIImage PHLivePhotoFrameProcessingBlock2 (IPHLivePhotoFrame frame, ref NSError error);
#endif

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NS_UNAVAILABLE
	interface PHLivePhotoEditingContext {
		[Export ("initWithLivePhotoEditingInput:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PHContentEditingInput livePhotoInput);

		[Export ("fullSizeImage")]
		CIImage FullSizeImage { get; }

		[Export ("duration")]
		CMTime Duration { get; }

		[Export ("photoTime")]
		CMTime PhotoTime { get; }

		[NullAllowed, Export ("frameProcessor", ArgumentSemantic.Copy)]
#if NET
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
		[Wrap ("_PrepareLivePhotoForPlayback (targetSize, (options as NSDictionary), handler)", IsVirtual = true)]
		void PrepareLivePhotoForPlayback (CGSize targetSize, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<PHLivePhoto, NSError> handler);

		// the API existed earlier but the key needed to create the strong dictionary did not work
		[MacCatalyst (13, 1)]
		[Async]
		[Wrap ("_PrepareLivePhotoForPlayback (targetSize, options.GetDictionary (), handler)")]
		void PrepareLivePhotoForPlayback (CGSize targetSize, [NullAllowed] PHLivePhotoEditingOption options, Action<PHLivePhoto, NSError> handler);

		[Internal]
		[Export ("saveLivePhotoToOutput:options:completionHandler:")]
		void _SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] NSDictionary options, Action<bool, NSError> handler);

		[Async]
		[Wrap ("_SaveLivePhoto (output, null, handler)")]
		void SaveLivePhoto (PHContentEditingOutput output, Action<bool, NSError> handler);

		[Async]
		[Wrap ("_SaveLivePhoto (output, options, handler)", IsVirtual = true)]
		void SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<bool, NSError> handler);

		// the API existed earlier but the key needed to create the strong dictionary did not work
		[MacCatalyst (13, 1)]
		[Async]
		[Wrap ("_SaveLivePhoto (output, options.GetDictionary (), handler)")]
		void SaveLivePhoto (PHContentEditingOutput output, [NullAllowed] PHLivePhotoEditingOption options, Action<bool, NSError> handler);

		[Export ("cancel")]
		void Cancel ();
	}

	interface IPHLivePhotoFrame { }

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface PHLivePhotoEditingOptionKeys {
		[Field ("PHLivePhotoShouldRenderAtPlaybackTime")]
		NSString ShouldRenderAtPlaybackTimeKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("PHLivePhotoEditingOptionKeys")]
	interface PHLivePhotoEditingOption {
		bool ShouldRenderAtPlaybackTime { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (PHAssetCollection))]
	interface PHProject {

		[Export ("projectExtensionData")]
		NSData ProjectExtensionData { get; }

		[Export ("hasProjectPreview")]
		bool HasProjectPreview { get; }
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (PHChangeRequest))]
	interface PHProjectChangeRequest {

		[Export ("initWithProject:")]
		NativeHandle Constructor (PHProject project);

		[Export ("title")]
		string Title { get; set; }

		[Export ("projectExtensionData", ArgumentSemantic.Copy)]
		NSData ProjectExtensionData { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 14)]
		[Export ("setKeyAsset:")]
		void SetKeyAsset ([NullAllowed] PHAsset keyAsset);

		[Export ("setProjectPreviewImage:")]
		void SetProjectPreviewImage (NSImage previewImage);

		[Export ("removeAssets:")]
		void RemoveAssets (PHAsset [] /*id<NSFastEnumeration>*/ assets); //a collection of PHAsset objects
	}

	[TV (15, 0)]
	[iOS (15, 0)]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHCloudIdentifier : NSSecureCoding {

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'PHPhotosError.IdentifierNotFound' instead.")]
		[NoTV, NoiOS, NoMacCatalyst]
		[Static]
		[Export ("notFoundIdentifier")]
		PHCloudIdentifier NotFoundIdentifier { get; }

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("initWithStringValue:")]
		NativeHandle Constructor (string stringValue);
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHCloudIdentifierMapping {
		[NullAllowed, Export ("cloudIdentifier")]
		PHCloudIdentifier CloudIdentifier { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }
	}

	[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHLocalIdentifierMapping {
		[NullAllowed, Export ("localIdentifier")]
		string LocalIdentifier { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }
	}

	[TV (16, 0), Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHPersistentChange {
		[Export ("changeToken")]
		PHPersistentChangeToken ChangeToken { get; }

		[Export ("changeDetailsForObjectType:error:")]
		[return: NullAllowed]
		PHPersistentObjectChangeDetails GetChangeDetails (PHObjectType objectType, [NullAllowed] out NSError error);
	}

	delegate void PHPersistentChangeFetchResultEnumerator (PHPersistentChange change, ref bool stop);


	[TV (16, 0), Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHPersistentChangeFetchResult {
		[Export ("enumerateChangesWithBlock:")]
		void EnumerateChanges (PHPersistentChangeFetchResultEnumerator block);
	}

	[TV (16, 0), Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHPersistentChangeToken : NSCopying, NSSecureCoding {
	}

	[TV (16, 0), Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PHPersistentObjectChangeDetails {
		[Export ("objectType")]
		PHObjectType ObjectType { get; }

		[Export ("insertedLocalIdentifiers", ArgumentSemantic.Strong)]
		NSSet<NSString> InsertedLocalIdentifiers { get; }

		[Export ("updatedLocalIdentifiers", ArgumentSemantic.Strong)]
		NSSet<NSString> UpdatedLocalIdentifiers { get; }

		[Export ("deletedLocalIdentifiers", ArgumentSemantic.Strong)]
		NSSet<NSString> DeletedLocalIdentifiers { get; }
	}

}
