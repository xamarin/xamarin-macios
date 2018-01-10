#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetSeatSettingsInCarIntent {

		public INSetSeatSettingsInCarIntent (bool? enableHeating, bool? enableCooling, bool? enableMassage, INCarSeat seat, NSNumber level, INRelativeSetting relativeLevelSetting) :
			this (enableHeating.HasValue ? new NSNumber (enableHeating.Value) : null, enableCooling.HasValue ? new NSNumber (enableCooling.Value) : null, enableMassage.HasValue ? new NSNumber (enableMassage.Value) : null, seat, level, relativeLevelSetting)
		{
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? EnableCooling {
			get { return _EnableCooling == null ? null : (bool?) _EnableCooling.BoolValue; }
		}

		public bool? EnableHeating {
			get { return _EnableHeating == null ? null : (bool?) _EnableHeating.BoolValue; }
		}

		public bool? EnableMassage {
			get { return _EnableMassage == null ? null : (bool?) _EnableMassage.BoolValue; }
		}
	}
}

#endif
