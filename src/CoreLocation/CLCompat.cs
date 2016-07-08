#if !XAMCORE_2_0

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;

namespace XamCore.CoreLocation {

	public partial class CLBeaconRegion {

		[Obsolete ("Does not return a valid instance on iOS8")]
		public CLBeaconRegion ()
		{
		}
	}

#if !XAMCORE_4_0
	public partial class CLHeading {

		[Obsolete ("Use the Description property from NSObject")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}

	public partial class CLLocation {

		[Obsolete ("Use the Description property from NSObject")]
		public new virtual string Description ()
		{
			return base.Description;
		}
	}
#endif
}

#endif