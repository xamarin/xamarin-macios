using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	partial class HMEventTrigger {

		internal static NSString GetEnumConstant (HMSignificantEvent value)
		{
			switch (value) {
			case HMSignificantEvent.Sunrise:
				return HMSignificantEventInternal.Sunrise;
			case HMSignificantEvent.Sunset:
				return HMSignificantEventInternal.Sunset;
			default:
				return null;
			}
		}

		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (GetEnumConstant (significantEvent), offset);
		}

		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (GetEnumConstant (significantEvent), offset);
		}
	}
}
