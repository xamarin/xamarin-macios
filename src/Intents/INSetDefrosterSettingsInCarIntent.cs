#if IOS

using System.Runtime.Versioning;
using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetDefrosterSettingsInCarIntent {

#if NET
		[UnsupportedOSPlatform ("ios12.0")]
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
