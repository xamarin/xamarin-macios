#if iOS

using System;

using ObjCRuntime;

namespace CoreLocation {

	public partial class CLBeaconRegion {

		[Introduced (PlatformName.iOS, 13,0, PlatformArchitecture.All)]
		static public CLBeaconRegion Create (NSUuid uuid, ushort? major, ushort? minor, string identifier)
		{
			var handle = IntPtr.Zero;
			if (!major.HasValue)
				handle = _Constructor (uuid, identifier);
			else if (!minor.HasValue)
				handle = _Constructor (uuid, major.Value, identifier);
			else
				handle = _Constructor (uuid, major.Value, minor.Value, identifier);
			return new CLBeaconRegion (handle);
		}
	}
}

#endif
