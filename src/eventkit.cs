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

#if !WATCH && !MONOMAC
using AddressBook;
#endif
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using MapKit;
using System;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace EventKit {

	[BaseType (typeof (NSObject))]
	[Abstract]
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

	[BaseType (typeof (EKObject))]
#if XAMCORE_4_0	
	[Abstract] // "The EKCalendarItem class is a an abstract superclass ..." from Apple docs.
#endif
	interface EKCalendarItem {
#if !MONOMAC
		// Never made avaialble on MonoMac
		[Export ("UUID")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'CalendarItemIdentifier' instead.")]
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

		[NullAllowed]
		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get;  }

		[NullAllowed, Export ("creationDate", ArgumentSemantic.Strong)]
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

		[NullAllowed]
		[Export ("attendees")]
		EKParticipant [] Attendees { get;  }

		[NullAllowed] // by default this property is null
		[Export ("alarms", ArgumentSemantic.Copy)]
		EKAlarm [] Alarms { get; set;  }

		[NullAllowed]
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

		[Export ("calendarItemIdentifier")]
		string CalendarItemIdentifier { get;  }

		[Export ("calendarItemExternalIdentifier")]
		string CalendarItemExternalIdentifier { get;  }
	}
	
	[BaseType (typeof (EKObject))]
	interface EKSource {
		[Export ("sourceType")]
		EKSourceType SourceType { get;  }

		[Export ("title")]
		string Title { get;  }

#if !MONOMAC
		[Export ("calendars")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'GetCalendars (EKEntityType)' instead.")]
		NSSet Calendars { get;  }
#endif

		[Export ("sourceIdentifier")]
		string SourceIdentifier { get; }

		[Export ("calendarsForEntityType:")]
		NSSet GetCalendars (EKEntityType entityType);
	}

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

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("locationWithMapItem:")]
		EKStructuredLocation FromMapItem (MKMapItem mapItem);
	}

	[BaseType (typeof (EKObject))]
	[DisableDefaultCtor] // Documentation says to use the static methods FromDate/FromTimeInterval to create instances
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

		[Export ("structuredLocation", ArgumentSemantic.Copy)]
		[NullAllowed]
		EKStructuredLocation StructuredLocation { get; set;  }

		[Export ("proximity")]
		EKAlarmProximity Proximity { get; set;  }

#if MONOMAC
		[Export ("type")]
		EKAlarmType Type { get; }

		[NullAllowed]
		[Export ("emailAddress")]
		string EmailAddress { get; set; }

		[NullAllowed]
		[Export ("soundName")]
		string SoundName { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[NullAllowed]
		[Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }
#endif
	}

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
#endif
		[Mac (10, 15)]
		[Export ("CGColor")]
		CGColor CGColor { get; set; }
		
		[Export ("supportedEventAvailabilities")]
		EKCalendarEventAvailability SupportedEventAvailabilities { get; }

		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get;  }

		[Export ("subscribed")]
		bool Subscribed { [Bind ("isSubscribed")] get;  }

		[Export ("immutable")]
		bool Immutable { [Bind ("isImmutable")] get;  }

#if !MONOMAC
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'Create (EKEntityType, EKEventStore)' instead.")]
		[Static, Export ("calendarWithEventStore:")]
		EKCalendar FromEventStore (EKEventStore eventStore);
#endif

		[Export ("source", ArgumentSemantic.Retain)]
		EKSource Source { get; set; }

		[Export ("allowedEntityTypes")]
		EKEntityMask AllowedEntityTypes { get;  }

		[Static]
		[Export ("calendarForEntityType:eventStore:")]
		EKCalendar Create (EKEntityType entityType, EKEventStore eventStore);
	}

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

		[NullAllowed]
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
		
		[iOS (9,0)]
		[Export ("occurrenceDate")]
		NSDate OccurrenceDate { get; }

#if MONOMAC
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Replaced by 'BirthdayContactIdentifier'.")]
		[NullAllowed]
		[Export ("birthdayPersonUniqueID")]
		string BirthdayPersonUniqueID { get; }
#else
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Replaced by 'BirthdayContactIdentifier'.")]
		[Export ("birthdayPersonID")]
		nint BirthdayPersonID { get;  }
#endif
		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("birthdayContactIdentifier")]
		string BirthdayContactIdentifier { get; }
	}

	[BaseType (typeof (EKObject))]
#if XAMCORE_3_0
	[DisableDefaultCtor]
#endif
	interface EKParticipant : NSCopying {
		[Export ("URL")]
		NSUrl Url { get;  }

		[NullAllowed]
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
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Replaced by 'ContactPredicate'.")]
		[return: NullAllowed]
		[Export ("ABRecordWithAddressBook:")]
		ABRecord GetRecord (ABAddressBook addressBook);
#endif // !WATCH

#endif
		[Mac (10,9)]
		[Export ("isCurrentUser")]
		bool IsCurrentUser { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("contactPredicate")]
		NSPredicate ContactPredicate { get; }
	}

	[BaseType (typeof (NSObject))]
	interface EKRecurrenceEnd : NSCopying, NSSecureCoding {
		[NullAllowed]
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

	[BaseType (typeof (NSObject))]
	interface EKRecurrenceDayOfWeek : NSCopying, NSSecureCoding {
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
#else
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek);
#endif

		[Static]
		[Export ("dayOfWeek:weekNumber:")]
#if XAMCORE_4_0
		EKRecurrenceDayOfWeek FromDay (EKWeekday dayOfTheWeek, nint weekNumber);
#else
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek, nint weekNumber);
#endif

		[Export ("initWithDayOfTheWeek:weekNumber:")]
#if XAMCORE_4_0
		IntPtr Constructor (EKWeekday dayOfTheWeek, nint weekNumber);
#else
		IntPtr Constructor (nint dayOfTheWeek, nint weekNumber);
#endif
	}

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
#else
		[Internal]
		nint _FirstDayOfTheWeek { get; }
#endif

		[NullAllowed]
		[Export ("daysOfTheWeek")]
		EKRecurrenceDayOfWeek [] DaysOfTheWeek { get;  }

		[NullAllowed]
		[Export ("daysOfTheMonth")]
		NSNumber [] DaysOfTheMonth { get;  }

		[NullAllowed]
		[Export ("daysOfTheYear")]
		NSNumber [] DaysOfTheYear { get;  }

		[NullAllowed]
		[Export ("weeksOfTheYear")]
		NSNumber [] WeeksOfTheYear { get;  }

		[NullAllowed]
		[Export ("monthsOfTheYear")]
		NSNumber [] MonthsOfTheYear { get;  }

		[NullAllowed]
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

	[BaseType (typeof (NSObject))]
	interface EKEventStore {
		[NoiOS, Mac (10,11), NoWatch]
		[Export ("initWithSources:")]
		IntPtr Constructor (EKSource[] sources);

		[Export ("eventStoreIdentifier")]
		string EventStoreIdentifier { get;  }

#if !MONOMAC
		[Export ("calendars")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'GetCalendars' instead.")]
		EKCalendar [] Calendars { get;  }
#endif

		[Export ("defaultCalendarForNewEvents"), NullAllowed]
		EKCalendar DefaultCalendarForNewEvents { get;  }
		
		[NoWatch, Mac (10,15)]
		[Export ("saveEvent:span:error:")]
		bool SaveEvent (EKEvent theEvent, EKSpan span, out NSError error);

		[NoWatch, Mac (10,15)]
		[Export ("removeEvent:span:error:")]
		bool RemoveEvents (EKEvent theEvent, EKSpan span, out NSError error);

		[return: NullAllowed]
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

		[Export ("sources")]
		EKSource [] Sources { get; }

		[return: NullAllowed]
		[Export ("sourceWithIdentifier:")]
		EKSource GetSource (string identifier);

		[return: NullAllowed]
		[Export ("calendarWithIdentifier:")]
		EKCalendar GetCalendar (string identifier);

		[NoWatch]
		[Export ("saveCalendar:commit:error:")]
		bool SaveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[Export ("removeCalendar:commit:error:")]
		bool RemoveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[Export ("saveEvent:span:commit:error:")]
		bool SaveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[Export ("removeEvent:span:commit:error:")]
		bool RemoveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[Export ("commit:")]
		bool Commit (out NSError error);

		[Export ("reset")]
		void Reset ();

		[NoWatch]
		[Export ("refreshSourcesIfNecessary")]
		void RefreshSourcesIfNecessary ();

		[return: NullAllowed]
		[Export ("calendarItemWithIdentifier:")]
		EKCalendarItem GetCalendarItem (string identifier);

		[Export ("calendarItemsWithExternalIdentifier:")]
		EKCalendarItem[] GetCalendarItems(string externalIdentifier);

		[Export ("calendarsForEntityType:")]
		EKCalendar[] GetCalendars (EKEntityType entityType);

		[NullAllowed]
		[Export ("defaultCalendarForNewReminders")]
		EKCalendar DefaultCalendarForNewReminders { get; }

		[Export ("fetchRemindersMatchingPredicate:completion:")]
		[Async]
		IntPtr FetchReminders (NSPredicate predicate, Action<EKReminder[]> completion);

		[Export ("cancelFetchRequest:")]
		void CancelFetchRequest (IntPtr fetchIdentifier);

		[Export ("predicateForIncompleteRemindersWithDueDateStarting:ending:calendars:")]
		NSPredicate PredicateForIncompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar[] calendars);

		[Export ("predicateForCompletedRemindersWithCompletionDateStarting:ending:calendars:")]
		NSPredicate PredicateForCompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar[] calendars);

		[Export ("predicateForRemindersInCalendars:")]
		NSPredicate PredicateForReminders ([NullAllowed] EKCalendar[] calendars);

		[NoWatch]
		[Export ("removeReminder:commit:error:")]
		bool RemoveReminder (EKReminder reminder, bool commit, out NSError error);

		[NoWatch]
		[Export ("saveReminder:commit:error:")]
		bool SaveReminder (EKReminder reminder, bool commit, out NSError error);

#if MONOMAC
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("initWithAccessToEntityTypes:")]
		IntPtr Constructor (EKEntityMask accessToEntityTypes);
#endif
		[Mac (10,11), Watch (5,0), iOS (12,0)]
		[Export ("delegateSources")]
		EKSource[] DelegateSources { get; }

		[Mac (10,9)]
		[Export ("requestAccessToEntityType:completion:")]
		[Async]
		void RequestAccess (EKEntityType entityType, Action<bool, NSError> completionHandler);

		[Mac (10,9)]
		[Static]
		[Export ("authorizationStatusForEntityType:")]
		EKAuthorizationStatus GetAuthorizationStatus (EKEntityType entityType);
	}

	delegate void EKEventSearchCallback (EKEvent theEvent, ref bool stop);

	[BaseType (typeof (EKCalendarItem))]
	[DisableDefaultCtor]
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
