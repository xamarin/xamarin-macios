using System;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	partial class HMEventTrigger {

#if NET
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantTimeEvent)' instead.")]
#endif
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringBeforeSignificantEvent (significantEvent.GetConstant (), offset);
		}

#if NET
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if TVOS
		[Obsolete ("Starting with tvos11.0 use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantTimeEvent)' instead.")]
#endif
		static public NSPredicate CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (HMSignificantEvent significantEvent, NSDateComponents offset)
		{
			return CreatePredicateForEvaluatingTriggerOccurringAfterSignificantEvent (significantEvent.GetConstant (), offset);
		}
	}
}
