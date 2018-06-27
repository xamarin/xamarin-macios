using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace MapKit {

#if !XAMCORE_4_0
	public partial class MKUserTrackingBarButtonItem {

		[Obsolete ("Does not return a valid instance on iOS 12.")]
		public MKUserTrackingBarButtonItem ()
		{
		}
	}
#endif
}
