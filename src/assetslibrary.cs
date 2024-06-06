//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2011-2014, Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using MediaPlayer;
using System;

namespace AssetsLibrary {

	/// <summary>Provides data for the  event.</summary>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	interface ALAssetLibraryChangedEventArgs {
		[Export ("ALAssetLibraryUpdatedAssetsKey")]
		NSSet UpdatedAssets { get; }

		[Export ("ALAssetLibraryInsertedAssetGroupsKey")]
		NSSet InsertedAssetGroups { get; }

		[Export ("ALAssetLibraryUpdatedAssetGroupsKey")]
		NSSet UpdatedAssetGroups { get; }

		[Export ("ALAssetLibraryDeletedAssetGroupsKey")]
		NSSet DeletedAssetGroupsKey { get; }
	}

	/// <summary>A class that encapsulates access to the video and media of the "Photos" application.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AssetsLibrary/Reference/ALAssetsLibrary_Class/index.html">Apple documentation for <c>ALAssetsLibrary</c></related>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAssetsLibrary {
		[Export ("assetForURL:resultBlock:failureBlock:")]
		void AssetForUrl (NSUrl assetURL, Action<ALAsset> resultBlock, Action<NSError> failureBlock);

		[Export ("enumerateGroupsWithTypes:usingBlock:failureBlock:")]
		void Enumerate (ALAssetsGroupType types, ALAssetsLibraryGroupsEnumerationResultsDelegate enumerationBlock, Action<NSError> failureBlock);

		[Export ("videoAtPathIsCompatibleWithSavedPhotosAlbum:")]
		bool VideoAtPathIsIsCompatibleWithSavedPhotosAlbum (NSUrl videoPathURL);

		[Export ("writeImageDataToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
		void WriteImageToSavedPhotosAlbum (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("writeImageToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
		void WriteImageToSavedPhotosAlbum (CGImage imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("writeImageToSavedPhotosAlbum:orientation:completionBlock:")]
		[Async]
		void WriteImageToSavedPhotosAlbum (CGImage imageData, ALAssetOrientation orientation, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("writeVideoAtPathToSavedPhotosAlbum:completionBlock:")]
		[Async]
		void WriteVideoToSavedPhotosAlbum (NSUrl videoPathURL, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Field ("ALAssetsLibraryChangedNotification")]
		[Notification]
		[Notification (typeof (ALAssetLibraryChangedEventArgs))]
		NSString ChangedNotification { get; }

		[Export ("groupForURL:resultBlock:failureBlock:")]
		void GroupForUrl (NSUrl groupURL, Action<ALAssetsGroup> resultBlock, Action<NSError> failureBlock);

		[Export ("addAssetsGroupAlbumWithName:resultBlock:failureBlock:")]
		void AddAssetsGroupAlbum (string name, Action<ALAssetsGroup> resultBlock, Action<NSError> failureBlock);

		[Static]
		[Export ("authorizationStatus")]
		ALAuthorizationStatus AuthorizationStatus { get; }

		[Static]
		[Export ("disableSharedPhotoStreamsSupport")]
		void DisableSharedPhotoStreamsSupport ();

		[Field ("ALAssetLibraryUpdatedAssetsKey")]
		NSString UpdatedAssetsKey { get; }

		[Field ("ALAssetLibraryInsertedAssetGroupsKey")]
		NSString InsertedAssetGroupsKey { get; }

		[Field ("ALAssetLibraryUpdatedAssetGroupsKey")]
		NSString UpdatedAssetGroupsKey { get; }

		[Field ("ALAssetLibraryDeletedAssetGroupsKey")]
		NSString DeletedAssetGroupsKey { get; }
	}

	/// <summary>A delegate that is used as the <c>enumerationBlock</c> parameter in calls to the <see cref="M:AssetsLibrary.ALAssetsLibrary.Enumerate(AssetsLibrary.ALAssetsGroupType,AssetsLibrary.ALAssetsLibraryGroupsEnumerationResultsDelegate,System.Action{Foundation.NSError})" /> method.</summary>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryGroupsEnumerationResultsDelegate (ALAssetsGroup group, ref bool stop);

	/// <summary>An asset managed by the Photo application (videos and photos).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AssetsLibrary/Reference/ALAsset_Class/index.html">Apple documentation for <c>ALAsset</c></related>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAsset {
		[Export ("valueForProperty:")]
		NSObject ValueForProperty (NSString property);

		[Export ("defaultRepresentation")]
		ALAssetRepresentation DefaultRepresentation { get; }

		[Export ("representationForUTI:")]
		ALAssetRepresentation RepresentationForUti (string uti);

		[Export ("thumbnail")]
		CGImage Thumbnail { get; }

		[Field ("ALAssetPropertyType"), Internal]
		NSString _PropertyType { get; }

		[Field ("ALAssetPropertyLocation"), Internal]
		NSString _PropertyLocation { get; }

		[Field ("ALAssetPropertyDuration"), Internal]
		NSString _PropertyDuration { get; }

		[Field ("ALAssetPropertyOrientation"), Internal]
		NSString _PropertyOrientation { get; }

		[Field ("ALAssetPropertyDate"), Internal]
		NSString _PropertyDate { get; }

		[Field ("ALAssetPropertyRepresentations"), Internal]
		NSString _PropertyRepresentations { get; }

		[Field ("ALAssetPropertyURLs"), Internal]
		NSString _PropertyURLs { get; }

		[Field ("ALAssetPropertyAssetURL"), Internal]
		NSString _PropertyAssetURL { get; }

		[Field ("ALAssetTypePhoto"), Internal]
		NSString _TypePhoto { get; }

		[Field ("ALAssetTypeVideo"), Internal]
		NSString _TypeVideo { get; }

		[Field ("ALAssetTypeUnknown"), Internal]
		NSString _TypeUnknown { get; }

		[Export ("originalAsset")]
		ALAsset OriginalAsset { get; }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; }

		[Export ("aspectRatioThumbnail")]
		CGImage AspectRatioThumbnail ();

		[Export ("writeModifiedImageDataToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
		void WriteModifiedImageToSavedToPhotosAlbum (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("writeModifiedVideoAtPathToSavedPhotosAlbum:completionBlock:")]
		[Async]
		void WriteModifiedVideoToSavedPhotosAlbum (NSUrl videoPathURL, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("setImageData:metadata:completionBlock:")]
		[Async]
		void SetImageData (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl, NSError> completionBlock);

		[Export ("setVideoAtPath:completionBlock:")]
		[Async]
		void SetVideoAtPath (NSUrl videoPathURL, [NullAllowed] Action<NSUrl, NSError> completionBlock);
	}

	/// <summary>A specific representation of an asset.</summary>
	///     <remarks>Some assets can have more than one representation.   Consider images that are stored in two different formats for example, this class represents a particular reprensetation of the asset.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AssetsLibrary/Reference/ALAssetRepresentation_Class/index.html">Apple documentation for <c>ALAssetRepresentation</c></related>
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	interface ALAssetRepresentation {
		[Export ("UTI")]
		string Uti { get; }

		[Export ("size")]
		long Size { get; }

		[Export ("getBytes:fromOffset:length:error:")]
		nuint GetBytes (IntPtr buffer, long offset, nuint length, out NSError error);

		[Export ("fullResolutionImage")]
		[Autorelease]
		CGImage GetImage ();

		[Export ("CGImageWithOptions:")]
		[Autorelease]
		CGImage GetImage (NSDictionary options);

		[Export ("fullScreenImage")]
		[Autorelease]
		CGImage GetFullScreenImage ();

		[Export ("url")]
		NSUrl Url { get; }

		[Export ("metadata")]
		NSDictionary Metadata { get; }

		[Export ("orientation")]
		ALAssetOrientation Orientation { get; }

		[Export ("scale")]
		float Scale { get; } /* float, not CGFloat */

		[Export ("filename")]
		string Filename { get; }

		[Export ("dimensions")]
		CGSize Dimensions { get; }
	}

	/// <summary>Keys used to limit asset enumeration by a specific kind.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AssetsLibrary/Reference/ALAssetsFilter_Class/index.html">Apple documentation for <c>ALAssetsFilter</c></related>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAssetsFilter {
		[Static, Export ("allPhotos")]
		ALAssetsFilter AllPhotos { get; }

		[Static, Export ("allVideos")]
		ALAssetsFilter AllVideos { get; }

		[Static, Export ("allAssets")]
		ALAssetsFilter AllAssets { get; }
	}

	/// <param name="result">The asset.</param>
	///     <param name="index">The index of this asset.</param>
	///     <param name="stop">If set to true, the enumeration process will stop.</param>
	///     <summary>Signature for delegates participating in asset enumeration.</summary>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	delegate void ALAssetsEnumerator (ALAsset result, nint index, ref bool stop);

	/// <summary>Sets of assets managed by the Photo application.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AssetsLibrary/Reference/ALAssetsGroup_Class/index.html">Apple documentation for <c>ALAssetsGroup</c></related>
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAssetsGroup {
		[Export ("valueForProperty:"), Internal]
		NSObject ValueForProperty (NSString property);

		[Export ("posterImage")]
		CGImage PosterImage { get; }

		[Export ("setAssetsFilter:")]
		void SetAssetsFilter (ALAssetsFilter filter);

		[Export ("numberOfAssets")]
		nint Count { get; }

		[Export ("enumerateAssetsUsingBlock:")]
		void Enumerate (ALAssetsEnumerator result);

		[Export ("enumerateAssetsWithOptions:usingBlock:")]
		void Enumerate (NSEnumerationOptions options, ALAssetsEnumerator result);

		[Export ("enumerateAssetsAtIndexes:options:usingBlock:")]
		void Enumerate (NSIndexSet indexSet, NSEnumerationOptions options, ALAssetsEnumerator result);

		[Field ("ALAssetsGroupPropertyName"), Internal]
		NSString _Name { get; }

		[Field ("ALAssetsGroupPropertyType"), Internal]
		NSString _Type { get; }

		[Field ("ALAssetsGroupPropertyPersistentID"), Internal]
		NSString _PersistentID { get; }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; }

		[Export ("addAsset:")]
		bool AddAsset (ALAsset asset);

		[Field ("ALAssetsGroupPropertyURL"), Internal]
		NSString _PropertyUrl { get; }
	}
}
