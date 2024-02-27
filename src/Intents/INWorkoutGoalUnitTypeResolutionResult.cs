//
// INWorkoutGoalUnitTypeResolutionResult.cs
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
	public partial class INWorkoutGoalUnitTypeResolutionResult {

		public static INWorkoutGoalUnitTypeResolutionResult GetSuccess (INWorkoutGoalUnitType resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return SuccessWithResolvedWorkoutGoalUnitType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INWorkoutGoalUnitTypeResolutionResult GetConfirmationRequired (INWorkoutGoalUnitType valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return ConfirmationRequiredWithWorkoutGoalUnitTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
