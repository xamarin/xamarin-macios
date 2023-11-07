//
// INPaymentStatusResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !MONOMAC && !TVOS
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INPaymentStatusResolutionResult {

		public static INPaymentStatusResolutionResult GetSuccess (INPaymentStatus resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return SuccessWithResolvedPaymentStatus (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INPaymentStatusResolutionResult GetConfirmationRequired (INPaymentStatus valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return ConfirmationRequiredWithPaymentStatusToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
