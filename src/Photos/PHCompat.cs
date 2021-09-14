// Copyright 2016 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

using System;
using CoreImage;
using Foundation;
using ImageIO;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace Photos {
	
#if !XAMCORE_3_0 && !MONOMAC
	public partial class PHContentEditingInputRequestOptions {

		[Obsolete ("Use 'CanHandleAdjustmentData' property.")]
		public virtual void SetCanHandleAdjustmentDataHandler (Func<PHAdjustmentData,bool> canHandleAdjustmentDataPredicate)
		{
			CanHandleAdjustmentData = canHandleAdjustmentDataPredicate;
		}

		[Obsolete ("Use 'ProgressHandler' property.")]
		public virtual void SetProgressHandler (PHProgressHandler progressHandler)
		{
			ProgressHandler = progressHandler;
		}
	}
#endif

#if !XAMCORE_4_0
	// incorrect signature, should have been `ref NSError`
	[Obsolete ("Use 'PHLivePhotoFrameProcessingBlock2' instead.")]
	public delegate CIImage PHLivePhotoFrameProcessingBlock (IPHLivePhotoFrame frame, NSError error);

	public partial class PHLivePhotoEditingContext {

		[Obsolete ("Use 'FrameProcessor2' instead.", true)]
		public virtual PHLivePhotoFrameProcessingBlock FrameProcessor { get; set; }
	}

#if MONOMAC
	public partial class PHAssetCollection {

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static PHFetchResult FetchMoments (PHFetchOptions options)
		{
			return null;
		}

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static PHFetchResult FetchMoments (PHCollectionList momentList, PHFetchOptions options)
		{
			return null;
		}
	}

	public partial class PHCollectionList {

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, PHFetchOptions options)
		{
			return null;
		}

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public static PHFetchResult FetchMomentLists (PHCollectionListSubtype subType, PHAssetCollection moment, PHFetchOptions options)
		{
			return null;
		}
	}

	public partial class PHContentEditingInput {

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX, message: "Use 'AudiovisualAsset' instead.")]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Use 'AudiovisualAsset' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual AVFoundation.AVAsset AvAsset {
			get { return AudiovisualAsset; }
		}
	}

	public delegate void PHImageDataHandler (NSData data, NSString dataUti, CGImagePropertyOrientation orientation, NSDictionary info);

	public partial class PHImageManager {

#if !NET
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX, message: "Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.")]
#else
		[UnsupportedOSPlatform ("macos")]
		[Obsolete ("Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public virtual int RequestImageData (PHAsset asset, PHImageRequestOptions options, PHImageDataHandler handler)
		{
			return -1;
		}
	}
#endif

#endif
}
