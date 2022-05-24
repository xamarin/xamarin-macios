#nullable enable

using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	partial class HMActionSet {

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (9,0)]
#endif
		public HMActionSetType ActionSetType {
			get {
				var s = _ActionSetType;
				// safety in case the field does not exists / cannot be loaded / new in future iOS versions...
				if (s == null)
					return HMActionSetType.Unknown;
				if (s == HMActionSetTypesInternal.WakeUp)
					return HMActionSetType.WakeUp;
				if (s == HMActionSetTypesInternal.Sleep)
					return HMActionSetType.Sleep;
				if (s == HMActionSetTypesInternal.HomeDeparture)
					return HMActionSetType.HomeDeparture;
				if (s == HMActionSetTypesInternal.HomeArrival)
					return HMActionSetType.HomeArrival;
				if (s == HMActionSetTypesInternal.UserDefined)
					return HMActionSetType.UserDefined;
				if (s == HMActionSetTypesInternal.TriggerOwned)
					return HMActionSetType.TriggerOwned;
				return HMActionSetType.Unknown;
			}
		}
	}
}
