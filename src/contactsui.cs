//
// Contacts bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;
using Contacts;
using CoreGraphics;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

namespace ContactsUI {

#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof (UIViewController))]
	interface CNContactPickerViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		ICNContactPickerDelegate Delegate { get; set; }

		[NullAllowed]
		[Export ("predicateForEnablingContact", ArgumentSemantic.Copy)]
		NSPredicate PredicateForEnablingContact { get; set; }

		[NullAllowed]
		[Export ("predicateForSelectionOfContact", ArgumentSemantic.Copy)]
		NSPredicate PredicateForSelectionOfContact { get; set; }

		[NullAllowed]
		[Export ("predicateForSelectionOfProperty", ArgumentSemantic.Copy)]
		NSPredicate PredicateForSelectionOfProperty { get; set; }
	}
#endif

	interface ICNContactPickerDelegate {}

#if MONOMAC
	[Mac (10,11, onlyOn64: true)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface CNContactPickerDelegate
	{
		[Export ("contactPicker:didSelectContact:")]
		void ContactSelected (CNContactPicker picker, CNContact contact);

		[Export ("contactPicker:didSelectContactProperty:")]
		void ContactPropertySelected (CNContactPicker picker, CNContactProperty contactProperty);

		[Export ("contactPickerWillClose:")]
		void WillClose (CNContactPicker picker);

		[Export ("contactPickerDidClose:")]
		void DidClose (CNContactPicker picker);
	}
#else
	[iOS (9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CNContactPickerDelegate {

		[Export ("contactPickerDidCancel:")]
		void ContactPickerDidCancel (CNContactPickerViewController picker);

		[Export ("contactPicker:didSelectContact:")]
		void DidSelectContact (CNContactPickerViewController picker, CNContact contact);

		[Export ("contactPicker:didSelectContactProperty:")]
		void DidSelectContactProperty (CNContactPickerViewController picker, CNContactProperty contactProperty);

		[Export ("contactPicker:didSelectContacts:")]
		void DidSelectContacts (CNContactPickerViewController picker, CNContact [] contacts);

		[Export ("contactPicker:didSelectContactProperties:")]
		void DidSelectContactProperties (CNContactPickerViewController picker, CNContactProperty [] contactProperties);
	}
#endif // MONOMAC

#if MONOMAC
	[Mac (10,11, onlyOn64: true)]
	[BaseType (typeof(NSViewController))]
	interface CNContactViewController
	{
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor DescriptorForRequiredKeys { get; }

		[NullAllowed, Export ("contact", ArgumentSemantic.Copy)]
		CNContact Contact { get; set; }
	}
#else
	[iOS (9,0)]
	[BaseType (typeof (UIViewController))]
	interface CNContactViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor DescriptorForRequiredKeys { get; }

		[Static]
		[Export ("viewControllerForContact:")]
		CNContactViewController FromContact (CNContact contact);

		[Static]
		[Export ("viewControllerForUnknownContact:")]
		CNContactViewController FromUnknownContact (CNContact contact);

		[Static]
		[Export ("viewControllerForNewContact:")]
		CNContactViewController FromNewContact ([NullAllowed] CNContact contact);

		[Export ("contact", ArgumentSemantic.Strong)]
		CNContact Contact { get; }

		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		ICNContactViewControllerDelegate Delegate { get; set; }

		[NullAllowed]
		[Export ("contactStore", ArgumentSemantic.Strong)]
		CNContactStore ContactStore { get; set; }

		[NullAllowed]
		[Export ("parentGroup", ArgumentSemantic.Strong)]
		CNGroup ParentGroup { get; set; }

		[NullAllowed]
		[Export ("parentContainer", ArgumentSemantic.Strong)]
		CNContainer ParentContainer { get; set; }

		[NullAllowed]
		[Export ("alternateName")]
		string AlternateName { get; set; }

		[NullAllowed]
		[Export ("message")]
		string Message { get; set; }

		[Export ("allowsEditing", ArgumentSemantic.Assign)]
		bool AllowsEditing { get; set; }

		[Export ("allowsActions", ArgumentSemantic.Assign)]
		bool AllowsActions { get; set; }

		[Export ("shouldShowLinkedContacts", ArgumentSemantic.Assign)]
		bool ShouldShowLinkedContacts { get; set; }

		[Export ("highlightPropertyWithKey:identifier:")] //TODO: Maybe we can mNullallowedake a strongly type version
		void HighlightProperty (NSString key, [NullAllowed] string identifier);
	}
#endif

	interface ICNContactViewControllerDelegate {}

	[iOS (9,0)]
	[NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CNContactViewControllerDelegate {

		[Export ("contactViewController:shouldPerformDefaultActionForContactProperty:")]
		bool ShouldPerformDefaultAction (CNContactViewController viewController, CNContactProperty property);

		[Export ("contactViewController:didCompleteWithContact:")]
		void DidComplete (CNContactViewController viewController, [NullAllowed] CNContact contact);
	}

#if MONOMAC
	[Mac (10,11, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface CNContactPicker
	{
		[Export ("displayedKeys", ArgumentSemantic.Copy)]
		string[] DisplayedKeys { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		[Protocolize]
		CNContactPickerDelegate Delegate { get; set; }

		[Export ("showRelativeToRect:ofView:preferredEdge:")]
		void Show (CGRect positioningRect, NSView positioningView, NSRectEdge preferredEdge);

		[Export ("close")]
		void Close ();
	}
#endif // MONOMAC
#endif // XAMCORE_2_0
}

