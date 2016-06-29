using System;
using System.Threading.Tasks;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[iOS (8,0)]
	public partial class HMService {

		public HMServiceType ServiceType {
			get {
				var s = _ServiceType;
				// safety in case the field does not exists / cannot be loaded / new in future iOS versions...
				if (s == null)
					return HMServiceType.None;

				if (s == HMServiceTypeInternal.LightBulb)
					return HMServiceType.LightBulb;
				if (s == HMServiceTypeInternal.Switch)
					return HMServiceType.Switch;
				if (s == HMServiceTypeInternal.Thermostat)
					return HMServiceType.Thermostat;
				if (s == HMServiceTypeInternal.GarageDoorOpener)
					return HMServiceType.GarageDoorOpener;
				if (s == HMServiceTypeInternal.AccessoryInformation)
					return HMServiceType.AccessoryInformation;
				if (s == HMServiceTypeInternal.Fan)
					return HMServiceType.Fan;
				if (s == HMServiceTypeInternal.Outlet)
					return HMServiceType.Outlet;
				if (s == HMServiceTypeInternal.LockMechanism)
					return HMServiceType.LockMechanism;
				if (s == HMServiceTypeInternal.LockManagement)
					return HMServiceType.LockManagement;
				// iOS 9
				if (s == HMServiceTypeInternal.AirQualitySensor)
					return HMServiceType.AirQualitySensor;
				if (s == HMServiceTypeInternal.Battery)
					return HMServiceType.Battery;
				if (s == HMServiceTypeInternal.CarbonDioxideSensor)
					return HMServiceType.CarbonDioxideSensor;
				if (s == HMServiceTypeInternal.CarbonMonoxideSensor)
					return HMServiceType.CarbonMonoxideSensor;
				if (s == HMServiceTypeInternal.ContactSensor)
					return HMServiceType.ContactSensor;
				if (s == HMServiceTypeInternal.Door)
					return HMServiceType.Door;
				if (s == HMServiceTypeInternal.HumiditySensor)
					return HMServiceType.HumiditySensor;
				if (s == HMServiceTypeInternal.LeakSensor)
					return HMServiceType.LeakSensor;
				if (s == HMServiceTypeInternal.LightSensor)
					return HMServiceType.LightSensor;
				if (s == HMServiceTypeInternal.MotionSensor)
					return HMServiceType.MotionSensor;
				if (s == HMServiceTypeInternal.OccupancySensor)
					return HMServiceType.OccupancySensor;
				if (s == HMServiceTypeInternal.SecuritySystem)
					return HMServiceType.SecuritySystem;
				if (s == HMServiceTypeInternal.StatefulProgrammableSwitch)
					return HMServiceType.StatefulProgrammableSwitch;
				if (s == HMServiceTypeInternal.StatelessProgrammableSwitch)
					return HMServiceType.StatelessProgrammableSwitch;
				if (s == HMServiceTypeInternal.SmokeSensor)
					return HMServiceType.SmokeSensor;
				if (s == HMServiceTypeInternal.TemperatureSensor)
					return HMServiceType.TemperatureSensor;
				if (s == HMServiceTypeInternal.Window)
					return HMServiceType.Window;
				if (s == HMServiceTypeInternal.WindowCovering)
					return HMServiceType.WindowCovering;
				// iOS 10
				if (s == HMServiceTypeInternal.CameraRtpStreamManagement)
					return HMServiceType.CameraRtpStreamManagement;
				if (s == HMServiceTypeInternal.CameraControl)
					return HMServiceType.CameraControl;
				if (s == HMServiceTypeInternal.Microphone)
					return HMServiceType.Microphone;
				if (s == HMServiceTypeInternal.Speaker)
					return HMServiceType.Speaker;

				return HMServiceType.None;
			}
		}

		NSString GetName (HMServiceType serviceType)
		{
			switch (serviceType) {
			case HMServiceType.LightBulb:
				return HMServiceTypeInternal.LightBulb;
			case HMServiceType.Switch:
				return HMServiceTypeInternal.Switch;
			case HMServiceType.Thermostat:
				return HMServiceTypeInternal.Thermostat;
			case HMServiceType.GarageDoorOpener:
				return HMServiceTypeInternal.GarageDoorOpener;
			case HMServiceType.AccessoryInformation:
				return HMServiceTypeInternal.AccessoryInformation;
			case HMServiceType.Fan:
				return HMServiceTypeInternal.Fan;
			case HMServiceType.Outlet:
				return HMServiceTypeInternal.Outlet;
			case HMServiceType.LockMechanism:
				return HMServiceTypeInternal.LockMechanism;
			case HMServiceType.LockManagement:
				return HMServiceTypeInternal.LockManagement;
			// iOS 9
			case HMServiceType.AirQualitySensor:
				return HMServiceTypeInternal.AirQualitySensor;
			case HMServiceType.Battery:
				return HMServiceTypeInternal.Battery;
			case HMServiceType.CarbonDioxideSensor:
				return HMServiceTypeInternal.CarbonDioxideSensor;
			case HMServiceType.CarbonMonoxideSensor:
				return HMServiceTypeInternal.CarbonMonoxideSensor;
			case HMServiceType.ContactSensor:
				return HMServiceTypeInternal.ContactSensor;
			case HMServiceType.Door:
				return HMServiceTypeInternal.Door;
			case HMServiceType.HumiditySensor:
				return HMServiceTypeInternal.HumiditySensor;
			case HMServiceType.LeakSensor:
				return HMServiceTypeInternal.LeakSensor;
			case HMServiceType.LightSensor:
				return HMServiceTypeInternal.LightSensor;
			case HMServiceType.MotionSensor:
				return HMServiceTypeInternal.MotionSensor;
			case HMServiceType.OccupancySensor:
				return HMServiceTypeInternal.OccupancySensor;
			case HMServiceType.SecuritySystem:
				return HMServiceTypeInternal.SecuritySystem;
			case HMServiceType.StatefulProgrammableSwitch:
				return HMServiceTypeInternal.StatefulProgrammableSwitch;
			case HMServiceType.StatelessProgrammableSwitch:
				return HMServiceTypeInternal.StatelessProgrammableSwitch;
			case HMServiceType.SmokeSensor:
				return HMServiceTypeInternal.SmokeSensor;
			case HMServiceType.TemperatureSensor:
				return HMServiceTypeInternal.TemperatureSensor;
			case HMServiceType.Window:
				return HMServiceTypeInternal.Window;
			case HMServiceType.WindowCovering:
				return HMServiceTypeInternal.WindowCovering;
			default:
				return null;
			}
		}

#if !WATCH
		public void UpdateAssociatedServiceType (HMServiceType serviceType, Action<NSError> completion)
		{
			UpdateAssociatedServiceType (GetName (serviceType), completion);
		}

		public Task UpdateAssociatedServiceTypeAsync (HMServiceType serviceType)
		{
			return UpdateAssociatedServiceTypeAsync (GetName (serviceType));
		}

#if !XAMCORE_3_0
		[Obsolete]
		public Task UpdateNameAsync (HMServiceType serviceType)
		{
			return UpdateNameAsync (GetName (serviceType));
		}
#endif
#endif
	}
}