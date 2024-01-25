//
// INMessageAttributeResolutionResult.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if !TVOS
using System;

using Foundation;

using ObjCRuntime;

namespace Intents {
#if !(NET && __MACOS__)
	public partial class INMessageAttributeResolutionResult {

		public static INMessageAttributeResolutionResult GetSuccess (INMessageAttribute resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return SuccessWithResolvedMessageAttribute (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INMessageAttributeResolutionResult GetConfirmationRequired (INMessageAttribute valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return ConfirmationRequiredWithMessageAttributeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
#endif // !(NET && __MACOS__)
}
#endif
