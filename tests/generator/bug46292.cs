using System;

using Foundation;

namespace BindingTests {
	[Obsolete ("Type level obsolete must also be copied")]
	public enum HMAccessoryCategoryType : int {
		[Field ("HMAccessoryCategoryTypeGarageDoorOpener", "__Internal")]
		GarageDoorOpener = 0,

		[Obsolete ("Use GarageDoorOpener")]
		DoorOpener = GarageDoorOpener,
	}
}
