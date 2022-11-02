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
	[iOS (9,0)][NoMac]
	[BaseType (typeof (UIViewController))]
	interface CNContactPickerViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

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
	[NoiOS][NoMacCatalyst][NoTV]
	[Mac (10,11)]
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

	[Mac (10,11)]
	[iOS (9,0)]
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
		[NullAllowed, Export ("contact", ArgumentSemantic.Copy)]
		CNContact Contact { get; [NoiOS]set; }
#else
		[Export ("contact", ArgumentSemantic.Strong)]
		CNContact Contact { get; }
#endif

		[NoMac]
		[Static]
		[Export ("viewControllerForContact:")]
		CNContactViewController FromContact (CNContact contact);

		[NoMac]
		[Static]
		[Export ("viewControllerForUnknownContact:")]
		CNContactViewController FromUnknownContact (CNContact contact);

		[NoMac]
		[Static]
		[Export ("viewControllerForNewContact:")]
		CNContactViewController FromNewContact ([NullAllowed] CNContact contact);

		[NoMac]
		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		[NoMac]
		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		ICNContactViewControllerDelegate Delegate { get; set; }

		[NoMac]
		[NullAllowed]
		[Export ("contactStore", ArgumentSemantic.Strong)]
		CNContactStore ContactStore { get; set; }

		[NoMac]
		[NullAllowed]
		[Export ("parentGroup", ArgumentSemantic.Strong)]
		CNGroup ParentGroup { get; set; }

		[NoMac]
		[NullAllowed]
		[Export ("parentContainer", ArgumentSemantic.Strong)]
		CNContainer ParentContainer { get; set; }

		[NoMac]
		[NullAllowed]
		[Export ("alternateName")]
		string AlternateName { get; set; }

		[NoMac]
		[NullAllowed]
		[Export ("message")]
		string Message { get; set; }

		[NoMac]
		[Export ("allowsEditing", ArgumentSemantic.Assign)]
		bool AllowsEditing { get; set; }

		[NoMac]
		[Export ("allowsActions", ArgumentSemantic.Assign)]
		bool AllowsActions { get; set; }

		[NoMac]
		[Export ("shouldShowLinkedContacts", ArgumentSemantic.Assign)]
		bool ShouldShowLinkedContacts { get; set; }

		[NoMac]
		[Export ("highlightPropertyWithKey:identifier:")] //TODO: Maybe we can mNullallowedake a strongly type version
		void HighlightProperty (NSString key, [NullAllowed] string identifier);
	}

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

	[Mac (10,11)]
	[NoiOS][NoTV][NoMacCatalyst]
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
}
