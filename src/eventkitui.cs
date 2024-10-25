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
	/// <summary>UIViewController used to display the details of a calendar event.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKEventViewControllerClassRef/index.html">Apple documentation for <c>EKEventViewController</c></related>
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
		IEKEventViewDelegate Delegate { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:EventKitUI.EKEventViewDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:EventKitUI.EKEventViewDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:EventKitUI.EKEventViewDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:EventKitUI.EKEventViewDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IEKEventViewDelegate { }

	/// <summary>A delegate object that provides the application developer fine-grained control over events in the life-cycle of a <see cref="T:EventKitUI.EKEventViewController" /> object.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKEventViewDelegateProtocolRef/index.html">Apple documentation for <c>EKEventViewDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface EKEventViewDelegate {
		[Abstract]
		[Export ("eventViewController:didCompleteWithAction:"), EventArgs ("EKEventView")]
		void Completed (EKEventViewController controller, EKEventViewAction action);
	}

	/// <summary>UIViewController used to create or edit calendar events.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKEventEditViewControllerClassRef/index.html">Apple documentation for <c>EKEventEditViewController</c></related>
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
		IEKEventEditViewDelegate EditViewDelegate { get; set; }

		[Export ("eventStore")]
		EKEventStore EventStore { get; set; }

		[NullAllowed]
		[Export ("event")]
		EKEvent Event { get; set; }

		[Export ("cancelEditing")]
		void CancelEditing ();
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:EventKitUI.EKEventEditViewDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:EventKitUI.EKEventEditViewDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:EventKitUI.EKEventEditViewDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:EventKitUI.EKEventEditViewDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IEKEventEditViewDelegate { }

	/// <summary>Delegate class used to receive notifications from the EKEventEditViewController class.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKEventEditViewDelegateRef/index.html">Apple documentation for <c>EKEventEditViewDelegate</c></related>
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

	/// <summary>A <see cref="T:UIKit.UIViewController" /> that manages the selection of one or more calendars.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKCalendarChooserClassRef/index.html">Apple documentation for <c>EKCalendarChooser</c></related>
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
		}

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		IEKCalendarChooserDelegate Delegate { get; set; }

		[Export ("showsDoneButton")]
		bool ShowsDoneButton { get; set; }

		[Export ("showsCancelButton")]
		bool ShowsCancelButton { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("selectedCalendars", ArgumentSemantic.Copy)]
		NSSet SelectedCalendars { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:EventKitUI.EKCalendarChooserDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:EventKitUI.EKCalendarChooserDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:EventKitUI.EKCalendarChooserDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:EventKitUI.EKCalendarChooserDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IEKCalendarChooserDelegate { }

	/// <summary>A delegate object that provides the application developer fine-grained control over events relating to the lifecycle of a <see cref="T:EventKitUI.EKCalendarChooser" /> object.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/EventKitUI/Reference/EKCalendarChooserDelegateProtocolRef/index.html">Apple documentation for <c>EKCalendarChooserDelegate</c></related>
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
