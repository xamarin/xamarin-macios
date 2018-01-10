#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;
using UIKit;

namespace Intents {

	public partial class INSetProfileInCarIntent {

		[DesignatedInitializer]
		public INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, bool? defaultProfile) :
			this (profileNumber, profileLabel, defaultProfile.HasValue ? new NSNumber (defaultProfile.Value) : null)
		{
		}

		protected INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, NSNumber defaultProfile)
		{
			// Apple created this change in 10,2
			if (UIDevice.CurrentDevice.CheckSystemVersion (10, 2))
				InitializeHandle (InitWithProfileNumberName (profileNumber, profileLabel, defaultProfile));
			else
				InitializeHandle (InitWithProfileNumberLabel (profileNumber, profileLabel, defaultProfile));
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? DefaultProfile {
			get { return _DefaultProfile == null ? null : (bool?) _DefaultProfile.BoolValue; }
		}
	}
}

#endif
