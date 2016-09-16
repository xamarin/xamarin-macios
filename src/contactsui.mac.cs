using System;
using System.ComponentModel;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.Contacts;
using XamCore.AppKit;
using XamCore.CoreGraphics;

namespace XamCore.ContactsUI {
#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
	[Mac (10,11, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
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

	interface ICNContactPickerDelegate { }

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

	[Mac (10,11, onlyOn64: true)]
	[BaseType (typeof(NSViewController))]
	interface CNContactViewController
	{
		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor DescriptorForRequiredKeys { get; }

		[NullAllowed, Export ("contact", ArgumentSemantic.Copy)]
		CNContact Contact { get; set; }
	}
#endif
}