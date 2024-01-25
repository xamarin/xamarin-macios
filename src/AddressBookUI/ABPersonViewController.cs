// 
// ABPersonViewController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using System;

using AddressBook;

using Foundation;

using ObjCRuntime;

namespace AddressBookUI {
#if NET
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPersonViewPerformDefaultActionEventArgs : EventArgs {
		public ABPersonViewPerformDefaultActionEventArgs (ABPerson person, ABPersonProperty property, int? identifier)
		{
			Person = person;
			Property = property;
			Identifier = identifier;
		}

		public ABPerson Person { get; private set; }
		public ABPersonProperty Property { get; private set; }
		public int? Identifier { get; private set; }

		public bool ShouldPerformDefaultAction { get; set; }
	}

	class InternalABPersonViewControllerDelegate : ABPersonViewControllerDelegate {

		internal EventHandler<ABPersonViewPerformDefaultActionEventArgs>? performDefaultAction;

		public InternalABPersonViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override bool ShouldPerformDefaultActionForPerson (ABPersonViewController personViewController, ABPerson person, int propertyId, int identifier)
		{
			ABPersonProperty property = ABPersonPropertyId.ToPersonProperty (propertyId);
			int? id = identifier == ABRecord.InvalidPropertyId ? null : (int?) identifier;

			var e = new ABPersonViewPerformDefaultActionEventArgs (person, property, id);
			personViewController.OnPerformDefaultAction (e);
			return e.ShouldPerformDefaultAction;
		}
	}

	partial class ABPersonViewController {

		ABPerson? displayedPerson;
		public ABPerson? DisplayedPerson {
			get {
				MarkDirty ();
				return BackingField.Get (ref displayedPerson, _DisplayedPerson, h => new ABPerson (h, AddressBook));
			}
			set {
				_DisplayedPerson = BackingField.Save (ref displayedPerson, value);
				MarkDirty ();
			}
		}

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

		public void SetHighlightedItemForProperty (ABPersonProperty property, int? identifier)
		{
			SetHighlightedItemForProperty (
					ABPersonPropertyId.ToId (property),
					identifier ?? ABRecord.InvalidPropertyId);
		}

		public void SetHighlightedProperty (ABPersonProperty property)
		{
			SetHighlightedItemForProperty (
					ABPersonPropertyId.ToId (property),
					ABRecord.InvalidPropertyId);
		}

		InternalABPersonViewControllerDelegate EnsureEventDelegate ()
		{
			var d = WeakDelegate as InternalABPersonViewControllerDelegate;
			if (d is null) {
				d = new InternalABPersonViewControllerDelegate ();
				WeakDelegate = d;
			}
			return d;
		}

		protected internal virtual void OnPerformDefaultAction (ABPersonViewPerformDefaultActionEventArgs e)
		{
			var h = EnsureEventDelegate ().performDefaultAction;
			if (h is not null)
				h (this, e);
		}

		public event EventHandler<ABPersonViewPerformDefaultActionEventArgs> PerformDefaultAction {
			add { EnsureEventDelegate ().performDefaultAction += value; }
			remove { EnsureEventDelegate ().performDefaultAction -= value; }
		}
	}
}
