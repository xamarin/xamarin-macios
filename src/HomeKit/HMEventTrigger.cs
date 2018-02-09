using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	partial class HMEventTrigger {

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (significantEvent.GetConstant (), offset);
		}

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (significantEvent.GetConstant (), offset);
		}
	}
}
