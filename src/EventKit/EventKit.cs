//
// Custom methods for EventKit
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012-2014, Xamarin Inc.
//

#if XAMCORE_2_0 || !MONOMAC

using ObjCRuntime;
using Foundation;
using System;

namespace EventKit {

#if !XAMCORE_4_0

#if XAMCORE_2_0
	partial class EKRecurrenceRule {
		public EKDay FirstDayOfTheWeek {
			get { return (EKDay) (int) _FirstDayOfTheWeek; }
		}
	}
#endif

	partial class EKRecurrenceDayOfWeek {
#if XAMCORE_2_0
		public static EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek)
		{
			return _FromDay ((int) dayOfTheWeek);
		}

		public static EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek, nint weekNumber)
		{
			return _FromDay ((int) dayOfTheWeek, weekNumber);
		}
#endif
		public static EKRecurrenceDayOfWeek FromWeekDay (nint dayOfWeek, nint weekNumber)
		{
			return FromDay ((EKDay) (int) dayOfWeek, weekNumber);
		}
	}

#endif // !XAMCORE_4_0

	partial class EKEventStore {
#if !MONOMAC && !XAMCORE_2_0
		[Obsolete ("Replaced by RemoveCalendar")]
		public bool RemoveCalendarc (EKCalendar calendar, bool commit, out NSError error)
		{
			return RemoveCalendar (calendar, commit, out error);
		}
#endif
	}

	partial class EKEvent {
#if !MONOMAC && !XAMCORE_2_0
		[Obsolete ("Use BirthdayPersonID")]
		nint birthdayPersonID {
			get {
				return BirthdayPersonID;
			}
		}
#endif
#if !XAMCORE_2_0
		// Apple will reject application using the 'recurrenceRule' selector - 
		// so we can't ship it anymore (e.g. for people not enabling linking)
		[Obsolete ("Removed in iOS 6.0", true)]
		public virtual EKRecurrenceRule RecurrenceRule {
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}
#endif
	}

	partial class EKAlarm {
#if !XAMCORE_4_0
		[Obsolete ("Use the static methods FromDate or FromTimeInterval to create alarms")]
		public EKAlarm () {}
#endif
	}
}
#endif // XAMCORE_2_0 || !MONOMAC
