// Compatibility stubs

#if XAMCORE_2_0 && IOS

using System;
using Foundation;
using ObjCRuntime;

namespace Intents {

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