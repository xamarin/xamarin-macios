using System;
using System.Globalization;
using ObjCRuntime;
using Foundation;

namespace UIKit {

	public partial class UIDevice {

#if NET
		[SupportedOSPlatformGuard ("ios")]
		[SupportedOSPlatformGuard ("tvos")]
		[SupportedOSPlatformGuard ("maccatalyst")]
#endif
		public bool CheckSystemVersion (int major, int minor)
		{
#if __MACCATALYST__
			return Runtime.CheckSystemVersion (major, minor, Runtime.iOSSupportVersion);	
#else
			return Runtime.CheckSystemVersion (major, minor, SystemVersion);
#endif
		}
	}
}
