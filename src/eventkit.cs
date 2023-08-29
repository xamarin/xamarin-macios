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

#if !WATCH && !MONOMAC
using EKAlarmType = Foundation.NSObject;
#else
using ABAddressBook = Foundation.NSObject;
using ABRecord = Foundation.NSObject;
#endif
#if !MONOMAC
using NSColor = UIKit.UIColor;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace EventKit {

	[iOS (13, 0), MacCatalyst (13, 1), Watch (6, 0), NoTV]
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
#if NET
	[Abstract] // "The EKCalendarItem class is a an abstract superclass ..." from Apple docs.
#endif
	interface EKCalendarItem {
		// Never made avaialble on MonoMac
		[Export ("UUID")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'CalendarItemIdentifier' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CalendarItemIdentifier' instead.")]
		string UUID { get; }

		[NullAllowed] // by default this property is null
		[Export ("calendar", ArgumentSemantic.Retain)]
		EKCalendar Calendar { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed] // it's null by default on iOS 6.1
		[Export ("location", ArgumentSemantic.Copy)]
		string Location { get; set; }

		[Export ("notes", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Notes { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[NullAllowed]
		[Export ("lastModifiedDate")]
		NSDate LastModifiedDate { get; }

		[NullAllowed, Export ("creationDate", ArgumentSemantic.Strong)]
		NSDate CreationDate { get; }

		[NullAllowed] // by default this property is null
		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("hasAlarms")]
		bool HasAlarms { get; }

		[Export ("hasRecurrenceRules")]
		bool HasRecurrenceRules { get; }

		[Export ("hasAttendees")]
		bool HasAttendees { get; }

		[Export ("hasNotes")]
		bool HasNotes { get; }

		[NullAllowed]
		[Export ("attendees")]
		EKParticipant [] Attendees { get; }

		[NullAllowed] // by default this property is null
		[Export ("alarms", ArgumentSemantic.Copy)]
		EKAlarm [] Alarms { get; set; }

		[NullAllowed]
		[Export ("recurrenceRules", ArgumentSemantic.Copy)]
		EKRecurrenceRule [] RecurrenceRules { get; set; }

		[Export ("addAlarm:")]
		void AddAlarm (EKAlarm alarm);

		[Export ("removeAlarm:")]
		void RemoveAlarm (EKAlarm alarm);

		[Export ("addRecurrenceRule:")]
		void AddRecurrenceRule (EKRecurrenceRule rule);

		[Export ("removeRecurrenceRule:")]
		void RemoveRecurrenceRule (EKRecurrenceRule rule);

		[Export ("calendarItemIdentifier")]
		string CalendarItemIdentifier { get; }

		[Export ("calendarItemExternalIdentifier")]
		string CalendarItemExternalIdentifier { get; }
	}

	[BaseType (typeof (EKObject))]
	interface EKSource {
		[Export ("sourceType")]
		EKSourceType SourceType { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("calendars")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'GetCalendars (EKEntityType)' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetCalendars (EKEntityType)' instead.")]
		NSSet Calendars { get; }

		[Export ("sourceIdentifier")]
		string SourceIdentifier { get; }

		[Export ("calendarsForEntityType:")]
		NSSet GetCalendars (EKEntityType entityType);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), NoTV]
		[Export ("isDelegate", ArgumentSemantic.Assign)]
		bool IsDelegate { get; }
	}

	[BaseType (typeof (EKObject))]
	interface EKStructuredLocation : NSCopying {
		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Strong)]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("geoLocation", ArgumentSemantic.Strong)]
		CLLocation GeoLocation { get; set; }

		[Export ("radius")]
		double Radius { get; set; }

		[Export ("locationWithTitle:"), Static]
		EKStructuredLocation FromTitle (string title);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("locationWithMapItem:")]
		EKStructuredLocation FromMapItem (MKMapItem mapItem);
	}

	[BaseType (typeof (EKObject))]
	[DisableDefaultCtor] // Documentation says to use the static methods FromDate/FromTimeInterval to create instances
	interface EKAlarm : NSCopying {
		[Export ("relativeOffset")]
		double RelativeOffset { get; set; }

		[Export ("absoluteDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate AbsoluteDate { get; set; }

		[Static]
		[Export ("alarmWithAbsoluteDate:")]
		EKAlarm FromDate (NSDate date);

		[Static]
		[Export ("alarmWithRelativeOffset:")]
		EKAlarm FromTimeInterval (double offsetSeconds);

		[Export ("structuredLocation", ArgumentSemantic.Copy)]
		[NullAllowed]
		EKStructuredLocation StructuredLocation { get; set; }

		[Export ("proximity")]
		EKAlarmProximity Proximity { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("type")]
		EKAlarmType Type { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[NullAllowed]
		[Export ("emailAddress")]
		string EmailAddress { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[NullAllowed]
		[Export ("soundName")]
		string SoundName { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[NullAllowed]
		[Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }
	}

	[BaseType (typeof (EKObject))]
	[DisableDefaultCtor]
	interface EKCalendar {
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("type")]
		EKCalendarType Type { get; }

		[Export ("allowsContentModifications")]
		bool AllowsContentModifications { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("CGColor")]
		CGColor CGColor { get; set; }

		[Export ("supportedEventAvailabilities")]
		EKCalendarEventAvailability SupportedEventAvailabilities { get; }

		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get; }

		[Export ("subscribed")]
		bool Subscribed { [Bind ("isSubscribed")] get; }

		[Export ("immutable")]
		bool Immutable { [Bind ("isImmutable")] get; }

		[NoMac]
		[NoMacCatalyst] // It's in the documentation and headers, but throws a "+[EKCalendar calendarWithEventStore:]: unrecognized selector" exception at runtime
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'Create (EKEntityType, EKEventStore)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Create (EKEntityType, EKEventStore)' instead.")]
		[Static, Export ("calendarWithEventStore:")]
		EKCalendar FromEventStore (EKEventStore eventStore);

		[Export ("source", ArgumentSemantic.Retain)]
		EKSource Source { get; set; }

		[Export ("allowedEntityTypes")]
		EKEntityMask AllowedEntityTypes { get; }

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
		bool AllDay { [Bind ("isAllDay")] get; set; }

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; set; }

		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; set; }

		[NullAllowed]
		[Export ("organizer")]
		EKParticipant Organizer { get; }

		[Export ("isDetached")]
		bool IsDetached { get; }

		[Export ("eventIdentifier")]
		string EventIdentifier { get; }

		[Export ("compareStartDateWithEvent:")]
		NSComparisonResult CompareStartDateWithEvent (EKEvent other);

		[Export ("refresh")]
		bool Refresh ();

		[Export ("availability")]
		EKEventAvailability Availability { get; set; }

		[Export ("status")]
		EKEventStatus Status { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("structuredLocation", ArgumentSemantic.Copy)]
		EKStructuredLocation StructuredLocation { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("occurrenceDate")]
		NSDate OccurrenceDate { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Replaced by 'BirthdayContactIdentifier'.")]
		[NullAllowed]
		[Export ("birthdayPersonUniqueID")]
		string BirthdayPersonUniqueID { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Replaced by 'BirthdayContactIdentifier'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'BirthdayContactIdentifier'.")]
		[Export ("birthdayPersonID")]
		nint BirthdayPersonID { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("birthdayContactIdentifier")]
		string BirthdayContactIdentifier { get; }
	}

	[BaseType (typeof (EKObject))]
#if XAMCORE_3_0
	[DisableDefaultCtor]
#endif
	interface EKParticipant : NSCopying {
		[Export ("URL")]
		NSUrl Url { get; }

		[NullAllowed]
		[Export ("name")]
		string Name { get; }

		[Export ("participantStatus")]
		EKParticipantStatus ParticipantStatus { get; }

		[Export ("participantRole")]
		EKParticipantRole ParticipantRole { get; }

		[Export ("participantType")]
		EKParticipantType ParticipantType { get; }

		[NoMac]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Replaced by 'ContactPredicate'.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'ContactPredicate'.")]
		[return: NullAllowed]
		[Export ("ABRecordWithAddressBook:")]
		ABRecord GetRecord (ABAddressBook addressBook);

		[MacCatalyst (13, 1)]
		[Export ("isCurrentUser")]
		bool IsCurrentUser { get; }

		[MacCatalyst (13, 1)]
		[Export ("contactPredicate")]
		NSPredicate ContactPredicate { get; }
	}

	[BaseType (typeof (NSObject))]
	interface EKRecurrenceEnd : NSCopying, NSSecureCoding {
		[NullAllowed]
		[Export ("endDate")]
		NSDate EndDate { get; }

		[Export ("occurrenceCount")]
		nint OccurrenceCount { get; }

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
#if NET
		EKWeekday DayOfTheWeek { get;  }
#else
		nint DayOfTheWeek { get; }
#endif

		[Export ("weekNumber")]
		nint WeekNumber { get; }

		[Static]
		[Export ("dayOfWeek:")]
#if NET
		EKRecurrenceDayOfWeek FromDay (EKWeekday dayOfTheWeek);
#else
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek);
#endif

		[Static]
		[Export ("dayOfWeek:weekNumber:")]
#if NET
		EKRecurrenceDayOfWeek FromDay (EKWeekday dayOfTheWeek, nint weekNumber);
#else
		[Internal]
		EKRecurrenceDayOfWeek _FromDay (nint dayOfTheWeek, nint weekNumber);
#endif

		[Export ("initWithDayOfTheWeek:weekNumber:")]
#if NET
		NativeHandle Constructor (EKWeekday dayOfTheWeek, nint weekNumber);
#else
		NativeHandle Constructor (nint dayOfTheWeek, nint weekNumber);
#endif
	}

	[BaseType (typeof (EKObject))]
	interface EKRecurrenceRule : NSCopying {
		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get; }

		[NullAllowed] // by default this property is null
		[Export ("recurrenceEnd", ArgumentSemantic.Copy)]
		EKRecurrenceEnd RecurrenceEnd { get; set; }

		[Export ("frequency")]
		EKRecurrenceFrequency Frequency { get; }

		[Export ("interval")]
		nint Interval { get; }

		[Export ("firstDayOfTheWeek")]
#if NET
		EKWeekday FirstDayOfTheWeek { get; }
#else
		[Internal]
		nint _FirstDayOfTheWeek { get; }
#endif

		[NullAllowed]
		[Export ("daysOfTheWeek")]
		EKRecurrenceDayOfWeek [] DaysOfTheWeek { get; }

		[NullAllowed]
		[Export ("daysOfTheMonth")]
		NSNumber [] DaysOfTheMonth { get; }

		[NullAllowed]
		[Export ("daysOfTheYear")]
		NSNumber [] DaysOfTheYear { get; }

		[NullAllowed]
		[Export ("weeksOfTheYear")]
		NSNumber [] WeeksOfTheYear { get; }

		[NullAllowed]
		[Export ("monthsOfTheYear")]
		NSNumber [] MonthsOfTheYear { get; }

		[NullAllowed]
		[Export ("setPositions")]
#if NET
		NSNumber [] SetPositions { get; }
#else
		NSObject [] SetPositions { get; }
#endif

		[Export ("initRecurrenceWithFrequency:interval:end:")]
		NativeHandle Constructor (EKRecurrenceFrequency type, nint interval, [NullAllowed] EKRecurrenceEnd end);

		[Export ("initRecurrenceWithFrequency:interval:daysOfTheWeek:daysOfTheMonth:monthsOfTheYear:weeksOfTheYear:daysOfTheYear:setPositions:end:")]
		NativeHandle Constructor (EKRecurrenceFrequency type, nint interval, [NullAllowed] EKRecurrenceDayOfWeek [] days, [NullAllowed] NSNumber [] monthDays, [NullAllowed] NSNumber [] months,
					[NullAllowed] NSNumber [] weeksOfTheYear, [NullAllowed] NSNumber [] daysOfTheYear, [NullAllowed] NSNumber [] setPositions, [NullAllowed] EKRecurrenceEnd end);

	}

	[BaseType (typeof (NSObject))]
	interface EKEventStore {
		[iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), NoTV]
		[Export ("initWithSources:")]
		NativeHandle Constructor (EKSource [] sources);

		[Export ("eventStoreIdentifier")]
		string EventStoreIdentifier { get; }

		[NoMac]
		[Export ("calendars")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'GetCalendars' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetCalendars' instead.")]
		EKCalendar [] Calendars { get; }

		[Export ("defaultCalendarForNewEvents"), NullAllowed]
		EKCalendar DefaultCalendarForNewEvents { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("saveEvent:span:error:")]
		bool SaveEvent (EKEvent theEvent, EKSpan span, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Export ("saveCalendar:commit:error:")]
		bool SaveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("removeCalendar:commit:error:")]
		bool RemoveCalendar (EKCalendar calendar, bool commit, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("saveEvent:span:commit:error:")]
		bool SaveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("removeEvent:span:commit:error:")]
		bool RemoveEvent (EKEvent ekEvent, EKSpan span, bool commit, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("commit:")]
		bool Commit (out NSError error);

		[Export ("reset")]
		void Reset ();

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("refreshSourcesIfNecessary")]
		void RefreshSourcesIfNecessary ();

		[return: NullAllowed]
		[Export ("calendarItemWithIdentifier:")]
		EKCalendarItem GetCalendarItem (string identifier);

		[Export ("calendarItemsWithExternalIdentifier:")]
		EKCalendarItem [] GetCalendarItems (string externalIdentifier);

		[Export ("calendarsForEntityType:")]
		EKCalendar [] GetCalendars (EKEntityType entityType);

		[NullAllowed]
		[Export ("defaultCalendarForNewReminders")]
		EKCalendar DefaultCalendarForNewReminders { get; }

		[Export ("fetchRemindersMatchingPredicate:completion:")]
		[Async]
		IntPtr FetchReminders (NSPredicate predicate, Action<EKReminder []> completion);

		[Export ("cancelFetchRequest:")]
		void CancelFetchRequest (IntPtr fetchIdentifier);

		[Export ("predicateForIncompleteRemindersWithDueDateStarting:ending:calendars:")]
		NSPredicate PredicateForIncompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar [] calendars);

		[Export ("predicateForCompletedRemindersWithCompletionDateStarting:ending:calendars:")]
		NSPredicate PredicateForCompleteReminders ([NullAllowed] NSDate startDate, [NullAllowed] NSDate endDate, [NullAllowed] EKCalendar [] calendars);

		[Export ("predicateForRemindersInCalendars:")]
		NSPredicate PredicateForReminders ([NullAllowed] EKCalendar [] calendars);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("removeReminder:commit:error:")]
		bool RemoveReminder (EKReminder reminder, bool commit, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("saveReminder:commit:error:")]
		bool SaveReminder (EKReminder reminder, bool commit, out NSError error);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("initWithAccessToEntityTypes:")]
		NativeHandle Constructor (EKEntityMask accessToEntityTypes);

		[Watch (5, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("delegateSources")]
		EKSource [] DelegateSources { get; }

		[MacCatalyst (13, 1)]
		[Deprecated(PlatformName.iOS, 17, 0, message: "Use RequestFullAccessToEventsWithCompletion, RequestWriteOnlyAccessToEventsWithCompletion, or RequestFullAccessToRemindersWithCompletion.")]
		[Deprecated(PlatformName.MacOSX, 14, 0, message: "Use RequestFullAccessToEventsWithCompletion, RequestWriteOnlyAccessToEventsWithCompletion, or RequestFullAccessToRemindersWithCompletion.")]
		[Deprecated(PlatformName.WatchOS, 10, 0, message: "Use RequestFullAccessToEventsWithCompletion, RequestWriteOnlyAccessToEventsWithCompletion, or RequestFullAccessToRemindersWithCompletion.")]
		[Deprecated(PlatformName.MacCatalyst, 17, 0, message: "Use RequestFullAccessToEventsWithCompletion, RequestWriteOnlyAccessToEventsWithCompletion, or RequestFullAccessToRemindersWithCompletion.")]
		[Export ("requestAccessToEntityType:completion:")]
		[Async]
		void RequestAccess (EKEntityType entityType, Action<bool, NSError> completionHandler);

		[Watch(10, 0), Mac(14, 0), iOS(17, 0), MacCatalyst(17,0)]
		[Export("requestFullAccessToEventsWithCompletion:")]
		[Async]
		void RequestFullAccessToEventsWithCompletion(EKEventStoreRequestAccessCompletionHandler completion);

		[Watch(10, 0), Mac(14, 0), iOS(17, 0), MacCatalyst(17, 0)]
		[Export("requestWriteOnlyAccessToEventsWithCompletion:")]
		[Async]
		void RequestWriteOnlyAccessToEventsWithCompletion(EKEventStoreRequestAccessCompletionHandler completion);

		[Watch(10, 0), Mac(14, 0), iOS(17, 0), MacCatalyst(17, 0)]
		[Export("requestFullAccessToRemindersWithCompletion:")]
		[Async]
		void RequestFullAccessToRemindersWithCompletion(EKEventStoreRequestAccessCompletionHandler completion);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("authorizationStatusForEntityType:")]
		EKAuthorizationStatus GetAuthorizationStatus (EKEntityType entityType);
	}

	delegate void EKEventStoreRequestAccessCompletionHandler(bool didRequestAccess, NSError error);
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
		[MacCatalyst (13, 1)]
		nint Priority { get; set; }
		// note: changed to NUInteger in Xcode 7 SDK

		[Export ("reminderWithEventStore:")]
		[Static]
		EKReminder Create (EKEventStore eventStore);
	}

	[Mac (12, 0), iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface EKVirtualConferenceDescriptor {
		[Export ("initWithTitle:URLDescriptors:conferenceDetails:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, EKVirtualConferenceUrlDescriptor [] urlDescriptors, [NullAllowed] string conferenceDetails);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("URLDescriptors", ArgumentSemantic.Copy)]
		EKVirtualConferenceUrlDescriptor [] UrlDescriptors { get; }

		[NullAllowed, Export ("conferenceDetails")]
		string ConferenceDetails { get; }
	}

	delegate void VirtualConferenceRoomTypeHandler (NSArray<EKVirtualConferenceRoomTypeDescriptor> virtualConferenceRoomTypeDescriptor, NSError error);
	delegate void VirtualConferenceHandler (EKVirtualConferenceDescriptor virtualConferenceDescriptor, NSError error);

	[Mac (12, 0), iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0), NoTV]
	[BaseType (typeof (NSObject))]
	interface EKVirtualConferenceProvider : NSExtensionRequestHandling {
		[Async]
		[Export ("fetchAvailableRoomTypesWithCompletionHandler:")]
		void FetchAvailableRoomTypes (VirtualConferenceRoomTypeHandler handler);

		[Async]
		[Export ("fetchVirtualConferenceForIdentifier:completionHandler:")]
		void FetchVirtualConference (string identifier, VirtualConferenceHandler handler);
	}

	[Mac (12, 0), iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface EKVirtualConferenceRoomTypeDescriptor {
		[Export ("initWithTitle:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, string identifier);

		[Export ("title")]
		string Title { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[Mac (12, 0), iOS (15, 0), Watch (8, 0), MacCatalyst (15, 0), NoTV]
	[BaseType (typeof (NSObject), Name = "EKVirtualConferenceURLDescriptor")]
	[DisableDefaultCtor]
	interface EKVirtualConferenceUrlDescriptor {
		[Export ("initWithTitle:URL:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, NSUrl url);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }
	}
}
