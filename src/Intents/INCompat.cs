// Compatibility stubs

#if XAMCORE_2_0 && IOS

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

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
#endif
}
#endif