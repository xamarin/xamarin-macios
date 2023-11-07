//
// INCallRecordTypeResolutionResult.cs
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
	public partial class INCallRecordTypeResolutionResult {

		public static INCallRecordTypeResolutionResult GetSuccess (INCallRecordType resolvedValue)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return SuccessWithResolvedCallRecordType (resolvedValue);
			else
				return SuccessWithResolvedValue (resolvedValue);
		}

		public static INCallRecordTypeResolutionResult GetConfirmationRequired (INCallRecordType valueToConfirm)
		{
#if IOS
			if (SystemVersion.CheckiOS (11, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (4, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (10, 13))
#endif
			return ConfirmationRequiredWithCallRecordTypeToConfirm (valueToConfirm);
			else
				return ConfirmationRequiredWithValueToConfirm (valueToConfirm);
		}
	}
}
#endif
