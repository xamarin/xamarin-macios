using System;
using MonoTouch.Foundation;

namespace BindingTests
{
	public enum HMAccessoryCategoryType {
		[Field ("HMAccessoryCategoryTypeGarageDoorOpener")]
		DoorOpener,

		[Field ("HMAccessoryCategoryTypeGarageDoorOpener")]
		GarageDoorOpener = DoorOpener,
	}
}
