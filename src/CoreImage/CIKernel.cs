// 
// CIKernel.cs: CoreImgage CIKernel class
//
// Authors:
//	Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace CoreImage {

	// This Api is also available on iOS 9 according to headers but depends on CIFilter.Apply methods 
	// which are only available on Mac. Filled a radar:22524785 and Apple replied that setROISelector: 
	// is Mac Only https://trello.com/c/kpksFWto

#if MONOMAC
	public delegate CGRect CIKernelRoiHandler (int samplerIndex, CGRect destRect, NSObject userInfo);

	public partial class CIKernel {
		const string xamarinRegionOfSelectorName = "xamarinRegionOf:destRect:userInfo:";
		static readonly Selector xamarinRegionOfSelector = new Selector (xamarinRegionOfSelectorName);
		CIKernelRoiHandler roiHandler;

		public void SetRegionOfInterest (CIKernelRoiHandler handler)
		{
			if (handler is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handler));

			roiHandler = handler;

			SetRegionOfInterestSelector (xamarinRegionOfSelector);
		}

		[Export (xamarinRegionOfSelectorName)]
		[Preserve (Conditional = true)]
		CGRect FunctionDispatcher (int samplerIndex, CGRect destRect, NSObject userInfo)
		{
			return roiHandler (samplerIndex, destRect, userInfo);
		}
	}
#endif
}
