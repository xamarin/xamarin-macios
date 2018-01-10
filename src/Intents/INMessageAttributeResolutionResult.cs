//
// INMessageAttributeResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INMessageAttributeResolutionResult {

		public static INMessageAttributeResolutionResult GetSuccess (INMessageAttribute resolvedValue)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#elif MONOMAC
			if (PlatformHelper.CheckSystemVersion (10, 13))
#endif
				return SuccessWithResolvedMessageAttribute (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INMessageAttributeResolutionResult GetConfirmationRequired (INMessageAttribute valueToConfirm)
		{
#if IOS
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
#elif WATCH
			if (WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (4, 0))
#elif MONOMAC
			if (PlatformHelper.CheckSystemVersion (10, 13))
#endif
				return ConfirmationRequiredWithMessageAttributeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
