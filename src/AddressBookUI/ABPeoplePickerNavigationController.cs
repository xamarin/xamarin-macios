// 
// ABPeoplePickerNavigationController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using AddressBook;
using UIKit;

namespace AddressBookUI {
	/// <summary>Provides data for the <see cref="AddressBookUI.ABPeoplePickerNavigationController.SelectPerson" /> event.</summary>
	///     <remarks>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/MonoCatalog-MonoDevelop/">monocatalog</related>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABPeoplePickerSelectPersonEventArgs : EventArgs {

		/// <param name="person">To be added.</param>
		///         <summary>Initializes a new instance of the ABPeoplePickerSelectPersonEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABPeoplePickerSelectPersonEventArgs (ABPerson person)
		{
			Person = person;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPerson Person { get; private set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Continue { get; set; }
	}

	/// <summary>Provides data for the <see cref="AddressBookUI.ABPeoplePickerNavigationController.PerformAction" /> event.</summary>
	///     <remarks>
	///     </remarks>
	///     <related type="sample" href="https://github.com/xamarin/ios-samples/tree/master/MonoCatalog-MonoDevelop/">monocatalog</related>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABPeoplePickerPerformActionEventArgs : ABPeoplePickerSelectPersonEventArgs {

		/// <param name="person">To be added.</param>
		///         <param name="property">To be added.</param>
		///         <param name="identifier">To be added.</param>
		///         <summary>Initializes a new instance of the ABPeoplePickerPerformActionEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABPeoplePickerPerformActionEventArgs (ABPerson person, ABPersonProperty property, int? identifier)
			: base (person)
		{
			Property = property;
			Identifier = identifier;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPersonProperty Property { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? Identifier { get; private set; }
	}

	/// <summary>Provides data for the <see cref="AddressBookUI.ABPeoplePickerNavigationController.SelectPerson2" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABPeoplePickerSelectPerson2EventArgs : EventArgs {

		/// <param name="person">To be added.</param>
		///         <summary>Initializes a new instance of the ABPeoplePickerSelectPerson2EventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABPeoplePickerSelectPerson2EventArgs (ABPerson person)
		{
			Person = person;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPerson Person { get; private set; }
	}

	/// <summary>Provides data for the <see cref="AddressBookUI.ABPeoplePickerNavigationController.PerformAction2" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABPeoplePickerPerformAction2EventArgs : ABPeoplePickerSelectPerson2EventArgs {

		/// <param name="person">To be added.</param>
		///         <param name="property">To be added.</param>
		///         <param name="identifier">To be added.</param>
		///         <summary>Initializes a new instance of the ABPeoplePickerPerformAction2EventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABPeoplePickerPerformAction2EventArgs (ABPerson person, ABPersonProperty property, int? identifier)
			: base (person)
		{
			Property = property;
			Identifier = identifier;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPersonProperty Property { get; private set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? Identifier { get; private set; }
	}

	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	class InternalABPeoplePickerNavigationControllerDelegate : ABPeoplePickerNavigationControllerDelegate {
		internal EventHandler<ABPeoplePickerSelectPersonEventArgs>? selectPerson;
		internal EventHandler<ABPeoplePickerPerformActionEventArgs>? performAction;
		internal EventHandler<ABPeoplePickerSelectPerson2EventArgs>? selectPerson2;
		internal EventHandler<ABPeoplePickerPerformAction2EventArgs>? performAction2;
		internal EventHandler? cancelled;

		[Preserve (Conditional = true)]
		public override bool RespondsToSelector (Selector? sel)
		{
			switch (sel?.Name) {
			case "peoplePickerNavigationController:shouldContinueAfterSelectingPerson:":
				return (selectPerson is not null);
			case "peoplePickerNavigationController:shouldContinueAfterSelectingPerson:property:identifier:":
				return (performAction is not null);
			case "peoplePickerNavigationController:didSelectPerson:":
				return (selectPerson2 is not null);
			case "peoplePickerNavigationController:didSelectPerson:property:identifier:":
				return (performAction2 is not null);
			}
			return base.RespondsToSelector (sel);
		}

		public InternalABPeoplePickerNavigationControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void DidSelectPerson (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
		{
			var e = new ABPeoplePickerSelectPerson2EventArgs (selectedPerson);
			peoplePicker.OnSelectPerson2 (e);
		}

		[Preserve (Conditional = true)]
		public override bool ShouldContinue (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
		{
			var e = new ABPeoplePickerSelectPersonEventArgs (selectedPerson);
			peoplePicker.OnSelectPerson (e);
			return e.Continue;
		}

		[Preserve (Conditional = true)]
		public override bool ShouldContinue (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson, int propertyId, int identifier)
		{
			ABPersonProperty property = ABPersonPropertyId.ToPersonProperty (propertyId);
			int? id = identifier == ABRecord.InvalidPropertyId ? null : (int?) identifier;

			var e = new ABPeoplePickerPerformActionEventArgs (selectedPerson, property, id);
			peoplePicker.OnPerformAction (e);
			return e.Continue;
		}

		[Preserve (Conditional = true)]
		public override void DidSelectPerson (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson, int propertyId, int identifier)
		{
			ABPersonProperty property = ABPersonPropertyId.ToPersonProperty (propertyId);
			int? id = identifier == ABRecord.InvalidPropertyId ? null : (int?) identifier;

			var e = new ABPeoplePickerPerformAction2EventArgs (selectedPerson, property, id);
			peoplePicker.OnPerformAction2 (e);
		}

		[Preserve (Conditional = true)]
		public override void Cancelled (ABPeoplePickerNavigationController peoplePicker)
		{
			peoplePicker.OnCancelled (EventArgs.Empty);
		}
	}


	partial class ABPeoplePickerNavigationController {

		DisplayedPropertiesCollection? displayedProperties;
		/// <summary>Gets the list of properties that the <see cref="AddressBookUI.ABPeoplePickerNavigationController" /> displays.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public DisplayedPropertiesCollection? DisplayedProperties {
			get {
				if (displayedProperties is null) {
					displayedProperties = new DisplayedPropertiesCollection (
						() => _DisplayedProperties,
						v => _DisplayedProperties = v);
					MarkDirty ();
				}
				return displayedProperties;
			}
		}

		ABAddressBook? addressBook;
		/// <summary>Gets or sets the <see cref="AddressBook.ABAddressBook" /> that contains the list of contacts.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABAddressBook? AddressBook {
			get {
				MarkDirty ();
				return BackingField.Get (ref addressBook, _AddressBook, h => new ABAddressBook (h, false));
			}
			set {
				_AddressBook = BackingField.Save (ref addressBook, value);
				MarkDirty ();
			}
		}

		T EnsureEventDelegate<T> () where T : NSObject, new()
		{
			var d = WeakDelegate is null ? null : (T) WeakDelegate;
			if (d is null) {
				d = new T ();
				WeakDelegate = d;
			}
			return d;
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnSelectPerson (ABPeoplePickerSelectPersonEventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson;
			if (h is not null)
				h (this, e);
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnSelectPerson2 (ABPeoplePickerSelectPerson2EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2;
			if (h is not null)
				h (this, e);
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnPerformAction (ABPeoplePickerPerformActionEventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction;
			if (h is not null)
				h (this, e);
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnPerformAction2 (ABPeoplePickerPerformAction2EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2;
			if (h is not null)
				h (this, e);
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnCancelled (EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().cancelled;
			if (h is not null)
				h (this, e);
		}

		/// <summary>Prior to iOS 8, this event handler was called when the user selected a contact.</summary>
		/// <remarks>
		///           <para>In iOS 8 and above, in addition to subscribing to this event, also subscribe to <see cref="AddressBookUI.ABPeoplePickerNavigationController.SelectPerson2" />.</para>
		///           <para>Set <see cref="AddressBookUI.ABPeoplePickerSelectPersonEventArgs.Continue" /> to <see langword="true" /> to display the contact and dismiss the picker.</para>
		///           <para>Set <see cref="AddressBookUI.ABPeoplePickerSelectPersonEventArgs.Continue" /> to <see langword="false" /> to do nothing.</para>
		///         </remarks>
		public event EventHandler<ABPeoplePickerSelectPersonEventArgs> SelectPerson {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson -= value;
			}
		}

		/// <summary>In iOS8 and above, this event handler is called after a person has been selected by the user.</summary>
		/// <remarks>To be added.</remarks>
		public event EventHandler<ABPeoplePickerSelectPerson2EventArgs> SelectPerson2 {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2 += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2 -= value;
			}
		}

		/// <summary>Prior to iOS 8, this event handler was called when the user selected one of the person’s properties.</summary>
		/// <remarks>
		///           <para>In addition to subscribing to this event, also subscribe to <see cref="AddressBookUI.ABPeoplePickerNavigationController.PerformAction2" /> in iOS 8 and above.
		///           </para>
		///         </remarks>
		public event EventHandler<ABPeoplePickerPerformActionEventArgs> PerformAction {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction -= value;
			}
		}

		/// <summary>In iOS8 and above, this event handler will be called after a person has been selected by the user.</summary>
		/// <remarks>To be added.</remarks>
		public event EventHandler<ABPeoplePickerPerformAction2EventArgs> PerformAction2 {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2 += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2 -= value;
			}
		}

		/// <summary>iOS will call event handler when the user taps Cancel.</summary>
		/// <remarks>
		///           <para>If the developer does not subscribe to this event, the people picker will dismiss itself when the user taps cancel.</para>
		///           <para />
		///           <para>Note: Prior to iOS 8, the event handler was responsible for dismissing the people picker.</para>
		///         </remarks>
		public event EventHandler Cancelled {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().cancelled += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().cancelled -= value;
			}
		}
	}
}
