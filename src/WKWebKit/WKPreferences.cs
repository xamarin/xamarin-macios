#if !TVOS
using System;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace WebKit {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class WKPreferences {

#if !COREBUILD
		// we use the attrs of the old property 
#if NET
		[SupportedOSPlatform ("macos11.3")]
		[SupportedOSPlatform ("ios14.5")]
		[SupportedOSPlatform ("maccatalyst14.5")]
#else
		[Mac (11,3)]
		[iOS (14,5)]
		[MacCatalyst (14,5)]
#endif
		public bool TextInteractionEnabled { 
			get {
#if IOS || __MACCATALYST__ 
				if (SystemVersion.CheckiOS (15, 0))
#elif MONOMAC
				if (SystemVersion.CheckmacOS (12, 0))
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
