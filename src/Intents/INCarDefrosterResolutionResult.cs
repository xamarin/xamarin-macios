//
// INCarDefrosterResolutionResult.cs
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
	public partial class INCarDefrosterResolutionResult {

		public static INCarDefrosterResolutionResult GetSuccess (INCarDefroster resolvedValue)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return SuccessWithResolvedCarDefroster (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INCarDefrosterResolutionResult GetConfirmationRequired (INCarDefroster valueToConfirm)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#endif
				return ConfirmationRequiredWithCarDefrosterToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
