using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	[iOS (8,0)]
	[TV (10,0)]
	public partial class HMHome
	{
		public HMService [] GetServices (HMServiceType serviceTypes) 
		{
			var arr = new List<NSString> ();

			if ((serviceTypes & HMServiceType.LightBulb) == HMServiceType.LightBulb)			
				arr.Add (HMServiceType.LightBulb.GetConstant ());
			if ((serviceTypes & HMServiceType.Switch) == HMServiceType.Switch)			
				arr.Add (HMServiceType.Switch.GetConstant ());
			if ((serviceTypes & HMServiceType.Thermostat) == HMServiceType.Thermostat)			
				arr.Add (HMServiceType.Thermostat.GetConstant ());
			if ((serviceTypes & HMServiceType.GarageDoorOpener) == HMServiceType.GarageDoorOpener)			
				arr.Add (HMServiceType.GarageDoorOpener.GetConstant ());
			if ((serviceTypes & HMServiceType.AccessoryInformation) == HMServiceType.AccessoryInformation)			
				arr.Add (HMServiceType.AccessoryInformation.GetConstant ());
			if ((serviceTypes & HMServiceType.Fan) == HMServiceType.Fan)			
				arr.Add (HMServiceType.Fan.GetConstant ());
			if ((serviceTypes & HMServiceType.Outlet) == HMServiceType.Outlet)			
				arr.Add (HMServiceType.Outlet.GetConstant ());
			if ((serviceTypes & HMServiceType.LockMechanism) == HMServiceType.LockMechanism)			
				arr.Add (HMServiceType.LockMechanism.GetConstant ());
			if ((serviceTypes & HMServiceType.LockManagement) == HMServiceType.LockManagement)			
				arr.Add (HMServiceType.LockManagement.GetConstant ());
			// iOS 9
			if ((serviceTypes & HMServiceType.AirQualitySensor) == HMServiceType.AirQualitySensor)
				arr.Add (HMServiceType.AirQualitySensor.GetConstant ());
			if ((serviceTypes & HMServiceType.SecuritySystem) == HMServiceType.SecuritySystem)
				arr.Add (HMServiceType.SecuritySystem.GetConstant ());
			if ((serviceTypes & HMServiceType.CarbonMonoxideSensor) == HMServiceType.CarbonMonoxideSensor)
				arr.Add (HMServiceType.CarbonMonoxideSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.ContactSensor) == HMServiceType.ContactSensor)
				arr.Add (HMServiceType.ContactSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.Door) == HMServiceType.Door)
				arr.Add (HMServiceType.Door.GetConstant ());
			if ((serviceTypes & HMServiceType.HumiditySensor) == HMServiceType.HumiditySensor)
				arr.Add (HMServiceType.HumiditySensor.GetConstant ());
			if ((serviceTypes & HMServiceType.LeakSensor) == HMServiceType.LeakSensor)
				arr.Add (HMServiceType.LeakSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.LightSensor) == HMServiceType.LightSensor)
				arr.Add (HMServiceType.LightSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.MotionSensor) == HMServiceType.MotionSensor)
				arr.Add (HMServiceType.MotionSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.OccupancySensor) == HMServiceType.OccupancySensor)
				arr.Add (HMServiceType.OccupancySensor.GetConstant ());
			if ((serviceTypes & HMServiceType.StatefulProgrammableSwitch) == HMServiceType.StatefulProgrammableSwitch)
				arr.Add (HMServiceType.StatefulProgrammableSwitch.GetConstant ());
			if ((serviceTypes & HMServiceType.StatelessProgrammableSwitch) == HMServiceType.StatelessProgrammableSwitch)
				arr.Add (HMServiceType.StatelessProgrammableSwitch.GetConstant ());
			if ((serviceTypes & HMServiceType.SmokeSensor) == HMServiceType.SmokeSensor)
				arr.Add (HMServiceType.SmokeSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.TemperatureSensor) == HMServiceType.TemperatureSensor)
				arr.Add (HMServiceType.TemperatureSensor.GetConstant ());
			if ((serviceTypes & HMServiceType.Window) == HMServiceType.Window)
				arr.Add (HMServiceType.Window.GetConstant ());
			if ((serviceTypes & HMServiceType.WindowCovering) == HMServiceType.WindowCovering)
				arr.Add (HMServiceType.WindowCovering.GetConstant ());
			// iOS 10.2
			if ((serviceTypes & HMServiceType.AirPurifier) == HMServiceType.AirPurifier)
				arr.Add (HMServiceType.AirPurifier.GetConstant ());
			if ((serviceTypes & HMServiceType.VentilationFan) == HMServiceType.VentilationFan)
				arr.Add (HMServiceType.VentilationFan.GetConstant ());
			if ((serviceTypes & HMServiceType.FilterMaintenance) == HMServiceType.FilterMaintenance)
				arr.Add (HMServiceType.FilterMaintenance.GetConstant ());
			if ((serviceTypes & HMServiceType.HeaterCooler) == HMServiceType.HeaterCooler)
				arr.Add (HMServiceType.HeaterCooler.GetConstant ());
			if ((serviceTypes & HMServiceType.HumidifierDehumidifier) == HMServiceType.HumidifierDehumidifier)
				arr.Add (HMServiceType.HumidifierDehumidifier.GetConstant ());
			if ((serviceTypes & HMServiceType.Slats) == HMServiceType.Slats)
				arr.Add (HMServiceType.Slats.GetConstant ());

			return GetServices (arr.ToArray ());
		}

		[NoTV]
		[NoWatch]
		[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		[Obsoleted (PlatformName.iOS, 9,0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		public virtual void RemoveUser (HMUser user, Action<NSError> completion) {
			throw new NotSupportedException ();
		}

		[NoTV]
		[NoWatch]
		[Introduced (PlatformName.iOS, 8,0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		[Obsoleted (PlatformName.iOS, 9,0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		public virtual Task RemoveUserAsync (HMUser user) {
			throw new NotSupportedException ();
		}
	}
}
