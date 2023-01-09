// 
// ABPeoplePickerNavigationController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using System;

using AddressBook;
using Foundation;
using UIKit;
using ObjCRuntime;

namespace AddressBookUI {
#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPeoplePickerSelectPersonEventArgs : EventArgs {

		public ABPeoplePickerSelectPersonEventArgs (ABPerson person)
		{
			Person = person;
		}

		public ABPerson Person { get; private set; }

		public bool Continue { get; set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPeoplePickerPerformActionEventArgs : ABPeoplePickerSelectPersonEventArgs {

		public ABPeoplePickerPerformActionEventArgs (ABPerson person, ABPersonProperty property, int? identifier)
			: base (person)
		{
			Property = property;
			Identifier = identifier;
		}

		public ABPersonProperty Property { get; private set; }
		public int? Identifier { get; private set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPeoplePickerSelectPerson2EventArgs : EventArgs {

		public ABPeoplePickerSelectPerson2EventArgs (ABPerson person)
		{
			Person = person;
		}

		public ABPerson Person { get; private set; }
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPeoplePickerPerformAction2EventArgs : ABPeoplePickerSelectPerson2EventArgs {

		public ABPeoplePickerPerformAction2EventArgs (ABPerson person, ABPersonProperty property, int? identifier)
			: base (person)
		{
			Property = property;
			Identifier = identifier;
		}

		public ABPersonProperty Property { get; private set; }
		public int? Identifier { get; private set; }
	}

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

		protected internal virtual void OnSelectPerson (ABPeoplePickerSelectPersonEventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson;
			if (h is not null)
				h (this, e);
		}

		protected internal virtual void OnSelectPerson2 (ABPeoplePickerSelectPerson2EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2;
			if (h is not null)
				h (this, e);
		}

		protected internal virtual void OnPerformAction (ABPeoplePickerPerformActionEventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction;
			if (h is not null)
				h (this, e);
		}

		protected internal virtual void OnPerformAction2 (ABPeoplePickerPerformAction2EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2;
			if (h is not null)
				h (this, e);
		}

		protected internal virtual void OnCancelled (EventArgs e)
		{
			var h = EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().cancelled;
			if (h is not null)
				h (this, e);
		}

		public event EventHandler<ABPeoplePickerSelectPersonEventArgs> SelectPerson {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson -= value;
			}
		}

		public event EventHandler<ABPeoplePickerSelectPerson2EventArgs> SelectPerson2 {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2 += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().selectPerson2 -= value;
			}
		}

		public event EventHandler<ABPeoplePickerPerformActionEventArgs> PerformAction {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction -= value;
			}
		}

		public event EventHandler<ABPeoplePickerPerformAction2EventArgs> PerformAction2 {
			add {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2 += value;
			}
			remove {
				EnsureEventDelegate<InternalABPeoplePickerNavigationControllerDelegate> ().performAction2 -= value;
			}
		}

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
