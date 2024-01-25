// Copyright 2016 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation

#nullable enable

using System;

using CoreImage;

using Foundation;

using ImageIO;

using ObjCRuntime;

namespace Photos {

#if !XAMCORE_3_0 && !MONOMAC
	public partial class PHContentEditingInputRequestOptions {

		[Obsolete ("Use 'CanHandleAdjustmentData' property.")]
		public virtual void SetCanHandleAdjustmentDataHandler (Func<PHAdjustmentData, bool> canHandleAdjustmentDataPredicate)
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

#if !NET
	// incorrect signature, should have been `ref NSError`
	[Obsolete ("Use 'PHLivePhotoFrameProcessingBlock2' instead.")]
	public delegate CIImage PHLivePhotoFrameProcessingBlock (IPHLivePhotoFrame frame, NSError error);

	public partial class PHLivePhotoEditingContext {

		[Obsolete ("Use 'FrameProcessor2' instead.", true)]
		public virtual PHLivePhotoFrameProcessingBlock? FrameProcessor { get; set; }
	}

#if MONOMAC
	public partial class PHAssetCollection {

		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
		public static PHFetchResult? FetchMoments (PHFetchOptions options)
		{
			return null;
		}

		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
		public static PHFetchResult? FetchMoments (PHCollectionList momentList, PHFetchOptions options)
		{
			return null;
		}
	}

	public partial class PHCollectionList {

		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
		public static PHFetchResult? FetchMomentLists (PHCollectionListSubtype subType, PHFetchOptions options)
		{
			return null;
		}

		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX)]
		public static PHFetchResult? FetchMomentLists (PHCollectionListSubtype subType, PHAssetCollection moment, PHFetchOptions options)
		{
			return null;
		}
	}

	public partial class PHContentEditingInput {
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX, message: "Use 'AudiovisualAsset' instead.")]
		public virtual AVFoundation.AVAsset? AvAsset {
			get { return AudiovisualAsset; }
		}
	}

	public delegate void PHImageDataHandler (NSData data, NSString dataUti, CGImagePropertyOrientation orientation, NSDictionary info);

	public partial class PHImageManager {
		[Obsolete ("Compatibility stub - This was marked as unavailable on macOS with Xcode 11.")]
		[Unavailable (PlatformName.MacOSX, message: "Use 'RequestImageDataAndOrientation (PHAsset asset, [NullAllowed] PHImageRequestOptions options, PHImageManagerRequestImageDataHandler resultHandler)' instead.")]
		public virtual int RequestImageData (PHAsset asset, PHImageRequestOptions options, PHImageDataHandler handler)
		{
			return -1;
		}
	}
#endif // MONOMAC

#endif // !NET
}
