//
// INPaymentStatusResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && !MONOMAC
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.Intents {
	public partial class INPaymentStatusResolutionResult {

		public static INPaymentStatusResolutionResult GetSuccess (INPaymentStatus resolvedValue)
		{
#if IOS
			if (XamCore.UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (XamCore.WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return SuccessWithResolvedPaymentStatus (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INPaymentStatusResolutionResult GetConfirmationRequired (INPaymentStatus valueToConfirm)
		{
#if IOS
			if (XamCore.UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (XamCore.WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return ConfirmationRequiredWithPaymentStatusToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
