// 
// ABUnknownPersonViewController.cs: 
//
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
//

#nullable enable

using AddressBook;

namespace AddressBookUI {
	/// <summary>Provides data for the <see cref="AddressBookUI.ABUnknownPersonViewController.PersonCreated" /> event.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	public class ABUnknownPersonCreatedEventArgs : EventArgs {

		/// <param name="person">To be added.</param>
		///         <summary>Initializes a new instance of the ABUnknownPersonCreatedEventArgs class.</summary>
		///         <remarks>
		///         </remarks>
		public ABUnknownPersonCreatedEventArgs (ABPerson? person)
		{
			Person = person;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ABPerson? Person { get; private set; }
	}

	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("ios", "Use the 'Contacts' API instead.")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("tvos")]
	class InternalABUnknownPersonViewControllerDelegate : ABUnknownPersonViewControllerDelegate {
		internal EventHandler<ABPersonViewPerformDefaultActionEventArgs>? performDefaultAction;
		internal EventHandler<ABUnknownPersonCreatedEventArgs>? personCreated;

		public InternalABUnknownPersonViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void DidResolveToPerson (ABUnknownPersonViewController personViewController, ABPerson? person)
		{
			personViewController.OnPersonCreated (new ABUnknownPersonCreatedEventArgs (person));
		}

		[Preserve (Conditional = true)]
		public override bool ShouldPerformDefaultActionForPerson (ABUnknownPersonViewController personViewController, ABPerson person, int propertyId, int identifier)
		{
			ABPersonProperty property = ABPersonPropertyId.ToPersonProperty (propertyId);
			int? id = identifier == ABRecord.InvalidPropertyId ? null : (int?) identifier;

			var e = new ABPersonViewPerformDefaultActionEventArgs (person, property, id);
			personViewController.OnPerformDefaultAction (e);
			return e.ShouldPerformDefaultAction;
		}
	}

	partial class ABUnknownPersonViewController {

		ABPerson? displayedPerson;
		/// <summary>Gets or sets the <see cref="AddressBook.ABPerson" /> whose data is being displayed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		ABAddressBook? addressBook;
		/// <summary>Gets or sets the <see cref="AddressBook.ABAddressBook" /> to which the controller will save data.</summary>
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

		InternalABUnknownPersonViewControllerDelegate EnsureEventDelegate ()
		{
			var d = WeakDelegate as InternalABUnknownPersonViewControllerDelegate;
			if (d is null) {
				d = new InternalABUnknownPersonViewControllerDelegate ();
				WeakDelegate = d;
			}
			return d;
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnPerformDefaultAction (ABPersonViewPerformDefaultActionEventArgs e)
		{
			var h = EnsureEventDelegate ().performDefaultAction;
			if (h is not null)
				h (this, e);
		}

		/// <param name="e">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		protected internal virtual void OnPersonCreated (ABUnknownPersonCreatedEventArgs e)
		{
			var h = EnsureEventDelegate ().personCreated;
			if (h is not null)
				h (this, e);
		}

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		public event EventHandler<ABPersonViewPerformDefaultActionEventArgs> PerformDefaultAction {
			add { EnsureEventDelegate ().performDefaultAction += value; }
			remove { EnsureEventDelegate ().performDefaultAction -= value; }
		}

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		public event EventHandler<ABUnknownPersonCreatedEventArgs> PersonCreated {
			add { EnsureEventDelegate ().personCreated += value; }
			remove { EnsureEventDelegate ().personCreated -= value; }
		}
	}
}
