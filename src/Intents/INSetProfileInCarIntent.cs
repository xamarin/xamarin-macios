#if IOS
using System;
using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace Intents {

	public partial class INSetProfileInCarIntent {

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios12.0")]
#if IOS
		[Obsolete ("Starting with ios12.0 use the overload that takes 'INSpeakableString carName'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
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
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios12.0")]
#if IOS
		[Obsolete ("Starting with ios12.0 use the overload that takes 'INSpeakableString carName'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
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
