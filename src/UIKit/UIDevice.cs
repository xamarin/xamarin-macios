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

#if !XAMCORE_2_0
		[Obsolete ("Deprecated in iOS 5.0. Apple now reject application using it the selector is removed and an empty string is returned")]
		public virtual string UniqueIdentifier {
			get { return string.Empty; }
		}
#endif
	}
}
