//
// INCarSeatResolutionResult.cs
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
	public partial class INCarSeatResolutionResult {
		public static INCarSeatResolutionResult GetSuccess (INCarSeat resolvedValue)
		{
#if __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return SuccessWithResolvedCarSeat (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
#endif
		}

		public static INCarSeatResolutionResult GetConfirmationRequired (INCarSeat valueToConfirm)
		{
#if __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return ConfirmationRequiredWithCarSeatToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
#endif
		}
	}
}
#endif
