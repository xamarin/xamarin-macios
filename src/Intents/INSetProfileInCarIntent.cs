#if IOS

using System.Runtime.Versioning;
using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace Intents {

	public partial class INSetProfileInCarIntent {

#if NET
		[UnsupportedOSPlatform ("ios12.0")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		public INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, bool? defaultProfile) :
			this (profileNumber, profileLabel, defaultProfile.HasValue ? new NSNumber (defaultProfile.Value) : null)
		{
		}

#if NET
		[UnsupportedOSPlatform ("ios12.0")]
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
#endif
		protected INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, NSNumber defaultProfile)
		{
			// Apple created this change in 10,2
			if (UIDevice.CurrentDevice.CheckSystemVersion (10, 2))
				InitializeHandle (InitWithProfileNumberName (profileNumber, profileLabel, defaultProfile));
			else
				InitializeHandle (InitWithProfileNumberLabel (profileNumber, profileLabel, defaultProfile));
		}
	}
}

#endif
