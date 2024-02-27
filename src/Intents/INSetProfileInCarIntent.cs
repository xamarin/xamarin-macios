#if IOS
using System;
using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Intents {

	public partial class INSetProfileInCarIntent {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios12.0", "Use the overload that takes 'INSpeakableString carName'.")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		public INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, bool? defaultProfile) :
			this (profileNumber, profileLabel, defaultProfile.HasValue ? new NSNumber (defaultProfile.Value) : null)
		{
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios12.0", "Use the overload that takes 'INSpeakableString carName'.")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		protected INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, NSNumber defaultProfile)
		{
			// Apple created this change in 10,2
			if (SystemVersion.CheckiOS (10, 2))
				InitializeHandle (InitWithProfileNumberName (profileNumber, profileLabel, defaultProfile));
			else
				InitializeHandle (InitWithProfileNumberLabel (profileNumber, profileLabel, defaultProfile));
		}
	}
}

#endif
