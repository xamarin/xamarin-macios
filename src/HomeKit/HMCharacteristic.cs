using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[iOS (8,0)]
	partial class HMCharacteristic 
	{
		public HMCharacteristicType CharacteristicType {
			get {
				var s = _CharacteristicType;
				// safety in case the field does not exists / cannot be loaded / new in future iOS versions...
				if (s == null)
					return HMCharacteristicType.None;
				if (s == HMCharacteristicTypeInternal.PowerState)
					return HMCharacteristicType.PowerState;				
				if (s == HMCharacteristicTypeInternal.Hue)
					return HMCharacteristicType.Hue;						
				if (s == HMCharacteristicTypeInternal.Saturation)
					return HMCharacteristicType.Saturation;
				if (s == HMCharacteristicTypeInternal.Brightness)
					return HMCharacteristicType.Brightness;
				if (s == HMCharacteristicTypeInternal.TemperatureUnits)
					return HMCharacteristicType.TemperatureUnits;
				if (s == HMCharacteristicTypeInternal.CurrentTemperature)
					return HMCharacteristicType.CurrentTemperature;
				if (s == HMCharacteristicTypeInternal.TargetTemperature)
					return HMCharacteristicType.TargetTemperature;
				if (s == HMCharacteristicTypeInternal.CurrentHeatingCooling)
					return HMCharacteristicType.CurrentHeatingCooling;
				if (s == HMCharacteristicTypeInternal.TargetHeatingCooling)
					return HMCharacteristicType.TargetHeatingCooling;
				if (s == HMCharacteristicTypeInternal.CoolingThreshold)
					return HMCharacteristicType.CoolingThreshold;
				if (s == HMCharacteristicTypeInternal.HeatingThreshold)
					return HMCharacteristicType.HeatingThreshold;
				if (s == HMCharacteristicTypeInternal.CurrentRelativeHumidity)
					return HMCharacteristicType.CurrentRelativeHumidity;
				if (s == HMCharacteristicTypeInternal.TargetRelativeHumidity)
					return HMCharacteristicType.TargetRelativeHumidity;
				if (s == HMCharacteristicTypeInternal.CurrentDoorState)
					return HMCharacteristicType.CurrentDoorState;
				if (s == HMCharacteristicTypeInternal.TargetDoorState)
					return HMCharacteristicType.TargetDoorState;
				if (s == HMCharacteristicTypeInternal.ObstructionDetected)
					return HMCharacteristicType.ObstructionDetected;
				if (s == HMCharacteristicTypeInternal.Name)
					return HMCharacteristicType.Name;
				if (s == HMCharacteristicTypeInternal.Manufacturer)
					return HMCharacteristicType.Manufacturer;
				if (s == HMCharacteristicTypeInternal.Model)
					return HMCharacteristicType.Model;
				if (s == HMCharacteristicTypeInternal.SerialNumber)
					return HMCharacteristicType.SerialNumber;
				if (s == HMCharacteristicTypeInternal.Identify)
					return HMCharacteristicType.Identify;
				if (s == HMCharacteristicTypeInternal.RotationDirection)
					return HMCharacteristicType.RotationDirection;
				if (s == HMCharacteristicTypeInternal.RotationSpeed)
					return HMCharacteristicType.RotationSpeed;
				if (s == HMCharacteristicTypeInternal.OutletInUse)
					return HMCharacteristicType.OutletInUse;
				if (s == HMCharacteristicTypeInternal.Version)
					return HMCharacteristicType.Version;
				if (s == HMCharacteristicTypeInternal.Logs)
					return HMCharacteristicType.Logs;
				if (s == HMCharacteristicTypeInternal.AudioFeedback)
					return HMCharacteristicType.AudioFeedback;
				if (s == HMCharacteristicTypeInternal.AdminOnlyAccess)
					return HMCharacteristicType.AdminOnlyAccess;
				if (s == HMCharacteristicTypeInternal.MotionDetected)
					return HMCharacteristicType.MotionDetected;
				if (s == HMCharacteristicTypeInternal.CurrentLockMechanismState)
					return HMCharacteristicType.CurrentLockMechanismState;
				if (s == HMCharacteristicTypeInternal.TargetLockMechanismState)
					return HMCharacteristicType.TargetLockMechanismState;
				if (s == HMCharacteristicTypeInternal.LockMechanismLastKnownAction)
					return HMCharacteristicType.LockMechanismLastKnownAction;
				if (s == HMCharacteristicTypeInternal.LockManagementControlPoint)
					return HMCharacteristicType.LockManagementControlPoint;
				if (s == HMCharacteristicTypeInternal.LockManagementAutoSecureTimeout)
					return HMCharacteristicType.LockManagementAutoSecureTimeout;
				// iOS 9
				if (s == HMCharacteristicTypeInternal.AirParticulateDensity)
					return HMCharacteristicType.AirParticulateDensity;
				if (s == HMCharacteristicTypeInternal.AirParticulateSize)
					return HMCharacteristicType.AirParticulateSize;
				if (s == HMCharacteristicTypeInternal.AirQuality)
					return HMCharacteristicType.AirQuality;
				if (s == HMCharacteristicTypeInternal.BatteryLevel)
					return HMCharacteristicType.BatteryLevel;
				if (s == HMCharacteristicTypeInternal.CarbonDioxideDetected)
					return HMCharacteristicType.CarbonDioxideDetected;
				if (s == HMCharacteristicTypeInternal.CarbonDioxideLevel)
					return HMCharacteristicType.CarbonDioxideLevel;
				if (s == HMCharacteristicTypeInternal.CarbonDioxidePeakLevel)
					return HMCharacteristicType.CarbonDioxidePeakLevel;
				if (s == HMCharacteristicTypeInternal.CarbonMonoxideDetected)
					return HMCharacteristicType.CarbonMonoxideDetected;
				if (s == HMCharacteristicTypeInternal.CarbonMonoxideLevel)
					return HMCharacteristicType.CarbonMonoxideLevel;
				if (s == HMCharacteristicTypeInternal.CarbonMonoxidePeakLevel)
					return HMCharacteristicType.CarbonMonoxidePeakLevel;
				if (s == HMCharacteristicTypeInternal.ChargingState)
					return HMCharacteristicType.ChargingState;
				if (s == HMCharacteristicTypeInternal.ContactState)
					return HMCharacteristicType.ContactState;
				if (s == HMCharacteristicTypeInternal.CurrentSecuritySystemState)
					return HMCharacteristicType.CurrentSecuritySystemState;
				if (s == HMCharacteristicTypeInternal.CurrentHorizontalTilt)
					return HMCharacteristicType.CurrentHorizontalTilt;
				if (s == HMCharacteristicTypeInternal.CurrentLightLevel)
					return HMCharacteristicType.CurrentLightLevel;
				if (s == HMCharacteristicTypeInternal.CurrentPosition)
					return HMCharacteristicType.CurrentPosition;
				if (s == HMCharacteristicTypeInternal.CurrentVerticalTilt)
					return HMCharacteristicType.CurrentVerticalTilt;
				if (s == HMCharacteristicTypeInternal.FirmwareVersion)
					return HMCharacteristicType.FirmwareVersion;
				if (s == HMCharacteristicTypeInternal.HardwareVersion)
					return HMCharacteristicType.HardwareVersion;
				if (s == HMCharacteristicTypeInternal.HoldPosition)
					return HMCharacteristicType.HoldPosition;
				if (s == HMCharacteristicTypeInternal.InputEvent)
					return HMCharacteristicType.InputEvent;
				if (s == HMCharacteristicTypeInternal.LeakDetected)
					return HMCharacteristicType.LeakDetected;
				if (s == HMCharacteristicTypeInternal.OccupancyDetected)
					return HMCharacteristicType.OccupancyDetected;
				if (s == HMCharacteristicTypeInternal.OutputState)
					return HMCharacteristicType.OutputState;
				if (s == HMCharacteristicTypeInternal.PositionState)
					return HMCharacteristicType.PositionState;
				if (s == HMCharacteristicTypeInternal.SmokeDetected)
					return HMCharacteristicType.SmokeDetected;
				if (s == HMCharacteristicTypeInternal.SoftwareVersion)
					return HMCharacteristicType.SoftwareVersion;
				if (s == HMCharacteristicTypeInternal.StatusActive)
					return HMCharacteristicType.StatusActive;
				if (s == HMCharacteristicTypeInternal.StatusFault)
					return HMCharacteristicType.StatusFault;
				if (s == HMCharacteristicTypeInternal.StatusJammed)
					return HMCharacteristicType.StatusJammed;
				if (s == HMCharacteristicTypeInternal.StatusLowBattery)
					return HMCharacteristicType.StatusLowBattery;
				if (s == HMCharacteristicTypeInternal.StatusTampered)
					return HMCharacteristicType.StatusTampered;
				if (s == HMCharacteristicTypeInternal.TargetSecuritySystemState)
					return HMCharacteristicType.TargetSecuritySystemState;
				if (s == HMCharacteristicTypeInternal.TargetHorizontalTilt)
					return HMCharacteristicType.TargetHorizontalTilt;
				if (s == HMCharacteristicTypeInternal.TargetPosition)
					return HMCharacteristicType.TargetPosition;
				if (s == HMCharacteristicTypeInternal.TargetVerticalTilt)
					return HMCharacteristicType.TargetVerticalTilt;
				// iOS 10.0
				if (s == HMCharacteristicTypeInternal.StreamingStatus)
					return HMCharacteristicType.StreamingStatus;
				if (s == HMCharacteristicTypeInternal.SetupStreamEndpoint)
					return HMCharacteristicType.SetupStreamEndpoint;
				if (s == HMCharacteristicTypeInternal.SupportedVideoStreamConfiguration)
					return HMCharacteristicType.SupportedVideoStreamConfiguration;
				if (s == HMCharacteristicTypeInternal.SupportedAudioStreamConfiguration)
					return HMCharacteristicType.SupportedAudioStreamConfiguration;
				if (s == HMCharacteristicTypeInternal.SupportedRtpConfiguration)
					return HMCharacteristicType.SupportedRtpConfiguration;
				if (s == HMCharacteristicTypeInternal.SelectedStreamConfiguration)
					return HMCharacteristicType.SelectedStreamConfiguration;
				if (s == HMCharacteristicTypeInternal.Volume)
					return HMCharacteristicType.Volume;
				if (s == HMCharacteristicTypeInternal.Mute)
					return HMCharacteristicType.Mute;
				if (s == HMCharacteristicTypeInternal.NightVision)
					return HMCharacteristicType.NightVision;
				if (s == HMCharacteristicTypeInternal.OpticalZoom)
					return HMCharacteristicType.OpticalZoom;
				if (s == HMCharacteristicTypeInternal.DigitalZoom)
					return HMCharacteristicType.DigitalZoom;
				if (s == HMCharacteristicTypeInternal.ImageRotation)
					return HMCharacteristicType.ImageRotation;
				if (s == HMCharacteristicTypeInternal.ImageMirroring)
					return HMCharacteristicType.ImageMirroring;

				return HMCharacteristicType.None;
			}
		}

		public bool SupportsEventNotification {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.SupportsEventNotification)
						return true;
				}
				return false;
			}
		}
		
		public bool Readable {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.Readable)
						return true;
				}
				return false;
			}
		}

		public bool Writable {
			get {
				foreach (var p in Properties){
					if (p == HMCharacteristicPropertyInternal.Writable)
						return true;
				}
				return false;
			}
		}

		[iOS (9,3)][Watch (2,2)]
		public bool Hidden {
			get {
				foreach (var p in Properties) {
					if (p == HMCharacteristicPropertyInternal.Hidden)
						return true;
				}
				return false;
			}
		}
	}
}