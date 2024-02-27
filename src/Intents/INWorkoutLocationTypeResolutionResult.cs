//
// INWorkoutLocationTypeResolutionResult.cs
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
	public partial class INWorkoutLocationTypeResolutionResult {

		public static INWorkoutLocationTypeResolutionResult GetSuccess (INWorkoutLocationType resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return SuccessWithResolvedWorkoutLocationType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INWorkoutLocationTypeResolutionResult GetConfirmationRequired (INWorkoutLocationType valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return ConfirmationRequiredWithWorkoutLocationTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
