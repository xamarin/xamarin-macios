using System;
using System.Globalization;
using ObjCRuntime;
using Foundation;

namespace UIKit {

#if WATCH
	internal
#else
	public
#endif
	partial class UIDevice {
		
		public bool CheckSystemVersion (int major, int minor)
		{
#if WATCH
			return Runtime.CheckSystemVersion (major, minor, WatchKit.WKInterfaceDevice.CurrentDevice.SystemVersion);
#else
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
#endif
		}
	}
}
