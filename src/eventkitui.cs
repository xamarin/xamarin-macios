//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2014 Xamarin Inc. All rights reserved.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using EventKit;
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace EventKitUI {
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (EKEventViewDelegate) })]
	interface EKEventViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed] // by default this property is null
		[Export ("event")]
		EKEvent Event { get; set; }

		[Export ("allowsEditing")]
		bool AllowsEditing { get; set; }

		[Export ("allowsCalendarPreview")]
		bool AllowsCalendarPreview { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		EKEventViewDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EKEventViewDelegate {
		[Abstract]
		[Export ("eventViewController:didCompleteWithAction:"), EventArgs ("EKEventView")]
		void Completed (EKEventViewController controller, EKEventViewAction action);
	}

	[BaseType (typeof (UINavigationController), Delegates = new string [] { "WeakEditViewDelegate" }, Events = new Type [] { typeof (EKEventEditViewDelegate) })]
	interface EKEventEditViewController : UIAppearance {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithRootViewController:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		NativeHandle Constructor (UIViewController rootViewController);

		[Export ("editViewDelegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakEditViewDelegate { get; set; }

		[Wrap ("WeakEditViewDelegate")]
		[Protocolize]
		EKEventEditViewDelegate EditViewDelegate { get; set; }

		[Export ("eventStore")]
		EKEventStore EventStore { get; set; }

		[NullAllowed]
		[Export ("event")]
		EKEvent Event { get; set; }

		[Export ("cancelEditing")]
		void CancelEditing ();
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EKEventEditViewDelegate {
		[Abstract]
		[Export ("eventEditViewController:didCompleteWithAction:"), EventArgs ("EKEventEdit")]
		void Completed (EKEventEditViewController controller, EKEventEditViewAction action);

		[Export ("eventEditViewControllerDefaultCalendarForNewEvents:"), DelegateName ("EKEventEditController"), DefaultValue (null)]
		EKCalendar GetDefaultCalendarForNewEvents (EKEventEditViewController controller);
	}

	[BaseType (typeof (UIViewController),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (EKCalendarChooserDelegate) })]
	interface EKCalendarChooser {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithSelectionStyle:displayStyle:eventStore:")]
		NativeHandle Constructor (EKCalendarChooserSelectionStyle selectionStyle, EKCalendarChooserDisplayStyle displayStyle, EKEventStore eventStore);

		[Export ("initWithSelectionStyle:displayStyle:entityType:eventStore:")]
		NativeHandle Constructor (EKCalendarChooserSelectionStyle selectionStyle, EKCalendarChooserDisplayStyle displayStyle, EKEntityType entityType, EKEventStore eventStore);

		[Export ("selectionStyle")]
		EKCalendarChooserSelectionStyle SelectionStyle {
			get;
#if !XAMCORE_3_0
			[NotImplemented]
			set;
#endif
		}

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		EKCalendarChooserDelegate Delegate { get; set; }

		[Export ("showsDoneButton")]
		bool ShowsDoneButton { get; set; }

		[Export ("showsCancelButton")]
		bool ShowsCancelButton { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("selectedCalendars", ArgumentSemantic.Copy)]
		NSSet SelectedCalendars { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EKCalendarChooserDelegate {
		[Export ("calendarChooserSelectionDidChange:")]
		void SelectionChanged (EKCalendarChooser calendarChooser);

		[Export ("calendarChooserDidFinish:")]
		void Finished (EKCalendarChooser calendarChooser);

		[Export ("calendarChooserDidCancel:")]
		void Cancelled (EKCalendarChooser calendarChooser);
	}

}
