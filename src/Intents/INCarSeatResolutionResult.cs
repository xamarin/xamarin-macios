//
// INCarSeatResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && IOS
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.Intents {
	public partial class INCarSeatResolutionResult {
		public static INCarSeatResolutionResult GetSuccess (INCarSeat resolvedValue)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				return SuccessWithResolvedCarSeat (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INCarSeatResolutionResult GetConfirmationRequired (INCarSeat valueToConfirm)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				return ConfirmationRequiredWithCarSeatToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif