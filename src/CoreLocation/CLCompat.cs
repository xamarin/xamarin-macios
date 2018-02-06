using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace CoreLocation {

#if !XAMCORE_2_0
	public partial class CLBeaconRegion {

		[Obsolete ("Does not return a valid instance on iOS 8.")]
		public CLBeaconRegion ()
		{
		}
	}
#endif

#if !XAMCORE_4_0 && !WATCH

#if !TVOS
	public partial class CLHeading {

		[Obsolete ("Use the 'Description' property from 'NSObject'.")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}
#endif

	public partial class CLLocation {

		[Obsolete ("Use the 'Description' property from 'NSObject'.")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}
#endif
}
