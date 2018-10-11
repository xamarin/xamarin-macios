#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace Intents {

	public partial class INSetProfileInCarIntent {

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
		public INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, bool? defaultProfile) :
			this (profileNumber, profileLabel, defaultProfile.HasValue ? new NSNumber (defaultProfile.Value) : null)
		{
		}

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the overload that takes 'INSpeakableString carName'.")]
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
