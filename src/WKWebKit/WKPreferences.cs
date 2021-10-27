#if !TVOS
using System;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {

	public partial class WKPreferences {

#if !COREBUILD
		// we use the attrs of the old property 
#if !NET
		[Mac (11,3)][iOS (14,5)][MacCatalyst (14,5)]
#else
		[SupportedOSPlatform ("ios14.5"), SupportedOSPlatform ("macos11.3"), SupportedOSPlatform ("maccatalyst14.5")]
#endif
		public bool TextInteractionEnabled { 
			get {
#if IOS || __MACCATALYST__ 
				if (UIKit.UIDevice.CurrentDevice.CheckSystemVersion (15, 0))
#elif MONOMAC
				if (PlatformHelper.CheckSystemVersion (12, 0))
#endif
					return _NewGetTextInteractionEnabled ();
				else
					return _OldTextInteractionEnabled;
			}
			set => _OldTextInteractionEnabled = value;
		}
#endif
	}
}
#endif
