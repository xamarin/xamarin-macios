using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace MapKit {

#if !NET && __IOS__
	public partial class MKUserTrackingBarButtonItem {

		[Obsolete ("Does not return a valid instance on iOS 12.")]
		public MKUserTrackingBarButtonItem ()
		{
		}
	}
#endif
}
