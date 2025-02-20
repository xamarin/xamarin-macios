#if !TVOS
using System;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {

	public partial class WKPreferences {

#if !COREBUILD
		// we use the attrs of the old property 
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios14.5")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		public bool TextInteractionEnabled {
			get {
#if IOS || __MACCATALYST__
				if (!SystemVersion.CheckiOS (15, 0))
					return _OldTextInteractionEnabled;
#endif
				return _NewGetTextInteractionEnabled ();
			}
			set => _OldTextInteractionEnabled = value;
		}
#endif
	}
}
#endif
