//
// INMessageAttributeOptionsResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !(NET && __MACOS__)
#if !TVOS
using System;
using Foundation;
using ObjCRuntime;

namespace Intents {
	public partial class INMessageAttributeOptionsResolutionResult {

		public static INMessageAttributeOptionsResolutionResult GetSuccess (INMessageAttributeOptions resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return SuccessWithResolvedMessageAttributeOptions (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INMessageAttributeOptionsResolutionResult GetConfirmationRequired (INMessageAttributeOptions valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return ConfirmationRequiredWithMessageAttributeOptionsToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
#endif // !(NET && __MACOS__)
