//
// Extensions to the ALAssetsLibrary class
//
// Authors:
//   Rolf Bjarne Kvinge
//
// Copyright 2011-2014, Xamarin Inc.
//

#if !XAMCORE_2_0

using System;

using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using MediaPlayer;

namespace AssetsLibrary
{
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Photos' API instead.")]
	public partial class ALAssetsLibrary
	{
		[Obsolete ("Use the overload that takes a CGImage instead")]
		public unsafe virtual void WriteImageToSavedPhotosAlbum (UIImage imageData, NSDictionary metadata, ALAssetsLibraryWriteCompletionDelegate completionBlock)
		{
			WriteImageToSavedPhotosAlbum (imageData.CGImage, metadata, completionBlock);
		}
		
		[Obsolete ("Use the overload that takes a CGImage instead")]
		public unsafe virtual void WriteImageToSavedPhotosAlbum (UIImage imageData, ALAssetOrientation orientation, ALAssetsLibraryWriteCompletionDelegate completionBlock)
		{
			WriteImageToSavedPhotosAlbum (imageData.CGImage, orientation, completionBlock);
		}
	}
}

#endif