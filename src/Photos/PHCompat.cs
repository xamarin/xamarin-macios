// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using XamCore.CoreImage;
using XamCore.Foundation;

namespace XamCore.Photos {
	
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
#endif
}
