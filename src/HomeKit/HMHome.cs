using System;
using System.Collections.Generic;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[iOS (8,0)]
	[TV (10,0)]
	public partial class HMHome
	{
		public HMService [] GetServices (HMServiceType serviceTypes) 
		{
			var arr = new List<NSString> ();

			if ((serviceTypes & HMServiceType.LightBulb) == HMServiceType.LightBulb)			
				arr.Add (HMServiceTypeInternal.LightBulb);
			if ((serviceTypes & HMServiceType.Switch) == HMServiceType.Switch)			
				arr.Add (HMServiceTypeInternal.Switch);
			if ((serviceTypes & HMServiceType.Thermostat) == HMServiceType.Thermostat)			
				arr.Add (HMServiceTypeInternal.Thermostat);
			if ((serviceTypes & HMServiceType.GarageDoorOpener) == HMServiceType.GarageDoorOpener)			
				arr.Add (HMServiceTypeInternal.GarageDoorOpener);
			if ((serviceTypes & HMServiceType.AccessoryInformation) == HMServiceType.AccessoryInformation)			
				arr.Add (HMServiceTypeInternal.AccessoryInformation);
			if ((serviceTypes & HMServiceType.Fan) == HMServiceType.Fan)			
				arr.Add (HMServiceTypeInternal.Fan);
			if ((serviceTypes & HMServiceType.Outlet) == HMServiceType.Outlet)			
				arr.Add (HMServiceTypeInternal.Outlet);
			if ((serviceTypes & HMServiceType.LockMechanism) == HMServiceType.LockMechanism)			
				arr.Add (HMServiceTypeInternal.LockMechanism);
			if ((serviceTypes & HMServiceType.LockManagement) == HMServiceType.LockManagement)			
				arr.Add (HMServiceTypeInternal.LockManagement);
			// iOS 9
			if ((serviceTypes & HMServiceType.AirQualitySensor) == HMServiceType.AirQualitySensor)
				arr.Add (HMServiceTypeInternal.AirQualitySensor);
			if ((serviceTypes & HMServiceType.SecuritySystem) == HMServiceType.SecuritySystem)
				arr.Add (HMServiceTypeInternal.SecuritySystem);
			if ((serviceTypes & HMServiceType.CarbonMonoxideSensor) == HMServiceType.CarbonMonoxideSensor)
				arr.Add (HMServiceTypeInternal.CarbonMonoxideSensor);
			if ((serviceTypes & HMServiceType.ContactSensor) == HMServiceType.ContactSensor)
				arr.Add (HMServiceTypeInternal.ContactSensor);
			if ((serviceTypes & HMServiceType.Door) == HMServiceType.Door)
				arr.Add (HMServiceTypeInternal.Door);
			if ((serviceTypes & HMServiceType.HumiditySensor) == HMServiceType.HumiditySensor)
				arr.Add (HMServiceTypeInternal.HumiditySensor);
			if ((serviceTypes & HMServiceType.LeakSensor) == HMServiceType.LeakSensor)
				arr.Add (HMServiceTypeInternal.LeakSensor);
			if ((serviceTypes & HMServiceType.LightSensor) == HMServiceType.LightSensor)
				arr.Add (HMServiceTypeInternal.LightSensor);
			if ((serviceTypes & HMServiceType.MotionSensor) == HMServiceType.MotionSensor)
				arr.Add (HMServiceTypeInternal.MotionSensor);
			if ((serviceTypes & HMServiceType.OccupancySensor) == HMServiceType.OccupancySensor)
				arr.Add (HMServiceTypeInternal.OccupancySensor);
			if ((serviceTypes & HMServiceType.StatefulProgrammableSwitch) == HMServiceType.StatefulProgrammableSwitch)
				arr.Add (HMServiceTypeInternal.StatefulProgrammableSwitch);
			if ((serviceTypes & HMServiceType.StatelessProgrammableSwitch) == HMServiceType.StatelessProgrammableSwitch)
				arr.Add (HMServiceTypeInternal.StatelessProgrammableSwitch);
			if ((serviceTypes & HMServiceType.SmokeSensor) == HMServiceType.SmokeSensor)
				arr.Add (HMServiceTypeInternal.SmokeSensor);
			if ((serviceTypes & HMServiceType.TemperatureSensor) == HMServiceType.TemperatureSensor)
				arr.Add (HMServiceTypeInternal.TemperatureSensor);
			if ((serviceTypes & HMServiceType.Window) == HMServiceType.Window)
				arr.Add (HMServiceTypeInternal.Window);
			if ((serviceTypes & HMServiceType.WindowCovering) == HMServiceType.WindowCovering)
				arr.Add (HMServiceTypeInternal.WindowCovering);

			return _ServicesWithTypes (arr.ToArray ());
		}
	}
}
