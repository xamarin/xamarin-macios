#if !XAMCORE_3_0

using System;

namespace MapKit {

	public partial class MKDirections {
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public MKDirections ()
		{
		}
	}
}

#endif
