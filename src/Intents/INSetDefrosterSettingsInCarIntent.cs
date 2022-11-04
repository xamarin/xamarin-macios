#if IOS
using System;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetDefrosterSettingsInCarIntent {

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios12.0")]
		[ObsoletedOSPlatform ("ios12.0", "Use the overload that takes 'INSpeakableString carName'.")]
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
