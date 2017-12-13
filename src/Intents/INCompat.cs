// Compatibility stubs

#if XAMCORE_2_0 && IOS

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.Intents {

#if !XAMCORE_4_0
	public partial class INRideDriver {

		[Obsolete ("This constructor does not create a valid instance of the type")]
		public INRideDriver () : base (NSObjectFlag.Empty)
		{
		}
	}

	public partial class INRideStatus {

		[Obsolete ("This constructor does not create a valid instance of the type")]
		public INRideStatus ()
		{
		}
	}

	public partial class INRestaurantGuest {
		[Obsolete ("This constructor does not create a valid instance of the type")]
		public INRestaurantGuest ()
			: base (IntPtr.Zero) // base class doesn't have a default ctor.
		{
		}
	}
#endif
}
#endif