//
// INCarDefrosterResolutionResult.cs
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
	public partial class INCarDefrosterResolutionResult {

		public static INCarDefrosterResolutionResult GetSuccess (INCarDefroster resolvedValue)
		{
#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return SuccessWithResolvedCarDefroster (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
#endif
		}

		public static INCarDefrosterResolutionResult GetConfirmationRequired (INCarDefroster valueToConfirm)
		{
#if __WATCHOS__
			throw new PlatformNotSupportedException ("This class is not supported on watchOS");
#elif __IOS__
			if (SystemVersion.CheckiOS (11, 0))
				return ConfirmationRequiredWithCarDefrosterToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
#endif
		}
	}
}
#endif
