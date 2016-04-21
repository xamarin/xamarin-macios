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
}

#endif