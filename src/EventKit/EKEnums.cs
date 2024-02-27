using System;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;

#if !MONOMAC
using UIKit;
#endif

namespace EventKit {

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarType : long {
		Local, CalDav, Exchange, Subscription, Birthday
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantType : long {
		Unknown, Person, Room, Resource, Group
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantRole : long {
		Unknown, Required, Optional, Chair, NonParticipant
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKParticipantStatus : long {
		Unknown, Pending, Accepted, Declined,
		Tentative, Delegated, Completed, InProcess
	}

	// untyped enum -> EKError.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	[ErrorDomain ("EKErrorDomain")]
	public enum EKErrorCode : long {
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
		InvalidInviteReplyCalendar,
		NotificationsCollectionFlagNotSet,
		SourceMismatch,
		NotificationCollectionMismatch,
		NotificationSavedWithoutCollection,
		ReminderAlarmContainsEmailOrUrl,
	}

	// untyped enum -> EKTypes.h
	// Special note: some API (like `dayOfWeek:` and `dayOfWeek:weekNumber:` use an `NSInteger` instead of the enum
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'EKWeekday'.")]
	[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'EKWeekday'.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EKWeekday'.")]
	public enum EKDay {
		NotSet = 0,
		Sunday = 1,
		Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
	}

	[MacCatalyst (13, 1)]
	[Native] // NSInteger (size change from previously untyped enum)
	public enum EKWeekday : long {
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
	public enum EKRecurrenceFrequency : long {
		Daily, Weekly, Monthly, Yearly
	}

	// untyped enum -> EKEventStore.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKSpan : long {
		ThisEvent, FutureEvents
	}

	// NSUInteger -> EKTypes.h
	[Native ("EKCalendarEventAvailabilityMask")]
	[Flags]
	public enum EKCalendarEventAvailability : ulong {
		None = 0,
		Busy = 1,
		Free = 2,
		Tentative = 4,
		Unavailable = 8
	}

	// untyped enum -> EKEvent.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventAvailability : long {
		NotSupported = -1,
		Busy = 0, Free, Tentative, Unavailable,
	}

	// untyped enum -> EKEvent.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventStatus : long {
		None, Confirmed, Tentative, Cancelled
	}

	// untyped enum -> EKTypes.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKSourceType : long {
		Local, Exchange, CalDav, MobileMe, Subscribed, Birthdays
	}

	// NSInteger -> EKTypes.h
	[Native]
	public enum EKAlarmProximity : long {
		None, Enter, Leave
	}

	// NSUInteger -> EKTypes.h
	[Native]
	[Flags]
	public enum EKEntityMask : ulong {
		Event = 1 << (int) EKEntityType.Event,
		Reminder = 1 << (int) EKEntityType.Reminder
	}

	// NSUInteger -> EKTypes.h
	[Native]
	public enum EKEntityType : ulong {
		Event,
		Reminder
	}

#if MONOMAC || WATCH
	// untyped enum -> EKTypes.h (but not in the iOS SDK, only OSX)
	// turned into a typed (NSInteger) enum in El Capitan (and also an NSInteger in watchOS)
	[Native]
	public enum EKAlarmType : long {
		Display, Audio, Procedure, Email
	}
#endif
	// NSInteger -> EKEventStore.h
	[Native]
	public enum EKAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized,
		WriteOnly,
	}

	[Native]
	public enum EKParticipantScheduleStatus : long {
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
	public enum EKReminderPriority : ulong {
		None = 0,
		High = 1,
		Medium = 5,
		Low = 9
	}

}
