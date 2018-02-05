#if XAMCORE_2_0 && IOS

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INSetDefrosterSettingsInCarIntent {

		public INSetDefrosterSettingsInCarIntent (bool? enable, INCarDefroster defroster) :
			this (enable.HasValue ? new NSNumber (enable.Value) : null, defroster)
		{
		}

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public bool? Enable {
			get { return _Enable == null ? null : (bool?) _Enable.BoolValue; }
		}
	}
}

#endif
