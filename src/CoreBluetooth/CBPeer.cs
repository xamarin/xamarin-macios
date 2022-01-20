#if IOS
using ObjCRuntime;
using Foundation;
using System;
using CoreFoundation;
using System.Runtime.Versioning;

#nullable enable

namespace CoreBluetooth {
	public partial class CBPeer  {
#if !NET
#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#if IOS
		[Obsolete ("Starting with ios9.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Obsoleted (PlatformName.iOS, 9, 0)]
#endif
		public virtual CBUUID UUID { 
			get {
				return CBUUID.FromCFUUID (_UUID);	
			}
		}
#endif // !NET
	}
}
#endif
