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
	public partial class INCarSignalOptionsResolutionResult {

		public static INCarSignalOptionsResolutionResult GetSuccess (INCarSignalOptions resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return SuccessWithResolvedCarSignalOptions (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INCarSignalOptionsResolutionResult GetConfirmationRequired (INCarSignalOptions valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#endif
			return ConfirmationRequiredWithCarSignalOptionsToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
