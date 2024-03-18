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
using NSView = UIKit.UIView;
using NSRectEdge = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ContactsUI {

#if !MONOMAC
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface CNContactPickerViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
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

	interface ICNContactPickerDelegate { }

#if MONOMAC
	[NoiOS][NoMacCatalyst][NoTV]
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
	[NoMac]
	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
#if MONOMAC
	[BaseType (typeof (NSViewController))]
#else
	[BaseType (typeof (UIViewController))]
#endif
	interface CNContactViewController {
		[Export ("initWithNibName:bundle:")]
#if !MONOMAC
		[PostGet ("NibBundle")]
#endif
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor DescriptorForRequiredKeys { get; }

#if MONOMAC
		[NullAllowed]
		[Export ("contact", ArgumentSemantic.Copy)]
#else
		[Export ("contact", ArgumentSemantic.Strong)]
#endif
		CNContact Contact {
			get;
			[NoiOS]
			[NoTV]
			[NoWatch]
			[NoMacCatalyst]
			set;
		}

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForContact:")]
		CNContactViewController FromContact (CNContact contact);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForUnknownContact:")]
		CNContactViewController FromUnknownContact (CNContact contact);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForNewContact:")]
		CNContactViewController FromNewContact ([NullAllowed] CNContact contact);

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		ICNContactViewControllerDelegate Delegate { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("contactStore", ArgumentSemantic.Strong)]
		CNContactStore ContactStore { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("parentGroup", ArgumentSemantic.Strong)]
		CNGroup ParentGroup { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("parentContainer", ArgumentSemantic.Strong)]
		CNContainer ParentContainer { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("alternateName")]
		string AlternateName { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("message")]
		string Message { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsEditing", ArgumentSemantic.Assign)]
		bool AllowsEditing { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsActions", ArgumentSemantic.Assign)]
		bool AllowsActions { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("shouldShowLinkedContacts", ArgumentSemantic.Assign)]
		bool ShouldShowLinkedContacts { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("highlightPropertyWithKey:identifier:")] //TODO: Maybe we can mNullallowedake a strongly type version
		void HighlightProperty (NSString key, [NullAllowed] string identifier);
	}

	interface ICNContactViewControllerDelegate { }

	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CNContactViewControllerDelegate {

		[Export ("contactViewController:shouldPerformDefaultActionForContactProperty:")]
		bool ShouldPerformDefaultAction (CNContactViewController viewController, CNContactProperty property);

		[Export ("contactViewController:didCompleteWithContact:")]
		void DidComplete (CNContactViewController viewController, [NullAllowed] CNContact contact);
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CNContactPicker {
		[Export ("displayedKeys", ArgumentSemantic.Copy)]
		string [] DisplayedKeys { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ICNContactPickerDelegate Delegate { get; set; }

		[Export ("showRelativeToRect:ofView:preferredEdge:")]
		void Show (CGRect positioningRect, NSView positioningView, NSRectEdge preferredEdge);

		[Export ("close")]
		void Close ();
	}
}
