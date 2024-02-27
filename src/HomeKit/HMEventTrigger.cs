#nullable enable

using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	partial class HMEventTrigger {

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
#endif
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			var constant = significantEvent.GetConstant ();
			if (constant is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (significantEvent));
			return CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (constant, offset);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("tvos11.0", "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[ObsoletedOSPlatform ("ios11.0", "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
#endif
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			var constant = significantEvent.GetConstant ();
			if (constant is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (significantEvent));
			return CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (constant, offset);
		}
	}
}
