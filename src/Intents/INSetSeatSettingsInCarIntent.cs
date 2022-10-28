#if IOS
using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetSeatSettingsInCarIntent {

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios12.0")]
#if IOS
		[Obsolete ("Starting with ios12.0 use the overload that takes 'INSpeakableString carName'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		public INSetSeatSettingsInCarIntent (bool? enableHeating, bool? enableCooling, bool? enableMassage, INCarSeat seat, NSNumber level, INRelativeSetting relativeLevelSetting) :
			this (enableHeating.HasValue ? new NSNumber (enableHeating.Value) : null, enableCooling.HasValue ? new NSNumber (enableCooling.Value) : null, enableMassage.HasValue ? new NSNumber (enableMassage.Value) : null, seat, level, relativeLevelSetting)
		{
		}
	}
}

#endif
