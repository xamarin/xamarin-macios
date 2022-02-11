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
using System.Runtime.Versioning;

namespace Intents {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
#if MONOMAC
	[Obsolete ("Starting with macos10.0 unavailable on macOS, will be removed in the future.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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
