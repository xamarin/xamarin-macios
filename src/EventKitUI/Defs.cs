//
// Defs.cs: Core Definitions for buildling the API eventkitui.cs
//
// Authors:
//   Miguel de Icaza
// 
// Copyright 2011-2015 Xamarin Inc.
//

#nullable enable

using ObjCRuntime;

namespace EventKitUI {

	// untyped enum -> EKCalendarChooser.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarChooserSelectionStyle : long {
		Single, Multiple
	}

	// untyped enum -> EKCalendarChooser.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKCalendarChooserDisplayStyle : long {
		AllCalendars, WritableCalendarsOnly
	}

	// untyped enum -> EKEventViewController.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventViewAction : long {
		Done, Responded, Deleted
	}

	// untyped enum -> EKEventEditViewController.h
	// iOS 9 promoted this to an NSInteger - which breaks compatibility
	[Native]
	public enum EKEventEditViewAction : long {
		Canceled, Saved, Deleted
	}
}
