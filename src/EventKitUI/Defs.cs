//
// Defs.cs: Core Definitions for buildling the API eventkitui.cs
//
// Authors:
//   Miguel de Icaza
// 
// Copyright 2011-2015 Xamarin Inc.
//

using XamCore.ObjCRuntime;

namespace XamCore.EventKitUI {

	// untyped enum -> EKCalendarChooser.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarChooserSelectionStyle : nint {
		Single, Multiple
	}
	
	// untyped enum -> EKCalendarChooser.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarChooserDisplayStyle : nint {
		AllCalendars, WritableCalendarsOnly
	}

	// note: old binding mistake - this should have been in EventKitUI (not EventKit)
#if XAMCORE_2_0
	// untyped enum -> EKEventViewController.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventViewAction : nint {
		Done, Responded, Deleted
	}

	// untyped enum -> EKEventEditViewController.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventEditViewAction : nint {
		Canceled, Saved, Deleted
	}
#endif
}