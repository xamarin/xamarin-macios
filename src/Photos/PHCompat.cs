// Copyright 2016 Xamarin Inc. All rights reserved.

#if !XAMCORE_3_0 && !MONOMAC

using System;

namespace XamCore.Photos {
	
	public partial class PHContentEditingInputRequestOptions {

		[Obsolete ("Use CanHandleAdjustmentData property")]
		public virtual void SetCanHandleAdjustmentDataHandler (Func<PHAdjustmentData,bool> canHandleAdjustmentDataPredicate)
		{
			CanHandleAdjustmentData = canHandleAdjustmentDataPredicate;
		}

		[Obsolete ("Use ProgressHandler property")]
		public virtual void SetProgressHandler (PHProgressHandler progressHandler)
		{
			ProgressHandler = progressHandler;
		}
	}
}

#endif
