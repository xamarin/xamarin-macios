// INGetCarPowerLevelStatusIntentResponse.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if IOS && !TVOS
#if !(NET && __MACOS__)

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INGetCarPowerLevelStatusIntentResponse {

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public float? FuelPercentRemaining {
			get { return _FuelPercentRemaining is null ? null : (float?) _FuelPercentRemaining.FloatValue; }
		}

		public float? ChargePercentRemaining {
			get { return _ChargePercentRemaining is null ? null : (float?) _ChargePercentRemaining.FloatValue; }
		}
	}
}

#endif // !(NET && __MACOS__)
#endif
