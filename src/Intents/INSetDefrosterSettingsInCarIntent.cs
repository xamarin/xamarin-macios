#if IOS
using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetDefrosterSettingsInCarIntent {

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[UnsupportedOSPlatform ("ios12.0")]
#if IOS
		[Obsolete ("Starting with ios12.0 use the overload that takes 'INSpeakableString carName'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		public INSetDefrosterSettingsInCarIntent (bool? enable, INCarDefroster defroster) :
			this (enable.HasValue ? new NSNumber (enable.Value) : null, defroster)
		{
		}
	}
}

#endif
