#if XAMCORE_2_0 && IOS

using XamCore.Foundation;
using XamCore.Intents;
using XamCore.ObjCRuntime;

namespace XamCore.Intents {

	public partial class INSetProfileInCarIntent {

		public INSetProfileInCarIntent (NSNumber profileNumber, string profileLabel, bool? defaultProfile) :
			this (profileNumber, profileLabel, defaultProfile.HasValue ? new NSNumber (defaultProfile.Value) : null)
		{
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? DefaultProfile {
			get { return _DefaultProfile == null ? null : (bool?) _DefaultProfile.BoolValue; }
		}
	}
}

#endif
