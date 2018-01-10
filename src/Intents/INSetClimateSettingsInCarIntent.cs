#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetClimateSettingsInCarIntent {

		public INSetClimateSettingsInCarIntent (bool? enableFan, bool? enableAirConditioner, bool? enableClimateControl, bool? enableAutoMode, INCarAirCirculationMode airCirculationMode, NSNumber fanSpeedIndex, NSNumber fanSpeedPercentage, INRelativeSetting relativeFanSpeedSetting, NSMeasurement<NSUnitTemperature> temperature, INRelativeSetting relativeTemperatureSetting, INCarSeat climateZone) :
			this (enableFan.HasValue ? new NSNumber (enableFan.Value) : null, enableAirConditioner.HasValue ? new NSNumber (enableAirConditioner.Value) : null, 
				enableClimateControl.HasValue ? new NSNumber (enableClimateControl.Value) : null, enableAutoMode.HasValue ? new NSNumber (enableAutoMode.Value) : null,
				airCirculationMode, fanSpeedIndex, fanSpeedPercentage, relativeFanSpeedSetting, temperature, relativeTemperatureSetting, climateZone)
		{
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? EnableFan {
			get { return _EnableFan == null ? null : (bool?) _EnableFan.BoolValue; }
		}

		public bool? EnableAirConditioner {
			get { return _EnableAirConditioner == null ? null : (bool?) _EnableAirConditioner.BoolValue; }
		}

		public bool? EnableClimateControl {
			get { return _EnableClimateControl == null ? null : (bool?) _EnableClimateControl.BoolValue; }
		}

		public bool? EnableAutoMode {
			get { return _EnableAutoMode == null ? null : (bool?) _EnableAutoMode.BoolValue; }
		}
	}
}

#endif
