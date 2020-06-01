//
// Custom methods for EventKit
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014, Xamarin Inc.
//

using ObjCRuntime;
using Foundation;
using System;

namespace EventKit {

#if !XAMCORE_4_0

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

#endif // !XAMCORE_4_0

	partial class EKAlarm {
#if !XAMCORE_4_0
		[Obsolete ("Use the static methods FromDate or FromTimeInterval to create alarms")]
		public EKAlarm () {}
#endif
	}

	partial class EKReminder {
#if !XAMCORE_4_0
		// https://github.com/xamarin/maccore/issues/1832
		[Obsolete ("Use 'Create' instead.")]
		public EKReminder () {}
#endif
	}
}
