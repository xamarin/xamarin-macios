using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;

#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.EventKit {

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarType : nint {
		Local, CalDav, Exchange, Subscription, Birthday
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantType : nint {
		Unknown, Person, Room, Resource, Group
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantRole : nint {
		Unknown, Required, Optional, Chair, NonParticipant
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantStatus : nint {
		Unknown, Pending, Accepted, Declined,
		Tentative, Delegated, Completed, InProcess
	}

	// untyped enum -> EKError.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	[ErrorDomain ("EKErrorDomain")]
	public enum EKErrorCode : nint {
		EventNotMutable,
		NoCalendar,
		NoStartDate,
		NoEndDate,
		DatesInverted,
		InternalFailure,
		CalendarReadOnly,
		DurationGreaterThanRecurrence,
		AlarmGreaterThanRecurrence,
		StartDateTooFarInFuture,
		StartDateCollidesWithOtherOccurrence,
		ObjectBelongsToDifferentStore,
		InvitesCannotBeMoved,
		InvalidSpan,
		CalendarHasNoSource,
		CalendarSourceCannotBeModified,
		CalendarIsImmutable,
		SourceDoesNotAllowCalendarAddDelete,
		RecurringReminderRequiresDueDate,
		StructuredLocationsNotSupported,
 		ReminderLocationsNotSupported,
		AlarmProximityNotSupported,
		CalendarDoesNotAllowEvents,
		CalendarDoesNotAllowReminders,
		SourceDoesNotAllowReminders,
		SourceDoesNotAllowEvents,
		PriorityIsInvalid,
		InvalidEntityType,
		ProcedureAlarmsNotMutable,
		EventStoreNotAuthorized,
		OSNotSupported,
	}

	// untyped enum -> EKTypes.h
	// Special note: some API (like `dayOfWeek:` and `dayOfWeek:weekNumber:` use an `NSInteger` instead of the enum
	[Availability (Deprecated = Platform.iOS_9_0 | Platform.Mac_10_11, Message = "Use EKWeekday")]
	public enum EKDay {
		NotSet = 0,
		Sunday = 1,
		Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
	}

	[iOS (9,0)][Mac (10,11)]
	[Native] // NSInteger (size change from previously untyped enum)
	public enum EKWeekday : nint {
		NotSet = 0,
		Sunday = 1,
		Monday,
		Tuesday,
		Wednesday,
		Thursday,
		Friday,
		Saturday,
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKRecurrenceFrequency : nint {
		Daily, Weekly, Monthly, Yearly
	}

	// untyped enum -> EKEventStore.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKSpan : nint {
		ThisEvent, FutureEvents
	}

	// note: old binding mistakes - they should have been in EventKitUI (not EventKit)
#if !XAMCORE_2_0
	// untyped enum -> EKEventViewController.h
	public enum EKEventViewAction {
		Done, Responded, Deleted
	}

	// untyped enum -> EKEventEditViewController.h
	public enum EKEventEditViewAction {
		Canceled, Saved, Deleted
	}
#endif

	// NSUInteger -> EKTypes.h
	[Native]
	[Flags]
	public enum EKCalendarEventAvailability : nuint_compat_int {
		None = 0,
		Busy = 1,
		Free = 2,
		Tentative = 4,
		Unavailable = 8
	}

	// untyped enum -> EKEvent.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventAvailability : nint {
		NotSupported = -1,
		Busy = 0, Free, Tentative, Unavailable,
	}

	// untyped enum -> EKEvent.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventStatus : nint {
		None, Confirmed, Tentative, Cancelled
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKSourceType : nint {
		Local, Exchange, CalDav, MobileMe, Subscribed, Birthdays
	}

	// NSInteger -> EKTypes.h
	[Native]
	public enum EKAlarmProximity : nint {
		None, Enter, Leave
	}

	// NSUInteger -> EKTypes.h
	[Native]
	[Flags]
	public enum EKEntityMask : nuint_compat_int {
		Event = 1 << (int) EKEntityType.Event,
		Reminder = 1 << (int) EKEntityType.Reminder
	}

	// NSUInteger -> EKTypes.h
	[Native]
	public enum EKEntityType : nuint_compat_int {
		Event,
		Reminder
	}

#if MONOMAC || WATCH
	// untyped enum -> EKTypes.h (but not in the iOS SDK, only OSX)
	// turned into a typed (NSInteger) enum in El Capitan (and also an NSInteger in watchOS)
	[Native]
	public enum EKAlarmType : nint {
		Display, Audio, Procedure, Email
	}
#endif
	// NSInteger -> EKEventStore.h
	[Native]
	public enum EKAuthorizationStatus : nint {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
	}

	[Native]
	public enum EKParticipantScheduleStatus : nint
	{
		None,
		Pending,
		Sent,
		Delivered,
		RecipientNotRecognized,
		NoPrivileges,
		DeliveryFailed,
		CannotDeliver,
		RecipientNotAllowed
	}

	[Native]
	public enum EKReminderPriority : nuint
	{
		None = 0,
		High = 1,
		Medium = 5,
		Low = 9
	}
	
}
