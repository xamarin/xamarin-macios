//
// Custom methods for EventKit
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014, Xamarin Inc.
//

#nullable enable

using ObjCRuntime;
using Foundation;
using System;

namespace EventKit {

#if !NET

	partial class EKRecurrenceRule {
		public EKDay FirstDayOfTheWeek {
			get { return (EKDay) (int) _FirstDayOfTheWeek; }
		}
	}

	partial class EKRecurrenceDayOfWeek {
		public static EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek)
		{
			return _FromDay ((int) dayOfTheWeek);
		}

		public static EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek, nint weekNumber)
		{
			return _FromDay ((int) dayOfTheWeek, weekNumber);
		}

		public static EKRecurrenceDayOfWeek FromWeekDay (nint dayOfWeek, nint weekNumber)
		{
			return FromDay ((EKDay) (int) dayOfWeek, weekNumber);
		}
	}

#endif // !NET

	partial class EKAlarm {
#if !NET
		[Obsolete ("Use the static methods FromDate or FromTimeInterval to create alarms")]
		public EKAlarm () { }
#endif
	}

	partial class EKReminder {
#if !NET
		// https://github.com/xamarin/maccore/issues/1832
		[Obsolete ("Use 'Create' instead.")]
		public EKReminder () { }
#endif
	}
}
