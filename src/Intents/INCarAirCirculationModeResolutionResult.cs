//
// INCarAirCirculationModeResolutionResult.cs
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
	public partial class INCarAirCirculationModeResolutionResult {

		public static INCarAirCirculationModeResolutionResult GetSuccess (INCarAirCirculationMode resolvedValue)
		{
#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return SuccessWithResolvedCarAirCirculationMode (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
#endif
		}

		public static INCarAirCirculationModeResolutionResult GetConfirmationRequired (INCarAirCirculationMode valueToConfirm)
		{

#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return ConfirmationRequiredWithCarAirCirculationModeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
#endif
		}
	}
}
#endif
