//
// INRadioTypeResolutionResult.cs
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
	public partial class INRadioTypeResolutionResult {

		public static INRadioTypeResolutionResult GetSuccess (INRadioType resolvedValue)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return SuccessWithResolvedRadioType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INRadioTypeResolutionResult GetConfirmationRequired (INRadioType valueToConfirm)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return ConfirmationRequiredWithRadioTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
