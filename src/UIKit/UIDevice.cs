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

#if NET
		[SupportedOSPlatformGuard ("ios")]
		[SupportedOSPlatformGuard ("tvos")]
		[SupportedOSPlatformGuard ("maccatalyst")]
#endif
		public bool CheckSystemVersion (int major, int minor)
		{
#if WATCH
			return Runtime.CheckSystemVersion (major, minor, WatchKit.WKInterfaceDevice.CurrentDevice.SystemVersion);
#elif __MACCATALYST__
			return Runtime.CheckSystemVersion (major, minor, Runtime.iOSSupportVersion);	
#else
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
#endif
		}
	}
}
