#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetSeatSettingsInCarIntent {

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		public INSetSeatSettingsInCarIntent (bool? enableHeating, bool? enableCooling, bool? enableMassage, INCarSeat seat, NSNumber level, INRelativeSetting relativeLevelSetting) :
			this (enableHeating.HasValue ? new NSNumber (enableHeating.Value) : null, enableCooling.HasValue ? new NSNumber (enableCooling.Value) : null, enableMassage.HasValue ? new NSNumber (enableMassage.Value) : null, seat, level, relativeLevelSetting)
		{
		}
	}
}

#endif
