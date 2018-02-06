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

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
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
	
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAssetsLibrary {
		[Export ("assetForURL:resultBlock:failureBlock:")]
#if XAMCORE_2_0
		void AssetForUrl (NSUrl assetURL, Action<ALAsset> resultBlock, Action<NSError> failureBlock);
#else
		void AssetForUrl (NSUrl assetURL, ALAssetsLibraryAssetForURLResultDelegate resultBlock, ALAssetsLibraryAccessFailureDelegate failureBlock);
#endif

		[Export ("enumerateGroupsWithTypes:usingBlock:failureBlock:")]
#if XAMCORE_2_0
		void Enumerate (ALAssetsGroupType types, ALAssetsLibraryGroupsEnumerationResultsDelegate enumerationBlock, Action<NSError> failureBlock);
#else
		void Enumerate (ALAssetsGroupType types, ALAssetsLibraryGroupsEnumerationResultsDelegate enumerationBlock, ALAssetsLibraryAccessFailureDelegate failureBlock);
#endif

		[Export ("videoAtPathIsCompatibleWithSavedPhotosAlbum:")]
		bool VideoAtPathIsIsCompatibleWithSavedPhotosAlbum (NSUrl videoPathURL);

		[Export ("writeImageDataToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteImageToSavedPhotosAlbum (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteImageToSavedPhotosAlbum (NSData imageData, NSDictionary metadata, [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("writeImageToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteImageToSavedPhotosAlbum (CGImage imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteImageToSavedPhotosAlbum (CGImage imageData, NSDictionary metadata, [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("writeImageToSavedPhotosAlbum:orientation:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteImageToSavedPhotosAlbum (CGImage imageData, ALAssetOrientation orientation, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteImageToSavedPhotosAlbum (CGImage imageData, ALAssetOrientation orientation,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("writeVideoAtPathToSavedPhotosAlbum:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteVideoToSavedPhotosAlbum (NSUrl videoPathURL, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteVideoToSavedPhotosAlbum (NSUrl videoPathURL,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Field ("ALAssetsLibraryChangedNotification")]
		[Notification]
		[Notification (typeof (ALAssetLibraryChangedEventArgs))]
		NSString ChangedNotification { get; }

		[Export ("groupForURL:resultBlock:failureBlock:")]
#if XAMCORE_2_0
		void GroupForUrl (NSUrl groupURL, Action<ALAssetsGroup> resultBlock, Action<NSError> failureBlock);
#else
		void GroupForUrl (NSUrl groupURL, ALAssetsLibraryGroupResult resultBlock, ALAssetsLibraryAccessFailure failureBlock);
#endif

		[Export ("addAssetsGroupAlbumWithName:resultBlock:failureBlock:")]
#if XAMCORE_2_0
		void AddAssetsGroupAlbum (string name, Action<ALAssetsGroup> resultBlock, Action<NSError> failureBlock);
#else
		void AddAssetsGroupAlbum (string name, ALAssetsLibraryGroupResult resultBlock, ALAssetsLibraryAccessFailure failureBlock);
#endif

		[iOS (6,0)]
		[Static]
		[Export ("authorizationStatus")]
		ALAuthorizationStatus AuthorizationStatus { get; }

		[iOS (6,0)]
		[Static]
		[Export ("disableSharedPhotoStreamsSupport")]
		void DisableSharedPhotoStreamsSupport ();

		[iOS (6,0)]
		[Field ("ALAssetLibraryUpdatedAssetsKey")]
		NSString UpdatedAssetsKey { get; }

		[iOS (6,0)]
		[Field ("ALAssetLibraryInsertedAssetGroupsKey")]
		NSString InsertedAssetGroupsKey { get; }

		[iOS (6,0)]
		[Field ("ALAssetLibraryUpdatedAssetGroupsKey")]
		NSString UpdatedAssetGroupsKey { get; }

		[iOS (6,0)]
		[Field ("ALAssetLibraryDeletedAssetGroupsKey")]
		NSString DeletedAssetGroupsKey { get; }
	}

#if !XAMCORE_2_0
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryAssetForURLResultDelegate (ALAsset asset);
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryAccessFailureDelegate (NSError error);
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryWriteCompletionDelegate (NSUrl assetURL, NSError error);
#endif
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryGroupsEnumerationResultsDelegate (ALAssetsGroup group, ref bool stop);

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
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

		[iOS (6,0)]
		[Field ("ALAssetPropertyAssetURL"), Internal]
		NSString _PropertyAssetURL { get; }

		[Field ("ALAssetTypePhoto"), Internal]
		NSString _TypePhoto { get; }

		[Field ("ALAssetTypeVideo"), Internal]
		NSString _TypeVideo { get; }

		[Field ("ALAssetTypeUnknown"), Internal]
		NSString _TypeUnknown { get; }

		[Export ("originalAsset")]
		ALAsset OriginalAsset { get;  }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get;  }

		[Export ("aspectRatioThumbnail")]
		CGImage AspectRatioThumbnail ();

		[Export ("writeModifiedImageDataToSavedPhotosAlbum:metadata:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteModifiedImageToSavedToPhotosAlbum (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteModifiedImageToSavedToPhotosAlbum (NSData imageData, NSDictionary metadata,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("writeModifiedVideoAtPathToSavedPhotosAlbum:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void WriteModifiedVideoToSavedPhotosAlbum (NSUrl videoPathURL, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void WriteModifiedVideoToSavedPhotosAlbum (NSUrl videoPathURL,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("setImageData:metadata:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void SetImageData (NSData imageData, NSDictionary metadata, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void SetImageData (NSData imageData, NSDictionary metadata,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif

		[Export ("setVideoAtPath:completionBlock:")]
		[Async]
#if XAMCORE_2_0
		void SetVideoAtPath (NSUrl videoPathURL, [NullAllowed] Action<NSUrl,NSError> completionBlock);
#else
		void SetVideoAtPath (NSUrl videoPathURL,  [NullAllowed] ALAssetsLibraryWriteCompletionDelegate completionBlock);
#endif
	}

	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
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

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	[BaseType (typeof (NSObject))]
	interface ALAssetsFilter {
		[Static, Export ("allPhotos")]
		ALAssetsFilter AllPhotos { get; }

		[Static, Export ("allVideos")]
		ALAssetsFilter AllVideos { get; }

		[Static, Export ("allAssets")]
		ALAssetsFilter AllAssets { get; }
	}

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsEnumerator (ALAsset result, nint index, ref bool stop);

#if !XAMCORE_2_0
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryGroupResult (ALAssetsGroup group);

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
	delegate void ALAssetsLibraryAccessFailure (NSError error);
#endif

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the 'Photos' API instead.")]
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
		bool Editable { [Bind ("isEditable")] get;  }

		[Export ("addAsset:")]
		bool AddAsset (ALAsset asset);

		[Field ("ALAssetsGroupPropertyURL"), Internal]
		NSString _PropertyUrl { get; }
	}
}
