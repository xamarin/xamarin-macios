//
// Contacts bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using Contacts;
using CoreGraphics;
#if MONOMAC
using AppKit;
using XViewController = AppKit.NSViewController;
#else
using UIKit;
using NSView = UIKit.UIView;
using NSRectEdge = Foundation.NSObject;
using XViewController = UIKit.UIViewController;
#endif

namespace ContactsUI {

#if !MONOMAC
	/// <summary>A standard <see cref="UIKit.UIViewController" /> that allows the user to select a <see cref="Contacts.CNContact" /> from a list of contacts.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ContactsUI/Reference/CNContactPickerViewController_Class/index.html">Apple documentation for <c>CNContactPickerViewController</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface CNContactPickerViewController {
		/// <param name="nibName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="bundle">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new <see cref="ContactsUI.CNContactPickerViewController" /> from the specified <paramref name="nibName" /> in the specified <paramref name="bundle" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		/// <summary>The names of the displayed keys (see <see cref="Contacts.CNContactKey" />).</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		/// <summary>An instance of the ContactsUI.ICNContactPickerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the ContactsUI.ICNContactPickerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		ICNContactPickerDelegate Delegate { get; set; }

		/// <summary>If not <see langword="null" />, only contacts that satisfy this <see cref="Foundation.NSPredicate" /> will be selectable by the user.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("predicateForEnablingContact", ArgumentSemantic.Copy)]
		NSPredicate PredicateForEnablingContact { get; set; }

		/// <summary>If not <see langword="null" />, if this <see cref="Foundation.NSPredicate" /> returns <see langword="true" />, the <see cref="Contacts.CNContact" /> will be returned on selection. Otherwise, the default action for the selected contact will be taken.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("predicateForSelectionOfContact", ArgumentSemantic.Copy)]
		NSPredicate PredicateForSelectionOfContact { get; set; }

		/// <summary>If not <see langword="null" />, if this <see cref="Foundation.NSPredicate" /> returns <see langword="true" />, the <see cref="Contacts.CNContact" /> will be returned on selection. Otherwise, the default action for the selected property will be taken.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("predicateForSelectionOfProperty", ArgumentSemantic.Copy)]
		NSPredicate PredicateForSelectionOfProperty { get; set; }
	}
#endif

	interface ICNContactPickerDelegate { }

	/// <summary>Delegate object that provides methods relating to picking a contact.</summary>
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	[MacCatalyst (13, 1)]
	interface CNContactPickerDelegate {
#if MONOMAC
		/// <param name="picker">To be added.</param>
		///         <param name="contact">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("contactPicker:didSelectContact:")]
		void ContactSelected (CNContactPicker picker, CNContact contact);

		/// <param name="picker">To be added.</param>
		///         <param name="contactProperty">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("contactPicker:didSelectContactProperty:")]
		void ContactPropertySelected (CNContactPicker picker, CNContactProperty contactProperty);

		/// <param name="picker">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("contactPickerWillClose:")]
		void WillClose (CNContactPicker picker);

		/// <param name="picker">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("contactPickerDidClose:")]
		void DidClose (CNContactPicker picker);
#else
		/// <param name="picker">To be added.</param>
		///         <summary>Called after the user selects the "Cancel" button.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[Export ("contactPickerDidCancel:")]
		void ContactPickerDidCancel (CNContactPickerViewController picker);

		/// <param name="picker">To be added.</param>
		///         <param name="contact">To be added.</param>
		///         <summary>Called after the user selects the <paramref name="contact" />.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[Export ("contactPicker:didSelectContact:")]
		void DidSelectContact (CNContactPickerViewController picker, CNContact contact);

		/// <param name="picker">To be added.</param>
		///         <param name="contactProperty">To be added.</param>
		///         <summary>Called after the user selects a property of the contact.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[Export ("contactPicker:didSelectContactProperty:")]
		void DidSelectContactProperty (CNContactPickerViewController picker, CNContactProperty contactProperty);

		/// <param name="picker">To be added.</param>
		///         <param name="contacts">To be added.</param>
		///         <summary>Called after the user selects multiple contacts. Devs must override this method to configure the <see cref="ContactsUI.CNContactPickerViewController" /> for multiple selection.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[Export ("contactPicker:didSelectContacts:")]
		void DidSelectContacts (CNContactPickerViewController picker, CNContact [] contacts);

		/// <param name="picker">To be added.</param>
		///         <param name="contactProperties">To be added.</param>
		///         <summary>Called after the user selects multiple properties. Devs must override this method to configure the <see cref="ContactsUI.CNContactPickerViewController" /> for multiple selection.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[Export ("contactPicker:didSelectContactProperties:")]
		void DidSelectContactProperties (CNContactPickerViewController picker, CNContactProperty [] contactProperties);
#endif // MONOMAC
	}

	/// <summary>A standard <see cref="XViewController" /> that allows the user to view or edit a <see cref="Contacts.CNContact" />.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ContactsUI/Reference/CNContactViewController_Class/index.html">Apple documentation for <c>CNContactViewController</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (XViewController))]
	interface CNContactViewController {
		/// <param name="nibName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="bundle">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new <see cref="ContactsUI.CNContactViewController" /> from the specified <paramref name="nibName" /> in the specified <paramref name="bundle" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithNibName:bundle:")]
#if !MONOMAC
		[PostGet ("NibBundle")]
#endif
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		/// <summary>Describes all the keys that must be fetched for display in a <see cref="ContactsUI.CNContactViewController" />. For use with <see cref="Contacts.CNContactFetchRequest.KeysToFetch" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("descriptorForRequiredKeys")]
		ICNKeyDescriptor DescriptorForRequiredKeys { get; }

#if MONOMAC
		/// <summary>The <see cref="Contacts.CNContact" /> that is displayed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contact", ArgumentSemantic.Copy)]
#else
		[Export ("contact", ArgumentSemantic.Strong)]
#endif
		CNContact Contact {
			get;
			[NoiOS]
			[NoTV]
			[NoMacCatalyst]
			set;
		}

		/// <param name="contact">To be added.</param>
		///         <summary>Creates a <see cref="ContactsUI.CNContactViewController" /> to display <paramref name="contact" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForContact:")]
		CNContactViewController FromContact (CNContact contact);

		/// <param name="contact">To be added.</param>
		///         <summary>Creates a <see cref="ContactsUI.CNContactViewController" /> to display <paramref name="contact" /> when it is not known if <paramref name="contact" /> was fetched or newly created.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForUnknownContact:")]
		CNContactViewController FromUnknownContact (CNContact contact);

		/// <param name="contact">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Creates a <see cref="ContactsUI.CNContactViewController" /> to display the newly-created <paramref name="contact" />.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("viewControllerForNewContact:")]
		CNContactViewController FromNewContact ([NullAllowed] CNContact contact);

		/// <summary>The property keys to be displayed. (see <see cref="Contacts.CNContactKey" />)</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed] // TODO: Maybe we can Strongify this puppy
		[Export ("displayedPropertyKeys")]
		NSString [] DisplayedPropertyKeys { get; set; }

		/// <summary>An instance of the ContactsUI.ICNContactViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the ContactsUI.ICNContactViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		ICNContactViewControllerDelegate Delegate { get; set; }

		/// <summary>The <see cref="Contacts.CNContactStore" /> from which the <see cref="ContactsUI.CNContactViewController.Contact" /> was retrieved and to which it will be saved.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("contactStore", ArgumentSemantic.Strong)]
		CNContactStore ContactStore { get; set; }

		/// <summary>If set, the <see cref="Contacts.CNGroup" /> to which to add the <see cref="ContactsUI.CNContactViewController.Contact" />.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("parentGroup", ArgumentSemantic.Strong)]
		CNGroup ParentGroup { get; set; }

		/// <summary>Gets or sets the parent container.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("parentContainer", ArgumentSemantic.Strong)]
		CNContainer ParentContainer { get; set; }

		/// <summary>The name to be displayed if the <see cref="Contacts.CNContact" /> does not have a name that can be displayed.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>
		///           <para>There is not a "Display Name" property that is checked. Rather, the <see cref="Contacts.CNContact" /> must have a <see langword="null" /><see cref="Contacts.CNContact.GivenName" /> and <see cref="Contacts.CNContact.FamilyName" />.</para>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("alternateName")]
		string AlternateName { get; set; }

		/// <summary>Message displayed below the name.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("message")]
		string Message { get; set; }

		/// <summary>Whether editing the contact is allowed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsEditing", ArgumentSemantic.Assign)]
		bool AllowsEditing { get; set; }

		/// <summary>Whether buttons triggering actions (such as sending a text) are displayed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsActions", ArgumentSemantic.Assign)]
		bool AllowsActions { get; set; }

		/// <summary>Whether to show data from contacts linked to the <see cref="ContactsUI.CNContactViewController.Contact" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("shouldShowLinkedContacts", ArgumentSemantic.Assign)]
		bool ShouldShowLinkedContacts { get; set; }

		/// <param name="key">To be added.</param>
		///         <param name="identifier">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Highlights the property identified by <paramref name="key" />. If <paramref name="key" /> is multivalued, <paramref name="identifier" /> specifies which to highlight. (See <see cref="Contacts.CNContactKey" /> for values for <paramref name="key" />.)</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("highlightPropertyWithKey:identifier:")] //TODO: Maybe we can mNullallowedake a strongly type version
		void HighlightProperty (NSString key, [NullAllowed] string identifier);
	}

	interface ICNContactViewControllerDelegate { }

	/// <summary>Delegate object that provides methods relating to viewing or editing a contact with a <see cref="ContactsUI.CNContactViewController" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ContactsUI/Reference/CNContactViewControllerDelegate_Protocol/index.html">Apple documentation for <c>CNContactViewControllerDelegate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CNContactViewControllerDelegate {

		/// <param name="viewController">To be added.</param>
		///         <param name="property">To be added.</param>
		///         <summary>Return <see langword="true" /> if the default action for the property should be triggered when it is selected by the user.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("contactViewController:shouldPerformDefaultActionForContactProperty:")]
		bool ShouldPerformDefaultAction (CNContactViewController viewController, CNContactProperty property);

		/// <param name="viewController">To be added.</param>
		///         <param name="contact">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Method that is called after the view is presented.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("contactViewController:didCompleteWithContact:")]
		void DidComplete (CNContactViewController viewController, [NullAllowed] CNContact contact);
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CNContactPicker {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("displayedKeys", ArgumentSemantic.Copy)]
		string [] DisplayedKeys { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ICNContactPickerDelegate Delegate { get; set; }

		/// <param name="positioningRect">To be added.</param>
		///         <param name="positioningView">To be added.</param>
		///         <param name="preferredEdge">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("showRelativeToRect:ofView:preferredEdge:")]
		void Show (CGRect positioningRect, NSView positioningView, NSRectEdge preferredEdge);

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("close")]
		void Close ();
	}
}
