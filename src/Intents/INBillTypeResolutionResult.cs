//
// INBillTypeResolutionResult.cs
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
	public partial class INBillTypeResolutionResult {

		public static INBillTypeResolutionResult GetSuccess (INBillType resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return SuccessWithResolvedBillType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INBillTypeResolutionResult GetConfirmationRequired (INBillType valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return ConfirmationRequiredWithBillTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
