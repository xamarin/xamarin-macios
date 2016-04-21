//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010, Novell, Inc.
// Copyright 2012-2015, Xamarin Inc.
//

#if !WATCH
using XamCore.AddressBook;
#endif
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
#if XAMCORE_2_0
using XamCore.MapKit;
#endif
using System;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
#endif

namespace XamCore.EventKit {

	[Since (5,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0 || MONOMAC
	[Abstract]
#endif
	interface EKObject {
		[Export ("hasChanges")]
		bool HasChanges { get; }

		[Export ("isNew")]
		bool IsNew { get; }

		[Export ("reset")]
		void Reset ();

		[Export ("rollback")]
		void Rollback ();

		[Export ("refresh")]
		bool Refresh ();
	}

	[Since (5,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	interface EKCalendarItem {
#if !MONOMAC
		// Never made avaialble on MonoMac
		[Export ("UUID")]
		[Availability (Introduced = Platform.iOS_5_0, Deprecated = Platform.iOS_6_0, Message = "Use CalendarItemIdentifier instead")]
		string UUID { get;  }
#endif

		[NullAllowed] // by default this property is null
		[Export ("calendar", ArgumentSemantic.Retain)]
		EKCalendar Calendar { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set;  }

		[NullAllowed] // it's null by default on iOS 6.1
		[Export ("location", ArgumentSemantic.Copy)]
		string Location { get; set;  }

		[Export ("notes", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Notes { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set;  }

		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get;  }

		[Export ("creationDate")]
		NSDate CreationDate { get;  }

		[NullAllowed] // by default this property is null
		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set;  }

		[Export ("hasAlarms")]
		bool HasAlarms { get;  }

		[Export ("hasRecurrenceRules")]
		bool HasRecurrenceRules { get;  }

		[Export ("hasAttendees")]
		bool HasAttendees { get;  }

		[Export ("hasNotes")]
		bool HasNotes { get;  }

		[Export ("attendees")]
		EKParticipant [] Attendees { get;  }

		[NullAllowed] // by default this property is null
		[Export ("alarms", ArgumentSemantic.Copy)]
		EKAlarm [] Alarms { get; set;  }

		[Export ("recurrenceRules", ArgumentSemantic.Copy)]
		EKRecurrenceRule [] RecurrenceRules { get; set;  }

		[Export ("addAlarm:")]
		void AddAlarm (EKAlarm alarm);

		[Export ("removeAlarm:")]
		void RemoveAlarm (EKAlarm alarm);

		[Export ("addRecurrenceRule:")]
		void AddRecurrenceRule (EKRecurrenceRule rule);

		[Export ("removeRecurrenceRule:")]
		void RemoveRecurrenceRule (EKRecurrenceRule rule);

		[Since(6,0)]
		[Export ("calendarItemIdentifier")]
		string CalendarItemIdentifier { get;  }

		[Since(6,0)]
		[Export ("calendarItemExternalIdentifier")]
		string CalendarItemExternalIdentifier { get;  }
	}
	
	[Since (5,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	interface EKSource {
		[Export ("sourceType")]
		EKSourceType SourceType { get;  }

		[Export ("title")]
		string Title { get;  }

#if !MONOMAC
		[Export ("calendars")]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_6_0, Message = "Use GetCalendars (EKEntityType) instead")]
		NSSet Calendars { get;  }
#endif

		[Export ("sourceIdentifier")]
		string SourceIdentifier { get; }

		[Since (6, 0)]
		[Export ("calendarsForEntityType:")]
		NSSet GetCalendars (EKEntityType entityType);
	}

	[Since (6,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	interface EKStructuredLocation : NSCopying {
		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("geoLocation", ArgumentSemantic.Strong)]
		CLLocation GeoLocation { get; set;  }

		[Export ("radius")]
		double Radius { get; set;  }

		[Export ("locationWithTitle:"), Static]
		EKStructuredLocation FromTitle (string title);

#if XAMCORE_2_0
		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("locationWithMapItem:")]
		EKStructuredLocation FromMapItem (MKMapItem mapItem);
#endif
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	interface EKAlarm : NSCopying {
		[Export ("relativeOffset")]
		double RelativeOffset { get; set;  }

		[Export ("absoluteDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate AbsoluteDate { get; set;  }

		[Static]
		[Export ("alarmWithAbsoluteDate:")]
		EKAlarm FromDate (NSDate date);

		[Static]
		[Export ("alarmWithRelativeOffset:")]
		EKAlarm FromTimeInterval (double offsetSeconds);

		[Since (6,0)]
		[Export ("structuredLocation", ArgumentSemantic.Copy)]
		[NullAllowed]
		EKStructuredLocation StructuredLocation { get; set;  }

		[Since (6,0)]
		[Export ("proximity")]
		EKAlarmProximity Proximity { get; set;  }

#if MONOMAC
		[Export ("type")]
		EKAlarmType Type { get; }

		[Export ("emailAddress")]
		string EmailAddress { get; set; }

		[Export ("soundName")]
		string SoundName { get; set; }

		[Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }
#endif
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	[DisableDefaultCtor]
	interface EKCalendar {
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("type")]
		EKCalendarType Type { get;  }

		[Export ("allowsContentModifications")]
		bool AllowsContentModifications { get;  }

#if MONOMAC
		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }
#else
		[Export ("CGColor")]
		CGColor CGColor { get; set; }
#endif

		[Export ("supportedEventAvailabilities")]
		EKCalendarEventAvailability SupportedEventAvailabilities { get; }

		[Since (5,0)]
		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get;  }

		[Since (5,0)]
		[Export ("subscribed")]
		bool Subscribed { [Bind ("isSubscribed")] get;  }

		[Since (5,0)]
		[Export ("immutable")]
		bool Immutable { [Bind ("isImmutable")] get;  }

#if !MONOMAC
		[Since (5,0)]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_6_0, Message = "Use Create (EKEntityType, EKEventStore) instead")]
		[Static, Export ("calendarWithEventStore:")]
		EKCalendar FromEventStore (EKEventStore eventStore);
#endif

		[Since (5,0)]
		[Export ("source", ArgumentSemantic.Retain)]
		EKSource Source { get; set; }

		[Since (6,0)]
		[Export ("allowedEntityTypes")]
		EKEntityMask AllowedEntityTypes { get;  }

		[Since (6,0)]
		[Static]
		[Export ("calendarForEntityType:eventStore:")]
		EKCalendar Create (EKEntityType entityType, EKEventStore eventStore);
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKCalendarItem))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: You must use [EKEvent eventWithStore:] to create an event
	[DisableDefaultCtor]
	interface EKEvent {
		[Static, Export ("eventWithEventStore:")]
		EKEvent FromStore (EKEventStore eventStore);

		[Export ("allDay")]
		bool AllDay { [Bind ("isAllDay")] get; set;  }

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; set;  }

		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; set;  }

		[Export ("organizer")]
		EKParticipant Organizer { get;  }

		[Export ("isDetached")]
		bool IsDetached { get;  }

		[Export ("eventIdentifier")]
		string EventIdentifier { get; }

		[Export ("compareStartDateWithEvent:")]
		NSComparisonResult CompareStartDateWithEvent (EKEvent other);

		[Export ("refresh")]
		bool Refresh ();

		[Export  ("availability")]
		EKEventAvailability Availability { get; set; }

		[Export ("status")]
		EKEventStatus Status { get; }

		[iOS (9,0), Mac(10,11)]
		[NullAllowed, Export ("structuredLocation", ArgumentSemantic.Copy)]
		EKStructuredLocation StructuredLocation { get; set; }
		
		[iOS (9,0)][Mac (10,8)]
		[Export ("occurrenceDate")]
		NSDate OccurrenceDate { get; }

#if MONOMAC
		[Availability (Introduced = Platform.Mac_10_8, Deprecated = Platform.Mac_10_11, Message = "Replaced by BirthdayContactIdentifier")]
		[Export ("birthdayPersonUniqueID")]
		string BirthdayPersonUniqueID { get; }
#else
		[Availability (Introduced = Platform.iOS_5_0, Deprecated = Platform.iOS_9_0, Message = "Replaced by BirthdayContactIdentifier")]
		[Export ("birthdayPersonID")]
		nint BirthdayPersonID { get;  }
#endif
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("birthdayContactIdentifier")]
		string BirthdayContactIdentifier { get; }
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
#if XAMCORE_3_0
	[DisableDefaultCtor]
#endif
	interface EKParticipant : NSCopying {
		[Export ("URL")]
		NSUrl Url { get;  }

		[Export ("name")]
		string Name { get;  }

		[Export ("participantStatus")]
		EKParticipantStatus ParticipantStatus { get;  }

		[Export ("participantRole")]
		EKParticipantRole ParticipantRole { get;  }

		[Export ("participantType")]
		EKParticipantType ParticipantType { get;  }

#if MONOMAC
// missing some Mac support for the address book
//		[Export ("ABPersonInAddressBook:")]
//		ABPerson GetPerson (ABAddressBook addressBook);
#else
#if !WATCH
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_9_0, Message = "Replaced by ContactPredicate")]
		[Export ("ABRecordWithAddressBook:")]
		ABRecord GetRecord (ABAddressBook addressBook);
#endif // !WATCH

#endif
		[Since (6,0)]
		[Mac (10,9)]
		[Export ("isCurrentUser")]
		bool IsCurrentUser { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("contactPredicate")]
		NSPredicate ContactPredicate { get; }
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface EKRecurrenceEnd : NSCopying {
		[Export ("endDate")]
		NSDate EndDate { get;  }

		[Export ("occurrenceCount")]
		nint OccurrenceCount { get;  }

		[Static]
		[Export ("recurrenceEndWithEndDate:")]
		EKRecurrenceEnd FromEndDate (NSDate endDate);

		[Static]
		[Export ("recurrenceEndWithOccurrenceCount:")]
		EKRecurrenceEnd FromOccurrenceCount (nint occurrenceCount);
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface EKRecurrenceDayOfWeek : NSCopying {
		[Export ("dayOfTheWeek")]
#if XAMCORE_4_0
		EKWeekday DayOfTheWeek { get;  }
#else
		nint DayOfTheWeek { get;  }
#endif

		[Export ("weekNumber")]
		nint WeekNumber { get;  }

		[Static]
		[Export ("dayOfWeek:")]
#if XAMCORE_4_0
		EKRecurrenceDayOfWeek FromDay (EKWeekday dayOfTheWeek);
#elif XAMCORE_2_0
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek);
#else
		EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek);
#endif

		[Static]
		[Export ("dayOfWeek:weekNumber:")]
#if XAMCORE_4_0
		EKRecurrenceDayOfWeek FromDay (EKWeekday dayOfTheWeek, nint weekNumber);
#elif XAMCORE_2_0
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek, nint weekNumber);
#else
		EKRecurrenceDayOfWeek FromDay (EKDay dayOfTheWeek, nint weekNumber);
#endif

		[Since (5,0)]
		[Export ("initWithDayOfTheWeek:weekNumber:")]
#if XAMCORE_4_0
		IntPtr Constructor (EKWeekday dayOfTheWeek, nint weekNumber);
#else
		IntPtr Constructor (nint dayOfTheWeek, nint weekNumber);
#endif
	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKObject))]
	interface EKRecurrenceRule : NSCopying {
		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get;  }

		[NullAllowed] // by default this property is null
		[Export ("recurrenceEnd", ArgumentSemantic.Copy)]
		EKRecurrenceEnd RecurrenceEnd { get; set;  }

		[Export ("frequency")]
		EKRecurrenceFrequency Frequency { get;  }

		[Export ("interval")]
		nint Interval { get;  }

		[Export ("firstDayOfTheWeek")]
#if XAMCORE_4_0
		EKWeekday FirstDayOfTheWeek { get; }
#elif XAMCORE_2_0
		[Internal]
		nint _FirstDayOfTheWeek { get; }
#else
		EKDay FirstDayOfTheWeek { get;  }
#endif

		[Export ("daysOfTheWeek")]
		EKRecurrenceDayOfWeek [] DaysOfTheWeek { get;  }

		[Export ("daysOfTheMonth")]
		NSNumber [] DaysOfTheMonth { get;  }

		[Export ("daysOfTheYear")]
		NSNumber [] DaysOfTheYear { get;  }

		[Export ("weeksOfTheYear")]
		NSNumber [] WeeksOfTheYear { get;  }

		[Export ("monthsOfTheYear")]
		NSNumber [] MonthsOfTheYear { get;  }

		[Export ("setPositions")]
#if XAMCORE_4_0
		NSNumber [] SetPositions { get; }
#else
		NSObject [] SetPositions { get;  }
#endif

		[Export ("initRecurrenceWithFrequency:interval:end:")]
		IntPtr Constructor (EKRecurrenceFrequency type, nint interval, [NullAllowed] EKRecurrenceEnd end);

		[Export ("initRecurrenceWithFrequency:interval:daysOfTheWeek:daysOfTheMonth:monthsOfTheYear:weeksOfTheYear:daysOfTheYear:setPositions:end:")]
		IntPtr Constructor (EKRecurrenceFrequency type, nint interval, [NullAllowed] EKRecurrenceDayOfWeek [] days, [NullAllowed] NSNumber [] monthDays, [NullAllowed] NSNumber [] months,
				    [NullAllowed] NSNumber [] weeksOfTheYear, [NullAllowed] NSNumber [] daysOfTheYear, [NullAllowed] NSNumber [] setPositions, [NullAllowed] EKRecurrenceEnd end);

	}

	[Since (4,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface EKEventStore {
		[Export ("eventStoreIdentifier")]
		string EventStoreIdentifier { get;  }

#if !MONOMAC
		[Export ("calendars")]
		[Availability (Introduced = Platform.iOS_4_0, Deprecated = Platform.iOS_6_0, Message = "Use GetCalendars instead")]
		EKCalendar [] Calendars { get;  }
#endif

		[Export ("defaultCalendarForNewEvents")]
		EKCalendar DefaultCalendarForNewEvents { get;  }

#if !MONOMAC
		[NoWatch]
		[Export ("saveEvent:span:error:")]
		bool SaveEvent (EKEvent theEvent, EKSpan span, out NSError error);

		[NoWatch]
		[Export ("removeEvent:span:error:")]
		bool RemoveEvents (EKEvent theEvent, EKSpan span, out NSError error);
#endif

		[Export ("eventWithIdentifier:")]
		EKEvent EventFromIdentifier (string identifier);

		[Export ("eventsMatchingPredicate:")]
		EKEvent [] EventsMatching (NSPredicate predicate);

		[Export ("enumerateEventsMatchingPredicate:usingBlock:")]
		void EnumerateEvents (NSPredicate predicate, EKEventSearchCallback block);

		[Export ("predicateForEventsWithStartDate:endDate:calendars:")]
		NSPredicate PredicateForEvents (NSDate startDate, NSDate endDate, [NullAllowed] EKCalendar [] calendars);

		[Field ("EKEventStoreChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }

		[Since (5,0)]
		[Export ("sources")]
		EKSource [] Sources { get; }

		[Since (5,0)]
		[Export ("sourceWithIdentifier:")]
		EKSource GetSource (string identifier);

		[Since (5,0)]
		[Export ("calendarWithIdentifier:")]
		EKCalendar GetCalendar (string identifier);

		[NoWatch]
		[Since (5,0)]
		[Export ("saveCalendar:commit:error:")]
		bool SaveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[Since (5,0)]
		[Export ("removeCalendar:commit:error:")]
		bool RemoveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[Since (5,0)]
		[Export ("saveEvent:span:commit:error:")]
		bool SaveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[Since (5,0)]
		[Export ("removeEvent:span:commit:error:")]
		bool RemoveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[Since (5,0)]
		[Export ("commit:")]
		bool Commit (out NSError error);

		[Since (5,0)]
		[Export ("reset")]
		void Reset ();

		[NoWatch]
		[Since (5,0)]
		[Export ("refreshSourcesIfNecessary")]
		void RefreshSourcesIfNecessary ();

		[Since (6,0)]
		[Export ("calendarItemWithIdentifier:")]
		EKCalendarItem GetCalendarItem (string identifier);

		[Since (6,0)]
		[Export ("calendarItemsWithExternalIdentifier:")]
		EKCalendarItem[] GetCalendarItems(string externalIdentifier);

		[Since (6,0)]
		[Export ("calendarsForEntityType:")]
		EKCalendar[] GetCalendars (EKEntityType entityType);

		[Since (6,0)]
		[Export ("defaultCalendarForNewReminders")]
		EKCalendar DefaultCalendarForNewReminders { get; }

		[Since (6,0)]
		[Export ("fetchRemindersMatchingPredicate:completion:")]
		[Async]
		IntPtr FetchReminders (NSPredicate predicate, Action<EKReminder[]> completion);

		[Since (6,0)]
		[Export ("cancelFetchRequest:")]
		void CancelFetchRequest (IntPtr fetchIdentifier);

		[Since (6,0)]
		[Export ("predicateForIncompleteRemindersWithDueDateStarting:ending:calendars:")]
		NSPredicate PredicateForIncompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar[] calendars);

		[Since (6,0)]
		[Export ("predicateForCompletedRemindersWithCompletionDateStarting:ending:calendars:")]
		NSPredicate PredicateForCompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar[] calendars);

		[Since (6,0)]
		[Export ("predicateForRemindersInCalendars:")]
		NSPredicate PredicateForReminders ([NullAllowed] EKCalendar[] calendars);

		[NoWatch]
		[Since (6,0)]
		[Export ("removeReminder:commit:error:")]
		bool RemoveReminder (EKReminder reminder, bool commit, out NSError error);

		[NoWatch]
		[Since (6,0)]
		[Export ("saveReminder:commit:error:")]
		bool SaveReminder (EKReminder reminder, bool commit, out NSError error);

#if MONOMAC
		[Export ("initWithAccessToEntityTypes:")]
		IntPtr Constructor (EKEntityMask accessToEntityTypes);

		[Mac (10,11)]
		[Export ("delegateSources")]
		EKSource[] DelegateSources { get; }
#endif
		[Since (6,0)]
		[Mac (10,9)]
		[Export ("requestAccessToEntityType:completion:")]
		[Async]
		void RequestAccess (EKEntityType entityType, Action<bool, NSError> completionHandler);

		[Since (6,0)]
		[Mac (10,9)]
		[Static]
		[Export ("authorizationStatusForEntityType:")]
		EKAuthorizationStatus GetAuthorizationStatus (EKEntityType entityType);
	}

	delegate void EKEventSearchCallback (EKEvent theEvent, ref bool stop);

	[Since (6,0)]
	[Mac (10,8, onlyOn64: true)]
	[BaseType (typeof (EKCalendarItem))]
	interface EKReminder {
		[Export ("startDateComponents", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDateComponents StartDateComponents { get; set; }

		[Export ("dueDateComponents", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDateComponents DueDateComponents { get; set; }

		[Export ("completed")]
		bool Completed { [Bind ("isCompleted")] get; set; }

		[Export ("completionDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate CompletionDate { get; set; }

		[Export ("priority")]
		[Mac (10,9)]
		nint Priority { get; set; }
		// note: changed to NUInteger in Xcode 7 SDK

		[Export ("reminderWithEventStore:")]
		[Static]
		EKReminder Create (EKEventStore eventStore);	
	}
}
