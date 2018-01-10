// INGetCarPowerLevelStatusIntentResponse.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && (IOS || TVOS)

using Foundation;
using Intents;
using ObjCRuntime;

namespace Intents {

	public partial class INGetCarPowerLevelStatusIntentResponse {

		// if/when we update the generator to allow this pattern we can move this back
		// into bindings and making them virtual (not a breaking change)

		public float? FuelPercentRemaining {
			get { return _FuelPercentRemaining == null ? null : (float?) _FuelPercentRemaining.FloatValue; }
		}

		public float? ChargePercentRemaining {
			get { return _ChargePercentRemaining == null ? null : (float?) _ChargePercentRemaining.FloatValue; }
		}
	}
}

#endif
