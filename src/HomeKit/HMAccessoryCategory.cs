using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	partial class HMAccessoryCategory {

		[iOS (9,0)]
		public HMAccessoryCategoryType CategoryType {
			get {
				var s = _CategoryType;
				// safety in case the field does not exists / cannot be loaded / new in future iOS versions...
				if (s == null)
					return HMAccessoryCategoryType.Other;
				if (s == HMAccessoryCategoryTypesInternal.SecuritySystem)
					return HMAccessoryCategoryType.SecuritySystem;
				if (s == HMAccessoryCategoryTypesInternal.Bridge)
					return HMAccessoryCategoryType.Bridge;
				if (s == HMAccessoryCategoryTypesInternal.Door)
					return HMAccessoryCategoryType.Door;
				if (s == HMAccessoryCategoryTypesInternal.DoorLock)
					return HMAccessoryCategoryType.DoorLock;
				if (s == HMAccessoryCategoryTypesInternal.Fan)
					return HMAccessoryCategoryType.Fan;
				if (s == HMAccessoryCategoryTypesInternal.DoorOpener)
					return HMAccessoryCategoryType.DoorOpener;
				if (s == HMAccessoryCategoryTypesInternal.Lightbulb)
					return HMAccessoryCategoryType.Lightbulb;
				if (s == HMAccessoryCategoryTypesInternal.Outlet)
					return HMAccessoryCategoryType.Outlet;
				if (s == HMAccessoryCategoryTypesInternal.ProgrammableSwitch)
					return HMAccessoryCategoryType.ProgrammableSwitch;
				if (s == HMAccessoryCategoryTypesInternal.Sensor)
					return HMAccessoryCategoryType.Sensor;
				if (s == HMAccessoryCategoryTypesInternal.Switch)
					return HMAccessoryCategoryType.Switch;
				if (s == HMAccessoryCategoryTypesInternal.Thermostat)
					return HMAccessoryCategoryType.Thermostat;
				if (s == HMAccessoryCategoryTypesInternal.Window)
					return HMAccessoryCategoryType.Window;
				if (s == HMAccessoryCategoryTypesInternal.WindowCovering)
					return HMAccessoryCategoryType.WindowCovering;
				return HMAccessoryCategoryType.Other;
			}
		}
	}
}
