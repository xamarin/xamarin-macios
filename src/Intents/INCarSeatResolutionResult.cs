//
// INCarSeatResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && !MONOMAC
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INCarSeatResolutionResult {
		public static INCarSeatResolutionResult GetSuccess (INCarSeat resolvedValue)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return SuccessWithResolvedCarSeat (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INCarSeatResolutionResult GetConfirmationRequired (INCarSeat valueToConfirm)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return ConfirmationRequiredWithCarSeatToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif