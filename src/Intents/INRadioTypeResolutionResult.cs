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
#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				return SuccessWithResolvedRadioType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
#endif
		}

		public static INRadioTypeResolutionResult GetConfirmationRequired (INRadioType valueToConfirm)
		{
#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (11, 0))
				return ConfirmationRequiredWithRadioTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
#endif
		}
	}
}
#endif
